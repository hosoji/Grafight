// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using AmplifyShaderEditor;

internal class ASEMaterialInspector : ShaderGUI
{
	private const string CopyButtonStr = "Copy Values";
	private const string PasteButtonStr = "Paste Values";
	private const string PreviewModelPref = "ASEMI_PREVIEWMODEL";

	private static MaterialEditor m_instance = null;

	private bool m_initialized = false;
	private double m_lastRenderedTime;
	private PreviewRenderUtility m_previewRenderUtility;
	private Mesh m_targetMesh;
	private Vector2 m_previewDir = new Vector2( 120f, -20f );
	private int m_selectedMesh = 0;


	// Reflection Fields
	private Type m_modelInspectorType = null;
	private MethodInfo m_renderMeshMethod = null;
	private Type m_previewGUIType = null;
	private MethodInfo m_dragMethod = null;
	private FieldInfo m_selectedField = null;
	private FieldInfo m_infoField = null;

	public override void OnGUI( MaterialEditor materialEditor, MaterialProperty[] properties )
	{
		IOUtils.Init();
		Material mat = materialEditor.target as Material;

		if( mat == null )
			return;

		m_instance = materialEditor;

		if( !m_initialized )
		{
			Init();
			m_initialized = true;
		}

		if( Event.current.type == EventType.repaint &&
			mat.HasProperty( IOUtils.DefaultASEDirtyCheckId ) &&
			mat.GetInt( IOUtils.DefaultASEDirtyCheckId ) == 1 )
		{
			mat.SetInt( IOUtils.DefaultASEDirtyCheckId, 0 );
			UIUtils.ForceUpdateFromMaterial();
#if !UNITY_5_5_OR_NEWER
			Event.current.Use();
#endif
		}



		if( materialEditor.isVisible )
		{
			GUILayout.BeginVertical();
			{
				GUILayout.Space( 3 );
				if( GUILayout.Button( "Open in Shader Editor" ) )
				{
					AmplifyShaderEditorWindow.LoadMaterialToASE( mat );
				}

				GUILayout.BeginHorizontal();
				{
					if( GUILayout.Button( CopyButtonStr ) )
					{
						Shader shader = mat.shader;
						int propertyCount = UnityEditor.ShaderUtil.GetPropertyCount( shader );
						string allProperties = string.Empty;
						for( int i = 0; i < propertyCount; i++ )
						{
							UnityEditor.ShaderUtil.ShaderPropertyType type = UnityEditor.ShaderUtil.GetPropertyType( shader, i );
							string name = UnityEditor.ShaderUtil.GetPropertyName( shader, i );
							string valueStr = string.Empty;
							switch( type )
							{
								case UnityEditor.ShaderUtil.ShaderPropertyType.Color:
								{
									Color value = mat.GetColor( name );
									valueStr = value.r.ToString() + IOUtils.VECTOR_SEPARATOR +
												value.g.ToString() + IOUtils.VECTOR_SEPARATOR +
												value.b.ToString() + IOUtils.VECTOR_SEPARATOR +
												value.a.ToString();
								}
								break;
								case UnityEditor.ShaderUtil.ShaderPropertyType.Vector:
								{
									Vector4 value = mat.GetVector( name );
									valueStr = value.x.ToString() + IOUtils.VECTOR_SEPARATOR +
												value.y.ToString() + IOUtils.VECTOR_SEPARATOR +
												value.z.ToString() + IOUtils.VECTOR_SEPARATOR +
												value.w.ToString();
								}
								break;
								case UnityEditor.ShaderUtil.ShaderPropertyType.Float:
								{
									float value = mat.GetFloat( name );
									valueStr = value.ToString();
								}
								break;
								case UnityEditor.ShaderUtil.ShaderPropertyType.Range:
								{
									float value = mat.GetFloat( name );
									valueStr = value.ToString();
								}
								break;
								case UnityEditor.ShaderUtil.ShaderPropertyType.TexEnv:
								{
									Texture value = mat.GetTexture( name );
									valueStr = AssetDatabase.GetAssetPath( value );
								}
								break;
							}

							allProperties += name + IOUtils.FIELD_SEPARATOR + type + IOUtils.FIELD_SEPARATOR + valueStr;

							if( i < ( propertyCount - 1 ) )
							{
								allProperties += IOUtils.LINE_TERMINATOR;
							}
						}
						EditorPrefs.SetString( IOUtils.MAT_CLIPBOARD_ID, allProperties );
					}

					if( GUILayout.Button( PasteButtonStr ) )
					{
						string propertiesStr = EditorPrefs.GetString( IOUtils.MAT_CLIPBOARD_ID, string.Empty );
						if( !string.IsNullOrEmpty( propertiesStr ) )
						{
							string[] propertyArr = propertiesStr.Split( IOUtils.LINE_TERMINATOR );
							bool validData = true;
							try
							{
								for( int i = 0; i < propertyArr.Length; i++ )
								{
									string[] valuesArr = propertyArr[ i ].Split( IOUtils.FIELD_SEPARATOR );
									if( valuesArr.Length != 3 )
									{
										Debug.LogWarning( "Material clipboard data is corrupted" );
										validData = false;
										break;
									}
									else if( mat.HasProperty( valuesArr[ 0 ] ) )
									{
										UnityEditor.ShaderUtil.ShaderPropertyType type = (UnityEditor.ShaderUtil.ShaderPropertyType)Enum.Parse( typeof( UnityEditor.ShaderUtil.ShaderPropertyType ), valuesArr[ 1 ] );
										switch( type )
										{
											case UnityEditor.ShaderUtil.ShaderPropertyType.Color:
											{
												string[] colorVals = valuesArr[ 2 ].Split( IOUtils.VECTOR_SEPARATOR );
												if( colorVals.Length != 4 )
												{
													Debug.LogWarning( "Material clipboard data is corrupted" );
													validData = false;
													break;
												}
												else
												{
													mat.SetColor( valuesArr[ 0 ], new Color( Convert.ToSingle( colorVals[ 0 ] ),
																								Convert.ToSingle( colorVals[ 1 ] ),
																								Convert.ToSingle( colorVals[ 2 ] ),
																								Convert.ToSingle( colorVals[ 3 ] ) ) );
												}
											}
											break;
											case UnityEditor.ShaderUtil.ShaderPropertyType.Vector:
											{
												string[] vectorVals = valuesArr[ 2 ].Split( IOUtils.VECTOR_SEPARATOR );
												if( vectorVals.Length != 4 )
												{
													Debug.LogWarning( "Material clipboard data is corrupted" );
													validData = false;
													break;
												}
												else
												{
													mat.SetVector( valuesArr[ 0 ], new Vector4( Convert.ToSingle( vectorVals[ 0 ] ),
																								Convert.ToSingle( vectorVals[ 1 ] ),
																								Convert.ToSingle( vectorVals[ 2 ] ),
																								Convert.ToSingle( vectorVals[ 3 ] ) ) );
												}
											}
											break;
											case UnityEditor.ShaderUtil.ShaderPropertyType.Float:
											{
												mat.SetFloat( valuesArr[ 0 ], Convert.ToSingle( valuesArr[ 2 ] ) );
											}
											break;
											case UnityEditor.ShaderUtil.ShaderPropertyType.Range:
											{
												mat.SetFloat( valuesArr[ 0 ], Convert.ToSingle( valuesArr[ 2 ] ) );
											}
											break;
											case UnityEditor.ShaderUtil.ShaderPropertyType.TexEnv:
											{
												mat.SetTexture( valuesArr[ 0 ], AssetDatabase.LoadAssetAtPath<Texture>( valuesArr[ 2 ] ) );
											}
											break;
										}
									}
								}
							}
							catch( Exception e )
							{
								Debug.LogException( e );
								validData = false;
							}


							if( validData )
							{
								materialEditor.PropertiesChanged();
								UIUtils.CopyValuesFromMaterial( mat );
							}
							else
							{
								EditorPrefs.SetString( IOUtils.MAT_CLIPBOARD_ID, string.Empty );
							}
						}
					}
				}
				GUILayout.EndHorizontal();
				GUILayout.Space( 5 );
			}
			GUILayout.EndVertical();
		}
		EditorGUI.BeginChangeCheck();
		//base.OnGUI( materialEditor, properties );

		// Draw custom properties instead of calling BASE to use single line texture properties
		materialEditor.SetDefaultGUIWidths();

		if( m_infoField == null )
		{
			m_infoField = typeof( MaterialEditor ).GetField( "m_InfoMessage", BindingFlags.Instance | BindingFlags.NonPublic );
		}

		string info = m_infoField.GetValue( materialEditor ) as string;
		if( !string.IsNullOrEmpty( info ) )
		{
			EditorGUILayout.HelpBox( info, MessageType.Info );
		}
		else
		{
			GUIUtility.GetControlID( "EditorTextField".GetHashCode(), FocusType.Passive, new Rect( 0f, 0f, 0f, 0f ) );
		}
		for( int i = 0; i < properties.Length; i++ )
		{
			if( ( properties[ i ].flags & ( MaterialProperty.PropFlags.HideInInspector | MaterialProperty.PropFlags.PerRendererData ) ) == MaterialProperty.PropFlags.None )
			{
				if( ( properties[ i ].flags & MaterialProperty.PropFlags.NoScaleOffset ) == MaterialProperty.PropFlags.NoScaleOffset )
				{
					materialEditor.TexturePropertySingleLine( new GUIContent( properties[ i ].displayName ), properties[ i ] );
				}
				else
				{
					float propertyHeight = materialEditor.GetPropertyHeight( properties[ i ], properties[ i ].displayName );
					Rect controlRect = EditorGUILayout.GetControlRect( true, propertyHeight, EditorStyles.layerMaskField, new GUILayoutOption[ 0 ] );
					materialEditor.ShaderProperty( controlRect, properties[ i ], properties[ i ].displayName );
				}
			}
		}
#if UNITY_5_5_2 || UNITY_5_5_3 || UNITY_5_5_4 || UNITY_5_5_5 || UNITY_5_6_OR_NEWER
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		materialEditor.RenderQueueField();
#endif
#if UNITY_5_6_OR_NEWER
		materialEditor.EnableInstancingField();
#endif
#if UNITY_5_6_2 || UNITY_5_6_3 || UNITY_5_6_4 || UNITY_2017_1_OR_NEWER
		materialEditor.DoubleSidedGIField();
#endif
		materialEditor.LightmapEmissionProperty();
		if( EditorGUI.EndChangeCheck() )
		{
			string isEmissive = mat.GetTag( "IsEmissive", false, "false" );
			if( isEmissive.Equals( "true" ) )
			{
				mat.globalIlluminationFlags &= (MaterialGlobalIlluminationFlags)3;
			}
			else
			{
				mat.globalIlluminationFlags |= MaterialGlobalIlluminationFlags.EmissiveIsBlack;
			}

			UIUtils.CopyValuesFromMaterial( mat );
		}

		if( materialEditor.RequiresConstantRepaint() && m_lastRenderedTime + 0.032999999821186066 < EditorApplication.timeSinceStartup )
		{
			this.m_lastRenderedTime = EditorApplication.timeSinceStartup;
			materialEditor.Repaint();
		}
	}

