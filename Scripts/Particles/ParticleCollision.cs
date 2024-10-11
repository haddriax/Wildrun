using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleCollision : MonoBehaviour
{
    [SerializeField] ParticleSystem particle;

    List<GameObject> go;
    Collision col;

    Rigidbody rb;
    float timer;
    bool isCollide;

    void Start()
    {
        go = new List<GameObject>();
        rb = this.GetComponent<Rigidbody>();

        timer = particle.main.startLifetime.constant;

        isCollide = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider != null && particle.isStopped)
        {
            isCollide = true;
            col = collision;
        }

    }

    private void OnCollisionExit(Collision collision)
    {
        if (particle.isPlaying)
        {
            isCollide = false;
        }
    }


    void SparksSpawn(Collision collision)
    {
        int tempSize = collision.contacts.Length;
        if (tempSize > 0)
        {
            ContactPoint contact = collision.contacts[0];
            Quaternion rotation = Quaternion.FromToRotation(Vector3.right, contact.normal);
            particle.transform.position = contact.point;

            go.Add(Instantiate(particle.gameObject, particle.transform.position, rotation));
        }
    }

    private void DestroySparks()
    {
        foreach (GameObject g in go)
        {
            Object.Destroy(g);
        }
        go.Clear();
    }

    private void Update()
    {
        if (isCollide == true)
        {
            SparksSpawn(col);
            timer -= Time.deltaTime;
            if (timer < 0)
            {
                DestroySparks();
                timer = particle.main.startLifetime.constant;
                isCollide = false;
            }
        }

    }
}
