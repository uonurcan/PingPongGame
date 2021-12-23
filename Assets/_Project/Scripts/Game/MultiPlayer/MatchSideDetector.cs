using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MatchSideDetector : NetworkBehaviour
{
    [ServerCallback]
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Ball _ball = collision.GetComponent<Ball>();
        MatchManager m_matchManager = GameObject.FindObjectOfType<MatchManager>();

        float _yVelocity = _ball.YVelocity;
        if(_yVelocity > 0)
            m_matchManager.PlayerScored(PlayerNo.PlayerTwo);
        else
            m_matchManager.PlayerScored(PlayerNo.PlayerOne);
    }
}
