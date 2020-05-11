using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeltScript : MonoBehaviour
{
    Renderer renderer;
    public float ScrollSpeed = 1f;
    Vector2 scrollValue = Vector2.zero;

    List<Rigidbody> BeltElements = new List<Rigidbody>();
    public float BeltSpeed = 1f;
    
    public float TileX, TileZ;
    public Bounds bounds;
    public float Lerpscale = 1;

    MeshFilter mf;

    // Start is called before the first frame update
    void Start()
    {
        renderer = GetComponent<Renderer>();
        mf = GetComponent<MeshFilter>();
    }

    // Update is called once per frame
    void Update()
    {
        bounds = mf.mesh.bounds;
        scrollValue.y -= ScrollSpeed * Time.deltaTime;
        scrollValue.y = scrollValue.y > 1f ? 0 : scrollValue.y;
        renderer.material.SetTextureOffset("_BaseMap", scrollValue);


        foreach(Rigidbody rb in BeltElements)
        {
            Vector3 CenterPoint = rb.transform.position;
            CenterPoint.x = transform.forward.x;

            Debug.DrawLine(Vector3.zero, transform.position + transform.forward);

            rb.velocity = Vector3.Lerp(rb.velocity, 
                (transform.forward * BeltSpeed) /*+ (CenterPoint - rb.transform.position)*/ * BeltSpeed, Lerpscale);
        }
        

        renderer.material.SetTextureScale("_BaseMap", 
            new Vector2(bounds.size.x * TileX, bounds.size.z * TileZ));
    }


    private void OnDrawGizmos()
    {
        //Gizmos.color = Color.red;
        //Gizmos.DrawCube(bounds.center, bounds.size * 10);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.GetComponent<ITransportable>() != null)
        {
            BeltElements.Add(other.transform.GetComponent<Rigidbody>());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.GetComponent<ITransportable>() != null)
        {
            BeltElements.Remove(BeltElements.Find(rb => rb == other.transform.GetComponent<Rigidbody>()));
        }
    }
}
