using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

namespace Vehicle
{
    [AddComponentMenu("_Scripts/Vehicle/Behaviours/Collision")]
    public class CollisionB : VehicleBehaviour
    {
        Shield shield;
        const float instaKill = 25f;

        public GameObject destructibleModel;
        [SerializeField] GameObject explosionEffect;

        public bool hasBeenDestroyed = false;

        public override void OnStart()
        {
            base.OnStart();
            // destructibleModel = GetComponent<Animation>().destroyedModel;
            shield = GetComponent<Shield>();
        }

        Coroutine invulnerabiltyFrame = null;
        public void CollisionEnter(in Collision collision)
        {
            HandleImpact(collision);

            Vector3 impactNormal = collision.GetContact(0).normal;
            impactNormal.y /= 20;

            StartCoroutine(AddForce(impactNormal * 15));

            if (invulnerabiltyFrame == null)
                invulnerabiltyFrame = StartCoroutine(InvulnerabiltyFrame(1.25f));
        }

        public void CollisionStay(in Collision collision)
        {
            mc.engineB.speedPercentage = Mathf.Lerp(Mathf.Clamp(mc.engineB.speedPercentage, 0, 15), 0, .08f);

            shield.SetImpact(collision.GetContact(0).point);

            if (shield.isActive)
            {
                shield.shieldCollide = true;
            }
            else
            {
                shield.shieldCollide = false;
            }
        }

        public void CollisionExit(in Collision collision)
        {

        }

        private void LoseSpeed(float intensity)
        {
            mc.engineB.speedPercentage /= intensity;
            mc.engineB.internalVelocityChange /= intensity;
        }

        private void HandleImpact(in Collision collision)
        {
            if (SoundManagerFMOD.GetInstance().startRace && !SoundManagerFMOD.GetInstance().collisionIsPlaying) StartCoroutine("PlayCollisionSound");

            if (collision.collider.tag == "Vehicle")
            {
                //Debug.Log("colliding an other vehicle");
            }

            Vector3 collisionVelocity = collision.relativeVelocity;
            float colMagnitude = collisionVelocity.magnitude;
            float collisionIntensity = colMagnitude * .65f;

            LoseSpeed(10);

            if(!mc.Invulnerable)
            {
                if (shield.isActive)
                {
                    shield.DamageShield(collisionIntensity);
                }
                else
                {
                    if (collisionIntensity > instaKill)
                    {
                        KillVehicle();
                    }
                }
            }
           
        }

        public void KillVehicle()
        {
            // mc.particlesB.SwitchOnOff(ParticlesB.FxName.trails, false);
            // mc.particlesB.SwitchOnOff(ParticlesB.FxName.reactorDistortion, false);

            mc.Controllable = false;
            mc.IsAlive = false;

            StartCoroutine(InitiateRespawn());

            GameObject modelP = mc.animationB.model.gameObject;
            modelP.GetComponentsInChildren<MeshRenderer>().ToList().ForEach(x => x.enabled = false);
            modelP.GetComponentsInChildren<SkinnedMeshRenderer>().ToList().ForEach(x => x.enabled = false);

            mc.GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.Discrete;
            mc.GetComponent<Rigidbody>().isKinematic = true;

            mc.engineB._Inertie.StoredVelocity = Vector3.zero;
            mc.engineB.internalVelocityChange = Vector3.zero;
            mc.engineB.externalVelocityChange = Vector3.zero;
            mc.engineB.speedPercentage = 0f;
            rb.velocity = Vector3.zero;

            InstantiateDestroyedModel();
        }

        public GameObject InstantiateDestroyedModel()
        {
            GameObject explosion = Instantiate(explosionEffect);
            explosion.transform.position = mc.animationB.model.position;
            explosion.transform.rotation = mc.animationB.model.rotation;
            explosion.transform.parent = null;
            Destroy(explosion, 5f);

            GameObject go = Instantiate(destructibleModel);
            go.transform.position = mc.animationB.model.position;
            go.transform.rotation = mc.animationB.model.rotation;
            go.transform.parent = null;
            go.SetActive(true);

            for (int i = 0; i < go.transform.childCount; i++)
            {
                go.transform.GetChild(i).gameObject.AddComponent<Rigidbody>();
            }

            Destroy(go, 15f);

            return go;
        }

        IEnumerator InitiateRespawn(float waitingTime = 3f)
        {
            yield return new WaitForSeconds(waitingTime);

            Transform bestCheckpoint = null;
            float bestDist = float.MaxValue;

            foreach (var item in mc.clm.currentCheckpoint.m_respawn)
            {
                float dist = Vector3.Distance(item.transform.position, transform.position);
                if (dist < bestDist)
                {
                    bestDist = dist;
                    bestCheckpoint = item.transform;
                }
            }

            Vector3 position = bestCheckpoint.position;
            Quaternion rotation = bestCheckpoint.rotation;
            mc.ResetForRespawn(position, rotation);

            hasBeenDestroyed = true;

            StartCoroutine(InvulnerabiltyFrame());
        }

        IEnumerator AddForce(Vector3 _force, float intensity = 0.1f)
        {
            Vector3 force = _force;
            while (force.sqrMagnitude > 0.01f)
            {
                mc.engineB.externalVelocityChange += force;
                force = Vector3.Lerp(force, Vector3.zero, intensity);
                yield return new WaitForFixedUpdate();
            }

        }

        IEnumerator InvulnerabiltyFrame(float time = 2f)
        {
            mc.Invulnerable = true;
            yield return new WaitForSeconds(time);
            mc.Invulnerable = false;
            invulnerabiltyFrame = null;
        }

        // Sound
        IEnumerator PlayCollisionSound()
        {
            SoundManagerFMOD.GetInstance().PlayImpact(transform);
            SoundManagerFMOD.GetInstance().collisionIsPlaying = true;
            yield return new WaitForSeconds(1.0f);
            SoundManagerFMOD.GetInstance().collisionIsPlaying = false;
        }
    }

}