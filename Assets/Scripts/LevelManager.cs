using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NavMeshBuilder = UnityEngine.AI.NavMeshBuilder;
using NavMeshSurface = Unity.AI.Navigation.NavMeshSurface;
using Random = UnityEngine.Random;
using Object = UnityEngine.Object;
using System;
using UnityEditor;
using Unity.VisualScripting;
using static Cinemachine.DocumentationSortingAttribute;
using UnityEditor.AI;
using UnityEngine.AI;
using System.Diagnostics;
using System.Linq;


public class LevelManager : MonoBehaviour
{
    Level level;

    /// Properties

    TileList tiles = new TileList();

    readonly float mapSize = 20.0f;
    readonly float updateLevelInterval = 0.5f;
    readonly float minPerlinSeed = 100.0f;
    readonly float maxPerlinSeed = 1000.0f;


    public Rect playerRect;
    public Rect mapRect;

    float perlinSeed;

    NavMeshDataInstance navMeshDataInstance;

    /// Methodes

    private void Start()
    {
        // Rect 관련
        playerRect = new Rect(-mapSize, mapSize, -mapSize, mapSize);
        mapRect = new Rect(-mapSize, mapSize, -mapSize, mapSize);
        
        navMeshDataInstance = new NavMeshDataInstance();

        perlinSeed = Random.Range(minPerlinSeed, maxPerlinSeed); // 범위는 어떻게 정해야하나?
    }

    private void Update()
    {
        
    }

    public void SetLevel(Level _level)
    {
        level = _level;
    }

    public IEnumerator UpdateLevel()
    {
        while (true)
        {
            // 영역 업데이트
            UpdatePlayerRect(level.PlayerObject.transform.position);
            UpdateMapRect();

            // 타일 리스트 업데이트
            UpdateTileList();

            /// 매 업데이트마다 중복 적용되고 있다. 최적화 필요
            // 장애물 추가
            AddObstacle(tiles);

            // NavMesh 데이터를 업데이트
            UpdateNavMeshData();

            // 머티리얼 적용
            tiles.ApplyMaterials(level.LevelMaterials);

            // 부모 지정
            tiles.SetParent(level.gameObject);

            tiles.Show();

            yield return new WaitForSeconds(updateLevelInterval);
        }
    }

    public void AddObstacle(TileList _tiles)
    {
        List<Tile> list = _tiles.GetList();

        foreach (var t in list)
        {
            int tIndex = Mathf.FloorToInt(t.perlinValue * (3.0f));

            if (tIndex > 1 && !t.updated)
            {
                Vector3 tPosition = new Vector3(0.0f, 1.0f, 0.0f);
                Transform tTransform = t.tile.transform;
                tTransform.Translate(tPosition);

                t.tile.layer = LayerMask.NameToLayer("Obstacle");

                t.updated = true;
            }
        }
    }

    public void UpdateNavMeshData()
    {
        // AABB 콜라이더 업데이트
        tiles.UpdateAABBCollider(level.gameObject);

        NavMesh.RemoveNavMeshData(navMeshDataInstance);

        BoxCollider collider = level.GetComponent<BoxCollider>();

        NavMeshBuildSettings settings = new NavMeshBuildSettings();

        Matrix4x4 matrix = new Matrix4x4();

        List<NavMeshBuildSource> sources = new List<NavMeshBuildSource>();
        NavMeshBuildSource source;

        NavMeshData data = new NavMeshData();

        settings = level.GetComponent<NavMeshSurface>().GetBuildSettings();

        foreach (var t in tiles.GetList())
        {
            if (t.tile.layer != LayerMask.NameToLayer("Level"))
            {
                continue;
            }

            source = new NavMeshBuildSource();

            source.transform = t.tile.transform.localToWorldMatrix;
            source.shape = NavMeshBuildSourceShape.Box;
            source.size = t.tile.GetComponent<BoxCollider>().size;
            sources.Add(source);
        }

        data = NavMeshBuilder.BuildNavMeshData(settings, sources, collider.bounds, Vector3.zero, Quaternion.identity);
        navMeshDataInstance = NavMesh.AddNavMeshData(data);

    }





    /////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////

    void UpdatePlayerRect(Vector3 _position)
    {
        playerRect.minX = (_position.x) - mapSize;
        playerRect.maxX = (_position.x) + mapSize;
        playerRect.minY = (_position.z) - mapSize;
        playerRect.maxY = (_position.z) + mapSize;
    }

    void UpdateMapRect()
    {
        // 우측으로 이동
        if (playerRect.maxX > mapRect.maxX)
        {
            mapRect.minX = Mathf.Max(playerRect.minX, mapRect.minX);
            mapRect.maxX = Mathf.Max(playerRect.maxX, mapRect.maxX);
        }
        // 좌측으로 이동
        else if (playerRect.minX < mapRect.minX)
        {
            mapRect.minX = Mathf.Min(playerRect.minX, mapRect.minX);
            mapRect.maxX = Mathf.Min(playerRect.maxX, mapRect.maxX);
        }
        // 상단으로 이동
        if (playerRect.maxY > mapRect.maxY)
        {
            mapRect.minY = Mathf.Max(playerRect.minY, mapRect.minY);
            mapRect.maxY = Mathf.Max(playerRect.maxY, mapRect.maxY);
        }
        // 하단으로 이동
        else if (playerRect.minY < mapRect.minY)
        {
            mapRect.minY = Mathf.Min(playerRect.minY, mapRect.minY);
            mapRect.maxY = Mathf.Min(playerRect.maxY, mapRect.maxY);
        }

        //Debug.Log(mapRect.minX + ", " + mapRect.maxX + ", " + mapRect.minY + ", " + mapRect.maxY);
    }

    void UpdateTileList()
    {
        // TODO : 최적화
        // player Rect을 순회
        for (int i = Mathf.FloorToInt(playerRect.minY); i < Mathf.CeilToInt(playerRect.maxY); i++)
        {
            for (int j = Mathf.FloorToInt(playerRect.minX); j < Mathf.CeilToInt(playerRect.maxX); j++)
            {
                Tile tTile = tiles.GetTile(j, i);

                if (tTile == null)
                {
                    // 타일 추가
                    GameObject tObject = Instantiate(level.TileObject, new Vector3(j, 0, i), level.TileObject.transform.rotation);
                    tTile = new Tile(j, i, tObject);

                    float perlinValue = Mathf.PerlinNoise(tTile.tile.transform.position.x * 0.1f + perlinSeed, tTile.tile.transform.position.z * 0.1f + perlinSeed);
                    tTile.perlinValue = Mathf.Clamp01(perlinValue);

                    tiles.Add(tTile);
                }
            }
        }

        // 타일이 mapRect 바깥에 있으면 리스트에서 제거
        tiles.RemoveIf(t => t.x < mapRect.minX || t.x > mapRect.maxX || t.y < mapRect.minY || t.y > mapRect.maxY);
    }
}