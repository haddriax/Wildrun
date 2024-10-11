using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(AIMovement))]
    [RequireComponent(typeof(AIDifficulty))]
    [RequireComponent(typeof(AIRoad))]
    public class AIController : MonoBehaviour
    {
        private AIDifficulty aiDifficulty;
        private AIMovement aiMovement;
        private AIRoad aiRoad;

        private void Awake()
        {
            aiDifficulty = this.GetComponent<AIDifficulty>();
            aiRoad = this.GetComponent<AIRoad>();
            aiMovement = this.GetComponent<AIMovement>();
        }

        private void Start()
        {
            aiDifficulty.Init();
            aiMovement.Init();
            aiRoad.Init();
        }

        private void Update()
        {
            aiDifficulty.ManageAIDifficulty();
            aiMovement.ManageSpeed();
            aiMovement.Movements();
            aiMovement.DebugAI();
        }

        //private void OnCollisionEnter(Collision collision)
        //{
            // Check if the vehicle is colliding with us and if he's behind, if it's true then we turn off the follow path
            //if (collision.collider.tag == "Car")
            //{
            //    Vector3 forward = this.transform.TransformDirection(Vector3.forward);
            //    Vector3 toOther = collision.collider.transform.position - this.transform.position;

            //    if (Vector3.Dot(forward, toOther) < 0.0f && collision.collider.GetComponent<Rigidbody>().velocity.magnitude > 50.0f)
            //    {
            //        followRoad = false;
            //        //collision.collider.GetComponent<Rigidbody>().AddExplosionForce(1000.0f, collision.collider.transform.position, Vector3.Distance(collision.collider.transform.position, this.transform.position));
            //    }
            //    //<SoundTest>
            //    SMF.PlayImpact(transform);
            //    //<\SoundTest>
            //}

            // TEST
            //if (collision.collider.name == "Terrain")
            //{
            //    followRoad = false;
            //    //<SoundTest>
            //    SMF.PlayImpact(transform);
            //    //<\SoundTest>
            //}
        //}
    }
}