using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobMove : MonoBehaviour
{
    public Vector3 TargetPos;
    public float ptime;
    Animator anim;
    private void Awake()
    {
        anim = GetComponent<Animator>();
    }
    void Update()
    {
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
        transform.position = Vector3.Lerp(transform.position, TargetPos, ptime);
        ptime += Time.deltaTime;
    }
}
