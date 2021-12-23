using System.Collections;
using UnityEngine;

public class AIRacketBehaviour : MonoBehaviour, IRocket
{
    /**
      * This code will control the opponent racket object in Scene_Game_Single.
     */

    private SingleCompetitionManager m_competitionManager;
    private Transform m_transform;

    [SerializeField] private Transform ballHoldTransfrom;
    [SerializeField] private bool hasBall;

    private IEnumerator m_moveAndShootRoutine;

    private void Start()
    {
        Init();
    }

    private void Update()
    {
        if (!m_competitionManager.GameStarted)
            return;

        if (hasBall)
            StartPlaying();
        else
            CatchTheBall();
    }

    // Follow the ball until catch it.
    private void CatchTheBall()
    {
        Vector3 _ballVelocity = BallBehaviour.BallRigidBody.velocity;
        Vector3 _ballPosition = BallBehaviour.BallTransform.position;

        if (BallBehaviour.BallOutOfScope())
            return;

        if (_ballVelocity.y > 0) // Activate if the ball coming up.
        {
            float _ballX = _ballPosition.x;
            float _thisX = m_transform.position.x;
            float _dist = Mathf.Abs(_thisX - _ballX); // Distance from the ball.
            Vector2 _newPos = m_transform.position;
            float _maxRocketX = SingleCompetitionManager.MaxRacketX;
            float _accuracy = SingleCompetitionManager.AIAccuracy;

            if (_dist <= 0.02f) return; // Return if close enough.

            if (_thisX > _ballX)
                _newPos += Vector2.left * Time.deltaTime * _accuracy * Mathf.Clamp01(_dist) * 2f;
            else if(_thisX < _ballX)
                _newPos += Vector2.right * Time.deltaTime * _accuracy * Mathf.Clamp01(_dist) * 2f;

            _newPos.x = Mathf.Clamp(_newPos.x, -_maxRocketX, _maxRocketX);
            m_transform.position = _newPos;
        }
    }

    // Special shoot method for AI racket.
    public void Shoot()
    {
        Vector2 _shootDir = Vector2.zero;
        float _maxRokcetX = SingleCompetitionManager.MaxRacketX;
        float _distFromLeft = Mathf.Abs(m_transform.position.x - (-_maxRokcetX));
        float _distFromRight = Mathf.Abs((_maxRokcetX) - m_transform.position.x);

        _shootDir = Vector2.down + (_distFromLeft > _distFromRight ? Vector2.left : Vector2.right);
        _shootDir.x *= Random.Range(0.7f, 1.2f);
        _shootDir.y *= BallBehaviour.YVelocity;

        BallBehaviour.ShootBall(_shootDir);
        hasBall = false;
    }

    // This method will set the ball owner as AI racket.
    public void GrabBall()
    {
        BallBehaviour.GrabBall(ballHoldTransfrom);
        hasBall = true;
    }

    private void StartPlaying()
    {
        if (BallBehaviour.OwnerTransform != ballHoldTransfrom) return;

        if (m_moveAndShootRoutine == null)
            m_moveAndShootRoutine = MoveAndShoot();
        else
            return;

        StartCoroutine(m_moveAndShootRoutine);
    }

    // This Coroutine will be executed when the AI racket has the ball.
    // The racket will move to a random position and then shoot towards the player.
    IEnumerator MoveAndShoot()
    {
        float _percent = 0f;
        float _speed = 1f;
        float _maxRocketX = SingleCompetitionManager.MaxRacketX;
        Vector2 _currentPos = m_transform.position;
        Vector2 _targetShootPos = _currentPos;
        _targetShootPos.x = Random.Range(-_maxRocketX, _maxRocketX);
        Vector2 _newPos = _currentPos;


        while (_percent < 1)
        {
            _percent += _speed * Time.deltaTime;
            _newPos.x = Mathf.Lerp(_currentPos.x, _targetShootPos.x, _percent);
            m_transform.position = _newPos;
            yield return null;
        }

        Shoot();
        m_moveAndShootRoutine = null;
    }

    // Initialize references.
    private void Init()
    {
        m_competitionManager = GameObject.FindObjectOfType<SingleCompetitionManager>();
        m_transform = this.GetComponent<Transform>();
    }
}
