using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class RadialBlurAlternative : MonoBehaviour {

    //[Header("Pattern")]
    //[SerializeField] Texture2D m_vignette;

    [Header("Shader")]
    [SerializeField] Shader FXRadialShader;

    [Tooltip("Radius of unblurred centre")]
    [SerializeField] float m_radialBlurRadius;

    [Tooltip("Strength of the blurring effect")]
    [SerializeField] float m_radialBlurStrength;

    [Tooltip("Horizontal Center of the Blur")]
    [Range(0f, 1f)] [SerializeField] float m_radialBlurCenterX;

    [Tooltip("Vertical Center of the Blur")]
    [Range(0f, 1f)] [SerializeField] float m_radialBlurCenterY;


    private Material FXRadialMaterial;

	void CreateMaterials() 
	{
		if (FXRadialMaterial == null)
		{
            FXRadialMaterial = new Material (FXRadialShader);
            FXRadialMaterial.hideFlags = HideFlags.HideAndDontSave;
        }
    }

	void OnRenderImage(RenderTexture source, RenderTexture destination) //Fonction appelée par unity à chaque fin de rendu. C'est maintenant qu'on fait le post-effet
	{
		CreateMaterials();
        FXRadialMaterial.SetFloat("_EffectAmount", m_radialBlurStrength);
        FXRadialMaterial.SetFloat("_Radius", m_radialBlurRadius);
        FXRadialMaterial.SetFloat("_CenterX", m_radialBlurCenterX);
        FXRadialMaterial.SetFloat("_CenterY", m_radialBlurCenterY);

        Graphics.Blit(source, destination, FXRadialMaterial);
    }
}

