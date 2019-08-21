using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineThicknes : MonoBehaviour
{
    [SerializeField] private Material outline;
    private Transform cam;
    public float distanceMin = 5;
    public float distanceMax = 20;
    public float distance;
    public float thickness;
    public float distanceNormalized;
    // Start is called before the first frame update
    void Start()
    {
        outline = GetComponent<Renderer>().material;
        cam = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        distance = Vector3.Distance(cam.position, this.transform.position);
        thickness = Mathf.Lerp(0.05f, 0.17f, distance / (distanceMin - distanceMax));
        distanceNormalized = distance / (distanceMin - distanceMax);
        if (distance < distanceMax) {
            
            outline.SetFloat("_Outline", Mathf.Lerp(0.05f, 0.20f, distance / (distanceMax - distanceMin))) ;
        }
    }
}
