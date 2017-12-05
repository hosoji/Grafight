using Assets.WrappingRope.Scripts.Utils;
using UnityEditor;
using UnityEngine;

namespace Assets.WrappingRope.Scripts.Editors
{
    [CustomEditor(typeof(Rope))]
    public class RopeEditor : Editor
    {

        GUIContent duplicateButtonContent = new GUIContent("+", "Add point");
        GUIContent deleteButtonContent = new GUIContent("-", "Delete point");

        void AWake()
        {
            Debug.Log("awake");

            Shader shader = Shader.Find("Hidden/Internal-Colored");
            _lineMaterial = new Material(shader);
            _lineMaterial.hideFlags = HideFlags.HideAndDontSave;
            // Turn on alpha blending
            //sfe
            //_lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            //_lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            //// Turn backface culling off
            //_lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            //// Turn off depth writes
            //_lineMaterial.SetInt("_ZWrite", 0);
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
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_ignoreLayer"));
            ShowMeshConfiguration();
            serializedObject.ApplyModifiedProperties();
        }


        protected virtual void ShowMeshConfiguration()
        {
            var meshConfig = serializedObject.FindProperty("MeshConfiguration");
            EditorGUILayout.PropertyField(meshConfig);
            if (meshConfig.isExpanded)
            {
                EditorGUI.indentLevel += 1;
                EditorGUILayout.PropertyField(meshConfig.FindPropertyRelative("BendCrossectionsNumber"));
                var profile = meshConfig.FindPropertyRelative("Profile");
                ShowProfiler(profile);
                EditorGUI.indentLevel -= 1;
            }
        }


        protected virtual void ShowProfiler(SerializedProperty list)
        {
            if (!list.isArray)
                return;
            var size = list.arraySize;
            EditorGUILayout.PropertyField(list);
            if (list.isExpanded)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(32);

                EditorGUILayout.BeginVertical();

                if (GUILayout.Button(duplicateButtonContent))
                {
                    list.InsertArrayElementAtIndex(size);
                }
                if (GUILayout.Button(deleteButtonContent))
                {
                    if (size > 3)
                    {
                        list.DeleteArrayElementAtIndex(size - 1);
                    }
                }
                EditorGUILayout.EndVertical();

                if (size < 3)
                {
                    for (var i = 0; i < 3 - size; i++)
                    {
                        list.InsertArrayElementAtIndex(size);
                    }
                }

                var poligon = Geometry.CreatePolygon(list.arraySize, Axis.Z, 0.5f, 0);
                for (var i = 0; i < list.arraySize; i++)
                {
                    var prop = list.GetArrayElementAtIndex(i);
                    prop.vector3Value = poligon[i];
                }
                RefreshProfile(list);
                EditorGUILayout.EndHorizontal();
                EditorGUI.indentLevel -= 1;
            }
        }

        protected virtual void RefreshProfile(SerializedProperty list)
        {
            var oldTarget = RenderTexture.active; 
            var width = EditorGUIUtility.currentViewWidth;
            if (_prevWidth != width)
            {
                //RecreateTexture((int)width, 100);
                _prevWidth = width;
            }

            LineMaterial.SetPass(0);

            Graphics.SetRenderTarget(texture);
            GL.PushMatrix();
            GL.LoadOrtho();
           // GL.LoadPixelMatrix(0, 0.5f, 0, 0.5f);
            GL.Clear(true, true, new Color(0, 0, 0, 255));
            GL.Color(Color.red);
            GL.Begin(GL.LINES);
            var ratio = 1f;// 100 / width;// 200 / width;
            for (var i = 0; i < list.arraySize; i++)
            {
                var prop = list.GetArrayElementAtIndex(i);
                var vertex = prop.vector3Value;
                GL.Vertex3((vertex.x + 0.5f) * ratio, vertex.y + 0.5f, 0);

                prop = list.GetArrayElementAtIndex(i + 1 == list.arraySize ? 0 : i + 1);
                vertex = prop.vector3Value;
                GL.Vertex3((vertex.x + 0.5f) * ratio, vertex.y + 0.5f, 0);
            }
            GL.End();
            GL.PopMatrix();
            Graphics.SetRenderTarget(oldTarget);
            var layoutRect = GUILayoutUtility.GetRect(100, 100);
            Graphics.DrawTexture(new Rect(layoutRect.position, new Vector2(100, 100)), texture);
        }


        private void RecreateTexture(int width, int height)
        {
            texture = new RenderTexture(width, height, 16, RenderTextureFormat.ARGB32);
        }
        private float _prevWidth = 100;
        private RenderTexture texture = new RenderTexture(100, 100, 16, RenderTextureFormat.ARGB32);
        private Material _lineMaterial;

        private Material LineMaterial
        {
            get
            {
                if (_lineMaterial == null)
                    _lineMaterial = CreateMaterial();
                return _lineMaterial;
            }
        }

        private Material CreateMaterial()
        {
            Shader shader = Shader.Find("Hidden/Internal-Colored");
            var lineMaterial = new Material(shader);
            lineMaterial.hideFlags = HideFlags.HideAndDontSave;
            // Turn on alpha blending
            //sfe
            lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            // Turn backface culling off
            lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            // Turn off depth writes
            lineMaterial.SetInt("_ZWrite", 0);
            return lineMaterial;

        }

    }

    public class RopeStub : MonoBehaviour
    {

    }
}
