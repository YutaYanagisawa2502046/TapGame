using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    [SerializeField] FadeController m_fadeController;

    public static SceneController instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    // Start is called before the first frame update
    // 呼び出される -> シーンが始まると解釈する
    void Start()
    {
        m_fadeController.sceneChageFade = false;
        m_fadeController.FadeIn();
    }

    // シーンを変える
    public void SceneChange()
    {
        m_fadeController.sceneChageFade = true;
        m_fadeController.FadeOut();
    }
}
