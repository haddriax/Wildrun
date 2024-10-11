using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapDisplay : MonoBehaviour
{
    [SerializeField] MapSelectionHandler m_mapSelectionHandler;

    [SerializeField] Text m_nameText;
    [SerializeField] Image m_mapImage;
    [SerializeField] GameObject lockImage;

    // Update is called once per frame
    void Update()
    {
        UpdateDisplay();
    }

    void UpdateDisplay()
    {
        m_nameText.text = m_mapSelectionHandler.GetCurrentMap().m_name;
        m_mapImage.sprite = m_mapSelectionHandler.GetCurrentMap().m_image;
        lockImage.gameObject.SetActive(m_mapSelectionHandler.GetCurrentMap().isLock);
    }
}