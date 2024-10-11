using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class RadialBlur : MonoBehaviour {

    //[Header("Pattern")]
    //[SerializeField] Texture2D m_vignette;

    [Header("Shader")]
    [SerializeField] Shader FXRadialShader;
    [SerializeField] float m_radialBlurStrength;
    [SerializeField] float m_radialBlurDistance;

    public float Strength { get => m_radialBlurStrength; set => m_radialBlurStrength = value; }
    public float Distance { get => m_radialBlurDistance; set => m_radialBlurDistance = value; }




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
        FXRadialMaterial.SetFloat("_blurStrength", m_radialBlurStrength);
        FXRadialMaterial.SetFloat("_blurDistance", m_radialBlurDistance);

        Graphics.Blit(source, destination, FXRadialMaterial);
    }
}

