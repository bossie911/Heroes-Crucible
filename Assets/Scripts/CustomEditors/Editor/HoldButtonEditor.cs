using UnityEditor;

namespace GameStudio.HunterGatherer.UI
{
#if UNITY_EDITOR
    /// <summary> Custom Unity inspector editor. Displays the new features of HoldButton such as OnHeldDownFinished </summary>
    [CustomEditor(typeof(HoldButton))]
    public class HoldButtonEditor : UnityEditor.UI.ButtonEditor
    {
        SerializedProperty holdDownDuration;
        SerializedProperty onHeldDown;
        SerializedProperty onHeldDownFinished;

        protected override void OnEnable()
        {
            base.OnEnable();

            holdDownDuration = serializedObject.FindProperty("holdDownDuration");
            onHeldDown = serializedObject.FindProperty("onHeldDown");
            onHeldDownFinished = serializedObject.FindProperty("onHeldDownFinished");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();
            EditorGUILayout.PropertyField(holdDownDuration);
            EditorGUILayout.PropertyField(onHeldDown);
            EditorGUILayout.PropertyField(onHeldDownFinished);
            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}
