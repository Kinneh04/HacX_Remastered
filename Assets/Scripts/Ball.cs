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
    public float COR = 0.85f; // coefficent of restitution (0-1)
    public float bounceForce;
    private float force;

    public float r;
    float p = 1.225f; //density of air 1.225kg/m^3
    public float area;
    private int target;
    Vector3 vel;

    public GameObject targetWindow;
    public Precise_Window targetWindowPrecision;
    public TrailRenderer trailRenderer;

    public bool hitFirstPoint = false;

    public float initDist = 0;
    public float currDist = 0;
    Vector2 culpritxz = Vector2.zero;
    Vector2 ricochetxz = Vector2.zero;
    bool stop = false;
    ContactPoint contact;
    public bool forceLower = false;
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
        COR = DontDestroyOnLoadSettings.Instance.coefficientOfRestitution;
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
        if (targetWindowPrecision.RicochetMarker != null)
        {
            culpritxz = new Vector2(parentShooter.transform.position.x, parentShooter.transform.position.z);
            ricochetxz = new Vector2(targetWindowPrecision.RicochetMarker.transform.position.x, targetWindowPrecision.RicochetMarker.transform.position.z);
            initDist = Vector2.Distance(culpritxz, ricochetxz);
        }
        forceLower = false;
        stop = false;
        GameObject target = PreciseTarget.WindowGO;
        targetWindowPrecision = PreciseTarget;
        transform.position = parentShooter.ShootPosition.position;
        trailRenderer.Clear();
        rbody.isKinematic = false;
        hitFirstPoint = false;
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

        //bounceForce = rbody.velocity.magnitude * Mathf.Clamp01(COR) * rbody.mass;
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
    private void LateUpdate()
    {
        if (rbody.isKinematic)
            return;

        if (initDist == 0)
            return;

        if (hitFirstPoint)
            return;
        if (targetWindowPrecision.RicochetMarker != null)
        {

            Vector2 ballxz = new Vector2(transform.position.x, transform.position.z);
            currDist = Vector2.Distance(culpritxz, ballxz);

            if (currDist > initDist)
            {
                //transform.position = new Vector3(ricochetxz.x, transform.position.y, ricochetxz.y);
                if (!hitFirstPoint)
                {
                    //hitFirstPoint = false;
                    
                    rbody.isKinematic = true;
                    parentShooter.travelling = false;
                    parentShooter.shootNext = true;
                    return;
                }
            }
            else
            {
                
            }
        }
    }

    bool IsGrounded()
    {
        return (transform.position.y < 0);
    }

    void SimulateInRealTime(float dt)
    {
        // calculate drag and add force in opposing direction
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

    public void CalculateBounce(ContactPoint contact)
    {
        Vector3 randomOffset = Random.insideUnitSphere * 0.01f;

        // Calculate reflection vector based on incoming direction and surface normal
        Vector3 reflectionDirection = Vector3.Reflect(vel, contact.normal) + randomOffset;

        transform.position = contact.point;

        // Apply force considering COR
        rbody.velocity = reflectionDirection * Mathf.Clamp01(COR);
    }
    public void ResetBall(ContactPoint contact)
    {
        rbody.isKinematic = true;
        parentShooter.travelling = false;
        parentShooter.shootNext = true;
    }
    private void OnCollisionEnter(Collision other)
    {

        contact = other.GetContact(0);
        transform.position = contact.point;

        if (other.transform.tag == "Roof")
        {
            ResetBall(contact);
            forceLower = true;
            return;
        }

        if (!hitFirstPoint && targetWindowPrecision.RicochetMarker != null)
        {
            if (targetWindowPrecision.RicochetMarker != null
          && Vector3.Distance(contact.point, targetWindowPrecision.RicochetMarker.transform.position) > 0.5f * targetWindowPrecision.RicochetMarker.transform.localScale.x
          )
            {
                hitFirstPoint = false;
                ResetBall(contact);
                return;
            }
            else
            {
                hitFirstPoint = true;

                // apply reflection bounce
                CalculateBounce(contact);
                return;
            }
        }

        ResetBall(contact);

        if (targetWindowPrecision.PrecisionMarker != null 
            && Vector3.Distance(contact.point, targetWindowPrecision.PrecisionMarker.transform.position) > 0.5f * targetWindowPrecision.PrecisionMarker.transform.localScale.x 
            )
        {
            return;
        }
        else
        {
            // return if havent hit first point or is not the target window
            if (other.transform.gameObject != targetWindow || (!hitFirstPoint && targetWindowPrecision.RicochetMarker != null))
            {
                return;
            }
        }

        Vector3 normal = contact.normal;
        parentShooter.windowHit[target] = true;
        parentShooter.TotalBallsHit++;
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
            Culprit.OnCantHit.Invoke(parentShooter.gameObject, target);
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
