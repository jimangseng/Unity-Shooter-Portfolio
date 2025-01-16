using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public TextMeshProUGUI timeUI;
    public TextMeshProUGUI stageUI;
    public TextMeshProUGUI enemyUI;

    public Stage stage;

    int currentStage;

    // Start is called before the first frame update
    void Start()
    {
        currentStage = 1;
    }

    // Update is called once per frame
    void Update()
    {
        timeUI.SetText(stage.remainingTime.ToString("F2"));
        enemyUI.SetText("킬: " + GameManager.Instance.kills.ToString());

        if(stage.remainingTime < 0.0f)
        {

            stopStage();
        }
    }

    public void startStage()
    {
        stageUI.SetText("스테이지 " + currentStage);

        StartCoroutine(stage.updateStage());
    }

    public void stopStage()
    {
        currentStage++;

        stageUI.SetText("스테이지 " + currentStage);

        StopCoroutine(stage.updateStage());

        stage.remainingTime = 10.0f;
        stage.elapsedTime = 0.0f;
    }
}
