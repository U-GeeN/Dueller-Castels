using UnityEngine;

namespace UnityStandardAssets.Characters.ThirdPerson
{
	[RequireComponent(typeof(Rigidbody))]
	[RequireComponent(typeof(CapsuleCollider))]
	[RequireComponent(typeof(Animator))]
	public class ThirdPersonCharacter : MonoBehaviour
	{
		[SerializeField] float m_MovingTurnSpeed = 360;
		[SerializeField] float m_StationaryTurnSpeed = 180;
		[SerializeField] float m_JumpPower = 12f;
		[Range(1f, 4f)][SerializeField] float m_GravityMultiplier = 2f;
		[SerializeField] float m_RunCycleLegOffset = 0.2f; //specific to the character in sample assets, will need to be modified to work with others
		[SerializeField] float m_MoveSpeedMultiplier = 1f;
		[SerializeField] float m_AnimSpeedMultiplier = 1f;
		[SerializeField] float m_GroundCheckDistance = 0.1f;
        [SerializeField] GameObject weaponScabbard;
        [SerializeField] GameObject weaponHand;

		Rigidbody m_Rigidbody;
		Animator m_Animator;
		bool m_IsGrounded;
		float m_OrigGroundCheckDistance;
		const float k_Half = 0.5f;
        [SerializeField] float m_TurnAmount;
		float m_ForwardAmount;
        float m_StrafeAmmount;
		Vector3 m_GroundNormal;
		float m_CapsuleHeight;
		Vector3 m_CapsuleCenter;
		CapsuleCollider m_Capsule;
		bool m_Crouching;
        [SerializeField] bool m_IsBlocking;

        [Header("Testvariablen")]

        public float speed;
        Vector3 lastPosition;


        private void DebugUpdate()
        {
            speed = Vector3.Distance(lastPosition, transform.position);
            lastPosition = transform.position;
        }


		void Start()
		{
			m_Animator = GetComponent<Animator>();
			m_Rigidbody = GetComponent<Rigidbody>();
			m_Capsule = GetComponent<CapsuleCollider>();
			m_CapsuleHeight = m_Capsule.height;
			m_CapsuleCenter = m_Capsule.center;

			m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
			m_OrigGroundCheckDistance = m_GroundCheckDistance;
		}
        #region Set Animator Parameters
        public void BlockHit ()
        {
            DebugUpdate();

            if (m_IsBlocking)
            {
                m_Animator.SetTrigger("HitTrigger");
            }
            else
            {
                m_Animator.SetBool("Attack",false);
            }
        }

        // what happens when it's blocked?
        public void HitControl (Vector3 hitDirection, bool incapacitate) {
            print(name + " incapacitaded " + incapacitate);
            m_Animator.SetFloat("HitDirectionX", hitDirection.x);
            m_Animator.SetFloat("HitDirectionZ", hitDirection.z);
            m_Animator.SetTrigger("HitTrigger");
            m_Animator.SetBool("Incapacitated", incapacitate);
        }

        /// <summary>
        /// Starts / aborts the Attack Animation depending on isAttacking. AttackSelect specifies .
        /// </summary>
        /// <param name="isAttacking">If set to <c>true</c> is attacking.</param>
        /// <param name="attackSelect">Attack select.</param>
        public void SetAttack (bool isAttacking, float attackSelect = 3f)
        {
            m_Animator.SetBool("Attack", isAttacking);
            m_Animator.SetFloat("AttackSelect", attackSelect);
        }

       

        public void SetBattleMode (bool isInBattleMode)
        {
            m_Animator.SetBool("BattleMode", isInBattleMode);
        }

        public void SetBlock (bool isBlocking, float blockSelect = 3f)
        {
            m_Animator.SetBool("Block", isBlocking);
            m_Animator.SetFloat("AttackSelect", blockSelect);
            m_IsBlocking = isBlocking;
            print("block is " + isBlocking);
            if (!isBlocking)
            {
                OnBlockExit();
            }
        }

