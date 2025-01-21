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
            // �역 �데�트
            level.UpdateArea();

            // �데�트 리스�에 �추� �범위�벗어��거
            level.AddTileToUpdateList();
            level.RemoveOutdatedTile();

            // �로 만들�진 ��만별도리스�에 �어 관리한            // �루프마다 1600개의 ��을 �팅�요가 �다
            if (level.listToUpdate.Count > 0)
            {
                //UnityEngine.Debug.Log(level.listToUpdate.Count);

                // �애�추�
                level.CreateObstacles();

                // 머티리얼 �용
                level.ApplyMaterials(level.LevelMaterials);

                // 리스�� �데�트�다
                level.RenewList();

                // NavMesh �이�� �데�트
                // NavMesh�애물이 �� �성�후만들�져�하므�리스�데�트 �후��다
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