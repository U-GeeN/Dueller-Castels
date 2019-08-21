
using UnityEngine;
using UnityStandardAssets.Utility;

namespace UnityStandardAssets.Characters.ThirdPerson                    
{
    [RequireComponent(typeof (ThirdPersonCharacter))]
    [RequireComponent(typeof (Interactable))]
    public class ThirdPersonControl : MonoBehaviour
    {
        private ThirdPersonCharacter m_Character; // A reference to the ThirdPersonCharacter on the object
        private Interactable m_Interactable;
        private AICharacterControl m_AiControl;
        private Transform m_Cam;                  // A reference to the main camera in the scenes transform

        [SerializeField] float m_Turn;
        private Vector3 m_Move;
        private bool m_Jump;                      // the world-relative desired move direction, calculated from the camForward and user input.
        private bool m_Battle = false;
        private bool m_AttackL;
        private bool m_AttackR;
        private bool m_Interact;
        public Transform t_ManualTarget;

        private void Start()
        {
            // get the third person character ( this should never be null due to require component )
            m_Character = GetComponent<ThirdPersonCharacter>();
            m_Interactable = GetComponent<Interactable>();
            m_AiControl = GetComponent<AICharacterControl>();
        }


        private void Update()
        {
            if (!m_Jump)
            {
                m_Jump = Input.GetButtonDown("Jump");
            }

            if (Input.GetButtonDown("BattleMode")) 
            {
                m_Character.SetBattleMode(m_Battle = !m_Battle);
            }

            if (Cursor.lockState == CursorLockMode.Locked)
            {
                if (Input.GetButtonDown("Fire1"))
                {
                    // Call only on change
                    m_Character.SetAttack(true);
                }
                if (Input.GetButtonUp("Fire1"))
                {
                    m_Character.SetAttack(false);
                }

                if (Input.GetButtonDown("Fire2"))
                {
                    // Call only on change
                    m_Character.SetBlock(true);
                }
                if (Input.GetButtonUp("Fire2"))
                {
                    m_Character.SetBlock(false);
                }

                if (Input.GetButtonDown("FreeMouse"))
                {
                    print("mouse free");
                }
            }

            // TODO:
            if( Input.GetKeyDown(KeyCode.LeftApple))
            {
                Cursor.lockState = CursorLockMode.None;
            }
            if (Input.GetKeyUp(KeyCode.LeftApple))
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
        }


        // Fixed update is called in sync with physics
        private void FixedUpdate()
        {
            float l = 0, v = 0, h = 0;
            if (Cursor.lockState == CursorLockMode.Locked || true)
            {
                // read inputs
                l = Input.GetAxis("Horizontal");
                v = Input.GetAxis("Vertical");
                h = Input.GetAxis("Mouse X");

            }
            bool crouch = Input.GetKey(KeyCode.C);

            // we use world-relative directions in the case of no main camera
            m_Turn = h;
            m_Character.SetRotation(m_Turn);
            m_Move = v * transform.forward + l * transform.right;
            if (t_ManualTarget != null)
            {
                t_ManualTarget.position = transform.position + m_Move;
            }

#if !MOBILE_INPUT
            // walk speed multiplier
            if (Input.GetKey(KeyCode.LeftShift)) m_Move *= 1.5f;
#endif

            // pass all parameters to the character control script
            //m_Character.Move(m_Move, crouch, m_Jump);
            if (m_Move != Vector3.zero)
            {
                //agent.SetDestination(m_Move);
                //print("destination set " + m_Move + " to " + agent.destination);
            }

            //m_Jump = false;
        }


        // Fixed update is called in sync with physics
        //        private void FixedUpdate()
        //        {
        //            float l = 0, v = 0, h = 0;
        //            if (Cursor.lockState == CursorLockMode.Locked)
        //            {
        //                // read inputs
        //                l = Input.GetAxis("Horizontal");
        //                v = Input.GetAxis("Vertical");
        //                h = Input.GetAxis("Mouse X");

        //            }
        //            bool crouch = Input.GetKey(KeyCode.C);
        //            // calculate move direction to pass to character
        //            if (m_Cam != null)
        //            {
        //                // calculate camera relative direction to move:
        //                m_CamForward = Vector3.Scale(m_Cam.forward, new Vector3(1, 0, 1)).normalized;
        //                m_Move = v * m_CamForward + h * m_Cam.right;
        //            }
        //            else
        //            {
        //                // we use world-relative directions in the case of no main camera
        //                m_Move = v * transform.forward + h * transform.right + l * transform.up;
        //            }
        //#if !MOBILE_INPUT
        //			// walk speed multiplier
        //	        if (Input.GetKey(KeyCode.LeftShift)) m_Move *= 0.5f;
        //#endif

        //    // pass all parameters to the character control script
        //    //m_Character.Move(m_Move, crouch, m_Jump);

        //    m_Jump = false;
        //}
    }
}
