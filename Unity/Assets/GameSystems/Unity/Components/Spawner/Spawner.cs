using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] GameObject prefab;
    [SerializeField] GameObject poolContainer;
    [SerializeField] GameObject[] spawnPoints;
    [Tooltip("On Start, set spawn points distance to center")]
    [SerializeField] float distanceToCenter;
    [SerializeField] bool useAllSpawnPointsPerSpawn;
    [SerializeField] int poolSize;
    [SerializeField] float delay;
    [SerializeField] Vector2 randomOffset;

    Stack<ISpawn> spawnPool = new();
    Coroutine spawningCo;

    void Awake()
    {
        PopulateSpawnPool();
        SetupSpawnPointsPosition();
    }

    void PopulateSpawnPool()
    {
        if (!prefab.TryGetComponent<ISpawn>(out var ISpawn))
        {
            Debug.Log("Error: Type Requirement: Spawn Obj is not of type ISpawn");
            return;
        }

        spawnPool = new();
        for (int i = 0; i < poolSize; i++)
        {
            var newSpawnObj = Instantiate(prefab);
            newSpawnObj.transform.SetParent(poolContainer.transform);
            var newSpawn = newSpawnObj.GetComponent<ISpawn>();
            SendObjToSpawnPool(newSpawn);
        }
    }

    void OnSpawnObjReadyToPool(ISpawn obj)
    {
        obj.OnReadyToPool -= OnSpawnObjReadyToPool;
        SendObjToSpawnPool(obj);
    }

    void SendObjToSpawnPool(ISpawn obj)
    {
        obj.Deactivate();
        spawnPool.Push(obj);
    }

    void Start()
    {
        spawningCo = StartCoroutine(StartSpawning());
    }

    void SetupSpawnPointsPosition()
    {
        foreach (var sp in spawnPoints)
        {
            var dir = (sp.transform.position - transform.position).normalized;
            sp.transform.position = transform.position + dir * distanceToCenter;
        }
    }

    IEnumerator StartSpawning()
    {
        while (true)
        {
            Spawn();
            yield return new WaitForSeconds(delay);
        }
    }

    void Spawn()
    {
        if (useAllSpawnPointsPerSpawn)
        {
            Vector3 randomOffsetV3 = new(randomOffset.x * RandomMinusOneToPlusOne, randomOffset.y * RandomMinusOneToPlusOne, 0);
            foreach (var spawnPoint in spawnPoints)
            {
                Debug.Log("spawnPool.Count: " + spawnPool.Count);
                if (spawnPool.Count == 0)
                    return;

                var obj = spawnPool.Pop();
                obj.OnReadyToPool += OnSpawnObjReadyToPool;
                var spawnPos = spawnPoint.transform.position + randomOffsetV3;
                obj.Activate(spawnPos);
            }
        }
    }

    float RandomMinusOneToPlusOne  => Random.Range(-1.0f, 1.0f);
}
