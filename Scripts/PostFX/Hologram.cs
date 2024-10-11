using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class Hologram : MonoBehaviour {

    //[Header("Pattern")]
    //[SerializeField] Texture2D m_vignette;

    [Header("Shader")]
    [SerializeField] Shader FXHologramShader;
    [SerializeField] Texture m_UIMask;
    [SerializeField] Texture m_UITexture;

    [SerializeField] Color m_UIHue;

    [Range(-5, 5)] [SerializeField] float m_Speed = -0.5f;
    [Range(0, 1)] [SerializeField] float m_Amplitude = 0.001f;
    [Range(0, 5)] [SerializeField] float m_Contraste = 1f;
    [Range(0, 1)] [SerializeField] float m_Precision = 0.20f;





    private Material FXHologramMaterial;

	void CreateMaterials() 
	{
		if (FXHologramMaterial == null)
		{
            FXHologramMaterial = new Material (FXHologramShader);
            FXHologramMaterial.hideFlags = HideFlags.HideAndDontSave;
        }
    }

	void OnRenderImage(RenderTexture source, RenderTexture destination) //Fonction appelée par unity à chaque fin de rendu. C'est maintenant qu'on fait le post-effet
	{
		CreateMaterials();
        FXHologramMaterial.SetTexture("_UIMask", m_UIMask);
        FXHologramMaterial.SetTexture("_UITex", m_UITexture);

        FXHologramMaterial.SetFloat("_Speed", m_Speed);
        FXHologramMaterial.SetFloat("_Amplitude", m_Amplitude);
        FXHologramMaterial.SetFloat("_Contraste", m_Contraste);
        FXHologramMaterial.SetFloat("_Precision", m_Precision);

        FXHologramMaterial.SetColor("_UIHue", m_UIHue);

        Graphics.Blit(source, destination, FXHologramMaterial);
    }
}

