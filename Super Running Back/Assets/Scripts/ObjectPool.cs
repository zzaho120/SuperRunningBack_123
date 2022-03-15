using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance;
    public int maxEnemyCnt;
    public int maxItemCnt;
    public int maxFixedEnemyCnt;
    public int maxParticleCnt;
    public int maxSoundCnt;

    public List<GameObject> prefabsList;

    public Dictionary<PoolName, Queue<GameObject>> pool = new Dictionary<PoolName, Queue<GameObject>>();
    public Dictionary<PoolName, GameObject> prefabs = new Dictionary<PoolName, GameObject>();

    private int poolNameCount = (int)PoolName.PoolNameMax;

    private void Awake()
    {
        Instance = this;

        for(int idx = 0; idx < poolNameCount; idx++)
        {
            pool.Add((PoolName)idx, new Queue<GameObject>());
            prefabs.Add((PoolName)idx, prefabsList[idx]);
        }

        queueInit(maxEnemyCnt, prefabs[PoolName.Enemy], pool[PoolName.Enemy]);
        queueInit(maxFixedEnemyCnt, prefabs[PoolName.FixedEnemy], pool[PoolName.FixedEnemy]);

        queueInit(maxItemCnt, prefabs[PoolName.Item], pool[PoolName.Item]);
        queueInit(maxItemCnt, prefabs[PoolName.ItemSound], pool[PoolName.ItemSound]);
        queueInit(maxItemCnt, prefabs[PoolName.ItemParticle], pool[PoolName.ItemParticle]);

        queueInit(maxParticleCnt, prefabs[PoolName.EnemyParticle0], pool[PoolName.EnemyParticle0]);
        queueInit(maxParticleCnt, prefabs[PoolName.EnemyParticle1], pool[PoolName.EnemyParticle1]);
        queueInit(0, prefabs[PoolName.KickSound], pool[PoolName.KickSound]);
        queueInit(0, prefabs[PoolName.HoldSound], pool[PoolName.HoldSound]);
        queueInit(maxEnemyCnt, prefabs[PoolName.FinishRagdoll], pool[PoolName.FinishRagdoll]);
    }

    private void queueInit(int initCnt, GameObject obj, Queue<GameObject> queue)
    {
        for (int idx = 0; idx < initCnt; idx++)
        {
            queue.Enqueue(Create(obj));
        }
    }

    private GameObject Create(GameObject obj)
    {
        var newGo = Instantiate(obj);
        newGo.SetActive(false);
        newGo.transform.SetParent(transform);
        return newGo;
    }

    public static GameObject GetObject(PoolName poolName)
    {
        var queue = Instance.pool[poolName];
        if (queue.Count > 0)
        {
            var obj = queue.Dequeue();
            obj.transform.SetParent(null);
            obj.SetActive(true); 
            return obj;
        }
        else
        {
            var newGo = Instance.Create(Instance.prefabs[poolName]);
            newGo.gameObject.SetActive(true);
            newGo.transform.SetParent(null);
            return newGo;
        }
          
    }
    
    public static void ReturnObject(PoolName poolName, GameObject obj)
    {
        obj.SetActive(false);
        obj.transform.SetParent(Instance.transform);
        Instance.pool[poolName].Enqueue(obj);
    }
}
