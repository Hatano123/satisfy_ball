using UnityEngine;

public class BallController : MonoBehaviour
{
    private Rigidbody2D rb;
    private CircleCollider2D circleCollider;

    [SerializeField] private float forceMultiplier = 10f; // タップ時に加える力の強さ

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        circleCollider = GetComponent<CircleCollider2D>();
    }

    void Start()
    {
        // GameManagerにボールの直径を報告する
        if (GameManager.instance != null && circleCollider != null)
        {
            // 自身のコライダーとスケールから正確な直径を計算
            float diameter = circleCollider.radius * 2 * transform.localScale.x;

            // GameManagerに直径の値を設定するよう依頼する
            GameManager.instance.SetBallDiameter(diameter);
        }
    }

    void Update()
    {
        if (GameManager.instance != null && GameManager.instance.currentState == GameManager.GameState.Playing)
        {
            // マウスの左クリックまたはモバイルのタップを検知
            if (Input.GetMouseButtonDown(0))
            {
                // タップされた画面座標をワールド座標に変換
                Vector2 tapPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                // 力の計算: タップした地点からボールの中心へ向かう方向ベクトルを計算
                Vector2 direction = ((Vector2)transform.position - tapPosition).normalized;

                // ボールに力を加える
                rb.AddForce(direction * forceMultiplier, ForceMode2D.Impulse);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

    }
}