using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class RacketMovement : NetworkBehaviour
{
    private PlayerManager m_playerManager;
    private Transform m_transform;
    private Camera m_camera;

    [SerializeField] private bool smoothMove;

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        Init();
    }

    private void Update()
    {
        if (isLocalPlayer && m_playerManager.CanMove)
            MoveRacket();
    }

    private void MoveRacket()
    {
        if (Input.GetMouseButton(0))
        {
            Vector2 _mousePos = m_camera.ScreenToWorldPoint(Input.mousePosition);
            Vector2 _racketPosX = m_transform.position;

            float _newX = _mousePos.x;
            _newX = Mathf.Clamp(_mousePos.x, -1.7f, 1.7f);

            if (!smoothMove)
                _racketPosX.x = _newX;
            else
                _racketPosX.x = Mathf.Lerp(_racketPosX.x, _newX, Time.deltaTime * 10f);

            m_transform.position = _racketPosX;
        }
    }

    private void Init()
    {
        m_playerManager = this.GetComponent<PlayerManager>();
        m_transform = this.GetComponent<Transform>();
        m_camera = Camera.main;
    }
}
