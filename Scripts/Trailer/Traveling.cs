using UnityEngine;
using System.Collections;

public class Traveling : MonoBehaviour
{
    [SerializeField] float TimeTraveling = -1;
    [SerializeField] Transform FinalPointTraveling;
    [SerializeField] Transform PointTravelingLookAt = null;

    private void Start()
    {
        StartCoroutine("Travel");
    }

    IEnumerator Travel()
    {
        float timer = 0;
        Transform origin;
        origin = transform;
        while (true)
        {
            if (TimeTraveling == -1)
                origin = transform;
            float lerpValue = TimeTraveling <= -1 ? 0.01f : timer / TimeTraveling;
            transform.position = Vector3.Lerp(origin.position, FinalPointTraveling.position, lerpValue);
            if (PointTravelingLookAt != null)
                transform.LookAt(PointTravelingLookAt);
            timer += Time.deltaTime;
            Debug.Log(timer);
            yield return null;
        }
    }
}
