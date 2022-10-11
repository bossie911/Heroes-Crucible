using System;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace GameStudio.HunterGatherer.GameTime
{

    [CustomEditor(typeof(GameTimeManager))]
    public class GameTimeEditor : Editor
    {
        private SerializedProperty timeEvents;
        private SerializedProperty roundTimes;
        private ReorderableList list;
        private GameTimeManager gameTimeManager;

        private void OnEnable()
        {
            gameTimeManager = (GameTimeManager)target;
            timeEvents = serializedObject.FindProperty("timeEvents");
            roundTimes = serializedObject.FindProperty("roundTimes");

            list = new ReorderableList(serializedObject, timeEvents)
            {
                draggable = true,
                displayAdd = true,
                displayRemove = true,
                drawHeaderCallback = rect =>
                {
                    EditorGUI.LabelField(rect, "Events");
                },
                drawElementCallback = (rect, index, sel, act) =>
                {
                    var element = timeEvents.GetArrayElementAtIndex(index);
                    
                    var unityEvent = element.FindPropertyRelative("onTimeMatched");
                    var second = element.FindPropertyRelative("second");
                    
                    EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), second);

                    rect.y += EditorGUIUtility.singleLineHeight;
                    rect.y += EditorGUIUtility.singleLineHeight;

                    EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUI.GetPropertyHeight(unityEvent)), unityEvent);
                },
                elementHeightCallback = index =>
                {
                    var element = timeEvents.GetArrayElementAtIndex(index);
                    var unityEvent = element.FindPropertyRelative("onTimeMatched");

                    var height = EditorGUI.GetPropertyHeight(unityEvent) + EditorGUIUtility.singleLineHeight * 2;
                    return height;
                }
            };
        }
        
        public override void OnInspectorGUI()
        {
            DrawScriptField();
            DrawSettingsFields();
            DrawListFields();
        }

        ///<summary> This method will draw the field of where the script is going to be </summary>
        private void DrawScriptField()
        {
            // Disable editing
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour(gameTimeManager), typeof(GameTimeManager), false);
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.Space();
        }

        ///<summary> This method will draw the fields of the variables of the DayNight script you can set in inspector</summary>
        private void DrawSettingsFields()
        {
            EditorGUILayout.LabelField("Amount of rounds and how many seconds they last.", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Keep track of how many levels the map has,", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("adding more rounds then nessecary is not useful and complicates things.", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(roundTimes);

            EditorGUILayout.Space();
            serializedObject.ApplyModifiedProperties();
        }

        ///<summary> This method will draw and update the reordable eventlist </summary>
        private void DrawListFields()
        {
            serializedObject.Update();
            list.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
        }
    }
}