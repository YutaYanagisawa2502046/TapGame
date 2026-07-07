using UnityEngine;

public class CameraShake : MonoBehaviour
{
    // シングルトンインスタンス
    private static CameraShake instance;
    // 元のカメラ位置を保持する変数
    private Vector3 originalPos;
    // シェイクの持続時間、振幅、減衰係数を保持する変数
    private float shakeDuration = 0f;
    private float shakeAmount = 0.1f;
    private float decreaseFactor = 1.0f;

    void Awake()
    {
        instance = this;
    }

    // カメラが有効になったときに元の位置を保存
    void OnEnable()
    {
        originalPos = transform.localPosition;
    }

    // カメラが無効になったときに元の位置を復元
    void Update()
    {
        // シェイクが有効な場合、ランダムな位置にカメラを移動させる
        if (shakeDuration > 0)
        {
            transform.localPosition = originalPos + Random.insideUnitSphere * shakeAmount;
            shakeDuration -= Time.deltaTime * decreaseFactor;
        }
        else
        {
            shakeDuration = 0f;
            transform.localPosition = originalPos;
        }
    }

    // シェイクを開始するための静的メソッド
    public static void Shake(float duration, float amount)
    {
        // シングルトンインスタンスが存在しない場合、メインカメラにCameraShakeコンポーネントを追加
        if (instance == null)
        {
            Camera mainCam = Camera.main;
            if (mainCam != null)
            {
                instance = mainCam.gameObject.AddComponent<CameraShake>();
                instance.originalPos = mainCam.transform.localPosition;
            }
        }

        // シングルトンインスタンスが存在する場合、シェイクの持続時間と振幅を更新
        if (instance != null)
        {
            instance.shakeDuration = Mathf.Max(instance.shakeDuration, duration);
            instance.shakeAmount = Mathf.Max(instance.shakeAmount, amount);
        }
    }
}
