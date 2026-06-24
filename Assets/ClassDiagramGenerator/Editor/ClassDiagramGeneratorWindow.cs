using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ClassDiagramGenerator.Application;
using ClassDiagramGenerator.Infrastructure.Export;
using ClassDiagramGenerator.Infrastructure.Generation;
using ClassDiagramGenerator.Infrastructure.Import;
using ClassDiagramGenerator.Infrastructure.Parsing;
using ClassDiagramGenerator.Infrastructure.Scanning;
using ClassDiagramGenerator.Infrastructure.Utilities;
using UnityEditor;
using UnityEngine;

namespace ClassDiagramGenerator
{
    public sealed class ClassDiagramGeneratorWindow : EditorWindow
    {
        private const string DocumentationUrl = "https://julestools.gitbook.io/julestools-docs/documentation/tools/class-diagram-generator";
        private const float BackgroundOverlayAlpha = 0.26f;

        private static readonly Color ColorBackground = new(0.12f, 0.12f, 0.14f, 1f);
        private static readonly Color ColorPanel = new(0.16f, 0.17f, 0.20f, 1f);
        private static readonly Color ColorPanelAlt = new(0.20f, 0.21f, 0.24f, 1f);
        private static readonly Color ColorBorder = new(0.30f, 0.32f, 0.36f, 1f);
        private static readonly Color ColorText = new(0.94f, 0.94f, 0.97f, 1f);
        private static readonly Color ColorTextSub = new(0.78f, 0.80f, 0.84f, 1f);
        private static readonly Color ColorRowEven = new(1f, 1f, 1f, 0.03f);
        private static readonly Color AccentViolet = new(0.43f, 0.34f, 0.68f, 1f);
        private static readonly Color AccentBlue = new(0.36f, 0.62f, 0.97f, 1f);
        private static readonly Color AccentVioletHover = new(0.54f, 0.45f, 0.80f, 1f);
        private static readonly Color AccentBlueHover = new(0.52f, 0.74f, 1.00f, 1f);

        private readonly ScriptSelectionManager _scriptSelectionManager = new();

        private DiagramExportFormat _exportFormat = DiagramExportFormat.PlantUmlFile;
        private string _lastDiagramUrl = string.Empty;
        private string _outputPath = "Assets/ClassDiagramGeneratorOutput/ClassDiagram.puml";
        private string _status = "Ready.";
        private string _scriptSearch = string.Empty;
        private bool _scriptsScanned;
        private bool _includeAssociations = true;

        private DefaultAsset _importDiagramAsset;
        private string _skeletonOutputPath = "Assets/ClassDiagramGeneratorOutput/GeneratedScripts";
        private bool _skipExistingFiles = true;

        private Vector2 _outerScroll;
        private Vector2 _scriptScroll;
        private DefaultAsset _scanTarget;
        private Texture2D _backgroundTexture;
        private Texture2D _panelTexture;
        private Texture2D _panelTextureAlt;
        private GUIStyle _titleStyle;
        private GUIStyle _subtitleStyle;
        private GUIStyle _cardHeaderStyle;
        private GUIStyle _cardBodyStyle;
        private GUIStyle _directoryStyle;

        [MenuItem("Tools/Diagram Generator")]
        private static void ShowWindow()
        {
            var window = GetWindow<ClassDiagramGeneratorWindow>("Diagram Generator");
            window.minSize = new Vector2(680f, 560f);
        }

        private void OnEnable()
        {
            _backgroundTexture = Resources.Load<Texture2D>("settings_bg");
            if (_backgroundTexture == null)
                _backgroundTexture = Resources.Load<Texture2D>("inspector_bg");

            TryAutoScan();
        }

        private void OnGUI()
        {
            EnsureThemeAssets();

            DrawBackground();
            DrawHeader();
            DrawToolbar();

            _outerScroll = EditorGUILayout.BeginScrollView(_outerScroll);
            DrawCard("Selection", DrawSelectionCard);
            DrawCard("Export Options", DrawExportCard);
            DrawCard("Import Diagram / Generate Skeleton", DrawImportCard);
            DrawCard("Advanced", DrawAdvancedCard);
            EditorGUILayout.EndScrollView();

            DrawFooter();
        }

