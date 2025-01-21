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

    readonly float updateLevelInterval = 1.0f;

    /// Methodes

    private void Start()
    {

    }

    private void Update()
    {
        
    }

    public IEnumerator UpdateLevel()
    {
        while (true)
        {
            // 영역 업데이트
            level.UpdateArea();

            // 업데이트 리스트에 타일 추가 및 범위를 벗어난 타일 제거
            level.AddTileToUpdateList();
            level.RemoveOutdatedTile();

            // 새로 만들어진 타일만을 별도의 리스트에 넣어 관리한다
            // 매 루프마다 1600개의 타일을 셋팅할 필요가 없다
            if (level.listToUpdate.Count > 0)
            {
                //UnityEngine.Debug.Log(level.listToUpdate.Count);
                // 장애물 추가
                level.CreateObstacles();

                // 머티리얼 적용
                level.ApplyMaterials(level.LevelMaterials);

                // 리스트를 업데이트한다
                level.RenewList();

                // NavMesh 데이터를 업데이트
                // NavMesh는 장애물이 전부 생성된 이후에 만들어져야하므로 리스트 업데이트 이후에 와야 한다
                level.UpdateNavMeshData();
            }

            yield return new WaitForSeconds(updateLevelInterval);
        }
    }

    public void SetLevel(Level _level)
    {
        level = _level;
    }
}