using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Android;
using Zenject;

public class Spawner : MonoBehaviour
{
    [SerializeField] SceneBounds sceneBounds;
    [SerializeField] GameObject prefab;
    [SerializeField] GameObject poolContainer;
    [SerializeField] GameObject[] spawnPoints;
    [Tooltip("On Start, set spawn points distance to center")]
    [SerializeField] float yDistanceToCenter;
    [SerializeField] bool useAllSpawnPointsPerSpawn;
    [SerializeField] int poolSize;
    [SerializeField] float delay;
    [SerializeField] Vector2 randomOffset;

    ServiceContainer<ISceneBounds?> sceneBoundsServiceContainer => SceneBoundsServiceContainer.Instance;
    Stack<ISpawn> spawnPool = new();
    Coroutine spawningCo;

    void Start()
    {
        Init();
    }

    async void Init()
    {
        PopulateSpawnPool();
        await ExistsValidSceneBoundsService();
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
            newSpawnObj.name += i;
            newSpawnObj.transform.SetParent(poolContainer.transform);
            var ISpawn = newSpawnObj.GetComponent<ISpawn>();
            SendObjToSpawnPool(ISpawn);
        }
    }

    void SetupSpawnPointsPosition()
    {
        // Set spawnPoints yDistanceToCenter
        foreach (var sp in spawnPoints)
        {
            var dir = (sp.transform.position - transform.position).normalized;
            sp.transform.position = transform.position + dir * yDistanceToCenter;
        }

        // Get leftMostSpawnPointX
        var leftMostSpawnPointX = spawnPoints[0].transform.position.x;
        for (int i = 1; i < spawnPoints.Length; i++)
        {
            if (spawnPoints[i].transform.position.x < leftMostSpawnPointX)
            {
                leftMostSpawnPointX = spawnPoints[i].transform.position.x;
            }
        }

        var screenRight = GetScreenBoundsRightValue();
        var spawnWidth = prefab.transform.localScale.x / 2;

        var minLeftMostSpawnPointX = screenRight + spawnWidth;

        var currentOffsetX = minLeftMostSpawnPointX - leftMostSpawnPointX;

        foreach (var sp in spawnPoints)
            sp.transform.position += new Vector3 (currentOffsetX, 0, 0);
    }

    async Task ExistsValidSceneBoundsService()
    {
        while (sceneBoundsServiceContainer.GetService() == null)
        {
            await Task.Yield();
        }
    }

    float GetScreenBoundsRightValue()
    {
        var sceneBounds = sceneBoundsServiceContainer.GetService();
        if (sceneBounds == null)
            return 0;

        return sceneBounds.GetCurrentCameraBoxBounds().Right;
    }

    IEnumerator Spawning()
    {
        while (true)
        {
            Spawn();
            yield return new WaitForSeconds(delay);
        }
    }

    void Spawn()
    {
        var randomOffset = RandomOffset();

        var nextSpawnPoints = NextSpawnPoints();

        var lastIndex = nextSpawnPoints.Length - 1;
        var isSpawnGroupLeader = false;
        for (int i = 0; i <= lastIndex; i++)
        {
            if (spawnPool.Count == 0)
                return;

            var obj = spawnPool.Pop();
            
            if (i == 0)
                obj.Flip();

            var randomOffsetedSpawnPos = nextSpawnPoints[i].transform.position + randomOffset;


            if (i == lastIndex)
                isSpawnGroupLeader = true;
            obj.Activate(randomOffsetedSpawnPos, isSpawnGroupLeader);
            obj.OnReadyToBackToPool += OnSpawnObjReadyToPool;
        }
    }

    GameObject[] NextSpawnPoints()
    {
        if (useAllSpawnPointsPerSpawn)
            return spawnPoints;

        return new GameObject[] { spawnPoints[Random.Range(0, spawnPoints.Length)] };
    }

    Vector3 RandomOffset() => new(randomOffset.x * RandomMinusOneToPlusOne, randomOffset.y * RandomMinusOneToPlusOne, 0);

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
