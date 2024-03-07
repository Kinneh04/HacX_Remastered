using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System;

public class Culprit : MonoBehaviour
{

    public SkinnedMeshRenderer culpritMeshRenderer;
    public static Action<GameObject, int> OnCantHit = delegate { };
    public static Action<GameObject, int> OnHit = delegate { };
    public static Action<Culprit> OnDone = delegate { };

    public int floor, column;

    public int MAX_ITERATIONS;

    public Transform ShootPosition;
    public int currentTarget = 0;

    public bool travelling = false;
    public bool below = true;
    public bool hit = false;

    public bool done = false;
    public bool finishedCurrent = false;
    public bool shootNext = false;
    public bool canHitCurrWindow = false;

    public Ball[] balls;
    public List<Ball> allBalls = new();
    public List<Precise_Window> windows = new();
    public List<bool> windowHit = new();
    public List<int> iterations = new();
    public List<float> probabilities = new();
    public float launchAngleMax = 0f;
    public float launchAngleMin = -90f;
    public float angle;
    public float probability = 0f;
    public float averageProbability = 0f;
    public float currentIteration;
    public float radius = 0f;
    public Vector3 initialPosition;

    public LayerMask layerMask;

    WindowsManager wManager;

    public GameObject Ball;
    Ball currentBall;
    Rigidbody currentBallRb;
    Quaternion targetRotation;

    public TMP_Text probabilityText;

    public Outline outline;

    [Header("BallsHit")]
    public int totalBallsThrown, TotalBallsHit;
    private void Awake()
    {
        MainGameManager.OnStartGame += OnStart;
        MainGameManager.OnNextWindow += CheckCanNext;

        wManager = WindowsManager.Instance;
    }

    private void Start()
    {
        outline = GetComponent<Outline>();

        initialPosition = transform.position;
    }

    private void OnDestroy()
    {
        MainGameManager.OnStartGame -= OnStart;
        MainGameManager.OnNextWindow -= CheckCanNext;
    }
    public void ResetVariables()
    {
        currentTarget = 0;
        travelling = false;
        below = true;
        hit = false;

        done = false;
        finishedCurrent = false;
        shootNext = false;
        canHitCurrWindow = false;

        Array.Clear(balls, 0, balls.Length);
        windows.Clear();
        windowHit.Clear();
        iterations.Clear();

        launchAngleMax = 0f;
        launchAngleMin = -90f;
        probability = 0f;

        radius = DontDestroyOnLoadSettings.Instance.positionRange;

        foreach(Ball ball in allBalls)
        {
            ball.trailRenderer.Clear();
            ball.gameObject.SetActive(false);
        }
    }

    public void RandomPosition()
    {
        Vector3 randomPosition = UnityEngine.Random.insideUnitSphere * radius;
        randomPosition += initialPosition;
        transform.position = new Vector3(randomPosition.x, transform.position.y, randomPosition.z);
    }
    public void OnStart(int newVelocity, int newIter)
    {
      
        ResetVariables();
        RandomPosition();

        currentIteration = newIter;
        MAX_ITERATIONS = DontDestroyOnLoadSettings.Instance.MaxIterationsValue;

        foreach(Precise_Window PW in WindowsManager.Instance.PreciseWindows)
        {
            windows.Add(PW);
        }
        //windows = WindowsManager.Instance.PreciseWindows;
        balls = new Ball[windows.Count];

        for (int i = 0; i < windows.Count; i++)
        {
            windowHit.Add(false);
            iterations.Add(0);

            // pre instantiate the balls for each window
            balls[i] = Instantiate(Ball, ShootPosition.position, ShootPosition.rotation, transform).GetComponent<Ball>();
            balls[i].NewVel(newVelocity);
            allBalls.Add(balls[i]);
            balls[i].gameObject.SetActive(false);
        }
        GameObject tar;
        if (windows[currentTarget].RicochetMarker != null)
            tar = windows[currentTarget].RicochetMarker;
        else
            tar = windows[currentTarget].PrecisionMarker;
        if (!CanSeeObject(tar))
        {
            finishedCurrent = true;
            OnCantHit?.Invoke(gameObject, currentTarget);
            return;
        }
        balls[currentTarget].gameObject.SetActive(true);
        currentBall = balls[currentTarget];
        currentBallRb = currentBall.GetComponent<Rigidbody>();
        FireProjectileAt(currentTarget);
    }

