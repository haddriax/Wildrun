using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class RewardManager : MonoBehaviour
{
    GameObject rewardGO = null;
    [HideInInspector] public Part reward = null;


    [SerializeField] public GameObject m_reward;
    [SerializeField] public GameObject m_rewardDisplay;
    [SerializeField] Text rewardName;
    public bool rewarded = false;

    SoundManagerFMOD manager;

    private void Start()
    {
        manager = SoundManagerFMOD.GetInstance();
    }
    
    private void Update()
    {
        if (rewardGO)
            rewardGO.transform.Rotate(Vector3.up, -10 * Time.deltaTime);
    }

    public void RewardPlayer()
    {
        if (!rewarded)
        {
            List<Part> lockedParts = SavedDatasManager.LockedParts;
            if (lockedParts.Count > 0)
            {
                reward = lockedParts[Random.Range(0, lockedParts.Count)];
                SavedDatasManager.SetLockState(true, reward.ID);
                rewardName.text = "You've Unlocked\n" + reward.Name;
            }
            manager.PlayGetFireworksLoot(transform);
            manager.PlayGetBoxLoot(transform);
            rewardGO = Instantiate(reward.gameObject, m_reward.transform.position, Quaternion.identity);
            m_rewardDisplay.SetActive(true);
            rewarded = true;
        }
    }
}
