using Cinemachine;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Vehicle
{
    public abstract class SubBehavior
    {
        /// <summary>
        /// Called once when this behaviour is the current behaviour.
        /// </summary>
        public virtual void OnEnter()
        {

        }
        /// <summary>
        /// Called each fixed upade when this behaviour is the current behaviour.
        /// </summary>
        public virtual void OnStay()
        {

        }
        public virtual void OnNotActive()
        {

        }
        /// <summary>
        /// Called once when this behaviour is no longer the current behaviour.
        /// </summary>
        public virtual void OnExit()
        {

        }
    }
    public class RaycastImpact
    {
        public bool hasLanded;
        public RaycastHit hit;
        public RaycastHit lastFrameHit;
        public Vector3 targetedPosition;
        public float hitInclination;
    };
    [Serializable]
    public class States
    {
        public enum Engine
        {
            accelerate,
            brake,
            boost,
            none,
            inactive,
        };
        public enum Movement
        {
            forward,
            turn_left,
            turn_right,
            inactive,
        };
        public enum Hover
        {
            grounded_valid,
            grounded_invalid,
            controled_Fall,
            free_fall,
            inactive,
        };
        public enum Inertie
        {
            free,
            controlled,
            none,
            inactive,
        };
        public enum Integrity
        {
            shielded,
            unshielded,
            destroyed,
            inactive,
        }

        public Engine engine = Engine.inactive;
        public Movement movement = Movement.inactive;
        public Hover hover = Hover.inactive;
        public Inertie inertie = Inertie.inactive;
    }
    [Serializable]
    public struct PartsReferences
    {
        public Transform part_FrontLeft;
        public Transform part_FrontRight;
        public Transform part_MidLeft;
        public Transform part_MidRight;
        public Transform part_BackLeft;
        public Transform part_BackRight;
        public Transform part_Body;
    }
    [Serializable]
    public struct Statistics
    {
        public float acceleration;
        public float speed;
        public float maniability;
        public float shield;
    }

    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Movement), typeof(Hover), typeof(Engine))]
    [RequireComponent(typeof(CollisionB), typeof(Shield), typeof(Animation))]
    // [RequireComponent(typeof(ParticlesB))]
    [AddComponentMenu("_Scripts/Vehicle/Controller")]
    public class MainController : MonoBehaviour
    {
        public bool controllerDebug = false;

        #region BEHAVIOURS
        List<VehicleBehaviour> vehicleBehaviours;
        [HideInInspector] public Hover hoverB = null;
        [HideInInspector] public Engine engineB = null;
        [HideInInspector] public Movement movementB = null;
        [HideInInspector] public CollisionB collisionB = null;
        [HideInInspector] public Shield shield = null;
        [HideInInspector] public Animation animationB = null;
        // [HideInInspector] public ParticlesB particlesB = null;
        [HideInInspector] public VehicleCamera cameraV = null;
        #endregion
        #region COMPONENTS_REFERENCES
        Rigidbody rb;
        CapsuleCollider cc;
        VehiculeLayout vl;
        VehicleUI UI;
        [HideInInspector] public CharacterLapManager clm;
        #endregion
        #region VEHICLE_DATAS
        public States states = new States();
        public States previousStates = new States();
        public RaycastImpact raycastDatas;
        public bool isAlive = true;
        #endregion
        #region INPUTS_DATAS
        [SerializeField] Statistics currentStatistics;
        private float turn = 0f;
        private float acceleration = 0f;
        private bool boost;
        private bool brake;
        private bool changeCamera = false;
        private bool controllable;
        private bool invulnerable;
        #endregion
        #region GETTERS&SETTERS
        /// <summary>
        /// INPUT ENTRY : Turn, set horizontal axis here.
        /// </summary>
        public float TurnValue
        {
            get => movementB.TurnValue;
            set
            {
                value = Mathf.Clamp(value, -1, 1);
                turn = value;
            }
        }
        /// <summary>
        /// INPUT ENTRY : Acceleration, set vertical axis here.
        /// </summary>
        public float AccelerationValue
        {
            get => engineB.AccelerationValue;
            set
            {
                acceleration = 0;
                if (value >= 0)
                {
                    acceleration = value;
                }
            }
        }
        public bool ChangeCamera { get => changeCamera; set => changeCamera = value; }
        /// <summary>
        /// Return true if the player is currently accelerating.
        /// </summary>
        public bool IsAccelerating { get => engineB.isAccelerating; }
        public bool IsAlive { get => isAlive; set => isAlive = value; }
        /// <summary>
        /// INPUT ENTRY : boost.
        /// </summary>
        public bool IsBoosting { get => boost; set => boost = value; }
        /// <summary>
        /// INPUT ENTRY : Brake;
        /// </summary>
        public bool IsBraking { get => brake; set => brake = value; }
        /// <summary>
        /// Allow or disallow the sending of the inputs to the controller.
        /// </summary>
        public bool Controllable { get => controllable; set => controllable = value; }
        /// <summary>
        /// Change the vehicle statistics and recreated every behaviours.
        /// </summary>
        public Statistics VehicleStatistics
        {
            set
            {
                currentStatistics = value;
                InitVehicleStats(currentStatistics);
            }
        }

        public bool Invulnerable { get => invulnerable; set => invulnerable = value; }

        #endregion
        #region Static
        public static readonly Statistics defaultStatistics = new Statistics()
        {
            acceleration = 500,
            speed = 400,
            maniability = 800,
            shield = 500
        };
        #endregion
        #region Camera
        public Camera cam;
        public CinemachineVirtualCamera cm_cam;
        const float defaultFieldOfView = 45;
        const float minFieldOfView = 38f;
        const float maxFieldOfView = 55f;
        float fieldOfViewRatio;
        #endregion

        /// <summary>
        /// True if the script have linked cameras.
        /// </summary>
        bool isPlayer = false;

        private void Awake()
        {
            vl = GetComponentInParent<VehiculeLayout>();
            UI = GetComponent<VehicleUI>();

            CreateBehaviours();
            ResetRigidbodyToDefault();

            controllable = true;

            VehicleStatistics = defaultStatistics; // DEBUG
            vehicleBehaviours.ForEach(x => x.OnAwake());
        }
        private void Start()
        {
            clm = GetComponent<CharacterLapManager>();
            vehicleBehaviours.ForEach(x => x.OnStart());

            foreach (Transform t in transform.GetComponentsInChildren<Transform>())
            {
                if (t.gameObject.GetComponent<Camera>())
                {
                    cam = t.gameObject.GetComponent<Camera>();
                }
            }
            foreach (Transform t in transform.GetComponentsInChildren<Transform>())
            {
                if (t.gameObject.GetComponent<Cinemachine.CinemachineVirtualCamera>())
                {
                    cm_cam = t.gameObject.GetComponent<CinemachineVirtualCamera>();
                }
            }

            isPlayer = cam != null && cm_cam != null;

            fieldOfViewRatio = (maxFieldOfView - minFieldOfView) / (100 * engineB._Boost.boostedSpeedCap);
        }
        private void Update()
        {
            engineB.BeforeOnUpdate();

            if (controllerDebug)
            {
                TurnValue = Input.GetAxis("Horizontal");
                // AccelerationValue = Input.GetAxis("Vertical");
                AccelerationValue = Input.GetAxis("TriggerRight");
                brake = Input.GetAxisRaw("ButtonB") != 0;
                boost = Input.GetAxisRaw("TriggerLeft") != 0;
            }

            if (!controllable)
            {
                brake = false;
                TurnValue = 0;
                AccelerationValue = 0;
                boost = false;
            }

            SendInputs();
            UpdateStates();

            engineB.OnUpdate();
        }
        private void FixedUpdate()
        {
            engineB.OnFixedUpdate();
            movementB.OnFixedUpdate();
            hoverB.OnFixedUpdate();
        }
        private void LateUpdate()
        {
            animationB.OnLateUpdate();
            
            // CAMERA UPDATE
            if (isPlayer)
            {
                RadialBlur radialBlur = cam.GetComponent<RadialBlur>();

                float speedPercent = engineB.speedPercentage;

                float targetedFov = Mathf.Clamp(defaultFieldOfView + (speedPercent * fieldOfViewRatio), minFieldOfView, maxFieldOfView);


                cm_cam.m_Lens.FieldOfView = Mathf.Lerp(cm_cam.m_Lens.FieldOfView, targetedFov, 0.075f);

                radialBlur.Strength = speedPercent / 65;
                radialBlur.Distance = .075f;
            }
        }
        private void OnCollisionEnter(Collision collision)
        {
            collisionB.CollisionEnter(in collision);
        }
        private void OnCollisionStay(Collision collision)
        {
            collisionB.CollisionStay(in collision);
        }
        private void OnCollisionExit(Collision collision)
        {
            collisionB.CollisionExit(in collision);
        }
        /// <summary>
        /// Update the states relatives to the inputs.
        /// </summary>
        private void UpdateStates()
        {
            previousStates = states;

            // Engine
            if (boost) states.engine = States.Engine.boost;
            else if (brake) states.engine = States.Engine.brake;
            else if (IsAccelerating) states.engine = States.Engine.accelerate;
            else states.engine = States.Engine.none;

            // Movement
            if (TurnValue < 0) states.movement = States.Movement.turn_left;
            else if (TurnValue > 0) states.movement = States.Movement.turn_right;
            else states.movement = States.Movement.forward;

            // Inertie
            if (IsAccelerating) states.inertie = States.Inertie.none;
            else states.inertie = States.Inertie.controlled;
        }
        /// <summary>
        /// Send the inputs value in the needed behaviours.
        /// </summary>
        private void SendInputs()
        {
            movementB.TurnValue = turn;
            acceleration = Mathf.Clamp(acceleration, -1f, 1f);
            engineB.AccelerationValue = acceleration;
        }
        private void DestroyVehicle()
        {
            rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
            rb.isKinematic = true;
            controllable = false;
        }
        public void ResetForRespawn(Vector3? position = null, Quaternion? rotation = null)
        {
            //Debug.Log(name + " has respawned.");

            shield.ActivateShield();
            shield.currentShield = shield.defaultMaxShield;

            transform.position = position ?? transform.position;
            transform.rotation = rotation ?? transform.rotation;

            ResetRigidbodyToDefault();

            GameObject modelP = animationB.model.gameObject;

            modelP.GetComponentsInChildren<MeshRenderer>().ToList().ForEach(x => x.enabled = true);
            modelP.GetComponentsInChildren<SkinnedMeshRenderer>().ToList().ForEach(x => x.enabled = true);

            // particlesB.SwitchOnOff(ParticlesB.FxName.reactorDistortion, true);
            //particlesB.SwitchOnOff(ParticlesB.FxName.trails, true);

            controllable = true;
        }
        private void ResetRigidbodyToDefault()
        {
            rb.mass = 70;
            rb.drag = 0;
            rb.angularDrag = 8;
            rb.useGravity = false;
            rb.isKinematic = false;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            rb.interpolation = RigidbodyInterpolation.None;
            rb.freezeRotation = true;
            rb.velocity = Vector3.zero;
        }
        public void InitVehicleStats(Statistics statistics)
        {
            currentStatistics = new Statistics()
            {
                acceleration = statistics.acceleration / 1000,
                maniability = statistics.maniability / 1000,
                speed = statistics.speed / 1000,
                shield = statistics.shield / 1000
            };

            engineB._Inactive = new Engine.InactiveSub(engineB);
            engineB._Inertie = new Engine.InertieSub(engineB)
            {
                lossFactor = .2f + currentStatistics.maniability / 15,
            };
            engineB._Acceleration = new Engine.AccelerationSub(engineB)
            {
                acceleration = 0.35f + (currentStatistics.acceleration / 5.2f)
            };
            engineB._Boost = new Engine.BoostSub(engineB)
            {
                cooldown = 1.5f,
                consumption = .4f,
                maxCapacity = 100f,
                currentCapacity = 100f,
                regen = .8f,
                boostAccelerationMultiplier = 1f + currentStatistics.acceleration,

            };
            engineB.defaultMaxSpeed = 200f + currentStatistics.acceleration * 100;
            engineB._Boost.boostedSpeedCap = 1.3f + currentStatistics.acceleration / 1.85f;
            engineB._Brake = new Engine.BrakeSub(engineB)
            {
                strength = .950f + (.05f - currentStatistics.maniability / 50),
            };

            movementB.defaultRotationSpeed = currentStatistics.maniability * 10;
            movementB.defaultSmoothness = 0.04f - (currentStatistics.maniability / 70); // minus is better
            movementB.speedFactor = currentStatistics.maniability * 1.5f;
            movementB.minRotationSpeed = 2.8f + currentStatistics.maniability * 3.6f;

            shield.defaultMaxShield = 100 + currentStatistics.shield * 100;
            shield.currentShield = shield.defaultMaxShield;

            vehicleBehaviours.ForEach(x => x.OnStart());
        }
        private void CreateBehaviours()
        {
            gameObject.AddComponent<PhysicControls>();
            rb = GetComponent<Rigidbody>() ?? gameObject.AddComponent<Rigidbody>();
            engineB = GetComponent<Engine>() ?? gameObject.AddComponent<Engine>();
            movementB = GetComponent<Movement>() ?? gameObject.AddComponent<Movement>();
            hoverB = GetComponent<Hover>() ?? gameObject.AddComponent<Hover>();
            collisionB = GetComponent<CollisionB>() ?? gameObject.AddComponent<CollisionB>();
            shield = GetComponent<Shield>() ?? gameObject.AddComponent<Shield>();
            animationB = GetComponent<Animation>() ?? gameObject.AddComponent<Animation>();
            // particlesB = GetComponent<ParticlesB>() ?? gameObject.AddComponent<ParticlesB>();

            vehicleBehaviours = GetComponents<VehicleBehaviour>().ToList();
        }
    }
}