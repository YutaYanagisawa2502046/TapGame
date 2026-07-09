using UnityEngine;

public class ParticleBurst : MonoBehaviour
{
    /// <summary>
    /// パーティクルバーストを生成するメソッド
    /// </summary>
    /// <param name="position">パーティクルバーストを生成する位置</param>
    /// <param name="color">パーティクルの色</param>
    public static void Create(Vector3 position, Color color)
    {
        GameObject burstParent = new GameObject("ParticleBurst");
        burstParent.transform.position = position;
        
        int particleCount = 12;
        Sprite circleSprite = GetDefaultCircleSprite();

        for (int i = 0; i < particleCount; i++)
        {
            GameObject p = new GameObject("Particle");
            p.transform.parent = burstParent.transform;
            p.transform.localPosition = Vector3.zero;
            
            SpriteRenderer sr = p.AddComponent<SpriteRenderer>();
            sr.color = color;
            sr.sortingOrder = 10;
            sr.sprite = circleSprite;

            MovingParticle mp = p.AddComponent<MovingParticle>();
            mp.color = color;
        }
        
        Destroy(burstParent, 1.5f);
    }

    /// <summary>
    /// particleBurst用のデフォルトの円形スプライトを生成するメソッド
    /// </summary>
    /// <returns>生成された円形スプライト</returns>
    private static Sprite GetDefaultCircleSprite()
    {
        // メモリ内で16x16の円形テクスチャを生成
        Texture2D tex = new Texture2D(16, 16, TextureFormat.RGBA32, false);
        Color transparent = new Color(0, 0, 0, 0);
        for (int y = 0; y < 16; y++)
        {
            for (int x = 0; x < 16; x++)
            {
                float dx = x - 7.5f;
                float dy = y - 7.5f;
                if (dx * dx + dy * dy <= 56.25f) // 半径 7.5
                {
                    tex.SetPixel(x, y, Color.white);
                }
                else
                {
                    tex.SetPixel(x, y, transparent);
                }
            }
        }
        tex.Apply();
        
        return Sprite.Create(tex, new Rect(0, 0, 16, 16), new Vector2(0.5f, 0.5f));
    }
}

/// <summary>
/// particleBurst内の個々のパーティクルを制御するクラス
/// </summary>
public class MovingParticle : MonoBehaviour
{
    // パーティクルの色を保持するための変数
    public Color color;
    // パーティクルの速度を保持するための変数
    private Vector3 velocity;
    // 重力の影響を与えるための変数
    private float gravity = -7f;
    // パーティクルの寿命を追跡するための変数
    private float elapsed = 0f;
    private float duration;

    void Start()
    {
        // ランダムな方向と速度を設定
        float angle = Random.Range(0f, Mathf.PI * 2f);
        float speed = Random.Range(2f, 5f);
        velocity = new Vector3(Mathf.Cos(angle) * speed, Mathf.Sin(angle) * speed, 0f);
        duration = Random.Range(0.4f, 0.7f);
        
        float startScale = Random.Range(0.15f, 0.35f);
        transform.localScale = new Vector3(startScale, startScale, 1f);
    }

    void Update()
    {
        // 経過時間の更新
        elapsed += Time.deltaTime;
        float t = elapsed / duration;
        
        // 重力と速度の更新
        velocity.y += gravity * Time.deltaTime;
        transform.position += velocity * Time.deltaTime;
        
        // 縮小とフェード
        float currentScale = Mathf.Lerp(transform.localScale.x, 0f, Time.deltaTime * 4f);
        transform.localScale = new Vector3(currentScale, currentScale, 1f);

        // スプライトの透明度を更新
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            Color c = sr.color;
            c.a = Mathf.Max(0f, 1f - t);
            sr.color = c;
        }

        // 一定時間経過後にオブジェクトを破棄
        if (elapsed >= duration)
        {
            Destroy(gameObject);
        }
    }
}
