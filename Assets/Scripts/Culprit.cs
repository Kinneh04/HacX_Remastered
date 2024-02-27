using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Culprit : MonoBehaviour
{
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
    }

    private void OnDestroy()
    {
        MainGameManager.OnStartGame -= OnStart;
    }
    public void OnStart()
    {
        windows = WindowsManager.Instance.SelectedWindows;
        balls = new Ball[windows.Count];
;       for (int i = 0; i < windows.Count; i++)
        {
            windowHit.Add(false);
            iterations.Add(0);

            balls[i] = Instantiate(Ball, ShootPosition.position, ShootPosition.rotation, transform).GetComponent<Ball>();
            //balls[i].SetTarget(i);
            balls[i].gameObject.SetActive(false);
        }
        balls[currentTarget].gameObject.SetActive(true);
        currentBall = balls[currentTarget];
        currentBallRb = currentBall.GetComponent<Rigidbody>();
        FireProjectileAt(currentTarget);
    }

    public void CheckIfNext()
    {
        if(windowHit[currentTarget] || iterations[currentTarget] >= MAX_ITERATIONS)
        {
            finishedCurrent = true;
            if (currentTarget + 1 <= windows.Count - 1)
            {
                currentTarget++;
                currentBall = balls[currentTarget];
                currentBall.gameObject.SetActive(true);
                currentBallRb = currentBall.GetComponent<Rigidbody>();
                FireProjectileAt(currentTarget);
            }
            else
                done = true;
        }
    }
    private void Update()
    {
        //Debug.Log(currentBall);
        if (!currentBall || !currentBallRb.isKinematic)
            return;

        CheckIfNext();

        if (done || !shootNext)
            return;

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

        currentBall.Shoot(windows[currentTarget], currentTarget);
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

        currentBall.Shoot(windows[windowIndex], windowIndex);

        travelling = true;
        shootNext = false;

        iterations[windowIndex]++;
    }
}
