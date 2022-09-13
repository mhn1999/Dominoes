using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Linq;
using System;
namespace Dominoes
{
    internal class TurnManager : State
    {
        private List<DominoScript> playerScriptList;
        private List<DominoScript> enemyScriptList;
        public delegate void HandDominoes(DominoScript dominoScript);
        public static HandDominoes MoveCloser;
        public static bool eventAdder = true;
        public static float dominoWidth = 30.29f;
        public static float dominoHeight = 60.26f;
        public float extraCardsMoveTime = 1f;
        public static float dominoScale = 2.5f;
        public float handCardThreshold = 366.39f;
        public static TurnManager turn;
        public override IEnumerator Start()
        {

            Initialization();
            if (TurnConditions(true))
                yield break;
            SelfClickEnabled();
            OtherClickDisabled();
            ChangingTurn();
            CheckingFirstTurn();
            yield return 0;
        }
        private void OtherClickDisabled()
        {
            for (int count = 0; count < enemyScriptList.Count; count++)
            {
                try
                {
                    enemyScriptList[count].gameObject.GetComponent<BoxCollider2D>().enabled = false;
                }
                catch (Exception e)
                {
                    continue;
                }

            }
        }
        private void ChangingTurn() => GameManager.instance.turn = !GameManager.instance.turn;
        private void CheckingFirstTurn()
        {

            if (eventAdder)
            {
                MoveCloser += MoveDominoesCloser;
                eventAdder = false;
            }
        }
    private void Initialization()
        {
            if (turn == null);
                turn = new TurnManager();
            if (GameManager.instance.turn)
            {
                playerScriptList = GameManager.instance.playerDominoes;
                enemyScriptList = GameManager.instance.enemyDominoes;
            }
            else
            {
                playerScriptList = GameManager.instance.enemyDominoes;
                enemyScriptList = GameManager.instance.playerDominoes;
            }
        }
        private void SelfClickEnabled()
        {
            for (int count = 0; count < playerScriptList.Count; count++)
                try
                {
                    playerScriptList[count].gameObject.GetComponent<BoxCollider2D>().enabled = true;
                }
                catch(Exception e)
                {
                    continue;
                }
        }
        private bool PlayerHandsNoMatchCheck(List<DominoScript> PlayerDominoes= null)
        {
            if(PlayerDominoes == null)
            {
                PlayerDominoes = playerScriptList;
            }
            bool flag = false;
            for(int count=0;count<PlayerDominoes.Count;count++)
            {
                if (PlayerDominoes[count].topNumber == GamePresentor.topNumber || PlayerDominoes[count].topNumber == GamePresentor.botNumber
                    || PlayerDominoes[count].downNumber == GamePresentor.topNumber || PlayerDominoes[count].downNumber == GamePresentor.botNumber)
                {
                    flag=true;
                    break;
                }
            }
            return flag;

        }
        private void ExtraCardBringer(bool setActive)
        {
            GameObject extraCardPanel = GameManager.instance.ExtraCards;
            
            if (setActive)
            {
                extraCardPanel.transform.DOLocalMove(GameManager.instance.PlayPanel.transform.localPosition, extraCardsMoveTime);
                extraCardPanel.SetActive(setActive);
            }
            else
            {
                extraCardPanel.transform.DOLocalMove(GameManager.instance.extraCardsStratingPosition, extraCardsMoveTime);
                GameManager.instance.Wait1SecThanDo();
            }
        }


