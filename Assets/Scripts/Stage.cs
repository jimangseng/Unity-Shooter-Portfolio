using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage : MonoBehaviour
{
    public float remainingTime;
    public float elapsedTime;

    // Start is called before the first frame update
    void Start()
    {
        remainingTime = 10.0f;
        elapsedTime = 0.0f;
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
