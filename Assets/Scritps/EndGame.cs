using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Linq;
using System;
using UnityEngine.SceneManagement;
namespace Dominoes
{
    internal class EndGame : State
    {
        public override IEnumerator Start()
        {
            CheckingWinner();
            MaxScoreWinner();
            SettingFirstMoveTrue();
            yield return new WaitForSeconds(5);
            SceneManager.LoadScene(0);

        }
        void SettingFirstMoveTrue()
        {
            TurnManager.MoveCloser -= TurnManager.turn.MoveDominoesCloser;
            GamePresentor.firstMove = true;
            //TurnManager.eventAdder
        }
        private void EndGameText(string message)
        {
            GameManager.instance.EndingText.gameObject.SetActive(true);
            GameManager.instance.EndingText.text = message;
            GameManager.instance.EndingText.transform.DOShakePosition(1f);
            GameManager.instance.EndingText.transform.DOShakeRotation(1f);
        }
        private void EndingPanel(string message)
        {
            Text endingText = GameManager.instance.EndingPanel.transform.Find("EndingText").GetComponent<Text>();
            Text playerScore= GameManager.instance.EndingPanel.transform.Find("PlayerScore").GetComponent<Text>();
            Text enemyScore= GameManager.instance.EndingPanel.transform.Find("EnemyScore").GetComponent<Text>();
            playerScore.text= PlayerPrefs.GetInt("playerPointsPerGame").ToString();
            enemyScore.text= PlayerPrefs.GetInt("enemyPointsPerGame").ToString();
            endingText.text = message;
            GameManager.instance.EndingPanel.transform.DOLocalMove(GameManager.instance.EndingText.transform.localPosition, 1f);
            PlayerPrefs.DeleteAll();
        }
        private void CheckingWinner()
        {
            int totalPoints = PlayerPrefs.GetInt("playerPointsPerGame")+PointsCalculator(GameManager.instance.enemyDominoes);
            PlayerPrefs.SetInt("playerPointsPerGame", totalPoints);
            GameManager.instance.PlayerScoreText.text = totalPoints.ToString();
            totalPoints = PlayerPrefs.GetInt("enemyPointsPerGame") + PointsCalculator(GameManager.instance.playerDominoes);
            PlayerPrefs.SetInt("enemyPointsPerGame", totalPoints);
            GameManager.instance.EnemyScoreText.text = totalPoints.ToString();

        }
        private int PointsCalculator(List<DominoScript> dominoList)
        {
            int totalPoints = 0;
            foreach (DominoScript domino in dominoList)
            {
                totalPoints += domino.topNumber + domino.downNumber;
            }
            return totalPoints;
        }
        private void MaxScoreWinner()
        {
            string message;
            int enemyScore = PlayerPrefs.GetInt("enemyPointsPerGame");
            int playerScore = PlayerPrefs.GetInt("playerPointsPerGame");
            if (playerScore >= GameManager.instance.GameConfig.maxScore && enemyScore > playerScore)
            {
                message = "Enemy Wins";
                EndingPanel(message);
            }
            else if (enemyScore >= GameManager.instance.GameConfig.maxScore && enemyScore < playerScore)
            {
                message = "Player Wins";
                EndingPanel(message);
            }
            else if (playerScore >= GameManager.instance.GameConfig.maxScore)
            {
                message = "Player Wins";
                EndingPanel(message);

            }
            else if (enemyScore >= GameManager.instance.GameConfig.maxScore)
            {
                message = "Enemy Wins";
                EndingPanel(message);
            }
            else
            {
                message = "End of round";
                EndGameText(message);
            }
        }
    }
}