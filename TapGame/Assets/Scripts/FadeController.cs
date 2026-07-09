using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class FadeController : MonoBehaviour
{
    // フェード用のImage (Canvas内のImageオブジェクトを指定する)
    [SerializeField] private Image fadeImage;

    // フェードの所要時間（秒）
    [SerializeField] private float fadeDuration = 1.0f;

    // どのシーンに変えたいかを指定できるようにする。
    public string changeScene = "Game";

    // シーンを変えるかどうか
    public bool sceneChageFade = false;

    // フェードイン処理 (アルファ値を1から0に変更)
    public void FadeIn()
    {
        TimerController.isFadeFinished = false; // フェード終了フラグをリセット
        if (!fadeImage.gameObject.activeSelf)
        {
            fadeImage.gameObject.SetActive(true);
        }
        // コルーチンを開始して非同期で処理を行う
        StartCoroutine(Fade(1, 0, true));
    }

    // フェードアウト処理 (アルファ値を0から1に変更)
    public void FadeOut()
    {
        TimerController.isFadeFinished = false; // フェード終了フラグをリセット
        // Imageを表示する
        fadeImage.gameObject.SetActive(true);
        // コルーチンを開始して非同期で処理を行う
        StartCoroutine(Fade(0, 1, false));
    }

    /// <summary>
    /// フェード処理を行うコルーチン
    /// </summary>
    /// <param name="startAlpha">開始時のアルファ値</param>
    /// <param name="endAlpha">終了時のアルファ値</param>
    /// <param name="isFadeIn">フェードインかどうか</param>
    /// <returns>IEnumerator</returns>
    private IEnumerator Fade(float startAlpha, float endAlpha, bool isFadeIn)
    {
        float time = 0f; // 経過時間を追跡する変数
        Color fadeColor = fadeImage.color; // 現在のImageの色を取得

        // 指定した時間（fadeDuration）が経過するまでループ
        while (time < fadeDuration)
        {
            time += Time.deltaTime; // 経過時間を加算
            float t = time / fadeDuration; // 時間の進行割合 (0.0～1.0)
            fadeColor.a = Mathf.Lerp(startAlpha, endAlpha, t); // アルファ値を線形補間で計算
            fadeImage.color = fadeColor; // 計算結果のアルファ値をImageに適用
            yield return null; // 次のフレームまで処理を一時停止
        }

        // 最終的なアルファ値を正確に設定
        fadeColor.a = endAlpha;
        fadeImage.color = fadeColor;
        // 判定が被るなどの理由によりフェードインなら表示、そうじゃないなら表示
        fadeImage.gameObject.SetActive(!isFadeIn);
        // フェード終了フラグを設定
        TimerController.isFadeFinished = true;
        // シーン遷移をする場合
        if (sceneChageFade)
        {
            SceneManager.LoadScene(changeScene);
        }
    }
}