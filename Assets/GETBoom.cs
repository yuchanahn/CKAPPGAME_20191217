using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FlatBuffers;

public class GETBoom : MonoBehaviour
{
    public static float coolT = 0;
    public static float cool = 3f;
    private void OnTriggerEnter(Collider other)
    {
        if (coolT < cool) return;
        coolT = 0;
        if (other.tag.Contains("Player"))
        {
            if(other.gameObject.GetComponent<Movement>().Boom.activeSelf) // 플레이어가 폭탄이면.
            {
                var fbb = new FlatBufferBuilder(1);
                fbb.Finish(fboom.Createfboom(fbb, eFB_Type.fboom, 0).Value);
                Server__TCP.Instance.Send(fbb.SizedByteArray());
            }
            else if(gameObject.GetComponentInParent<Movement>().Boom.activeSelf) // 자신이 폭탄이면.
            {
                var fbb = new FlatBufferBuilder(1);
                fbb.Finish(fboom.Createfboom(fbb, eFB_Type.fboom, 1).Value);
                Server__TCP.Instance.Send(fbb.SizedByteArray());
            }
        }
    }

    private void Update()
    {
        coolT += Time.deltaTime;
    }
}
