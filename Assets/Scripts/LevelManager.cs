using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NavMeshSurface = Unity.AI.Navigation.NavMeshSurface;
using Random = UnityEngine.Random;
using Object = UnityEngine.Object;

using System;
using UnityEditor;

public class LevelData
{
    public GameObject tileObject;
    public GameObject obstacleObject;
    public Material[] mats;
    public NavMeshSurface surface;

    public LevelData(GameObject _tileObject, GameObject _obstaclceObject, Material[] _mats, NavMeshSurface _surface)
    {
        tileObject = _tileObject;
        obstacleObject = _obstaclceObject;
        mats = _mats;
        surface = _surface;
    }
}

public class LevelManager
{
    /// Internal Classese
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
        public float perlinValue;
    }

    public class Rect
    {
        public Rect(float _minX, float _maxX, float _minY, float _maxY)
        {
            minX = _minX;
            maxX = _maxX;
            minY = _minY;
            maxY = _maxY;
        }

        public float minX = 0;
        public float maxX = 0;
        public float minY = 0;
        public float maxY = 0;
    }

    public class TileList
    {
        public List<Tile> list;
        List<Tile> listToRemove;

        public TileList()
        {
            list = new List<Tile>();
            listToRemove = new List<Tile>();
        }

        public Tile GetTile(int _x, int _y)
        {
            //해당 위치에 타일이 있는지 검사
            Tile tTileFound = list.Find(t => (t.x == _x) && (t.y == _y));

            if (tTileFound == null)
            {
                return null;
            }
            else
            {
                return tTileFound;
            }
        }

        public void Add(Tile _tile)
        {
            list.Add(_tile);
        }

        public void RemoveAt(int _x, int _y)
        {
            Tile tTileFound = list.Find(t => (t.x == _x) && (t.y == _y));

            list.Remove(tTileFound);
        }

        public void RemoveIf(Predicate<Tile> match)
        {
            listToRemove = list.FindAll(match);

            foreach(Tile t in listToRemove)
            {
                Object.Destroy(t.tile);
                list.Remove(t);
            }
        }

        public List<Tile> GetList()
        {
            return list;
        }
        public void ApplyMaterials(Material[] _mats)
        {
            foreach (var t in list)
            {
                int tIndex = Mathf.FloorToInt(t.perlinValue * (5.0f));
                t.tile.GetComponent<MeshRenderer>().material = _mats[tIndex];
            }
        }

        public void Show()
        {
            foreach (var t in list)
            {
                t.tile.SetActive(true);
            }
        }
    }


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
    float updateTime = 2.0f;

    Rect playerRect;
    Rect mapRect;

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

            navMeshSurface.BuildNavMesh();

            yield return new WaitForSeconds(updateTime);
        }
    }

    public IEnumerator CreateObstacles()
    {
        while (true)
        {
            GameObject obstacleTile;


            Vector3 obstaclePosition = new Vector3(
                Random.Range(playerObject.transform.position.x - 7, playerObject.transform.position.x + 7),
                0.5f,
                Random.Range(playerObject.transform.position.z - 7, playerObject.transform.position.z + 7));

            GameObject t = Object.Instantiate(obstacleObject, obstaclePosition, obstacleObject.transform.rotation);

            float height = Mathf.PerlinNoise(Random.Range(0.0f, 10.0f), Random.Range(0.0f, 10.0f));
            int h = Mathf.RoundToInt(Mathf.Lerp(1, 4, height));

            for (int i = 0; i < h; i++)
            {
                obstacleTile = Object.Instantiate(t.transform.GetChild(0).gameObject, new Vector3(0.0f, i, 0.0f), t.transform.rotation);
                obstacleTile.transform.SetParent(t.transform, false);

            }

            t.SetActive(true);

            yield return new WaitForSeconds(3.0f);
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
