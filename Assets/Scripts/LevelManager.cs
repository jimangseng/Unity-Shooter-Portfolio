using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NavMeshSurface = Unity.AI.Navigation.NavMeshSurface;
using Random = UnityEngine.Random;
using Object = UnityEngine.Object;

using System;
using UnityEditor;
using UnityEngine.AI;
using Unity.VisualScripting;

public class LevelManager
{
    /// Properties
    // external
    GameObject playerObject;
    GameObject tileObject;
    GameObject obstacleObject;
    Material[] materials;
    NavMeshSurface navMeshSurface;

    // internal
    TileList tiles = new TileList();

    float range = 20.0f;
    float updateTime = 0.1f;

    public Rect playerRect;
    public Rect mapRect;

    float perlinSeed = Random.Range(100.0f, 1000.0f); // 범위는 어떻게 정해야하나?

    /// Methodes
    public LevelManager(GameObject _player, LevelData _levelData)
    {
        // 프로퍼티 초기화
        playerObject = _player;
        tileObject = _levelData.tileObject;
        obstacleObject = _levelData.obstacleObject;
        materials = _levelData.mats;
        navMeshSurface = _levelData.surface;

        playerRect = new Rect(-range, range, -range, range);
        mapRect = new Rect(-range, range, -range, range);
    }


    public IEnumerator UpdateLevel()
    {
        while (true)
        {
            // 영역들을 업데이트
            UpdatePlayerRect(playerObject.transform.position);
            UpdateMapRect();

            // 타일 리스트 업데이트
            AddToTileList();

            // 타일이 mapRect 바깥에 있으면 리스트에서 제거
            tiles.RemoveIf(t => t.x < mapRect.minX || t.x > mapRect.maxX || t.y < mapRect.minY || t.y > mapRect.maxY);

            // 머티리얼 적용
            tiles.ApplyMaterials(materials);

            // 그리기
            tiles.Show();

            //Debug.Log(tileList.Count);
            yield return new WaitForSeconds(updateTime);
        }
    }

    public void BuildNavMeshData()
    {
        navMeshSurface.BuildNavMesh();
    }

    public IEnumerator UpdateNavMeshData()
    {
        while(true)
        {
            navMeshSurface.UpdateNavMesh(navMeshSurface.navMeshData);
            yield return new WaitForSeconds(updateTime);
        }
    }


    /////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////////////////////////////////////

    void UpdatePlayerRect(Vector3 _position)
    {
        playerRect.minX = (_position.x) - range;
        playerRect.maxX = (_position.x) + range;
        playerRect.minY = (_position.z) - range;
        playerRect.maxY = (_position.z) + range;
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

    void AddToTileList()
    {
        // player Rect을 순회
        for (int i = Mathf.FloorToInt(playerRect.minY); i < Mathf.CeilToInt(playerRect.maxY); i++)
        {
            for (int j = Mathf.FloorToInt(playerRect.minX); j < Mathf.CeilToInt(playerRect.maxX); j++)
            {
                Tile tTile = tiles.GetTile(j, i);

                if (tTile == null)
                {
                    GameObject tObject = Object.Instantiate(tileObject, new Vector3(j, 0, i), tileObject.transform.rotation);
                    tTile = new Tile(j, i, tObject);

                    float perlinValue = Mathf.PerlinNoise(tTile.tile.transform.position.x * 0.1f + perlinSeed, tTile.tile.transform.position.z * 0.1f + perlinSeed); 
                    tTile.perlinValue = Mathf.Clamp01(perlinValue);

                    tiles.Add(tTile);
                }
            }
        }
    }
}
