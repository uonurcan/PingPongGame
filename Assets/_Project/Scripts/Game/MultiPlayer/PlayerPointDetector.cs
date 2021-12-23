using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerPointDetector : NetworkBehaviour
{
    [SerializeField] private PlayerNo playerNo;

    [ServerCallback]
    private void OnTriggerEnter2D(Collider2D collision)
    {
        MatchManager m_matchManager = GameObject.FindObjectOfType<MatchManager>();
        m_matchManager.PlayerScored(playerNo);
    }
}
