using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Spawn : MonoBehaviour
{ 

    [SerializeField] private GameObject spawnObject;    // スポーンオブジェクト
    [SerializeField] private Camera mainCamera; // カメラ
    public float spawnInterval = 1.2f;  // 初期スポーン間隔
    
    [Header("Acceleration Settings")]
    [SerializeField] private float minSpawnInterval = 0.35f; // スポーン間隔の最小値
    [SerializeField] private float accelerationRate = 0.025f; // 1秒ごとに減少する間隔（秒）

    
    private float currentTimer = 0; // スポーン時間計測
    private float baseSpawnInterval;
    private float gameTimer = 0f; // ゲーム開始からの経過時間

    // スポーン数
    public int SpawnNum = 1;

    // Start is called before the first frame update
    void Start()
    {
        baseSpawnInterval = spawnInterval;
    }

    // Update is called once per frame
    void Update()
    {
        if (TimerController.isTimeUp || !TimerController.isFadeFinished) return;

        gameTimer += Time.deltaTime;

        // 時間の経過に合わせてスポーン間隔を短く（加速）していく
        float currentInterval = Mathf.Max(minSpawnInterval, baseSpawnInterval - gameTimer * accelerationRate);

        currentTimer += Time.deltaTime;
        if (currentTimer >= currentInterval) 
        {
            currentTimer = 0;
            SpawnObjectInCamera();
        }
    }

    // カメラ内でのスポーンを行う
    void SpawnObjectInCamera()
    {
        // カメラの表示範囲（ワールド座標）を取得
        float vertExtent = mainCamera.orthographicSize;
        float horzExtent = vertExtent * mainCamera.aspect;

        const float offsetX = 1.0f;
        const float offsetY = 1.5f;
        // 一度に多くのスポーンを行う
        for (int i = 0; i < SpawnNum; i++)
        {
            // 生成位置を画面内のランダムな座標に設定
            float RandomX = Random.Range(-horzExtent, horzExtent - offsetX);
            float RandomY = Random.Range(-vertExtent + offsetY, vertExtent);

            // オブジェクトのZ軸を0に調整
            Vector3 spawnPos = new Vector3(RandomX, RandomY, 0f);
            Vector3 cameraPos = new Vector3(mainCamera.gameObject.transform.position.x, mainCamera.gameObject.transform.position.y, 0f);

            // カメラの現在位置を基準に生成
            Instantiate(spawnObject, spawnPos + cameraPos, Quaternion.identity);
        }
    }
}
