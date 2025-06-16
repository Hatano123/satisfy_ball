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
        // GameManagerにボールの直径を渡す
        if (GameManager.instance != null && circleCollider != null)
        {
            // transform.localScale.x はボールのX軸方向のスケールを表す
            // CircleCollider2Dのradiusはローカルスケールに依存しないため、
            // グローバルな直径を計算するにはtransform.localScale.xを乗算する必要がある
            GameManager.instance.ballDiameter = circleCollider.radius * 2 * transform.localScale.x;
        }
    }

    void Update()
    {
        if (GameManager.instance != null && GameManager.instance.currentState == GameManager.GameState.Playing)
        {
            // モバイルのタップを検知
            if (Input.GetMouseButtonDown(0))
            {
                // タップされた画面座標をワールド座標に変換
                Vector2 tapPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                // 力の計算: タップした地点からボールの中心へ向かう方向ベクトルを計算
                Vector2 direction = (transform.position - (Vector3)tapPosition).normalized;

                // ボールに力を加える
                rb.AddForce(direction * forceMultiplier, ForceMode2D.Impulse);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 衝突した相手が Ring タグを持つオブジェクトだった場合、ゲームオーバー
        if (collision.gameObject.CompareTag("Ring"))
        {
            if (GameManager.instance != null)
            {
                GameManager.instance.GameOver();
            }
        }
    }
}
