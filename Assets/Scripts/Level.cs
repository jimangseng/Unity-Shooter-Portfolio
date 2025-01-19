using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.AI.Navigation;
using UnityEngine;
using static Cinemachine.DocumentationSortingAttribute;

public class Level : MonoBehaviour
{
    [field: SerializeField] public GameObject PlayerObject { get; set; }
    [field: SerializeField] public GameObject TileObject { get; set; }
    [field: SerializeField] public GameObject ObstacleObject { get; set; }
    [field: SerializeField] public Material[] LevelMaterials { get; set; }
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
    public float perlinValue;

    public bool updated;
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

    readonly int materialCount = 5;

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

        foreach (Tile t in listToRemove)
        {
            UnityEngine.Object.Destroy(t.tile);
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
            int tIndex = Mathf.FloorToInt(t.perlinValue * materialCount);
            t.tile.GetComponent<MeshRenderer>().material = _mats[tIndex];
        }
    }

    public void Show()
    {
        foreach (var t in list)
        {
            //t.tile.transform.SetParent(t.tile.transform.parent);
            t.tile.SetActive(true);
        }

    }

    public void SetParent(GameObject _parent)
    {
        foreach(var t in list)
        {
            t.tile.transform.SetParent(_parent.transform);
        }
    }

    // 레벨 전체의 콜라이더를 생성하는 데에 사용한다. NavMesh 생성을 위해 필요하다.
    public void UpdateAABBCollider(GameObject _level)
    {
        Bounds totalBounds;

        Tile startingPoint = list.ElementAt(0);
        totalBounds = startingPoint.tile.GetComponent<MeshRenderer>().bounds;

        foreach (var t in list)
        {
            totalBounds.Encapsulate(t.tile.GetComponent<MeshRenderer>().bounds);
        }

        _level.GetComponent<BoxCollider>().center = totalBounds.center;
        _level.GetComponent<BoxCollider>().size = totalBounds.size;
    }


}
