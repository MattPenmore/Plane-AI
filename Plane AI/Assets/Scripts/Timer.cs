using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    float time = 0;
    public bool reachedEnd = false;

    [SerializeField]
    Text text;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!reachedEnd)
        {
            time += Time.deltaTime;

            text.text = time.ToString("F2");
        }
    }
}
