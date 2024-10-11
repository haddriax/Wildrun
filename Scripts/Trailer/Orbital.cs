using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Orbital : MonoBehaviour
{
    private List<PodModel> podModels;
    private List<GameObject> podGameObjects;
    [SerializeField] GameObject modelParent;
    [SerializeField] GameObject cameraRotator;
    [SerializeField] float angleMaxRotation = 90;

    private void Start()
    {
        podModels = SavedDatasManager.PodModels;
        podGameObjects = new List<GameObject>();
        for (int i = 0; i < podModels.Count; i++)
        {
            //GameObject podBase = Instantiate(SavedDatasManager.GetPartByID(podModels[i].IDBaseFrame).gameObject, modelParent.transform);
            //GameObject fractured = Instantiate(VehiculeGenerator.GenerateFracturedPodGameObject(podModels[i]) , modelParent.transform);
            //podBase.transform.parent = gameObject.transform;
            //podBase.transform.localPosition = Vector3.zero;
            //podBase.transform.localRotation = Quaternion.identity;
            podGameObjects.Add(VehiculeGenerator.GeneratePodGameObject(podModels[i]));
            podGameObjects[i].SetActive(false);
        }

        StartCoroutine("TurnAroundPods");
    }

    IEnumerator TurnAroundPods()
    {
        for (int i = 0; i < podGameObjects.Count; i++)
        {
            podGameObjects[i].SetActive(true);
            float angle = 0;
            while (angle <= angleMaxRotation)
            {
                cameraRotator.transform.Rotate(Vector3.up, 1);
                cameraRotator.GetComponentInChildren<Camera>().transform.LookAt(podGameObjects[i].transform);
                angle += 1f;
                yield return null;
            }
            podGameObjects[i].SetActive(false);
        }
    }
}
