using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Linq;

namespace Dominoes
{
    internal class Initialization : State
    {
        public override IEnumerator Start()
        {
            InitializeDominoPrefabs();
            yield return PlayerDominoDistribution(GameManager.instance.playerDominoes);
            yield return PlayerDominoDistribution(GameManager.instance.enemyDominoes);
            yield return MoveDominoesToPlayerHand();
            yield return ExtraCardInitializer();
            yield return SetTurn();
        }
        private void InitializeDominoPrefabs()
        {
            for (int i = 0; i < 7; i++)
                for (int j = 0; j < 7; j++)
                {
                    GameObject newDomino = GameObject.Instantiate(GameManager.instance.DominoPrefab, GameManager.instance.PlayGround.transform);
                    DominoScript dominoScript = newDomino.GetComponent<DominoScript>();

                    dominoScript.verticalImage = GameManager.instance.DominoesData.imagesVertical[7 * i + j];
                    dominoScript.horizantalImage = GameManager.instance.DominoesData.imagesHorizantal[0];

                    dominoScript.topNumber = j;
                    dominoScript.downNumber = i;
                    GameManager.instance.stashDominoes.Add(dominoScript);
                    // float[] pos= DominoesCellPositions(playPanelRect.rect.width, playPanelRect.rect.height,padding:10,culomn:7,row:7);
                    //newDomino.transform.localPosition = new Vector3(pos[0] * j, pos[1] * i, 0);
                }
        }
        private int PlayerDominoDistribution(List<DominoScript> distribuitedDominoes)
        {
            int max = GameManager.instance.GameConfig.numberOfDominoes - 1;
            for (int count = 0; count < GameManager.instance.GameConfig.numberOfFirstHandDominoes; count++)
            {
                int dominoToAdd = Random.Range(0, max);
                while (GameManager.instance.trackDominoes.Contains(dominoToAdd))
                {

                    dominoToAdd = Random.Range(0, max);
                }
                max--;
                distribuitedDominoes.Add(GameManager.instance.stashDominoes[dominoToAdd].GetComponent<DominoScript>());
                GameManager.instance.trackDominoes.Add(dominoToAdd);
            }
            return 0;

        }
        private int MoveDominoesToPlayerHand()
        {
            RectTransform playerRect = GameManager.instance.Player.GetComponent<RectTransform>();
            RectTransform enemyRect = GameManager.instance.Enemy.GetComponent<RectTransform>();
            RectTransform dominoRect = GameManager.instance.DominoPrefab.GetComponent<RectTransform>();
            float[] playerPreset = { playerRect.localPosition.x - playerRect.rect.width / 2, playerRect.localPosition.y - playerRect.rect.height / 2 };
            float[] enemyPreset = { enemyRect.localPosition.x - enemyRect.rect.width / 2, enemyRect.localPosition.y - enemyRect.rect.height / 2 };

            for (int count = 1; count <= GameManager.instance.GameConfig.numberOfFirstHandDominoes; count++)
            {
                float[] playerDominoesHandDestination = DominoesCellPositions(playerRect.rect.width, playerRect.rect.height, dominoWidth: dominoRect.rect.width, culomn: (GameManager.instance.GameConfig.numberOfFirstHandDominoes + 1), row: 2);
                float[] enemyDominoesHandDestination = DominoesCellPositions(enemyRect.rect.width, enemyRect.rect.height, dominoWidth: dominoRect.rect.width, culomn: (GameManager.instance.GameConfig.numberOfFirstHandDominoes + 1), row: 2);
                GameManager.instance.playerDominoStations.Add(new Vector3(playerPreset[0] + playerDominoesHandDestination[0] + dominoRect.rect.width * (count-0.5f) * GameManager.instance.DominoPrefab.transform.localScale.x, playerDominoesHandDestination[1] + playerPreset[1], 0));
                GameManager.instance.enemyDominoStations.Add(new Vector3(enemyPreset[0] + enemyDominoesHandDestination[0] + dominoRect.rect.width * (count-0.5f) * GameManager.instance.DominoPrefab.transform.localScale.x, enemyDominoesHandDestination[1] + enemyPreset[1], 0));
                GameManager.instance.playerDominoes[count - 1].transform.SetParent(GameManager.instance.PlayerChangablePanel.transform);
                GameManager.instance.playerDominoes[count - 1].transform.DOLocalMove(GameManager.instance.playerDominoStations[count - 1], GameManager.instance.GameConfig.moveAndRotateSpeed);
                GameManager.instance.enemyDominoes[count - 1].transform.SetParent(GameManager.instance.EnemyChangablePanel.transform);
                GameManager.instance.enemyDominoes[count - 1].transform.DOLocalMove(GameManager.instance.enemyDominoStations[count - 1], GameManager.instance.GameConfig.moveAndRotateSpeed);
            }
            return 0;

        }
        private float[] DominoesCellPositions(float width, float height, float dominoWidth, int culomn, int row)
        {
            float[] pos = { (width - GameManager.instance.GameConfig.numberOfFirstHandDominoes * dominoWidth * GameManager.instance.DominoPrefab.transform.localScale.x) / 2, (height) / row };
            return pos;
        }
        private int SetTurn()
        {
            int playerBigest = FindBigestDomino(GameManager.instance.playerDominoes);
            int enemyBigest = FindBigestDomino(GameManager.instance.enemyDominoes);
            if (playerBigest <= enemyBigest)
                GameManager.instance.turn = false;
            else
                GameManager.instance.turn = true;
            GameManager.instance.SetState(new TurnManager());

            return 0;

        }
        private int FindBigestDomino(List<DominoScript> dominoeList)
        {
            int bigest = 0;
            int number = -1;
            for (int count=0;count<dominoeList.Count;count++)
            {
                if (dominoeList[count].topNumber == dominoeList[count].downNumber)
                    number = (dominoeList[count].topNumber + dominoeList[count].downNumber + 13);
                else
                    number = dominoeList[count].topNumber + dominoeList[count].downNumber;
                if (bigest < number)
                    bigest = number;
            }
            return bigest;
        }
        private int ExtraCardInitializer()
        {
            for (int count = 0; count < GameManager.instance.GameConfig.numberOfExtraDominoes; count++)
            {
                int dominoToAdd = Random.Range(0, GameManager.instance.GameConfig.numberOfDominoes - 1);
                while (GameManager.instance.trackDominoes.Contains(dominoToAdd))
                    dominoToAdd= Random.Range(0, GameManager.instance.GameConfig.numberOfDominoes - 1);
                GameManager.instance.trackDominoes.Add(dominoToAdd);
                GameManager.instance.stashDominoes[dominoToAdd].transform.SetParent(GameManager.instance.ExtraCards.transform);
                GameManager.instance.stashDominoes[dominoToAdd].ChangePicture();
                GameManager.instance.extraDominoes.Add(GameManager.instance.stashDominoes[dominoToAdd].GetComponent<DominoScript>());


            }
            return 0;
        }
    }
}