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
        [Header("Testvariablen")]
        public bool zoomToMouse = false;

        void Awake()
        {
            startPos = transform.position;
            startRot = transform.rotation;
        }

        void Update()
        {

            if (Input.GetKey(KeyCode.LeftShift))
            {
                CameraRotate();
            }

            if (Input.GetKey(KeyCode.R))
            {
                resetPosition();
            }
            if (refocus)
            {
                Refocus(startPos);
            }

            if (zoomToMouse) {
                ZoomToMouse();
            } else {
                CameraZoom();
            }

            CameraMove();
            CameraMoveToParent();
        }



        void resetPosition()
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

        void ZoomToMouse () {
            var cam = Camera.main;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            float zoomDistance = ScrollSensetivity * Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime;
            cam.transform.Translate(ray.direction* zoomDistance, Space.World);
        }

        void CameraRotate()
        {
            float moveAxisX = Input.GetAxis("Mouse X");
            float moveAxisY = Input.GetAxis("Mouse Y");
            transform.parent.eulerAngles += Vector3.up * moveAxisX;
            transform.eulerAngles -= Vector3.right * moveAxisY;
        }

        void CameraMove()
        {
            float h = Input.GetAxis("Horizontal") / 10;
            float v = Input.GetAxis("Vertical") / 10;
            var tempDir = transform.forward;

            //rotation = Quaternion.Euler(transform.localRotation.x, 0, 0) * transform.forward;
            //transform.parent.position += new Vector3(v, 0, h) * motionSensetivity;
            transform.parent.localPosition += transform.parent.forward * v * motionSensetivity;
            transform.parent.localPosition += transform.parent.right * h * motionSensetivity;
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
    }
}