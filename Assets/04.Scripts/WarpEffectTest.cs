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

    private void OnGUI()
    {
        if (GUI.Button(new Rect(new Vector2(0,0), new Vector2(100,50)), "Play"))
        {
            StopAllCoroutines();
            StartCoroutine(WarpEffect());
        }
    }
}
