using UnityEngine;

public class ScoreTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        // トリガーに進入したオブジェクトが Ball タグを持つ場合
        if (other.CompareTag("Ball"))
        {
            if (GameManager.instance != null)
            {
                GameManager.instance.IncrementScore(); // スコアを加算

                // 親オブジェクト（わっか全体）をリストから削除し、破棄する
                RingController parentRing = transform.parent.GetComponent<RingController>();
                if (parentRing != null)
                {
                    GameManager.instance.activeRings.Remove(parentRing);
                }
                Destroy(transform.parent.gameObject);
            }
        }
    }
}
