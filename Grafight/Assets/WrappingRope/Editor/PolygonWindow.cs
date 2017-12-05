using UnityEngine;
using System.Collections;
using UnityEditor;
using Assets.WrappingRope.Contracts;
using Assets.WrappingRope.Contracts.Events;
using System;
using System.Collections.Generic;
using Assets.WrappingRope.Scripts.Utils;
using Assets.WrappingRope.Scripts;
using System.Linq;

public class PolygonEditor : EditorWindow, IPolygonView
{

    //public static PolygonEditor polygonWindow;
    private Rope _rope;
    private List<Vector2> _polygon;
    private List<Symbol> _symbols;
    private GUIContent _makeRegularButtonContent;
    private GUIContent _insertPointButtonContent;
    private GUIContent _deletePointButtonContent;
    private bool _isInsertPointOn;
    private bool _isDeletePointOn;
    private RenderTexture _texture;
    private Rect _currentLayout;
    private float _zoom;
    private PolygonController _controller;
    private SerializedObject _serializedObject;
    private SerializedProperty _profile;

    private RenderTexture texture
    {
        get
        {
            if (_texture == null)
            {
                _texture = new RenderTexture(1000, 1000, 16, RenderTextureFormat.ARGB32);
            }
            return _texture;
        }
    }

    public List<Vector2> Polygon { get { return _polygon; } }

    public List<Symbol> Symbols { get { return _symbols; } }


    public float Height
    {
        get
        {
            return position.height;
        }
    }


    public float Width
    {
        get
        {
            return position.width;
        }
    }

    public float Zoom { get { return _zoom; } }

    public event Mouse MouseDown;
    public event Mouse MouseMove;
    public event Mouse MouseUp;
    public event Action<bool> InsertPointChanged;
    public event Action<bool> DeletePointChanged;
    public event Action MakeRegular;


    [MenuItem("Window/Polygon Editor %e")]
    public static void Init()
    {
        PolygonEditor polygonWindow = GetWindow<PolygonEditor>(false, "Polygon Editor", true);
        polygonWindow.maxSize = new Vector2(1000, 1000);
        polygonWindow.Show();
        polygonWindow.Populate();
    }


    void OnFocus() { Populate(); }
    void OnSelectionChange()
    {
        Populate();
        Repaint();
    }
    void OnEnable()
    {
        Initialize();
        Populate();
    }


    void Initialize()
    {
        _makeRegularButtonContent = new GUIContent("Make Regular", "Make Regular");
        _insertPointButtonContent = new GUIContent("Insert points", "Insert points");
        _deletePointButtonContent = new GUIContent("Delete points", "Delete points");
        _zoom = 100;
        _isInsertPointOn = false;
        _isDeletePointOn = false;
        _symbols = new List<Symbol>();
        wantsMouseMove = true;
    }
 

    void OnGUI()
    {
        if (_polygon == null)
        {
            GUILayout.TextArea("Please, select a game object with the Rope component for edit profile in this window");
            return;
        }
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button(_makeRegularButtonContent))
        {
            OnMakeRegular();
        }

        if (GUILayout.Toggle(_isInsertPointOn, _insertPointButtonContent))
        {
            if (!_isInsertPointOn)
            {
                _isInsertPointOn = true;
                _isDeletePointOn = false;
                OnInsertPointChanged(true);
                //Debug.Log("On");
            }
        }
        else
        {
            if (_isInsertPointOn)
            {
                _isInsertPointOn = false;
                OnInsertPointChanged(false);
                //Debug.Log("Off");
            }
        }

        if (GUILayout.Toggle(_isDeletePointOn, _deletePointButtonContent))
        {
            if (!_isDeletePointOn)
            {
                _isDeletePointOn = true;
                _isInsertPointOn = false;
                OnDeletePointChanged(true);
                //Debug.Log("On");
            }
        }
        else
        {
            if (_isDeletePointOn)
            {
                _isDeletePointOn = false;
                OnDeletePointChanged(false);
                //Debug.Log("Off");
            }
        }

        EditorGUILayout.EndHorizontal();
        _currentLayout = GUILayoutUtility.GetRect(1000, 1000);
        var evnt = Event.current;
        var spacePosition = MousePositionToSpace(evnt.mousePosition);
        if (evnt.type == EventType.MouseDown)
        {
            OnMouseDown(spacePosition);
        }
        if (evnt.type == EventType.mouseUp)
        {

            OnMouseUp(spacePosition);
        }

        if (evnt.type == EventType.mouseDrag)
        {
            OnMouseMove(spacePosition);
        }
        if (evnt.type == EventType.mouseMove)
        {
            OnMouseMove(spacePosition);
        }

