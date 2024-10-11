using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

namespace Vehicle
{
    [DisallowMultipleComponent]
    [AddComponentMenu("_Scripts/Vehicle/Behaviours/Camera")]
    public class VehicleCamera : MonoBehaviour
    {
        enum State
        {
            virtual_cockpit,
            Third_Pers,
        }

        public MainController controller;
        RadialBlur radialBlur;

        public CinemachineVirtualCamera cm_camera;
        public Camera def_camera;

        [SerializeField] float defaultFieldOfView = 40;
        public float minFieldOfView = 40;
        public float maxFieldOfView = 65;

        [SerializeField] float fieldOfViewRatio;

        [Header("Virtual Cockpit")]
        [SerializeField] Transform lookAtV;
        [SerializeField] Transform camPosV;

        [Header("Virtual 3rdPers")]
        [SerializeField] Transform lookAtP;
        [SerializeField] Transform camPosP;

        [Header("Helper")]
        [SerializeField] float speedPercent;
        [SerializeField] float currentFieldOfView;

        private bool isInit;

        private void Start()
        {
            controller = GetComponentInParent<MainController>();

            foreach (Transform t in transform.GetComponentsInChildren<Transform>())
            {
                def_camera = t.gameObject.GetComponent<Camera>();

                if (def_camera != null)
                    break;
            }

            foreach (Transform t in transform.GetComponentsInChildren<Transform>())
            {
                cm_camera = t.gameObject.GetComponent<CinemachineVirtualCamera>();

                if (cm_camera != null)
                    break;
            }

            isInit = def_camera != null || cm_camera != null;

            if (isInit)
            {
                Debug.Log("Destroying script VehicleCamera for " + name + "One or more camera are not registered");
                Destroy(this);
            }
            else
            {
                Debug.Log("Init camera script for " + name);

                cm_camera.m_Lens.FieldOfView = defaultFieldOfView;

                radialBlur = def_camera.GetComponent<RadialBlur>();

                minFieldOfView = 35f;
                maxFieldOfView = 55f;

                fieldOfViewRatio = (maxFieldOfView - minFieldOfView) / 100;
            }
        }

        void Update()
        {
            if (!isInit)
                return;

            speedPercent = controller.engineB.speedPercentage;

            float targetedFov = Mathf.Clamp(defaultFieldOfView + (speedPercent * fieldOfViewRatio), minFieldOfView, maxFieldOfView);

            cm_camera.m_Lens.FieldOfView = Mathf.Lerp(currentFieldOfView, targetedFov, 0.08f);

            currentFieldOfView = cm_camera.m_Lens.FieldOfView;

            SendDatasToShaders();
        }

        private void SendDatasToShaders()
        {
            radialBlur.Strength = speedPercent / 65;
            radialBlur.Distance = .075f;
        }

        private void LateUpdate()
        {
            /*
            float angle = 20f;
            Vector3 rotation = Vector3.zero;
            rotation.z = -GetComponent<Movement>().TurnValue * angle;
            cm_camera.transform.localEulerAngles = rotation;
            */
        }
    }

}
