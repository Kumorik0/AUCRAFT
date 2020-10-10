using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class TilemapOverflow : EditorWindow
{
    private Texture2D _tileset;
    private int _tilePadding = 2; //How much padding between the original tiles
    private int _tileSpacing = 2; //How much spacing between the original tiles
    private int _newTilePadding = 4; //How much padding between the tiles when exported
    private int _newTileSpacing = 4; //How much spacing between the tiles when exported
    private int _tileSize = 32; //The size of each tile
    private int _overFlow = 2; //How much overflow to give each tile
    private float _UIupperHeight = 195; //The vertical height of the Settings section
    private float _tabHeight = 20;
    private float _UIpadding = 5; //The padding from the edge of the window
    private int _hTiles; //The number of tiles placed vertically in the tileset
    private int _vTiles; //The number of tiles placed horizontaly in the tileset
    private Texture2D _previewTexture; //Temp texture for previewing
    private bool _isPixelArt;
    private bool _fillCorners;

    private Vector2 _scrollPos;
    private int _zoomLevel = 1;
    private float _scrollHeight, _scrollWidth;

    //Size of the editor window
    private static float __wWidth = 382, __wHeight = 550;

    private bool _imported = false;
    private Texture2D _previous;

    //Checker board background settings
    private Texture2D _checkerTexture;
    private bool _showChecker = true;
    private int _checkerDefaultSize = 8;    
    private int _checkerSize = 16;
    private int _checkerScale = 2;    
    private Color _checkerColorA = new Color(.8f, .8f, .8f);
    private Color _checkerColorB = new Color(.45f, .45f, .45f);

    //Curretn settings tab
    private int _currentTab = 0;


    [MenuItem("Window/Tileset Tools/Overflow Tool")]
    static void Init()
    {
        TilemapOverflow window = (TilemapOverflow)EditorWindow.GetWindow<TilemapOverflow>();
        window.titleContent = new GUIContent("Overflow Tool");
        window.minSize = new Vector2(__wWidth, __wHeight);
        window.maxSize = new Vector2(__wWidth, __wHeight);
        window._scrollWidth = __wWidth - window._UIpadding * 2;
        window._scrollHeight = __wHeight - window._UIupperHeight - window._UIpadding;
        window.SetupChecker();
    }

    public void OnGUI()
    {
        GUILayout.BeginArea(new Rect(_UIpadding, _UIpadding, (this.position.width - 10), _tabHeight));
        GUILayout.BeginHorizontal();
        GUI.color = _currentTab == 0 ? Color.green : Color.white;
        if(GUILayout.Button("Overflow", "tab first", GUILayout.MinWidth(70)))
        {
            _currentTab = 0;
        }
        
        GUI.color = _currentTab == 1 ? Color.green : Color.white;
        if (GUILayout.Button("Settings", "tab last", GUILayout.MinWidth(70)))
        {
            _currentTab = 1;
        }
        GUI.color = Color.white;
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.EndArea();
        if (_currentTab == 0)
        {
            DrawUI();
            DrawPreview();
        }else if(_currentTab == 1)
        {
            Rect settingRect = new Rect(_UIpadding, _tabHeight, (this.position.width - 10), (this.position.height - _tabHeight - _UIpadding));
            GUI.Box(settingRect, "", "Groupbox");

            GUI.Label(new Rect(settingRect.x+2, settingRect.y+2, 120, 16), "Checker Settings", "tab first");

            GUILayout.BeginArea(new Rect(settingRect.x + 2, settingRect.y + 18, settingRect.width - 4, 100));

            GUILayout.BeginVertical("Groupbox");

            GUILayout.BeginHorizontal();
            GUILayout.Label("Show Checker Background : ");
            _showChecker = EditorGUILayout.Toggle(_showChecker);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Checker Background Size : ");
            _checkerScale = EditorGUILayout.IntSlider(_checkerScale, 1, 6);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("First Color : ");
            _checkerColorA = EditorGUILayout.ColorField(_checkerColorA);

            GUILayout.Label("Second Color : ");
            _checkerColorB = EditorGUILayout.ColorField(_checkerColorB);
            GUILayout.EndHorizontal();

            if (GUILayout.Button("Apply Background Changes"))
            {
                switch (_checkerScale)
                {
                    case 1:
                        _checkerSize = _checkerDefaultSize;
                        break;
                    case 2:
                        _checkerSize = _checkerDefaultSize * 2;
                        break;
                    case 3:
                        _checkerSize = _checkerDefaultSize * 4;
                        break;
                    case 4:
                        _checkerSize = _checkerDefaultSize * 6;
                        break;
                    case 5:
                        _checkerSize = _checkerDefaultSize * 8;
                        break;
                    case 6:
                        _checkerSize = _checkerDefaultSize * 10;
                        break;
                }
                SetupChecker();
            }
            GUILayout.EndVertical();
            GUILayout.EndArea();

            GUI.Label(new Rect(settingRect.x + 2, settingRect.y + 118, 120, 16), "Other Settings", "tab first");

            GUILayout.BeginArea(new Rect(settingRect.x + 2, settingRect.y + 134, settingRect.width - 4, settingRect.height - 136));
            GUILayout.BeginVertical("Groupbox");
            EditorGUILayout.Space();
            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();            
            GUILayout.EndArea();
        }
    }

    public void SetupChecker()
    {
        Vector2 texSize = new Vector2(_scrollWidth, _scrollHeight);
        _checkerTexture = new Texture2D((int)texSize.x - 16, (int)texSize.y - 16);
        int cSize = _checkerSize * _checkerSize;
        Color[] greyCheck = new Color[cSize];
        Color[] dkGreyCheck = new Color[cSize];
        for (int i = 0; i < cSize; i++)
        {
            greyCheck[i] = _checkerColorA;
            dkGreyCheck[i] = _checkerColorB;
        }
        Color[] currentColor = dkGreyCheck;
        int tHeight = (int)Mathf.Ceil((_checkerTexture.height / (float)_checkerSize));
        int tWidth = (int)Mathf.Ceil((_checkerTexture.width / (float)_checkerSize));
        for (int y = 0; y < tHeight; y++)
        {
            for (int x = 0; x < tWidth; x++)
            {
                int _xx = _checkerSize * x;
                int _yy = _checkerSize * y;
                int xSize = _checkerSize;
                int ySize = _checkerSize;
                currentColor = greyCheck;
                if (_xx + xSize + 1 >= _checkerTexture.width)
                {

                    //Debug.Log("Width Exceeded! Tried position : " + (_xx + xSize) + ", Max Position : " + _checkerTexture.width + ". Difference of " + (_xx + xSize - _checkerTexture.width) + " pixels.");
                    xSize -= (_xx + xSize - _checkerTexture.width);
                }
                if (_yy + ySize >= _checkerTexture.height)
                {
                    //Debug.Log("Height Exceeded! Tried position : " + (_yy + ySize) + ", Max Position : " + _checkerTexture.height + ". Difference of " + (_yy + ySize - _checkerTexture.height) + " pixels.");
                    ySize -= (_yy + ySize - _checkerTexture.height);
                }
                if ((y % 2 == 0 && x % 2 == 0) || (y % 2 == 1 && x % 2 == 1))
                    currentColor = dkGreyCheck;    
                _checkerTexture.SetPixels(_xx, _yy, xSize, ySize, currentColor);
            }
        }
        //Flip the texture vertically so it looks nicer
        Texture2D tempTex = new Texture2D(_checkerTexture.width, _checkerTexture.height);
        for (int i = 0; i < _checkerTexture.height; i++)
        {
            tempTex.SetPixels(0, tempTex.height - 1 - i, tempTex.width - 1, 1, _checkerTexture.GetPixels(0, i, _checkerTexture.width - 1, 1));
        }
        tempTex.Apply();
        _checkerTexture = tempTex;

        _checkerTexture.filterMode = FilterMode.Point;
        _checkerTexture.Apply();
    }

    public void UpdatePreviewTexture()
    {
        int tsWidth = _tileset.width;
        int tsHeight = _tileset.height;
        _hTiles = (tsWidth - _tilePadding) / (_tileSize + _tileSpacing);
        _vTiles = (tsHeight - _tilePadding) / (_tileSize + _tileSpacing);

        _previewTexture = CreateTileset(_tileset, _tileset.width, _tileset.height);
        _previewTexture.Apply();
    }

    private void ApplyOverflowToTile(Texture2D texture, int tx, int ty)
    {
        Color[] colors;

        float _ox = _tilePadding + (_tileSpacing + _tileSize) * tx;
        float _oy = _tilePadding + (_tileSpacing + _tileSize) * ty;

        float _nx = _newTilePadding + (_newTileSpacing + _tileSize) * tx;
        float _ny = _newTilePadding + (_newTileSpacing + _tileSize) * ty;

        colors = GetTile(_tileset, (int)_ox, (int)_oy);
        texture.SetPixels((int)_nx, (int)_ny, _tileSize, _tileSize, colors);

        for (int _c = 1; _c <= _overFlow; _c++)
        {
            bool overBottomEdge = !(((int)_ny - _c) >= 0),
                overTopEdge = !(((int)_ny + _tileSize + (_c - 1)) < texture.height),
                overLeftEdge = !(((int)_nx - _c) >= 0),
                overRightEdge = !(((int)_nx + _tileSize + (_c - 1)) < texture.width);

            StretchEdges(texture, (int)_nx, (int)_ny, _c, overBottomEdge, overTopEdge, overLeftEdge, overRightEdge);
        }
    }

    private void StretchEdges(Texture2D texture, int _tx, int _ty, int _stretch, bool overBottomEdge, bool overTopEdge, bool overLeftEdge, bool overRightEdge)
    {
        Color[] colors;

        //Overflow top                    
        if (!overBottomEdge)
        {
            colors = GetPixels(texture, _tx, _ty, _tileSize, 1);
            texture.SetPixels(_tx, _ty - _stretch, _tileSize, 1, colors);
        }
        //Overflow bottom    
        if (!overTopEdge)
        {
            colors = GetPixels(texture, _tx, _ty + _tileSize - 1, _tileSize, 1);
            texture.SetPixels(_tx, _ty + _tileSize + (_stretch - 1), _tileSize, 1, colors);
        }

        //Overflow left
        if (!overLeftEdge)
        {
            colors = GetPixels(texture, _tx, _ty, 1, _tileSize);
            texture.SetPixels(_tx - _stretch, _ty, 1, _tileSize, colors);
        }

        //Overflow right
        if (!overRightEdge)
        {
            colors = GetPixels(texture, _tx + _tileSize - 1, _ty, 1, _tileSize);
            texture.SetPixels(_tx + _tileSize + (_stretch - 1), _ty, 1, _tileSize, colors);
        }

        if (_fillCorners && _stretch == _overFlow)
        {
            FillCorners(texture, _tx, _ty, _stretch, overBottomEdge, overTopEdge, overLeftEdge, overRightEdge);
        }
    }

    private void FillCorners(Texture2D texture, int _tx, int _ty, int _stretch, bool overBottomEdge, bool overTopEdge, bool overLeftEdge, bool overRightEdge)
    {
        Color[] colors;
        int size = _stretch * _stretch;
        colors = new Color[size];

        if (!overBottomEdge)
        {
            if (!overLeftEdge)
            {
                
                for (int i = 0; i < colors.Length; i++)
                    colors[i] = GetPixel(texture, _tx - 1, _ty);
                texture.SetPixels(_tx - _stretch, _ty - _stretch, _stretch, _stretch, colors);
            }
            if (!overRightEdge)
            {
                colors = new Color[size];
                for (int i = 0; i < colors.Length; i++)
                    colors[i] = GetPixel(texture, _tx + _tileSize, _ty);
                texture.SetPixels(_tx + _tileSize, _ty - _stretch, _stretch, _stretch, colors);
            }
        }
        
        if (!overTopEdge)
        {
            if (!overLeftEdge)
            {
                colors = new Color[size];
                for (int i = 0; i < colors.Length; i++)
                    colors[i] = GetPixel(texture, _tx, _ty + _tileSize - 1);
                texture.SetPixels(_tx - _stretch, _ty + _tileSize, _stretch, _stretch, colors);
            }
            if (!overRightEdge)
            {
                colors = new Color[size];
                for (int i = 0; i < colors.Length; i++)
                    colors[i] = GetPixel(texture, _tx + _tileSize - 1, _ty + _tileSize - 1);
                texture.SetPixels(_tx + _tileSize, _ty + _tileSize, _stretch, _stretch, colors);
            }
        }
    }

    private void DrawUI()
    {
        GUILayout.BeginArea(new Rect(5, _tabHeight, (this.position.width - 10), _UIupperHeight-_tabHeight), "", "Groupbox");
        EditorGUI.BeginChangeCheck();
        _tileset = EditorGUILayout.ObjectField(_tileset, typeof(Texture2D), false) as Texture2D;
        if (EditorGUI.EndChangeCheck())
        {

            if (_tileset == null)
            {
                _imported = false;
                _previewTexture = null;
            }
            if (_previous != null && _tileset != null)
            {
                if (_previous.GetPixels() != _tileset.GetPixels())
                {
                    _imported = false;
                    _previewTexture = null;
                }
            }
        }
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Padding : ");
        _tilePadding = EditorGUILayout.IntField(_tilePadding);

        GUILayout.Label("Spacing : ");
        _tileSpacing = EditorGUILayout.IntField(_tileSpacing);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        _isPixelArt = EditorGUILayout.Toggle(_isPixelArt, GUILayout.MaxWidth(20));
        GUILayout.Label("Is Pixel art  |");

        _fillCorners = EditorGUILayout.Toggle(_fillCorners, GUILayout.MaxWidth(20));
        GUILayout.Label("Fill Corners   |");

        GUILayout.Label("Tile Size : ");
        _tileSize = EditorGUILayout.IntField(_tileSize);
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button(!_imported ? "Import" : "Revert"))
        {
            if (_tileset == null)
                return;
            UpdatePreviewTexture();
            _imported = true;
            _previous = _tileset;
        }

        EditorGUI.BeginDisabledGroup(!_imported);

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("New Padding : ");
        _newTilePadding = EditorGUILayout.IntField(_newTilePadding);

        GUILayout.Label("New Spacing : ");
        EditorGUI.BeginChangeCheck();
        _newTileSpacing = EditorGUILayout.IntField(_newTileSpacing);
        if (EditorGUI.EndChangeCheck())
        {
            _newTileSpacing = (int)Mathf.Clamp(_newTileSpacing, 2, Mathf.Infinity);//if the tile spacing is less than 2 the overflow wont work
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Tile Overflow Amount : ");
        _overFlow = EditorGUILayout.IntSlider(_overFlow, 1, (int)Mathf.Floor(_newTileSpacing / 2)); //If the overflow is greater than half the spacing it will bleed into the next tile
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Zoom Level : ");
        _zoomLevel = EditorGUILayout.IntSlider(_zoomLevel, 1, 15);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Apply", "MiniButtonLeft"))
        {
            ApplyChangesToPreview();
        }

        if (GUILayout.Button("Export", "MiniButtonMid"))
        {
            Export(_previewTexture);
        }

        if (GUILayout.Button("Apply and Export", "MiniButtonRight"))
        {
            ApplyAndExport();
        }

        EditorGUILayout.EndHorizontal();

        EditorGUI.EndDisabledGroup();

        GUILayout.EndArea();
    }

    private void DrawPreview()
    {
        Rect outerRect = new Rect(_UIpadding, _UIupperHeight + _UIpadding, _scrollWidth-1, _scrollHeight-1);
        GUI.Box(outerRect, "", "Groupbox");
        if (_showChecker)
        {
            Rect pos = new Rect(outerRect.x + 1, outerRect.y + 1, _checkerTexture.width, _checkerTexture.height);
            GUI.DrawTexture(pos, _checkerTexture);
        }
        if (_previewTexture == null)
            return;

        Rect scrollRect = new Rect(_UIpadding + 1, _UIupperHeight + _UIpadding + 1, _scrollWidth - 3, _scrollHeight - 3);

        Rect viewRect = new Rect(0, 0, (_previewTexture.width) * _zoomLevel, (_previewTexture.height) * _zoomLevel);

        _scrollPos = GUI.BeginScrollView(scrollRect, _scrollPos, viewRect, true, true);
        GUI.DrawTexture(new Rect(0, 0, _previewTexture.width * _zoomLevel, _previewTexture.height * _zoomLevel), _previewTexture);
        GUI.EndScrollView();
    }

    private void ApplyAndExport()
    {
        ApplyChangesToPreview();
        Export(_previewTexture);
    }

    private void ApplyChangesToPreview()
    {
        float newWidth = _newTilePadding * 2 + _hTiles * (_tileSize + _newTileSpacing) - _newTileSpacing;
        float newHeight = _newTilePadding * 2 + _vTiles * (_tileSize + _newTileSpacing) - _newTileSpacing;

        Texture2D newTexture = new Texture2D((int)newWidth, (int)newHeight, TextureFormat.ARGB32, false);

        if (_isPixelArt)
            newTexture.filterMode = FilterMode.Point;

        //Fill the new texture with all transparent pixels
        Color[] colors = new Color[(int)(newWidth * newHeight)];
        for (int i = 0; i < colors.Length; i++)
        {
            colors[i] = new Color(0, 0, 0, 0);
        }
        newTexture.SetPixels(colors);

        //Create the new tileset and apply the overflow
        for (int _y = 0; _y < _vTiles; _y++)
        {
            for (int _x = 0; _x < _hTiles; _x++)
            {
                ApplyOverflowToTile(newTexture, _x, _y);
            }
        }

        //Save all changes
        newTexture.Apply(false);
        _previewTexture = CreateTileset(newTexture, (int)newWidth, (int)newHeight);
        _previewTexture.Apply(false);
    }

    private void Export(Texture2D texture)
    {
        //Save To Disk as PNG
        TextureExporter.ExportTexture(texture, "Exported Images/", _tileset.name + "_overflow", _tileSize);
    }

    private Texture2D CreateTileset(Texture2D tex, int width, int height)
    {
        Texture2D newTex = new Texture2D(width, height, TextureFormat.ARGB32, false);
        if (_isPixelArt)
            newTex.filterMode = FilterMode.Point;
        newTex.SetPixels(tex.GetPixels());
        newTex.Apply(false);

        return newTex;
    }

    private Color[] GetTile(Texture2D oTexture, int x, int y)
    {
        return GetPixels(oTexture, (int)x, (int)y, _tileSize, _tileSize);
    }

    private Color[] GetPixels(Texture2D oTexture, int x, int y, int width, int height)
    {
        return oTexture.GetPixels((int)x, (int)y, width, height);
    }

    private Color GetPixel(Texture2D oTexture, int x, int y)
    {
        return oTexture.GetPixel((int)x, (int)y);
    }
}