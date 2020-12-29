using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarpEffectTest : MonoBehaviour
{
    private Material warp;
    public GameObject m_WarpTarget;
    
    void Start()
    {
        warp = this.GetComponent<MeshRenderer>().material;
    }

    IEnumerator WarpEffect()
    {
        CreateWarpBound(m_WarpTarget);
        float startTime = Time.time;
        float timeDiff = 0;
        while (timeDiff < 1)
        {
            timeDiff = Time.time - startTime > 1 ? 1f : Time.time - startTime;
            warp.SetFloat("_CenterDistort", Mathf.Sin(Mathf.Deg2Rad * 180f * timeDiff) * 0.9f);
            yield return null;
        }
        warp.SetFloat("_CenterDistort", 0f);
    }
    
    void CreateWarpBound(GameObject _target, Vector3 _destination)
    {
        //Warp effect plane mesh data
        Mesh warpBox;
        Vector3[] vertices = new Vector3[5];
        int[] tris = new int[12];
        Vector2[] uvs = new Vector2[5];

        //Basic datas to create effect
        Bounds bounds = _target.GetComponent<MeshRenderer>().bounds;
        Vector3 obj2Cam = _target.transform.position - Camera.main.transform.position;
        obj2Cam.Normalize();
        Vector3 right = Camera.main.transform.right;
        Vector3 up = Camera.main.transform.up;
        float objMaxLength = bounds.max.magnitude;
        Vector3 nearest2Obj = obj2Cam * objMaxLength;

        //Convert world destination point on effect plane position
        Vector3 cameraLocalPlaneCenter = Camera.main.transform.InverseTransformPoint(nearest2Obj);
        Vector3 cameraLocalDestination = Camera.main.transform.InverseTransformPoint(_destination);
        Vector3 destOnEffectPlane = Camera.main.transform.TransformPoint(new Vector3(cameraLocalDestination.x, cameraLocalDestination.y, cameraLocalPlaneCenter.z));
        Vector3 center2DestOnPlane = destOnEffectPlane - nearest2Obj;
        float center2DestOnPlaneDiff = center2DestOnPlane.magnitude;
        center2DestOnPlane.Normalize();

        //Warp effect plane mesh data initialize
        Vector3 center = center2DestOnPlaneDiff > objMaxLength ? center2DestOnPlane * objMaxLength : destOnEffectPlane;
        Vector3 leftBottom = nearest2Obj - right * objMaxLength - up * objMaxLength;
        Vector3 leftUpper = nearest2Obj - right * objMaxLength + up * objMaxLength;
        Vector3 rightBottom = nearest2Obj + right * objMaxLength - up * objMaxLength;
        Vector3 rightUpper = nearest2Obj + right * objMaxLength + up * objMaxLength;

        vertices[0] = nearest2Obj;
        vertices[1] = leftBottom;
        vertices[2] = leftUpper;
        vertices[3] = rightBottom;
        vertices[4] = rightUpper;

        tris[0] = 0;
        tris[1] = 2;
        tris[2] = 1;
        tris[3] = 0;
        tris[4] = 1;
        tris[5] = 3;
        tris[6] = 0;
        tris[7] = 3;
        tris[8] = 4;
        tris[9] = 0;
        tris[10] = 4;
        tris[11] = 2;

        uvs[0] = new Vector2(0.5f, 0.5f);
        uvs[1] = new Vector2(0f, 0f);
        uvs[2] = new Vector2(0f, 1f);
        uvs[3] = new Vector2(1f, 0f);
        uvs[4] = new Vector2(1f, 1f);

        //Create object

        //attach material

        //attach script

        
    }

    private void OnGUI()
    {
        if (GUI.Button(new Rect(new Vector2(0,0), new Vector2(100,50)), "Play"))
        {
            StopAllCoroutines();
            StartCoroutine(WarpEffect());
        }
    }
}
