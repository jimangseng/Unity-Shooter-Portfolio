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
    [Header("Managers")]
    [SerializeField] LevelManager levelManager;
    [SerializeField] StageManager stageManager;

    [Header("Level")]
    [SerializeField] Level level;

    [Header("Player & Enemy")]
    [SerializeField] GameObject player;
    [SerializeField] GameObject enemyObject;

    public List<GameObject> Enemies { get; set; }
    public int Kills { get; set; } = 0;

    readonly float updateEnemyInterval = 1.0f;
    readonly int maxEnemy = 5;

    // Start is called before the first frame updatez`
    void Start()
    {
        // Stage Manager
        stageManager = GameObject.Find("StageManager").GetComponent<StageManager>();
        stageManager.startStage();

        // initialize enemy list
        Enemies = new List<GameObject>();

        // designate level to level manager
        levelManager.SetLevel(level);

        // update level
        StartCoroutine(levelManager.UpdateLevel());

        // start to spawn enemies
        StartCoroutine(UpdateEnemies());
    }

    // Update is called once per frame
    void Update()
    {

    }

    // repeatedly spawn enemies
    IEnumerator UpdateEnemies()
    {

        Area area;

        Quaternion rotation;
        Vector3 position;

        GameObject instance;

        while (true)
        {
            area = level.Area;

            while(Enemies.Count >= 0 && Enemies.Count < maxEnemy)
            {
                rotation = Quaternion.Euler(new Vector3(0.0f, Random.Range(0.0f, 360.0f), 0.0f));
                position = new Vector3(
                    player.transform.position.x + Random.Range(1.0f, 5.0f),
                    1.5f,
                    player.transform.position.z + Random.Range(1.0f, 5.0f)
                    );

                if (position.x > area.maxX)
                {
                    position.x = area.maxX;
                }
                else if (position.x < area.minX)
                {
                    position.x = area.minX;
                }
                if (position.z > area.maxY)
                {
                    position.z = area.maxY;
                }
                else if (position.z < area.minY)
                {
                    position.z = area.minY;
                }

                instance = Instantiate(enemyObject, position, rotation);
                Enemies.Add(instance);
            }

            foreach(var e in Enemies)
            {
                e.SetActive(true);

                // set navmesh agent destination and start to move
                e.GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = true;
                e.GetComponent<UnityEngine.AI.NavMeshAgent>().SetDestination(Vector3Int.RoundToInt(player.transform.position));

                // activate  trail particle system
                e.transform.GetChild(0).gameObject.SetActive(true);
            }

            yield return new WaitForSeconds(updateEnemyInterval);
        }


    }

}