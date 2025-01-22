using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] TextMeshProUGUI timeUI;
    [SerializeField] TextMeshProUGUI stageUI;
    [SerializeField] TextMeshProUGUI enemyUI;

    [Header("Stage")]
    [SerializeField] Stage stage;

    public int CurrentStage { get; set; } = 1;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        timeUI.SetText(stage.remainingTime.ToString("F2"));
        enemyUI.SetText("킬: " + GameManager.Instance.Kills.ToString());

        if(stage.remainingTime < 0.0f)
        {
            stopStage();
        }
    }

    public void startStage()
    {
        stageUI.SetText("스테이지 " + CurrentStage);

        StartCoroutine(stage.updateStage());
    }

    public void stopStage()
    {
        CurrentStage++;

        stageUI.SetText("스테이지 " + CurrentStage);

        StopCoroutine(stage.updateStage());

        stage.remainingTime = 10.0f;
        stage.elapsedTime = 0.0f;
    }
}
