using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimerController : MonoBehaviour
{
    // バーの画像
    [SerializeField] private Image timerBarImage;
    // タイマー初期制限時間
    [SerializeField] float maxTimer = 30.0f;
    // タイムアップ表示テキスト
    [SerializeField] TextMeshProUGUI timeUpText;

    // タイマー表示用
    private float currentTime;
    // 時間切れになったか
    public static bool isTimeUp = false;

    // 演出用
    private Color originalBarColor;

    // フェード終了前にタイマーを開始しないためのフラグ
    public static bool isFadeFinished = false;

    // Start is called before the first frame update
    void Start()
    {
        currentTime = maxTimer;
        
        if (timeUpText != null)
            timeUpText.gameObject.SetActive(false);

        // シーン遷移時にstatic変数を必ず初期化する（バグ防止）
        isTimeUp = false;

        // タイマーバーの初期情報保存
        if (timerBarImage != null)
        {
            originalBarColor = timerBarImage.color;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isTimeUp || !isFadeFinished) return;

        if (currentTime > 0.0f)
        {
            currentTime -= Time.deltaTime;
            
            if (timerBarImage != null)
            {
                timerBarImage.fillAmount = currentTime / maxTimer;

                // 残り時間5秒以下の警告演出
                if (currentTime <= 5.0f)
                {
                    // 赤色と初期色を激しく点滅させる
                    float flashSpeed = 10f;
                    // Mathf.PingPongを使って0～1の範囲で値を変化させる
                    float flashVal = Mathf.PingPong(Time.time * flashSpeed, 1f);
                    // Color.Lerpを使って赤色と初期色を補間する
                    timerBarImage.color = Color.Lerp(originalBarColor, Color.red, flashVal);
                }
                else
                {
                    // 通常時
                    timerBarImage.color = originalBarColor;
                }
            }
        }
        else 
        { 
            isTimeUp = true;

            if (timerBarImage != null)
            {
                timerBarImage.color = Color.red;
            }

            // タイムアップ表示テキストをポンと表示
            if (timeUpText != null)
            {
                timeUpText.gameObject.SetActive(true);
                ScoreJuice.PunchScale(timeUpText.gameObject);
            }

            // カメラシェイク（最後のドスンという感覚）
            CameraShake.Shake(0.4f, 0.15f);

            // タイムアップ効果音
            AudioManager.Instance.PlayMissSound();

            // 即時遷移せず、少し余韻を残す（1.2秒待機）
            StartCoroutine(TransitionDelayRoutine());
        }
    }

    // シーン遷移の遅延処理
    private IEnumerator TransitionDelayRoutine()
    {
        // 1.2秒待機
        yield return new WaitForSeconds(1.2f);
        if (SceneController.instance != null)
        {
            SceneController.instance.SceneChange();
        }
    }
}
