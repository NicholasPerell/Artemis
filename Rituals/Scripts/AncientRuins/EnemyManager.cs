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
        int numEnemiesAlive;
        List<EnemyHealth> enemiesAlive;

        [SerializeField]
        GameObject triclopsPrefab;
        [SerializeField]
        GameObject blobPrefab;
        [SerializeField]
        GameObject cacodemonPrefab;
        [SerializeField]
        GameObject soulPrefab;

        public event UnityAction EnemiesClearedOut;

        public bool HasEnemies { get { return numEnemiesAlive > 0; } }

        private void Awake()
        {
            enemiesAlive = new List<EnemyHealth>();
        }

        private void OnDisable()
        {
            ClearEnemies();
        }

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
                numEnemiesAlive++;
                EnemyHealth enemyHealth = enemySpawned.GetComponentInChildren<EnemyHealth>();
                enemiesAlive.Add(enemyHealth);
                enemyHealth.EnemyDied += RespondToEnemyDied;
            }
        }

        private void RespondToEnemyDied(EnemyHealth reporter)
        {
            numEnemiesAlive--;
            enemiesAlive.Remove(reporter);
            reporter.EnemyDied -= RespondToEnemyDied;

            if (!AncientRuinsManager.PlayerController.IsPossessed)
            {
                Instantiate(soulPrefab, reporter.transform.position, Quaternion.identity).transform.parent = AncientRuinsManager.Dungeon;
            }

            Destroy(reporter.transform.parent.gameObject);

            if(numEnemiesAlive == 0)
            {
                EnemiesClearedOut?.Invoke();
            }
        }

        public Vector3 ClosestEnemyPosition(Vector3 searchingPosition)
        {
            Vector3 result = searchingPosition;

            if(HasEnemies)
            {
                result = enemiesAlive[0].transform.position;
                //TODO: use square magnitudes to compare the distances to skip the square root functions
                for(int i = 1; i < enemiesAlive.Count; i++)
                {   
                    if(Vector3.Distance(searchingPosition, result) > Vector3.Distance(searchingPosition, enemiesAlive[i].transform.position))
                    {
                        result = enemiesAlive[i].transform.position;
                    }
                }
            }

            return result;
        }

        public void ClearEnemies()
        {
            for(int i = 0; i < enemiesAlive.Count; i++)
            {
                Destroy(enemiesAlive[i].transform.parent.gameObject);
            }
            enemiesAlive.Clear();
            numEnemiesAlive = 0;
        }
    }
}