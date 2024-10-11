using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Vehicle
{
    public class PhysicControls : MonoBehaviour
    {
        Rigidbody rb;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        public void PhysicRotation(Quaternion target)
        {
            //q will rotate from our current rotation to desired rotation
            Quaternion qRotation = target * Quaternion.Inverse(transform.rotation);

            //convert to angle axis representation so we can do math with angular velocity
            qRotation.ToAngleAxis(out float xMag, out Vector3 angleAxis);
            angleAxis.Normalize();

            //targetedAngularVelocity is the angular velocity we need to achieve
            Vector3 targetedAngularVelocity = angleAxis * xMag * Mathf.Deg2Rad / Time.fixedDeltaTime;
            targetedAngularVelocity -= rb.angularVelocity;

            //to multiply with inertia tensor local then rotationTensor coords
            Vector3 targetedAngularVelocityLocal = transform.InverseTransformDirection(targetedAngularVelocity);
            Vector3 Tl;

            targetedAngularVelocityLocal = rb.inertiaTensorRotation * targetedAngularVelocityLocal;
            targetedAngularVelocityLocal.Scale(rb.inertiaTensor);

            Tl = Quaternion.Inverse(rb.inertiaTensorRotation) * targetedAngularVelocityLocal;

            Vector3 force = transform.TransformDirection(Tl);
            rb.AddTorque(force);
        }

        public void PhysicDisplacment(Vector3 position, float multiplier = 1f)
        {
            float dt = Time.fixedDeltaTime;
            Vector3 pNew = position;

            Vector3 p = transform.position;
            Vector3 v = rb.velocity;
            Vector3 force = rb.mass * (pNew - p - v * dt) / (dt);

            rb.AddForce(force * multiplier);
        }

        public void PhysicTransformAlignement(Transform target)
        {
            PhysicRotation(target.rotation);
            PhysicDisplacment(target.position);
        }

        public void PhysicDisplacmentPos(Vector3 position, Vector3 forcePosition, float multiplier = 1f)
        {
            float dt = Time.fixedDeltaTime;
            Vector3 pNew = position;

            Vector3 pos = transform.position;
            Vector3 v = rb.velocity;
            Vector3 force =  (pNew - pos - v * dt) / (dt);

            rb.AddForceAtPosition(force * multiplier, forcePosition, ForceMode.Acceleration);
        }

        public Vector3 PhysicDisp(Vector3 position, Vector3 target)
        {
            float dt = Time.fixedDeltaTime;
            return (target - position - rb.velocity * dt) / (dt);
        }
    }

}
