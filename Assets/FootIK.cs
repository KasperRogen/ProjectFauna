using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootIK : MonoBehaviour
{
    public WalkingAgent WalkingAgent;
    public float FootHeight = 0.25f;
    Transform _transform;
    public FootIK _childIK;
    public bool IsRoot;
    bool hasChild = false;

    // Start is called before the first frame update
    void Start()
    {
        _transform = transform;
        if (IsRoot)
        {
            StartCoroutine(UpdateFootIKs());
        }

        if(transform.childCount > 0)
        {
            _childIK = _transform.GetChild(0).GetComponent<FootIK>();
            hasChild = true;
        }
    }

    IEnumerator UpdateFootIKs()
    {
        yield return new WaitForSeconds(1);
        bool t = true;
        while (t)
        {
            yield return new WaitForEndOfFrame();
            if(WalkingAgent.Speed > 0f)
            MoveToGround();
        }
    }

    // Update is called once per frame
    public void MoveToGround()
    {
        float dist;
        RaycastHit hit;

        if (Physics.Raycast(_transform.position, Vector3.down, out hit)){

            Debug.DrawLine(_transform.position, hit.point, Color.yellow);
            dist = _transform.position.y - hit.point.y;
            Vector3 CurrPos = _transform.position;
            CurrPos.y -= dist - FootHeight;
            _transform.position = CurrPos;
        }

        if (hasChild)
        {
                _childIK.MoveToGround();
                transform.LookAt(_childIK.transform);
        }
    }
}
