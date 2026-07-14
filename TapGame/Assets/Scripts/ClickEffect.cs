using UnityEngine;
using System.Collections;

/// <summary>
/// クリックまたはタップ時にエフェクトを表示するクラス。
/// </summary>
public class ClickEffect : MonoBehaviour
{
    public GameObject effectPrefab; // エフェクトのプレハブ
    public float maxScale = 1.5f; // エフェクトが広がる大きさ
    public float duration = 0.3f; // エフェクトの表示時間

    private Camera mainCamera;

    void Start()
    {
        // メインカメラを取得
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var color = effectPrefab.GetComponent<SpriteRenderer>().color;
            color.a = 1f;
            effectPrefab.GetComponent<SpriteRenderer>().color = color;

            // 画面をタップしたときの処理
            Vector2 tapPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);

            // エフェクトを生成
            GameObject effect = Instantiate(effectPrefab, tapPosition, Quaternion.identity, this.transform);
            Destroy(effect, duration);

            StartCoroutine(ScaleEffect(effect.transform));
        }
    }

    /// <summary>
    /// エフェクトのスケールを時間経過に応じて変更するコルーチン。
    /// </summary>
    /// <param name="effectTransform">スケールを変更するエフェクトのTransform</param>
    /// <returns>IEnumerator</returns>
    IEnumerator ScaleEffect(Transform effectTransform)
    {
        // 経過時間を定義
        float elapsedTime = 0f;

        // エフェクトの表示時間中繰り返し
        while (elapsedTime < duration)
        {
            // オブジェクトが破棄されたかどうかを確認
            if (effectTransform == null)
            {
                yield break; // メソッドを終了
            }

            // ローカルスケールを変更
            float scale = Mathf.Lerp(0f, maxScale, elapsedTime / duration);
            Color effectColor = effectTransform.GetComponent<SpriteRenderer>().color;
            effectColor.a = 1f - (scale / maxScale);
            effectTransform.localScale = new Vector2(scale, scale);
            effectTransform.GetComponent<SpriteRenderer>().color = effectColor;

            // 経過時間を進める
            elapsedTime += Time.deltaTime;

            // 1フレーム進める
            yield return null;

        }

        var color = effectPrefab.GetComponent<SpriteRenderer>().color;
        color.a = 1f;
        effectPrefab.GetComponent<SpriteRenderer>().color = color;
    }
}