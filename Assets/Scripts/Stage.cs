using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage : MonoBehaviour
{
    public float remainingTime { get; set; } = 10.0f;
    public float elapsedTime { get; set; } = 0.0f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public IEnumerator updateStage()
    {
        while(true)
        {
            elapsedTime += Time.deltaTime;
            remainingTime -= Time.deltaTime;

            yield return Time.deltaTime;
        }

    }
}
