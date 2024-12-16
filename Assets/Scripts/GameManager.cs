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

    // Manager Property
    public GameObject player;
    public GameObject enemy;

    public GameObject levelObject;
    GameObject tileObject;
    GameObject obstacleObject;

    public List<GameObject> enemies;

    NavMeshSurface navMeshSurface;
    Material[] materials;

    LevelManager levelManager;

    // Start is called before the first frame update
    void Start()
    {
        // initialize enemy list
        enemies = new List<GameObject>();
        tileObject = levelObject.transform.GetChild(0).gameObject;
        navMeshSurface = levelObject.GetComponent<NavMeshSurface>();
        materials = levelObject.GetComponent<LevelScript>().mats;

        levelManager = new LevelManager(player, new LevelData(tileObject, obstacleObject, materials, navMeshSurface));

        // update level
        StartCoroutine(levelManager.UpdateLevel());

        // start to spawn enemies
        //StartCoroutine(SpawnEnemies());
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