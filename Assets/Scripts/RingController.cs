using UnityEngine;

public class RingController : MonoBehaviour
{
    [SerializeField] private float shrinkSpeed = 0.05f; // 1秒あたりにスケールが減少する量

    void Update()
    {
        if (GameManager.instance != null && GameManager.instance.currentState == GameManager.GameState.Playing)
        {
            // 徐々にスケールを小さくする
            transform.localScale -= new Vector3(shrinkSpeed, shrinkSpeed, 0) * Time.deltaTime;

            // スケールが0以下になったら、オブジェクトを破棄
            if (transform.localScale.x <= 0)
            {
                if (GameManager.instance != null)
                {
                    GameManager.instance.activeRings.Remove(this); // GameManagerのリストから自身を削除
                }
                Destroy(gameObject);
            }
        }
    }
}
