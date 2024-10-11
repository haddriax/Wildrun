using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct Map
{
    public string m_name;
    public Sprite m_image;
    public bool isLock;
}

public class MapSelectionHandler : MonoBehaviour
{
    [SerializeField] List<Map> m_maps;
    int currentMap;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        #region Change Map

        if (Input.GetButtonDown("ButtonRight"))
        {
            NextMap();
        }

        if (Input.GetButtonDown("ButtonLeft"))
        {
            PreviousMap();
        }


        currentMap = Mathf.Clamp(currentMap, 0, m_maps.Count);
        #endregion
    }

    public void PreviousMap()
    {
        currentMap = (currentMap - 1) % m_maps.Count;
    }

    public void NextMap()
    {
        currentMap = (currentMap + 1) % m_maps.Count;
    }

    public Map GetCurrentMap()
    {
        return m_maps[currentMap];
    } 
}
