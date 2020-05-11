using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BeltBuilder : MonoBehaviour
{

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
            Belt = Instantiate(BeltPrefab);
            Belt.GetComponent<Renderer>().material = BeltMat;
            BeltScript = Belt.GetComponent<ProceduralBelt>();
            Selection.activeGameObject = Belt;
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 10, GroundAndConnectorMask, QueryTriggerInteraction.Collide))
            {
                Belt.transform.position = hit.point;
                if (hit.transform.GetComponent<BeltConnector>() != null)
                {
                    BeltConnector connector = hit.transform.GetComponent<BeltConnector>();
                    if(connector.Type == ConnectorType.OUTPUT)
                    {
                        BeltStartVector = connector.Pos;
                        _startHookedToConnector = true;
                        startHookedConnector = hit.transform.GetComponent<BeltConnector>();
                        BeltScript.SetPoint(0, startHookedConnector.Pos, startHookedConnector.Axis);

                    } else
                    {
                        BeltEndVector = connector.Pos;
                        _endHookedToConnector = true;
                        endHookedConnector = hit.transform.GetComponent<BeltConnector>();
                        BeltScript.SetPoint(1, endHookedConnector.Pos, endHookedConnector.Axis);
                    }
                    

                }
                else
                {
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
                BeltEndVector = hit.point;
                Vector3 BeltDir = BeltEndVector - BeltStartVector;
                
                if(_startHookedToConnector && startHookedConnector.Type == ConnectorType.INPUT)
                {

                }


                if(hit.transform.GetComponent<BeltConnector>() != null)
                {
                    BeltConnector connector = hit.transform.GetComponent<BeltConnector>();
                    BeltEndVector = connector.Pos;
                    _endHookedToConnector = true;
                    endHookedConnector = hit.transform.GetComponent<BeltConnector>();
                    BeltScript.SetPoint(1, endHookedConnector.Pos, endHookedConnector.Axis);
                } else
                {
                    _endHookedToConnector = false;
                    endHookedConnector = null;
                    BeltScript.SetPoint(1, BeltStartVector + Belt.transform.forward * BeltDir.magnitude, Vector3.right);
                }




                if (_startHookedToConnector)
                {
                    Debug.DrawLine(transform.position, startHookedConnector.Pos, Color.red);
                    BeltScript.SetPoint(0, startHookedConnector.Pos, Belt.transform.InverseTransformDirection(startHookedConnector.Axis));
                } else
                {
                    BeltScript.SetPoint(0, BeltStartVector, Vector3.right);
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

}
