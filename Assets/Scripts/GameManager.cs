using Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NavMeshSurface = Unity.AI.Navigation.NavMeshSurface;

public class GameManager : MonoBehaviour
{
    #region Singleton
    // making a singleton GameManager object
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType(typeof(GameManager)) as GameManager;
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }
    #endregion
    [Header("Level")]
    public LevelManager levelManager;
    public Level level;

    [Header("Player & Enemy")]
    public GameObject player;
    public GameObject enemy;
    public List<GameObject> enemies;

    public Status playerMode = Status.Stopped;

    // Start is called before the first frame updatez`
    void Start()
    { 

        // initialize enemy list
        enemies = new List<GameObject>();

        // designate level to level manager
        levelManager.SetLevel(level);

        // update level
        StartCoroutine(levelManager.UpdateLevel());
        StartCoroutine(levelManager.UpdateNavMeshData());

        // start to spawn enemies
        StartCoroutine(SpawnEnemies());
    }

    // Update is called once per frame
    void Update()
    {

    }

    // repeatedly spawn enemies
    IEnumerator SpawnEnemies()
    {
        while (true)
        {
            GameObject enemyInstance = null;

            if (enemies.Count >= 0 && enemies.Count < 5)
            {
                // 적 스폰
                Quaternion rotation = Quaternion.Euler(new Vector3(0.0f, Random.Range(0.0f, 360.0f), 0.0f));

                Vector3 enemyPosition = new Vector3(
                    player.transform.position.x + Random.Range(1.0f, 5.0f),
                    1.5f,
                    player.transform.position.z + Random.Range(1.0f, 5.0f));
                enemyInstance = Instantiate(enemy, enemyPosition, rotation);

                // set enemy
                enemyInstance.SetActive(true);
                enemyInstance.transform.GetChild(0).gameObject.SetActive(false);
                enemies.Add(enemyInstance);

                enemyInstance.GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = false;
            }

            yield return new WaitForSeconds(1.0f);

            for (int i = 0; i < enemies.Count; i++)
            {
                // 맵 크기에 접근 가능해야 함.
                Vector3 enemyPosition = enemies[i].transform.position;
                Rect mapRect = levelManager.mapRect;
                if (enemyPosition.x > mapRect.maxX)
                {
                    enemyPosition.x -= enemyPosition.x - mapRect.maxX;
                }
                else if (enemyPosition.x < mapRect.minX)
                {
                    enemyPosition.x += mapRect.maxX - enemyPosition.x;
                }
                else if (enemyPosition.y > mapRect.maxY)
                {
                    enemyPosition.y -= enemyPosition.y - mapRect.maxY;
                }
                else
                {
                    enemyPosition.y += mapRect.maxY - enemyPosition.y;
                }

                if (enemies[i].activeInHierarchy)
                {
                    // start to move
                    enemies[i].GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = true;
                    // set navmesh agent destination
                    enemies[i].GetComponent<UnityEngine.AI.NavMeshAgent>().SetDestination(player.transform.position);
                    // activate  trail particle system
                    enemies[i].transform.GetChild(0).gameObject.SetActive(true);
                }
                else
                {
                    enemies[i].GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = true;

                    Destroy(enemies[i], 0.01f);
                    enemies.Remove(enemies[i]);
                }
            }

        }
    }
}