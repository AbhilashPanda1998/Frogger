using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrogController : MonoBehaviour
{
    #region Variables
    private Animator m_Animator;
    [SerializeField] private float m_PerUnitMovement;
    private bool m_OnPlatform;
    private bool m_OnWater;
    #endregion


    #region Unity Callbacks
    private void Awake()
    {
        m_Animator = GetComponent<Animator>();
    }

    private void Start()
    {
        GameManager.NewFrogWave += ResetFrogParent;
    }

    private void OnDestroy()
    {
        GameManager.NewFrogWave -= ResetFrogParent;
    }

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            m_Animator.SetTrigger("Right");
            transform.position += Vector3.right * m_PerUnitMovement;
        }

        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            m_Animator.SetTrigger("Left");
            transform.position += Vector3.left * m_PerUnitMovement;
        }

        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            m_Animator.SetTrigger("Front");
            transform.position += Vector3.up * m_PerUnitMovement;
        }

        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            m_Animator.SetTrigger("Back");
            transform.position += Vector3.down * m_PerUnitMovement;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Enemy" || collision.tag == "Boundary")
        {
            GameManager.instance.ReduceLife();
        }

        if (collision.tag == "Platform")
        {
            m_OnPlatform = true;
            transform.SetParent(collision.transform);
            transform.localPosition = Vector3.zero;
        }

        if (collision.tag == "Finish")
        {
            GameManager.instance.AddScore();
             m_OnPlatform = true;
            GameManager.instance.ReduceLife();
        }

        if (collision.tag == "Water")
        {
            m_OnWater = true;
            if (m_OnWater && !m_OnPlatform)
            {
                GameManager.instance.ReduceLife();
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Platform")
        {
            if (m_OnPlatform != true)
            {
                transform.SetParent(collision.transform);
                m_OnPlatform = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {

        if (collision.tag == "Platform")
        {
            m_OnPlatform = false;
            CheckForWater();
            if (gameObject.activeInHierarchy)
                transform.SetParent(null);
        }

        if (collision.tag == "Water")
        {
            m_OnWater = false;
        }

    }

    private void ResetFrogParent()
    {
        if (transform.parent != null)
            transform.SetParent(null);
    }

    private void CheckForWater()
    {
        if (!gameObject.activeInHierarchy) return;
        StartCoroutine(CheckWater());
    }

    IEnumerator CheckWater()
    {
        yield return new WaitForSeconds(0.25f);

        if (!m_OnPlatform && m_OnWater)
        {
            GameManager.instance.ReduceLife();
        }
    }
    #endregion

}