    public void CheckIfNext()
    {
        Debug.Log("hit");
        if(windowHit[currentTarget] || iterations[currentTarget] >= MAX_ITERATIONS)
        {
            CheckCanNext();
        }
    }

    public void CheckCanNext()
    {
        if (currentTarget + 1 <= windows.Count - 1)
        {
            currentTarget++;
            GameObject tar;
            if (windows[currentTarget].RicochetMarker != null)
                tar = windows[currentTarget].RicochetMarker;
            else
                tar = windows[currentTarget].PrecisionMarker;
            if (!CanSeeObject(tar))
            {
                finishedCurrent = true;
                OnCantHit?.Invoke(gameObject, currentTarget);
                return;
            }
            finishedCurrent = false;
            currentBall = balls[currentTarget];
            currentBall.gameObject.SetActive(true);
            currentBallRb = currentBall.GetComponent<Rigidbody>();
            FireProjectileAt(currentTarget);
        }
        else
        {
            done = true;
            CalculateProbability();
        }
    }

    public bool CanSeeObject(GameObject target) // check both ways because of how colliders work
    {
        if (target == null)
            target = windows[currentTarget].PrecisionMarker;

        bool pointSeeCulprit = false;
        bool culpritSeePoint = false;

        // check to see if the point can see culprit
        Vector3 origin = ShootPosition.position;
        Vector3 direction = origin - target.transform.position;

        float maxDistance = Vector3.Distance(origin, target.transform.position);

        RaycastHit hit;

        if (Physics.Raycast(target.transform.position, direction, out hit, maxDistance))
        {
            // Check if the raycast hit the target object
            if (hit.transform == ShootPosition || hit.transform == transform)
            {
                // No obstacle between the objects, so the object can see the target
                pointSeeCulprit = true;
            }
            else
            {
                // Raycast hit something else, so the object cannot see the target
                pointSeeCulprit = false;
                //Debug.Log(this.name + " | " + hit.transform.gameObject.name);
            }
        }
        else
        {
            // Raycast didn't hit anything within the max distance, so the object can potentially see the target
            pointSeeCulprit = true;
        }

        // check to see if the culprit can see point
        //origin = ShootPosition.position;
        direction = target.transform.position - origin;

        //maxDistance = Vector3.Distance(origin, target.transform.position);
        //hit everything but culprit layer
        if (Physics.Raycast(origin, direction, out hit, maxDistance, ~layerMask))
        {
            // Check if the raycast hit the target object
            if (hit.transform == target.transform)
            {
                // No obstacle between the objects, so the object can see the target
                culpritSeePoint = true;
            }
            else
            {
                // Raycast hit something else, so the object cannot see the target
                culpritSeePoint = false;
                //Debug.Log(this.name + " | " + hit.transform.gameObject.name);
            }
        }
        else
        {
            // Raycast didn't hit anything within the max distance, so the object can potentially see the target
            culpritSeePoint = true;
        }

        return pointSeeCulprit && culpritSeePoint;
    }
    private void Update()
    {
        
         outline.OutlineColor = Color.Lerp(outline.OutlineColor, new Color(0, 0, 0, 0), Time.deltaTime);

        if (!currentBall || !currentBallRb.isKinematic)
            return;

        if (done || !shootNext || windowHit[currentTarget] || finishedCurrent)
            return;

        if (iterations[currentTarget] >= MAX_ITERATIONS)
        {
            finishedCurrent = true;
            OnCantHit?.Invoke(gameObject, currentTarget);
            //Debug.Log(gameObject.name + " " + iterations[currentTarget] + " " + MAX_ITERATIONS);

            return;
        }

        // binary search
        if(currentBall.forceLower)
        {
            launchAngleMax = angle;
        }
        else // for ricochet
        {
            if (!balls[currentTarget].hitFirstPoint && windows[currentTarget].RicochetMarker != null)
            {
                if (currentBall.transform.position.y - windows[currentTarget].RicochetMarker.transform.position.y > 0.5 * currentBall.transform.localScale.x)
                {
                    launchAngleMax = angle;
                }
                else if (windows[currentTarget].RicochetMarker.transform.position.y - currentBall.transform.position.y > 0.5 * currentBall.transform.localScale.x)
                {
                    launchAngleMin = angle;
                }
                else
                {
                    if (currentBall.currDist > currentBall.initDist)
                    {
                        launchAngleMax = angle;
                    }
                    else if (currentBall.currDist < currentBall.initDist)
                    {
                        launchAngleMin = angle;
                    }
                }
            }
            else// for just window
            {
                if (currentBall.transform.position.y > windows[currentTarget].PrecisionMarker.transform.position.y)
                {
                    launchAngleMax = angle;
                }
                else if (currentBall.transform.position.y < windows[currentTarget].PrecisionMarker.transform.position.y)
                {
                    launchAngleMin = angle;
                }
            }
        }
        


        angle = (launchAngleMin + launchAngleMax) * 0.5f;
        Quaternion tiltRotation = Quaternion.Euler(angle, 0, 0);
        Quaternion finalRotation = targetRotation * tiltRotation;
        ShootPosition.rotation = finalRotation;
        currentBall.Shoot(WindowsManager.Instance.PreciseWindows[currentTarget], currentTarget);
        travelling = true;
        shootNext = false;
        totalBallsThrown++;
        iterations[currentTarget]++;
    }
    public void FireProjectileAt(int windowIndex)
    {
        launchAngleMax = -90f;
        Vector3 dir;
        if (windows[windowIndex].RicochetMarker != null)
            dir = windows[windowIndex].RicochetMarker.transform.position - ShootPosition.position;
        else
            dir = windows[windowIndex].PrecisionMarker.transform.position - ShootPosition.position;
        targetRotation = Quaternion.LookRotation(dir);

        angle = transform.rotation.eulerAngles.x;
        launchAngleMin = angle;
        ShootPosition.rotation = targetRotation;

        currentBall.Shoot(windows[windowIndex], windowIndex);

        travelling = true;
        shootNext = false;

        iterations[windowIndex]++;
    }