	private void Init()
	{
		string guid = EditorPrefs.GetString( PreviewModelPref, "" );
		if( !string.IsNullOrEmpty( guid ) )
		{
			m_targetMesh = AssetDatabase.LoadAssetAtPath<Mesh>( AssetDatabase.GUIDToAssetPath( guid ) );
		}
	}

	public override void OnMaterialPreviewSettingsGUI( MaterialEditor materialEditor )
	{

		base.OnMaterialPreviewSettingsGUI( materialEditor );

		if( ShaderUtil.hardwareSupportsRectRenderTexture )
		{
			EditorGUI.BeginChangeCheck();
			m_targetMesh = (Mesh)EditorGUILayout.ObjectField( m_targetMesh, typeof( Mesh ), false, GUILayout.MaxWidth( 120 ) );
			if( EditorGUI.EndChangeCheck() )
			{
				if( m_targetMesh != null )
				{
					EditorPrefs.SetString( PreviewModelPref, AssetDatabase.AssetPathToGUID( AssetDatabase.GetAssetPath( m_targetMesh ) ) );
				}
				else
				{
					EditorPrefs.SetString( PreviewModelPref, "" );
				}
			}

			if( m_selectedField == null )
			{
				m_selectedField = typeof( MaterialEditor ).GetField( "m_SelectedMesh", BindingFlags.Instance | BindingFlags.NonPublic );
			}

			m_selectedMesh = (int)m_selectedField.GetValue( materialEditor );

			if( m_selectedMesh != 0 )
			{
				if( m_targetMesh != null )
				{
					m_targetMesh = null;
					EditorPrefs.SetString( PreviewModelPref, "" );
				}
			}
		}
	}