        /// <summary>
        /// Move the specified move, turn, crouch and jump.
        /// </summary>
        /// <param name="move">Move.</param>
        /// <param name="turn">Turn.</param>
        /// <param name="crouch">If set to <c>true</c> crouch.</param>
        /// <param name="jump">If set to <c>true</c> jump.</param>
        public void SetMotion(Vector3 move, bool crouch, bool jump, float turn = 0.0f)
		{

			// convert the world relative moveInput vector into a local-relative
			// turn amount and forward amount required to head in the desired
			// direction.
			if (move.magnitude > 1f) move.Normalize();

			move = transform.InverseTransformDirection(move);
			CheckGroundStatus();
			move = Vector3.ProjectOnPlane(move, m_GroundNormal);

            m_TurnAmount = Mathf.Clamp(turn * turn * Mathf.Sign(turn), -1f, 1f);
			m_ForwardAmount = move.z;
            m_StrafeAmmount = move.x;
            ApplyExtraTurnRotation();

			// control and velocity handling is different when grounded and airborn
			if (m_IsGrounded)
			{
				HandleGroundedMovement(crouch, jump);
			}
			else
			{
				HandleAirborneMovement();
			}

			ScaleCapsuleForCrouching(crouch);
			PreventStandingInLowHeadroom();

			// send input and other state parameters to the animator
			UpdateAnimator(move);
		}

        public void SetRotation(float rotaton)
        {
            m_TurnAmount = Mathf.Clamp(rotaton * rotaton * Mathf.Sign(rotaton), -1f, 1f);
            ApplyExtraTurnRotation();
        }
        #endregion

        #region Animation Recievers

        // reciever from Animation Idle_to_SwordIdle
        public void GrabWeapon()
        {
            print("weapon grabbed");
            //newTarget = handPosition;
            //newTargetWeight = 1;
            //weapon.SetParent(rightHandIK);
            weaponScabbard.SetActive(false);
            weaponHand.SetActive(true);
        }

        // reciever from Animation SwordIdle_to_Idle
        public void OnSheath()
        {
            weaponScabbard.SetActive(true);
            weaponHand.SetActive(false);
        }

        // 
        public void OnAttackEnter()
        {
            weaponHand.GetComponent<WeaponProperties>().OnAttackEnter();
        }

        public void OnAttackExit()
        {
            weaponHand.GetComponent<WeaponProperties>().OnAttackExit();
        }

        public void OnBlockEnter()
        {

            print("TPC block enter");
            weaponHand.GetComponent<WeaponProperties>().OnBlockEnter();
        }
        public void OnBlockExit()
        {
            print(name + " tries to exit block");
            if (!m_IsBlocking){
                print("Block exit done");
                weaponHand.GetComponent<WeaponProperties>().OnBlockExit();
            }
                
        }
        //Placeholder functions for Animation events.
        public void Hit()
        {
        }

        public void Shoot()
        {
        }

        public void FootR()
        {
        }

        public void FootL()
        {
        }

        public void Land()
        {
        }

        public void Jump()
        {
        }
        #endregion

        void ScaleCapsuleForCrouching(bool crouch)
		{
			if (m_IsGrounded && crouch)
			{
				if (m_Crouching) return;
				m_Capsule.height = m_Capsule.height / 2f;
				m_Capsule.center = m_Capsule.center / 2f;
				m_Crouching = true;
			}
			else
			{
				Ray crouchRay = new Ray(m_Rigidbody.position + Vector3.up * m_Capsule.radius * k_Half, Vector3.up);
				float crouchRayLength = m_CapsuleHeight - m_Capsule.radius * k_Half;
				if (Physics.SphereCast(crouchRay, m_Capsule.radius * k_Half, crouchRayLength, Physics.AllLayers, QueryTriggerInteraction.Ignore))
				{
					m_Crouching = true;
					return;
				}
				m_Capsule.height = m_CapsuleHeight;
				m_Capsule.center = m_CapsuleCenter;
				m_Crouching = false;
			}
		}

		void PreventStandingInLowHeadroom()
		{
			// prevent standing up in crouch-only zones
			if (!m_Crouching)
			{
				Ray crouchRay = new Ray(m_Rigidbody.position + Vector3.up * m_Capsule.radius * k_Half, Vector3.up);
				float crouchRayLength = m_CapsuleHeight - m_Capsule.radius * k_Half;
				if (Physics.SphereCast(crouchRay, m_Capsule.radius * k_Half, crouchRayLength, Physics.AllLayers, QueryTriggerInteraction.Ignore))
				{
					m_Crouching = true;
				}
			}
		}


