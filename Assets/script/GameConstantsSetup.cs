// using UnityEngine;
// using UnityEditor;

// #if UNITY_EDITOR
// [System.Serializable]
// public class GameConstantsSetup : MonoBehaviour
// {
//     [Header("Setup Tool")]
//     [Tooltip("Drag your GameConstants asset here, then click 'Assign to All Scripts'")]
//     public GameConstants gameConstants;

//     [Space(10)]
//     [Header("Info")]
//     [TextArea(3, 5)]
//     public string instructions = "1. Drag your GameConstants asset to the field above\n2. Click 'Assign GameConstants to All Scripts' button in the inspector\n3. This will automatically assign the GameConstants to all compatible scripts in the scene";

//     [ContextMenu("Assign GameConstants to All Scripts")]
//     public void AssignGameConstantsToAllScripts()
//     {
//         if (gameConstants == null)
//         {
//             Debug.LogError("Please assign GameConstants asset first!");
//             return;
//         }

//         int assignedCount = 0;

//         // Find all scripts that need GameConstants
//         PlayerMovement[] players = FindObjectsOfType<PlayerMovement>();
//         foreach (var player in players)
//         {
//             player.gameConstants = gameConstants;
//             assignedCount++;
//             EditorUtility.SetDirty(player);
//         }

//         EnemyMovement[] enemies = FindObjectsOfType<EnemyMovement>();
//         foreach (var enemy in enemies)
//         {
//             enemy.gameConstants = gameConstants;
//             assignedCount++;
//             EditorUtility.SetDirty(enemy);
//         }

//         Fireball[] fireballs = FindObjectsOfType<Fireball>();
//         foreach (var fireball in fireballs)
//         {
//             fireball.gameConstants = gameConstants;
//             assignedCount++;
//             EditorUtility.SetDirty(fireball);
//         }

//         Boss[] bosses = FindObjectsOfType<Boss>();
//         foreach (var boss in bosses)
//         {
//             boss.gameConstants = gameConstants;
//             assignedCount++;
//             EditorUtility.SetDirty(boss);
//         }

//         GameManager[] gameManagers = FindObjectsOfType<GameManager>();
//         foreach (var gm in gameManagers)
//         {
//             gm.gameConstants = gameConstants;
//             assignedCount++;
//             EditorUtility.SetDirty(gm);
//         }

//         SpecialSkillAttack[] skills = FindObjectsOfType<SpecialSkillAttack>();
//         foreach (var skill in skills)
//         {
//             skill.gameConstants = gameConstants;
//             assignedCount++;
//             EditorUtility.SetDirty(skill);
//         }

//         PowerUpBox[] powerBoxes = FindObjectsOfType<PowerUpBox>();
//         foreach (var box in powerBoxes)
//         {
//             box.gameConstants = gameConstants;
//             assignedCount++;
//             EditorUtility.SetDirty(box);
//         }

//         QnsBox[] qnsBoxes = FindObjectsOfType<QnsBox>();
//         foreach (var box in qnsBoxes)
//         {
//             box.gameConstants = gameConstants;
//             assignedCount++;
//             EditorUtility.SetDirty(box);
//         }

//         Debug.Log($"GameConstants assigned to {assignedCount} scripts successfully!");
//     }
// }

// // Custom Editor to add a button
// [CustomEditor(typeof(GameConstantsSetup))]
// public class GameConstantsSetupEditor : Editor
// {
//     public override void OnInspectorGUI()
//     {
//         DrawDefaultInspector();

//         GUILayout.Space(10);

//         GameConstantsSetup setup = (GameConstantsSetup)target;

//         GUI.backgroundColor = Color.green;
//         if (GUILayout.Button("Assign GameConstants to All Scripts", GUILayout.Height(30)))
//         {
//             setup.AssignGameConstantsToAllScripts();
//         }
//         GUI.backgroundColor = Color.white;

//         GUILayout.Space(10);

//         EditorGUILayout.HelpBox(
//             "This tool will automatically find all scripts in the scene that use GameConstants and assign the selected GameConstants asset to them.", 
//             MessageType.Info
//         );
//     }
// }
// #endif