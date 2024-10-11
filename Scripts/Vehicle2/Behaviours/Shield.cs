using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Vehicle
{
    public class Shield : VehicleBehaviour
    {
        public float defaultMaxShield;
        public float currentShield;
        public float regen;
        public float regenCooldown = 5f;

        public bool regenIsActive;
        public bool isActive;

#pragma warning disable IDE0052 // Supprimer les membres privés non lus
        Coroutine RegenCoroutine = null;
        Coroutine ImpactsCoroutine = null;
#pragma warning restore IDE0052 // Supprimer les membres privés non lus

        [SerializeField] GameObject shieldColliderPrefab;

        public Collider shieldCollider;
        public Collider vehicleCollider;

        Material shieldMaterial;

        public float CurrentShield { get => currentShield; }
        public float CurrentShield01 { get => currentShield / defaultMaxShield; }
        public bool ShieldIsUp { get => currentShield > 0; }

        public override void OnAwake()
        {
            base.OnAwake();
        }

        public override void OnStart()
        {
            base.OnStart();
            RegenCoroutine = StartCoroutine(Regen());

            Collider[] colliders = GetComponentsInChildren<Collider>();

            foreach (var item in colliders)
            {
                if (item.gameObject.name.Contains("Shield"))
                    shieldCollider = item;
                else if (item.gameObject.name.Contains("Vehicle "))
                    vehicleCollider = item;
            }

            shieldCollider.gameObject.SetActive(true);
            vehicleCollider.gameObject.SetActive(false);

            try
            {
                shieldMaterial = shieldCollider.GetComponentInChildren<MeshRenderer>().material;
            }
            catch
            {
                shieldMaterial = shieldCollider.GetComponentInChildren<SkinnedMeshRenderer>().material;
            }

            /*
            shieldMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            shieldMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            shieldMaterial.SetInt("_ZWrite", 0);
            shieldMaterial.DisableKeyword("_ALPHATEST_ON");
            shieldMaterial.EnableKeyword("_ALPHABLEND_ON");
            shieldMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            shieldMaterial.renderQueue = 3000;
            */

            ImpactsCoroutine = StartCoroutine(UpdateImpacts());
        }

        public void DamageShield(float amount)
        {
            if (!mc.Invulnerable)
            {
                currentShield = Mathf.Clamp(currentShield - Mathf.Abs(amount), 0, defaultMaxShield);

                if (!ShieldIsUp)
                {
                    DesactivateShield();
                }

            }
        }

        public void RegenShield()
        {
            currentShield = Mathf.Clamp(currentShield + regen * Time.deltaTime * 10, 0, defaultMaxShield);
        }

        public void DesactivateShield()
        {
            isActive = false;
            shieldCollider.gameObject.SetActive(false);
            vehicleCollider.gameObject.SetActive(true);
        }

        public void ActivateShield()
        {
            shieldCollider.gameObject.SetActive(true);
            vehicleCollider.gameObject.SetActive(false);
            isActive = true;
        }

        IEnumerator Regen(float startAfter = 0f)
        {
            yield return new WaitForSeconds(startAfter);

            while (true)
            {
                if (!isActive)
                {
                    yield return new WaitForSeconds(regenCooldown);
                    if (mc.isAlive)
                    {
                        ActivateShield();
                    }
                }
                else
                {
                    if (regenIsActive)
                    {
                        RegenShield();
                    }

                    yield return new WaitForEndOfFrame();
                }
            }
        }

        public bool shieldCollide = false;
        public Vector3 worldImpact;
        public float impactIntensity;
        public void SetImpact(Vector3 worldImpact)
        {
            this.worldImpact = worldImpact;
            this.impactIntensity = 1;            
        }

        IEnumerator UpdateImpacts()
        {
            float impactDisparitionSpeed = 0.01f;
            for (; ; )
            {
                shieldMaterial.SetVector("Impacto0", worldImpact);
                shieldMaterial.SetFloat("Impact0Active", impactIntensity);
                impactIntensity = Mathf.Lerp(impactIntensity, 0, impactDisparitionSpeed);
                yield return new WaitForEndOfFrame();
            }           
        }
    }

}
