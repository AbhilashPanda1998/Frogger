using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class GameManager : MonoBehaviour
{
    #region Variables
    [Space(10)]
    [Header("Total Lanes in Game -------")]
    [SerializeField] private List<LaneController> availableLanes = new List<LaneController>();
    [Space(10)]

    [Header("Player Life ------------")]
    [SerializeField] private int m_MaxLifes;
    [SerializeField] private Stack<GameObject> m_FrogLifesList = new Stack<GameObject>();
    [SerializeField] private GameObject m_Frog, m_FrogLogo;
    [SerializeField] private Transform m_FrogLogoContainer;
    [Space(10)]

    [Header("UI ---------")]
    [SerializeField] private GameObject m_GameOver, m_AllUI;
    [SerializeField] private Button m_YesBtn, m_NoBtn;
    [SerializeField] private Slider m_TimeSlider;
    [SerializeField] private float m_MaxTime;
    [SerializeField] private bool m_StartTimer;
    [SerializeField] private Text m_Score;

    private float m_CurrentTime, m_ScoreValue;
    private bool m_IsGameOver;
    private Vector3 m_FrogStartPos;

    private IEnumerator m_NewFrogLifeCoroutine;

    public static Action NewFrogWave;
    public static GameManager instance;
    #endregion

    #region Unity Callbacks
    private void Awake()
    {
        instance = this;
        SpawnObjectsInLane();
        SpawnFrogLifesLogo();
    }

    private void Start()
    {
        m_YesBtn.onClick.AddListener(RestartGame);
        m_NoBtn.onClick.AddListener(QuitGame);
        m_CurrentTime = m_MaxTime;
        m_FrogStartPos = m_Frog.transform.position;
        StartCoroutine(StartDelay());
    }

    private void OnDestroy()
    {
        m_YesBtn.onClick.RemoveListener(RestartGame);
        m_NoBtn.onClick.RemoveListener(QuitGame);
    }

    private void Update()
    {
        if (m_StartTimer)
        {
            if (m_CurrentTime >= 0)
            {
                m_CurrentTime -= Time.deltaTime;
                m_TimeSlider.value = m_CurrentTime / m_MaxTime;
            }
            else
            {
                OnTimerComplete();
                m_StartTimer = false;
            }
        }
    }
    #endregion

    #region Class Functions
    private void SpawnObjectsInLane()
    {
        for (int i = 0; i < availableLanes.Count; i++)
        {
            availableLanes[i].SpawnObjects();
        }
    }

    private void SpawnFrogLifesLogo()
    {
        for (int i = 0; i < m_MaxLifes; i++)
        {
            GameObject life = Instantiate(m_FrogLogo, m_FrogLogoContainer);
            m_FrogLifesList.Push(life);
        }
    }

    private void OnTimerComplete()
    {
        ReduceLife();
    }

    private IEnumerator NextFrogWave()
    {
        ShowFrogElements(false);

        yield return new WaitForSeconds(2);
        NewFrogWave();
        m_Frog.transform.position = m_FrogStartPos;
        m_CurrentTime = m_MaxTime;
        ShowFrogElements(true);
    }

    public void ReduceLife()
    {
        if (m_FrogLifesList.Count == 0) return;
        GameObject life = m_FrogLifesList.Pop();
        life.SetActive(false);
        m_MaxLifes--;
        if (m_MaxLifes <= 0)
        {
            ShowFrogElements(false);
            m_GameOver.SetActive(true);
            if (m_NewFrogLifeCoroutine != null)
                StopCoroutine(m_NewFrogLifeCoroutine);
        }
        else
        {
            PrepareNewCoroutine();
        }
    }

    private void PrepareNewCoroutine()
    {
        if (m_NewFrogLifeCoroutine != null)
            StopCoroutine(m_NewFrogLifeCoroutine);

        m_NewFrogLifeCoroutine = NextFrogWave();
        StartCoroutine(m_NewFrogLifeCoroutine);
    }

    private void ShowFrogElements(bool value)
    {
        m_StartTimer = value;
        m_Frog.SetActive(value);
        m_AllUI.SetActive(value);
    }

    public void AddScore()
    {
        m_ScoreValue += 100;
        m_Score.text = "SCORE : " + m_ScoreValue.ToString();
    }

    private IEnumerator StartDelay()
    {
        yield return new WaitForSeconds(2);
        m_StartTimer = true;
        m_Frog.SetActive(true);
    }

    private void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void QuitGame()
    {
        Application.Quit();
    }
    #endregion
}
