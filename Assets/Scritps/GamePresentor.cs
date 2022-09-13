using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using DG.Tweening;
namespace Dominoes
{
    internal class GamePresentor : State
    {
        private DominoScript dominoScript;
        private RectTransform dominoRect;
        public static bool firstMove = true;
        public static int topNumber { get; private set; }
        public static int botNumber { get; private set; }
        private static DominoScript topPositionScript;
        private static DominoScript botPositionScript;
        private static DominoScript topPositionScriptPhase2;
        private static DominoScript botPositionScriptPhase2;
        public static int topCounter=0;
        public static int botCounter=0;

        private int direction=0;
        private float dominoLocalScale;
        public override IEnumerator Start()
        {
            Initialization();
            if(GameManager.instance.fieldDominoes.Count<=10)
            {
                PuttingDominoOnFieldPhase1();
                topPositionScriptPhase2 = topPositionScript;
                botPositionScriptPhase2 = botPositionScript;
            }
            else
                PuttingDominoOnFieldPhase1(2);
            yield return 0;
        }
        private void Initialization()
        {
            GameObject PutOnFieldDomino = GameManager.instance.fieldDominoes.Last();
            dominoRect = PutOnFieldDomino.GetComponent<RectTransform>();
            dominoScript = PutOnFieldDomino.GetComponent<DominoScript>();
            dominoLocalScale = dominoScript.transform.localScale.x;
        }
        private void PuttingDominoOnFieldPhase1(int phase=1)
        {

            if (firstMove)
            {
                Vector3 dominoScriptPos = dominoScript.transform.localPosition;
                FirstDominoToPutPhase1();
                TurnManager.MoveCloser?.Invoke(dominoScript);

            }
            else
            {
                Vector3 dominoScriptPos = dominoScript.transform.localPosition;
                DominoScript destinationDominoScript = FindingCorrectLastDomino(dominoScript);
                if (destinationDominoScript != null)
                {
                    
                    if (dominoScript.topNumber != dominoScript.downNumber)
                    {
                        CheckingToZoom(destinationDominoScript,phase);
                        if (phase == 1)
                            CheckingDominoValues_Vertical(destinationDominoScript);
                        else
                        {
                            CheckingDominoValues_VerticalPhase2(destinationDominoScript);

                        }
                        dominoScript.transform.DOLocalMove(GameManager.instance.nextPositionInField, GameManager.instance.GameConfig.moveAndRotateSpeed);
                        dominoScript.transform.DOLocalRotate(GameManager.instance.nextRotaionInField, GameManager.instance.GameConfig.moveAndRotateSpeed,RotateMode.FastBeyond360);
                    }
                    else
                    {
                        dominoScript.isVertical = false;
                        CheckingToZoom(destinationDominoScript,phase);
                        if (phase==1)
                            CheckingDominoValues_Horizantal(destinationDominoScript);
                        else
                            CheckingDominoValues_HorizantalPhase2(destinationDominoScript);
                        dominoScript.transform.DOLocalMove(GameManager.instance.nextPositionInField, GameManager.instance.GameConfig.moveAndRotateSpeed);
                        dominoScript.transform.DOLocalRotate(GameManager.instance.nextRotaionInField, GameManager.instance.GameConfig.moveAndRotateSpeed,RotateMode.FastBeyond360);
                    }
                    dominoScript.transform.SetParent(GameManager.instance.ChangablePanel.transform);
                    dominoScript.transform.DOScale(dominoLocalScale, GameManager.instance.GameConfig.moveAndRotateSpeed);
                    GameObject.Destroy(dominoScript.gameObject.GetComponent<BoxCollider2D>());
                    TurnManager.MoveCloser?.Invoke(dominoScript);

                }
                else
                {
                    dominoScript.transform.DOLocalMove(GameManager.instance.nextPositionInField, GameManager.instance.GameConfig.moveAndRotateSpeed);
                    dominoScript.transform.DOLocalRotate(GameManager.instance.nextRotaionInField, GameManager.instance.GameConfig.moveAndRotateSpeed, RotateMode.FastBeyond360);
                    GameManager.instance.fieldDominoes.Remove(dominoScript.gameObject);
                }

            }
        }
        private void FirstDominoToPutPhase1()
        {

            firstMove = false;
            if (dominoScript.topNumber==dominoScript.downNumber)
            {
                dominoScript.transform.DOLocalMove(GameManager.instance.nextPositionInField,GameManager.instance.GameConfig.moveAndRotateSpeed);
                dominoScript.transform.DOLocalRotate(new Vector3(0, 0, 90), GameManager.instance.GameConfig.moveAndRotateSpeed);
                dominoScript.isVertical = false;
            }
            else
            {
                dominoScript.transform.DOLocalMove(GameManager.instance.nextPositionInField, GameManager.instance.GameConfig.moveAndRotateSpeed);
            }
            topNumber = dominoScript.topNumber;
            botNumber = dominoScript.downNumber;
            topPositionScript = dominoScript;
            botPositionScript = dominoScript;
            dominoScript.transform.SetParent(GameManager.instance.ChangablePanel.transform);
            GameObject.Destroy(dominoScript.gameObject.GetComponent<BoxCollider2D>());
        }
        private void CheckingDominoValues_Vertical(DominoScript lastDominoScript,int phase=1)
        {
            float ypreset = (lastDominoScript.isVertical) ? dominoRect.rect.height : (dominoRect.rect.height / 2 + dominoRect.rect.width / 2);
            ypreset *= lastDominoScript.transform.localScale.y;
            float xpreset = 0;
            if (phase == 2)
            {
                ypreset = -ypreset;
                GameManager.instance.nextRotaionInField+= new Vector3(0,0, 180);
            }

            if (direction==1)
            {
                GameManager.instance.nextPositionInField = new Vector3(lastDominoScript.transform.localPosition.x+xpreset, lastDominoScript.transform.localPosition.y + ypreset, 0);
                SwapDominoNumbers(dominoScript);
                topPositionScript = dominoScript;


            }
            else if (direction==-1)
            {
                GameManager.instance.nextPositionInField = new Vector3(lastDominoScript.transform.localPosition.x-xpreset, lastDominoScript.transform.localPosition.y - ypreset, 0);
                botPositionScript = dominoScript;

            }
        }
        private void CheckingDominoValues_VerticalPhase2(DominoScript lastDominoScript)
        {
            if(direction==1 && topCounter==4)
            {
                ChecKingDominoValues_Vertical_Phase3First(lastDominoScript);
            }
            else if (direction == -1 && botCounter == 4)
            {
                ChecKingDominoValues_Vertical_Phase3First(lastDominoScript);
            }
            else if (direction == -1 && botCounter > 4)
            {
                CheckingDominoValues_Vertical(lastDominoScript,2);
            }
            else if (direction == 1 && topCounter > 4)
            {
                CheckingDominoValues_Vertical(lastDominoScript,2);
            }
            else
            {
                float ypreset = 0;
                float xpreset = 0;
                xpreset = (lastDominoScript.isVertical) ? dominoRect.rect.height : (dominoRect.rect.height / 2 + dominoRect.rect.width / 2);
                xpreset *= lastDominoScript.transform.localScale.x;
                if (topPositionScriptPhase2==lastDominoScript || botPositionScriptPhase2 == lastDominoScript)
                {
                    ypreset = (lastDominoScript.isVertical) ? dominoRect.rect.height/4 : 0;
                    ypreset *= lastDominoScript.transform.localScale.y;
                    xpreset = (lastDominoScript.isVertical) ?(dominoRect.rect.height / 2 + dominoRect.rect.width / 2):dominoRect.rect.height;
                    xpreset *= lastDominoScript.transform.localScale.x;

                }
                
                if (direction == 1)
                {
                    topCounter++;
                    GameManager.instance.nextPositionInField = new Vector3(lastDominoScript.transform.localPosition.x+xpreset, lastDominoScript.transform.localPosition.y + ypreset, 0);
                    GameManager.instance.nextRotaionInField -=new Vector3(0, 0, 90);
                    SwapDominoNumbers(dominoScript);
                    topPositionScript = dominoScript;


                }
                else if (direction == -1)
                {
                    botCounter++;
                    GameManager.instance.nextPositionInField = new Vector3(lastDominoScript.transform.localPosition.x - xpreset, lastDominoScript.transform.localPosition.y - ypreset, 0);
                    GameManager.instance.nextRotaionInField -= new Vector3(0, 0, 90);
                    botPositionScript = dominoScript;

                }
            }

        }
        private void ChecKingDominoValues_Vertical_Phase3First(DominoScript lastDominoScript)
        {
            float ypreset = (lastDominoScript.isVertical) ? (dominoRect.rect.height / 2 + dominoRect.rect.width / 2) : dominoRect.rect.height;
            ypreset *= lastDominoScript.transform.localScale.y;
            ypreset = -ypreset;
            float xpreset = (lastDominoScript.isVertical) ? dominoRect.rect.height / 4 : 0;
            xpreset *= lastDominoScript.transform.localScale.x;
            GameManager.instance.nextRotaionInField-=new Vector3(0,0,180) ;
            
            if (direction==1)
            {
                GameManager.instance.nextPositionInField = new Vector3(lastDominoScript.transform.localPosition.x+xpreset, lastDominoScript.transform.localPosition.y + ypreset, 0);
                SwapDominoNumbers(dominoScript);
                topPositionScript = dominoScript;
                topCounter++;
            }
            else if (direction==-1)
            {
                GameManager.instance.nextPositionInField = new Vector3(lastDominoScript.transform.localPosition.x-xpreset, lastDominoScript.transform.localPosition.y - ypreset, 0);
                botPositionScript = dominoScript;
                botCounter++;
            }
        }
        private void CheckingDominoValues_Horizantal(DominoScript lastDominoScript,int phase=1)
        {
            float ypreset = (lastDominoScript.isVertical) ? (dominoRect.rect.height / 2 + dominoRect.rect.width / 2):dominoRect.rect.width;
            ypreset *= lastDominoScript.transform.localScale.y;
            float xpreset = 0;
            if(phase==2)
            {
                ypreset = -ypreset;
            }
            if (direction==1)
            {
                GameManager.instance.nextPositionInField = new Vector3(lastDominoScript.transform.localPosition.x+xpreset, lastDominoScript.transform.localPosition.y +ypreset, 0);
                GameManager.instance.nextRotaionInField = new Vector3(0, 0, 90);
                topPositionScript = dominoScript;

            }
            else if (direction==-1)
            {
                GameManager.instance.nextPositionInField = new Vector3(lastDominoScript.transform.localPosition.x-xpreset, lastDominoScript.transform.localPosition.y -ypreset, 0);
                GameManager.instance.nextRotaionInField = new Vector3(0, 0, 90);
                botPositionScript = dominoScript;

            }

        }
        private void CheckingDominoValues_HorizantalPhase2(DominoScript lastDominoScript)
        {
            if (direction == 1 && topCounter == 4)
            {
                CheckingDominoValues_Horizantal_Phase3First(lastDominoScript);
            }
            else if (direction == -1 && botCounter == 4)
            {
                CheckingDominoValues_Horizantal_Phase3First(lastDominoScript);
            }            
            else if (direction == -1 && botCounter > 4)
            {
                CheckingDominoValues_Horizantal(lastDominoScript,2);
            }            
            else if (direction == 1 && topCounter > 4)
            {
                CheckingDominoValues_Horizantal(lastDominoScript,2);
            }
            else
            {
                float ypreset = 0;
                float xpreset = 0;
                if (topPositionScriptPhase2 == lastDominoScript || botPositionScriptPhase2 == lastDominoScript)
                {
                    ypreset = (lastDominoScript.isVertical) ? dominoRect.rect.height / 4 : 0;
                    ypreset *= lastDominoScript.transform.localScale.y;
                    xpreset = (lastDominoScript.isVertical) ? dominoRect.rect.width : (dominoRect.rect.height / 2 + dominoRect.rect.width / 2);
                    xpreset *= lastDominoScript.transform.localScale.x;
                }
                else
                {
                    xpreset = (lastDominoScript.isVertical) ? (dominoRect.rect.height / 2 + dominoRect.rect.width / 2) : dominoRect.rect.width;
                    xpreset *= lastDominoScript.transform.localScale.x;
                }
                if (direction == 1)
                {
                    topCounter++;
                    GameManager.instance.nextPositionInField = new Vector3(lastDominoScript.transform.localPosition.x+xpreset, lastDominoScript.transform.localPosition.y + ypreset, 0);
                    topPositionScript = dominoScript;

                }
                else if (direction == -1)
                {
                    botCounter++;
                    GameManager.instance.nextPositionInField = new Vector3(lastDominoScript.transform.localPosition.x-xpreset, lastDominoScript.transform.localPosition.y - ypreset, 0);
                    botPositionScript = dominoScript;

                }
            }
        }
        private void CheckingDominoValues_Horizantal_Phase3First(DominoScript lastDominoScript)
        {
            float ypreset = (lastDominoScript.isVertical) ? dominoRect.rect.width : (dominoRect.rect.height / 2 + dominoRect.rect.width / 2);
            ypreset *= lastDominoScript.transform.localScale.y;
            ypreset = -ypreset;
            float xpreset = (lastDominoScript.isVertical) ? dominoRect.rect.height / 4 : 0;
            xpreset *= lastDominoScript.transform.localScale.x;
            if (direction==1)
            {
                GameManager.instance.nextPositionInField = new Vector3(lastDominoScript.transform.localPosition.x+xpreset, lastDominoScript.transform.localPosition.y +ypreset, 0);
                GameManager.instance.nextRotaionInField = new Vector3(0, 0, 90);
                topPositionScript = dominoScript;
                topCounter++;

            }
            else if (direction==-1)
            {
                GameManager.instance.nextPositionInField = new Vector3(lastDominoScript.transform.localPosition.x-xpreset, lastDominoScript.transform.localPosition.y -ypreset, 0);
                GameManager.instance.nextRotaionInField = new Vector3(0, 0, 90);
                botPositionScript = dominoScript;
                botCounter++;

            }
        }
        private DominoScript FindingCorrectLastDomino(DominoScript dominoScript)
        {
            if (dominoScript.topNumber == topNumber && dominoScript.topNumber == botNumber)
            {
                return DualitySolver(dominoScript);
            }
            else if (dominoScript.topNumber == topNumber && dominoScript.downNumber == botNumber)
            {
                return DualitySolver(dominoScript);
            }
            else if (dominoScript.downNumber == topNumber && dominoScript.topNumber == botNumber)
            {
                return DualitySolver(dominoScript);
            }
            else if (dominoScript.downNumber == topNumber && dominoScript.downNumber == botNumber)
            {
                return DualitySolver(dominoScript);
            }
            else if (dominoScript.topNumber==topNumber  )
            {
                GameManager.instance.nextRotaionInField = new Vector3(0, 0, 180);
                //SwapDominoNumbers(dominoScript);
                direction = 1;
                topNumber = dominoScript.downNumber;
                return topPositionScript;
            }
            else if (dominoScript.downNumber == topNumber)
            {
                GameManager.instance.nextRotaionInField = Vector3.zero;
                direction = 1;
                topNumber = dominoScript.topNumber;
                return topPositionScript;


            }
            else if (dominoScript.topNumber==botNumber)
            {
                GameManager.instance.nextRotaionInField = Vector3.zero;
                direction = -1;
                botNumber = dominoScript.downNumber;
                return botPositionScript;
            }
            else if (dominoScript.downNumber == botNumber)
            {
                GameManager.instance.nextRotaionInField = new Vector3(0, 0, 180);
                //SwapDominoNumbers(dominoScript);
                direction = -1;
                botNumber = dominoScript.topNumber;
                return botPositionScript;

            }
            else
            {
                GameManager.instance.nextPositionInField = dominoScript.savedPosition;
                GameManager.instance.nextRotaionInField = Vector3.zero;
                return null;
            }

        }
        private void SwapDominoNumbers(DominoScript dominoScript)
        {
            int temp = dominoScript.topNumber;
            dominoScript.topNumber = dominoScript.downNumber;
            dominoScript.downNumber = dominoScript.topNumber;
        }
        private float Distance(Vector3 a,Vector3 b)
        {
            return (float)Math.Sqrt((a.x - b.x) * (a.x - b.x) + (a.y - b.y) * (a.y - b.y));
        }
        private DominoScript DualitySolver(DominoScript dominoScript)
        {

            float distanceToBotPosition = Distance(dominoScript.transform.position, botPositionScript.transform.position);
            float distanceToTopPosition = Distance(dominoScript.transform.position, topPositionScript.transform.position);
            if (distanceToTopPosition >= distanceToBotPosition)
            {
                if(dominoScript.topNumber == botNumber)
                {
                    GameManager.instance.nextRotaionInField = Vector3.zero;
                    botNumber = dominoScript.downNumber;
                }
                else
                {
                    GameManager.instance.nextRotaionInField = new Vector3(0, 0, 180);
                    botNumber = dominoScript.topNumber;
                }
                direction = -1;
                return botPositionScript;
            }
            else
            {
                if (dominoScript.topNumber == topNumber)
                {
                    GameManager.instance.nextRotaionInField = new Vector3(0, 0, 180);
                    topNumber = dominoScript.downNumber;
                }
                else
                {
                    GameManager.instance.nextRotaionInField = Vector3.zero;
                    topNumber = dominoScript.topNumber;
                }
                direction = 1;
                return topPositionScript;
            }
        }
        private void CheckingToZoom(DominoScript lastDominoScript,int phase=1)
        {
            bool zoomControl = true;
            if (direction == 1 && topCounter >= 4)
                zoomControl = false;
            else if (direction == -1 && botCounter >= 4)
                zoomControl = false;
            RectTransform lastDominoRect = lastDominoScript.GetComponent<RectTransform>();
            RectTransform playPanelRect = GameManager.instance.PlayPanel.GetComponent<RectTransform>();
            float spaceToMove;
            float centerDistance = Distance(GameManager.instance.PlayPanel.transform.localPosition, lastDominoScript.transform.localPosition);
            float topLength = (topPositionScript.isVertical) ? dominoRect.rect.height / 2 : dominoRect.rect.width / 2;
            float botLength = (botPositionScript.isVertical) ? dominoRect.rect.height / 2 : dominoRect.rect.width / 2;
            float dominoLenght = ((dominoScript.isVertical) ? dominoRect.rect.height : dominoRect.rect.width)*GameManager.instance.changeablePanelZoomLevel* dominoScript.transform.localScale.y;
            float threshold = (centerDistance +( (lastDominoScript.isVertical) ? lastDominoRect.rect.height/2 : lastDominoRect.rect.width/2) * lastDominoScript.transform.localScale.y)*GameManager.instance.changeablePanelZoomLevel+dominoLenght;
            if ( threshold>= -Math.Abs(GameManager.instance.ChangablePanel.transform.localPosition.y) + GameManager.instance.PlayPanel.transform.localPosition.y + playPanelRect.rect.height/ 2 )
            {
                float AllDominoesLength = (topPositionScript.transform.localPosition.y - botPositionScript.transform.localPosition.y + (botLength + topLength) * topPositionScript.transform.localScale.x) * GameManager.instance.changeablePanelZoomLevel;
                if (phase == 2)
                    dominoLenght = ((dominoScript.isVertical) ? -1000 : dominoRect.rect.height - dominoRect.rect.width) * GameManager.instance.changeablePanelZoomLevel * dominoScript.transform.localScale.y / 2;
                if (!zoomControl)
                    dominoLenght = -10000;
                if (5 + playPanelRect.rect.height - AllDominoesLength < dominoLenght)
                {
                    float dominoSize = TurnManager.dominoScale * TurnManager.dominoHeight * GameManager.instance.changeablePanelZoomLevel;
                    GameManager.instance.changeablePanelZoomLevel *= (5 + playPanelRect.rect.height) / (AllDominoesLength + dominoLenght);
                    EmptySpaceToDominoHeight(dominoSize);
                    GameManager.instance.ChangablePanel.transform.DOScale(Vector3.one * GameManager.instance.changeablePanelZoomLevel, GameManager.instance.GameConfig.moveAndRotateSpeed);
                    GameManager.instance.dominZoomLevel = GameManager.instance.changeablePanelZoomLevel;
                }
                topLength = (topPositionScript.isVertical) ? dominoRect.rect.height / 2 : dominoRect.rect.width / 2;
                botLength = (botPositionScript.isVertical) ? dominoRect.rect.height / 2 : dominoRect.rect.width / 2;
                AllDominoesLength = (topPositionScript.transform.localPosition.y - botPositionScript.transform.localPosition.y + (botLength + topLength) * topPositionScript.transform.localScale.x) * GameManager.instance.changeablePanelZoomLevel;
                dominoLenght = ((dominoScript.isVertical) ? dominoRect.rect.height : dominoRect.rect.width) * GameManager.instance.changeablePanelZoomLevel * dominoScript.transform.localScale.y;

                if (phase == 2)
                    dominoLenght = ((dominoScript.isVertical) ? +10000 : dominoRect.rect.height - dominoRect.rect.width) * GameManager.instance.changeablePanelZoomLevel * dominoScript.transform.localScale.y / 2;
                if (!zoomControl)
                    dominoLenght = +10000;

                float botPlayPanelPosition = Math.Abs(GameManager.instance.PlayPanel.transform.localPosition.y - playPanelRect.rect.height / 2) - Math.Abs(botPositionScript.transform.localPosition.y - botLength * topPositionScript.transform.localScale.x) * GameManager.instance.changeablePanelZoomLevel;
                float topPlayPanelPosition = Math.Abs(GameManager.instance.PlayPanel.transform.localPosition.y + playPanelRect.rect.height / 2) - Math.Abs(topPositionScript.transform.localPosition.y + topLength * topPositionScript.transform.localScale.x) * GameManager.instance.changeablePanelZoomLevel;
                if (20+playPanelRect.rect.height-AllDominoesLength>=dominoLenght)
                {
                    Vector3 destination = GameManager.instance.ChangablePanel.transform.localPosition;
                    spaceToMove = (direction == 1) ? -(dominoLenght-topPlayPanelPosition+destination.y) : (dominoLenght - botPlayPanelPosition-destination.y);
                    destination.y +=spaceToMove;
                    GameManager.instance.ChangablePanel.transform.DOLocalMove(destination, GameManager.instance.GameConfig.moveAndRotateSpeed);
                }

            }
        }

