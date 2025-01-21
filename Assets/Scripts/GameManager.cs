using Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
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
    [Header("Managers")]
    [SerializeField] LevelManager levelManager;
    [SerializeField] StageManager stageManager;

    [Header("Level")]
    [SerializeField] Level level;

    [Header("Player & Enemy")]
    public GameObject player;
    [SerializeField] GameObject enemyObject;


    // public properties
    public List<GameObject> Enemies { get; set; }
    public int Kills { get; set; } = 0;


    // readonly
    readonly float updateEnemyInterval = 1.0f;
    readonly int maxEnemy = 5;


    // Start is called before the first frame updatez`
    void Start()
    {
        //// 스테이지 관련
        stageManager = GameObject.Find("StageManager").GetComponent<StageManager>();
        stageManager.startStage();


        //// 레벨 관련
        // build a navmesh
        level.BuildNavMeshData();

        // designate level to level manager
        levelManager.SetLevel(level);

        // update level
        StartCoroutine(levelManager.UpdateLevel());


        //// 적 관련
        // initialize enemy list
        Enemies = new List<GameObject>();

        // start to spawn enemies
        StartCoroutine(UpdateEnemies());

    }

    // Update is called once per frame
    void Update()
    {

    }

    void SpawnEnemy()
    {
        // 적 transform 정의
        Quaternion rotation = Quaternion.Euler(new Vector3(0.0f, Random.Range(0.0f, 360.0f), 0.0f));
        Vector3 position = new Vector3(
            player.transform.position.x + Random.Range(1.0f, 5.0f),
            1.5f,
            player.transform.position.z + Random.Range(1.0f, 5.0f)
            );

        // 적 스폰 공간 제한
        if (position.x > level.Area.maxX)
        {
            position.x = level.Area.maxX;
        }
        else if (position.x < level.Area.minX)
        {
            position.x = level.Area.minX;
        }
        if (position.z > level.Area.maxY)
        {
            position.z = level.Area.maxY;
        }
        else if (position.z < level.Area.minY)
        {
            position.z = level.Area.minY;
        }

        // 스폰
        GameObject instance = Instantiate(enemyObject, position, rotation);
        Enemies.Add(instance);
    }


    IEnumerator UpdateEnemies()
    {
        while (true)
        {
            while (Enemies.Count >= 0 && Enemies.Count < maxEnemy)
            {
                SpawnEnemy();
            }

            foreach (var enemy in Enemies)
            {
                enemy.SetActive(true);

                enemy.GetComponent<NavMeshAgent>().enabled = true;
                enemy.GetComponent<NavMeshAgent>().SetDestination(player.transform.position);

                enemy.transform.GetChild(0).gameObject.SetActive(true);
            }

            yield return new WaitForSeconds(updateEnemyInterval);
        }

    }
}