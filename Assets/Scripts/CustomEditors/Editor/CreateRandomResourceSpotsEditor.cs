using UnityEditor;
using UnityEngine;
using System.Linq;

namespace GameStudio.HunterGatherer.Resources
{

    /// <summary>Editor script for spawning resources in editor without starting the scene</summary>
    [CustomEditor(typeof(CreateRandomResourceSpots))]
    public class CreateRandomResourceSpotsEditor : Editor
    {
        private const float pixelSpace = 10;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            GUILayout.Space(pixelSpace);

            CreateRandomResourceSpots script = (CreateRandomResourceSpots)target;
            if (GUILayout.Button("Distribute resources"))
            {
                script.DestroyEditorSpawns();
                script.SpawnResourcesInEditor();
            }

            if (GUILayout.Button("Redo ground attachment after edit"))
            {
                script.RedoGroundAttachmentAfterEdit();
            }

            GUILayout.Space(pixelSpace * 0.5f);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Convert To Spawn Points"))
            {
                script.ConvertToSpawnPoints();
            }

            if (GUILayout.Button("Convert From Spawn Points"))
            {
                script.ConvertFromSpawnPoints();
            }
            GUILayout.EndHorizontal();

            if (GUILayout.Button("Destroy Spawns"))
            {
                script.DestroyEditorSpawns();
            }
        }
    }
}
