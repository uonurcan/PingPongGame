using UnityEngine;

public class PlayerRacketController : MonoBehaviour, IRocket
{
    /**
      * This code will control the player racket object in Scene_Game_Single.
      */

    private SingleCompetitionManager m_competitionManager;
    private Transform m_transform;
    private Camera m_camera;
    private static Racket m_playerRacket;
    public static Racket PlayerRacket { set { m_playerRacket = value; } }

    [SerializeField] private Transform ballHoldTransfrom;
    [SerializeField] private bool smoothMove;

    private void Start()
    {
        Init();
    }

    private void Update()
    {
        if (!m_competitionManager.GameStarted)
            return;

        if(Input.GetMouseButton(0))
            PlayerRacketMovement();

        if (Input.GetMouseButtonDown(0) 
            && BallBehaviour.OwnerTransform == ballHoldTransfrom
            && Vector2.Distance(BallBehaviour.BallTransform.position, ballHoldTransfrom.position) <= 0.05f)
            Shoot();
    }

    // Movement of player racket with mouse.
    private void PlayerRacketMovement()
    {
        Vector2 _mousePos = m_camera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 _racketPosX = m_transform.position;
        float _maxRocketX = SingleCompetitionManager.MaxRacketX;

        float _newX = _mousePos.x;
        // Clamp new X so racket doesn't get out of the sceen.
        _newX = Mathf.Clamp(_mousePos.x, -_maxRocketX, _maxRocketX);

        if (!smoothMove)
            _racketPosX.x = _newX;
        else
            _racketPosX.x = Mathf.Lerp(_racketPosX.x, _newX, Time.deltaTime * 10f);

        m_transform.position = _racketPosX;
    }

    // This method will set the ball owner as this racket.
    public void GrabBall()
    {
        BallBehaviour.GrabBall(ballHoldTransfrom);
    }
    
    // Special shoot method for player racket.
    public void Shoot()
    {
        float _ballX = BallBehaviour.BallTransform.position.x;
        float _thisX = m_transform.position.x;
        float _dist = Mathf.Abs(_thisX - _ballX);

        Vector2 _shootDir = Vector2.up + Vector2.left;
        _shootDir.x *= ((_ballX > _thisX) ? 1f : -1f) * Mathf.Clamp01(_dist) * (4f * (1f + m_playerRacket.RacketPower));
        _shootDir.x *= (_ballX == _thisX) ? 0 : 1f;
         // Apply racket power to y velocity.
        _shootDir.y *= (1f + m_playerRacket.RacketPower) * (BallBehaviour.YVelocity);

        BallBehaviour.ShootBall(_shootDir);
    }

    // Initialize references.
    private void Init()
    {
        m_competitionManager = GameObject.FindObjectOfType<SingleCompetitionManager>();
        m_transform = this.GetComponent<Transform>();
        m_camera = Camera.main;
    }
}
