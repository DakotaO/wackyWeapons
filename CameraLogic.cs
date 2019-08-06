using UnityEngine;
using System.Collections.Generic;

public class CameraLogic : MonoBehaviour {

    //Variables
    private Transform m_currentTarget;
    private float m_distance = 2f;
    private float m_height = 1;
    private float m_lookAtAroundAngle = 180;
    [SerializeField] private List<Transform> m_targets;
    private int m_currentIndex;

    /// <summary>
    /// start camera function
    /// </summary>
	private void Start () {
        if(m_targets.Count > 0)
        {
            m_currentIndex = 0;
            m_currentTarget = m_targets[m_currentIndex];
        }
	}

    /// <summary>
    /// switch the view
    /// </summary>
    private void SwitchTarget(int step)
    {
        if(m_targets.Count == 0) { return; }
        m_currentIndex+=step;
        if (m_currentIndex > m_targets.Count-1) { m_currentIndex = 0; }
        if (m_currentIndex < 0) { m_currentIndex = m_targets.Count - 1; }
        m_currentTarget = m_targets[m_currentIndex];
    }

    /// <summary>
    /// next target function
    /// </summary>
    public void NextTarget() { SwitchTarget(1); }

    /// <summary>
    /// previous target function
    /// </summary>
    public void PreviousTarget() { SwitchTarget(-1); }

    /// <summary>
    /// Update function
    /// </summary>
    private void Update () {
        if (m_targets.Count == 0) { return; }
    }

    /// <summary>
    /// Update camera angle to rotate as intended
    /// </summary>
    private void LateUpdate()
    {
        if(m_currentTarget == null) { return; }

        float targetHeight = m_currentTarget.position.y + m_height;
        float currentRotationAngle = m_lookAtAroundAngle;

        Quaternion currentRotation = Quaternion.Euler(0, currentRotationAngle, 0);

        Vector3 position = m_currentTarget.position;
        position -= currentRotation * Vector3.forward * m_distance;
        position.y = targetHeight;

        transform.position = position;
        transform.LookAt(m_currentTarget.position + new Vector3(0, m_height, 0));
    }
}
