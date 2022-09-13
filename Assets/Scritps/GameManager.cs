using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Linq;
using System;
namespace Dominoes
{
    public class GameManager : StateMachine
    {
        // sealizable fields

        public GameObject PlayGround;

        public GameObject DominoPrefab;

        public DominoesScriptableObject DominoesData;

        public GameObject Enemy;
        public GameObject PlayerEmptySpace;
        public GameObject EnemyEmptySpace;
        public GameObject ExtraCards;

        public GameObject PlayPanel;

        public GameObject ChangablePanel;
        public GameObject PlayerChangablePanel;
        public GameObject EnemyChangablePanel;
        public Text EndingText;
        public Text PlayerScoreText;
        public Text EnemyScoreText;

        public GameObject Player;

        public GameConfig GameConfig;

        public GameObject UI;
        public GameObject EndingPanel;
        //Game controll
        public static GameManager instance { get; private set; }
        public Vector3 nextPositionInField { get; set; }
        public Vector3 nextRotaionInField { get; set; }
        public float changeablePanelZoomLevel { get; set; }
        public float playerChangeablePanelZoomLevel;
        public float enemyChangeablePanelZoomLevel;
        public float dominZoomLevel { get; set; }
        public float border { get; set; }
        public bool turn { get; set; }
        public Vector3 extraCardsStratingPosition;

        public int playerPoints { get; set; }
        public int enemyPoints { get; set; }

        //events



        public List<DominoScript> stashDominoes;
        
        public List<DominoScript> extraDominoes;
        
        public List<DominoScript> playerDominoes;

        public List<DominoScript> enemyDominoes;

        public List<GameObject> fieldDominoes;

        public List<int> trackDominoes;

        public List<GameObject> dominoGameObjects;
        public List<Vector3> playerDominoStations;
        public List<Vector3> enemyDominoStations;


        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(instance);
            }
            else
                Destroy(gameObject);
        }

        void Start()
        {
            InitializingValues();
            SetState(new Initialization());
            

        }
        private void InitializingValues()
        {
            stashDominoes = new List<DominoScript>();
            enemyDominoes = new List<DominoScript>();
            extraDominoes = new List<DominoScript>();
            playerDominoes = new List<DominoScript>();
            playerDominoStations = new List<Vector3>();
            enemyDominoStations = new List<Vector3>();
            trackDominoes = new List<int>();
            fieldDominoes = new List<GameObject>();
            SettingLastRoundScore();
            nextPositionInField = PlayPanel.transform.localPosition;
            dominZoomLevel = 1f;
            border = 30f;
            changeablePanelZoomLevel = 1f;
            playerChangeablePanelZoomLevel = 1f;
            enemyChangeablePanelZoomLevel = 1f;
            extraCardsStratingPosition = ExtraCards.transform.localPosition;
            GamePresentor.topCounter = 0;
            GamePresentor.botCounter=0;
            //TurnManager.eventAdder = true;
            
        }
        private void SettingLastRoundScore()
        {
            if (PlayerPrefs.HasKey("playerPointsPerGame") || PlayerPrefs.HasKey("enemyPointsPerGame"))
            {
                PlayerScoreText.text = PlayerPrefs.GetInt("playerPointsPerGame").ToString();
                EnemyScoreText.text = PlayerPrefs.GetInt("enemyPointsPerGame").ToString();
            }
            else
            {
                PlayerPrefs.SetInt("playerPointsPerGame", 0);
                PlayerPrefs.SetInt("enemyPointsPerGame", 0);
                PlayerScoreText.text = PlayerPrefs.GetInt("playerPointsPerGame").ToString();
                EnemyScoreText.text = PlayerPrefs.GetInt("enemyPointsPerGame").ToString();
            }
  
        }
        public  void Wait1SecThanDo()
        {
            StartCoroutine(funtionToWait());
        }
        public IEnumerator funtionToWait()
        {
            yield return new WaitForSeconds(1f);
            ExtraCards.SetActive(false);
        }
    }
}
