using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // シングルトンインスタンス
    public static GameManager instance;

    // ゲーム状態管理
    public enum GameState { Playing, GameOver }
    public GameState currentState;

    [Header("ゲームオブジェクト設定")]
    [SerializeField] private GameObject ballPrefab; // ボールのプレハブ
    [SerializeField] private GameObject ringPrefab; // わっかのプレハブ
    [SerializeField] private Transform spawnPoint; // わっかの初期生成位置

    [Header("ゲーム設定")]
    [SerializeField] private int totalRings = 100; // 生成するわっかの総数
    [SerializeField] private float spawnInterval = 2.0f; // わっかを生成する時間間隔
    public List<RingController> activeRings; // 現在画面上にあるわっかのリスト
    
    // ボールの直径。外部からは読み取り専用のプロパティ(BallDiameter)経由でアクセスする
    private float ballDiameter;
    public float BallDiameter { get { return ballDiameter; } }

    /*
    [Header("UI設定")]
    [SerializeField] private Text scoreText;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private Text finalScoreText;
    */

    /*
    [Header("オーディオ設定")]
    [SerializeField] private AudioSource soundPlayer;
    [SerializeField] private AudioClip[] pianoNotes;
    [SerializeField] private AudioClip gameOverSound;
    */

    private int score = 0;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        activeRings = new List<RingController>();
    }

    void Start()
    {
        StartGame();
    }

    void Update()
    {
        if (currentState == GameState.Playing)
        {
            if (activeRings.Count > 0)
            {
                // 最もスケールが小さいわっかを探す
                RingController smallestRing = null;
                float minScale = float.MaxValue;

                foreach (RingController ring in activeRings)
                {
                    if (ring != null && ring.transform.localScale.x < minScale)
                    {
                        minScale = ring.transform.localScale.x;
                        smallestRing = ring;
                    }
                }

                if (smallestRing != null)
                {
                    // リングの内径がボールの直径より小さくなったらゲームオーバー
                    // 仮定: リングの transform.localScale.x が、そのリングの「内径」を表す
                    float innerDiameter = smallestRing.transform.localScale.x;
                    
                    if (innerDiameter < ballDiameter)
                    {
                        //GameOver();
                    }
                }
            }
        }
    }

    // BallControllerからボールの直径を受け取るための公開メソッド
    public void SetBallDiameter(float diameter)
    {
        ballDiameter = diameter;
    }

    void StartGame()
    {
        currentState = GameState.Playing;
        score = 0;
        // scoreText.text = "Score: " + score;
        // gameOverPanel.SetActive(false);
        Time.timeScale = 1f;
        StartCoroutine(SpawnRings());
    }

    IEnumerator SpawnRings()
    {
        for (int i = 0; i < totalRings; i++)
        {
            if (currentState == GameState.GameOver) yield break;

            GameObject newRingObj = Instantiate(ringPrefab, spawnPoint.position, Quaternion.Euler(0, 0, Random.Range(0f, 360f)));
            RingController newRing = newRingObj.GetComponent<RingController>();
            if (newRing != null)
            {
                activeRings.Add(newRing);
            }
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    public void IncrementScore()
    {
        if (currentState == GameState.Playing)
        {
            score++;
            // scoreText.text = "Score: " + score;

            /*
            if (pianoNotes != null && pianoNotes.Length > 0)
            {
                int noteIndex = (score - 1) % pianoNotes.Length;
                soundPlayer.PlayOneShot(pianoNotes[noteIndex]);
            }
            */
        }
    }

    public void GameOver()
    {
        if (currentState == GameState.Playing)
        {
            currentState = GameState.GameOver;
            Time.timeScale = 0f;
            // gameOverPanel.SetActive(true);
            // finalScoreText.text = "Final Score: " + score;
            
            /*
            if (gameOverSound != null)
            {
                soundPlayer.PlayOneShot(gameOverSound);
            }
            */
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}