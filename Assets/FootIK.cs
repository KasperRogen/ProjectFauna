using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootIK : MonoBehaviour
{

    public float FootHeight = 0.25f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Update()
    {
        if(transform.localPosition.y > 0.01f)
        {
            transform.localPosition += Vector3.down * Time.deltaTime;
        } else
        {
            transform.localPosition += Vector3.up * Time.deltaTime;
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        float dist;
        RaycastHit hit;

        if (Physics.Raycast(transform.position, Vector3.down, out hit)){

            Debug.DrawLine(transform.position, hit.point, Color.yellow);
            dist = transform.position.y - hit.point.y;
            Vector3 CurrPos = transform.position;
            CurrPos.y -= dist - FootHeight;
            transform.position = CurrPos;
        }
    }
}
