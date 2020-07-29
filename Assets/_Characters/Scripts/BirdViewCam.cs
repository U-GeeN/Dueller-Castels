using UnityEngine;

namespace UnityStandardAssets.Utility
{
    public class BirdViewCam: MonoBehaviour
    {
        Vector3 startPos;
        Quaternion startRot;

        public float ScrollSensetivity;
        public float motionSensetivity;
        public Vector3 refocusPosition;
        public bool refocus;

        public Vector3 rotation;
        //[Header("Testvariablen")]
        //public bool zoomToMouse = false;
        //public Vector3 addPosition;

        void Awake()
        {
            startPos = transform.position;
            startRot = transform.rotation;
        }

        private void OnEnable()
        {
            
            CameraMoveToParent();
        }

        void Update()
        {

            if (Input.GetKey(KeyCode.Mouse2))
            {
                CameraRotate();
            }

            if (Input.GetKey(KeyCode.R))
            {
                ResetPosition();
            }

            if (refocus)
            {
                Refocus(startPos);
            }
            CameraZoom();
            CameraMove();
        }

        private void ResetPosition()
        {
            transform.position = startPos;
            transform.rotation = startRot;
        }

        // Unfinisched!!!!
        void Refocus(Vector3 distance)
        {
            transform.position = Vector3.Lerp(transform.localPosition, distance, Time.deltaTime);
        }


        void CameraZoom()
        {
            float mouseScrollValue = Input.GetAxis("Mouse ScrollWheel");
            transform.parent.position += transform.forward * mouseScrollValue * ScrollSensetivity;
        }

        private void CameraRotate()
        {
            float moveAxisX = Input.GetAxis("Mouse X");
            float moveAxisY = Input.GetAxis("Mouse Y");
            transform.parent.eulerAngles += Vector3.up * moveAxisX * motionSensetivity * 2;
            transform.eulerAngles -= Vector3.right * moveAxisY * motionSensetivity * 2;
        }

        private void CameraMove()
        {
            float horizontal = Input.GetAxis("Horizontal") / 10;
            float vertical = Input.GetAxis("Vertical") / 10;
            transform.parent.localPosition += transform.parent.forward * vertical * motionSensetivity;
            transform.parent.localPosition += transform.parent.right * horizontal * motionSensetivity;
        }

        void CameraMoveToParent()
        {
            if (transform.localPosition.magnitude > 0)
            {
                transform.localPosition = transform.localPosition * 7 / 10;
                //transform.localEulerAngles = transform.localEulerAngles * Vector3.up * 3 / 10;
                var tmp = transform.localEulerAngles;
                tmp.y *= 7 / 10;
                transform.localEulerAngles = tmp;
            }
        }

        public void SetActive(bool value)
        {
            enabled = value;
            
        }
    }
}