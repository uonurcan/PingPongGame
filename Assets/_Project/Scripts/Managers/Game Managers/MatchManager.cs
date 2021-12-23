using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;

public class MatchManager : NetworkBehaviour
{
    /**
      * This is a manager script.
      * Manager scripts control the general flow of a scene.
      * .... 
      * MatchManager:
      * This script will manage the Scene_Game_Multi.
      */

    public Ball Ball;
    public PlayerManager PlayerOne;
    public PlayerManager PlayerTwo;
    public PlayerManager StarterPlayer;

    [SerializeField] private GameSetting gameSetting;
    public GameSetting GameSetting { get { return gameSetting; } }
    
    [ServerCallback]
    public void PlayerScored(PlayerNo playerNo)
    {
        PlayerManager _winner = null;
        PlayerManager _looser = null;

        if (playerNo == PlayerNo.PlayerOne)
        {
            int _newScore = PlayerOne.PlayerScore + 1;
            PlayerOne.PlayerScore = _newScore;
            PlayerTwo.SetOpponentScore(_newScore);

            if (_newScore == gameSetting.MaxScoreToWin)
            {
                _winner = PlayerOne;
                _looser = PlayerTwo;
            }
        }
        else
        {
            int _newScore = PlayerTwo.PlayerScore + 1;
            PlayerTwo.PlayerScore = _newScore;
            PlayerOne.SetOpponentScore(_newScore);

            if (_newScore == gameSetting.MaxScoreToWin)
            {
                _winner = PlayerTwo;
                _looser = PlayerOne;
            }
        }

        if (!_winner && !_looser)
            ChangeTurn();
        else
        {
            PlayersCanMove(false);

            Ball.StopBall();
            Ball.HideBall();

            _winner.PlayerWon();
            _looser.PlayerLost();
        }
    }

    [ServerCallback]
    private void ChangeTurn()
    {
        PlayerManager _nextPlayer = (StarterPlayer == PlayerOne ? PlayerTwo : PlayerOne);
        StartCoroutine(ChangeTurnRoutine(_nextPlayer));
    }

    [ServerCallback]
    private IEnumerator ChangeTurnRoutine(PlayerManager nextPlayer)
    {
        Ball.StopBall();
        Ball.HideBall();
        yield return new WaitForSeconds(1f);
        AssignBallToPlayer(nextPlayer);
        Ball.ShowBall();
    }

    [ServerCallback]
    public void SetupAndStartMatch()
    {
        StartCoroutine(SetupAndStartRoutine());
    }

    [ServerCallback]
    IEnumerator SetupAndStartRoutine()
    {
        List<GameObject> _players = GameObject.FindGameObjectsWithTag("Racket").ToList();

        foreach (GameObject player in _players)
        {
            PlayerManager _playerManager = player.GetComponent<PlayerManager>();

            if (_playerManager.PlayerNo == PlayerNo.PlayerOne)
                PlayerOne = _playerManager;
            else if (_playerManager.PlayerNo == PlayerNo.PlayerTwo)
                PlayerTwo = _playerManager;
        }

        PlayerOne.SetOpponentInfo(PlayerTwo.UserID);
        PlayerTwo.SetOpponentInfo(PlayerOne.UserID);

        PlayerOne.SetOpponentRacket(PlayerTwo.UserID, PlayerTwo.gameObject);
        PlayerTwo.SetOpponentRacket(PlayerOne.UserID, PlayerOne.gameObject);

        Ball = GameObject.FindObjectOfType<Ball>();
        Ball.ShowBall();
        yield return new WaitForSeconds(1f);
        AssignBallToPlayer(PlayerOne);
        yield return new WaitForSeconds(0.2f);
        PlayersCanMove(true);
    }

    [ServerCallback]
    private void PlayersCanMove(bool state)
    {
        PlayerOne.CanMove = state;
        PlayerTwo.CanMove = state;
    }

    [ServerCallback]
    private void AssignBallToPlayer(PlayerManager player)
    {
        Ball.SetOwner(player.BallHolder);
        player.HasBall = true;
    }
}
