using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RewardUI : MonoBehaviour
{
    [SerializeField] Text rewardText;
    Part reward = null;
    RewardManager rewardManager;

    private void Start()
    {
        rewardManager = FindObjectOfType<RewardManager>();
        SetRewardText();
    }

    private void Update()
    {
        if (reward == null)
            SetRewardText();
    }

    private void SetRewardText()
    {
        reward = rewardManager.reward;
        if (reward != null)
            rewardText.text = "You've Unlock " + reward.GetComponent<Part>().Name;
        else
            rewardText.text = "You've Nothing More to Unlock.";
    }
}
