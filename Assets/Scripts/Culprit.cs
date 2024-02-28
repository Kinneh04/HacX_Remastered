using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System;

public class Culprit : MonoBehaviour
{
    public static Action<GameObject, int> OnCantHit = delegate { };
    public static Action<GameObject, int> OnHit = delegate { };

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
    public List<GameObject> windows = new();
    public List<bool> windowHit = new();
    public List<int> iterations = new();
    
    public float launchAngleMax = 0f;
    public float launchAngleMin = -90f;
    public float angle;

    public GameObject Ball;
    Ball currentBall;
    Rigidbody currentBallRb;
    Quaternion targetRotation;

    private void Awake()
    {
        MainGameManager.OnStartGame += OnStart;
        MainGameManager.OnNextWindow += CheckCanNext;
    }

    private void OnDestroy()
    {
        MainGameManager.OnStartGame -= OnStart;
        MainGameManager.OnNextWindow -= CheckCanNext;
    }
    public void OnStart()
    {
        finishedCurrent = false;

        foreach(Precise_Window PW in WindowsManager.Instance.PreciseWindows)
        {
            if (PW.PrecisionMarker != null)
                windows.Add(PW.PrecisionMarker);
            else windows.Add(PW.WindowGO);
        }
        //windows = WindowsManager.Instance.PreciseWindows;
        balls = new Ball[windows.Count];
        for (int i = 0; i < windows.Count; i++)
        {
            windowHit.Add(false);
            iterations.Add(0);

            // pre instantiate the balls for each window
            balls[i] = Instantiate(Ball, ShootPosition.position, ShootPosition.rotation, transform).GetComponent<Ball>();
            //balls[i].SetTarget(i);
            balls[i].gameObject.SetActive(false);
        }
        if (!CanSeeObject(windows[currentTarget]))
        {
            OnCantHit?.Invoke(gameObject, currentTarget);
            finishedCurrent = true;
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
            if (!CanSeeObject(windows[currentTarget]))
            {
                OnCantHit?.Invoke(gameObject, currentTarget);
                finishedCurrent = true;
                return;
            }
            finishedCurrent = false;
            currentBall = balls[currentTarget];
            currentBall.gameObject.SetActive(true);
            currentBallRb = currentBall.GetComponent<Rigidbody>();
            FireProjectileAt(currentTarget);
        }
        else
            done = true;
    }

    public bool CanSeeObject(GameObject target)
    {
        Vector3 origin = ShootPosition.position;
        Vector3 direction = origin - target.transform.position;

        float maxDistance = Vector3.Distance(origin, target.transform.position);

        RaycastHit hit;

        if (Physics.Raycast(target.transform.position, direction, out hit, maxDistance))
        {
            // Check if the raycast hit the target object
            if (hit.transform == ShootPosition)
            {
                // No obstacle between the objects, so the object can see the target
                return true;
            }
            else
            {
                // Raycast hit something else, so the object cannot see the target
                return false;
            }
        }
        else
        {
            // Raycast didn't hit anything within the max distance, so the object can potentially see the target
            return true;
        }
    }
    private void Update()
    {
        
        if (!currentBall || !currentBallRb.isKinematic)
            return;

        if (done || !shootNext || windowHit[currentTarget] || finishedCurrent)
            return;

        if (iterations[currentTarget] == MAX_ITERATIONS)
        {
            OnCantHit?.Invoke(gameObject, currentTarget);
            finishedCurrent = true;
            return;
        }


        if (currentBall.transform.position.y > windows[currentTarget].transform.position.y)
        {
            launchAngleMax = angle;
        }
        else if (currentBall.transform.position.y < windows[currentTarget].transform.position.y)
        {
            launchAngleMin = angle;
        }

        angle = (launchAngleMin + launchAngleMax) * 0.5f;
        //Debug.Log(angle + " " + launchAngleMin + " " + launchAngleMax);
        Quaternion tiltRotation = Quaternion.Euler(angle, 0, 0);
        Quaternion finalRotation = targetRotation * tiltRotation;
        ShootPosition.rotation = finalRotation;
        currentBall.Shoot(WindowsManager.Instance.PreciseWindows[currentTarget], currentTarget);
        travelling = true;
        shootNext = false;

        iterations[currentTarget]++;
    }
    public void FireProjectileAt(int windowIndex)
    {
        //maxIterations = SettingsMenu.instance.GetMaxIterations();

        //targets = MainGameManager.instance.GetWindows();

        launchAngleMax = -90f;

        Vector3 dir = windows[windowIndex].transform.position - ShootPosition.position;

        targetRotation = Quaternion.LookRotation(dir);

        angle = transform.rotation.eulerAngles.x;
        launchAngleMin = angle;
        ShootPosition.rotation = targetRotation;

        currentBall.Shoot(WindowsManager.Instance.PreciseWindows[windowIndex], windowIndex);

        travelling = true;
        shootNext = false;

        iterations[windowIndex]++;
    }
}
