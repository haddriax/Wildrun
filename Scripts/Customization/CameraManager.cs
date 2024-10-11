using UnityEngine;
using Cinemachine;

namespace Customization
{
    public class CameraManager : MonoBehaviour
    {
        [SerializeField] CinemachineFreeLook fcam;
        public CinemachineVirtualCamera vcamsForward;
        public CinemachineVirtualCamera vcamsBackward;
        public CinemachineVirtualCamera vcamsMiddle;

        //private List<CinemachineVirtualCamera> virtualCameras;
        CinemachineVirtualCamera currentCamera = null;

        private void Start()
        {
            PodOnCustom pod = FindObjectOfType<PodOnCustom>();
            SetCurrentCamera(PositionType.Orbital);
        }

        private void FixedUpdate()
        {
            if (currentCamera != null)
            {
                currentCamera.m_Lens.FieldOfView -= Zoom() * 50 * Time.deltaTime;
                currentCamera.m_Lens.FieldOfView = Mathf.Clamp(fcam.m_Lens.FieldOfView, 20, 60);
            }
            else
            {
                fcam.m_Lens.FieldOfView -= Zoom() * 50 * Time.deltaTime;
                fcam.m_Lens.FieldOfView = Mathf.Clamp(fcam.m_Lens.FieldOfView, 20, 60);
            }
        }

        public void SetCurrentCamera(PositionType _position)
        {
            //fcam.Priority          = _camera == PositionCamera.Orbital  ? 1 : 0;
            //vcamsForward.Priority  = _camera == PositionCamera.Forward  ? 1 : 0;
            //vcamsBackward.Priority = _camera == PositionCamera.Backward ? 1 : 0;
            //vcamsMiddle.Priority   = _camera == PositionCamera.Middle   ? 1 : 0;
            if (_position == PositionType.Orbital)
            {
                fcam.Priority = 1;
                currentCamera = null;
            } else fcam.Priority = 0;
            if (_position == PositionType.Forward)
            {
                vcamsForward.Priority = 1;
                currentCamera = vcamsForward;
            } else vcamsForward.Priority = 0;
            if (_position == PositionType.Middle)
            {
                vcamsMiddle.Priority = 1;
                currentCamera = vcamsMiddle;
            } else vcamsMiddle.Priority = 0;
            if (_position == PositionType.Back)
            {
                vcamsBackward.Priority = 1;
                currentCamera = vcamsBackward;
            } else vcamsBackward.Priority = 0;
        }

        public void SetCamerasTarget(PositionType _position, GameObject _target)
        {
            switch (_position)
            {
                case PositionType.Forward:
                    vcamsForward.Follow = _target.transform;
                    vcamsForward.LookAt = _target.transform;
                    break;
                case PositionType.Middle:
                    vcamsMiddle.Follow = _target.transform;
                    vcamsMiddle.LookAt = _target.transform;
                    break;
                case PositionType.Back:
                    vcamsBackward.Follow = _target.transform;
                    vcamsBackward.LookAt = _target.transform;
                    break;
                case PositionType.Orbital:
                    fcam.Follow = _target.transform;
                    fcam.LookAt = _target.transform;
                    break;
                default: break;
            }
        }

        private int Zoom()
        {
            return 0;
            return Input.GetAxisRaw("Zoom") == 1 || Input.GetButton("ButtonRight") ? 1 : (Input.GetAxisRaw("Zoom") == -1 || Input.GetButton("ButtonLeft") ? -1 : 0);
        }
    }
}
