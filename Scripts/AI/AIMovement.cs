using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace AI
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(AIDifficulty))]
    public class AIMovement : MonoBehaviour
    {
        /// <Sound>
        private SoundManagerFMOD SMF;
        private int currentInstance;
        [SerializeField]
        private bool playable;
        /// </Sound>

        /// <SplinePoint>
        private CatmullRomSpline CRSpline;
        private Vector3[] currentPoint;
        public Vector3[] CurrentPoint { get => currentPoint; }
        private int currentPointsIndex;
        private GameObject[] currentTurnPoint;
        private int currentTurnsIndex;
        /// </SplinePoint>

        /// <AnnexeSplinePoint>
        private byte useShortcut;
        private bool shortcutDefined;
        private float percentageChance;
        private CatmullRomSpline[] CRAnnexeSpline;
        private Vector3[][] currentAnnexePoint;
        private int[] currentAnnexePointsIndex;
        private int currentAnnexeIndex;
        private byte sizeAnnexe;
        /// </AnnexeSplinePoint>

        /// <Speed>
        [SerializeField]
        private float currentSpeed;
        [SerializeField]
        private float maxSpeed = 100.0f;
        public float totalDistance;
        public bool followRoad = false;
        private bool hasBeenReplaceOnSpline;
        private enum States { Inactive, Forward, Break, Bumped, Boost };
        private States currentState;
        private float timerToBoost;
        /// </Speed>

        /// <Rotation>
        [SerializeField]
        [Range(10.0f, 30.0f)]
        private float speedTurn = 20.0f;
        /// </Rotation>

        /// <Difficulty>
        private AIDifficulty aiDifficulty;
        /// </Difficulty>

        /// <DistanceRegulation>      
        private GameObject[] othersVehicles;
        /// </DistanceRegulation>

        /// <Debug>
        [SerializeField]
        private bool debugCurrentPoint = false;
        private GameObject debugSphere;
        private GameObject debugSphereTemp;
        /// </Debug>

        private Vehicle.MainController mainController;

        #region Init
        public void Init()
        {
            //<Sound>
            SMF = SoundManagerFMOD.GetInstance();
            currentInstance = SMF.InitInstance("event:/FX/InGame/Vehicle2AI");
            SMF.PlayInstance(currentInstance, transform, GetComponent<Rigidbody>());
            //<\Sound>

            CRSpline = GameObject.FindGameObjectWithTag("PrincipalCatmullRom").GetComponent<CatmullRomSpline>();
            currentPoint = new Vector3[CRSpline.pathPoints.Count];
            currentPointsIndex = 0;
            useShortcut = 0;
            shortcutDefined = false;
            percentageChance = 0.0f;
            sizeAnnexe = (byte)GameObject.FindGameObjectsWithTag("AnnexeCatmullRom").Length;
            CRAnnexeSpline = new CatmullRomSpline[sizeAnnexe];
            currentAnnexePoint = new Vector3[sizeAnnexe][];
            for (byte i = 0; i < sizeAnnexe; i++)
            {
                CRAnnexeSpline[i] = GameObject.FindGameObjectsWithTag("AnnexeCatmullRom")[i].GetComponent<CatmullRomSpline>();
                currentAnnexePoint[i] = new Vector3[CRAnnexeSpline[i].pathPoints.Count];
            }
            currentAnnexePointsIndex = new int[sizeAnnexe];
            for (byte i = 0; i < sizeAnnexe; i++)
            {
                currentAnnexePointsIndex[i] = 0;
            }
            currentAnnexeIndex = sizeAnnexe - 1;
            currentTurnPoint = GameObject.FindGameObjectsWithTag("TurnTrigger");
            currentTurnsIndex = 0;
            currentSpeed = 0.0f;
            totalDistance = 0.0f;
            followRoad = false;
            hasBeenReplaceOnSpline = true;
            othersVehicles = GameObject.FindGameObjectsWithTag("AIVehicle");
            currentState = States.Forward;
            timerToBoost = 0.0f;
            aiDifficulty = this.GetComponent<AIDifficulty>();
            mainController = this.GetComponent<Vehicle.MainController>();

            InitPoints();
        }
        #endregion

        #region InitPoints
        private void InitPoints()
        {
            CRSpline.pathPoints.CopyTo(currentPoint);
            for (byte i = 0; i < sizeAnnexe; i++)
            {
                CRAnnexeSpline[i].pathPoints.CopyTo(currentAnnexePoint[i]);
            }
            //CRSpline.pathPoints.Clear();

            // Define the index of nearest turnPoint (little broken, need to work with spline)
            //for (byte i = 0; i < currentTurnPoint.Length; i++)
            //{
            //    if (Vector3.Distance(this.transform.position, currentTurnPoint[i].transform.position) < Vector3.Distance(this.transform.position, currentTurnPoint[currentTurnsIndex].transform.position))
            //    {
            //        currentTurnsIndex = i;
            //    }
            //}
            //currentTurnsIndex = (currentTurnsIndex + 1) % currentTurnPoint.Length;

            // Define the 1st point (Nearest of player)
            ReplaceAIOnSpline();
        }
        #endregion

        #region ReplaceAIOnSpline
        // Replace the AI correctly on the spline
        private void ReplaceAIOnSpline()
        {
            //if (useShortcut == 0)
            //{
            for (int i = 0; i < currentPoint.Length; i++)
            {
                if (Vector3.Distance(this.transform.position, currentPoint[i]) < Vector3.Distance(this.transform.position, currentPoint[(int)totalDistance % currentPoint.Length]))
                {
                    totalDistance = (float)i;
                }
            }
            //}
            //else
            //{
            //    for (int i = 0; i < currentPoint.Length; i++)
            //    {
            //        if (Vector3.Distance(currentAnnexePoint[currentAnnexeIndex][currentAnnexePointsIndex[currentAnnexeIndex]], currentPoint[i]) < 5.0f)
            //        {
            //            totalDistance = (float)i;
            //        }
            //    }
            //}
        }
        #endregion

        #region CalculPoint
        private void CalculFuturePoint(float distance)
        {
            // Check if distance is different of 0 because we will divide with it
            if (distance != 0.0f)
            {
                // We create a ratio so that the point always remains at a certain distance
                float ratioDistance = (30.0f * (aiDifficulty.CurrentAIDifficulty + 0.2f)) / distance;
                //totalDistance += ratioDistance * ((currentSpeed + mainController.engineB.speedPercentage) / 2.0f) * Time.deltaTime;
                totalDistance += ratioDistance * mainController.engineB.speedPercentage * Time.deltaTime;
                if (useShortcut == 0)
                {
                    currentPointsIndex = (int)totalDistance % currentPoint.Length;
                }
                else
                {
                    currentAnnexePointsIndex[currentAnnexeIndex] = ((int)totalDistance < (currentAnnexePoint[currentAnnexeIndex].Length - 1)) ? (int)totalDistance : (currentAnnexePoint[currentAnnexeIndex].Length - 1);

                    //print(this.transform.parent.name + ": Current Point: " + currentAnnexePointsIndex[currentAnnexeIndex] + " | Size Max: " + (currentAnnexePoint[currentAnnexeIndex].Length - 1));
                }
            }

            // Replace the AI on the principal spline correctly when the shortcut is finished
            if (useShortcut == 1)
            {
                if (currentAnnexePoint[currentAnnexeIndex][currentAnnexePointsIndex[currentAnnexeIndex]] == currentAnnexePoint[currentAnnexeIndex][currentAnnexePoint[currentAnnexeIndex].Length - 1])
                {
                    ReplaceAIOnSpline();
                    useShortcut = 0;
                }
            }
        }
        #endregion

        #region Rotation
        private void Rotation(Vector3 tempCurrentPoint)
        {
            Vector3 relativeVector = this.transform.InverseTransformPoint(tempCurrentPoint);
            float newSteer = relativeVector.x / relativeVector.magnitude;
            float difficulty = aiDifficulty.CurrentAIDifficulty + (1.0f - aiDifficulty.MaxAIDifficulty);
            float t = speedTurn * difficulty * Time.deltaTime;
            mainController.TurnValue = Mathf.Lerp(mainController.TurnValue, newSteer, t);
        }
        #endregion

        #region DistanceRegulation
        private void DistanceRegulation()
        {
            if (othersVehicles.Length > 1)
            {
                for (byte i = 0; i < othersVehicles.Length; i++)
                {
                    if (this.transform.parent != othersVehicles[i])
                    {
                        for (byte j = 0; j < othersVehicles[i].transform.childCount; j++)
                        {
                            if (othersVehicles[i].transform.GetChild(j).name.Contains("Layout"))
                            {
                                float distance = Vector3.Distance(this.transform.position, othersVehicles[i].transform.GetChild(j).transform.position);

                                // Check distance to execute test
                                if (distance <= 5.0f)
                                {
                                    Vector3 toTarget = (othersVehicles[i].transform.GetChild(j).transform.position - this.transform.position).normalized;

                                    // Check if the target is forward
                                    if (Vector3.Dot(toTarget, this.transform.forward) > 0)
                                    {
                                        currentState = States.Break;
                                    }
                                }
                                else
                                {
                                    currentState = States.Forward;
                                }
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region CheckCanBoost
        private void CheckCanBoost()
        {
            timerToBoost += Time.deltaTime;

            if (timerToBoost >= 10.0f)
            {
                Vector3 tempCurrentPoint = (useShortcut == 0) ?
                    currentPoint[(currentPointsIndex + 150) % currentPoint.Length] :
                    currentAnnexePoint[currentAnnexeIndex][(currentAnnexePointsIndex[currentAnnexeIndex] + 150) % currentAnnexePoint[currentAnnexeIndex].Length];
                Vector3 tempPos = new Vector3(tempCurrentPoint.x, this.transform.position.y, tempCurrentPoint.z);
                Vector3 right = this.transform.TransformDirection(Vector3.right).normalized;
                Vector3 toOther = (tempPos - this.transform.position).normalized;

                float tempDot = Vector3.Dot(right, toOther);

                if (Mathf.Abs(mainController.TurnValue) < 0.1f && Mathf.Abs(tempDot) < 0.2f)
                {
                    currentState = States.Boost;
                }
                else
                {
                    currentState = States.Forward;
                    timerToBoost = 0.0f;
                }
            }
            else if (timerToBoost >= 11.0f)
            {
                currentState = States.Forward;
                timerToBoost = 0.0f;
            }
        }

        #endregion

        #region CheckDestroyed
        private void CheckDestroyed()
        {
            if(mainController.collisionB.hasBeenDestroyed)
            {
                ReplaceAIOnSpline();
                mainController.collisionB.hasBeenDestroyed = false;
            }
        }
        #endregion

        #region Movements
        public void Movements()
        {
            if (followRoad)
            {
                // Replace AI correctly when followRoad is true
                if (!hasBeenReplaceOnSpline)
                {
                    ReplaceAIOnSpline();
                    currentState = States.Forward;
                    hasBeenReplaceOnSpline = true;
                }

                // Make offset on the Y of currentPoint, we need to align it with the AI
                Vector3 tempCurrentPoint = (useShortcut == 0) ?
                    new Vector3(currentPoint[currentPointsIndex].x, this.transform.position.y, currentPoint[currentPointsIndex].z) :
                    new Vector3(currentAnnexePoint[currentAnnexeIndex][currentAnnexePointsIndex[currentAnnexeIndex]].x, this.transform.position.y, currentAnnexePoint[currentAnnexeIndex][currentAnnexePointsIndex[currentAnnexeIndex]].z);
                
                // Get the distance between AI and "currentPoint"
                float distance = Vector3.Distance(this.transform.position, tempCurrentPoint);

                CalculFuturePoint(distance);

                if (distance >= 1.0f)
                {
                    mainController.AccelerationValue = currentSpeed / maxSpeed;
                }

                Rotation(tempCurrentPoint);
                DistanceRegulation();
                CheckCanBoost();
                CheckDestroyed();
            }
            else
            {
                currentState = States.Inactive;
                hasBeenReplaceOnSpline = false;
            }
        }
        #endregion

        #region Speed        
        private void MoveForward()
        {
            mainController.IsBraking = false;
            mainController.IsBoosting = false;

            if (currentSpeed > maxSpeed / 2.0f)
            {
                currentSpeed += 80.0f * aiDifficulty.CurrentAIDifficulty * Time.deltaTime;
            }
            else
            {
                currentSpeed += 100.0f * aiDifficulty.CurrentAIDifficulty * Time.deltaTime;
            }

            currentSpeed = Mathf.Clamp(currentSpeed, 0.0f, maxSpeed);
        }

        private void Break()
        {
            if (mainController.engineB.speedPercentage > maxSpeed / 1.5f)
            {
                if (currentSpeed > maxSpeed / 1.25f)
                {
                    currentSpeed -= (currentSpeed / 2.0f) * Time.deltaTime;
                    currentSpeed = Mathf.Clamp(currentSpeed, 0.0f, maxSpeed);
                }
                mainController.IsBraking = true;
            }
            else
            {
                currentState = States.Forward;
            }
        }

        private void Bump()
        {
            mainController.IsBraking = false;
            mainController.IsBoosting = false;

            currentSpeed = 70.0f;
        }

        private void AngleBreak()
        {
            if (Mathf.Abs(mainController.TurnValue) > 0.5f)
            {
                currentState = States.Break;
            }
            else
            {
                currentState = States.Forward;
            }
        }

        private void Boost()
        {
            mainController.IsBoosting = true;
        }

        public void ManageSpeed()
        {
            switch (currentState)
            {
                case States.Inactive:
                    mainController.TurnValue = 0.0f;
                    mainController.AccelerationValue = -1.0f;
                    break;
                case States.Forward:
                    AngleBreak();
                    MoveForward();
                    break;
                case States.Break:
                    AngleBreak();
                    Break();
                    break;
                case States.Bumped:
                    Bump();
                    break;
                case States.Boost:
                    AngleBreak();
                    MoveForward();
                    Boost();
                    break;

                default:
                    MoveForward();
                    Debug.LogError("Impossible to make the AI moving correctly.");
                    break;
            }

            //<Sound>
            SMF.SetAccelerationAI(currentSpeed, currentInstance);
            //<\Sound>
        }
        #endregion

        #region Shortcut
        private void CanUseShortcut()
        {
            percentageChance = Random.value;
            useShortcut = (percentageChance < aiDifficulty.CurrentAIDifficulty) ? (byte)1 : (byte)0;

            //print(this.transform.parent.name + ": Percantage Chance: " + (percentageChance * 100.0f) + "% < " + (aiDifficulty.CurrentAIDifficulty * 100.0f) + "%" + " Use Shortcut: " + ((useShortcut == 1) ? "True" : "False"));
        }
        #endregion

        private void OnTriggerEnter(Collider other)
        {
            if (followRoad)
            {
                if (other.tag == "ShortcutTrigger")
                {
                    if (shortcutDefined)
                        return;

                    CanUseShortcut();

                    if (useShortcut == 1)
                    {
                        totalDistance = 0.0f;
                    }

                    currentAnnexeIndex = (currentAnnexeIndex + 1) % sizeAnnexe;

                    shortcutDefined = true;
                }
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (followRoad)
            {
                if (other.tag == "TurnTrigger")
                {
                    //if (currentSpeed > maxSpeed / 1.5f)
                    //{
                    currentState = States.Break;
                    //}
                    //else
                    //{
                    //    currentState = States.Forward;
                    //}
                }

                if (other.tag == "SpeedBumpTrigger")
                {
                    currentState = States.Bumped;
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (followRoad)
            {
                if (other.tag == "TurnTrigger")
                {
                    currentState = States.Forward;
                }

                if (other.tag == "ShortcutTrigger")
                {
                    if (shortcutDefined)
                    {
                        shortcutDefined = false;
                    }
                }
            }
        }

        #region DEBUG
        public void DebugAI()
        {
            #region currentPoint
            // Init
            if (debugCurrentPoint && !debugSphere && !debugSphereTemp)
            {
                debugSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                debugSphere.GetComponent<SphereCollider>().enabled = false;

                debugSphereTemp = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                debugSphereTemp.GetComponent<SphereCollider>().enabled = false;
            }
            // Delete
            else if (!debugCurrentPoint && debugSphere && debugSphereTemp)
            {
                Destroy(debugSphere);
                Destroy(debugSphereTemp);
            }

            // Update
            if (debugCurrentPoint && debugSphere && debugSphereTemp)
            {
                if (useShortcut == 0)
                {
                    debugSphere.transform.position = currentPoint[currentPointsIndex];
                    debugSphereTemp.transform.position = currentPoint[(currentPointsIndex + 150) % currentPoint.Length];
                }
                else
                {
                    debugSphere.transform.position = currentAnnexePoint[currentAnnexeIndex][currentAnnexePointsIndex[currentAnnexeIndex]];
                    debugSphereTemp.transform.position = currentAnnexePoint[currentAnnexeIndex][(currentAnnexePointsIndex[currentAnnexeIndex] + 150) % currentAnnexePoint[currentAnnexeIndex].Length];
                }
            }
            #endregion
        }
        #endregion
    }
}