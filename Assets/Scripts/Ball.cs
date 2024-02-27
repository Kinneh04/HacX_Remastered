using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    private Rigidbody rbody;
    Collider collider;

    public bool isLaunched;

    public float density = 7750f; //kg/m^3
    public float dragCoefficient = 0.1f;
    public float volume = 0;
    public float initialVel = 70f; // m/s
    private float force;
    float distToGround;
    public float r;
    float p = 1.225f; //density of air 1.225kg/m^3
    public float area;
    public Material window1;
    public Material window2;
    private int target;
    Vector3 vel;
    float defaultVel = 70.0f;
    private GameObject targetWindow;
    TrailRenderer trailRenderer;

    Culprit parentShooter;
    void Awake()
    {
        rbody = GetComponent<Rigidbody>();
        trailRenderer = GetComponent<TrailRenderer>();
        parentShooter = transform.parent.GetComponent<Culprit>();
        isLaunched = false;

        dragCoefficient = 0.1f;
        // get drag coefficient 

        r = transform.localScale.y * 0.5f;
        volume = (4 * Mathf.PI * r * r * r) / 3;
        rbody.mass = (density * volume); // in grams

        area = 2 * Mathf.PI * r * r;

        //initialVel = SettingsMenu.instance.GetInitVel();
        force = rbody.mass * initialVel;

        rbody.isKinematic = true;
        
    }

    public void Shoot(GameObject target, int targetIndex)
    {
        trailRenderer.Clear();
        trailRenderer.Clear();
        transform.position = parentShooter.ShootPosition.position;
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

        if (other.transform.gameObject != targetWindow)
        {
            trailRenderer.Clear();
            return;
        }
 


        //Vector3 vel = rbody.velocity;
        Vector3 normal = other.contacts[0].normal;
        parentShooter.travelling = false;
        rbody.isKinematic = true;
        transform.position = other.contacts[0].point;

        if (Vector3.Distance(other.contacts[0].point, other.transform.position) > 0.2f)
        {
            trailRenderer.Clear();
            return;
        }


        parentShooter.windowHit[target] = true;
        parentShooter.CheckIfNext();

        //if (target == 0)
        //    shooter.hitWindow1 = true;

        //else if (target == 1)
        //    shooter.hitWindow2 = true;

        //Debug.Log("HIT TARGET: " + target);

        //HitBall HB = new HitBall();
        //HB.RelatedHumanGameObject = transform.parent.gameObject;
        //HB.WindowHit = target;
        //if (target == 0)
        //{
        //    HB.DistanceFromCenterW1 = Vector3.Distance(transform.position, targets[target].transform.position);
        //    shooter.angle1 = Vector3.Angle(vel, -normal);
        //    shooter.hitSpeed1 = vel.magnitude;
        //}
        //else
        //{
        //    HB.DistanceFromCenterW2 = Vector3.Distance(transform.position, targets[target].transform.position);
        //    shooter.angle2 = Vector3.Angle(vel, -normal);
        //    shooter.hitSpeed2 = vel.magnitude;
        //}
        //HB.CalculateAccuracy();
        //HB.Hitposition = transform.position;
        //MainGameManager.instance.AddNewHitRegistryToList(HB);
        //}


        //if (target == 0 && (shooter.iterations1 < shooter.maxIterations))
        //    return;

        //else if (target == 1 && (shooter.iterations2 < shooter.maxIterations))
        //    return;

        //if (other.transform.gameObject != targets[target].gameObject)
        //{
        //    shooter.travelling = false;
        //    rbody.isKinematic = true;
        //}
        //else
        //{
        //    //Vector3 vel = rbody.velocity;
        //    Vector3 normal = other.contacts[0].normal;

        //    shooter.travelling = false;
        //    rbody.isKinematic = true;
        //    transform.position = other.contacts[0].point;

        //    if (Vector3.Distance(other.contacts[0].point, other.transform.position) > 0.2f)
        //        return;

        //    if (target == 0)
        //        shooter.hitWindow1 = true;

        //    else if (target == 1)
        //        shooter.hitWindow2 = true;



        //    //Debug.Log("HIT TARGET: " + target);

        //    HitBall HB = new HitBall();
        //    HB.RelatedHumanGameObject = transform.parent.gameObject;
        //    HB.WindowHit = target;
        //    if (target == 0)
        //    {
        //        HB.DistanceFromCenterW1 = Vector3.Distance(transform.position, targets[target].transform.position);
        //        shooter.angle1 = Vector3.Angle(vel, -normal);
        //        shooter.hitSpeed1 = vel.magnitude;
        //    }
        //    else
        //    {
        //        HB.DistanceFromCenterW2 = Vector3.Distance(transform.position, targets[target].transform.position);
        //        shooter.angle2 = Vector3.Angle(vel, -normal);
        //        shooter.hitSpeed2 = vel.magnitude;
        //    }
        //    HB.CalculateAccuracy();
        //    HB.Hitposition = transform.position;
        //    MainGameManager.instance.AddNewHitRegistryToList(HB);
        //}
    }
}
