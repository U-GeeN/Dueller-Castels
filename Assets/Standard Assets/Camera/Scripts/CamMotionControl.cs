using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamMotionControl : MonoBehaviour {
    public float cameraSpeed = 1;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        // Kamera nach rechts bewegen
        if (Input.mousePosition.x > Screen.width * 0.8f  || Input.GetKey(KeyCode.D)){
            var temp = Camera.main.transform.position;
            temp.x += Time.deltaTime * cameraSpeed * 10;
            Camera.main.transform.position = temp;
        }
        if (Input.mousePosition.x < Screen.width * 0.2f || Input.GetKey(KeyCode.A))
        {
            var temp = Camera.main.transform.position;
            temp.x -= Time.deltaTime * cameraSpeed * 10;
            Camera.main.transform.position = temp;
        }
       
	}
}
