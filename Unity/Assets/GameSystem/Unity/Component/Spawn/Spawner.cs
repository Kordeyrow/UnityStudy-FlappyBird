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

    void Start()
    {
        PopulateSpawnPool();
        SetupSpawnPointsPosition();
        spawningCo = StartCoroutine(Spawning());
    }

    void PopulateSpawnPool()
    {
        if (prefab.GetComponent<ISpawn>() == null)
        {
            Debug.Log("Error: Type Requirement: Spawn Obj is not of type ISpawn");
            return;
        }

        spawnPool = new();
        for (int i = 0; i < poolSize; i++)
        {
            var newSpawnObj = Instantiate(prefab);
            newSpawnObj.name = newSpawnObj.name + i;
            newSpawnObj.transform.SetParent(poolContainer.transform);
            var ISpawn = newSpawnObj.GetComponent<ISpawn>();
            SendObjToSpawnPool(ISpawn);
        }
    }

    void SetupSpawnPointsPosition()
    {
        foreach (var sp in spawnPoints)
        {
            var dir = (sp.transform.position - transform.position).normalized;
            sp.transform.position = transform.position + dir * distanceToCenter;
        }
    }

    IEnumerator Spawning()
    {
        while (true)
        {
            yield return new WaitForEndOfFrame();
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
                if (spawnPool.Count == 0)
                    return;

                var obj = spawnPool.Pop();
                obj.OnReadyToBackToPool += OnSpawnObjReadyToPool;
                var spawnPos = spawnPoint.transform.position + randomOffsetV3;
                obj.Activate(spawnPos);
            }
        }
    }

    void OnSpawnObjReadyToPool(ISpawn obj)
    {
        obj.OnReadyToBackToPool -= OnSpawnObjReadyToPool;
        SendObjToSpawnPool(obj);
    }

    void SendObjToSpawnPool(ISpawn obj)
    {
        obj.Deactivate();
        spawnPool.Push(obj);
    }

    float RandomMinusOneToPlusOne  => Random.Range(-1.0f, 1.0f);
}
