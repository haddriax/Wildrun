using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Vehicle
{
    [AddComponentMenu("_Scripts/Vehicle/Behaviours/Hover")]
    public class Hover : VehicleBehaviour
    {
        public abstract class HoverSubBehavior : SubBehavior
        {
            protected Hover hover;

            protected HoverSubBehavior(Hover hover)
            {
                this.hover = hover;
            }
        }

        public class GroundedSub : HoverSubBehavior
        {
            public GroundedSub(Hover hover) : base(hover)
            {
            }

            public override void OnEnter()
            {
            }
            public override void OnStay()
            {
                hover.HeighAlignment();
                hover.GroundAlign();
            }
            public override void OnExit()
            {
            }

        }

        public class FallingSub : HoverSubBehavior
        {
            public FallingSub(Hover hover) : base(hover)
            {
            }

            public override void OnEnter()
            {
                hover.accumulatedGravity = 0f;
            }
            public override void OnStay()
            {
                hover.Falling();
                hover.GroundAlign();
            }

            public override void OnExit()
            {
                hover.accumulatedGravity = 0f;
            }
        }

        int layerMask;

        float averageHoverHeight = 5.0f;
        [SerializeField] Transform theRaycast;

        readonly float hoverMargin = 1.5f;
        readonly float gripAngle = 0.4f;
        readonly float gravity = 2.4f;

        float accumulatedGravity = 0f;

        PhysicControls pc;

        // SubBehaviours
        GroundedSub groundedSub;
        FallingSub fallingSub;

        HoverSubBehavior currentSubState;
        HoverSubBehavior previousSubState;
        public HoverSubBehavior CurrentSubState { get => currentSubState; set => currentSubState = value; }
        public HoverSubBehavior PreviousSubState { get => previousSubState; set => previousSubState = value; }


        // Getters and Setters
        public States.Hover HoverState { get => mc.states.hover; set => mc.states.hover = value; }
        public RaycastImpact RaycastImpact { get => mc.raycastDatas; set => mc.raycastDatas = value; }

        public override void OnAwake()
        {
            base.OnAwake();

        }

        public override void OnStart()
        {
            base.OnStart();

            pc = gameObject.GetComponent<PhysicControls>();
            mc = gameObject.GetComponent<MainController>();

            groundedSub = new GroundedSub(this);
            fallingSub = new FallingSub(this);

            currentSubState = groundedSub;
            
            HoverInit();
        }

        public override void OnFixedUpdate()
        {
            Check();
            Execute();
        }

        void HoverInit()
        {
            RaycastImpact = new RaycastImpact
            {
                hasLanded = false,
            };

            layerMask = 1 << LayerMask.NameToLayer("Vehicle");
            layerMask = ~layerMask;
        }

        void UpdateRaycastDatas()
        {
            UpdateRaycastImpact();
        }

        private void UpdateRaycastImpact()
        {
            RaycastImpact.lastFrameHit = RaycastImpact.hit;

            RaycastImpact.hasLanded = Physics.Raycast(theRaycast.position, Vector3.down, out RaycastImpact.hit, Mathf.Infinity, layerMask);
            if (RaycastImpact.hasLanded)
            {
                RaycastImpact.targetedPosition = theRaycast.position + transform.TransformVector(Vector3.up * averageHoverHeight);
                RaycastImpact.hitInclination = 1 - Vector3.Dot(RaycastImpact.hit.normal, Vector3.up);
            }
            else
            {
                RaycastImpact.hit.normal = Vector3.up;
            }
        }

        void ChangeStates()
        {
            if (RaycastImpact.hit.distance > averageHoverHeight + hoverMargin)
            {
                HoverState = States.Hover.free_fall;
            }
            else if (Mathf.Abs(RaycastImpact.hitInclination) < gripAngle)
            {
                HoverState = States.Hover.grounded_valid;
            }
            else
            {
                HoverState = States.Hover.grounded_invalid;
            }

        }
        void Check()
        {
            mc.engineB.externalVelocityChange = Vector3.Lerp(mc.engineB.externalVelocityChange, Vector3.zero, 0.4f);
            UpdateRaycastDatas();
            ChangeStates();

            switch (HoverState)
            {
                case States.Hover.grounded_valid:
                    CurrentSubState = groundedSub;
                    break;
                case States.Hover.grounded_invalid:
                    CurrentSubState = fallingSub;
                    break;
                case States.Hover.free_fall:
                    CurrentSubState = fallingSub;
                    break;
                case States.Hover.inactive:
                    break;
                default:
                    throw new NotImplementedException(nameof(HoverState) + " : State not implemented.");
            }
        }
        void Execute()
        {
            if (PreviousSubState == null)
                PreviousSubState = CurrentSubState;

            if (PreviousSubState.GetType() != CurrentSubState.GetType())
                PreviousSubState.OnExit();

            switch (CurrentSubState)
            {
                case GroundedSub groundedSub:
                    if (!(PreviousSubState is GroundedSub))
                        groundedSub.OnEnter();
                    else
                        groundedSub.OnStay();
                    break;
                case FallingSub fallingSub:
                    if (!(PreviousSubState is FallingSub))
                        fallingSub.OnEnter();
                    else
                        fallingSub.OnStay();
                    break;
                case null:
                    throw new ArgumentNullException(nameof(CurrentSubState) + " is null.");
                default:
                    throw new ArgumentException(nameof(CurrentSubState) + " is invalid.");
            }

            ApplyGravity();
            PreviousSubState = CurrentSubState;
        }

        public void ApplyGravity()
        {
            if (accumulatedGravity == 0) return;

            mc.engineB.externalVelocityChange.y -= accumulatedGravity * Time.deltaTime * 100;
        }

        /// <summary>
        /// Directly change Y velocity to hover at the desired height.
        /// </summary>
        void HeighAlignment()
        {
            // Raycast at the center of the vehicle.

            // Get the lowest height from all Raycasts.
            float lowestHeight = RaycastImpact.hit.distance;

            // Get biggest angle from all Raycasts.
            float biggestAngle = RaycastImpact.hitInclination;

            // Get the heigh difference to cover
            float heightDifference = lowestHeight - averageHoverHeight;

            Vector3 pos = transform.position;
            float amount = Time.fixedDeltaTime
                                      * (1f + (heightDifference == 0 ? 1 : Mathf.Abs(heightDifference) * 12f)
                                      + biggestAngle * 100f);

            pos.y = Mathf.MoveTowards(pos.y,
                                      RaycastImpact.hit.point.y + averageHoverHeight,
                                      amount);
            rb.MovePosition(pos);
        }

        void Falling()
        {
            accumulatedGravity += gravity;
        }
        
        void GroundAlign()
        {
            Vector3 normalToAlign = RaycastImpact.hit.normal;
            Transform transformToAlign = mc.animationB.modelParent;
            /*
            Vector3 forward = Vector3.Cross(transformToAlign.right, normalToAlign);
            Quaternion groundRotation = Quaternion.LookRotation(forward, normalToAlign);
            */
            Quaternion groundRotation = Quaternion.LookRotation(transformToAlign.forward, normalToAlign);
            groundRotation = Quaternion.Lerp(transformToAlign.rotation, groundRotation, 0.08f);
            transformToAlign.rotation = groundRotation;
        }
        public float ClampAngle(float angle, float min, float max)
        {
            angle = Mathf.Repeat(angle, 360);
            min = Mathf.Repeat(min, 360);
            max = Mathf.Repeat(max, 360);
            bool inverse = false;
            var tmin = min;
            var tangle = angle;
            if (min > 180)
            {
                inverse = !inverse;
                tmin -= 180;
            }
            if (angle > 180)
            {
                inverse = !inverse;
                tangle -= 180;
            }
            var result = !inverse ? tangle > tmin : tangle < tmin;
            if (!result)
                angle = min;

            inverse = false;
            tangle = angle;
            var tmax = max;
            if (angle > 180)
            {
                inverse = !inverse;
                tangle -= 180;
            }
            if (max > 180)
            {
                inverse = !inverse;
                tmax -= 180;
            }

            result = !inverse ? tangle < tmax : tangle > tmax;
            if (!result)
                angle = max;
            return angle;
        }
    }

}