	public override void OnMaterialInteractivePreviewGUI( MaterialEditor materialEditor, Rect r, GUIStyle background )
	{
		if( Event.current.type == EventType.DragExited )
		{
			if( DragAndDrop.objectReferences.Length > 0 )
			{
				GameObject dropped = DragAndDrop.objectReferences[ 0 ] as GameObject;
				if( dropped != null )
				{
					m_targetMesh = AssetDatabase.LoadAssetAtPath<Mesh>( AssetDatabase.GetAssetPath( dropped ) );
					EditorPrefs.SetString( PreviewModelPref, AssetDatabase.AssetPathToGUID( AssetDatabase.GetAssetPath( m_targetMesh ) ) );
				}
			}
		}

		if( m_targetMesh == null )
		{
			base.OnMaterialInteractivePreviewGUI( materialEditor, r, background );
			return;
		}

		Material mat = materialEditor.target as Material;

		if( m_previewRenderUtility == null )
		{
			m_previewRenderUtility = new PreviewRenderUtility();
			m_previewRenderUtility.m_CameraFieldOfView = 30f;
		}

		if( m_previewGUIType == null )
		{
			m_previewGUIType = Type.GetType( "PreviewGUI, UnityEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null" );
			m_dragMethod = m_previewGUIType.GetMethod( "Drag2D", BindingFlags.Static | BindingFlags.Public );
		}

		if( m_modelInspectorType == null )
		{
			m_modelInspectorType = Type.GetType( "UnityEditor.ModelInspector, UnityEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null" );
			m_renderMeshMethod = m_modelInspectorType.GetMethod( "RenderMeshPreview", BindingFlags.Static | BindingFlags.NonPublic );
		}

		m_previewDir = (Vector2)m_dragMethod.Invoke( m_previewGUIType, new object[] { m_previewDir, r } );

		if( Event.current.type == EventType.Repaint )
		{
			m_previewRenderUtility.BeginPreview( r, background );
			m_renderMeshMethod.Invoke( m_modelInspectorType, new object[] { m_targetMesh, m_previewRenderUtility, mat, null, m_previewDir, -1 } );
			m_previewRenderUtility.EndAndDrawPreview( r );
		}
	}

	public static MaterialEditor Instance { get { return m_instance; } set { m_instance = value; } }
}
