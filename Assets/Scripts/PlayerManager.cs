using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerManager : MonoBehaviour
{
    [Header("Speed Settings")]
    [Range(0f, 1f)] public float maxSpeed;
    [Range(0f, 1f)] public float camSpeed;
    [Range(0, 100)] public float pathSpeed;

    [Header("Particle Effects")]
    public ParticleSystem collideEffect;
    public ParticleSystem airEffect;
    public ParticleSystem dustEffect;
    public ParticleSystem BallTrail;
    public ParticleSystem.MainModule airEffectMain;

    public Transform path;
    private Transform ball;
    private Vector3 startMousePos, startBallPos;
    private bool moveTheBall;

    private float camVelocity_x, camVelocity_y;
    private Camera mainCam;
    private Rigidbody rb;
    private Collider _collider;
    private float startPathSpeed;
    private Renderer BallRenderer;
    private bool moveRightOrLeft;
    [Header("Score Settings")]
    [SerializeField] private int redScore;
    [SerializeField] private int greenScore;
    [SerializeField] private int blueScore;
    [SerializeField] private int yellowScore;


    private void Awake()
    {
        airEffectMain = airEffect.main;
        ball = transform;
        mainCam = Camera.main;
        rb = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
        startPathSpeed = pathSpeed;
        BallRenderer = GetComponent<Renderer>();
        Application.targetFrameRate = 60;
        moveRightOrLeft = true;
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            UIManager.UIManagerInstance.StartTheGame();
            moveTheBall = true;
            Plane newPlane = new Plane(Vector3.up, 0f);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (newPlane.Raycast(ray, out var distance))
            {
                startMousePos = ray.GetPoint(distance);
                startBallPos = ball.position;
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            moveTheBall = false;
        }
        if (moveTheBall)
        {
            Plane newPlane = new Plane(Vector3.up, 0f);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (newPlane.Raycast(ray, out var distance))
            {
                Vector3 mouseNewPos = ray.GetPoint(distance);
                Vector3 MouseNewPos = mouseNewPos - startMousePos;
                Vector3 DesireBallPos = mouseNewPos + startBallPos;

                DesireBallPos.x = Mathf.Clamp(DesireBallPos.x, -1.5f, 1.5f);
                if (moveRightOrLeft)
                {
                    ball.position = new Vector3(Mathf.SmoothDamp(ball.position.x, DesireBallPos.x, ref camVelocity_x, maxSpeed), ball.position.y, ball.position.z);
                }
            }
        }
        if (UIManager.UIManagerInstance.GameState)
        {
            Vector3 pathNewPos = path.position;
            path.position = new Vector3(pathNewPos.x, pathNewPos.y, Mathf.MoveTowards(pathNewPos.z, -1000f, Time.deltaTime * pathSpeed));
        }
    }
    private void LateUpdate()
    {
        Vector3 cameraNewPos = mainCam.transform.position;

        mainCam.transform.position = new Vector3(
         Mathf.SmoothDamp(cameraNewPos.x, ball.position.x, ref camVelocity_x, camSpeed),
         Mathf.SmoothDamp(cameraNewPos.y, ball.position.y + 3f, ref camVelocity_y, camSpeed),
         cameraNewPos.z);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("obstacle"))
        {
            Dead();
        }
        if (other.CompareTag("finishCube"))
        {
            ball.GetComponent<Renderer>().material.color = other.GetComponent<Renderer>().material.color;
        }

        switch (other.tag)
        {
            case "red":
                ParticleSystem newParticle = Instantiate(collideEffect, transform.position, Quaternion.identity);
                newParticle.GetComponent<Renderer>().material = other.GetComponent<Renderer>().material;
                Destroy(other.gameObject);
                BallRenderer.material = other.GetComponent<Renderer>().material;
                var BallTrailColor_1 = this.BallTrail.trails;
                BallTrailColor_1.colorOverLifetime = other.GetComponent<Renderer>().material.color;
                ScoreManager.instance.score += redScore;
                if (ScoreManager.instance.score < 0)
                {
                    Dead();
                }
                UIManager.UIManagerInstance.UpdateSlider();
                break;

            case "blue":
                ParticleSystem newParticle1 = Instantiate(collideEffect, transform.position, Quaternion.identity);
                newParticle1.GetComponent<Renderer>().material = other.GetComponent<Renderer>().material;

                Destroy(other.gameObject);
                BallRenderer.material = other.GetComponent<Renderer>().material;
                var BallTrailColor_2 = this.BallTrail.trails;
                BallTrailColor_2.colorOverLifetime = other.GetComponent<Renderer>().material.color;
                ScoreManager.instance.score += blueScore;
                UIManager.UIManagerInstance.UpdateSlider();
                break;

            case "yellow":
                ParticleSystem newParticle2 = Instantiate(collideEffect, transform.position, Quaternion.identity);
                newParticle2.GetComponent<Renderer>().material = other.GetComponent<Renderer>().material;
                Destroy(other.gameObject);
                BallRenderer.material = other.GetComponent<Renderer>().material;
                var BallTrailColor_3 = this.BallTrail.trails;
                BallTrailColor_3.colorOverLifetime = other.GetComponent<Renderer>().material.color;
                ScoreManager.instance.score += yellowScore;
                UIManager.UIManagerInstance.UpdateSlider();
                break;

            case "green":
                ParticleSystem newParticle3 = Instantiate(collideEffect, transform.position, Quaternion.identity);
                newParticle3.GetComponent<Renderer>().material = other.GetComponent<Renderer>().material;
                Destroy(other.gameObject);
                BallRenderer.material = other.GetComponent<Renderer>().material;
                var BallTrailColor_4 = this.BallTrail.trails;
                BallTrailColor_4.colorOverLifetime = other.GetComponent<Renderer>().material.color;
                ScoreManager.instance.score += greenScore;
                UIManager.UIManagerInstance.UpdateSlider();
                break;
        }
    }

    private void Dead()
    {
        gameObject.SetActive(false);
        UIManager.UIManagerInstance.GameState = false;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("path"))
        {
            rb.isKinematic = _collider.isTrigger = false;
            rb.velocity = new Vector3(0f, 8f, 0f);
            pathSpeed = pathSpeed * 2;
            airEffectMain.simulationSpeed = 10f;
            BallTrail.Stop();
        }
        if (other.CompareTag("beforeFinish"))
        {
            moveRightOrLeft = false;
            rb.isKinematic = _collider.isTrigger = false;
            rb.velocity = new Vector3(0f, 8f, 0f);
            pathSpeed = pathSpeed * 2;
            BallTrail.Stop();
            StartCoroutine(GoToMid());
        }
    }
    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.CompareTag("path") || (other.collider.CompareTag("beforeFinish")))
        {
            rb.isKinematic = _collider.isTrigger = true;
            pathSpeed = startPathSpeed;

            airEffectMain.simulationSpeed = 5f;

            dustEffect.GetComponent<Renderer>().material = gameObject.GetComponent<Renderer>().material;
            dustEffect.transform.position = other.contacts[0].point;
            dustEffect.Play();
            BallTrail.Play();
        }
        if (other.collider.CompareTag("finish"))
        {
            print("Finishhh....");
            StartCoroutine(SlowDown());

            BallTrail.Stop();
            airEffect.Stop();
            dustEffect.Stop();
        }
    }

    IEnumerator SlowDown()
    {
        float lastScore = ScoreManager.instance.score;
        while (true)
        {
            DOTween.To(x => pathSpeed = x, pathSpeed, 0, lastScore / 10f)
            .SetEase(Ease.OutQuad);

            DOTween.To(x => ScoreManager.instance.score = x, ScoreManager.instance.score, 0, lastScore / 10f)
            .SetEase(Ease.OutQuad)
            .OnUpdate(UIManager.UIManagerInstance.UpdateSlider);

            print("pathspeed : " + pathSpeed);
            if (pathSpeed <= 0.1)
            {
                print("dongu kirildi");
                break;
            }
            yield return new WaitForSeconds(.016f);
        }
    }

    IEnumerator GoToMid()
    {
        while (true)
        {
            if (0.03 >= ball.position.x & ball.position.x >= -0.03)
            {
                break;
            }
            else
            {
                ball.position = new Vector3(Mathf.SmoothDamp(ball.position.x, 0, ref camVelocity_x, maxSpeed), ball.position.y, ball.position.z);
            }
            yield return new WaitForSeconds(.048f);
        }
    }
}
// }
