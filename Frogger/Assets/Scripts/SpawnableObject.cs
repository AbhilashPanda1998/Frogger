using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SpawnableObject : MonoBehaviour
{
    #region Variables
    protected float m_Speed;
    protected LaneController m_AssignedLane;
    protected Vector3 m_StartPos;

    public static Action<LaneController> ReachedCheckPoint;
    #endregion

    #region Unity Callbacks
    public virtual void Update()
    {
        transform.position += transform.right * Time.deltaTime * m_Speed;
    }

    public virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "CheckPoint")
        {
            gameObject.SetActive(false);
            transform.position = m_StartPos;
            ReachedCheckPoint(m_AssignedLane);
        }
    }
    #endregion

    #region Class Functions
    public virtual void Initialize(float speed, LaneController assignedLane)
    {
        m_StartPos = transform.position;
        m_Speed = speed;
        m_AssignedLane = assignedLane;
    }
    #endregion

}
