using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionByCollision : MonoBehaviour, ICollisable
{
    public GameObject ragdollPrefab;
    public float forcePower;
    private bool isAction;
    public void Init()
    {
        isAction = false;
    }
    public void onActionByCollision(GameObject other)
    {
        if(!isAction)
        {
            isAction = true;
            
            EnemyStats enemyStats = null;
            if (gameObject.CompareTag("Enemy"))
                enemyStats = GetComponent<EnemyController>().stats;
            else if (gameObject.CompareTag("FixedEnemy"))
                enemyStats = GetComponent<FixedEnemyController>().stats;

            var player = other.GetComponent<PlayerController>();
            var scoreManager = GameManager.Instance.scoreManager;

            var isKick = Random.Range(0, 100) < enemyStats.kickRate;

            GameObject effectObj = null;
            GameObject soundObj = null;
            if (isKick)
            {
                var effectPos = transform.position + new Vector3(0f, 5f, 2f);
                effectObj = ObjectPool.GetObject(PoolName.KickParticle);
                effectObj.transform.position = effectPos;

                soundObj = ObjectPool.GetObject(PoolName.KickSound);
                soundObj.transform.position = transform.position;

                var ragdoll = Instantiate(ragdollPrefab, transform.position, transform.rotation);
                var ragdollMgr = ragdoll.GetComponent<RagdollManager>();

                ragdollMgr.SetStats(enemyStats);
                scoreManager.AddKickEnemyNumber();

                var dir = ragdoll.transform.position - other.transform.position;

                dir.y = 0.5f;
                ragdollMgr.ApplyForce(dir * forcePower);

                Destroy(ragdoll, 2f);
            }
            else
            {
                var effectPos = transform.position + new Vector3(0f, 3f, 0f);
                effectObj = ObjectPool.GetObject(PoolName.HoldParticle);
                effectObj.transform.position = effectPos;

                soundObj = ObjectPool.GetObject(PoolName.HoldSound);
                soundObj.transform.position = transform.position;

                var playerStat = player.stats;

                player.MsgGetPenalty();

                player.SetActiveRagdoll(enemyStats);
                scoreManager.AddHoldEnemyWeight(enemyStats.weight);
            }

            var effect = effectObj.GetComponent<ParticleSystem>();
            var sound = soundObj.GetComponent<AudioSource>();
            effect.Play();
            sound.Play();
            GameManager.Instance.InplayPrintScore();
        }
    }
}