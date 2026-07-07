using UnityEngine;

public class FloatingText : MonoBehaviour
{
    // スコアが増えたときに表示されるテキストを生成するための静的メソッド
    public static void Create(Vector3 position, string text, Color color)
    {
        GameObject go = new GameObject("FloatingText");
        go.transform.position = position + Vector3.forward * -1f; // offset slightly forward in 2D
        FloatingText ft = go.AddComponent<FloatingText>();

        // テキストメッシュを追加して、テキストの内容と色を設定
        TextMesh tm = go.AddComponent<TextMesh>();
        tm.text = text;
        tm.color = color;
        tm.fontSize = 48;
        tm.characterSize = 0.1f;
        tm.anchor = TextAnchor.MiddleCenter;
        tm.alignment = TextAlignment.Center;
        
        // フォントのスタイルを設定
        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (font == null)
        {
            font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        }

        if (font != null)
        {
            tm.font = font;
            go.GetComponent<MeshRenderer>().material = font.material;
        }
    }

    // アニメーションの設定
    // duration: テキストが表示される時間
    private float duration = 0.8f;
    // elapsed: 経過時間を追跡する変数
    private float elapsed = 0f;
    // velocity: 上方向に浮かぶ速度を設定
    private Vector3 velocity = new Vector3(0, 1.8f, 0); // 上方向に浮かぶ

    void Update()
    {
        // 経過時間を更新
        elapsed += Time.deltaTime;
        float t = elapsed / duration;
        
        // 上方向に移動
        transform.position += velocity * Time.deltaTime;
        
        // スケールとフェードのアニメーション
        if (t < 0.2f)
        {
            // すばやくポップ/拡大
            float scale = Mathf.Lerp(0f, 1.3f, t / 0.2f);
            transform.localScale = new Vector3(scale, scale, 1f);
        }
        else
        {
            // ゆっくり縮小してフェードアウト
            float scale = Mathf.Lerp(1.3f, 0.8f, (t - 0.2f) / 0.8f);
            transform.localScale = new Vector3(scale, scale, 1f);
        
            TextMesh tm = GetComponent<TextMesh>();
            if (tm != null)
            {
                Color c = tm.color;
                c.a = Mathf.Max(0f, Mathf.Lerp(1f, 0f, (t - 0.2f) / 0.8f));
                tm.color = c;
            }
        }

        // 一定時間経過後にオブジェクトを破棄
        if (elapsed >= duration)
        {
            Destroy(gameObject);
        }
    }
}
