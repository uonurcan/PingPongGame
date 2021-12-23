using UnityEngine;

public class OpponentPointDetector : MonoBehaviour
{
    /**
      * This code will detect if the ball entered the opponent's score zone.
      */
      
    SingleCompetitionManager m_competitionManager;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        BallBehaviour.StopAndHideBall();
        m_competitionManager = GameObject.FindObjectOfType<SingleCompetitionManager>();
        m_competitionManager.AIScored();
    }
}
