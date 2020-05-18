using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WalkingAgent : MonoBehaviour
{
    public List<Vector3> positions;
    public float Speed;
    int index = 0;
    public float RotSpeed;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(Vector3.Distance(transform.position, positions[index]) < 1)
        {
            index = index == 0 ? 1 : 0;
        }

        if (Speed > 0f)
        {
            transform.position = Vector3.MoveTowards(transform.position, positions[index], Time.deltaTime * Speed);
        }

        if(RotSpeed > 0f)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(positions[index] - transform.position), Time.deltaTime * RotSpeed);
        }


    }


    private void OnDrawGizmos()
    {
        foreach(Vector3 pos in positions)
        {
            Gizmos.DrawSphere(pos, 0.25f);
        }
    }
}
