using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System.Linq;

namespace AI
{
    public class AISwitchCamera : MonoBehaviour
    {
        [SerializeField]
        private bool useCinemachine = false;

        private GameObject[] AI;
        private int index;

        private void Start()
        {
            try
            {
                AI = GameObject.FindGameObjectsWithTag("AIVehicle");
            }
            catch
            // Modifié par Ewen -> J'ai fait ça pour eviter un bug dans le build
            {
                AI = FindObjectsOfType<AIController>().Select(x => x.transform.parent.gameObject).ToArray();
            }
            index = AI.Length - 1;
            DisableCameras(index);
        }

        private void Update()
        {
            try
            {
                SwitchCamera();
            }
            catch { }
        }

        private void DisableCameras(int j)
        {
            int k = Mathf.Abs(j);
            for (int i = 0; i < AI.Length; i++)
            {
                if (i == k)
                {
                    if (AI[i].GetComponentInChildren<Camera>())
                    {
                        AI[i].GetComponentInChildren<Camera>().enabled = true;
                    }

                    if (useCinemachine)
                    {
                        if (AI[i].GetComponentInChildren<CinemachineVirtualCamera>())
                        {
                            AI[i].GetComponentInChildren<CinemachineVirtualCamera>().enabled = true;
                        }
                    }
                }
                else
                {
                    if (AI[i].GetComponentInChildren<Camera>())
                    {
                        AI[i].GetComponentInChildren<Camera>().enabled = false;
                    }

                    if (useCinemachine)
                    {
                        if (AI[i].GetComponentInChildren<CinemachineVirtualCamera>())
                        {
                            AI[i].GetComponentInChildren<CinemachineVirtualCamera>().enabled = false;
                        }
                    }
                }
            }
        }

        private void SwitchCamera()
        {
            if (Input.GetKeyDown(KeyCode.Keypad1))
            {
                index = (index - 1) % AI.Length;
                DisableCameras(index);
            }
            else if (Input.GetKeyDown(KeyCode.Keypad2))
            {
                index = (index + 1) % AI.Length;
                DisableCameras(index);
            }
        }
    }
}