using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Linq;

public class Countdown : MonoBehaviour
{
    SoundManagerFMOD manager;
    [SerializeField] Text countdownText;

    private void Awake()
    {
        manager = SoundManagerFMOD.GetInstance();
    }

    private void Start()
    {
        StartCoroutine("CountDown");
    }

    IEnumerator CountDown()
    {
        int stepTimer = 3;
        float interTime = 1;
        countdownText.gameObject.SetActive(true);
        
        manager.PlayRaceLaunch(GameObject.Find("StartingBlocks").transform);

        while (stepTimer > -1)
        {
            interTime -= Time.deltaTime;
            if (interTime <= 0)
            {
                interTime += 1;
                stepTimer--;
            }
            if (stepTimer == 0)
            {
                countdownText.text = "GO!";
                FindObjectsOfType<UserControllerRewired>().ToList().ForEach(x => x.RaceRunning = true);
                FindObjectsOfType<AI.AIMovement>().ToList().ForEach(x => x.followRoad = true);
            }
            else if (stepTimer < 0)
            {
                countdownText.gameObject.SetActive(false);
                manager.startRace = true;
            }
            else countdownText.text = stepTimer.ToString();
            countdownText.color = new Color(countdownText.color.r, countdownText.color.g, countdownText.color.b, interTime / 1);
            yield return null;
        }
        countdownText.gameObject.SetActive(false);
        yield return null;
    }
}
