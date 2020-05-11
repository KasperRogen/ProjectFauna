using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeltConnector : MonoBehaviour
{
    public ConnectorType Type;
    public bool IsConnected = false;
    public GameObject ConnectedBelt = null;
    public Vector3 Pos;
    public Vector3 Axis;


    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position + Pos, 0.25f);
    }
}

public enum ConnectorType
{
    INPUT, OUTPUT
}

