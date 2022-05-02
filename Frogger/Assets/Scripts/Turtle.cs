using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Turtle : SpawnableObject
{
    [SerializeField] private float m_SubmergeTime;
    [SerializeField] private bool m_HasPlayer;
    [SerializeField] private Slider m_Slider;

    private float m_CurrentTime;

    private void OnEnable()
    {
        m_Slider.gameObject.SetActive(false);
        m_HasPlayer = false;
        m_CurrentTime = m_SubmergeTime;
    }

    public override void Update()
    {
        base.Update();
        if (m_HasPlayer)
        {
            if (m_CurrentTime >= 0)
            {
                m_CurrentTime -= Time.deltaTime;
                m_Slider.value = m_CurrentTime / m_SubmergeTime;
            }
            else
            {
                GameManager.instance.ReduceLife();
                m_HasPlayer = false;
            }
        }

    }

    public override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
        if (collision.tag == "Player")
        {
            m_Slider.gameObject.SetActive(true);
            m_CurrentTime = m_SubmergeTime;
            m_HasPlayer = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            m_Slider.gameObject.SetActive(false);
            m_HasPlayer = false;
        }
    }

    public override void Initialize(float speed, LaneController assignedLane)
    {
        base.Initialize(speed, assignedLane);
    }

}