        private void TryAutoScan()
        {
            string[] preferredFolders =
            {
                "Assets/Scripts",
                "Assets/_Project",
                "Assets"
            };

            foreach (string folder in preferredFolders)
            {
                if (!AssetDatabase.IsValidFolder(folder))
                    continue;

                _scriptSelectionManager.Scan(folder);

                if (_scriptSelectionManager.Scripts.Count <= 0)
                    continue;

                _scriptsScanned = true;
                _status = $"Scanned: {_scriptSelectionManager.Scripts.Count} files under '{folder}'.";
                return;
            }

            _scriptsScanned = false;
            _status = "Select a folder or drop .cs files to begin.";
        }

        private void EnsureThemeAssets()
        {
            if (Event.current == null)
                return;

            if (_panelTexture == null)
                _panelTexture = MakeTexture(2, 2, ColorPanel);

            if (_panelTextureAlt == null)
                _panelTextureAlt = MakeTexture(2, 2, ColorPanelAlt);

            if (_titleStyle == null)
            {
                _titleStyle = new GUIStyle(EditorStyles.boldLabel)
                {
                    alignment = TextAnchor.MiddleLeft,
                    fontSize = 14,
                    richText = true
                };
                _titleStyle.normal.textColor = ColorText;
            }

            if (_subtitleStyle == null)
            {
                _subtitleStyle = new GUIStyle(EditorStyles.miniLabel)
                {
                    alignment = TextAnchor.MiddleLeft
                };
                _subtitleStyle.normal.textColor = ColorTextSub;
            }

            if (_cardHeaderStyle == null)
            {
                _cardHeaderStyle = new GUIStyle(EditorStyles.boldLabel)
                {
                    alignment = TextAnchor.MiddleLeft,
                    fontSize = 12
                };
                _cardHeaderStyle.normal.textColor = ColorText;
            }

            if (_cardBodyStyle == null)
            {
                _cardBodyStyle = new GUIStyle(GUI.skin.box)
                {
                    padding = new RectOffset(12, 12, 10, 12),
                    margin = new RectOffset(0, 0, 0, 0)
                };
                _cardBodyStyle.normal.background = _panelTextureAlt;
            }

            if (_directoryStyle == null)
            {
                _directoryStyle = new GUIStyle(EditorStyles.miniLabel);
                _directoryStyle.normal.textColor = new Color(0.76f, 0.78f, 0.82f, 1f);
            }
        }

        private void DrawBackground()
        {
            var rect = new Rect(0f, 0f, position.width, position.height);
            if (_backgroundTexture != null)
            {
                GUI.DrawTexture(rect, _backgroundTexture, ScaleMode.ScaleAndCrop, true);
                EditorGUI.DrawRect(rect, new Color(0f, 0f, 0f, BackgroundOverlayAlpha));
            }
            else
            {
                EditorGUI.DrawRect(rect, ColorBackground);
            }
        }

        private void DrawHeader()
        {
            GUILayout.Space(6f);
            var icon = Resources.Load<Texture2D>("Icon 160x160 - Diagram Generator");

            using (new EditorGUILayout.HorizontalScope())
            {
                if (icon != null)
                    GUILayout.Label(icon, GUILayout.Width(40f), GUILayout.Height(40f));

                using (new EditorGUILayout.VerticalScope())
                {
                    GUILayout.Space(2f);
                    EditorGUILayout.LabelField("Class Diagram Generator", _titleStyle);
                    EditorGUILayout.LabelField("Generate UML (PlantUML) and import diagrams into script skeletons", _subtitleStyle);
                }
            }

            var line = GUILayoutUtility.GetRect(1f, 1f, GUILayout.ExpandWidth(true));
            EditorGUI.DrawRect(line, ColorBorder);
            GUILayout.Space(2f);
        }

