using System.Collections.Generic;
using System.Reflection;
using Helpers.Attributes;
using Helpers.Editor.Ext;
using Helpers.Editor.Theming.SolarizedDark;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Cursor = UnityEngine.UIElements.Cursor;

namespace Helpers.Editor.Tools.FolderNav
{
	[AiGenerated("Claude", "Sonnet 4.6")]
	public class Window : EditorWindow
	{
		private bool _editMode;

		private List<FolderEntry> _folders;

		private VisualElement _listContainer;

		public void CreateGUI()
		{
			_folders ??= FolderNavigatorData.Load();

			var root = Ele.SolRoot();
			root.style.SetPadding(8);

			root.Add(BuildHeader());
			root.Add(Ele.SolDivider());

			_listContainer = new VisualElement();
			root.Add(_listContainer);

			root.Add(Ele.SolDivider());
			root.Add(BuildAddRow());

			RefreshList();
			rootVisualElement.Add(root);
		}

		[MenuItem("Tools/Helpers/Folder Navigator %#f")] public static void Open() => GetWindow<Window>("Folders");

		// ── Header ────────────────────────────────────────────────────────────

		private VisualElement BuildHeader()
		{
			var editButton = Ele.SolButton(_ => ToggleEditMode(), _editMode ? "Done" : "Edit");

			var resetButton = Ele.SolButton(
				_ =>
				{
					if (EditorUtility.DisplayDialog(
							"Reset Folders",
							"Restore default folders? This cannot be undone.",
							"Reset",
							"Cancel"
						))
					{
						FolderNavigatorData.Reset();
						_folders = FolderNavigatorData.Load();
						RefreshList();
					}
				},
				"Reset"
			);

			return Ele.SolRow()
					  .WithChildren(Ele.SolLabel("Quick Nav"), Ele.SolRow().WithChildren(editButton, resetButton))
					  .WithStyle(r => r.justifyContent = Justify.SpaceBetween);
		}

		// ── Folder list ───────────────────────────────────────────────────────

		private void RefreshList()
		{
			_listContainer.Clear();

			for (var i = 0; i < _folders.Count; i++)
			{
				// Capture for lambda
				var index = i;
				_listContainer.Add(_editMode ? BuildEditRow(index) : BuildNavRow(index));
			}
		}

		private VisualElement BuildNavRow(int index)
		{
			var entry = _folders[index];
			var exists = AssetDatabase.IsValidFolder(entry.Path);

			var label = Ele.SolLabel(entry.Label).WithClass(exists ? StyleHelper.TextBase1 : StyleHelper.TextRed);

			var row = Ele.SolRow()
						 .WithChildren(label)
						 .WithStyle(r =>
							  {
								  r.paddingTop = 4;
								  r.paddingBottom = 4;
								  r.paddingLeft = 6;
								  r.paddingRight = 6;
							  }
						  );

			row.style.SetBorderRadius(3);

			if (exists)
			{
				row.RegisterCallback<ClickEvent>(_ => SelectFolder(entry.Path));

				row.RegisterCallback<MouseEnterEvent>(_ => row.style.backgroundColor = Palette.Base02.ToColor());

				row.RegisterCallback<MouseLeaveEvent>(_ => row.style.backgroundColor = Palette.Base03.ToColor());

				row.style.cursor = new StyleCursor(
					new Cursor
					{
						texture = null,
						hotspot = Vector2.zero,
					}
				);
			}
			else
			{
				row.style.opacity = 0.5f;
			}

			return row;
		}

