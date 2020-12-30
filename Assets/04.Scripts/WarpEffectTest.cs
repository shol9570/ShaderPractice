﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarpEffectTest : MonoBehaviour
{
    public Material m_WarpEffectMaterial;
    public GameObject m_WarpTarget;
    public Vector3 m_WarpDestination;
    
    void Start()
    {
        
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(1))
        {
            m_WarpDestination = Camera.main.ScreenToWorldPoint(Input.mousePosition) + Camera.main.transform.forward * 2f;
        }
    }

    IEnumerator WarpEffect()
    {
        GameObject effect = CreateWarpBound(m_WarpTarget, m_WarpDestination);
        Destroy(effect, 1.1f);
        Material warp = effect.GetComponent<MeshRenderer>().material;
        yield return null;
        float startTime = Time.time;
        float timeDiff = 0;
        while (timeDiff < 1)
        {
            timeDiff = Time.time - startTime > 1 ? 1f : Time.time - startTime;
            warp.SetFloat("_CenterDistort", Mathf.Sin(Mathf.Deg2Rad * 180f * timeDiff) * 0.9f);
            yield return null;
        }
        warp.SetFloat("_CenterDistort", 0f);
        yield return null;
    }
    
    GameObject CreateWarpBound(GameObject _target, Vector3 _destination)
    {
        //Warp effect plane mesh data
        Mesh warpPlane = new Mesh();
        Vector3[] vertices = new Vector3[5];
        int[] tris = new int[12];
        Vector2[] uvs = new Vector2[5];

        //Basic datas to create effect
        MeshRenderer[] targetMeshRenderers = _target.GetComponentsInChildren<MeshRenderer>();
        SkinnedMeshRenderer[] targetSkinnedMeshRenderers = _target.GetComponentsInChildren<SkinnedMeshRenderer>();
        Bounds[] bounds = new Bounds[targetMeshRenderers.Length + targetSkinnedMeshRenderers.Length];
        Vector3 targetCenter = Vector3.zero;
        float objMaxLength = 0f;
        for (int i = 0; i < targetMeshRenderers.Length; i++)
        {
            bounds[i] = targetMeshRenderers[i].bounds;
            targetCenter += bounds[i].center;
            if (bounds[i].max.magnitude > objMaxLength) objMaxLength = bounds[i].max.magnitude;
        }
        for (int i = 0; i < targetSkinnedMeshRenderers.Length; i++)
        {
            int index = i + targetMeshRenderers.Length;
            bounds[index] = targetSkinnedMeshRenderers[i].bounds;
            targetCenter += bounds[index].center;
            if (bounds[index].max.magnitude > objMaxLength) objMaxLength = bounds[index].max.magnitude;
        }
        targetCenter /= bounds.Length;
        Vector3 obj2Cam = Camera.main.transform.position - targetCenter;
        float obj2CamDiff = obj2Cam.magnitude - 0.1f;
        obj2Cam.Normalize();
        Vector3 right = Camera.main.transform.right;
        Vector3 up = Camera.main.transform.up;
        Vector3 nearest2Obj = targetCenter + obj2Cam * (objMaxLength > obj2CamDiff ? obj2CamDiff : objMaxLength);

        //Convert world destination point on effect plane position
        Vector3 cameraLocalPlaneCenter = Camera.main.transform.InverseTransformPoint(nearest2Obj);
        Vector3 cameraLocalDestination = Camera.main.transform.InverseTransformPoint(_destination);
        Vector3 destOnEffectPlane = Camera.main.transform.TransformPoint(new Vector3(cameraLocalDestination.x, cameraLocalDestination.y, cameraLocalPlaneCenter.z));
        Vector3 center2DestOnPlane = destOnEffectPlane - nearest2Obj;
        float center2DestOnPlaneDiff = center2DestOnPlane.magnitude;
        center2DestOnPlane.Normalize();

        //Warp effect plane mesh data initialize
        Vector3 center = center2DestOnPlaneDiff > objMaxLength * 0.5f ? nearest2Obj + center2DestOnPlane * objMaxLength * 0.5f : nearest2Obj + center2DestOnPlane * center2DestOnPlaneDiff;
        Vector3 leftBottom = nearest2Obj - right * objMaxLength - up * objMaxLength;
        Vector3 leftUpper = nearest2Obj - right * objMaxLength + up * objMaxLength;
        Vector3 rightBottom = nearest2Obj + right * objMaxLength - up * objMaxLength;
        Vector3 rightUpper = nearest2Obj + right * objMaxLength + up * objMaxLength;

        vertices[0] = center - nearest2Obj;
        vertices[1] = leftBottom - nearest2Obj;
        vertices[2] = leftUpper - nearest2Obj;
        vertices[3] = rightBottom - nearest2Obj;
        vertices[4] = rightUpper - nearest2Obj;
        print(vertices[0]);

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

        warpPlane.vertices = vertices;
        warpPlane.triangles = tris;
        warpPlane.uv = uvs;
        warpPlane.RecalculateNormals();
        warpPlane.RecalculateTangents();

        //Create object
        GameObject warpEffect = new GameObject("WarpEffect");
        warpEffect.transform.position = nearest2Obj;
        warpEffect.transform.SetParent(_target.transform);

        //attach material
        MeshFilter meshFilter = warpEffect.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = warpEffect.AddComponent<MeshRenderer>();
        meshFilter.mesh = warpPlane;
        meshRenderer.material = m_WarpEffectMaterial;

        meshRenderer.material.SetVector("_WarpFocus", new Vector2((vertices[0].x + objMaxLength) / (2 * objMaxLength), (vertices[0].y + objMaxLength) / (2 * objMaxLength)));//new Vector2(0.5f, 0.5f));

        //attach script
        warpEffect.AddComponent<WarpEffect>();

        return warpEffect;
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
