using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    [SerializeField]
    float timeScale;
    public float time = 0;
    public bool reachedEnd = false;

    [SerializeField]
    Text text;
    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = timeScale;
        //Application.targetFrameRate = 60;
    }

    // Update is called once per frame
    void Update()
    {
        Time.timeScale = timeScale;
        if (!reachedEnd && Time.time > 2)
        {
            time += Time.deltaTime;

            text.text = time.ToString("F2");
        }
    }
}
