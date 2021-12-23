using UnityEngine;

[CreateAssetMenu(fileName = "New Rocket", menuName = "GameSetting")]
public class GameSetting : ScriptableObject
{
    /**
      * GameSetting is a ScriptableObject for storing game modes info as an in-project file.
     */

    [SerializeField] private int maxScoreToWin;
    [SerializeField] private float maxBallSpeed;
    [Header("Only for single player modes:")]
    [SerializeField] [Range(0.8f, 2f)] private float aIRocketAccuracy;

    public int MaxScoreToWin { get { return maxScoreToWin; } }
    public float MaxBallSpeed { get { return maxBallSpeed; } }
    public float AIRocketAccuracy { get { return aIRocketAccuracy; } }
}
