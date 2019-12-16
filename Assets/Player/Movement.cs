using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FlatBuffers;

public class Movement : MonoBehaviour
{
    public int id = -1;
    Animator anim;
    float h;
    float v;

    public int y__rot = 0;
    [SerializeField, Range(0f, 0.5f)] float sendRate = 0.1f;

    public GameObject Boom;

    float sendRateT = 0;

    public Vector3 TargetPos;
    public float ptime;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        TargetPos = transform.position;
    }



    // Update is called once per frame
    void Update()
    {
        if (Server__TCP.Instance.MY_ID != id)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, y__rot, 0), Time.deltaTime);

            var vel = TargetPos - transform.position;
            anim.SetFloat("XVel", Vector2.ClampMagnitude(vel, 1).x);
            anim.SetFloat("ZVel", Vector2.ClampMagnitude(vel, 1).y);
            anim.SetFloat("Walk", vel.magnitude);

            if (vel.x != 0 && vel.z != 0)
            {
                anim.SetFloat("Speed", vel.z);
            }
            else if (vel.z == 0 && vel.x != 0)
            {
                anim.SetFloat("Speed", 1);
            }
            else if (vel.z != 0 && vel.x == 0)
            {
                anim.SetFloat("Speed", vel.z);
            }
        }
        transform.position = Vector3.Lerp(transform.position, TargetPos, ptime / sendRate);
        ptime += Time.deltaTime;
        if (Server__TCP.Instance.MY_ID != id) return;


        float rotSpeed = 1.0f; //ADD
        float MouseX = Input.GetAxis("Mouse X");

        transform.Rotate(Vector3.up * rotSpeed * MouseX);

        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");

        Vector2 v2 = new Vector2(h, v);

        anim.SetFloat("XVel", Vector2.ClampMagnitude(v2, 1).x);
        anim.SetFloat("ZVel", Vector2.ClampMagnitude(v2, 1).y);
        anim.SetFloat("Walk", v2.magnitude);

        if (h != 0 && v != 0)
        {
            anim.SetFloat("Speed", v);
        }
        else if (v == 0 && h != 0)
        {
            anim.SetFloat("Speed", 1);
        }
        else if (v != 0 && h == 0)
        {
            anim.SetFloat("Speed", v);
        }

        sendRateT += Time.deltaTime;

        if (sendRateT > sendRate)
        {
            Vector3 nextPos = transform.position;
            var vl = (Camera.main.transform.forward * v) + (Camera.main.transform.right * h);

            nextPos += vl * 3 * sendRateT;

            var fbb = new FlatBufferBuilder(1);
            fbb.Finish(fplayer.Createfplayer(fbb, eFB_Type.fplayer, 0, default(StringOffset), 0, 0, 0, 0, 0, 0, nextPos.x, nextPos.z, Mathf.RoundToInt(transform.rotation.y), 0).Value);
            Server__TCP.Instance.Send(fbb.SizedByteArray());
            sendRateT = 0;
        }
    }
}