using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
    public float ballDiameter; // ボールの直径（ゲームオーバー判定用）

    [Header("UI設定")]
    [SerializeField] private Text scoreText; // スコア表示用UIテキスト
    [SerializeField] private GameObject gameOverPanel; // ゲームオーバー時に表示するパネル
    [SerializeField] private Text finalScoreText; // ゲームオーバー時の最終スコア表示用テキスト

    [Header("オーディオ設定")]
    [SerializeField] private AudioSource soundPlayer; // 効果音再生用のAudioSource
    [SerializeField] private AudioClip[] pianoNotes; // ピアノの音階（ドレミファソラシ）のAudioClip配列。7つ設定する。
    [SerializeField] private AudioClip gameOverSound; // ゲームオーバー時の効果音

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
            // ゲームオーバー条件B: 最も内側のわっかの内径がボールの直径以下になったらゲームオーバー
            if (activeRings.Count > 0)
            {
                // activeRingsリストは、生成順にわっかが追加されるため、
                // リストの最初の要素が最も古い（最も収縮している）わっかであると仮定します。
                // ただし、わっかがDestroyされるとリストから削除されるため、
                // 常にリストの最初の要素が最も古いとは限りません。
                // ここでは、最もスケールが小さいわっかを探すロジックを実装します。
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
                    // わっかの内径はスケールに比例すると仮定
                    // わっかのプレハブの初期スケールが1の場合、現在のスケールがそのまま内径の比率になる
                    // ここでは、わっかの初期スケールを1とし、そのスケールがボールの直径以下になったらゲームオーバーとする
                    // 厳密にはわっかのスプライトの内径と外径の差を考慮する必要があるが、ここでは簡略化
                    if (smallestRing.transform.localScale.x * 0.5f < ballDiameter) // 0.5fはわっかの厚みを考慮した仮の値
                    {
                        GameOver();
                    }
                }
            }
        }
    }

    void StartGame()
    {
        currentState = GameState.Playing;
        score = 0;
        scoreText.text = "Score: " + score;
        gameOverPanel.SetActive(false);
        Time.timeScale = 1f; // ゲーム開始時に時間を元に戻す
        StartCoroutine(SpawnRings());
    }

    IEnumerator SpawnRings()
    {
        for (int i = 0; i < totalRings; i++)
        {
            if (currentState == GameState.GameOver) yield break; // ゲームオーバーになったら生成を停止

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
            scoreText.text = "Score: " + score;

            // ピアノ音再生ロジック
            if (pianoNotes != null && pianoNotes.Length > 0)
            {
                int noteIndex = (score - 1) % pianoNotes.Length; // スコアを元に再生する音を決定
                soundPlayer.PlayOneShot(pianoNotes[noteIndex]);
            }
        }
    }

    public void GameOver()
    {
        if (currentState == GameState.Playing) // 多重呼び出し防止
        {
            currentState = GameState.GameOver;
            Time.timeScale = 0f; // ゲームの時間を停止
            gameOverPanel.SetActive(true);
            finalScoreText.text = "Final Score: " + score;
            
            if (gameOverSound != null)
            {
                soundPlayer.PlayOneShot(gameOverSound);
            }
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f; // 時間を元に戻す
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // 現在のシーンをリロード
    }
}
