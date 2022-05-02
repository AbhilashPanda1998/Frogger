using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaneController : MonoBehaviour
{
    #region Variables
    public int poolCount;

    [SerializeField] private bool autoDirection, autoTimeBetweenObjects;
    [SerializeField] private LANE_DIRECTION laneDirection;
    [SerializeField] private float minTime, maxTime, timeGapForNextObject;

    [SerializeField] private float m_MinSpeed, m_MaxSpeed;
    [SerializeField] private List<Transform> m_SpawnPoints;
    [SerializeField] private List<GameObject> m_SpawnableObjects;
    [SerializeField] private List<SpawnableObject> m_ItemContainer;

    private float m_ObjectSpeed;
    private Transform m_SpawnPoint;
    #endregion

    #region Unity Callbacks
    private void Start()
    {
        SpawnableObject.ReachedCheckPoint += PrepareNextSpawn;
    }

    private void OnDestroy()
    {
        SpawnableObject.ReachedCheckPoint -= PrepareNextSpawn;
    }
    #endregion

    #region Class Functions
    public void SpawnObjects()
    {
        SetSpawnPoint();

        m_ObjectSpeed = Random.Range(m_MinSpeed, m_MaxSpeed);

        for (int i = 0; i < poolCount; i++)
        {
            GameObject spawnedObject = Instantiate(m_SpawnableObjects[Random.Range(0, m_SpawnableObjects.Count)], m_SpawnPoint.position, m_SpawnPoint.rotation);
            spawnedObject.SetActive(false);
            SpawnableObject spawnableObjectClass = spawnedObject.GetComponent<SpawnableObject>();
            spawnableObjectClass.Initialize(m_ObjectSpeed, this);
            m_ItemContainer.Add(spawnableObjectClass);
        }

        StartCoroutine(EnableObjects());
    }


    private void SetSpawnPoint()
    {
        if (autoDirection) laneDirection = (LANE_DIRECTION)Random.Range(0, 2);

        m_SpawnPoint = (laneDirection == LANE_DIRECTION.LEFT) ? m_SpawnPoints[0] : m_SpawnPoints[1];
    }

    public IEnumerator EnableObjects()
    {

        for (int i = 0; i < m_ItemContainer.Count; i++)
        {
            if (autoTimeBetweenObjects) timeGapForNextObject = Random.Range(minTime, maxTime);
            m_ItemContainer[i].gameObject.SetActive(true);
            yield return new WaitForSeconds(timeGapForNextObject);
        }

    }

    private void PrepareNextSpawn(LaneController controller)
    {
        if(this == controller)
        {
            StartCoroutine(SendNextSpawn());
        }
    }

    private IEnumerator SendNextSpawn()
    {
        for (int i = 0; i < m_ItemContainer.Count; i++)
        {
            if (!m_ItemContainer[i].gameObject.activeInHierarchy)
            {
                yield return new WaitForSeconds(timeGapForNextObject);
                m_ItemContainer[i].gameObject.SetActive(true);
            }
        }
    }
    #endregion

}
