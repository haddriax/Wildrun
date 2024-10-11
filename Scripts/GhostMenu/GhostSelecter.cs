using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Rewired;
using UnityEngine.UI;

public class GhostSelecter : MonoBehaviour
{
    RewiredManager rewiredManager;

    List<Ghost> ghosts = new List<Ghost>();
    Ghost currentGhost = null;
    int indiceCurrentGhost = 0;
    Player player = null;

    [SerializeField] Text id;
    [SerializeField] Text time;

    private void Start()
    {
        rewiredManager = RewiredManager.Instance;
        ghosts = SavedDatasManager.Ghosts;
        if (ghosts.Count > 0) currentGhost = ghosts.First();
        else time.text = "Pas de Ghost";
        StartCoroutine("DetectGamePad");
    }

    private void Update()
    {
        if (ghosts.Count > 0)
        {
            if (player != null)
            {
                if (player.GetButtonDown("Right"))
                    indiceCurrentGhost++;
                else if (player.GetButtonDown("Left"))
                    indiceCurrentGhost--;
            }
            int n = (indiceCurrentGhost - 1) % ghosts.Count;
            currentGhost = ghosts[(n < 0) ? ghosts.Count + n : n];

            string scd = ((int)currentGhost.totalTime % 60).ToString();
            string min = ((int)currentGhost.totalTime / 60).ToString();
            time.text = ((min.Length == 1) ? "0" : "") + min + "." + ((scd.Length == 1) ? "0" : "") + scd;
        }
    }

    IEnumerator DetectGamePad()
    {
        while (rewiredManager.GetPlayerAnyButton() == null)
        {
            yield return null;
        }
        player = rewiredManager.GetPlayerAnyButton();
    }
}
