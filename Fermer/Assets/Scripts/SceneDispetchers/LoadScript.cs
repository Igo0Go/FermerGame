using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadScript : MonoBehaviour
{
    public Slider loadSlide;

    AsyncOperation oper;

    void Start()
    {
        oper = SceneManager.LoadSceneAsync(1, LoadSceneMode.Single);
        oper.allowSceneActivation = true;
        loadSlide.maxValue = 100;
    }

    private void Update()
    {
        loadSlide.value = oper.progress;
    }
}
