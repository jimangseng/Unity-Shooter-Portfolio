using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;
using static Cinemachine.DocumentationSortingAttribute;

public class Level : MonoBehaviour
{
    // Objects
    [field: SerializeField] public GameObject PlayerObject { get; set; }
    [field: SerializeField] public GameObject TileObject { get; set; }
    [field: SerializeField] public GameObject ObstacleObject { get; set; }
    [field: SerializeField] public Material[] LevelMaterials { get; set; }


    // Lists
    List<Tile> list { get; set; }
    List<Tile> listToRemove { get; set; }   // 필요한가?
    public List<Tile> listToUpdate { get; set; }


    // Readonly
    readonly float minPerlinSeed = 100.0f;
    readonly float maxPerlinSeed = 1000.0f;
    
    readonly int materialCount = 5;

    readonly float mapSize = 20.0f;
    readonly float obstacleFactor = 3.0f;


    // public properties
    public Area Area { get; set; }


    // private fields
    float perlinSeed; // 범위는 어떻게 정해야하나?

    // navmesh 관련
    Collider collider;
    NavMeshDataInstance navMeshDataInstance;
    NavMeshData navMeshData;
    NavMeshBuildSettings navMeshSettings;
    List<NavMeshBuildSource> navMeshSources = new List<NavMeshBuildSource>();
    NavMeshBuildSource navMeshSource;


    private void Start()
    {
        // 필드 초기화
        list = new List<Tile>();
        listToRemove = new List<Tile>();
        listToUpdate = new List<Tile>();

        Area = new Area();

        perlinSeed = UnityEngine.Random.Range(minPerlinSeed, maxPerlinSeed);

        collider = gameObject.GetComponent<BoxCollider>();
        navMeshDataInstance = new NavMeshDataInstance();
    }

    public void UpdateArea()
    {
        Area.minX = PlayerObject.transform.position.x - mapSize;
        Area.maxX = PlayerObject.transform.position.x + mapSize;
        Area.minY = PlayerObject.transform.position.z - mapSize;
        Area.maxY = PlayerObject.transform.position.z + mapSize;
    }

    public void AddTileToUpdateList()
    {
        // 영역 순회
        for (int i = Mathf.CeilToInt(Area.minY); i < Mathf.CeilToInt(Area.maxY); i++)
        {
            for (int j = Mathf.CeilToInt(Area.minX); j < Mathf.CeilToInt(Area.maxX); j++)
            {
                Tile tTile = GetTile(j, i);

                if (tTile == null)  // 해당 위치에 타일이 존재하지 않으면
                {
                    // 업데이트 리스트에 타일 추가
                    GameObject tObject = UnityEngine.Object.Instantiate(TileObject, new Vector3(j, 0, i), TileObject.transform.rotation, transform);
                    tTile = new Tile(j, i, tObject);

                    float perlinValue = Mathf.PerlinNoise(tTile.tile.transform.position.x * 0.1f + perlinSeed, tTile.tile.transform.position.z * 0.1f + perlinSeed);
                    tTile.perlinValue = Mathf.Clamp01(perlinValue);

                    listToUpdate.Add(tTile);
                }
            }
        }
    }

    public void RemoveOutdatedTile()
    {
        // 타일이 영역 바깥에 있으면 리스트에서 제거
        RemoveIf(t => t.x < Area.minX || t.x > Area.maxX || t.y < Area.minY || t.y > Area.maxY);
    }


    public void ApplyMaterials(Material[] _mats)
    {
        foreach (var t in listToUpdate)
        {
            // 타일 내의 펄린값을 이용하여 타일 머티리얼 종류 결정
            int tIndex = Mathf.FloorToInt(t.perlinValue * materialCount);
            t.tile.GetComponent<MeshRenderer>().material = _mats[tIndex];
        }
    }

    public void RenewList()
    {
        // 업데이트 리스트에서 현재 리스트로 데이터 이동
        // TODO: 반복문 복사가 아니라, 리스트 자체를 바꾸면 안될까?
        foreach (var t in listToUpdate)
        {
            t.Updated = true;
            list.Add(t);
        }
        listToUpdate.Clear();
    }


    // 레벨 전체의 콜라이더를 생성하는 데에 사용한다. NavMesh 생성을 위해 필요하다.
    public void UpdateAABBCollider()
    {
        Bounds totalBounds;

        Tile startingPoint = list.ElementAt(0);

        totalBounds = startingPoint.tile.GetComponent<MeshRenderer>().bounds;

        foreach (var t in list)
        {
            totalBounds.Encapsulate(t.tile.GetComponent<MeshRenderer>().bounds);
        }

        GetComponent<BoxCollider>().center = totalBounds.center;
        GetComponent<BoxCollider>().size = totalBounds.size;
    }

