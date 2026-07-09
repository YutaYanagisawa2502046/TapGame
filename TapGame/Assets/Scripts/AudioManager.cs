using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    // シングルトンパターンのインスタンス
    private static AudioManager instance;

    // シングルトンのインスタンスを取得するためのプロパティ
    public static AudioManager Instance
    {
        get
        {
            // すでにインスタンスが存在する場合はそれを返す
            if (instance == null)
            {
                // 新しいGameObjectを作成してAudioManagerをアタッチする
                GameObject go = new GameObject("AudioManager");
                instance = go.AddComponent<AudioManager>();
                DontDestroyOnLoad(go);
            }
            return instance;
        }
    }

    // AudioSourceを保持するための変数
    private AudioSource audioSource;

    void Awake()
    {
        // シングルトンのインスタンスを設定
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // AudioSourceコンポーネントを追加
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    // Tap音を再生するためのメソッド
    public void PlayTapSound()
    {
        // 880Hzのトーンを生成して再生
        AudioClip clip = CreateToneClip(880f, 0.15f, 0.03f, 0.12f); // 880Hz (A5), fast decay
        audioSource.PlayOneShot(clip, 0.7f);
    }

    // Miss音を再生するためのメソッド
    public void PlayMissSound()
    {
        // 150Hzのトーンを生成して再生
        AudioClip clip = CreateToneClip(150f, 0.25f, 0.02f, 0.23f, isThud: true); // 150Hz, quick pitch drop
        audioSource.PlayOneShot(clip, 0.6f);
    }

    // 効果音を再生するためのメソッド
    public void PlayResultSound()
    {
        // 523.25Hz, 659.25Hz, 783.99Hz, 1046.50Hzの順に再生するアルペジオを生成して再生
        StartCoroutine(PlayArpeggioRoutine());
    }

    // アルペジオを再生するコルーチン
    private IEnumerator PlayArpeggioRoutine()
    {
        float[] freqs = { 523.25f, 659.25f, 783.99f, 1046.50f }; // C5, E5, G5, C6 (Cメジャーアルペジオ)
        foreach (float f in freqs)
        {
            // 0.2秒のトーンを生成して再生
            AudioClip clip = CreateToneClip(f, 0.2f, 0.02f, 0.18f);
            audioSource.PlayOneShot(clip, 0.7f);
            yield return new WaitForSeconds(0.12f);
        }
    }

    // 高得点（100,000点以上）用の豪華なファンファーレを再生するメソッド
    public void PlayGrandResultSound()
    {
        StartCoroutine(PlayGrandFanfareRoutine());
    }

    /// <summary>
    /// 豪華なファンファーレのアルペジオを再生するコルーチン
    /// </summary>
    /// <returns>WaitForSeconds</returns>
    private IEnumerator PlayGrandFanfareRoutine()
    {
        // お祝いのファンファーレのアルペジオを生成して再生
        float[] freqs = { 523.25f, 659.25f, 783.99f, 1046.50f, 783.99f, 1046.50f }; // C5, E5, G5, C6, G5, C6
        float[] durations = { 0.1f, 0.1f, 0.1f, 0.15f, 0.1f, 0.5f };// 各トーンの長さ
        float[] delays = { 0.12f, 0.12f, 0.12f, 0.18f, 0.12f, 0.5f };// 各トーンの間隔（次のトーンまでの待機時間）

        for (int i = 0; i < freqs.Length; i++)
        {
            AudioClip clip = CreateToneClip(freqs[i], durations[i], 0.02f, durations[i] - 0.02f);
            audioSource.PlayOneShot(clip, 0.8f);
            yield return new WaitForSeconds(delays[i]);
        }
    }

    /// <summary>
    /// メモリ内でトーンを生成するためのヘルパーメソッド
    /// </summary>
    /// <param name="frequency">周波数 (Hz)</param>
    /// <param name="duration">長さ (秒)</param>
    /// <param name="attack">音のピークまでの時間 (秒)</param>
    /// <param name="decay">減衰時間 (秒)</param>
    /// <param name="isThud">ドスン音かどうか</param>
    /// <returns>生成されたAudioClip</returns>
    private AudioClip CreateToneClip(float frequency, float duration, float attack, float decay, bool isThud = false)
    {
        // サンプルレートとサンプル数を計算
        int sampleRate = 44100;
        int sampleCount = (int)(sampleRate * duration);
        float[] samples = new float[sampleCount];

        // サンプルごとに波形を生成
        for (int i = 0; i < sampleCount; i++)
        {
            float time = (float)i / sampleRate;
            
            // エンベロープ
            float envelope = 0f;
            if (time < attack)
            {
                envelope = time / attack;
            }
            else
            {
                float decayTime = time - attack;
                envelope = Mathf.Exp(-decayTime / (decay * 0.5f));
            }

            // 周波数をスライドさせる場合の処理
            float freq = frequency;
            if (isThud)
            {
                // 低い周波数にスライドさせる
                freq = Mathf.Max(40f, frequency * (1f - (time / duration) * 0.8f));
            }

            // Sine波を生成してサンプルに格納
            samples[i] = Mathf.Sin(2f * Mathf.PI * freq * time) * envelope;
        }

        // AudioClipを生成してサンプルデータを設定
        AudioClip clip = AudioClip.Create("ProceduralTone", sampleCount, 1, sampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }
}
