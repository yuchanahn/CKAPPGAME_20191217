using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    Animator anim;
    Rigidbody rd;
    float h;
    float v;
    float speed = 10f;
    float JumpSpeed = 10f;
    [SerializeField] Transform CamView;
    Transform CamTr;

    DetectGround dg;
    Gravity gv;

    public bool IsJumping { get; private set; }

    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        rd = GetComponent<Rigidbody>();
        dg = GetComponent<DetectGround>();
        gv = GetComponent<Gravity>();
        CamTr = GetComponentInChildren<Camera>().transform;
    }

    void Update()
    {
        float rotSpeed = 1.0f;
        
        var MouseY = -1 * Input.GetAxis("Mouse Y") * rotSpeed;

        CamView.Rotate(Vector3.right * rotSpeed * MouseY);
        float angle = Vector3.Angle(transform.forward, CamView.forward);

        float maxAngle = 45;
        if (angle > maxAngle)
        {
            if (CamView.forward.y > 0)
            {
                CamView.rotation = Quaternion.Euler(-maxAngle, CamView.eulerAngles.y, CamView.eulerAngles.z);
            }
            else if (CamView.forward.y < 0)
            {
                CamView.rotation = Quaternion.Euler(maxAngle, CamView.eulerAngles.y, CamView.eulerAngles.z);
            }
        }

        var MouseX = Input.GetAxis("Mouse X");
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


    }

    private void LateUpdate()
    {

        anim.SetBool("IsGround", dg.IsGround);
    }

    private void FixedUpdate()
    {
        gv.Apply(!dg.IsGround);
        dg.Detect(!IsJumping);
        



        Vector3 Vel = ((transform.forward * v) + (CamView.right * h)).normalized;
        Vel *= speed;
        rd.velocity = new Vector3(Vel.x, rd.velocity.y, Vel.z);
    }




    void Jump()
    {

    }
}