using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserPointDetector : MonoBehaviour
{
    /**
      * This code will detect if the ball entered the opponent's score zone.
      */
      
    SingleCompetitionManager m_competitionManager;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        BallBehaviour.StopAndHideBall();
        m_competitionManager = GameObject.FindObjectOfType<SingleCompetitionManager>();
        m_competitionManager.UserScored();
    }
}
