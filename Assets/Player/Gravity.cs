using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gravity : MonoBehaviour
{
    Rigidbody rd;


    private void Awake()
    {
        rd = GetComponent<Rigidbody>();
    }

    public void Apply(bool UseGravity)
    {
        if (!UseGravity) return;
        Debug.Log("중력적용.");
        rd.velocity = new Vector3(rd.velocity.x, rd.velocity.y + (-10 * Time.fixedDeltaTime), rd.velocity.y);
    }
}
