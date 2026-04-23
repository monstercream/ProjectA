using UnityEngine;

public class Vector3Json
{
    public float x { get; set; }
    public float y { get; set; }
    public float z { get; set; }

    // Vector3로 변환
    public Vector3 ToVector3() => new Vector3(x, y, z);
}