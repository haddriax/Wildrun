using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class VignetteBlur : MonoBehaviour {

    [Header("Pattern")]
    [SerializeField] Texture2D m_vignette;

    [Header("Blurring Shaders")]
    [SerializeField] Shader FXShaderHorizontal;
    [Range(0.0f, 0.10f)][SerializeField] float m_horiontalBlurStrength;

    [Header("Vertical Blur")]
    [SerializeField] Shader FXShaderVertical;
    [Range (0.0f, 0.10f)][SerializeField] float m_verticalBlurStrength;








    private Material FXMaterialHorizontal;
	private Material FXMaterialVertical;

	void CreateMaterials() 
	{
		if (FXMaterialHorizontal == null)
		{
            FXMaterialHorizontal = new Material (FXShaderHorizontal);
            FXMaterialHorizontal.hideFlags = HideFlags.HideAndDontSave;
        }
        if (FXMaterialVertical == null)
        {
            FXMaterialVertical = new Material(FXShaderVertical);
            FXMaterialVertical.hideFlags = HideFlags.HideAndDontSave;
        }
    }

	void OnRenderImage(RenderTexture source, RenderTexture destination) //Fonction appelée par unity à chaque fin de rendu. C'est maintenant qu'on fait le post-effet
	{
		CreateMaterials();
        FXMaterialHorizontal.SetTexture("_VignetteTex", m_vignette);
        FXMaterialHorizontal.SetFloat("_blurModifier", m_horiontalBlurStrength);

        FXMaterialVertical.SetTexture("_VignetteTex", m_vignette);
        FXMaterialVertical.SetFloat("_blurModifier", m_verticalBlurStrength);

        RenderTexture buffer = RenderTexture.GetTemporary(Screen.width, Screen.height);

        Graphics.Blit(source, buffer, FXMaterialHorizontal);
        Graphics.Blit(buffer, destination, FXMaterialVertical);

        RenderTexture.ReleaseTemporary(buffer);
    }
}

