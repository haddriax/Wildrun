using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(AIMovement))]
    public class AIRoad : MonoBehaviour
    {
        /// <Road>
        private bool changeRoad;
        private float offsetRoad;
        /// </Road>

        private AIMovement aiMovement;

        #region Init
        public void Init()
        {
            aiMovement = this.GetComponent<AIMovement>();

            InitVehiclePosOnSpline();
            InitChangeRoad();
            ChangeRoad();
        }
        #endregion

        #region CalculRoad
        private void CalculRoad()
        {
            for (int i = 0; i < aiMovement.CurrentPoint.Length; i++)
            {
                float temp;
                float ratio = (i / 10.0f);

                if (ratio <= 1.0f)
                {
                    temp = offsetRoad * ratio;
                }
                else
                {
                    temp = offsetRoad;
                }

                aiMovement.CurrentPoint[i] = new Vector3(aiMovement.CurrentPoint[i].x + temp, aiMovement.CurrentPoint[i].y, aiMovement.CurrentPoint[i].z);
            }
        }
        #endregion

        #region InitVehiclePos
        private void InitVehiclePosOnSpline()
        {
            // Set vehicle position correctly at the spawn
            Vector3 right = this.transform.TransformDirection(Vector3.right);
            Vector3 toOther = aiMovement.CurrentPoint[(int)aiMovement.totalDistance] - this.transform.position;
            offsetRoad = Vector3.Dot(right, toOther) * -1;
            offsetRoad = Mathf.Clamp(offsetRoad, -6.0f, 6.0f);
            CalculRoad();
        }
        #endregion

        #region InitChangeRoad
        private void InitChangeRoad()
        {
            offsetRoad = Random.Range(-1.5f, 1.5f);
            changeRoad = true;
        }
        #endregion

        #region ChangeRoad
        private void ChangeRoad()
        {
            if (changeRoad)
            {
                CalculRoad();
                changeRoad = false;
                offsetRoad = 0.0f;
            }
        }
        #endregion

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "StartTrigger")
            {
                InitChangeRoad();
                ChangeRoad();
            }
        }
    }
}