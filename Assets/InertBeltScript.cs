using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InertBeltScript : MonoBehaviour
{
    Vector3 startPos, endPos;
    Vector3 BeltDirection;
    List<Transform> BeltItems = new List<Transform>();
    public float BeltSpeed = 1f;
    Vector3 bounds;

    [SerializeField] private float tileX = 1;
    [SerializeField] private float tileY = 1;
    private Material mat;





    // Start is called before the first frame update
    void Start()
    {
        mat = GetComponent<Renderer>().material;
        bounds = GetComponent<BoxCollider>().bounds.extents;
        startPos = endPos = transform.position;
        startPos.z += bounds.z;
        endPos.z -= bounds.z;
        BeltDirection = endPos - startPos;
    }

    // Update is called once per frame
    void Update()
    {
        BeltItems.ForEach(transform => transform.position += BeltDirection * Time.deltaTime * BeltSpeed);
        mat.SetTextureScale("_BaseMap", new Vector2(bounds.x * transform.localScale.x / 100 * tileX, bounds.y * transform.localScale.y / 100 * tileY));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<ITransportable>())
        {
            BeltItems.Add(other.transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<ITransportable>())
        {
            BeltItems.Remove(other.transform);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(startPos, 0.1f);
        Gizmos.DrawSphere(endPos, 0.1f);
    }

}
