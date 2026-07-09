using UnityEngine;
using System.Collections;

/// <summary>
/// スコアのフローティングテキストやスコア表示のパンチスケール演出を管理するクラス。
/// </summary>
public class ScoreJuice : MonoBehaviour
{
    /// <summary>
    /// ターゲットのGameObjectにScoreJuiceコンポーネントを追加し、パンチスケール演出を開始する静的メソッド。
    /// </summary>
    /// <param name="target">パンチスケール演出を適用するターゲットのGameObject</param>
    public static void PunchScale(GameObject target)
    {
        ScoreJuice sj = target.GetComponent<ScoreJuice>();
        if (sj == null)
        {
            sj = target.AddComponent<ScoreJuice>();
        }
        sj.StartPunch();
    }

    // 元のスケールを保持するための変数
    private Vector3 originalScale;
    // アクティブなコルーチンを保持するための変数
    private Coroutine activeCoroutine;

    void Awake()
    {
        // 元のスケールを保存
        originalScale = transform.localScale;
    }

    public void StartPunch()
    {
        // すでにアクティブなコルーチンがある場合は停止して元のスケールに戻す
        if (activeCoroutine != null)
        {
            StopCoroutine(activeCoroutine);
            transform.localScale = originalScale;
        }
        activeCoroutine = StartCoroutine(PunchRoutine());
    }

    /// <summary>
    /// パンチスケール演出を行うコルーチン。スケールを一時的に拡大し、元のスケールに戻す。
    /// </summary>
    /// <returns>IEnumerator コルーチン</returns>
    private IEnumerator PunchRoutine()
    {
        // パンチスケールの時間と目標スケールを設定
        float duration = 0.15f;
        float elapsed = 0f;
        Vector3 targetScale = originalScale * 1.3f;

        // スケールアップ
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            transform.localScale = Vector3.Lerp(originalScale, targetScale, Mathf.Sin(t * Mathf.PI * 0.5f));
            yield return null;
        }

        // スケールダウン
        elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            transform.localScale = Vector3.Lerp(targetScale, originalScale, Mathf.Sin(t * Mathf.PI * 0.5f));
            yield return null;
        }

        // 最後に元のスケールに戻す
        transform.localScale = originalScale;
        activeCoroutine = null;
    }
}
