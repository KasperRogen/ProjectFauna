using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Producer : MonoBehaviour
{

    public GameObject ToProduce;
    public Vector3 SendTo;
    public Vector3 SpawnPoint;
    public float SendSpeed = 1f;
    GameObject CurrentGO = null;
    Rigidbody rb;

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject == CurrentGO)
        {
            CurrentGO = null;
            rb = null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(rb == null)
        {
            rb = CurrentGO.GetComponent<Rigidbody>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(CurrentGO == null)
        {
            CurrentGO = Instantiate(ToProduce, transform.position + SpawnPoint, Quaternion.identity);
        }

        if(rb != null)
        {
            rb.velocity = (transform.position + SendTo - rb.position) * SendSpeed;
        }
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(transform.position + SendTo, Vector3.one * 0.25f);
        Gizmos.DrawWireCube(transform.position + SpawnPoint, Vector3.one * 0.25f);
    }
}
