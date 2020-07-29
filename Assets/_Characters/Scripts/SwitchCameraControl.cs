using UnityEngine;

namespace UnityStandardAssets.Utility
{
    public class SwitchCameraControl : MonoBehaviour
    {
        BirdViewCam birdViewCam;
        SmoothFollow smoothFollow;
        Interactable interactable;

        // Use this for initialization
        void Start()
        {
            birdViewCam = Camera.main.GetComponent<BirdViewCam>();
            smoothFollow = Camera.main.GetComponent<SmoothFollow>();
            SetBirdViewCamera();
        }

        // bind camControl to interactable
        public void Init(Interactable interactable)
        {
            this.interactable = interactable;
            interactable.OnActionExecute += OnSetControl;
            
        }

        public void Deinit()
        {
            interactable = null;
            interactable.OnActionExecute -= OnSetControl;
        }

        public void OnSetControl(Interactable.ActionOption actionOption, Vector3 destination, Interactable target)
        {
            print(name + " activated through event");
            if (actionOption == Interactable.ActionOption.EnterInteractable)
            {
                SetFollowCamera(target.transform);
            }
            else if (actionOption == Interactable.ActionOption.ExitInteractable)
            {
                SetBirdViewCamera();
            }

        }

        public void SetFollowCamera(Transform target) => SetControl(target);

        public void SetBirdViewCamera() => SetControl();

        void SetControl(Transform target = null)
        {
            bool isThirdPerson = target != null;
            if (!birdViewCam) { return; }
            birdViewCam.enabled = !isThirdPerson;
            if (!isThirdPerson) { Cursor.lockState = CursorLockMode.None; }

            // Direct Control of Character
            smoothFollow.SetActive(isThirdPerson, target);
            if (isThirdPerson) { Cursor.lockState = CursorLockMode.Locked; }

        }
    }
}
