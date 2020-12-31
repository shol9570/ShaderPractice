using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class WarpEffect : MonoBehaviour
{
    public Vector2 m_TargetUV = new Vector2(0.5f,0.5f);
    private Material m_Mat;

    void Start()
    {
#if UNITY_EDITOR
        m_Mat = this.GetComponent<MeshRenderer>().sharedMaterial;
#else
        m_Mat = this.GetComponent<MeshRenderer>().material;
#endif
    }

    void Update()
    {
        LookCamera();
//        if (m_Mat == null) return;
//#if UNITY_EDITOR
//        Vector2 screenPos = Camera.main.WorldToScreenPoint(this.transform.position);
//        Vector2 screenUV = new Vector2(screenPos.x / Screen.width, screenPos.y / Screen.height);
//        m_Mat.SetVector("_WarpFocus", screenUV);
//#else
//        Vector2 screenPos = Camera.main.WorldToScreenPoint(this.transform.position);
//        Vector2 screenUV = new Vector2(screenPos.x / Screen.width, screenPos.y / Screen.height);
//        m_Mat.SetVector("_WarpFocus", screenUV);
//#endif
    }

    void LookCamera()
    {
        if (this.transform.parent == null) return;
        Transform parent = this.transform.parent;
        this.transform.LookAt(Camera.main.transform);

        MeshRenderer[] targetMeshRenderers = parent.GetComponentsInChildren<MeshRenderer>();
        SkinnedMeshRenderer[] targetSkinnedMeshRenderers = parent.GetComponentsInChildren<SkinnedMeshRenderer>();
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

        StartCoroutine(LookCameraCoroutine(targetCenter, objMaxLength));
    }

    IEnumerator LookCameraCoroutine(Vector3 _from, float _diff)
    {
        Transform parent = this.transform.parent;
        while (true)
        {
            Vector3 baseCoord = parent.position + _from;
            Vector3 base2Cam = Camera.main.transform.position - baseCoord;
            float base2CamDiff = (base2Cam.magnitude - Camera.main.nearClipPlane) * 0.75f;
            base2Cam.Normalize();
            this.transform.position = base2CamDiff < _diff ? baseCoord + base2Cam * base2CamDiff : baseCoord + base2Cam * _diff;
            this.transform.LookAt(Camera.main.transform);

            yield return null;
        }
    }
}
