using FMODUnity;
using System;
using System.Collections;
using UnityEditor;
using UnityEngine;

namespace Vehicle
{

    [AddComponentMenu("_Scripts/Vehicle/Behaviours/Engine")]
    public class Engine : VehicleBehaviour
    {

        public abstract class EngineSubBehavior : SubBehavior
        {
            protected Engine engine;

            protected EngineSubBehavior(Engine engine)
            {
                this.engine = engine;
            }

            public override void OnEnter()
            {
                // Debug.Log("Enter : " + GetType().ToString());
            }
        }

        public class AccelerationSub : EngineSubBehavior
        {
            public float acceleration;
            public float decceleration;
            public AccelerationSub(Engine engine)
                : base(engine)
            {
                this.decceleration = engine._Inertie.lossFactor;
            }

            public override void OnEnter()
            {
                base.OnEnter();
            }

            public override void OnStay()
            {
                float desiredSpeed = Mathf.Clamp(engine.AccelerationValue * 100, 0, 100); // 0 to 100 ratio for acceleration curve

                if (engine.speedPercentage > 100) // Accelerate, but is faster than max speed
                {
                    engine.speedPercentage = Mathf.MoveTowards(engine.speedPercentage, desiredSpeed, engine._Inertie.lossFactor * 2);
                }
                else if (engine.speedPercentage > desiredSpeed) // Accelerate, but is faster than trigger pressure
                {
                    engine.speedPercentage = Mathf.MoveTowards(engine.speedPercentage, desiredSpeed, decceleration);
                }
                else // Accelerate, apply maximum acceleration
                {
                    engine.speedPercentage = Mathf.MoveTowards(engine.speedPercentage, desiredSpeed, acceleration);
                }

                AccelerateVelocity();
            }
            public override void OnExit()
            {
            }
            public void AccelerateVelocity()
            {
                float magnitude = engine.accelerationCurve.Evaluate(engine.speedPercentage);
                engine.internalVelocityChange = engine.transform.forward * magnitude;
            }
        }
        public class BoostSub : EngineSubBehavior
        {
            public float boostedSpeedCap;
            public float boostAccelerationMultiplier;
            public float maxCapacity;
            public float consumption;
            public float regen;
            public float cooldown;
            public float currentCapacity;

            // Interne
            bool canEnterBoost = false;
            public float CurrentCapacity01 { get => currentCapacity / 100; }

            public BoostSub(Engine engine)
            : base(engine)
            {
                currentCapacity = maxCapacity;
            }

            public override void OnEnter()
            {
                base.OnEnter();
                engine.maxSpeed *= boostedSpeedCap;
                canEnterBoost = currentCapacity > 10f;

                /*
                if(canEnterBoost)
                {
                   engine.mc.particlesB.SwitchOnOff(ParticlesB.FxName.reactorFlames, true);
                }
                */
            }

            public override void OnStay()
            {
                if (currentCapacity > 0 && canEnterBoost) // Can boost
                {
                    engine.speedPercentage = Mathf.MoveTowards(engine.speedPercentage, 100 * boostedSpeedCap, engine._Acceleration.acceleration * boostAccelerationMultiplier);
                    engine.Boost(); // Sound ... to move elsewhere
                }
                else
                {
                    engine.speedPercentage = Mathf.MoveTowards(engine.speedPercentage, 100, engine._Acceleration.decceleration * 1.8f);
                }


                engine._Acceleration.AccelerateVelocity();

                currentCapacity -= consumption;

            }
            public override void OnNotActive()
            {
                currentCapacity = Mathf.Clamp(currentCapacity + regen, 0, maxCapacity);
                canEnterBoost = currentCapacity > 10;
            }

            public override void OnExit()
            {
                engine.maxSpeed = engine.defaultMaxSpeed;
                engine.boostCooldown = engine.StartCoroutine(engine.BoostCooldown(cooldown));
                canEnterBoost = false;
                // engine.mc.particlesB.SwitchOnOff(ParticlesB.FxName.reactorFlames, false);
            }
        }
        public class BrakeSub : EngineSubBehavior
        {
            public float strength;

            public BrakeSub(
                Engine engine)
                : base(engine)
            {
            }

            public override void OnEnter()
            {
                base.OnEnter();
            }

            public override void OnStay()
            {
                engine.speedPercentage *= strength;
                engine._Inertie.OnStay();
            }

            public override void OnExit()
            {
            }
        }
        public class InactiveSub : EngineSubBehavior
        {
            public InactiveSub(Engine engine) : base(engine)
            {
            }

            public override void OnEnter()
            {
                base.OnEnter();
            }

            public override void OnStay()
            {
            }

