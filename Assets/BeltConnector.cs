using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeltConnector : MonoBehaviour
{
    public ConnectorType Type;
    public bool IsConnected = false;
    public bool Fixed = false;
    public GameObject ConnectedBelt = null;
    Vector3 _pos;

    [SerializeField]
    Vector3 _axis;
    public Vector3 Pos { get => Fixed ? transform.position + _pos : _pos; set => _pos = value; }

    [HideInInspector]
    public Vector3 Axis { get => Fixed ? transform.TransformDirection(_axis) : _axis; set => _axis = value; }


    private void OnDrawGizmos()
    {
        Gizmos.DrawCube(transform.position + Pos, new Vector3(1, 0.1f, 0.1f));
    }
}

public enum ConnectorType
{
    INPUT, OUTPUT
}