        Vector2 delta = Vector2.zero;
        if (evnt.type == EventType.ScrollWheel)
        {
            delta = evnt.delta;
            _zoom -= delta.y * 1f;
            if (_zoom < 1)
                _zoom = 1;
        }
        VisualizeAll();
        CopyPolygonToProfile();
        _serializedObject.ApplyModifiedProperties();
    }

    private void VisualizeAll()
    {
        var strokeList = new List<Stroke>();
        // Чертим "поля"
        strokeList.Add(new Stroke { Start = new Vector2(-0.5f, 0.5f), End = new Vector2(0.5f, 0.5f), Color = new Color(0.2f, 0.2f, 0.2f, 1) });
        strokeList.Add(new Stroke { Start = new Vector2(-0.5f, -0.5f), End = new Vector2(0.5f, -0.5f), Color = new Color(0.2f, 0.2f, 0.2f, 1) });
        strokeList.Add(new Stroke { Start = new Vector2(-0.5f, 0.5f), End = new Vector2(-0.5f, -0.5f), Color = new Color(0.2f, 0.2f, 0.2f, 1) });
        strokeList.Add(new Stroke { Start = new Vector2(0.5f, 0.5f), End = new Vector2(0.5f, -0.5f), Color = new Color(0.2f, 0.2f, 0.2f, 1) });
        // Чертим сам полигон
        strokeList.AddRange(PolygonToStrokeList(_polygon, new Color(100, 100, 0, 100)));
        // Чертим точки полигона
        GetSymbolsForPolygonPoints().ForEach(symbol => strokeList.AddRange(symbol.GetStrokeList(GetColorForSymbol(symbol), 10f / _zoom)));
        // Чертим вспомогательные символы
        _symbols.ForEach(symbol => strokeList.AddRange(symbol.GetStrokeList(10f / _zoom)));
        Draw(strokeList);
    }

    private List<Symbol> GetSymbolsForPolygonPoints()
    {
        var list = new List<Symbol>();
        _polygon.ForEach(point => list.Add(new Dot(point)));
        return list;

    }

    private Color GetColorForSymbol(Symbol symbol)
    {
        switch(symbol.GetType().Name)
        {
            case "Dot": return new Color(0, 1, 0, 1);
            case "Cross": return new Color(1, 0, 0, 1);
            default: return new Color(0, 1, 0, 1);
        }
    }

    protected void SetPolygon()
    {
        if (_serializedObject == null)
            return;
        var meshConfig = _serializedObject.FindProperty("MeshConfiguration");
        if (meshConfig == null)
            return;
        var profile = meshConfig.FindPropertyRelative("Profile");
        if (profile == null)
            return;
        _profile = profile;
        _polygon = GetPolygonFromProfile(profile);
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


    private void CopyPolygonToProfile()
    {
        for (var i = 0; i < _polygon.Count; i++)
        {
            if (_profile.arraySize < i + 1)
                _profile.InsertArrayElementAtIndex(i);
            var prop = _profile.GetArrayElementAtIndex(i);
            prop.vector3Value = _polygon[i];
        }
        var differ = _profile.arraySize - _polygon.Count;
        if (differ > 0)
        {
            for (var i = 0; i < differ; i++)
            {
                _profile.DeleteArrayElementAtIndex(_polygon.Count);
            }
        }
    }

    private void Draw(List<Stroke> strokeList)
    {
        var xPos = (texture.width - position.width) / texture.width / 2;
        var yPos = (texture.height - position.height + _currentLayout.y) / texture.height / 2;
        var chamber = new Rect(xPos, yPos, position.width / texture.width, (position.height - _currentLayout.y) / texture.height);
        Plotter.Render(texture, strokeList, _zoom);
        Graphics.DrawTexture(new Rect(_currentLayout.position, new Vector2(position.width, position.height - _currentLayout.y)), texture, chamber, 0, 0, 0, 0);
        Repaint();
    }

    private List<Stroke> PolygonToStrokeList(List<Vector2> polygon, Color color)
    {
        var strokeList = new List<Stroke>();
        for(var i = 0; i < polygon.Count; i++)
        {
            var nextIndex = i == polygon.Count - 1 ? 0 : i + 1;
            strokeList.Add(new Stroke { Color = color, Start = polygon[i], End = polygon[nextIndex] });
        }
        return strokeList;

    }

    void OnMouseDown(Vector2 position)
    {
        if (MouseDown != null)
        {
            MouseDown(position);
        }
    }

    void OnMouseUp(Vector2 position)
    {
        if (MouseUp != null)
        {
            MouseUp(position);
        }
    }

    void OnMouseMove(Vector2 position)
    {
        if (MouseMove != null)
        {
            MouseMove(position);
        }
    }


    void OnMakeRegular()
    {
        if (MakeRegular != null)
        {
            MakeRegular();
        }
    }


    void OnInsertPointChanged(bool isOn)
    {
        if (InsertPointChanged != null)
        {
            InsertPointChanged(isOn);
        }
    }


    void OnDeletePointChanged(bool isOn)
    {
        if (DeletePointChanged != null)
        {
            DeletePointChanged(isOn);
        }
    }

    Vector2 MousePositionToSpace(Vector2 pos)
    {
        var viewPortHeight = position.height - _currentLayout.y;
        //var aspect = position.width / viewPortHeight;
        //float widthConstraint = position.width < viewPortHeight ? 1 : aspect;
        //float heightConstraint = widthConstraint == 1 ? 1 / aspect : 1;
        return new Vector2(((position.width) / -2 + pos.x) / _zoom , ((viewPortHeight) / 2 - pos.y + _currentLayout.y) / _zoom );

    }

    void Populate()
    {
        UnityEngine.Object[] selection = Selection.GetFiltered(typeof(Rope), SelectionMode.Assets);
        if (selection.Length > 0)
        {
            if (selection[0] != null)
            {
                _rope = (Rope)selection[0];
                _serializedObject = new SerializedObject(_rope);
                SetPolygon();
                _symbols.Clear();
                if (_polygon != null)
                {
                    if (_controller == null)
                        _controller = new PolygonController(this);
                    else
                        _controller.SetPolygon();
                    return;
                }
            }
        }
        Reset();
    }

    private void Reset()
    {
        _rope = null;
        _serializedObject = null;
        _profile = null;
        _polygon = null;
    }
}