    public void CalculateProbability()
    {
        probabilityText.gameObject.SetActive(true);
        probability = 0.0f;

        foreach (Ball ball in balls)
        {
            if (ball.impactForce == 0)
            {
                probability += 0;
            }
            else
                probability += (((90 - ball.angleOfImpact) / 90) + (ball.impactSpeed / ball.initialVel)) * 0.5f;
        }
        probability = 100 * (probability / balls.Length);

        probabilities.Add(probability);
        float total = 0;
        for (int i = 0; i < probabilities.Count; i++)
        {
            total += probabilities[i];
        }
        averageProbability = total / probabilities.Count;

        probabilityText.text = averageProbability.ToString("F1") + "%";

        OnDone?.Invoke(this);
    }

    public void CalculateProbabilityWithWindows(List<int> Windows)
    {
        float totalProb = 0.0f;
        int calculations = 0;
        for(int i = 0; i < Windows.Count; i++)
        {
            foreach(Ball b in allBalls)
            {
                if(b.targetWindowPrecision == WindowsManager.Instance.PreciseWindows[Windows[i]])
                {
                    totalProb += calculateAccForBall(b);
                    calculations++;
                }
            }
        }
        averageProbability = totalProb / calculations;
        probabilityText.text = averageProbability.ToString("F1") + "%";
    }

    public float calculateAccForBall(Ball ball)
    {
        return (((90 - ball.angleOfImpact) / 90) + (ball.impactSpeed / ball.initialVel)) * 50f;
    }

}
