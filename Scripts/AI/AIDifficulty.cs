using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{
    public class AIDifficulty : MonoBehaviour
    {
        /// <Difficulty>
        [SerializeField]
        [Range(1, 3)]
        private int AILvlDifficulty = 1;
        private float AIEasyValue, AIMediumValue, AIHardValue;
        [SerializeField]
        [Range(0.1f, 1.0f)]
        private float minAIDifficulty = 0.5f;
        [SerializeField]
        [Range(0.1f, 1.0f)]
        private float maxAIDifficulty = 0.8f;
        public float MaxAIDifficulty { get => maxAIDifficulty; }
        private float currentAIDifficulty;
        public float CurrentAIDifficulty { get => currentAIDifficulty; }
        /// </Difficulty>

        #region Init
        public void Init()
        {
            // Make a ratio with min and max value and divide it by the number of difficulty
            float ratioDifficulty = (minAIDifficulty / maxAIDifficulty) / 3.0f;

            float maxAIEasyValue = minAIDifficulty + (minAIDifficulty * ratioDifficulty);
            AIEasyValue = Random.Range(minAIDifficulty, maxAIEasyValue);

            float minAIMediumValue = maxAIEasyValue + 0.001f;
            float maxAIMediumValue = maxAIEasyValue + (maxAIEasyValue * ratioDifficulty);
            AIMediumValue = Random.Range(minAIMediumValue, maxAIMediumValue);

            float minAIHardValue = maxAIMediumValue + 0.001f;
            AIHardValue = Random.Range(minAIHardValue, maxAIDifficulty);

            currentAIDifficulty = AIEasyValue;
        }
        #endregion

        #region ManageDifficulty
        public void ManageAIDifficulty()
        {
            switch (AILvlDifficulty)
            {
                case 1:
                    currentAIDifficulty = AIEasyValue;
                    break;
                case 2:
                    currentAIDifficulty = AIMediumValue;
                    break;
                case 3:
                    currentAIDifficulty = AIHardValue;
                    break;

                default:
                    currentAIDifficulty = AIEasyValue;
                    break;
            }

            currentAIDifficulty = Mathf.Clamp(currentAIDifficulty, minAIDifficulty, maxAIDifficulty);
        }
        #endregion
    }
}