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
        if (m_Mat == null) return;
#if UNITY_EDITOR
        Vector2 screenPos = Camera.main.WorldToScreenPoint(this.transform.position);
        Vector2 screenUV = new Vector2(screenPos.x / Screen.width, screenPos.y / Screen.height);
        m_Mat.SetVector("_WarpFocus", screenUV);
#else
        Vector2 screenPos = Camera.main.WorldToScreenPoint(this.transform.position);
        Vector2 screenUV = new Vector2(screenPos.x / Screen.width, screenPos.y / Screen.height);
        m_Mat.SetVector("_WarpFocus", screenUV);
#endif
    }
}
