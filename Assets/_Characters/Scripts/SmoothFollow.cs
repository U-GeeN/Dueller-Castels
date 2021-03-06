﻿using UnityEngine;

namespace UnityStandardAssets.Utility
{
    public class SmoothFollow : MonoBehaviour
	{

		// The target we are following
		public Transform target;
        public Transform lookAt;
		// The distance in the x-z plane to the target
		[SerializeField]
		private float distance = 10.0f;
        [SerializeField] float heightChange;
		// the height we want the camera to be above the target
		[SerializeField]
		private float height = 5.0f;
        [SerializeField]
        private float offset = 1.0f;
		[SerializeField]
		private float rotationDamping = 1;
		[SerializeField]
        private float heightDamping = 1;

        // Update is called once per frame
        void LateUpdate()
		{
			// Early out if we don't have a target
			if (!target)
				return;

            height -= Input.GetAxis("Mouse ScrollWheel")/5;
            distance -= Input.GetAxis("Mouse ScrollWheel");

            // Calculate the current rotation angles
            float wantedRotationAngle = target.eulerAngles.y;
            float wantedHeight = target.position.y + height;

            float currentRotationAngle = transform.eulerAngles.y;
            float currentHeight = transform.position.y;

            // Damp the rotation around the y-axis
            currentRotationAngle = Mathf.LerpAngle(currentRotationAngle, wantedRotationAngle, rotationDamping * Time.deltaTime);

			// Damp the height
			currentHeight = Mathf.Lerp(currentHeight, wantedHeight, heightDamping * Time.deltaTime);

            // Convert the angle into a rotation
            Quaternion currentRotation = Quaternion.Euler(0, currentRotationAngle, 0);

            // Set the position of the camera on the x-z plane to:
            // distance meters behind the target
            transform.position = target.position;
            transform.position -= currentRotation * Vector3.forward * distance;

            // Set the height of the camera
            transform.position = new Vector3(transform.position.x, currentHeight, transform.position.z);

            Vector3 targetOffset = target.position;

            // Always look at the target
            if (lookAt != null)
            {
                transform.LookAt(lookAt.position);
            }
            else
            {
                transform.LookAt(targetOffset + (Vector3.up * offset));
            }
        }

        public void SetActive(bool becomeEnabled, Transform target = null)
        {
            enabled = becomeEnabled;
            this.target = target;

        }

	}
}