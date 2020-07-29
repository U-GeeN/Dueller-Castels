
using UnityStandardAssets.Utility;
using UnityEngine;

public class CameraControl : MonoBehaviour
{

    BirdViewCam birdViewCam;
    SmoothFollow smoothFollow;

	// Use this for initialization
	void Start () {
        birdViewCam = Camera.main.GetComponent<BirdViewCam>();
        smoothFollow = Camera.main.GetComponent<SmoothFollow>();
	}

    /// <summary>
    /// SelectableObject Broadcast. 
    /// </summary>
    /// <param name="value">If set to <c>true</c> value.</param>
    public void OnSetControl (bool value) {

        birdViewCam.enabled = !value;
        // TODO: anzeige zum nächsten waypoint machen und aktivieren
        // Direct Control of Character
       
        smoothFollow.SetActive(value, transform);
        print(name + " hat Kameras umgeschaltet. Selected = " + value);
    }
}
