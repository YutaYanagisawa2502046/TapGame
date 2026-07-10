using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// スコア集計結果を表示するクラス。ゲーム終了後にスコアをカウントアップ表示し、演出を行う。
/// </summary>
public class ScoreResult : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;

    // 集計にかける時間
    public float resultTime = 1.5f;
    private float elapsedTime = 0f;

    private int resultScore = 0;
    private int totalScore = 0;
    private int lastDisplayedScore = 0;
    private bool isFinished = false;
    private float lastTickSoundTime = 0f;
    private const float tickSoundInterval = 0.06f; // 音割れ防止のための最小発音間隔（60ms）

    // Start is called before the first frame update
    void Start()
    {
        // 前回のゲームスコアを読み込む
        totalScore = ScoreManager.score;
        lastDisplayedScore = 0;
        isFinished = false;

        // 次のゲームのためにスコアをリセット
        ScoreManager.score = 0;
        ScoreManager.combo = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (!TimerController.isFadeFinished) 
        {
            // フェードイン中は演出を止める
            return;
        }

        if (isFinished)
        {
            // 演出が完了した後、画面タップでタイトルに戻る
            if (Input.GetMouseButtonDown(0))
            {
                if (SceneController.instance != null)
                {
                    SceneController.instance.SceneChange();
                }
            }
            return;
        }

        elapsedTime += Time.deltaTime;
        float t = Mathf.Clamp01(elapsedTime / resultTime);

        // Ease-out-quadイージングを適用して、徐々にカウントアップが遅くなる演出
        float easeOutT = 1f - (1f - t) * (1f - t);
        resultScore = (int)(totalScore * easeOutT);
        
        if (scoreText != null)
        {
            scoreText.text = "Score: " + resultScore;
        }

        // 数値が増えるタイミングでプチプチという効果音（10スコアごと、またはスコアが小さいときは毎加算）
        if (resultScore != lastDisplayedScore)
        {
            if (resultScore % 10 == 0 || totalScore < 10)
            {
                // 前回の再生から設定した間隔（60ms）以上経過している場合のみ鳴らす
                if (Time.unscaledTime - lastTickSoundTime >= tickSoundInterval)
                {
                    AudioManager.Instance.PlayTapSound();
                    lastTickSoundTime = Time.unscaledTime;
                }
            }
            lastDisplayedScore = resultScore;
        }

        // 演出完了
        if (t >= 1f)
        {
            isFinished = true;
            resultScore = totalScore;
            
            if (scoreText != null)
            {
                scoreText.text = "Score: " + resultScore;
                // テキストを大きくポップさせる
                ScoreJuice.PunchScale(scoreText.gameObject);

                // 10万点以上の場合はスコアをゴールド（金色）にする
                if (totalScore >= 100000)
                {
                    scoreText.color = new Color(1f, 0.85f, 0f);
                }
            }

            // スコアに応じたファンファーレと紙吹雪
            if (totalScore >= 100000)
            {
                // 超豪華ファンファーレ
                AudioManager.Instance.PlayGrandResultSound();
                // 大量の紙吹雪
                SpawnMassiveConfetti();
                // 画面中央にお祝いメッセージ
                SpawnCongratsBanner();
            }
            else
            {
                // 通常ファンファーレ
                AudioManager.Instance.PlayResultSound();
                // 通常紙吹雪
                SpawnConfetti();
            }
        }
    }

    // 周囲に紙吹雪を数箇所生成 (通常)
    private void SpawnConfetti()
    {
        Camera mainCam = Camera.main;
        if (mainCam == null) return;

        Vector3 camPos = mainCam.transform.position;
        float vertExtent = mainCam.orthographicSize;
        float horzExtent = vertExtent * mainCam.aspect;

        Color[] colors = { Color.red, Color.yellow, Color.cyan, Color.green, Color.magenta, new Color(1f, 0.5f, 0f) };
        
        for (int i = 0; i < 6; i++)
        {
            // カメラの視界内でランダムな位置を設定
            Vector3 pos = new Vector3(
                Random.Range(-horzExtent * 0.6f, horzExtent * 0.6f),
                Random.Range(-vertExtent * 0.4f, vertExtent * 0.4f),
                0f
            ) + camPos;
            pos.z = 0f; // 2DなのでZ軸は0に固定

            Color randColor = colors[Random.Range(0, colors.Length)];
            ParticleBurst.Create(pos, randColor);
        }
    }

    // 10万点以上用の大爆発紙吹雪
    private void SpawnMassiveConfetti()
    {
        Camera mainCam = Camera.main;
        if (mainCam == null) return;

        Vector3 camPos = mainCam.transform.position;
        float vertExtent = mainCam.orthographicSize;
        float horzExtent = vertExtent * mainCam.aspect;

        Color[] colors = { Color.red, Color.yellow, Color.cyan, Color.green, Color.magenta, new Color(1f, 0.5f, 0f), Color.white };
        
        // 20回爆発させて画面中を紙吹雪にする
        for (int i = 0; i < 20; i++)
        {
            Vector3 pos = new Vector3(
                Random.Range(-horzExtent * 0.8f, horzExtent * 0.8f),
                Random.Range(-vertExtent * 0.6f, vertExtent * 0.6f),
                0f
            ) + camPos;
            pos.z = 0f;

            Color randColor = colors[Random.Range(0, colors.Length)];
            ParticleBurst.Create(pos, randColor);
        }
    }

    // お祝いUIテキストバナーを生成
    private void SpawnCongratsBanner()
    {
        if (scoreText == null) return;

        GameObject go = new GameObject("CongratsText");
        go.transform.SetParent(scoreText.transform.parent, false);

        // スコアテキストの少し下に配置
        RectTransform scoreRect = scoreText.GetComponent<RectTransform>();
        RectTransform rect = go.AddComponent<RectTransform>();
        rect.anchoredPosition = scoreRect.anchoredPosition + new Vector2(0f, -100f);
        rect.sizeDelta = new Vector2(800f, 120f);

        // 高得点を取ったプレイヤーへのお祝いメッセージ
        TextMeshProUGUI tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = "CONGRATULATIONS!";
        tmp.fontSize = 48;
        tmp.color = new Color(1f, 0.85f, 0f); // 初期は金色
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.font = scoreText.font; // 同じフォントアセットを使用
        
        // テキストポップ演出
        ScoreJuice.PunchScale(go);

        // 呼吸するパルス・色明滅スクリプトをアタッチ
        CongratsPulse pulse = go.AddComponent<CongratsPulse>();
        pulse.text = tmp;
    }
}

// 金色と白色に明滅しつつ呼吸するお祝いアニメーションコンポーネント
public class CongratsPulse : MonoBehaviour
{
    public TextMeshProUGUI text;
    private Color gold = new Color(1f, 0.85f, 0f);
    private Color white = Color.white;

    void Update()
    { 
        if (text != null)
        {
            // 時間の経過でゴールドとホワイトをループ（ unscaledTime なのでタイムスケール影響なし）
            float val = Mathf.PingPong(Time.unscaledTime * 4f, 1f);
            text.color = Color.Lerp(gold, white, val);

            // 呼吸するように少しスケールを拡大縮小する
            float scaleVal = 1.0f + 0.06f * Mathf.Sin(Time.unscaledTime * 4f);
            transform.localScale = new Vector3(scaleVal, scaleVal, 1f);
        }
    }
}