            public override void OnExit()
            {
            }
        }
        public class InertieSub : EngineSubBehavior
        {
            public float lossFactor;

            // Intern
            private Vector3 storedVelocity = Vector3.zero;

            public Vector3 StoredVelocity
            {
                get => storedVelocity;
                set => storedVelocity = value;
            }
            public InertieSub(Engine engine)
                : base(engine)
            {
            }

            public float SpeedValueFromVelocity { get => engine.inverseAccelerationCurve.Evaluate(engine._Inertie.StoredVelocity.magnitude); }
            public override void OnEnter()
            {
                base.OnEnter();
            }
            public override void OnStay()
            {
                if (engine.speedPercentage > 100) // Accelerate, but is faster than max speed
                {
                    engine.speedPercentage = Mathf.MoveTowards(engine.speedPercentage, 0, engine._Inertie.lossFactor * 2);
                }
                else // Accelerate, apply maximum acceleration
                {
                    engine.speedPercentage = Mathf.MoveTowards(engine.speedPercentage, 0, engine._Inertie.lossFactor);
                }

                engine._Acceleration.AccelerateVelocity();
                engine.speedPercentage = Mathf.MoveTowards(engine.speedPercentage, 0, lossFactor / 2);

            }

            public override void OnNotActive()
            {
            }
            public override void OnExit()
            {

            }
        }

        SubBehavior[] subBehaviors = new SubBehavior[5];
        public InertieSub _Inertie { get => (InertieSub)subBehaviors[0]; set => subBehaviors[0] = value; }
        public AccelerationSub _Acceleration { get => (AccelerationSub)subBehaviors[1]; set => subBehaviors[1] = value; }
        public BoostSub _Boost { get => (BoostSub)subBehaviors[2]; set => subBehaviors[2] = value; }
        public BrakeSub _Brake { get => (BrakeSub)subBehaviors[3]; set => subBehaviors[3] = value; }
        public InactiveSub _Inactive { get => (InactiveSub)subBehaviors[4]; set => subBehaviors[4] = value; }

        SubBehavior currentSubState;
        SubBehavior previousSubState;
        public SubBehavior CurrentSubState { get => currentSubState; set => currentSubState = value; }
        public SubBehavior PreviousSubState { get => previousSubState; set => previousSubState = value; }

        Hover hover;

        // Sound
        SoundManagerFMOD managerFMOD;
        private int currentInstanceFMOD;

        UserControllerRewired rewired;
        // ---

        [SerializeField] AnimationCurve accelerationCurve;
        [SerializeField] AnimationCurve inverseAccelerationCurve;

        public float defaultMaxSpeed = 0f;
        float maxSpeed;

        public float maxBoostSpeed = 0f;

        float accelerationValue = 0f;
        float previousAccelerationValue = 0f;

        /// <summary>
        /// 0 to 100 for normal speed, more is overcapped.
        /// </summary>
        public float speedPercentage = 0f;

        /// <summary>
        /// Store the external velocity change applied this frame. (Gravity, Hovering ...)
        /// </summary>
        public Vector3 externalVelocityChange = new Vector3();
        /// <summary>
        /// Store the internal velocity change applied this frame. (Acceleration, Inertie, ...)
        /// </summary>
        public Vector3 internalVelocityChange = new Vector3();

        // Memory
        [HideInInspector] public bool isAccelerating = false;

        public float AccelerationValue
        {
            get => accelerationValue;
            set
            {
                isAccelerating = (value != 0 && value >= previousAccelerationValue - 0.04f);
                previousAccelerationValue = accelerationValue;
                accelerationValue = value;
            }
        }

        public float SpeedPercentage01 { get => speedPercentage / 100; }
        public float SpeedPercentage01Clamped { get => Mathf.Clamp01(speedPercentage / 100); }

        public override void OnAwake()
        {
            // Creating Substates
            // subBehaviors[0] = new InertieSub(this);
            // subBehaviors[1] = new AccelerationSub(this);
            // subBehaviors[2] = new BoostSub(this);
            // subBehaviors[3] = new BrakeSub(this);
            // subBehaviors[4] = new InactiveSub(this);            
            externalVelocityChange = new Vector3();
            internalVelocityChange = new Vector3();

            base.OnAwake();
        }

