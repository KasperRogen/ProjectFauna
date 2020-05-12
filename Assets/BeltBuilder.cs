using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BeltBuilder : MonoBehaviour
{
    class BeltPoint
    {
        public GameObject Belt;
        public float AllowedAngle = 45f;
        public BeltPoint OtherPoint;
        public ProceduralBelt ProcBeltScript;
        Vector3 _pos;
        Vector3 _axis;
        public Vector3 pos { get => _pos; set { if (PosChangeAllowed(value)) _pos = value; } }
        public Vector3 axis { get => _axis; set { if (AxisLocked == false) _axis = value; } }
        Vector3 LocalAxis;
        BeltConnector _connector;
        public BeltConnector connector { get => _connector; set { if (ConnectorLocked == false) _connector = value; } }
        public int index;
        public bool PosLocked = false;
        public bool AxisLocked = false;
        public bool ConnectorLocked = false;
        public BeltPoint(int _index)
        {
            index = _index;
        }

        bool PosChangeAllowed(Vector3 pos)
        {

            Vector3 BeltDir = index == 0 ? OtherPoint.pos - pos : pos - OtherPoint.pos;

            if (connector != null && Vector3.Angle(connector.transform.forward, BeltDir) > AllowedAngle ||
               OtherPoint.connector != null && Vector3.Angle(OtherPoint.connector.transform.forward, BeltDir) > AllowedAngle ||
               PosLocked == true)
            {
                return false;
            }

            return true;
        }

        public void UpdateMesh()
        {
            LocalAxis = Belt.transform.InverseTransformDirection(axis);
            SetMeshPoint(pos, LocalAxis);
        }

        public void SetMeshPoint(Vector3 pos, Vector3 axis)
        {
            ProcBeltScript.SetPoint(index, pos, axis);
        }
    }

    public float AllowedAngle = 45f;
    public float BeltHeight = 0.1f;
    public Material BeltMat;
    public GameObject PlaceHolderPrefab;
    GameObject PlaceHolder;
    bool _showingPlaceholder = false;
    public GameObject BeltPrefab;
    public GameObject ConnectorPrefab;
    GameObject Belt;
    GameObject BeltGO;
    Vector3 dir;
    public Vector3 BeltDefaultOrientation = new Vector3(0, 0, -1);
    float dist;
    Vector3 rotAxisV;
    public LayerMask GroundMask;
    public LayerMask GroundAndConnectorMask;
    bool _startHookedToConnector = false;

    bool _endHookedToConnector = false;

    private bool _isBuilding = false;

    public PhysicMaterial PhysMat;
    BeltPoint StartPoint;
    BeltPoint EndPoint;

    private void Awake()
    {
        CreateStartPoints();
    }

    void CreateStartPoints()
    {
        StartPoint = new BeltPoint(0);
        EndPoint = new BeltPoint(1);

        StartPoint.OtherPoint = EndPoint;
        EndPoint.OtherPoint = StartPoint;

        StartPoint.AllowedAngle = AllowedAngle;
        EndPoint.AllowedAngle = AllowedAngle;
    }

    // Update is called once per frame
    void Update()
    {

        if (_isBuilding == false)
        {
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 10, GroundAndConnectorMask, QueryTriggerInteraction.Collide))
            {
                BeltConnector connector = hit.transform.GetComponent<BeltConnector>();

                if (_showingPlaceholder == false)
                {
                    _showingPlaceholder = true;
                    PlaceHolder = Instantiate(PlaceHolderPrefab, Vector3.zero, Quaternion.identity);
                }

                if (connector?.IsConnected == false)
                {
                    PlaceHolder.transform.position = connector.Pos;
                    PlaceHolder.transform.rotation = Quaternion.Euler(PlaceHolder.transform.InverseTransformDirection(connector.Axis));
                }
                else
                {
                    PlaceHolder.transform.position = hit.point;
                    PlaceHolder.transform.LookAt(transform.position);
                }
            }
            else
            {
                if (_showingPlaceholder)
                {
                    Destroy(PlaceHolder);
                    _showingPlaceholder = false;
                }
            }
        }


        if (Input.GetMouseButtonDown(0) && _isBuilding)
        {
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
            BoxCollider collider = Belt.AddComponent<BoxCollider>();
            collider.material = PhysMat;
            //Add Trigger
            BoxCollider trigger = Belt.AddComponent<BoxCollider>();
            float triggerHeight = 0.1f;
            trigger.center = new Vector3(0, collider.size.y + triggerHeight / 2, collider.size.z / 2);
            trigger.size = new Vector3(0.15f, triggerHeight, collider.size.z);
            trigger.isTrigger = true;


            Vector3 BeltDir = EndPoint.pos - StartPoint.pos;
            StartPoint.axis = -Vector3.Cross(BeltDir, Vector3.up).normalized;
            EndPoint.axis = -Vector3.Cross(BeltDir, Vector3.up).normalized;

            GameObject FrontConnector = Instantiate(ConnectorPrefab, Belt.transform.position - Belt.transform.forward * 0.5f, Belt.transform.rotation, Belt.transform);
            FrontConnector.GetComponent<BeltConnector>().Axis = StartPoint.axis;
            FrontConnector.GetComponent<BeltConnector>().Pos = StartPoint.pos;
            FrontConnector.GetComponent<BeltConnector>().Type = ConnectorType.INPUT;
            FrontConnector.GetComponent<BeltConnector>().IsConnected = StartPoint.connector == null ? false : true;



            BeltConnector BackBeltConnector = null;

            GameObject BackConnector = Instantiate(ConnectorPrefab, Belt.transform.position +
                                                               Belt.transform.forward * 0.5f +
                                                               Belt.transform.forward * collider.size.z, Belt.transform.rotation, Belt.transform);

            BackBeltConnector = BackConnector.GetComponent<BeltConnector>();
            BackBeltConnector.Axis = EndPoint.axis;
            BackBeltConnector.Pos = EndPoint.pos;
            BackBeltConnector.Type = ConnectorType.OUTPUT;
            BackBeltConnector.IsConnected = EndPoint.connector == null ? false : true;

            if (StartPoint.connector != null)
                StartPoint.connector.IsConnected = true;
            if (EndPoint.connector != null)
                EndPoint.connector.IsConnected = true;


            Belt.GetComponent<MeshRenderer>().sharedMaterial = BeltMat;

            if (EndPoint.connector != null && StartPoint.connector != null)
            {
                stopBuild();
                CreateStartPoints();
                return;
            }
            
            StartPoint.connector = null;
            EndPoint.connector = null;






            BeltPoint LastEndPoint = EndPoint;
            CreateStartPoints();
            StartPoint.pos = LastEndPoint.pos;
            StartPoint.axis = LastEndPoint.axis;
            StartPoint.connector = BackBeltConnector;
            StartPoint.PosLocked = true;
            StartPoint.AxisLocked = true;
            StartPoint.ConnectorLocked = true;

        }



        if (Input.GetMouseButtonDown(0))
        {
            if (_showingPlaceholder)
            {
                Destroy(PlaceHolder);
                _showingPlaceholder = false;
            }

            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
            CreateBelt();

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 10, GroundAndConnectorMask, QueryTriggerInteraction.Collide))
            {
                if (hit.transform.GetComponent<BeltConnector>() != null && hit.transform.GetComponent<BeltConnector>().IsConnected == false)
                {
                    BeltConnector connector = hit.transform.GetComponent<BeltConnector>();

                    BeltPoint tempPoint = connector.Type == ConnectorType.OUTPUT ? StartPoint : EndPoint;

                    tempPoint.pos = connector.Pos;
                    tempPoint.connector = connector;
                    tempPoint.axis = tempPoint.connector.Axis;
                    tempPoint.PosLocked = true;
                    tempPoint.AxisLocked = true;
                    tempPoint.ConnectorLocked = true;

                }
                else
                {
                    StartPoint.pos = hit.point;
                    StartPoint.axis = Belt.transform.TransformDirection(Vector3.right);
                    StartPoint.connector = null;
                    StartPoint.PosLocked = true;
                }

                _isBuilding = true;
            }
        }

        if (_isBuilding)
        {
            RaycastHit groundConnectorHit;
            RaycastHit groundHit;
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
            Physics.Raycast(ray, out groundHit, 10, GroundMask);

            if (Physics.Raycast(ray, out groundConnectorHit, 10, GroundAndConnectorMask))
            {
                BeltConnector connector = groundConnectorHit.transform.GetComponent<BeltConnector>();

                BeltPoint tempPoint = EndPoint.connector != null && EndPoint.connector.Type == ConnectorType.INPUT ? StartPoint : EndPoint;


                if (connector != null && connector != tempPoint.OtherPoint.connector && connector.IsConnected == false)
                {
                    tempPoint.pos = connector.Pos;
                    tempPoint.connector = groundConnectorHit.transform.GetComponent<BeltConnector>();
                    tempPoint.axis = tempPoint.connector.Axis;
                }
                else
                {
                    tempPoint.pos = groundHit.point;
                    Vector3 BeltDir = EndPoint.pos - tempPoint.pos;
                    tempPoint.connector = null;
                    if (EndPoint.connector?.Type == ConnectorType.INPUT)
                    {
                        Belt.transform.position = EndPoint.pos - Belt.transform.forward * BeltDir.magnitude;
                    }
                    else
                    {
                        Belt.transform.position = StartPoint.pos;
                    }
                    tempPoint.axis = Belt.transform.TransformDirection(Vector3.right);
                }



                if (StartPoint.connector != null)
                {
                    Belt.transform.position = StartPoint.connector.Pos;
                    StartPoint.pos = StartPoint.connector.Pos;
                    StartPoint.axis = StartPoint.connector.Axis;
                }
                else
                {
                    Belt.transform.position = StartPoint.pos;
                    StartPoint.axis = Belt.transform.TransformDirection(Vector3.right);
                }

                if (EndPoint.connector != null && EndPoint.connector.Type == ConnectorType.INPUT)
                {
                    EndPoint.pos = EndPoint.connector.Pos;
                    EndPoint.axis = EndPoint.connector.Axis;
                }

                Debug.DrawLine(Vector3.zero, StartPoint.pos);
                Debug.DrawLine(Vector3.zero, EndPoint.pos);

                Belt.transform.LookAt(EndPoint.pos);

                StartPoint.UpdateMesh();
                EndPoint.UpdateMesh();
            }


        }

        if (Input.GetMouseButtonDown(1) && _isBuilding)
        {
            stopBuild();
            Destroy(Belt);
        }


        void stopBuild()
        {
            CreateStartPoints();
            _isBuilding = false;
        }


    }

    private void CreateBelt()
    {
        Belt = Instantiate(BeltPrefab);
        Belt.GetComponent<Renderer>().material = BeltMat;
        ProceduralBelt BeltScript = Belt.GetComponent<ProceduralBelt>();
        BeltScript.height = BeltHeight;
        StartPoint.ProcBeltScript = BeltScript;
        EndPoint.ProcBeltScript = BeltScript;
        StartPoint.Belt = Belt;
        EndPoint.Belt = Belt;
        Selection.activeGameObject = Belt;
    }
}
