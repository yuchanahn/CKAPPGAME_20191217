using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Ping : MonoBehaviour
{

    [SerializeField, Range(0.01f, 1f)] float TestPing_Rate;
    [SerializeField] Text UI_PingM;

    float dt = -2f;

    void Update()
    {
        UI_PingM.text = Server__TCP.__ping.ToString();

        dt += Time.deltaTime;
        if (dt > TestPing_Rate)
        {
            dt = 0;
            Server__TCP.TestPing();
        }
    }
}
