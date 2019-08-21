using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FocusLight : MonoBehaviour {
    [Range(0.0f, 10.0f)]
    public float quality = 5;
    [Range(0.0f, 5.0f)]
    public float dotSize;
    [SerializeField] Transform scopeCenter;
    Transform cameraTransform;
    [SerializeField] Transform scopeLight;

	// Use this for initialization
	void Start () {
        cameraTransform = Camera.main.transform;
	}
	
	// Update is called once per frame
	void Update () {
        var camPos = scopeCenter.InverseTransformPoint(cameraTransform.position);
        camPos.z = -camPos.z/quality;
        var camPosMirrored = scopeCenter.TransformPoint(camPos);
        scopeLight.LookAt(camPosMirrored);

	}
}
