using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectGround : MonoBehaviour
{


    [SerializeField] float outtersnapDist = 0.1f;
    [SerializeField] float offset = 0.2f;
    [SerializeField] LayerMask layer;

    CapsuleCollider capsuleCollider;
    Rigidbody rd;

    private void Awake()
    {
        rd = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
    }

    public bool IsGround { get; private set; }
    
    public void Detect(bool IsDetect)
    {
        IsGround = false;
        if (!IsDetect) return;


        Vector3 point1 = transform.position;
        point1.y -= (capsuleCollider.height / 2) - (capsuleCollider.radius);
        point1.y += offset;
        Vector3 point2 = transform.position;
        point2.y += (capsuleCollider.height / 2) - (capsuleCollider.radius);
        point2.y += offset;


        var cast = Physics.CapsuleCastAll(point1, point2, capsuleCollider.radius, Vector3.down, outtersnapDist + offset, layer);
        if (cast.Length == 0) return;

        float? dist = null;
        foreach (var i in cast)
        {
            var d = i.distance - offset;
            dist = dist.HasValue ? dist > d ? d : dist : d;
            //if (transform.position.y + d <= transform.position.y)
            //{
                
            //}
        }
        if (!dist.HasValue) return;

        transform.position += Vector3.down * dist.Value;
        IsGround = true;
        rd.velocity = new Vector3(rd.velocity.x, 0, rd.velocity.z);
    }
}
