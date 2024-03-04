using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    private Rigidbody rbody;

    public bool isLaunched;

    public float density = 7750f; //kg/m^3
    public float dragCoefficient = 0.1f;
    public float volume = 0;
    public float initialVel = 70f; // m/s
    private float force;

    public float r;
    float p = 1.225f; //density of air 1.225kg/m^3
    public float area;
    private int target;
    Vector3 vel;

    public GameObject targetWindow;
    public Precise_Window targetWindowPrecision;
    public TrailRenderer trailRenderer;

    [Header("CalcProbability")]
    public float distanceFromCenter = 0f;
    public float angleOfImpact = 0f;
    public float impactSpeed = 0f;
    public float impactForce = 0f;
    public float pressureApplied = 0f;

    Culprit parentShooter;
    void Awake()
    {
        rbody = GetComponent<Rigidbody>();
        trailRenderer = GetComponent<TrailRenderer>();
        parentShooter = transform.parent.GetComponent<Culprit>();

        isLaunched = false;

        dragCoefficient = DontDestroyOnLoadSettings.Instance.dragCoefficient;
        density = DontDestroyOnLoadSettings.Instance.density;
        initialVel = DontDestroyOnLoadSettings.Instance.minVelocity;
        transform.localScale = new Vector3(DontDestroyOnLoadSettings.Instance.diameter
                                        , DontDestroyOnLoadSettings.Instance.diameter
                                        , DontDestroyOnLoadSettings.Instance.diameter);
        r = transform.localScale.y * 0.5f;

        volume = (4 * Mathf.PI * r * r * r) / 3;

        rbody.mass = (density * volume); // in kilograms

        area = Mathf.PI * Mathf.Pow(r,2);

        force = rbody.mass * initialVel;

        rbody.isKinematic = true;
    }

    public void NewVel(int _vel)
    {
        initialVel = _vel;
        force = rbody.mass * initialVel;
    }

    public void Shoot(Precise_Window PreciseTarget, int targetIndex)
    {
        GameObject target = PreciseTarget.WindowGO;
        targetWindowPrecision = PreciseTarget;
        transform.position = parentShooter.ShootPosition.position;
        trailRenderer.Clear();
        rbody.isKinematic = false;
        isLaunched = true;
        targetWindow = target;
        this.target = targetIndex;
        // Assuming that 'transform.forward' represents the direction the ball is facing.
        Vector3 facingDirection = parentShooter.ShootPosition.forward;

        // Calculate the angle between the forward vector and the upward direction.
        float angle = Mathf.Atan2(facingDirection.y, facingDirection.z) * Mathf.Rad2Deg;

        //float yComponent = Mathf.Cos(angle * Mathf.Deg2Rad) * force;
        //float zComponent = Mathf.Sin(angle * Mathf.Deg2Rad) * force;
        //Vector3 forceApplied = new Vector3(0, yComponent, zComponent);

        rbody.AddForce(facingDirection * force, ForceMode.Impulse);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!IsGrounded() && !rbody.isKinematic)
        {
            // Debug.Log(rbody.velocity.magnitude);
            SimulateInRealTime(Time.deltaTime);
            vel = rbody.velocity;
        }
        else
        {
            //rbody.isKinematic = true;
            //parentShooter.travelling = false;
            //parentShooter.shootNext = true;
        }
    }

    bool IsGrounded()
    {
        return (transform.position.y < 0);
    }

    void SimulateInRealTime(float dt)
    {
        //Vector3 direction = -rbody.velocity.normalized;
        //float velocity = rbody.velocity.magnitude;
        //var forceAmount = (p * velocity * velocity * dragCoefficient * area) * 0.5f;
        //rbody.AddForce(direction * forceAmount);



        Vector3 dragForce = -0.5f * (p * rbody.velocity.sqrMagnitude * dragCoefficient * area * rbody.velocity.normalized);
        rbody.AddForce(dragForce, ForceMode.Force);

        // Calculate the relative velocity of the ball with respect to the wind
        //Vector3 relativeVelocity = rbody.velocity - SettingsMenu.instance.GetWindDirection() * SettingsMenu.instance.GetWindSpeed();
        //Vector3 dragForce = -0.5f * p * dragCoefficient * area * relativeVelocity.sqrMagnitude * relativeVelocity.normalized;
        //rbody.AddForce(dragForce, ForceMode.Force);
    }

    public void Simulate()
    {

    }

    public void setDragCoefficient(float newValue)
    {
        dragCoefficient = newValue;
    }

    //public void SetTarget(int tar)
    //{
    //    target = tar;
    //    if (target==0)
    //    {
    //        TrailRenderer window_1 = gameObject.GetComponent<TrailRenderer>();
    //        window_1.material = window1;
    //    }
    //    else
    //    {
    //        TrailRenderer window_2 = gameObject.GetComponent<TrailRenderer>();
    //        window_2.material = window2;
    //    }
    //}
    private void OnCollisionEnter(Collision other)
    {
        rbody.isKinematic = true;
        parentShooter.travelling = false;
        parentShooter.shootNext = true;

        ContactPoint contact = other.GetContact(0);
        Vector3 normal = contact.normal;
        transform.position = contact.point;


        if (other.transform.gameObject != targetWindow)
        {
            return;
        }

        if (targetWindowPrecision.PrecisionMarker != null 
            && Vector3.Distance(other.contacts[0].point, targetWindowPrecision.PrecisionMarker.transform.position) > 0.5f * targetWindowPrecision.PrecisionMarker.transform.localScale.x 
            )
        {
            return;
        }


        parentShooter.windowHit[target] = true;

        // values to calc accuracy
        distanceFromCenter = Vector3.Distance(transform.position, targetWindowPrecision.PrecisionMarker.transform.position);
        angleOfImpact = Vector3.Angle(vel, -normal);
        impactSpeed = vel.magnitude;

        // impact in newtons, kinetic energy f = 0.5 * m *v^2 / distance
        impactForce = other.impulse.magnitude;

        // calculate contact area (for spheres only)
        float contactArea = CalculateContactArea(angleOfImpact);

        // calculate pressure (mPa)
        pressureApplied = impactForce / area;

        if(pressureApplied > targetWindowPrecision.breakingStress)
        {
            Culprit.OnHit?.Invoke(gameObject, target);
        }
        else
        {
            parentShooter.finishedCurrent = true;
        }
    }

    float CalculateContactArea(float angle)
    {
        // Adjust contact area calculation based on impact angle

        if (angle < Mathf.Epsilon)  // Consider a normal impact
        {
            return Mathf.PI * Mathf.Pow(r, 2);  // Circular contact area
        }
        else  // Consider an oblique impact
        {
            // Calculate semi-major and semi-minor axes of the ellipse
            float majorAxis = r / Mathf.Cos(angle * Mathf.Deg2Rad);
            float minorAxis = r;

            // Calculate elliptical contact area
            return Mathf.PI * majorAxis * minorAxis;
        }
    }
}
