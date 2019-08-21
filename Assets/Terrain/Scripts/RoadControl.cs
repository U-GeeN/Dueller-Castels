using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadControl : MonoBehaviour
{
    public Material roadMask;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<MeshRenderer>().material = roadMask;
    }
}
