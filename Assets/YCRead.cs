using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FlatBuffers;
using System;

public class YCRead : MonoBehaviour
{
    public static Dictionary<eFB_Type, Action<Base>> REACT = new Dictionary<eFB_Type, Action<Base>>();
}
