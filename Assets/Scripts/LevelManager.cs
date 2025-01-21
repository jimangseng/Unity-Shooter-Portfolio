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
            // ì—­ …ë°´íŠ¸
            level.UpdateArea();

            // …ë°´íŠ¸ ë¦¬ìŠ¤¸ì— €ì¶”ê ë°ë²”ìœ„ë¥ë²—ì–´€œê±°
            level.AddTileToUpdateList();
            level.RemoveOutdatedTile();

            // ˆë¡œ ë§Œë“¤´ì§„ €¼ë§Œë³„ë„ë¦¬ìŠ¤¸ì— £ì–´ ê´€ë¦¬í•œ            // ë§ë£¨í”„ë§ˆë‹¤ 1600ê°œì˜ €¼ì„ ‹íŒ…„ìš”ê°€ †ë‹¤
            if (level.listToUpdate.Count > 0)
            {
                //UnityEngine.Debug.Log(level.listToUpdate.Count);

                // ¥ì• ë¬ì¶”ê
                level.CreateObstacles();

                // ë¨¸í‹°ë¦¬ì–¼ ìš©
                level.ApplyMaterials(level.LevelMaterials);

                // ë¦¬ìŠ¤¸ë …ë°´íŠ¸œë‹¤
                level.RenewList();

                // NavMesh °ì´°ë …ë°´íŠ¸
                // NavMesh¥ì• ë¬¼ì´ „ë ì„±´í›„ë§Œë“¤´ì ¸¼í•˜ë¯€ë¡ë¦¬ìŠ¤…ë°´íŠ¸ ´í›„€œë‹¤
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