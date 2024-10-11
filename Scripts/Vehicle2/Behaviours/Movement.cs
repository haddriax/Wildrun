using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Vehicle
{
    [AddComponentMenu("_Scripts/Vehicle/Behaviours/Movement")]
    public class Movement : VehicleBehaviour
    {
        enum MovementSates
        {
            forward,
            turn_left,
            turn_right,
            inactive,
        };

        [SerializeField] MovementSates movementSate = MovementSates.inactive;

        public float defaultRotationSpeed;
        public float defaultSmoothness;
        public float speedFactor;
        public float minRotationSpeed = 6f;
        private float smoothness;

        float currentRotationSpeed;

        Engine e;

        private float turnValue = 0f;

        // Last Frame memory
        private float lastFrameTurnValue = 0f;

        // Lerp To
        Vector3 modelDesiredRotation;

        // Getters and Setters
        public float TurnValue
        {
            get => turnValue;
            set
            {
                lastFrameTurnValue = turnValue;
                value = Mathf.Clamp(value, -1f, 1f);
                turnValue = Mathf.MoveTowards(lastFrameTurnValue, value, smoothness * (Time.deltaTime * 100));
            }
        }

        public override void OnStart()
        {
            base.OnStart();
            e = GetComponent<Engine>();
            currentRotationSpeed = defaultRotationSpeed;
            smoothness = defaultSmoothness;
        }

        public override void OnFixedUpdate()
        {
            base.OnFixedUpdate();

            UpdateMovementState();
            UpdateTurnEfficiency();
            TurnPhysic();
        }

        void UpdateMovementState()
        {
            if (turnValue == 0)
            {
                movementSate = MovementSates.forward;
            }
            else if (turnValue < 0)
            {
                movementSate = MovementSates.turn_left;
            }
            else if (turnValue > 0)
            {
                movementSate = MovementSates.turn_right;
            }
        }

        void Turn()
        {
            switch (movementSate)
            {
                case MovementSates.forward:
                    break;
                case MovementSates.turn_left:
                    break;
                case MovementSates.turn_right:
                    break;
                case MovementSates.inactive:
                    break;
                default:
                    throw new System.NotImplementedException("State not implemented.");
            }
        }

        void TurnPhysic()
        {
            // float value = turnValue * 0.3f * currentRotationSpeed;

            float value = turnValue * currentRotationSpeed;
            Vector3 m_EulerAngleVelocity = new Vector3(0, value * 15f, 0) * (Time.fixedDeltaTime * 100f);

            Quaternion deltaRotation = Quaternion.Euler(m_EulerAngleVelocity * Time.deltaTime);
            rb.MoveRotation(rb.rotation * deltaRotation);
        }

        void UpdateTurnEfficiency()
        {
            smoothness = defaultSmoothness; // TO DO
            currentRotationSpeed = Mathf.Clamp(defaultRotationSpeed - (defaultRotationSpeed * e.speedPercentage) / (speedFactor * 100), minRotationSpeed, defaultRotationSpeed);
        }
    }
}