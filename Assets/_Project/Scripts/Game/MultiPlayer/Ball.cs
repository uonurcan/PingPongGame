using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Ball : NetworkBehaviour
{
    [SerializeField] private GameObject kickEffect;

    private Rigidbody2D m_rigidbody;
    private Transform m_transform;
    private Animator m_animator;
    private MatchManager m_matchManager;

    private Transform m_ownerTransform;
    public float YVelocity { get { return m_rigidbody.velocity.y; } }

    public override void OnStartServer()
    {
        base.OnStartServer();

        Init();
    }

    [ServerCallback]
    private void Update()
    {
        if(m_ownerTransform != null)
        {
            m_transform.position = Vector3.Lerp(m_transform.position, m_ownerTransform.position, Time.deltaTime * 15f);
        }
    }

    [ServerCallback]
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (m_ownerTransform != null && m_ownerTransform.parent == collision.transform)
            return;

        GameObject _hitEffect = Instantiate(kickEffect, collision.GetContact(0).point, Quaternion.identity);
        NetworkServer.Spawn(_hitEffect);

        float _maxBallSpeed = m_matchManager.GameSetting.MaxBallSpeed;
        float _racketPower = collision.gameObject.GetComponent<PlayerManager>().PlayerRacketPower;

        float _ballX = m_transform.position.x;
        float _ballY = m_transform.position.y;
        float _RacketX = collision.collider.transform.position.x;
        float _RacketY = collision.collider.transform.position.y;
        float _dist = Mathf.Abs(_RacketX - _ballX);

        Vector2 _shootDir = Vector2.up + Vector2.left;
        _shootDir.x *= ((_ballX > _RacketX) ? 1f : -1f) * Mathf.Clamp01(_dist) * (4f * (1f + _racketPower));
        _shootDir.x *= (_ballX == _RacketX) ? 0 : 1f;
        _shootDir.y *= (1f + _racketPower) * _maxBallSpeed * (_ballY > _RacketY ? 1f: -1f);
        ShootBall(_shootDir);
    }

    [ServerCallback]
    public void ShootBall(Vector2 shootDirection)
    {
        ReleaseOwner();
        m_rigidbody.velocity = shootDirection;
    }

    [ServerCallback]
    public void StopBall()
    {
        m_rigidbody.velocity = Vector2.zero;
    }

    [ServerCallback]
    public void SetOwner(Transform ownerTransform)
    {
        m_ownerTransform = ownerTransform;
    }

    [ServerCallback]
    public void ReleaseOwner()
    {
        if (m_ownerTransform)
        {
            MatchManager _matchManager = GameObject.FindObjectOfType<MatchManager>();
            _matchManager.StarterPlayer = m_ownerTransform.parent.GetComponent<PlayerManager>();

            m_ownerTransform = null;
        }
    }

    [ServerCallback]
    public void ShowBall()
    {
        m_animator.Play("Ball_Appear");
        RpcEnableSpriteRenderer();
    }

    [ServerCallback]
    public void HideBall()
    {
        m_animator.Play("Ball_Disappear_Multiplayer");
    }

    [ClientRpc]
    private void RpcEnableSpriteRenderer()
    {
        SpriteRenderer _spriteRenderer = this.GetComponent<SpriteRenderer>();
        _spriteRenderer.enabled = true;
    }

    [ServerCallback]
    private void Init()
    {
        m_rigidbody = this.GetComponent<Rigidbody2D>();
        m_transform = this.transform;
        m_animator = this.GetComponent<Animator>();
        m_matchManager = GameObject.FindObjectOfType<MatchManager>();
        
        m_rigidbody.simulated = true;
    }
}
