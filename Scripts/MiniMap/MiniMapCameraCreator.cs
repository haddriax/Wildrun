using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapCameraCreator : MonoBehaviour
{
    private RenderTexture rt;
    public RenderTexture Rt { get => rt; }
    private GameObject camera;
    [SerializeField]
    private Vector2Int sizeOfImg = new Vector2Int(256, 256);
    [SerializeField]
    private Vector3Int camPos = new Vector3Int(0, 7350, 0);
    [SerializeField]
    private Vector3 camRot = new Vector3(90.0f, 0.0f, 0.0f);

    private void Start()
    {
        CreateRenderTexture();
        CreateCamera();
    }

    private void CreateRenderTexture()
    {
        rt = new RenderTexture(sizeOfImg.x, sizeOfImg.y, 16, RenderTextureFormat.ARGB32);
        rt.Create();
        rt.name = "RenderTextureMiniMap";
    }

    private void CreateCamera()
    {
        camera = new GameObject();
        camera.transform.SetParent(this.transform);
        camera.AddComponent<Camera>();
        camera.AddComponent<MiniMapSaveScreen>();
        camera.name = "MiniMap Camera";
        camera.GetComponent<Camera>().orthographic = true;
        camera.GetComponent<Camera>().orthographicSize = 4000.0f;
        camera.GetComponent<Camera>().nearClipPlane = camPos.y - 500.0f;
        camera.GetComponent<Camera>().farClipPlane = camPos.y + 100.0f;
        camera.GetComponent<Camera>().targetTexture = rt;
        camera.transform.position = camPos;
        camera.transform.rotation = Quaternion.Euler(camRot);
    }
}
