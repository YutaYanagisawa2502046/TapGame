using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ターゲットオブジェクトを制御するクラス
/// </summary>
public class Target : MonoBehaviour
{
    public int score = 100;

    public float maxTimer = 1.0f;   // タイマー最大値
    public float minTimer = 2.0f;   // タイマー最小値

    // 時間計測用
    private float timer = 0.0f;
    private float currentTimer = 0.0f;

    // 演出用
    private Vector3 originalScale;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    
    private bool isProcessed = false;
    private float spawnDuration = 0.25f;
    private float spawnTime = 0f;

    // Start is called before the first frame update
    void Start()
    {
        // min/maxが逆になっている場合を考慮してRandom.Rangeを実行
        timer = Random.Range(Mathf.Min(minTimer, maxTimer), Mathf.Max(minTimer, maxTimer));

        // 初期スケールと色を保存
        originalScale = transform.localScale;
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        if (spriteRenderer != null)
        {
            // 目に優しいパステルカラーのリスト
            Color[] eyeFriendlyColors = new Color[]
            {
                new Color(0.65f, 0.8f, 0.95f),   // ソフトブルー
                new Color(0.65f, 0.88f, 0.72f),  // ミントグリーン
                new Color(0.95f, 0.72f, 0.72f),  // ソフトピンク
                new Color(0.95f, 0.88f, 0.65f),  // ソフトイエロー
                new Color(0.82f, 0.72f, 0.95f),  // ラベンダー
                new Color(0.65f, 0.85f, 0.82f)   // ソフトティール
            };
            
            Color selectedColor = eyeFriendlyColors[Random.Range(0, eyeFriendlyColors.Length)];
            selectedColor.a = spriteRenderer.color.a; // スプライト本来の透明度を維持
            spriteRenderer.color = selectedColor;
            originalColor = selectedColor;
        }
        else
        {
            originalColor = Color.white;
        }

        // 出現時はスケールをゼロにする（ポップイン演出）
        transform.localScale = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        if (isProcessed) return;

        // シーン上のタイムアップ時の処理
        if (TimerController.isTimeUp)
        {
            isProcessed = true;
            Destroy(gameObject);
            return;
        }

        currentTimer += Time.deltaTime;

        // 寿命切れ（ミス）の判定
        if (currentTimer >= timer)
        {
            OnMiss();
            return;
        }

        // スケール計算のブレンド（出現時のポップイン × 消失前の縮小・警告）
        float remainingFactor = 1f;
        float lifeProgress = currentTimer / timer;

        // 寿命の残り30%になったら警告演出（縮小と赤点滅）
        if (lifeProgress > 0.7f)
        {
            float warningProgress = (lifeProgress - 0.7f) / 0.3f;
            remainingFactor = Mathf.Lerp(1f, 0f, warningProgress);

            if (spriteRenderer != null)
            {
                float flashSpeed = 15f;
                float flashVal = Mathf.PingPong(Time.time * flashSpeed, 1f);
                spriteRenderer.color = Color.Lerp(originalColor, Color.red, flashVal);
            }
        }

        // 出現中のスケールアニメーション（Sin関数を用いたプルプルとしたバウンド）
        if (spawnTime < spawnDuration)
        {
            spawnTime += Time.deltaTime;
            float t = spawnTime / spawnDuration;
            // イージングカーブ (Back-Ease-Outの簡易シミュレーション)
            float bounceScale = 1f + 0.3f * Mathf.Sin(t * Mathf.PI) * (1f - t);
            transform.localScale = originalScale * Mathf.Lerp(0f, bounceScale, t) * remainingFactor;
        }
        else
        {
            transform.localScale = originalScale * remainingFactor;
        }
    }

    // タップされた時の処理
    public void OnTouchObject()
    {
        if (isProcessed || TimerController.isTimeUp) return;
        isProcessed = true;

        // 連打防止のため、コライダーを無効化
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        StartCoroutine(TapAnimationRoutine());
    }

    // ミス（タイムアウト）時の処理
    private void OnMiss()
    {
        isProcessed = true;

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        // 画面揺れ (重めの鈍い揺れ)
        // 数が多いとき酔うかもしれないため多すぎるときは揺らさないようにする
        if (GameObject.FindGameObjectsWithTag("Target").Length <= 10)
            CameraShake.Shake(0.2f, 0.08f);

        // ミス効果音
        AudioManager.Instance.PlayMissSound();

        StartCoroutine(MissAnimationRoutine());
    }

    // タップ時の弾けるアニメーション
    private IEnumerator TapAnimationRoutine()
    {
        // スコア加算 (コンボとフローティングテキストの生成を含む)
        if (ScoreManager.instance != null)
        {
            ScoreManager.instance.AddScore(score, transform.position);
        }

        // カメラ揺れ (軽快な揺れ)
        CameraShake.Shake(0.12f, 0.05f);

        // タップ効果音
        AudioManager.Instance.PlayTapSound();

        // パーティクル生成 (スプライト色に合わせる。ない場合は白)
        Color particleColor = spriteRenderer != null ? spriteRenderer.color : Color.white;
        ParticleBurst.Create(transform.position, particleColor);

        // 拡大しながら消えていく演出
        float duration = 0.15f;
        float elapsed = 0f;
        Vector3 startScale = transform.localScale;
        Vector3 targetScale = originalScale * 1.4f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            transform.localScale = Vector3.Lerp(startScale, targetScale, t);
            if (spriteRenderer != null)
            {
                Color c = spriteRenderer.color;
                c.a = Mathf.Lerp(originalColor.a, 0f, t);
                spriteRenderer.color = c;
            }
            yield return null;
        }

        Destroy(gameObject);
    }

    // タイムアウト時のしぼんで消えるアニメーション
    private IEnumerator MissAnimationRoutine()
    {
        float duration = 0.15f;
        float elapsed = 0f;
        Vector3 startScale = transform.localScale;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            transform.localScale = Vector3.Lerp(startScale, Vector3.zero, t);
            if (spriteRenderer != null)
            {
                Color c = spriteRenderer.color;
                // 赤点滅した状態からフェードアウト
                c.a = Mathf.Lerp(spriteRenderer.color.a, 0f, t);
                spriteRenderer.color = c;
            }
            yield return null;
        }

        Destroy(gameObject);
    }
}