		void UpdateAnimator(Vector3 move)
		{
			// update the animator parameters
			m_Animator.SetFloat("Forward", m_ForwardAmount, 0.1f, Time.deltaTime);
            m_Animator.SetFloat("Strafe", m_StrafeAmmount, 0.1f, Time.deltaTime);
			m_Animator.SetFloat("Turn", m_TurnAmount, 0.1f, Time.deltaTime);
			m_Animator.SetBool("Crouch", m_Crouching);
			m_Animator.SetBool("OnGround", m_IsGrounded);

			if (!m_IsGrounded)
			{
				m_Animator.SetFloat("Jump", m_Rigidbody.velocity.y);
			}

			// calculate which leg is behind, so as to leave that leg trailing in the jump animation
			// (This code is reliant on the specific run cycle offset in our animations,
			// and assumes one leg passes the other at the normalized clip times of 0.0 and 0.5)
			float runCycle =
				Mathf.Repeat(
					m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime + m_RunCycleLegOffset, 1);
			float jumpLeg = (runCycle < k_Half ? 1 : -1) * m_ForwardAmount;
			if (m_IsGrounded)
			{
				m_Animator.SetFloat("JumpLeg", jumpLeg);
			}

			// the anim speed multiplier allows the overall speed of walking/running to be tweaked in the inspector,
			// which affects the movement speed because of the root motion.
			if (m_IsGrounded && move.magnitude > 0)
			{
				m_Animator.speed = m_AnimSpeedMultiplier;
			}
			else
			{
				// don't use that while airborne
				m_Animator.speed = 1;
			}
		}


		void HandleAirborneMovement()
		{
			// apply extra gravity from multiplier:
			Vector3 extraGravityForce = (Physics.gravity * m_GravityMultiplier) - Physics.gravity;
			m_Rigidbody.AddForce(extraGravityForce);

			m_GroundCheckDistance = m_Rigidbody.velocity.y < 0 ? m_OrigGroundCheckDistance : 0.01f;
		}


		void HandleGroundedMovement(bool crouch, bool jump)
		{
			// check whether conditions are right to allow a jump:
			if (jump && !crouch && m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Grounded"))
			{
				// jump!
				m_Rigidbody.velocity = new Vector3(m_Rigidbody.velocity.x, m_JumpPower, m_Rigidbody.velocity.z);
				m_IsGrounded = false;
				m_Animator.applyRootMotion = false;
				m_GroundCheckDistance = 0.1f;
			}
		}

		void ApplyExtraTurnRotation()
		{
			// help the character turn faster (this is in addition to root rotation in the animation)
			float turnSpeed = Mathf.Lerp(m_StationaryTurnSpeed, m_MovingTurnSpeed, m_ForwardAmount);
			transform.Rotate(0, m_TurnAmount * turnSpeed * Time.deltaTime, 0);
		}


		public void OnAnimatorMove()
		{
			// we implement this function to override the default root motion.
			// this allows us to modify the positional speed before it's applied.
			if (m_IsGrounded && Time.deltaTime > 0)
			{
				Vector3 v = (m_Animator.deltaPosition * m_MoveSpeedMultiplier) / Time.deltaTime;

				// we preserve the existing y part of the current velocity.
				v.y = m_Rigidbody.velocity.y;
				m_Rigidbody.velocity = v;
			}
		}


		void CheckGroundStatus()
		{
			RaycastHit hitInfo;
#if UNITY_EDITOR
			// helper to visualise the ground check ray in the scene view
			Debug.DrawLine(transform.position + (Vector3.up * 0.1f), transform.position + (Vector3.up * 0.1f) + (Vector3.down * m_GroundCheckDistance));
#endif
			// 0.1f is a small offset to start the ray from inside the character
			// it is also good to note that the transform position in the sample assets is at the base of the character
			if (Physics.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.down, out hitInfo, m_GroundCheckDistance))
			{
				m_GroundNormal = hitInfo.normal;
				m_IsGrounded = true;
				m_Animator.applyRootMotion = true;
			}
			else
			{
				m_IsGrounded = false;
				m_GroundNormal = Vector3.up;
				m_Animator.applyRootMotion = false;
			}
		}
	}
}
