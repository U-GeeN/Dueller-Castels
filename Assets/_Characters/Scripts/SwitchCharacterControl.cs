using UnityStandardAssets.Characters.ThirdPerson;
using UnityEngine;
using UnityEngine.AI;

namespace UnityStandardAssets.Utility
{
    public class SwitchCharacterControl: MonoBehaviour
    {
        NavMeshAgent agent;
        AICharacterControl AIContol;
        ThirdPersonControl control;
        BirdViewCam birdViewCam;
        SmoothFollow smoothFollow;
        Interactable interactable;

        // Use this for initialization
        void Start()
        {
            OnStart();
        }

        public void OnStart () {
            interactable = GetComponent<Interactable>();
            interactable.OnActionExecute += OnSetControl;
            agent = GetComponent<NavMeshAgent>();
            AIContol = GetComponent<AICharacterControl>();
            control = GetComponent<ThirdPersonControl>();
            birdViewCam = Camera.main.GetComponent<BirdViewCam>();
            smoothFollow = Camera.main.GetComponent<SmoothFollow>();
        }

        /// <summary>
        /// SelectableObject Broadcast. 
        /// </summary>
        /// <param name="value">If set to <c>true</c> value.</param>
        public void OnSetControl(Interactable.ActionOption actionOption, Vector3 destination, Interactable target)
        {
            if (actionOption != Interactable.ActionOption.EnterInteractable) { return; }

            var isThirdPersonControlled = control.isActiveAndEnabled;
            // switch control
            isThirdPersonControlled = !isThirdPersonControlled;

            // AI Navigation 
            //AIContol. set primary target
            birdViewCam.enabled = !isThirdPersonControlled;
            if (!isThirdPersonControlled) { Cursor.lockState = CursorLockMode.None; }

            // Direct Control of Character
            control.enabled = isThirdPersonControlled;
            smoothFollow.SetActive(isThirdPersonControlled, transform);

            //TODO: in die Init vom thirdpersoncontrol werfen
            if (isThirdPersonControlled) { Cursor.lockState = CursorLockMode.Locked; }
            print(name + " hat Kameras umgeschaltet. Selected = " + isThirdPersonControlled);
        }

    }
}
