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
        timeUI.SetText(stage.remainedTime.ToString("F2"));
        enemyUI.SetText("킬: " + GameManager.Instance.kills.ToString());
    }

    public void startStage()
    {
        stageUI.SetText("스테이지 " + currentStage);
        StartCoroutine(stage.StartStage());
    }
}
