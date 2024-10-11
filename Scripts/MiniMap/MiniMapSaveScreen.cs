using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniMapSaveScreen : MonoBehaviour
{
    private RenderTexture rt;
    private int index;
    private string filePathPng;
    private string filePathTxt;
    private GameObject miniMapImg;

    private void Start()
    {
        rt = GameObject.FindGameObjectWithTag("MiniMap").GetComponent<MiniMapCameraCreator>().Rt;
        index = 0;
        filePathPng = Application.dataPath + "/Production/Sprites/MiniMap/MiniMap.png";
        filePathTxt = Application.dataPath + "/Production/Sprites/MiniMap/LastMiniMapSize.txt";
        miniMapImg = GameObject.FindGameObjectWithTag("MiniMapImg");
    }

    private void OnPostRender()
    {
        // To prevent black image
        if (index > 1)
        {
            // Save current render texture
            RenderTexture currentActiveRT = RenderTexture.active;
            // Active render texture of minimap
            RenderTexture.active = rt;

            SaveImgToPng();
            LoadImg();

            // Active last render texture 
            RenderTexture.active = currentActiveRT;

            // Destroy the camera
            Destroy(this.gameObject);
        }
        else
        {
            index++;
        }
    }

    private bool CheckLastImgSize()
    {
        // Check if an image exist
        if (System.IO.File.Exists(filePathPng))
        {
            // Get info of current image
            System.IO.FileInfo fileInfo = new System.IO.FileInfo(filePathPng);
            // Get size of last image
            string sizeLastImg = "";
            if (System.IO.File.Exists(filePathTxt))
            {
                sizeLastImg = System.IO.File.ReadAllText(filePathTxt);
            }

            if (fileInfo.Length.ToString() == sizeLastImg)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    private void SaveImgToPng()
    {
        // If the size of last image is the same of the current image then we don't create an other image
        if (!CheckLastImgSize())
        {
            // Read current render
            Texture2D img = new Texture2D(rt.width, rt.height, TextureFormat.RGB24, false);
            img.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
            img.Apply();

            // Encode it
            byte[] byteImg = img.EncodeToPNG();
            Destroy(img);

            // Save to PNG
            System.IO.File.WriteAllBytes(filePathPng, byteImg);

            // Save current size of the image
            if (System.IO.File.Exists(filePathPng))
            {
                System.IO.FileInfo fileInfo = new System.IO.FileInfo(filePathPng);
                System.IO.File.WriteAllText(filePathTxt, fileInfo.Length.ToString());
            }
        }
    }

    private void LoadImg()
    {
        // Check if file exist
        if (System.IO.File.Exists(filePathPng))
        {
            // Get data of image
            byte[] fileData = System.IO.File.ReadAllBytes(filePathPng);

            // Create texture
            Texture2D tex = new Texture2D(rt.width, rt.height);
            tex.LoadImage(fileData);

            // Add texture to UI
            miniMapImg.GetComponentInChildren<RawImage>().texture = tex;
        }
    }
}
