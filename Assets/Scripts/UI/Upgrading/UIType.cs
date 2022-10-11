using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GameStudio.HunterGatherer.UI
{

    [CreateAssetMenu(menuName = "Upgrade UI/UI Type", fileName = "UIType")]
    public class UIType : ScriptableObject
    {
        public GameObject prefabUI;
        public int poolSize = 1;
        public string script;

        [HideInInspector]
        public Object scriptObj;
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(UIType))]
    public class UITypeEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var prefabUI = serializedObject.FindProperty("prefabUI");
            var poolSize = serializedObject.FindProperty("poolSize");

            EditorGUILayout.PropertyField(prefabUI, new GUIContent("UI Prefab"));
            EditorGUILayout.PropertyField(poolSize, new GUIContent("Pool Size"));

            (target as UIType).scriptObj = EditorGUILayout.ObjectField((target as UIType).scriptObj, typeof(MonoScript), false);

            if ((target as UIType).scriptObj)
            {
                (target as UIType).script = ((target as UIType).scriptObj as MonoScript).GetClass().FullName;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}