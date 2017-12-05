using Assets.WrappingRope.Contracts.Events;
using Assets.WrappingRope.Scripts.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Assets.WrappingRope.Scripts.Editors
{
    [CustomEditor(typeof(Rope))]
    public class RopeEditor : Editor
    {

        private Rope _rope;
        private bool isPlolygonWindowVisible;
        private RenderTexture _texture;


        void OnEnable()
        {
            _texture = new RenderTexture(1000, 100, 16, RenderTextureFormat.ARGB32);
            _rope = (Rope)target;
            if (_rope == null)
                return;
        }

        void OnDisable()
        {
            //print("script was removed");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_frontEnd"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_backEnd"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("Threshold"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("WrapDistance"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_pieceInstance"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("BendInstance"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_width"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("extendAxis"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("TexturingMode"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("UVLocation"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_anchoringMode"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("ElasticModulus"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("IgnoreLayer"));

            ShowMeshConfiguration();
            serializedObject.ApplyModifiedProperties();
        }


        protected virtual void ShowMeshConfiguration()
        {
            var meshConfig = serializedObject.FindProperty("MeshConfiguration");
            if (meshConfig == null)
                return;
            EditorGUILayout.PropertyField(meshConfig);
            if (meshConfig.isExpanded)
            {
                EditorGUI.indentLevel += 1;
                EditorGUILayout.PropertyField(meshConfig.FindPropertyRelative("BendCrossectionsNumber"));
                EditorGUILayout.PropertyField(meshConfig.FindPropertyRelative("FlipNormals"));
                var profile = meshConfig.FindPropertyRelative("Profile");
                if (profile == null)
                    return;
                ShowProfilePreview(profile);
                EditorGUI.indentLevel -= 1;
            }
        }


        protected virtual void ShowProfilePreview(SerializedProperty profile)
        {
            if (!profile.isArray)
                return;
            EditorGUILayout.PropertyField(profile);
            if (profile.isExpanded)
            {
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.BeginVertical();

                EditorGUILayout.EndVertical();
                var polygon = GetPolygonFromProfile(profile);
                DrawPreview(polygon);
                EditorGUILayout.EndHorizontal();
                EditorGUI.indentLevel -= 1;
            }
        }


        private List<Vector2> GetPolygonFromProfile(SerializedProperty profile)
        {
            var polygon = new List<Vector2>();
            for (var i = 0; i < profile.arraySize; i++)
            {
                var prop = profile.GetArrayElementAtIndex(i);
                polygon.Add(new Vector2(prop.vector3Value.x, prop.vector3Value.y));
            }
            return polygon;
        }


        protected virtual void DrawPreview(List<Vector2> polygon)
        {
            var layoutRect = GUILayoutUtility.GetRect(100, 100);
            Plotter.Render(_texture, polygon, 100f);
            Graphics.DrawTexture(new Rect(layoutRect.position, new Vector2(layoutRect.width, 100)), _texture, new Rect(((1000- layoutRect.width)) / 2 / 1000, 0, layoutRect.width / 1000, 1), 0, 0, 0, 0);
        }
    }

    public class RopeStub : MonoBehaviour
    {

    }
}
