using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BeltBuilder : MonoBehaviour
{
    public float BeltHeight = 0.1f;
    public Material BeltMat;

    Vector3 BeltStartVector, BeltEndVector, BeltStartAxis, BeltEndAxis;
    public GameObject BeltPrefab;
    public GameObject ConnectorPrefab;
    GameObject Belt;
    GameObject BeltGO;
    Vector3 dir;
    public Vector3 BeltDefaultOrientation = new Vector3(0, 0, -1);
    float dist;
    Vector3 rotAxisV;
    ProceduralBelt BeltScript;
    public LayerMask GroundMask;
    public LayerMask GroundAndConnectorMask;
    bool _startHookedToConnector = false;
    BeltConnector startHookedConnector;

    bool _endHookedToConnector = false;
    BeltConnector endHookedConnector;

    private bool _isBuilding = false;

    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        if (Input.GetMouseButtonDown(0))
        {
            CreateBelt();
            
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 10, GroundAndConnectorMask, QueryTriggerInteraction.Collide))
            {
                if (hit.transform.GetComponent<BeltConnector>() != null)
                {
                    BeltConnector connector = hit.transform.GetComponent<BeltConnector>();
                    if(connector.Type == ConnectorType.OUTPUT)
                    {
                        BeltStartVector = connector.Pos;
                        _startHookedToConnector = true;
                        startHookedConnector = hit.transform.GetComponent<BeltConnector>();
                        Belt.transform.position = startHookedConnector.Pos;
                        BeltScript.SetPoint(0, startHookedConnector.Pos, Belt.transform.InverseTransformDirection(startHookedConnector.Axis));

                    } else
                    {
                        BeltEndVector = connector.Pos;
                        _endHookedToConnector = true;
                        endHookedConnector = hit.transform.GetComponent<BeltConnector>();
                        BeltScript.SetPoint(1, endHookedConnector.Pos, Belt.transform.InverseTransformDirection(endHookedConnector.Axis));
                    }
                    

                }
                else
                {
                    Belt.transform.position = hit.point;
                    BeltScript.SetPoint(0, hit.point, Vector3.right);
                    BeltStartVector = hit.point;
                }

                _isBuilding = true;
            }
        }

        if (Input.GetMouseButton(0) && _isBuilding)
        {
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 10, GroundAndConnectorMask))
            {
                if(_endHookedToConnector && endHookedConnector.Type == ConnectorType.INPUT)
                {
                    BeltStartVector = hit.point;
                    Vector3 BeltDir = BeltEndVector - BeltStartVector;
                    Debug.DrawLine(BeltStartVector, BeltStartVector + Belt.transform.forward * BeltDir.magnitude, Color.magenta);

                    if (hit.transform.GetComponent<BeltConnector>() != null && hit.transform.GetComponent<BeltConnector>() != endHookedConnector)
                    {
                        BeltConnector connector = hit.transform.GetComponent<BeltConnector>();
                        BeltStartVector = connector.Pos;
                        _startHookedToConnector = true;
                        startHookedConnector = hit.transform.GetComponent<BeltConnector>();
                        Belt.transform.position = startHookedConnector.Pos;
                        BeltScript.SetPoint(0, startHookedConnector.Pos, Belt.transform.InverseTransformDirection(startHookedConnector.Axis));
                    }
                    else
                    {
                        Physics.Raycast(ray, out hit, 10, GroundMask);
                        BeltStartVector = hit.point;
                        BeltDir = BeltEndVector - BeltStartVector;
                        _startHookedToConnector = false;
                        startHookedConnector = null;
                        Belt.transform.position = BeltEndVector - Belt.transform.forward * BeltDir.magnitude;
                        BeltScript.SetPoint(0, BeltStartVector + Belt.transform.forward * BeltDir.magnitude, Vector3.right);
                    }

                } else
                {
                    BeltEndVector = hit.point;
                    Vector3 BeltDir = BeltEndVector - BeltStartVector;

                    if (hit.transform.GetComponent<BeltConnector>() != null && hit.transform.GetComponent<BeltConnector>() != startHookedConnector)
                    {
                        BeltConnector connector = hit.transform.GetComponent<BeltConnector>();
                        BeltEndVector = connector.Pos;
                        _endHookedToConnector = true;
                        endHookedConnector = hit.transform.GetComponent<BeltConnector>();
                        BeltScript.SetPoint(1, endHookedConnector.Pos, Belt.transform.InverseTransformDirection(endHookedConnector.Axis));
                    }
                    else
                    {
                        Physics.Raycast(ray, out hit, 10, GroundMask);
                        BeltEndVector = hit.point;
                        BeltDir = BeltEndVector - BeltStartVector;
                        _endHookedToConnector = false;
                        endHookedConnector = null;
                        BeltScript.SetPoint(1, BeltStartVector + Belt.transform.forward * BeltDir.magnitude, Vector3.right);
                    }

                }

                if (_startHookedToConnector)
                {
                    Debug.DrawLine(transform.position, startHookedConnector.Pos, Color.red);
                    Belt.transform.position = startHookedConnector.Pos;
                    BeltScript.SetPoint(0, startHookedConnector.Pos, Belt.transform.InverseTransformDirection(startHookedConnector.Axis));
                }
                else
                {
                    Belt.transform.position = BeltStartVector;
                    BeltScript.SetPoint(0, BeltStartVector, Vector3.right);
                }

                if(_endHookedToConnector && endHookedConnector.Type == ConnectorType.INPUT)
                {
                    BeltScript.SetPoint(1, endHookedConnector.Pos, Belt.transform.InverseTransformDirection(endHookedConnector.Axis));
                }

                Debug.DrawLine(Vector3.zero, BeltStartVector);
                Debug.DrawLine(Vector3.zero, BeltEndVector);

                Belt.transform.LookAt(BeltEndVector);
                

            }
          

        }

        if (Input.GetMouseButtonUp(0))
        {
            _isBuilding = false;
            //Add Collider
            BoxCollider collider = Belt.AddComponent<BoxCollider>();
            //Add Trigger
            BoxCollider trigger = Belt.AddComponent<BoxCollider>();
            float triggerHeight = 0.1f;
            trigger.center = new Vector3(0, collider.size.y + triggerHeight / 2, collider.size.z / 2);
            trigger.size = new Vector3(0.15f, triggerHeight, collider.size.z);
            trigger.isTrigger = true;


            Vector3 BeltDir = BeltEndVector - BeltStartVector;
            BeltStartAxis = -Vector3.Cross(BeltDir, Vector3.up).normalized;
            BeltEndAxis = -Vector3.Cross(BeltDir, Vector3.up).normalized;

            if (_startHookedToConnector == false)
            {
                GameObject FrontConnector = Instantiate(ConnectorPrefab, Belt.transform.position - Belt.transform.forward * 0.5f, Belt.transform.rotation, Belt.transform);
                FrontConnector.GetComponent<BeltConnector>().Axis = BeltStartAxis;
                FrontConnector.GetComponent<BeltConnector>().Pos = BeltStartVector;
                FrontConnector.GetComponent<BeltConnector>().Type = ConnectorType.INPUT;
            }

            if(_endHookedToConnector == false)
            {
                GameObject BackConnector = Instantiate(ConnectorPrefab, Belt.transform.position +
                                                                   Belt.transform.forward * 0.5f +
                                                                   Belt.transform.forward * collider.size.z, Belt.transform.rotation, Belt.transform);
                BackConnector.GetComponent<BeltConnector>().Axis = BeltEndAxis;
                BackConnector.GetComponent<BeltConnector>().Pos = BeltEndVector;
                BackConnector.GetComponent<BeltConnector>().Type = ConnectorType.OUTPUT;
            }


            _startHookedToConnector = false;
            startHookedConnector = null;

            _endHookedToConnector = false;
            endHookedConnector = null;

            Belt.GetComponent<MeshRenderer>().sharedMaterial = BeltMat;

        }



    }

    private void CreateBelt()
    {
        Belt = Instantiate(BeltPrefab);
        Belt.GetComponent<Renderer>().material = BeltMat;
        BeltScript = Belt.GetComponent<ProceduralBelt>();
        BeltScript.height = BeltHeight;
        Selection.activeGameObject = Belt;
    }
}