		// Ping + Select Button
		// private VisualElement BuildNavRow(int index) {
		//     var entry = _folders[index];
		//     var exists = AssetDatabase.IsValidFolder(entry.Path);
		//
		//     var label = Theming.SolarizedDark.UIToolkitWrapper.SolLabel(entry.Label)
		//                        .WithClass(
		//                                 exists
		//                                     ? Theming.SolarizedDark.StyleHelper.ClassTextBody
		//                                     : Theming.SolarizedDark.StyleHelper.ClassAccentRed
		//                             );
		//
		//     var pingButton = Theming.SolarizedDark.UIToolkitWrapper.SolButton("Ping", () => PingFolder(entry.Path));
		//     pingButton.SetEnabled(exists);
		//
		//     var selectButton
		//         = Theming.SolarizedDark.UIToolkitWrapper.SolButton("Select", () => SelectFolder(entry.Path));
		//
		//     selectButton.SetEnabled(exists);
		//
		//     return Theming.SolarizedDark.UIToolkitWrapper.SolRow()
		//                   .WithChildren(
		//                        label,
		//                        Theming.SolarizedDark.UIToolkitWrapper.SolRow().WithChildren(pingButton, selectButton)
		//                    )
		//                   .WithStyle(r =>
		//                            {
		//                                r.justifyContent = Justify.SpaceBetween;
		//                                r.paddingTop = 2;
		//                                r.paddingBottom = 2;
		//                            }
		//                        );
		// }

		private VisualElement BuildEditRow(int index)
		{
			var entry = _folders[index];

			var labelField = new TextField
			{
				value = entry.Label,
				style =
				{
					width = 100,
				},
			};

			labelField.RegisterValueChangedCallback(e =>
				{
					_folders[index].Label = e.newValue;
					FolderNavigatorData.Save(_folders);
				}
			);

			var pathField = new TextField
			{
				value = entry.Path,
				style =
				{
					flexGrow = 1,
					flexShrink = 1,
					minWidth = 0,
				},
			};

			pathField.RegisterValueChangedCallback(e =>
				{
					_folders[index].Path = e.newValue;
					FolderNavigatorData.Save(_folders);
				}
			);

			// Let the user drag a folder asset onto the path field
			pathField.RegisterCallback<DragUpdatedEvent>(_ => DragAndDrop.visualMode = DragAndDropVisualMode.Generic);

			pathField.RegisterCallback<DragPerformEvent>(_ =>
				{
					if (DragAndDrop.paths.Length <= 0) return;

					pathField.value = DragAndDrop.paths[0];
					_folders[index].Path = DragAndDrop.paths[0];
					FolderNavigatorData.Save(_folders);
				}
			);

			var moveUpButton = Ele.SolButton(
				_ =>
				{
					if (index <= 0) return;

					(_folders[index], _folders[index - 1]) = (_folders[index - 1], _folders[index]);
					FolderNavigatorData.Save(_folders);
					RefreshList();
				},
				"↑"
			);

			var moveDownButton = Ele.SolButton(
				_ =>
				{
					if (index >= _folders.Count - 1) return;

					(_folders[index], _folders[index + 1]) = (_folders[index + 1], _folders[index]);
					FolderNavigatorData.Save(_folders);
					RefreshList();
				},
				"↓"
			);

			var deleteButton = Ele.SolButton(
				_ =>
				{
					_folders.RemoveAt(index);
					FolderNavigatorData.Save(_folders);
					RefreshList();
				},
				"✕"
			);

			deleteButton.WithClass(StyleHelper.TextRed);

			return Ele.SolRow()
					  .WithChildren(
						   labelField,
						   pathField,
						   moveUpButton,
						   moveDownButton,
						   deleteButton
					   )
					  .WithStyle(r =>
						   {
							   r.paddingTop = 2;
							   r.paddingBottom = 2;
						   }
					   );
		}

		// ── Add row ───────────────────────────────────────────────────────────