        private bool ExtraCardsNOMachCech()
        {
            bool flag = (GameManager.instance.extraDominoes.Count == 0) ? false : true;
            if (flag)
                foreach(DominoScript domino in GameManager.instance.extraDominoes)
                {
                    if (domino.topNumber == GamePresentor.topNumber || domino.topNumber == GamePresentor.botNumber
                        || domino.downNumber == GamePresentor.topNumber || domino.downNumber == GamePresentor.botNumber)
                    {
                        flag = true;
                        break;
                    }
                }
            return flag;
        }
        public void MoveDominoesCloser(DominoScript dominoScript)
        {
            loadDominoList();
            playerScriptList.Remove(dominoScript);
            float spaceToMove = dominoWidth * dominoScale / 2;
            foreach (DominoScript domino in playerScriptList)
            {
                    Vector3 destination = domino.transform.localPosition;
                    if (destination.x > dominoScript.savedPosition.x)
                    {
                    destination.x -= spaceToMove;
                        domino.transform.DOLocalMove(destination, GameManager.instance.GameConfig.moveAndRotateSpeed);
                    }
                    else if (destination.x < dominoScript.savedPosition.x)
                    {
                        destination.x += spaceToMove;
                        domino.transform.DOLocalMove(destination, GameManager.instance.GameConfig.moveAndRotateSpeed);
                    }
            }

            SaveDominoList();

            GameManager.instance.SetState(new TurnManager());
        }
        public void MoveDominoesCloser1(DominoScript dominoScript)
        {
            loadDominoList();
            int index = playerScriptList.IndexOf(dominoScript);
            float spaceToMove = dominoWidth * dominoScale/2;
            for(int count=0; count<index;count++)
            {
                Vector3 destination = playerScriptList[count].transform.localPosition;
                destination = new Vector3(destination.x + spaceToMove, destination.y, 0);
                playerScriptList[count].transform.DOLocalMove(destination, GameManager.instance.GameConfig.moveAndRotateSpeed);
            }
            for (int count = index+1; count < playerScriptList.Count; count++)
            {
                Vector3 destination = playerScriptList[count].transform.localPosition;
                destination = new Vector3(destination.x - spaceToMove, destination.y, 0);
                playerScriptList[count].transform.DOLocalMove(destination, GameManager.instance.GameConfig.moveAndRotateSpeed);
            }
            playerScriptList.Remove(dominoScript);
            SaveDominoList();

            GameManager.instance.SetState(new TurnManager());
        }
        private void SaveDominoList()
        {
            if(GameManager.instance.turn)
            {
                GameManager.instance.playerDominoes = enemyScriptList;
                GameManager.instance.enemyDominoes = playerScriptList;
            }
            else
            {
                GameManager.instance.playerDominoes = playerScriptList;
                GameManager.instance.enemyDominoes = enemyScriptList;
            }
        }
        private void loadDominoList()
        {
            if (GameManager.instance.turn)
            {
                playerScriptList = GameManager.instance.enemyDominoes;
                enemyScriptList = GameManager.instance.playerDominoes;
            }
            else
            {
                playerScriptList = GameManager.instance.playerDominoes;
                enemyScriptList = GameManager.instance.enemyDominoes;

            }
        }
        public static void ExtraDominoToHand(DominoScript dominoScript)
        {
            bool flag=false;
            float scale;
            Vector3 destination = Vector3.zero;
            float max = GameManager.instance.enemyDominoes.First().transform.localPosition.x;
            float min = GameManager.instance.enemyDominoes.First().transform.localPosition.x;
            if (GameManager.instance.turn)
            {
                for(int count=0;count<GameManager.instance.enemyDominoes.Count;count++)
                {
                    if (GameManager.instance.enemyDominoes[count].transform.localPosition.x>max)
                    {
                        max = GameManager.instance.enemyDominoes[count].transform.localPosition.x;
                    }
                    if (GameManager.instance.enemyDominoes[count].transform.localPosition.x <min)
                    {
                        min = GameManager.instance.enemyDominoes[count].transform.localPosition.x;
                    }
                }
                CheckZoom(min, max,ref GameManager.instance.enemyChangeablePanelZoomLevel,false);
                //max *= GameManager.instance.playerChangeablePanelZoomLevel;
               // min *= GameManager.instance.playerChangeablePanelZoomLevel;
                destination.y = GameManager.instance.enemyDominoes.First().transform.localPosition.y;
                if (Math.Abs(min) >= Math.Abs(max))
                {
                    destination.x = max;
                    flag = true;
                }
                else if(Math.Abs(min) <= Math.Abs(max))
                {
                    destination.x = min;
                    flag = false;
                }
                scale = GameManager.instance.enemyChangeablePanelZoomLevel;
                GameManager.instance.enemyDominoes.Add(dominoScript);
            }
            else
            {
                for (int count = 0; count < GameManager.instance.playerDominoes.Count; count++)
                {
                    if (GameManager.instance.playerDominoes[count].transform.localPosition.x > max)
                    {
                        max = GameManager.instance.playerDominoes[count].transform.localPosition.x;
                    }
                    if (GameManager.instance.playerDominoes[count].transform.localPosition.x < min)
                    {
                        min = GameManager.instance.playerDominoes[count].transform.localPosition.x;
                    }
                }
                CheckZoom(min, max,ref GameManager.instance.playerChangeablePanelZoomLevel,true);
                //max *= GameManager.instance.playerChangeablePanelZoomLevel;
                //min *= GameManager.instance.playerChangeablePanelZoomLevel;
                destination.y= GameManager.instance.playerDominoes.First().transform.localPosition.y;
                if (Math.Abs(min) >= Math.Abs(max))
                {
                    destination.x = max;
                    flag = true;
                }
                else if(Math.Abs(min) <= Math.Abs(max))
                {
                    destination.x = min;
                    flag = false;
                }
                scale = GameManager.instance.playerChangeablePanelZoomLevel;
                GameManager.instance.playerDominoes.Add(dominoScript);
            }
            GameManager.instance.extraDominoes.Remove(dominoScript);
             float preset=0;
            if (min <= 0 && max <= 0)
                preset = dominoWidth;
            else if (min >= 0 && max >= 0)
                preset = -dominoWidth;
            else if (min <= 0 && max >= 0 && flag)
                preset = dominoWidth;
            else if (min <= 0 && max >= 0 && !flag)
                preset = -dominoWidth;
            destination.x += preset*dominoScale*scale;
            dominoScript.transform.DOLocalMove(destination, GameManager.instance.GameConfig.moveAndRotateSpeed);
            TurnConditions(false);

        }
        private static bool TurnConditions(bool lastturn)
        {
            if (GameManager.instance.enemyDominoes.Count == 0 || GameManager.instance.playerDominoes.Count == 0)
            {
                GameManager.instance.SetState(new EndGame());
                return true;
            }
            bool PlayerHandNoMach, ExtraCardsNoMach,enemyHandNoMatch;
            ExtraCardsNoMach = turn.ExtraCardsNOMachCech();
            bool thisTun = (lastturn) ? GameManager.instance.turn : !GameManager.instance.turn;
            PlayerHandNoMach = (thisTun) ? turn.PlayerHandsNoMatchCheck(GameManager.instance.playerDominoes) : turn.PlayerHandsNoMatchCheck(GameManager.instance.enemyDominoes);
            enemyHandNoMatch = (thisTun) ? turn.PlayerHandsNoMatchCheck(GameManager.instance.enemyDominoes): turn.PlayerHandsNoMatchCheck(GameManager.instance.playerDominoes) ;

            if (!eventAdder)
            {
                if (!PlayerHandNoMach && !ExtraCardsNoMach)
                {
                    if(!enemyHandNoMatch)
                        GameManager.instance.SetState(new EndGame());
                    else
                        GameManager.instance.SetState(new TurnManager());
                    return true;
                }
                else if (!PlayerHandNoMach && ExtraCardsNoMach)
                    turn.ExtraCardBringer(true);
                else
                    turn.ExtraCardBringer(false);

            }
            return false;
        }
        private static void CheckZoom(float min,float max,ref float zoomLevel,bool flag)
        {
            float bigest = (Math.Abs(max) > Math.Abs(min)) ? max : min;
            if (Math.Abs(bigest) + dominoWidth * dominoScale * zoomLevel > turn.handCardThreshold || Math.Abs(bigest) + dominoWidth * dominoScale * zoomLevel > turn.handCardThreshold)
                zoomLevel *= turn.handCardThreshold / (Math.Abs(bigest) + dominoWidth * dominoScale * zoomLevel);
            else
                zoomLevel = 1f;
            Vector3 scale = Vector3.one;
            scale.x = zoomLevel;
            if (flag)
                GameManager.instance.PlayerChangablePanel.transform.DOScale(scale, GameManager.instance.GameConfig.moveAndRotateSpeed);
            else
                GameManager.instance.EnemyChangablePanel.transform.DOScale(scale, GameManager.instance.GameConfig.moveAndRotateSpeed);
        }
    }
}