using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookObjectRotationCamera : MonoBehaviour
{
    public Transform m_Target;
    public float m_RotateCycle = 10f;
    public Vector3 m_RotateDistance = new Vector3(0f, 1f, 10f);

    private float cycle = 0f;

    void Update()
    {
        Camera cam = Camera.main;
        cycle += Mathf.PI * 2 * Time.deltaTime / m_RotateCycle;
        Vector3 targetPos = new Vector3(
            m_Target.position.x + Mathf.Sin(cycle) * m_RotateDistance.z,
            m_Target.position.y,
            m_Target.position.z + Mathf.Cos(cycle) * m_RotateDistance.z
            );
        cam.transform.position = targetPos;
        cam.transform.LookAt(m_Target);
        cam.transform.localPosition += new Vector3(
            m_RotateDistance.x,
            m_RotateDistance.y,
            0f
            );
    }
}
