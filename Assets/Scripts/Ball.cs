using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    private Rigidbody rbody;
    public GameObject marker;
    public GameObject Go;
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
    public LineRenderer lineRenderer;
    public List<Vector3> points;

    public bool hitFirstPoint = false;

    public float initDist = 0;
    public float currDist = 0;
    Vector2 culpritxz = Vector2.zero;
    Vector2 ricochetxz = Vector2.zero;
    bool stop = false;
    RaycastHit contact;
    public bool forceLower = false;
    [Header("CalcProbability")]
    public float distanceFromCenter = 0f;
    public float angleOfImpact = 0f;
    public float impactSpeed = 0f;
    public float impactForce = 0f;
    public float pressureApplied = 0f;

    //debug purposes
    public bool collided = false;
    public bool canRico = false;
    public bool rico = false;
    public string collidedName;
    public bool calculated = false;
    public bool toreset = false;
    public bool checkDist = false;
    public Vector3 prevPos;
    public Plane ricochetPlane;
    public Vector3 contactPoint;
    public Vector3 contactOffset;
    public Vector3 ricochetNormal;
    public Vector3 intersectionPoint;
    public LayerMask layerMask;
    public Vector3 ricoPoint;
    Culprit parentShooter;
    void Awake()
    {
        rbody = GetComponent<Rigidbody>();
        //trailRenderer = GetComponent<TrailRenderer>();
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 0;
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
        points.Clear();
        prevPos = Vector3.zero;
        contactPoint = Vector3.zero;
        intersectionPoint = Vector3.zero;
        ricochetNormal = Vector3.zero;
        ricoPoint = Vector3.zero;
        collided = false;
        canRico = false;
        calculated = false;
        toreset = false;
        rico = false;
        collidedName = " ";
        contactOffset = Vector3.zero;
        lineRenderer.positionCount = 0;
        if (Go != null)
            Destroy(Go);
        if (targetWindowPrecision.RicochetMarker != null)
        {
            culpritxz = new Vector2(parentShooter.transform.position.x, parentShooter.transform.position.z);
            ricochetxz = new Vector2(targetWindowPrecision.RicochetMarker.transform.position.x, targetWindowPrecision.RicochetMarker.transform.position.z);
            initDist = Vector2.Distance(culpritxz, ricochetxz);
            
            Vector3 planeNormal = -parentShooter.ShootPosition.forward;
            Vector3 planePoint = targetWindowPrecision.RicochetMarker.transform.position /*+ (parentShooter.ShootPosition.forward * targetWindowPrecision.RicochetMarker.transform.localScale.x * 0.4f)*/;
            ricochetPlane = new Plane(planeNormal, planePoint);
        }
        collided = false;
        forceLower = false;
        stop = false;
        GameObject target = PreciseTarget.WindowGO;
        targetWindowPrecision = PreciseTarget;
        transform.position = parentShooter.ShootPosition.position;
        //trailRenderer.Clear();
        rbody.isKinematic = false;
        hitFirstPoint = false;
        isLaunched = true;
        targetWindow = target;
        this.target = targetIndex;
        // Assuming that 'transform.forward' represents the direction the ball is facing.
        Vector3 facingDirection = parentShooter.ShootPosition.forward;

        // Calculate the angle between the forward vector and the upward direction.
        float angle = Mathf.Atan2(facingDirection.y, facingDirection.z) * Mathf.Rad2Deg;

        rbody.AddForce(facingDirection * force, ForceMode.Impulse);

        //bounceForce = rbody.velocity.magnitude * Mathf.Clamp01(COR) * rbody.mass;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!IsGrounded() && !rbody.isKinematic)
        {
            // Debug.Log(rbody.velocity.magnitude);
            SimulateInRealTime();
            vel = rbody.velocity;

            points.Add(transform.position);

            if (targetWindowPrecision.RicochetMarker != null)
            {
                if(!toreset)
                {
                    Vector2 ballxz = new Vector2(transform.position.x, transform.position.z);
                    currDist = Vector2.Distance(culpritxz, ballxz);
                }

                if (!hitFirstPoint && (currDist > initDist))
                {
                    CheckRicochet();
                        
                }
                if (!collided)
                    StartCoroutine(HandlePotentialCollision());
            }
            prevPos = transform.position;
        }

    }

    bool CheckRicochet()
    {
        if (rbody.velocity.magnitude == 0)
            return false;
        float distance;

        Vector3 movement = transform.position - prevPos;

        if (ricochetPlane.Raycast(new(prevPos, movement), out distance))
        {
            Vector3 intersectionPoint = prevPos + movement.normalized * distance;

            rbody.isKinematic = true;
            parentShooter.shootNext = true;

            contactPoint = intersectionPoint;
            //transform.position = contactPoint;
            this.intersectionPoint = intersectionPoint;

            contactOffset = Vector3.zero;
            RenderLine();
            toreset = true;

            return false;
        }
        return true;
    }

    bool CheckCollision()
    {
        if (rbody.isKinematic)
            return false;
        // Calculate projected position one fixed frame ahead
        Vector3 nextPosition = transform.position + rbody.velocity.normalized * rbody.velocity.magnitude * Time.fixedDeltaTime;
        
        // Perform sphere cast with fixed time step
        RaycastHit hit;
        if (Physics.SphereCast(transform.position, r, rbody.velocity.normalized, out hit, rbody.velocity.magnitude * Time.fixedDeltaTime, ~layerMask))
        {
            // Collision detected! Handle the hit
            Debug.Log("Hit something at: " + hit.point);
            contact = hit;
            return true;
        }
        else
            return false;
    }

    IEnumerator HandlePotentialCollision()
    {
        collided = CheckCollision();

        if(collided)
        {
            yield return new WaitForFixedUpdate();

            HandleCollision(contact);
            //collided = false;
            yield return new WaitForFixedUpdate();

            collided = false;
        }
    }
    void HandleCollision(RaycastHit other)
    {
        collidedName = other.transform.gameObject.name;
        contact = other;
        
        CheckIfHitRoof(other);

        if (!toreset)
        {
            contactPoint = contact.point;
            contactOffset = contact.normal * r;
            transform.position = contact.point + contactOffset;
            currDist = Vector3.Distance(culpritxz, new Vector2(contactPoint.x, contactPoint.z));
        }




        if (targetWindowPrecision.RicochetMarker != null && !hitFirstPoint)
        {
            rico = Vector3.Distance(targetWindowPrecision.RicochetMarker.transform.position, contact.point) < 0.5f * targetWindowPrecision.RicochetMarker.transform.localScale.x;
            if (rico && contact.normal == targetWindowPrecision.RicochetMarker.transform.right)
            {
                hitFirstPoint = true;
                toreset = false;
                ricochetNormal = contact.normal;
                ricoPoint = contactPoint;
                // apply reflection bounce
                CalculateBounce(contact);

                calculated = true;
                contactPoint = contact.point;
                points.Add(transform.position);
                RenderLine();

                return;
            }
            else // not within range
            {
                hitFirstPoint = false;

                ResetBall();

                points.Add(transform.position);
                RenderLine();
                currDist = Vector3.Distance(culpritxz, new Vector2(contactPoint.x, contactPoint.z));

                return;
            }
        }

        points.Add(transform.position);
        RenderLine();

        ResetBall();

        if (targetWindowPrecision.PrecisionMarker != null)
        {
            // see if hit window
            if (Vector3.Distance(contact.point, targetWindowPrecision.PrecisionMarker.transform.position) > 0.5f * targetWindowPrecision.PrecisionMarker.transform.localScale.x)
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
        }




        //transform.position = contact.point + (contact.normal * r);

        Vector3 normal = contact.normal;
        parentShooter.windowHit[target] = true;
        parentShooter.TotalBallsHit++;
        // values to calc accuracy
        distanceFromCenter = Vector3.Distance(transform.position, targetWindowPrecision.PrecisionMarker.transform.position);
        angleOfImpact = Vector3.Angle(vel, -normal);
        impactSpeed = vel.magnitude;

        // impact in newtons, kinetic energy f = 0.5 * m *v^2 / distance
        Vector3 potentialVelocityChange = other.point - prevPos;
        impactForce = potentialVelocityChange.magnitude / Time.fixedDeltaTime;
        //impactForce = other.impulse.magnitude;

        // calculate contact area (for spheres only)
        float contactArea = CalculateContactArea(angleOfImpact);

        // calculate pressure (mPa)
        pressureApplied = impactForce / area;

        if (pressureApplied > targetWindowPrecision.breakingStress)
        {
            Culprit.OnHit?.Invoke(gameObject, target);
        }
        else
        {
            parentShooter.finishedCurrent = true;
            Culprit.OnCantHit.Invoke(parentShooter.gameObject, target);
        }
    }
    bool IsGrounded()
    {
        return (transform.position.y < 0);
    }

    void SimulateInRealTime()
    {
        // calculate drag and add force in opposing direction
        Vector3 dragForce = -0.5f * (p * rbody.velocity.sqrMagnitude * dragCoefficient * area * rbody.velocity.normalized);
        rbody.AddForce(dragForce, ForceMode.Force);
    }

    public void CalculateBounce(RaycastHit contact)
    {
        //Vector3 randomOffset = Random.insideUnitSphere * 0.01f;

        // Calculate reflection vector based on incoming direction and surface normal
        Vector3 reflectionDirection = Vector3.Reflect(vel, contact.normal);

        // Apply force considering COR
        rbody.velocity = reflectionDirection * Mathf.Clamp01(COR);
    }
    public void ResetBall()
    {
        rbody.isKinematic = true;
        parentShooter.shootNext = true;
    }

    void CheckIfHitRoof(RaycastHit other)
    {
        if (other.transform.tag == "Roof") // if hit roof lower angle
        {
            ResetBall();
            forceLower = true;
            return;
        }
    }
    private void OnCollisionEnter(Collision other)
    {
        //collided = true;
        //collidedName = other.transform.gameObject.name;
        //contact = other.GetContact(0);

        //CheckIfHitRoof(other);

        //if (!toreset)
        //{
        //    contactPoint = contact.point;
        //    //contactOffset = contact.normal * r;
        //    currDist = Vector3.Distance(culpritxz, new Vector2(contactPoint.x, contactPoint.z));
        //}




        //if (targetWindowPrecision.RicochetMarker != null && !hitFirstPoint)
        //{
        //    Vector3 balldir = targetWindowPrecision.RicochetMarker.transform.position - transform.position;
        //    //Vector3 projectedDir = Vector3.Project(balldir, targetWindowPrecision.RicochetMarker.transform.right);
        //    float isUnder = Vector3.Dot(balldir, -targetWindowPrecision.RicochetMarker.transform.right);
        //    rico = Vector3.Distance(targetWindowPrecision.RicochetMarker.transform.position, contact.point) < 0.5f * targetWindowPrecision.RicochetMarker.transform.localScale.x;
        //    if (rico && contact.normal == targetWindowPrecision.RicochetMarker.transform.right /*&& isUnder >= 0*/)
        //    {
        //        hitFirstPoint = true;

        //        ricochetNormal = contact.normal;
        //        // apply reflection bounce
        //        CalculateBounce(contact);
                
        //        calculated = true;
        //        contactPoint = contact.point;
        //        points.Add(transform.position);
        //        RenderLine();

        //        return;
        //    }
        //    else // not within range
        //    {
        //        hitFirstPoint = false;

        //        ResetBall();
                
        //        points.Add(transform.position);
        //        RenderLine();

                
        //        return;
        //    }
        //}

        //points.Add(transform.position);
        //RenderLine();

        //ResetBall();

        //if (targetWindowPrecision.PrecisionMarker != null)
        //{
        //    // see if hit window
        //    if (Vector3.Distance(contact.point, targetWindowPrecision.PrecisionMarker.transform.position) > 0.5f * targetWindowPrecision.PrecisionMarker.transform.localScale.x)
        //    {
        //        return;
        //    }
        //    else
        //    {
        //        // return if havent hit first point or is not the target window
        //        if (other.transform.gameObject != targetWindow || (!hitFirstPoint && targetWindowPrecision.RicochetMarker != null))
        //        {
        //            return;
        //        }
        //    }
        //}




        ////transform.position = contact.point + (contact.normal * r);
        
        //Vector3 normal = contact.normal;
        //parentShooter.windowHit[target] = true;
        //parentShooter.TotalBallsHit++;
        //// values to calc accuracy
        //distanceFromCenter = Vector3.Distance(transform.position, targetWindowPrecision.PrecisionMarker.transform.position);
        //angleOfImpact = Vector3.Angle(vel, -normal);
        //impactSpeed = vel.magnitude;

        //// impact in newtons, kinetic energy f = 0.5 * m *v^2 / distance
        //impactForce = other.impulse.magnitude;

        //// calculate contact area (for spheres only)
        //float contactArea = CalculateContactArea(angleOfImpact);

        //// calculate pressure (mPa)
        //pressureApplied = impactForce / area;

        //if (pressureApplied > targetWindowPrecision.breakingStress)
        //{
        //    Culprit.OnHit?.Invoke(gameObject, target);
        //}
        //else
        //{
        //    parentShooter.finishedCurrent = true;
        //    Culprit.OnCantHit.Invoke(parentShooter.gameObject, target);
        //}

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

    public void RenderLine()
    {
        lineRenderer.positionCount = 0;

        if (points.Count <= 0)
            return;
        lineRenderer.positionCount = points.Count;

        for (int i = 0; i < points.Count; i++)
            lineRenderer.SetPosition(i, points[i]);
    }
}
