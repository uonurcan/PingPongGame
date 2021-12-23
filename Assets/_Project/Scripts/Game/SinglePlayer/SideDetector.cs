using UnityEngine;

public class SideDetector : MonoBehaviour
{
    /**
      * This code will detect if the ball gets out of the table from the sides.
      */

    SingleCompetitionManager m_competitionManager;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        m_competitionManager = GameObject.FindObjectOfType<SingleCompetitionManager>();
        BallBehaviour.StopAndHideBall();

        // From the ball's y velocity we will check which way the ball was moving.
        float _yVelocity = BallBehaviour.YVelocity;

        if (_yVelocity > 0)
            m_competitionManager.AIScored();
        else
            m_competitionManager.UserScored();
        // ....
    }
}
