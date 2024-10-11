using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GhostController : MonoBehaviour
{
    Ghost ghost;

    void Start()
    {
        ////ghost = DatasSavingManager.Ghosts[g];
        //Debug.Log(ghost.timerOffsetSave);
        //StartCoroutine("MoveToDestination");
    }

    IEnumerator MoveToDestination()
    {
        List<float[]> poss = ghost.positions;
        List<float[]> rots = ghost.quaternions;
        Vector3 currentPos = new Vector3(poss[0][0], poss[0][1], poss[0][2]);
        Quaternion currentRot = new Quaternion(rots[0][0], rots[0][1], rots[0][2], rots[0][3]);
        for (int i = 0; i < poss.Count - 1; i++)
        {
            float t = 0f;
            Vector3 nextPos = new Vector3(poss[i + 1][0], poss[i + 1][1], poss[i + 1][2]);
            Quaternion nextRot = new Quaternion(rots[i + 1][0], rots[i + 1][1], rots[i + 1][2], rots[i + 1][3]);
            
            while (t / ghost.timerOffsetSave <= 1)
            {
                t += Time.deltaTime;
                transform.position = Vector3.Lerp(currentPos, nextPos, t / ghost.timerOffsetSave);
                transform.rotation = Quaternion.Lerp(currentRot, nextRot, t / ghost.timerOffsetSave);
                yield return null;
            }
            transform.SetPositionAndRotation(nextPos, nextRot);
            currentPos = nextPos;
            currentRot = nextRot;
            yield return null;
        }
    }

    public void SetGhost(Ghost _ghost)
    {
        ghost = _ghost;
        StartCoroutine("MoveToDestination");
    }
}