        private void DrawToolbar()
        {
            using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                DrawFormatTabs();
                GUILayout.FlexibleSpace();

                if (ToolbarSecondary("Docs"))
                    OpenDocumentation();

                GUILayout.Space(4f);

                if (ToolbarPrimary("Generate Diagram"))
                    GenerateDiagram();
            }

            GUILayout.Space(4f);
        }

        private void DrawSelectionCard()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                _scanTarget = (DefaultAsset)EditorGUILayout.ObjectField(
                    new GUIContent("Folder / .cs", "Choose a folder or a .cs file"),
                    _scanTarget,
                    typeof(DefaultAsset),
                    false,
                    GUILayout.ExpandWidth(true),
                    GUILayout.MinWidth(200f));

                GUILayout.Space(6f);

                if (SecondaryButton("Scan", 20f))
                    ScanSelectionTarget();

                if (MiniButton("Scan Assets", 88f))
                    ScanFolder("Assets");

                GUILayout.FlexibleSpace();
            }

            GUILayout.Space(6f);

            var dropZone = GUILayoutUtility.GetRect(1f, 64f, GUILayout.ExpandWidth(true));
            EditorGUI.DrawRect(dropZone, ColorPanel);
            EditorGUI.DrawRect(new Rect(dropZone.x, dropZone.y, dropZone.width, 1f), ColorBorder);
            EditorGUI.DrawRect(new Rect(dropZone.x, dropZone.yMax - 1f, dropZone.width, 1f), ColorBorder);

            var dropLabelStyle = new GUIStyle(EditorStyles.centeredGreyMiniLabel) { fontSize = 11 };
            dropLabelStyle.normal.textColor = ColorTextSub;
            GUI.Label(dropZone, "Drop a folder to scan it • Drop .cs files to add them", dropLabelStyle);

            HandleDragAndDrop(dropZone);

            GUILayout.Space(8f);

            using (new EditorGUILayout.HorizontalScope())
            {
                _scriptSearch = EditorGUILayout.TextField(new GUIContent("Search", "Filter by file name"), _scriptSearch);

                if (MiniButton("All", 56f))
                {
                    foreach (var script in _scriptSelectionManager.Scripts)
                        script.IsSelected = true;
                }

                if (MiniButton("None", 56f))
                {
                    foreach (var script in _scriptSelectionManager.Scripts)
                        script.IsSelected = false;
                }

                if (MiniButton("Invert", 64f))
                {
                    for (int index = 0; index < _scriptSelectionManager.Scripts.Count; index++)
                        _scriptSelectionManager.Scripts[index].IsSelected = !_scriptSelectionManager.Scripts[index].IsSelected;
                }
            }

            GUILayout.Space(4f);

            using (new EditorGUILayout.HorizontalScope())
            {
                int total = _scriptSelectionManager.Scripts.Count;
                int selected = _scriptSelectionManager.Scripts.Count(script => script.IsSelected);

                GUILayout.Label($"Total: {total}", EditorStyles.miniBoldLabel, GUILayout.Width(90f));
                GUILayout.Label($"Selected: {selected}", EditorStyles.miniBoldLabel, GUILayout.Width(110f));
                GUILayout.FlexibleSpace();
            }

            var filtered = string.IsNullOrWhiteSpace(_scriptSearch)
                ? _scriptSelectionManager.Scripts
                : _scriptSelectionManager.Scripts
                    .Where(script => script.FileName.IndexOf(_scriptSearch, StringComparison.OrdinalIgnoreCase) >= 0)
                    .ToList();

            _scriptScroll = EditorGUILayout.BeginScrollView(_scriptScroll, GUILayout.Height(240f));

            if (filtered.Count == 0)
            {
                EditorGUILayout.HelpBox("Use Scan or drop content above to populate the list.", MessageType.Info);
            }
            else
            {
                for (int index = 0; index < filtered.Count; index++)
                {
                    var script = filtered[index];
                    var row = GUILayoutUtility.GetRect(1f, 20f, GUILayout.ExpandWidth(true));

                    if (index % 2 == 0)
                        EditorGUI.DrawRect(row, ColorRowEven);

                    var toggleRect = new Rect(row.x + 6f, row.y + 2f, 20f, row.height - 2f);
                    script.IsSelected = EditorGUI.Toggle(toggleRect, script.IsSelected);

                    var nameRect = new Rect(toggleRect.xMax + 4f, row.y + 1f, 320f, row.height - 2f);
                    var nameStyle = new GUIStyle(EditorStyles.label);
                    nameStyle.normal.textColor = ColorText;
                    EditorGUI.LabelField(nameRect, script.FileName, nameStyle);

                    var pathRect = new Rect(nameRect.xMax + 8f, row.y + 1f, row.width - nameRect.width - 40f, row.height - 2f);
                    if (!string.IsNullOrEmpty(script.Path))
                    {
                        EditorGUIUtility.AddCursorRect(pathRect, MouseCursor.Link);
                        EditorGUI.LabelField(pathRect, script.Path, _directoryStyle);

                        if (Event.current.type == EventType.MouseDown && pathRect.Contains(Event.current.mousePosition))
                        {
                            var asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(script.Path);
                            if (asset != null)
                            {
                                Selection.activeObject = asset;
                                EditorGUIUtility.PingObject(asset);
                            }
                        }
                    }
                }
            }

            EditorGUILayout.EndScrollView();
        }

        private void DrawExportCard()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Label("Output (.puml)", GUILayout.Width(120f));
                _outputPath = EditorGUILayout.TextField(_outputPath);

                if (MiniButton("…", 28f))
                {
                    string path = EditorUtility.SaveFilePanelInProject(
                        "Save PlantUML file",
                        "ClassDiagram",
                        "puml",
                        "Choose where to save the PlantUML file.");

                    if (!string.IsNullOrEmpty(path))
                        _outputPath = path;
                }
            }

            GUILayout.Space(8f);

            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();
                if (PrimaryButton("Generate Diagram", 42f))
                    GenerateDiagram();
                GUILayout.FlexibleSpace();
            }

            if (_exportFormat == DiagramExportFormat.PlantUmlUrl && !string.IsNullOrEmpty(_lastDiagramUrl))
            {
                GUILayout.Space(6f);
                EditorGUILayout.SelectableLabel(_lastDiagramUrl, EditorStyles.textField, GUILayout.Height(20f));

                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.FlexibleSpace();
                    if (SecondaryButton("Copy URL"))
                        EditorGUIUtility.systemCopyBuffer = _lastDiagramUrl;
                    GUILayout.FlexibleSpace();
                }
            }
        }

        private void DrawImportCard()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                _importDiagramAsset = (DefaultAsset)EditorGUILayout.ObjectField(
                    new GUIContent("Diagram (.puml)", "Select a PlantUML diagram file"),
                    _importDiagramAsset,
                    typeof(DefaultAsset),
                    false,
                    GUILayout.ExpandWidth(true));

                if (MiniButton("…", 28f))
                {
                    string path = EditorUtility.OpenFilePanel("Select PlantUML file", UnityEngine.Application.dataPath, "puml");
                    if (!string.IsNullOrWhiteSpace(path) && path.StartsWith(UnityEngine.Application.dataPath))
                    {
                        string relativePath = "Assets" + path.Substring(UnityEngine.Application.dataPath.Length).Replace('\\', '/');
                        _importDiagramAsset = AssetDatabase.LoadAssetAtPath<DefaultAsset>(relativePath);
                    }
                }
            }

            GUILayout.Space(6f);

            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Label("Scripts Output", GUILayout.Width(120f));
                _skeletonOutputPath = EditorGUILayout.TextField(_skeletonOutputPath);

                if (MiniButton("…", 28f))
                {
                    string selectedFolder = EditorUtility.OpenFolderPanel("Select output folder", UnityEngine.Application.dataPath, string.Empty);
                    if (!string.IsNullOrWhiteSpace(selectedFolder) && selectedFolder.StartsWith(UnityEngine.Application.dataPath))
                        _skeletonOutputPath = "Assets" + selectedFolder.Substring(UnityEngine.Application.dataPath.Length).Replace('\\', '/');
                }
            }

            GUILayout.Space(6f);

            _skipExistingFiles = EditorGUILayout.ToggleLeft(
                new GUIContent("Skip existing files (recommended)"),
                _skipExistingFiles);

            GUILayout.Space(8f);

            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();
                if (PrimaryButton("Import Diagram & Generate Skeleton", 42f))
                    ImportDiagramAndGenerateSkeleton();
                GUILayout.FlexibleSpace();
            }
        }

        private void DrawAdvancedCard()
        {
            _includeAssociations = EditorGUILayout.ToggleLeft(
                new GUIContent("Include associations (fields/parameters of other classes)"),
                _includeAssociations);

            GUILayout.Space(2f);
        }

        private void DrawFooter()
        {
            var line = GUILayoutUtility.GetRect(1f, 1f, GUILayout.ExpandWidth(true));
            EditorGUI.DrawRect(line, ColorBorder);

            var box = new GUIStyle(EditorStyles.helpBox)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 12
            };
            box.normal.textColor = ColorText;

            var rect = GUILayoutUtility.GetRect(1f, 32f, GUILayout.ExpandWidth(true));
            if (Event.current.type == EventType.Repaint)
                GUI.DrawTexture(rect, _panelTexture);

            GUI.Label(rect, string.IsNullOrEmpty(_status) ? "Ready." : _status, box);

            var signatureStyle = new GUIStyle(EditorStyles.centeredGreyMiniLabel)
            {
                alignment = TextAnchor.LowerCenter,
                fontStyle = FontStyle.Italic
            };
            signatureStyle.normal.textColor = ColorTextSub;

            GUILayout.Label("© 2026 ClassDiagramGenerator • 2.3.1", signatureStyle);
        }

        private void DrawCard(string title, Action body)
        {
            var header = GUILayoutUtility.GetRect(1f, 24f, GUILayout.ExpandWidth(true));
            if (Event.current.type == EventType.Repaint)
                GUI.DrawTexture(header, _panelTexture);

            GUI.Label(new Rect(header.x + 10f, header.y, header.width - 20f, header.height), title, _cardHeaderStyle);
            EditorGUI.DrawRect(new Rect(header.x, header.yMax - 1f, header.width, 1f), ColorBorder);

            EditorGUILayout.BeginVertical(_cardBodyStyle);
            try
            {
                body?.Invoke();
            }
            finally
            {
                EditorGUILayout.EndVertical();
            }

            GUILayout.Space(10f);
        }

        private void DrawFormatTabs()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                for (int index = 0; index < 2; index++)
                {
                    bool active = (int)_exportFormat == index;
                    string label = index == 0 ? "File (.puml)" : "URL";

                    var content = new GUIContent(label);
                    var rect = GUILayoutUtility.GetRect(content, EditorStyles.toolbarButton, GUILayout.Width(110f), GUILayout.Height(20f));

                    EditorGUI.DrawRect(rect, active ? AccentBlue : new Color(0f, 0f, 0f, 0f));

                    bool hover = rect.Contains(Event.current.mousePosition);
                    if (active)
                    {
                        var underline = new Rect(rect.x, rect.yMax - 2f, rect.width, 2f);
                        EditorGUI.DrawRect(underline, hover ? AccentBlueHover : AccentViolet);
                    }
                    else if (hover)
                    {
                        var outline = new Rect(rect.x, rect.yMax - 1f, rect.width, 1f);
                        EditorGUI.DrawRect(outline, ColorBorder);
                    }

                    var style = new GUIStyle(EditorStyles.toolbarButton)
                    {
                        fontStyle = active ? FontStyle.Bold : FontStyle.Normal,
                        alignment = TextAnchor.MiddleCenter
                    };
                    style.normal.textColor = active ? Color.white : ColorTextSub;

                    if (GUI.Button(rect, content, style))
                        _exportFormat = (DiagramExportFormat)index;

                    GUILayout.Space(2f);
                }
            }
        }

        private bool ToolbarPrimary(string label)
        {
            var rect = GUILayoutUtility.GetRect(new GUIContent(label), EditorStyles.toolbarButton, GUILayout.Width(120f));
            bool hover = rect.Contains(Event.current.mousePosition);
            EditorGUI.DrawRect(rect, hover ? AccentBlueHover : AccentBlue);

            var style = new GUIStyle(EditorStyles.toolbarButton) { fontStyle = FontStyle.Bold };
            style.normal.textColor = Color.white;
            return GUI.Button(rect, label, style);
        }

        private bool ToolbarSecondary(string label)
        {
            var rect = GUILayoutUtility.GetRect(new GUIContent(label), EditorStyles.toolbarButton, GUILayout.Width(72f));
            bool hover = rect.Contains(Event.current.mousePosition);
            EditorGUI.DrawRect(rect, hover ? ColorPanelAlt : ColorPanel);

            var style = new GUIStyle(EditorStyles.toolbarButton)
            {
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };
            style.normal.textColor = ColorText;

            return GUI.Button(rect, label, style);
        }

        private bool PrimaryButton(string label, float height)
        {
            var rect = GUILayoutUtility.GetRect(
                new GUIContent(label),
                EditorStyles.miniButton,
                GUILayout.Height(height),
                GUILayout.MinWidth(280f),
                GUILayout.ExpandWidth(false));

            bool hover = rect.Contains(Event.current.mousePosition);

            var top = new Rect(rect.x, rect.y, rect.width, Mathf.Round(rect.height * 0.5f));
            EditorGUI.DrawRect(top, hover ? AccentBlueHover : AccentBlue);

            var bottom = new Rect(rect.x, rect.y + top.height, rect.width, rect.height - top.height);
            EditorGUI.DrawRect(bottom, hover ? AccentVioletHover : AccentViolet);

            EditorGUI.DrawRect(new Rect(rect.x, rect.y, rect.width, 1f), ColorBorder);
            EditorGUI.DrawRect(new Rect(rect.x, rect.yMax - 1f, rect.width, 1f), ColorBorder);

            var style = new GUIStyle(GUI.skin.button)
            {
                fontSize = 15,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };
            style.normal.textColor = Color.white;

            return GUI.Button(rect, label, style);
        }

        private bool SecondaryButton(string label, float height = 28f)
        {
            var rect = GUILayoutUtility.GetRect(
                new GUIContent(label),
                EditorStyles.miniButton,
                GUILayout.Height(height),
                GUILayout.MaxWidth(180f));

            EditorGUI.DrawRect(rect, ColorPanelAlt);

            var style = new GUIStyle(GUI.skin.button)
            {
                alignment = TextAnchor.MiddleCenter
            };
            style.normal.textColor = ColorText;

            return GUI.Button(rect, label, style);
        }

        private bool MiniButton(string label, float width)
        {
            var rect = GUILayoutUtility.GetRect(
                new GUIContent(label),
                EditorStyles.miniButton,
                GUILayout.Width(width));

            EditorGUI.DrawRect(rect, ColorPanelAlt);

            var style = new GUIStyle(EditorStyles.miniButton);
            style.normal.textColor = ColorText;

            return GUI.Button(rect, label, style);
        }

        private void OpenDocumentation()
        {
            if (string.IsNullOrWhiteSpace(DocumentationUrl))
            {
                EditorUtility.DisplayDialog("Documentation", "Documentation URL is empty.", "OK");
                return;
            }

            UnityEngine.Application.OpenURL(DocumentationUrl);
        }

        private void HandleDragAndDrop(Rect dropArea)
        {
            var currentEvent = Event.current;

            if (currentEvent.type != EventType.DragUpdated && currentEvent.type != EventType.DragPerform)
                return;

            if (!dropArea.Contains(currentEvent.mousePosition))
                return;

            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

            if (currentEvent.type == EventType.DragPerform)
            {
                DragAndDrop.AcceptDrag();

                if (DragAndDrop.paths.Length == 1 && AssetDatabase.IsValidFolder(DragAndDrop.paths[0]))
                {
                    ScanFolder(DragAndDrop.paths[0]);
                }
                else
                {
                    var toAdd = new List<string>();

                    foreach (string path in DragAndDrop.paths)
                    {
                        if (AssetDatabase.IsValidFolder(path))
                        {
                            string[] guids = AssetDatabase.FindAssets("t:MonoScript", new[] { path });
                            foreach (string guid in guids)
                            {
                                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                                if (assetPath.EndsWith(".cs", StringComparison.OrdinalIgnoreCase))
                                    toAdd.Add(assetPath);
                            }
                        }
                        else if (path.EndsWith(".cs", StringComparison.OrdinalIgnoreCase))
                        {
                            toAdd.Add(NormalizeAssetPath(path));
                        }
                    }

                    AddFiles(toAdd);
                }
            }

            currentEvent.Use();
        }

        private void ScanSelectionTarget()
        {
            if (_scanTarget == null)
            {
                EditorUtility.DisplayDialog("Scan", "Select a folder or a .cs file first.", "OK");
                return;
            }

            string path = AssetDatabase.GetAssetPath(_scanTarget);
            if (string.IsNullOrEmpty(path))
            {
                EditorUtility.DisplayDialog("Scan", "Invalid asset.", "OK");
                return;
            }

            if (AssetDatabase.IsValidFolder(path))
                ScanFolder(path);
            else if (path.EndsWith(".cs", StringComparison.OrdinalIgnoreCase))
                AddFiles(new[] { path });
            else
                EditorUtility.DisplayDialog("Scan", "Please select a folder or a .cs file.", "OK");

            Repaint();
        }

        private void ScanFolder(string folder)
        {
            _scriptSelectionManager.Scan(folder);
            _scriptsScanned = _scriptSelectionManager.Scripts.Count > 0;
            _status = _scriptsScanned
                ? $"Scanned: {_scriptSelectionManager.Scripts.Count} files under '{folder}'."
                : $"No C# scripts found under '{folder}'.";
        }

        private void AddFiles(IEnumerable<string> paths)
        {
            int before = _scriptSelectionManager.Scripts.Count;
            _scriptSelectionManager.AddFiles(paths);
            int added = _scriptSelectionManager.Scripts.Count - before;

            if (added > 0)
            {
                _scriptsScanned = true;
                _status = $"Added {added} file(s) to the list.";
            }
            else
            {
                _status = "No new .cs files added.";
            }

            Repaint();
        }

        private void GenerateDiagram()
        {
            if (!_scriptsScanned || _scriptSelectionManager.Scripts.Count == 0)
            {
                _status = "❌ No scripts scanned or added. Use Scan or drop files.";
                EditorUtility.DisplayDialog("Error", "No scripts have been scanned.", "OK");
                return;
            }

            var selectedScripts = _scriptSelectionManager.GetSelected();
            if (selectedScripts.Count == 0)
            {
                _status = "❗ No script selected!";
                EditorUtility.DisplayDialog("Error", "Select at least one script to include in the diagram.", "OK");
                return;
            }

            try
            {
                var service = new DiagramGenerationService(
                    new RegexClassDiagramParser(),
                    new PlantUmlExporter());

                var result = service.Generate(new GenerateDiagramRequest
                {
                    SelectedScriptPaths = selectedScripts.Select(script => script.Path).ToList(),
                    IncludeAssociations = _includeAssociations
                });

                if (!result.Success)
                {
                    _status = "❗ " + result.StatusMessage;
                    EditorUtility.DisplayDialog("Nothing generated", result.StatusMessage, "OK");
                    return;
                }

                if (_exportFormat == DiagramExportFormat.PlantUmlFile)
                {
                    UnityAssetFileWriter.WriteTextAsset(_outputPath, result.PlantUmlText);
                    _status = $"✅ Diagram generated: {_outputPath}  •  Types: {result.TypeCount}";
                    EditorUtility.DisplayDialog("Done!", $"Diagram generated:\n{_outputPath}\nTypes: {result.TypeCount}", "OK");
                }
                else
                {
                    _lastDiagramUrl = PlantUmlUrlEncoder.ConvertTextToUrl(result.PlantUmlText);
                    _status = $"✅ Diagram URL generated. Types: {result.TypeCount}";
                    EditorUtility.DisplayDialog("Done!", "URL generated below.\nCopy-paste it in your browser.", "OK");
                }

                if (result.Warnings != null && result.Warnings.Count > 0)
                    Debug.LogWarning("[ClassDiagramGenerator]\n" + string.Join("\n", result.Warnings));
            }
            catch (Exception exception)
            {
                _status = $"❌ Generation failed: {exception.Message}";
                Debug.LogException(exception);
                EditorUtility.DisplayDialog("Generation failed", exception.Message, "OK");
            }
        }

        private void ImportDiagramAndGenerateSkeleton()
        {
            if (_importDiagramAsset == null)
            {
                _status = "❌ No diagram selected.";
                EditorUtility.DisplayDialog("Error", "Select a .puml diagram first.", "OK");
                return;
            }

            string diagramAssetPath = AssetDatabase.GetAssetPath(_importDiagramAsset);
            if (string.IsNullOrWhiteSpace(diagramAssetPath) || !File.Exists(diagramAssetPath))
            {
                _status = "❌ Invalid diagram path.";
                EditorUtility.DisplayDialog("Error", "The selected diagram file is invalid.", "OK");
                return;
            }

            if (!diagramAssetPath.EndsWith(".puml", StringComparison.OrdinalIgnoreCase))
            {
                _status = "❌ Selected file is not a .puml.";
                EditorUtility.DisplayDialog("Error", "Select a PlantUML file with .puml extension.", "OK");
                return;
            }

            try
            {
                string diagramText = File.ReadAllText(diagramAssetPath);

                var service = new DiagramImportAndSkeletonService(
                    new PlantUmlDiagramImporter(),
                    new CSharpSkeletonGenerator());

                var result = service.ImportAndGenerate(diagramText, _skeletonOutputPath, _skipExistingFiles);

                if (!result.Success)
                {
                    _status = "❗ " + result.StatusMessage;
                    EditorUtility.DisplayDialog("Generation failed", result.StatusMessage, "OK");
                    return;
                }

                _status = $"✅ Skeleton generated. Created: {result.CreatedCount}, Skipped: {result.SkippedCount}";
                EditorUtility.DisplayDialog(
                    "Done!",
                    $"Skeleton generation done.\nCreated: {result.CreatedCount}\nSkipped: {result.SkippedCount}",
                    "OK");

                if (result.Warnings != null && result.Warnings.Count > 0)
                    Debug.LogWarning("[ClassDiagramGenerator]\n" + string.Join("\n", result.Warnings));

                if (result.SkippedFiles != null && result.SkippedFiles.Count > 0)
                    Debug.LogWarning("[ClassDiagramGenerator] Existing files skipped:\n" + string.Join("\n", result.SkippedFiles));
            }
            catch (Exception exception)
            {
                _status = $"❌ Import/generation failed: {exception.Message}";
                Debug.LogException(exception);
                EditorUtility.DisplayDialog("Generation failed", exception.Message, "OK");
            }
        }

        private static Texture2D MakeTexture(int width, int height, Color color)
        {
            var texture = new Texture2D(width, height);
            var pixels = new Color[width * height];
            for (int index = 0; index < pixels.Length; index++)
                pixels[index] = color;

            texture.SetPixels(pixels);
            texture.Apply();
            return texture;
        }

        private static string NormalizeAssetPath(string path)
        {
            return string.IsNullOrWhiteSpace(path)
                ? string.Empty
                : path.Replace('\\', '/');
        }
    }
}