        public override void OnStart()
        {
            base.OnStart();

            hover = GetComponent<Hover>();
            rewired = gameObject.GetComponent<UserControllerRewired>(); mc = GetComponent<MainController>();

            managerFMOD = SoundManagerFMOD.GetInstance();
            currentInstanceFMOD = managerFMOD.InitInstance("event:/FX/InGame/Vehicle2");

            managerFMOD.PlayInstance(currentInstanceFMOD, transform, GetComponent<Rigidbody>());
            StudioListener listener = gameObject.GetComponent<StudioListener>();

            try
            {
                if (rewired.player == null)
                {
                    listener.ListenerNumber = 0;
                }
                else
                {
                    listener.ListenerNumber = rewired.player.id;
                }
            }
            catch
            {

            }
            

            maxSpeed = defaultMaxSpeed;

            accelerationCurve = new AnimationCurve();
            accelerationCurve.AddKey(0, defaultMaxSpeed * 0f);
            accelerationCurve.AddKey(100, defaultMaxSpeed * 1.00f);
            accelerationCurve.AddKey(150, defaultMaxSpeed * _Boost.boostedSpeedCap);

            for (int i = 0; i < accelerationCurve.keys.Length; i++)
            {
                accelerationCurve.keys[i].outTangent = 0f;
            }

            inverseAccelerationCurve = new AnimationCurve();
            for (int i = 0; i < accelerationCurve.keys.Length; i++)
            {
                int a = inverseAccelerationCurve.AddKey(accelerationCurve.keys[i].value, accelerationCurve.keys[i].time);
                //inverseAccelerationCurve.keys[a].outTangent = 0f;
            }

            PreviousSubState = _Inactive;
            currentSubState = _Inactive;
        }

        public override void BeforeOnUpdate()
        {
            isAccelerating = false;
        }

        public override void OnFixedUpdate()
        {
            Check();
            Execute();
        }

        Coroutine boostCooldown;
        public IEnumerator BoostCooldown(float cd)
        {
            // managerFMOD.SetBurst(0, currentInstanceFMOD);
            yield return new WaitForSeconds(cd);
            boostCooldown = null;
        }

        void Check()
        {
            switch (mc.states.engine)
            {
                case States.Engine.accelerate:
                    currentSubState = _Acceleration;
                    break;
                case States.Engine.boost:
                    if (boostCooldown == null)
                        currentSubState = _Boost;
                    break;
                case States.Engine.brake:
                    currentSubState = _Brake;
                    break;
                case States.Engine.none:
                    currentSubState = _Inertie;
                    break;
                case States.Engine.inactive:
                    currentSubState = _Inactive;
                    break;
                default:
                    throw new ArgumentException();
            }
        }

        void Execute()
        {
            internalVelocityChange = Vector3.Lerp(internalVelocityChange, Vector3.zero, 0.1f);

            Type currentSubStateType = CurrentSubState.GetType();

            if (previousSubState == null)
                previousSubState = currentSubState;

            if (PreviousSubState.GetType() != currentSubStateType)
                PreviousSubState.OnExit();

            for (int i = 0; i < subBehaviors.Length; i++)
            {
                if (currentSubStateType != subBehaviors[i].GetType())
                    subBehaviors[i].OnNotActive();
            }

            switch (CurrentSubState)
            {
                case AccelerationSub accelerationSub:
                    if (!(PreviousSubState is AccelerationSub))
                        accelerationSub.OnEnter();
                    else
                        accelerationSub.OnStay();
                    break;
                case BrakeSub brakeSub:
                    if (!(PreviousSubState is BrakeSub))
                        brakeSub.OnEnter();
                    else
                    {
                        brakeSub.OnStay();
                    }
                    break;
                case BoostSub boostSub:
                    if (!(PreviousSubState is BoostSub))
                        boostSub.OnEnter();
                    else
                        boostSub.OnStay();
                    break;
                case InactiveSub inactiveSub:
                    if (!(PreviousSubState is InactiveSub))
                        inactiveSub.OnEnter();
                    else
                        inactiveSub.OnStay();
                    break;
                case InertieSub inertie:
                    if (!(PreviousSubState is InertieSub))
                        inertie.OnEnter();
                    else
                        inertie.OnStay();
                    break;
                case null:
                    throw new ArgumentNullException(nameof(CurrentSubState) + " is null.");
                default:
                    throw new ArgumentException(nameof(CurrentSubState) + " is invalid.");
            }

            // Apply all the velocity changes to the rigidbody.
            rb.velocity = (internalVelocityChange + externalVelocityChange);
            previousSubState = currentSubState;

            managerFMOD.SetAcceleration(speedPercentage, currentInstanceFMOD);

        }

        void AccelerateVelocity(float speedPercentage = 0f)
        {
            float magnitude = accelerationCurve.Evaluate(speedPercentage);
            Vector3 velocity = transform.forward * magnitude;
            rb.velocity = velocity;
        }

        void Boost()
        {
            managerFMOD.SetBurst(1, currentInstanceFMOD);

        }
    }

}