    public void CreateObstacles()
    {
        List<Tile> list = listToUpdate;

        foreach (var t in list)
        {
            // 타일 내 펄린값을 이용하여 장애물로 만들지의 여부 판단
            if (Mathf.FloorToInt(t.perlinValue * obstacleFactor) > 1)
            {
                Vector3 tPosition = new Vector3(0.0f, 1.0f, 0.0f);
                Transform tTransform = t.tile.transform;
                tTransform.Translate(tPosition);

                t.tile.layer = LayerMask.NameToLayer("Obstacle");
            }
        }
    }

    // 게임 스타트 시에 최초로 NavMesh데이터를 빌드한다.
    public void BuildNavMeshData()
    {
        navMeshSettings = GetComponent<NavMeshSurface>().GetBuildSettings();

        navMeshSource = new NavMeshBuildSource()
        {
            transform = Matrix4x4.identity,
            shape = NavMeshBuildSourceShape.Box,
            size = new Vector3(1.0f,  1.0f, 1.0f)
        };
        navMeshSources.Add(navMeshSource);

        // NavMeshData를 빌드할 때 필요한 것 - settings, sources, bounds
        navMeshData = NavMeshBuilder.BuildNavMeshData(navMeshSettings, navMeshSources, collider.bounds, Vector3.zero, Quaternion.identity);
        navMeshDataInstance = NavMesh.AddNavMeshData(navMeshData);
    }

    // 레벨 업데이트 루프마다 NavMesh데이터를 업데이트 한다.
    public void UpdateNavMeshData()
    {
        UpdateAABBCollider();

        // TODO: NavMeshSource 역시 매 루프마다 전부 삭제하고 다시 넣을 필요가 없지 않은가?
        navMeshSources.Clear();

        foreach (var t in list)
        {
            if (t.tile.layer != LayerMask.NameToLayer("Level"))
            {
                continue;
            }

            navMeshSource = new NavMeshBuildSource()
            {
                transform = t.tile.transform.localToWorldMatrix,
                shape = NavMeshBuildSourceShape.Box,
                size = t.tile.GetComponent<BoxCollider>().size
            };

            navMeshSources.Add(navMeshSource);
        }

        NavMeshBuilder.UpdateNavMeshData(navMeshData, navMeshSettings, navMeshSources, collider.bounds);
    }


    public Tile GetTile(int _x, int _y)
    {
        //해당 위치에 타일이 있는지 검사
        Tile tTileFound = list.Find(t => (t.x == _x) && (t.y == _y));

        return tTileFound ?? null;
    }

    public void RemoveAt(int _x, int _y)
    {
        Tile tTileFound = list.Find(t => (t.x == _x) && (t.y == _y));

        list.Remove(tTileFound);
    }

    public void RemoveIf(Predicate<Tile> match)
    {
        listToRemove = list.FindAll(match);

        foreach (Tile t in listToRemove)
        {
            UnityEngine.Object.Destroy(t.tile);
            list.Remove(t);
        }
    }
}


public class Tile
{
    public Tile(GameObject _tile)
    {
        x = _tile.transform.position.x;
        y = _tile.transform.position.z;
        tile = _tile;
    }

    public Tile(float _x, float _y, GameObject _tile)
    {
        x = _x;
        y = _y;
        tile = _tile;
    }

    public float x;
    public float y;
    public GameObject tile;
    public float perlinValue;   // 타일마다 갖는 0-1 범위의 펄린 값 - 머티리얼 결정과 장애물 생성에 사용

    public bool Updated { get; set; }   // 한 번 업데이트한 타일은 중복하여 업데이트 하지 않는다
}


public class Area
{
    public float minX = 0;
    public float maxX = 0;
    public float minY = 0;
    public float maxY = 0;

    public Area()
    {

    }

    public Area(float _minX, float _maxX, float _minY, float _maxY)
    {
        minX = _minX;
        maxX = _maxX;
        minY = _minY;
        maxY = _maxY;
    }

    public Area(Vector3 _position, float _mapSize)
    {
        minX = _position.x - _mapSize;
        maxX = _position.x + _mapSize;
        minY = _position.z - _mapSize;
        maxY = _position.z + _mapSize;
    }
}