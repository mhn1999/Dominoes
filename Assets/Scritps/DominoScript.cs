using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using DG.Tweening;
namespace Dominoes
{
    public class DominoScript : MonoBehaviour
    {
        public int topNumber;
        public int downNumber;
        public Sprite verticalImage;
        public Sprite horizantalImage;
        public bool isVertical = true;
        private Image imageComponent;
        private RectTransform playPanleRect;
        private RectTransform dominoRect;
        public Vector3 savedPosition { get; private set; }
        Vector3 mousePositionOffSet;
        private Vector3 GetMouseWorldPosition()
        {

            return GameManager.instance.PlayGround.transform.InverseTransformPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        }
        // Start is called before the first frame update
        private void Awake()
        {
            imageComponent = gameObject.GetComponent<Image>();
             playPanleRect = GameManager.instance.PlayPanel.GetComponent<RectTransform>();
             dominoRect = gameObject.GetComponent<RectTransform>();
            
        }
        void Start()
        {
            Initialization();

        }

        // Update is called once per frame
        void Update()
        {

        }
        private void Initialization()
        {
            isVertical = (topNumber == downNumber) ? false : true;
            imageComponent.sprite = verticalImage;
        }
        public void ChangePicture()
        {
            if (imageComponent.sprite == verticalImage)
                imageComponent.sprite = horizantalImage;
            else
                imageComponent.sprite = verticalImage;
        }
        private void OnMouseDown()
        {
            if(transform.parent== GameManager.instance.ExtraCards.transform)
            {
                AddingDominoToPlayerHand(this);
            }
            else
            {
            savedPosition = transform.localPosition;
            mousePositionOffSet = gameObject.transform.localPosition - GetMouseWorldPosition();
            }
        }
        private void OnMouseDrag()
        {
            Vector3 temp = GetMouseWorldPosition() - mousePositionOffSet;
            transform.localPosition = new Vector3(temp.x, temp.y, 0);

        }
        private void OnMouseUp()
        {
            Vector3 distance = transform.localPosition - GameManager.instance.PlayPanel.transform.localPosition;
            if ( Math.Abs(distance.x)<=playPanleRect.rect.width/2 && Math.Abs(distance.y)<=playPanleRect.rect.height/2 && GameManager.instance.ExtraCards.activeSelf==false)
            {
                GameManager.instance.fieldDominoes.Add(gameObject);
                GameManager.instance.SetState(new GamePresentor());
            }
            else
            {
                transform.localPosition = savedPosition;
            }
        }
        private void AddingDominoToPlayerHand(DominoScript dominoScript)
        {
            if(!GameManager.instance.turn)
                transform.SetParent(GameManager.instance.PlayerChangablePanel.transform);
            else
                transform.SetParent(GameManager.instance.EnemyChangablePanel.transform);
            ChangePicture();
            TurnManager.ExtraDominoToHand(this);


        }


    }
}
