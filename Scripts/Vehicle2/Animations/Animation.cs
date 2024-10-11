using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Vehicle
{
    enum PartType
    {
        Speeder,
        Shielder,
        Maneuver,
        Beginner,
        Undefined
    }
    [AddComponentMenu("_Scripts/Vehicle/Behaviours/Animation")]
    public class Animation : VehicleBehaviour
    {
        Hover h;
        Engine e;
        Movement m;
        VehiculeLayout vl;

        const string speederName = "Speeder";
        const string shielderName = "Shielder";
        const string manoeuverName = "Maneuver";
        const string beginnerName = "Beginner";

        public Transform model;
        public Transform modelParent;

        Vector3[] frontSlotRotationDirection;
        Vector3[] midSlotRotationDirection;
        Vector3[] backSlotRotationDirection;
        Vector3[] bodySlotRotationDirection;

        [SerializeField] PartType frontType = PartType.Undefined;
        [SerializeField] PartType midType = PartType.Undefined;
        [SerializeField] PartType backType = PartType.Undefined;
        [SerializeField] PartType bodyType = PartType.Undefined;

        List<Transform> frontSlotsTransforms = new List<Transform>();
        List<Transform> midSlotsTransforms = new List<Transform>();
        List<Transform> backSlotsTransforms = new List<Transform>();
        public List<Transform> bodySlotsTransforms = new List<Transform>();

        [HideInInspector] public GameObject destroyedModel;
        public Fx[] fxs;
        private bool isInit = false;
              

        public override void OnAwake()
        {
            foreach (Transform t in transform.GetComponentsInChildren<Transform>())
            {
                if (t.name == "Model")
                {
                    model = t;
                    modelParent = t.parent;
                    break;
                }
            }
        }

        public void Init()
        {
            h = GetComponent<Hover>();
            e = GetComponent<Engine>();
            m = GetComponent<Movement>();
            mc = GetComponent<MainController>();

            vl = GetComponentInParent<VehiculeLayout>();

            frontType = PartType.Undefined;
            midType = PartType.Undefined;
            backType = PartType.Undefined;
            bodyType = PartType.Undefined;

            fxs = vl.fxs.ToArray();

            GetComponent<CollisionB>().destructibleModel = vl.fracturedModel;
            destroyedModel = vl.fracturedModel;

            CheckPartsTypes();

            if (frontType == PartType.Undefined || midType == PartType.Undefined || backType == PartType.Undefined || bodyType == PartType.Undefined)
            {
                return;
            }

            CheckPartsReferences();
            CheckRotationDirections();
            isInit = true;
        }
        public override void OnLateUpdate()
        {
            if (isInit)
                ApplyRotationVisual();
        }
        void CheckPartsTypes()
        {
            if (vl.frontWings.Count > 0)
                CheckPartType(ref frontType, vl.frontWings[0]);
            if (vl.reactors.Count > 0)
                CheckPartType(ref midType, vl.reactors[0]);
            if (vl.backWings.Count > 0)
                CheckPartType(ref backType, vl.backWings[0]);
            if (vl.body != null)
                CheckPartType(ref bodyType, vl.body);
        }
        void CheckPartType(ref PartType slot, GameObject part)
        {
            if (part.name.Contains(speederName))
            {
                slot = PartType.Speeder;
            }
            else if (part.name.Contains(shielderName))
            {
                slot = PartType.Shielder;
            }
            else if (part.name.Contains(manoeuverName))
            {
                slot = PartType.Maneuver;
            }
            else if (part.name.Contains(beginnerName))
            {
                slot = PartType.Beginner;
            }
            else
            {
                slot = PartType.Undefined;
            }
        }
        void CheckRotationDirections()
        {
            frontSlotRotationDirection = new Vector3[1];
            midSlotRotationDirection = new Vector3[2];
            backSlotRotationDirection = new Vector3[2];
            bodySlotRotationDirection = new Vector3[2];

            switch (frontType)
            {
                case PartType.Speeder:
                    frontSlotRotationDirection[0] = Vector3.zero;
                    break;
                case PartType.Shielder:
                    frontSlotRotationDirection[0] = Vector3.zero;
                    break;
                case PartType.Maneuver:
                    midSlotRotationDirection[0] = Vector3.right; //
                    midSlotRotationDirection[1] = Vector3.up;
                    break;
                case PartType.Beginner:
                    frontSlotRotationDirection[0] = Vector3.up;
                    break;
                case PartType.Undefined:
                    throw new System.ArgumentException("Part type not matching any referenced type");
                default:
                    break;
            }
            switch (midType)
            {
                case PartType.Speeder:
                    midSlotRotationDirection[0] = Vector3.right;
                    midSlotRotationDirection[1] = Vector3.forward;
                    break;
                case PartType.Shielder:
                    midSlotRotationDirection[0] = Vector3.right;
                    midSlotRotationDirection[1] = Vector3.up;
                    break;
                case PartType.Maneuver:
                    break;
                case PartType.Beginner:
                    break;
                case PartType.Undefined:
                    throw new System.ArgumentException("Part type not matching any referenced type");
                default:
                    break;
            }
            switch (backType)
            {
                case PartType.Speeder:
                    backSlotRotationDirection[0] = Vector3.up;
                    break;
                case PartType.Shielder:
                    backSlotRotationDirection[0] = Vector3.up;
                    break;
                case PartType.Maneuver:
                    break;
                case PartType.Beginner:
                    backSlotRotationDirection[0] = Vector3.up;
                    break;
                case PartType.Undefined:
                    throw new System.ArgumentException("Part type not matching any referenced type");
                default:
                    break;
            }
            switch (bodyType)
            {
                case PartType.Speeder:
                    bodySlotRotationDirection[0] = Vector3.right;
                    break;
                case PartType.Shielder:
                    break;
                case PartType.Maneuver:
                    bodySlotRotationDirection[0] = Vector3.right;
                    break;
                case PartType.Beginner:
                    break;
                case PartType.Undefined:
                    throw new System.ArgumentException("Part type not matching any referenced type");
                default:
                    break;
            }
        }
        private void CheckPartsReferences()
        {
            CheckControlPartReferences(vl.frontWings[0].transform, ref frontSlotsTransforms);
            CheckControlPartReferences(vl.frontWings[1].transform, ref frontSlotsTransforms);

            CheckControlPartReferences(vl.reactors[0].transform, ref midSlotsTransforms);
            CheckControlPartReferences(vl.reactors[1].transform, ref midSlotsTransforms);

            CheckControlPartReferences(vl.backWings[0].transform, ref backSlotsTransforms);
            CheckControlPartReferences(vl.backWings[1].transform, ref backSlotsTransforms);

            CheckControlPartReferences(vl.body.transform, ref bodySlotsTransforms);
        }
        /// <summary>
        /// Iterate through a transform and his children, reference every GameObject containing "_CTRL_".
        /// References are placed into the List
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="container"></param>
        private void CheckControlPartReferences(Transform transform, ref List<Transform> container)
        {
            foreach (Transform t in transform.GetComponentsInChildren<Transform>())
            {
                if (t.name.Contains("_CTRL_"))
                {
                    container.Add(t);
                }
            }
        }
        private void ApplyRotationVisual()
        {
            // For Speeder is for debug only.
            switch (frontType)
            {
                case PartType.Speeder:
                    // RotateFrontSlot_Speeder();
                    break;
                case PartType.Shielder:
                    // RotateFrontSlot_Shielder();
                    break;
                case PartType.Maneuver:
                    RotateFrontSlot_Maneuver();
                    break;
                case PartType.Beginner:
                    RotateFrontSlot_Beginner();
                    break;
                case PartType.Undefined:
                    break;
                default:
                    break;
            }
            switch (midType)
            {
                case PartType.Speeder:
                    RotateMidSlot_Speeder();
                    break;
                case PartType.Shielder:
                    RotateMidSlot_Shielder();
                    break;
                case PartType.Maneuver:
                    RotateMidSlot_Maneuver();
                    break;
                case PartType.Beginner:
                    RotateMidSlot_Beginner();
                    break;
                case PartType.Undefined:
                    break;
                default:
                    break;
            }
            switch (backType)
            {
                case PartType.Speeder:
                    RotateBackSlot_Speeder();
                    break;
                case PartType.Shielder:
                    RotateBackSlot_Shielder();
                    break;
                case PartType.Maneuver:
                    RotateBackSlot_Maneuver();
                    break;
                case PartType.Beginner:
                    RotateBackSlot_Beginner();
                    break;
                case PartType.Undefined:
                    break;
                default:
                    break;
            }
            switch (bodyType)
            {
                case PartType.Speeder:
                    RotateBodySlot_Speeder();
                    break;
                case PartType.Shielder:
                    // RotateBodySlot_Shielder();
                    break;
                case PartType.Maneuver:
                    RotateBodySlot_Maneuver();
                    break;
                case PartType.Beginner:
                    RotateBodySlot_Beginner();
                    break;
                case PartType.Undefined:
                    break;
                default:
                    break;
            }

            RotationByTurnValue();
        }
        private void RotationByTurnValue()
        {
            const float angle = 30f;
            Vector3 rotation = Vector3.zero;

            // float 

            rotation.z = -m.TurnValue * angle;

            model.transform.localEulerAngles = rotation;
        }

        #region Speeder
        private void RotateBackSlot_Speeder()
        {
            const float angle = 20f;

            if (m.TurnValue > 0)
                backSlotsTransforms[0].localEulerAngles = backSlotRotationDirection[0] * angle * m.TurnValue;
            else
                backSlotsTransforms[2].localEulerAngles = backSlotRotationDirection[0] * -angle * m.TurnValue;
        }

        private void RotateMidSlot_Speeder()
        {
            void RotateShuttles(float angle, float speedF)
            {
                midSlotsTransforms[1].localEulerAngles = Vector3.Lerp(midSlotsTransforms[1].localEulerAngles, midSlotRotationDirection[0] * angle, speedF);
                midSlotsTransforms[2].localEulerAngles = Vector3.Lerp(midSlotsTransforms[2].localEulerAngles, midSlotRotationDirection[0] * angle, speedF);
                midSlotsTransforms[3].localEulerAngles = Vector3.Lerp(midSlotsTransforms[3].localEulerAngles, midSlotRotationDirection[0] * angle, speedF);
                midSlotsTransforms[4].localEulerAngles = Vector3.Lerp(midSlotsTransforms[4].localEulerAngles, midSlotRotationDirection[0] * angle, speedF);
                midSlotsTransforms[6].localEulerAngles = Vector3.Lerp(midSlotsTransforms[6].localEulerAngles, midSlotRotationDirection[0] * angle, speedF);
                midSlotsTransforms[7].localEulerAngles = Vector3.Lerp(midSlotsTransforms[7].localEulerAngles, midSlotRotationDirection[0] * angle, speedF);
                midSlotsTransforms[8].localEulerAngles = Vector3.Lerp(midSlotsTransforms[8].localEulerAngles, midSlotRotationDirection[0] * angle, speedF);
                midSlotsTransforms[9].localEulerAngles = Vector3.Lerp(midSlotsTransforms[9].localEulerAngles, midSlotRotationDirection[0] * angle, speedF);
            }

            /// Shuttles
            float angleShuttles = 40f;
            float speed = 0.1f;

            if (mc.states.engine == States.Engine.brake)
            {
                RotateShuttles(angleShuttles, speed);
            }
            else
            {
                RotateShuttles(0, speed);
            }

            // Reactors Up and Down
            float angleReactors = 20f;

            Vector3 rotationR = Vector3.zero;
            rotationR.z = -m.TurnValue * angleReactors;

            Vector3 rotationL = Vector3.zero;
            rotationL.z = m.TurnValue * angleReactors;

            midSlotsTransforms[0].localEulerAngles = rotationR;
            midSlotsTransforms[5].localEulerAngles = rotationL;
        }
        private void RotateBodySlot_Speeder()
        {
            const float angle = 20f;
            const float speed = .2f;

            bodySlotsTransforms[0].localEulerAngles = Vector3.Lerp(bodySlotsTransforms[0].localEulerAngles, bodySlotRotationDirection[0] * angle * (1 - e.SpeedPercentage01Clamped), speed);
            bodySlotsTransforms[1].localEulerAngles = Vector3.Lerp(bodySlotsTransforms[1].localEulerAngles, bodySlotRotationDirection[0] * angle * (1 - e.SpeedPercentage01Clamped), speed);
            bodySlotsTransforms[2].localEulerAngles = Vector3.Lerp(bodySlotsTransforms[2].localEulerAngles, bodySlotRotationDirection[0] * angle * (1 - e.SpeedPercentage01Clamped), speed);
            bodySlotsTransforms[3].localEulerAngles = Vector3.Lerp(bodySlotsTransforms[3].localEulerAngles, bodySlotRotationDirection[0] * angle * (1 - e.SpeedPercentage01Clamped), speed);
            bodySlotsTransforms[4].localEulerAngles = Vector3.Lerp(bodySlotsTransforms[4].localEulerAngles, bodySlotRotationDirection[0] * angle * (1 - e.SpeedPercentage01Clamped), speed);
            bodySlotsTransforms[5].localEulerAngles = Vector3.Lerp(bodySlotsTransforms[5].localEulerAngles, bodySlotRotationDirection[0] * angle * (1 - e.SpeedPercentage01Clamped), speed);
            bodySlotsTransforms[6].localEulerAngles = Vector3.Lerp(bodySlotsTransforms[6].localEulerAngles, bodySlotRotationDirection[0] * angle * (1 - e.SpeedPercentage01Clamped), speed);
            bodySlotsTransforms[7].localEulerAngles = Vector3.Lerp(bodySlotsTransforms[7].localEulerAngles, bodySlotRotationDirection[0] * angle * (1 - e.SpeedPercentage01Clamped), speed);
            bodySlotsTransforms[8].localEulerAngles = Vector3.Lerp(bodySlotsTransforms[8].localEulerAngles, bodySlotRotationDirection[0] * angle * (1 - e.SpeedPercentage01Clamped), speed);
            bodySlotsTransforms[9].localEulerAngles = Vector3.Lerp(bodySlotsTransforms[9].localEulerAngles, bodySlotRotationDirection[0] * angle * (1 - e.SpeedPercentage01Clamped), speed);
        }
        #endregion
        #region Beginner
        private void RotateFrontSlot_Beginner()
        {
            const float speed = .1f;
            const float angleAcc = 30f;
            const float boostedMaxAngle = 40f;

            if (mc.states.engine == States.Engine.brake)
            {
                frontSlotsTransforms[0].localEulerAngles = Vector3.Lerp(frontSlotsTransforms[0].localEulerAngles, Vector3.zero, speed);
                frontSlotsTransforms[1].localEulerAngles = Vector3.Lerp(frontSlotsTransforms[1].localEulerAngles, Vector3.zero, speed);
            }
            else
            {
                float val = Mathf.Clamp(e.SpeedPercentage01 * angleAcc, 0, boostedMaxAngle);
                frontSlotsTransforms[0].localEulerAngles = Vector3.Lerp(frontSlotsTransforms[0].localEulerAngles, frontSlotRotationDirection[0] * val, speed);
                frontSlotsTransforms[1].localEulerAngles = Vector3.Lerp(frontSlotsTransforms[1].localEulerAngles, frontSlotRotationDirection[0] * val, speed);
            }
        }
        private void RotateMidSlot_Beginner()
        {
        }
        private void RotateBackSlot_Beginner()
        {
            const float speed = .1f;
            const float angleAcc = 30f;
            const float boostedMaxAngle = 40f;

            if (mc.states.engine == States.Engine.brake)
            {
                backSlotsTransforms[0].localEulerAngles = Vector3.Lerp(backSlotsTransforms[0].localEulerAngles, Vector3.zero, speed);
                backSlotsTransforms[1].localEulerAngles = Vector3.Lerp(backSlotsTransforms[1].localEulerAngles, Vector3.zero, speed);
            }
            else
            {
                float val = Mathf.Clamp(e.SpeedPercentage01 * angleAcc, 0, boostedMaxAngle);
                backSlotsTransforms[0].localEulerAngles = Vector3.Lerp(backSlotsTransforms[0].localEulerAngles, backSlotRotationDirection[0] * val, speed);
                backSlotsTransforms[1].localEulerAngles = Vector3.Lerp(backSlotsTransforms[1].localEulerAngles, backSlotRotationDirection[0] * val, speed);
            }

        }
        private void RotateBodySlot_Beginner()
        {

        }
        #endregion
        #region Shielder
        private void RotateFrontSlot_Shielder()
        {
        }
        private void RotateMidSlot_Shielder()
        {
            const float angle = 38;
            midSlotsTransforms[1].localEulerAngles = midSlotRotationDirection[1] * m.TurnValue * angle;
            midSlotsTransforms[3].localEulerAngles = midSlotRotationDirection[1] * -m.TurnValue * angle;

        }
        private void RotateBackSlot_Shielder()
        {
            const float angle = 38;
            backSlotsTransforms[0].localEulerAngles = backSlotRotationDirection[0] * m.TurnValue * angle;
            backSlotsTransforms[1].localEulerAngles = backSlotRotationDirection[0] * -m.TurnValue * angle;
        }
        private void RotateBodySlot_Shielder()
        {
            const float angle = 20f;
            const float speed = .2f;

            bodySlotsTransforms[8].localEulerAngles = Vector3.Lerp(bodySlotsTransforms[0].localEulerAngles, bodySlotRotationDirection[0] * angle * (1 - e.SpeedPercentage01Clamped), speed);
            bodySlotsTransforms[9].localEulerAngles = Vector3.Lerp(bodySlotsTransforms[1].localEulerAngles, bodySlotRotationDirection[0] * angle * (1 - e.SpeedPercentage01Clamped), speed);
            bodySlotsTransforms[10].localEulerAngles = Vector3.Lerp(bodySlotsTransforms[2].localEulerAngles, bodySlotRotationDirection[0] * angle * (1 - e.SpeedPercentage01Clamped), speed);
            bodySlotsTransforms[11].localEulerAngles = Vector3.Lerp(bodySlotsTransforms[3].localEulerAngles, bodySlotRotationDirection[0] * angle * (1 - e.SpeedPercentage01Clamped), speed);
            bodySlotsTransforms[12].localEulerAngles = Vector3.Lerp(bodySlotsTransforms[4].localEulerAngles, bodySlotRotationDirection[0] * angle * (1 - e.SpeedPercentage01Clamped), speed);
            bodySlotsTransforms[13].localEulerAngles = Vector3.Lerp(bodySlotsTransforms[5].localEulerAngles, bodySlotRotationDirection[0] * angle * (1 - e.SpeedPercentage01Clamped), speed);
            bodySlotsTransforms[14].localEulerAngles = Vector3.Lerp(bodySlotsTransforms[6].localEulerAngles, bodySlotRotationDirection[0] * angle * (1 - e.SpeedPercentage01Clamped), speed);
            bodySlotsTransforms[15].localEulerAngles = Vector3.Lerp(bodySlotsTransforms[7].localEulerAngles, bodySlotRotationDirection[0] * angle * (1 - e.SpeedPercentage01Clamped), speed);
            bodySlotsTransforms[16].localEulerAngles = Vector3.Lerp(bodySlotsTransforms[8].localEulerAngles, bodySlotRotationDirection[0] * angle * (1 - e.SpeedPercentage01Clamped), speed);
            bodySlotsTransforms[17].localEulerAngles = Vector3.Lerp(bodySlotsTransforms[9].localEulerAngles, bodySlotRotationDirection[0] * angle * (1 - e.SpeedPercentage01Clamped), speed);
        }
        #endregion
        #region Maneuver
        private void RotateFrontSlot_Maneuver()
        {
        }
        private void RotateMidSlot_Maneuver()
        {
            // Rotation propeller parents
            const float angle = 55f;
            const float pSpeed = .15f;
            Vector3 target;
            if (mc.states.engine == States.Engine.accelerate || mc.states.engine == States.Engine.boost)
            {
                target = midSlotRotationDirection[0] * angle * e.SpeedPercentage01;
                // target = Vector3.Lerp(midSlotsTransforms[0].localEulerAngles, midSlotRotationDirection[0] * angle * e.SpeedPercentage01, pSpeed * 1.2f);
            }
            else
            {
                target = Vector3.Lerp(midSlotsTransforms[1].localEulerAngles, midSlotRotationDirection[0] * angle * e.SpeedPercentage01, pSpeed); // Vector3.zero, pSpeed);
            }
            midSlotsTransforms[1].localEulerAngles = target;
            midSlotsTransforms[4].localEulerAngles = target;
            midSlotsTransforms[8].localEulerAngles = target;
            midSlotsTransforms[11].localEulerAngles = target;

            // Rotation propeller
            const float speed = 9.5f;
            Vector3 rotation = midSlotsTransforms[2].localEulerAngles;
            rotation += midSlotRotationDirection[1] * speed;
            midSlotsTransforms[2].localEulerAngles = rotation;
            midSlotsTransforms[3].localEulerAngles = rotation;
            midSlotsTransforms[5].localEulerAngles = rotation;
            midSlotsTransforms[6].localEulerAngles = rotation;
            midSlotsTransforms[9].localEulerAngles = rotation;
            midSlotsTransforms[10].localEulerAngles = rotation;
            midSlotsTransforms[12].localEulerAngles = rotation;
            midSlotsTransforms[13].localEulerAngles = rotation;
        }
        private void RotateBackSlot_Maneuver()
        {
        }
        private void RotateBodySlot_Maneuver()
        {
            const float angle = 20f;
            const float speed = .2f;

            for (int i = 0; i < 10; i++)
            {              
                bodySlotsTransforms[i].localEulerAngles = Vector3.Lerp(bodySlotsTransforms[i].localEulerAngles, bodySlotRotationDirection[0] * angle * (1 - e.SpeedPercentage01Clamped), speed);
            }
        }
        #endregion   
    }
}