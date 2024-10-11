using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingTextScript : MonoBehaviour
{
    private Text m_loadingText;

    void Awake()
    {
        m_loadingText = gameObject.GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        m_loadingText.color = new Color(m_loadingText.color.r, m_loadingText.color.g, m_loadingText.color.b, Mathf.PingPong(Time.time, 1));
    }
}
