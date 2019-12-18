using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gravity : MonoBehaviour
{
    [SerializeField] float GravityScale;
    Rigidbody rd;

    private void Awake()
    {
        rd = GetComponent<Rigidbody>();
    }

    public void Apply(bool UseGravity)
    {
        if (!UseGravity) return;
        rd.velocity = new Vector3(rd.velocity.x, rd.velocity.y + (-GravityScale * Time.fixedDeltaTime), rd.velocity.y);
    }
}
