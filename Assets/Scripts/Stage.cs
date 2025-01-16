using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage : MonoBehaviour
{
    public float remainedTime;
    public float elapsedTime;

    // Start is called before the first frame update
    void Start()
    {
        remainedTime = 60.00f;
        elapsedTime = 0.00f;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public IEnumerator StartStage()
    {
        while(true)
        {
            updateTime();

            yield return Time.deltaTime;
        }

    }

    void updateTime()
    {
        elapsedTime += Time.deltaTime;
        remainedTime -= Time.deltaTime;
    }
}
