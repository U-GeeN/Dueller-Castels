
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;


//[RequireComponent(typeof (ThirdPersonCharacter))]
[RequireComponent(typeof (Interactable))]
public class ThirdPersonControl : MonoBehaviour
{
    private AnimationController animControl; // A reference to the ThirdPersonCharacter on the object
    private Interactable m_Interactable;
    private Transform m_Cam;                  // A reference to the main camera in the scenes transform

    [SerializeField] float m_Turn;
    private Vector3 m_Move;
    private bool m_Jump;                      // the world-relative desired move direction, calculated from the camForward and user input.
    private bool m_Battle = false;
    private bool m_AttackL;
    private bool m_AttackR;
    private bool m_Interact;
    public Transform t_ManualTarget;
    //public bool isActive = false;

    private void Start()
    {
        // get the third person character ( this should never be null due to require component )
        animControl = GetComponent<AnimationController>();
        m_Interactable = GetComponent<Interactable>();
        

        m_Interactable.OnActionExecute += Activate;
        enabled = false;
    }

    private void Activate(Interactable.ActionOption actionOption, Vector3 destination, Interactable target)
    {
        if (actionOption == Interactable.ActionOption.EnterInteractable)
        {
            enabled = true;
        }
        else
        {
            enabled &= actionOption != Interactable.ActionOption.EnterInteractable;
        }
    }

    private void Update()
    {
        //if (!isActive) { return; }
        if (!m_Jump)
        {
            m_Jump = Input.GetButtonDown("Jump");
        }

        if (Input.GetButtonDown("BattleMode")) 
        {
            animControl.SetCombatMode(m_Battle = !m_Battle);
        }

        if (Cursor.lockState == CursorLockMode.Locked)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                // Call only on change
                animControl.HandleAttack(true);
            }
            if (Input.GetButtonUp("Fire1"))
            {
                animControl.HandleAttack(false);
            }

            if (Input.GetButtonDown("Fire2"))
            {
                // Call only on change
                animControl.HandleBlock(true);
            }
            if (Input.GetButtonUp("Fire2"))
            {
                animControl.HandleBlock(false);
            }

            if (Input.GetButtonDown("FreeMouse"))
            {
                print("mouse free");
            }
        }

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
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            // read inputs
            l = Input.GetAxis("Horizontal");
            v = Input.GetAxis("Vertical");
            h = Input.GetAxis("Mouse X");

        }
        bool crouch = Input.GetKey(KeyCode.C);

        // we use world-relative directions in the case of no main camera
        m_Turn = h;
        m_Move = (v * transform.forward) + (h * transform.right) + 0.25f * l * transform.right;

#if !MOBILE_INPUT
        // walk speed multiplier
        if (Input.GetKey(KeyCode.LeftShift)) m_Move *= 1.5f;
#endif
        
        if (t_ManualTarget != null)
        {
            t_ManualTarget.position = transform.position + m_Move;
        }

        // pass all parameters to the character control script
        //m_Character.Move(m_Move, crouch, m_Jump);
        if (m_Move != Vector3.zero)
        {
            //agent.SetDestination(m_Move);
            //print("destination set " + m_Move + " to " + agent.destination);
        }

        //m_Jump = false;
    }

}

