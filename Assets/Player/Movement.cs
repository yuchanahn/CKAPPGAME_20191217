using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    // components 
    [SerializeField] Transform mCameraViewerObject;

    Animator mAnimator;
    Rigidbody mRigidbody;
    Transform mCameraTransfrom;
    DetectGround mDetectGround;
    Gravity mGravity;

    [SerializeField] float Speed = 10f;
    [SerializeField] float JumpSpeed = 10f;
    [SerializeField] float JumpTime = 1;
    [SerializeField] float CameraRotateSpeed = 1.0f;

    bool bJumpStart;
    float h;
    float v;

    public bool IsJumping { get; private set; }

    void Start()
    {
        mAnimator = GetComponentInChildren<Animator>();
        mRigidbody = GetComponent<Rigidbody>();
        mDetectGround = GetComponent<DetectGround>();
        mGravity = GetComponent<Gravity>();
        mCameraTransfrom = GetComponentInChildren<Camera>().transform;
    }

    void Update()
    {
        var MouseY = -1 * Input.GetAxis("Mouse Y") * CameraRotateSpeed;
        mCameraViewerObject.Rotate(Vector3.right * CameraRotateSpeed * MouseY);
        float angle = Vector3.Angle(transform.forward, mCameraViewerObject.forward);
        float maxAngle = 45;
        if (angle > maxAngle)
        {
            if (mCameraViewerObject.forward.y > 0)
            {
                mCameraViewerObject.rotation = Quaternion.Euler(-maxAngle, mCameraViewerObject.eulerAngles.y, mCameraViewerObject.eulerAngles.z);
            }
            else if (mCameraViewerObject.forward.y < 0)
            {
                mCameraViewerObject.rotation = Quaternion.Euler(maxAngle, mCameraViewerObject.eulerAngles.y, mCameraViewerObject.eulerAngles.z);
            }
        }

        var MouseX = Input.GetAxis("Mouse X");
        transform.Rotate(Vector3.up * CameraRotateSpeed * MouseX);

        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");

        Animation();

        if (Input.GetKeyDown(KeyCode.Space)) Jump();
    }

    private void LateUpdate()
    {
        mAnimator.SetBool("IsGround", !IsJumping && mDetectGround.IsGround);
        mAnimator.SetBool("Jump", IsJumping);
    }

    void Animation()
    {
        Vector2 v2 = new Vector2(h, v);

        mAnimator.SetFloat("XVel", Vector2.ClampMagnitude(v2, 1).x);
        mAnimator.SetFloat("ZVel", Vector2.ClampMagnitude(v2, 1).y);
        mAnimator.SetFloat("Walk", v2.magnitude);

        if (h != 0 && v != 0)
        {
            mAnimator.SetFloat("Speed", v);
        }
        else if (v == 0 && h != 0)
        {
            mAnimator.SetFloat("Speed", 1);
        }
        else if (v != 0 && h == 0)
        {
            mAnimator.SetFloat("Speed", v);
        }
    }

    private void FixedUpdate()
    {
        mGravity.Apply(!mDetectGround.IsGround);
        mDetectGround.Detect(!IsJumping);
        if (bJumpStart || IsJumping) Jumping();

        Vector3 Vel = ((transform.forward * v) + (mCameraViewerObject.right * h)).normalized;
        Vel *= Speed;
        mRigidbody.velocity = new Vector3(Vel.x, mRigidbody.velocity.y, Vel.z);
    }

    void Jump()
    {
        if (!mDetectGround.IsGround) return;
        bJumpStart = true;
    }

    void Jumping()
    {
        bJumpStart = false;
        IsJumping = true;
        mRigidbody.velocity = new Vector3(mRigidbody.velocity.x, mRigidbody.velocity.y + JumpSpeed * Time.fixedDeltaTime, mRigidbody.velocity.z);
        StartCoroutine(JumpTimer(JumpTime));
    }

    IEnumerator JumpTimer(float t)
    {
        yield return new WaitForSeconds(t);
        IsJumping = false;
    }

}