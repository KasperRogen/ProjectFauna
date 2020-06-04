using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ProceduralBelt : MonoBehaviour
{
    public float width = 1;
    public float height = 2;
    int[] triIndexes = new int[] {
        0,4,1, //TopLeft
        1,4,5, //TopRight
        6,2,3, //BottomRight
        3,7,6, //BottomLeft
        0,2,4, //LeftRight
        4,2,6, //LeftLeft
        1,5,3, //RightLeft
        3,5,7, //RightRight
        0,1,2, //FrontLeft
        2,1,3, //FrontRight
        5,4,7, //BackLeft
        7,4,6  //BackRight
    };
    
    //List<Vector3> vertices = new List<Vector3>();
    Vector3[] vertices = new Vector3[8];
    List<int> tris = new List<int>();
    Mesh mesh;
    MeshFilter meshFilter;

    public void SetPoint(int index, Vector3 pos, Vector3 axis)
    {
        index *= 4;

        Debug.DrawRay(pos - transform.TransformDirection(axis) , transform.TransformDirection(axis) * 2, Color.yellow);

        if (index == 0)
        { 
            pos -= transform.position;
        } else
        {
            pos -= transform.position;
            pos = Vector3.forward * Vector3.Distance(transform.position, transform.position + pos);
        }
        


        pos += Vector3.up * height;
        vertices[index] = pos - axis * (width / 2);
        vertices[index + 1] = pos + axis * (width / 2);
        pos -= Vector3.up * height;
        vertices[index + 2] = pos - axis * (width / 2);
        vertices[index + 3] = pos + axis * (width / 2);

        CalculateTris();
    }

    private void CalculateTris()
    {
        tris.Clear();
        for (int i = 0; i < vertices.Length / 4; i += 4)
        {
            foreach(int index in triIndexes)
            {
                tris.Add(index);
            }
        }

        //transform.forward = ((vertices[4] + vertices[5]) / 2) - ((vertices[0] + vertices[1]) / 2);

        mesh.vertices = vertices;
        mesh.triangles = tris.ToArray();
        mesh.RecalculateNormals();
        CalculateUVs();
        meshFilter.mesh = mesh;
        mesh.RecalculateBounds();
    }

    private void CalculateUVs()
    {
        //List<Vector2> UVs = new List<Vector2>();
        //for (var i = 0; i < vertices.Count; i++)
        //{
        //    UVs.Add(new Vector2(vertices[i].x, vertices[i].y));
        //}

        Vector2[] UVs = new Vector2[vertices.Length];
        UVs[1] = new Vector2(1, 0);
        UVs[4] = new Vector2(0, 1);
        UVs[5] = new Vector2(1, 1);
        mesh.uv = UVs;
    }

    public void Awake()
    {
        meshFilter = gameObject.GetComponent<MeshFilter>();
        MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
        mesh = new Mesh();
    }

    void OnDrawGizmos()
    {
        for(int i = 0; i < vertices.Length; i++)
        {
            drawString(i.ToString(), transform.position + vertices[i], Color.white);
        }
    }

    static void drawString(string text, Vector3 worldPos, Color? colour = null)
    {
        UnityEditor.Handles.BeginGUI();
        if (colour.HasValue) GUI.color = colour.Value;
        var view = UnityEditor.SceneView.currentDrawingSceneView;
        Vector3 screenPos = view.camera.WorldToScreenPoint(worldPos);

        if (screenPos.y < 0 || screenPos.y > Screen.height || screenPos.x < 0 || screenPos.x > Screen.width || screenPos.z < 0)
        {
            UnityEditor.Handles.EndGUI();
            return;
        }

        Vector2 size = GUI.skin.label.CalcSize(new GUIContent(text));
        GUI.Label(new Rect(screenPos.x - (size.x / 2), -screenPos.y + view.position.height + 4, size.x, size.y), text);
        UnityEditor.Handles.EndGUI();
    }

}
