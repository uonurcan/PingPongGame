using UnityEngine;

public class BallBehaviour : MonoBehaviour
{
    /**
      * This code will control the ball object in Scene_Game_Single.
      */
    
    private static Rigidbody2D m_ballRigidBody;
    private static Animator m_anim;
    private static Transform m_ballTransform;
    private static Transform m_ownerTransform = null;
    private static float m_yVelocity = 5f;

    public static Rigidbody2D BallRigidBody { get { return m_ballRigidBody; } }
    public static Transform BallTransform { get { return m_ballTransform; } }
    public static Transform OwnerTransform { get { return m_ownerTransform; } }
    public static float YVelocity { get { return m_yVelocity; } }

    private SingleCompetitionManager m_competitionManager;

    [SerializeField] private GameObject kickEffect;

    private GameObject m_lastObject;
    
    private void Awake()
    {
        Init();
    }

    private void Update()
    {
        // If the ball has been owned by a player, chase its racket.
        if(m_ownerTransform != null && !m_competitionManager.GameEnded)
        {
            this.transform.position = Vector2.Lerp(this.transform.position, m_ownerTransform.position, Time.deltaTime * 10f);
        }
    }

    // If player hit an object (in out case, a racket).
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Return if the last object the ball hit, and the current object it hit were the same (for preventing some bugs).
        if (m_lastObject == collision.collider.gameObject)
            return;

        m_lastObject = collision.collider.gameObject; // Set last object immediately.
        Instantiate(kickEffect, collision.GetContact(0).point, Quaternion.identity); // Instantiate kick effect.

        // Increase ball speed with a magnitude of 0.5 every time the ball hit something,
        // Until it reaches the max amount specified in the game setting.
        m_yVelocity = Mathf.Min(m_yVelocity + 0.5f, SingleCompetitionManager.CurrentSetting.MaxBallSpeed);

        // Without a doubt the object that the ball hit is racket,
        // So access the racket and execute rackets shoot method.
        IRocket _racket = collision.gameObject.GetComponent<IRocket>();
        _racket.Shoot();
    }

    public static void ShootBall(Vector2 shootVector)
    {
        ReleaseBall(); // First release the ball.

        if (!m_ballRigidBody.simulated)
            m_ballRigidBody.simulated = true; // Enable physic simulation on the ball.

        m_ballRigidBody.velocity = shootVector; // Then shoot the ball.
    }

    // Assign the ball an owner.
    public static void GrabBall(Transform ownerT)
    {
        m_ownerTransform = ownerT;
    }

    public static void StopAndHideBall()
    {
        m_ballRigidBody.simulated = false;
        m_ballRigidBody.velocity = Vector2.zero;
        m_anim.Play("Ball_Disappear");
    }

    public void ShowBall()
    {
        m_anim.Play("Ball_Appear");
    }

    // Change turn and make the last thing that the ball hit null.
    public void RespawnBall()
    {
        m_competitionManager.ChangeTurn();

        if (!m_competitionManager.GameStarted)
            return;

        ShowBall();
        m_lastObject = null;
    }

    // If the ball is owned by a racket and following it, release the ball by making 'm_ownerTransform' null.
    public static void ReleaseBall()
    {
        m_ownerTransform = null;
    }

    // Check if the ball is out of playing scope,
    // So the AI racket can stop chasing it.
    public static bool BallOutOfScope()
    {
        Vector2 _ballPosition = m_ballTransform.position;
        Vector2 _gameBounds = SingleCompetitionManager.GetGameBounds();
        float _ballXPos = Mathf.Abs(_ballPosition.x);
        float _ballYPos = Mathf.Abs(_ballPosition.y);

        if (_ballXPos > _gameBounds.x || _ballYPos > _gameBounds.y)
            return true;
        else
            return false;
    }

    // Initialize references.
    private void Init()
    {
        m_ballRigidBody = this.GetComponent<Rigidbody2D>();
        m_anim = this.GetComponent<Animator>();
        m_ballTransform = this.GetComponent<Transform>();
        m_competitionManager = GameObject.FindObjectOfType<SingleCompetitionManager>();
    }
}