        private void EmptySpaceToDominoHeight(float notScaledDominoSize)
        {
            Transform playerSpace = GameManager.instance.EnemyEmptySpace.transform;
            Transform enemySpace = GameManager.instance.PlayerEmptySpace.transform;
            Transform UI = GameManager.instance.UI.transform;
            Transform Player = GameManager.instance.Player.transform;
            Transform Enemy = GameManager.instance.Enemy.transform;
            Transform playerChangeAblePanel = GameManager.instance.PlayerChangablePanel.transform;
            Transform enemyChangeAblePanel = GameManager.instance.EnemyChangablePanel.transform;
            float scale = GameManager.instance.changeablePanelZoomLevel;
            float heightDifferance = notScaledDominoSize - scale * TurnManager.dominoScale * TurnManager.dominoHeight;
            RectTransform playerSpaceRect=playerSpace.GetComponent<RectTransform>();
            RectTransform enemySpaceRect=enemySpace.GetComponent<RectTransform>();
            playerSpaceRect.sizeDelta =new Vector2(playerSpaceRect.rect.width ,scale * TurnManager.dominoScale * TurnManager.dominoHeight);
            enemySpaceRect.sizeDelta =new Vector2(enemySpaceRect.rect.width ,scale * TurnManager.dominoScale * TurnManager.dominoHeight);
            Enemy.localPosition=new Vector3(Enemy.localPosition.x,Enemy.localPosition.y-heightDifferance,0);
            UI.localPosition=new Vector3(UI.localPosition.x,UI.localPosition.y-heightDifferance,0);
            Player.localPosition=new Vector3(Player.localPosition.x,Player.localPosition.y+heightDifferance, 0);
            playerChangeAblePanel.localPosition=new Vector3(playerChangeAblePanel.localPosition.x,playerChangeAblePanel.localPosition.y+heightDifferance, 0);
            enemyChangeAblePanel.localPosition=new Vector3(enemyChangeAblePanel.localPosition.x,enemyChangeAblePanel.localPosition.y-heightDifferance,0);
        }
    }
}