		private VisualElement BuildAddRow()
		{
			var labelField = MakeTextField("Label", 100);
			var pathField = MakeTextField("Assets/...", grow: true);

			pathField.RegisterCallback<DragUpdatedEvent>(_ => DragAndDrop.visualMode = DragAndDropVisualMode.Generic);

			pathField.RegisterCallback<DragPerformEvent>(_ =>
				{
					if (DragAndDrop.paths.Length > 0)
					{
						pathField.value = DragAndDrop.paths[0];

						pathField.RemoveFromClassList(StyleHelper.TextBase01);
						pathField.RemoveFromClassList(StyleHelper.TextSm);
						pathField.AddToClassList(StyleHelper.TextBase1);
					}
				}
			);

			var addButton = Ele.SolButton(
				_ =>
				{
					// Treat the placeholder value as empty
					var label = labelField.value == "Label" ? string.Empty : labelField.value;
					var path = pathField.value == "Assets/..." ? string.Empty : pathField.value;

					if (string.IsNullOrWhiteSpace(label)
						|| string.IsNullOrWhiteSpace(path))
					{
						Debug.LogWarning("FolderNavigator: Label and Path must both be filled in");

						return;
					}

					_folders.Add(
						new FolderEntry
						{
							Label = label,
							Path = path,
						}
					);

					FolderNavigatorData.Save(_folders);

					// Reset both fields back to placeholder state
					labelField.value = "Label";
					labelField.RemoveFromClassList(StyleHelper.TextBase1);
					labelField.AddToClassList(StyleHelper.TextBase01);
					labelField.AddToClassList(StyleHelper.TextSm);

					pathField.value = "Assets/...";
					pathField.RemoveFromClassList(StyleHelper.TextBase1);
					pathField.AddToClassList(StyleHelper.TextBase01);
					pathField.AddToClassList(StyleHelper.TextSm);

					RefreshList();
				},
				"Add Folder"
			);

			addButton.style.width = 80;
			addButton.style.flexShrink = 0;

			var container = Ele.SolCard();
			container.style.marginTop = 8;
			container.Add(Ele.SolLabel("Add Folder"));

			container.Add(
				Ele.SolRow()
				   .WithChildren(labelField, pathField, addButton)
				   .WithStyle(r =>
						{
							r.marginTop = 4;
							r.overflow = Overflow.Hidden;
						}
					)
			);

			return container;
		}
		// ── Helpers ───────────────────────────────────────────────────────────

		private void ToggleEditMode()
		{
			_editMode = !_editMode;
			RefreshList();

			// Rebuild header so the button label updates
			rootVisualElement.Clear();
			CreateGUI();
		}

		// private static void PingFolder(string path)
		// {
		// 	var obj = AssetDatabase.LoadAssetAtPath<Object>(path);
		// 	EditorGUIUtility.PingObject(obj);
		// }

		// private static void SelectFolder(string path) {
		//     var obj = AssetDatabase.LoadAssetAtPath<Object>(path);
		//     Selection.activeObject = obj;
		//     EditorUtility.FocusProjectWindow();
		// }

		private static TextField MakeTextField(string placeholder, float width = -1, bool grow = false)
		{
			var field = new TextField
			{
				style =
				{
					flexGrow = grow ? 1 : 0,
					flexShrink = grow ? 1 : 0,
					minWidth = 0,
				},
			};

			if (width > 0) field.style.width = width;

			// Placeholder simulation
			field.value = placeholder;
			field.AddToClassList(StyleHelper.TextBase01);
			field.AddToClassList(StyleHelper.TextSm);

			field.RegisterCallback<FocusInEvent>(_ =>
				{
					if (field.value == placeholder)
					{
						field.value = string.Empty;
						field.RemoveFromClassList(StyleHelper.TextBase01);
						field.RemoveFromClassList(StyleHelper.TextSm);
						field.AddToClassList(StyleHelper.TextBase1);
					}
				}
			);

			field.RegisterCallback<FocusOutEvent>(_ =>
				{
					if (string.IsNullOrWhiteSpace(field.value))
					{
						field.value = placeholder;
						field.RemoveFromClassList(StyleHelper.TextBase1);
						field.AddToClassList(StyleHelper.TextBase01);
						field.AddToClassList(StyleHelper.TextSm);
					}
				}
			);

			return field;
		}

		private static void SelectFolder(string path)
		{
			var obj = AssetDatabase.LoadAssetAtPath<Object>(path);

			if (obj == null)
			{
				Debug.LogWarning($"FolderNavigator: Could not load folder at '{path}'");

				return;
			}

			Selection.activeObject = obj;
			EditorUtility.FocusProjectWindow();

			EditorApplication.delayCall += () =>
			{
				var focused = focusedWindow;

				if (focused == null) return;

				var showFolderContents = focused.GetType()
												.GetMethod(
													 "ShowFolderContents",
													 BindingFlags.NonPublic | BindingFlags.Instance
												 );

				if (showFolderContents != null)
					showFolderContents.Invoke(
						focused,
						new object[]
						{
							obj.GetInstanceID(),
							true,
						}
					);
				else
					Debug.LogWarning(
						"FolderNavigator: Could not find ShowFolderContents — Unity may have changed internals"
					);
			};
		}
	}
}