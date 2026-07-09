using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    // シングルトンパターンのインスタンス
    public static ScoreManager instance;
    public static int score = 0;
    public static int combo = 0;

    // UIのスコア表示用テキスト
    public TextMeshProUGUI scoreText;

    // コンボタイマーの管理
    private static float comboTimer = 0f;
    private const float comboDuration = 1.2f; // コンボが続く時間（秒）

    private void Awake()
    {
        if (instance == null) 
            instance = this;
           
        score = 0;
        combo = 0;
        comboTimer = 0f;
    }

    private void Start()
    {
        // 初期スコア表示の更新
        if (scoreText != null) 
            scoreText.text = "Score: " + score;
    }

    private void Update()
    {
        // コンボタイマーの更新
        if (combo > 0)
        {
            comboTimer -= Time.deltaTime;
            if (comboTimer <= 0f)
            {
                combo = 0;
            }
        }
    }

    // スコアを追加する基本的なメソッド
    public void AddScore(int amount)
    {
        AddScore(amount, Vector3.zero);
    }

    /// <summary>
    /// スコアを追加し、コンボを管理するメソッド
    /// </summary>
    /// <param name="baseAmount">追加する基本スコア</param>
    /// <param name="position">フローティングテキストを表示する位置</param>
    public void AddScore(int baseAmount, Vector3 position)
    {
        // コンボを増加
        combo++;
        comboTimer = comboDuration;

        // コンボ倍率を適用して最終スコアを計算   
        int finalAmount = baseAmount * combo;
        score += finalAmount;

        // スコア表示を更新して視覚的なパンチスケールを適用
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
            ScoreJuice.PunchScale(scoreText.gameObject);
        }

        // フローティングテキストを生成
        string displayText = "+" + finalAmount;
        Color displayColor;

        if (combo > 1)
        {
            displayText += "\n" + combo + " Combo!";
            // 高いコンボの場合、色を黄色からオレンジ/赤に補間
            displayColor = Color.Lerp(Color.yellow, new Color(1f, 0.3f, 0f), (combo - 2) / 8f);
        }
        else
        {
            // デフォルトのヒットカラー: 明るい緑/シアン
            displayColor = new Color(0.2f, 1f, 0.4f);
        }

        // 位置がゼロでない場合のみフローティングテキストを生成（ターゲットの少し上に表示）
        if (position != Vector3.zero)
        {
            FloatingText.Create(position, displayText, displayColor);
        }
    }
}

