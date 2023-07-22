using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Perell.Artemis.Example.Rituals
{
    public enum EnemyType
    {
        TRICLOPS,
        BLOB,
        CACODEMON
    }

    public class EnemyManager : MonoBehaviour
    {
        int enemiesAlive;

        [SerializeField]
        GameObject triclopsPrefab;
        [SerializeField]
        GameObject blobPrefab;
        [SerializeField]
        GameObject cacodemonPrefab;
        [SerializeField]
        GameObject soulPrefab;

        public event UnityAction EnemiesClearedOut;

        public bool HasEnemies { get { return enemiesAlive > 0; } }

        public void SpawnEnemy(Vector3 position, EnemyType enemyType)
        {
            GameObject enemySpawned = null;
            switch (enemyType)
            {
                case EnemyType.TRICLOPS:
                    enemySpawned = Instantiate(triclopsPrefab, position, Quaternion.identity);
                    break;
                case EnemyType.BLOB:
                    enemySpawned = Instantiate(blobPrefab, position, Quaternion.identity);
                    break;
                case EnemyType.CACODEMON:
                    enemySpawned = Instantiate(cacodemonPrefab, position, Quaternion.identity);
                    break;
            }

            if (enemySpawned != null)
            {
                enemiesAlive++;
                enemySpawned.GetComponentInChildren<EnemyHealth>().EnemyDied += RespondToEnemyDied;
            }
        }

        private void RespondToEnemyDied(EnemyHealth reporter)
        {
            enemiesAlive--;
            reporter.EnemyDied -= RespondToEnemyDied;

            if (!AncientRuinsManager.PlayerController.IsPossessed)
            {
                Instantiate(soulPrefab, reporter.transform.position, Quaternion.identity).transform.parent = AncientRuinsManager.Dungeon;
            }

            Destroy(reporter.transform.parent.gameObject);

            if(enemiesAlive == 0)
            {
                EnemiesClearedOut?.Invoke();
            }
        }
    }
}