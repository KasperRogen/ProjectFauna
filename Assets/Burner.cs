using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Burner : MonoBehaviour
{

    public GameObject ToProduce;
    public Vector3 ProductionPoint;


    private void OnTriggerEnter(Collider other)
    {
        Destroy(other.gameObject);

        Instantiate(ToProduce, transform.position + ProductionPoint, Quaternion.identity);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(transform.position + ProductionPoint, Vector3.one * 0.25f);
    }
}
