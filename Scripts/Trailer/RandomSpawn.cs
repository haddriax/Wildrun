using UnityEngine;
using System.Collections.Generic;

public class RandomSpawn : MonoBehaviour
{
    [SerializeField] List<GameObject> modelParents;
    private GameObject podGameObject;

    private void Start()
    {
        //for (int i = 0; i < modelParents.Count; i++)
        for (int i = 0; i < 4; i++)
        {
            PodModel podModel = SavedDatasManager.PodModels[Random.Range(0, SavedDatasManager.PodModels.Count)];
            //GameObject podBase = Instantiate(SavedDatasManager.GetPartByID(podModel.IDBaseFrame).gameObject, modelParents[i].transform);
            //podBase.transform.localPosition = Vector3.zero;
            //podBase.transform.localRotation = Quaternion.identity;
            //podGameObject = VehiculeGenerator.GeneratePodGameObject(podModel, podBase);
            podGameObject = VehiculeGenerator.GenerataPodDestroyable(podModel);
        }
    }
}
