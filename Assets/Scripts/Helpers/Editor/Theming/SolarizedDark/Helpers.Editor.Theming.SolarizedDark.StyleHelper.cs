// Editor/Theming/SolarizedDarkStyleHelper.cs

using System.IO;
using Helpers.Attributes;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Helpers.Editor.Theming.SolarizedDark
{
	[PublicAPI]
	[AiGenerated("Claude", "Sonnet 4.6", "Reviewed by Rev 7-30-26")]
	public static class StyleHelper
	{
		// ── Background ────────────────────────────────────────────────────────

		public const string BgBase03 = "sol-bg-base03";

		public const string BgBase02 = "sol-bg-base02";

		public const string BgBase01 = "sol-bg-base01";

		public const string BgBase00 = "sol-bg-base00";

		public const string BgBase0 = "sol-bg-base0";

		public const string BgBase1 = "sol-bg-base1";

		public const string BgBase2 = "sol-bg-base2";

		public const string BgBase3 = "sol-bg-base3";

		// ── Background — accents ──────────────────────────────────────────────

		public const string BgBlue = "sol-bg-blue";

		public const string BgCyan = "sol-bg-cyan";

		public const string BgYellow = "sol-bg-yellow";

		public const string BgOrange = "sol-bg-orange";

		public const string BgGreen = "sol-bg-green";

		public const string BgRed = "sol-bg-red";

		public const string BgMagenta = "sol-bg-magenta";

		public const string BgViolet = "sol-bg-violet";

		// ── Text color — base scale ───────────────────────────────────────────

		public const string TextBase03 = "sol-text-base03";

		public const string TextBase02 = "sol-text-base02";

		public const string TextBase01 = "sol-text-base01";

		public const string TextBase00 = "sol-text-base00";

		public const string TextBase0 = "sol-text-base0";

		public const string TextBase1 = "sol-text-base1";

		public const string TextBase2 = "sol-text-base2";

		public const string TextBase3 = "sol-text-base3";

		// ── Text color — accents ──────────────────────────────────────────────

		public const string TextBlue = "sol-text-blue";

		public const string TextCyan = "sol-text-cyan";

		public const string TextYellow = "sol-text-yellow";

		public const string TextOrange = "sol-text-orange";

		public const string TextGreen = "sol-text-green";

		public const string TextRed = "sol-text-red";

		public const string TextMagenta = "sol-text-magenta";

		public const string TextViolet = "sol-text-violet";

		// ── Text color — special ──────────────────────────────────────────────

		public const string TextDefault = "sol-text-default";

		// ── Font ──────────────────────────────────────────────────────────────

		public const string FontBold = "sol-font-bold";

		public const string FontNormal = "sol-font-normal";

		public const string TextSm = "sol-text-sm";

		// ── Layout ────────────────────────────────────────────────────────────

		public const string FlexRow = "sol-flex-row";

		public const string FlexCol = "sol-flex-col";

		public const string FlexGrow = "sol-flex-grow";

		// ── Spacing ───────────────────────────────────────────────────────────

		public const string Pa1 = "sol-pa-1";

		public const string Pa2 = "sol-pa-2";

		// ── Components ────────────────────────────────────────────────────────

		public const string VContainer = "sol-v-container";

		public const string VRow = "sol-v-row";

		public const string VCol = "sol-v-col";

		public const string VCard = "sol-v-card";

		public const string VDivider = "sol-v-divider";

		public const string VBorder = "sol-v-border";

		public const string VBtn = "sol-v-btn";

		public const string VBtnPrimary = "sol-v-btn-primary";

		public const string VField = "sol-v-field";

		public const string VSwitch = "sol-v-switch";

		public const string VBooleanLabel = "sol-v-boolean-label";

		public const string VFoldout = "sol-v-foldout";

		public const string VList = "sol-v-list";

		public const string BorderRadiusRounded = "sol-border-radius-rounded";

		private static StyleSheet _cachedSheet;

		// ── RuntimeStyleSheet generation ─────────────────────────────────────

		private static string _unityDefaultTextColor => OriginalUnity.Palette.DefaultText;

		public static string USSPath
		{
			get
			{
				var guids = AssetDatabase.FindAssets("Helpers.Editor.Theming.SolarizedDark.StyleHelper t:Script");

				if (guids.Length == 0)
				{
					Debug.LogWarning("SolarizedDark.StyleHelper script asset not found — USS path could not be resolved.");

					return null;
				}

				var scriptPath = AssetDatabase.GUIDToAssetPath(guids[0]);
				var themeDir = Path.GetDirectoryName(Path.GetDirectoryName(scriptPath));

				return (themeDir + "/SolarizedDark.uss").Replace('\\', '/');
			}
		}

		public static StyleSheet GetStyleSheet()
		{
			if (_cachedSheet != null) return _cachedSheet;

			_cachedSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(USSPath);

			if (_cachedSheet == null)
			{
				GenerateUSSFile();
				_cachedSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(USSPath);
			}

			return _cachedSheet;
		}

		public static void ApplyTo(VisualElement root) => root.styleSheets.Add(GetStyleSheet());

		// ── USS string builder ────────────────────────────────────────────────

		private static string BuildUSS() =>
			$@"
            /* ── Background ─────────────────────────────────────────────────── */
            .{BgBase03} {{ background-color: {Palette.Base03}; }}
            .{BgBase02} {{ background-color: {Palette.Base02}; }}
            .{BgBase01} {{ background-color: {Palette.Base01}; }}
            .{BgBase00} {{ background-color: {Palette.Base00}; }}
            .{BgBase0}  {{ background-color: {Palette.Base0}; }}
            .{BgBase1}  {{ background-color: {Palette.Base1}; }}
            .{BgBase2}  {{ background-color: {Palette.Base2}; }}
            .{BgBase3}  {{ background-color: {Palette.Base3}; }}

            /* ── Background — accents ────────────────────────────────────────── */
            /* Type-qualified variants outrank component rules (e.g. .sol-v-btn's own
               background) so accents stay visible on buttons regardless of rule order. */
            .{BgBlue},    Button.{BgBlue}    {{ background-color: {Palette.Blue}; }}
            .{BgCyan},    Button.{BgCyan}    {{ background-color: {Palette.Cyan}; }}
            .{BgYellow},  Button.{BgYellow}  {{ background-color: {Palette.Yellow}; }}
            .{BgOrange},  Button.{BgOrange}  {{ background-color: {Palette.Orange}; }}
            .{BgGreen},   Button.{BgGreen}   {{ background-color: {Palette.Green}; }}
            .{BgRed},     Button.{BgRed}     {{ background-color: {Palette.Red}; }}
            .{BgMagenta}, Button.{BgMagenta} {{ background-color: {Palette.Magenta}; }}
            .{BgViolet},  Button.{BgViolet}  {{ background-color: {Palette.Violet}; }}

            /* ── Text color — base scale ─────────────────────────────────────── */
            .{TextBase03} {{ color: {Palette.Base03}; }}
            .{TextBase02} {{ color: {Palette.Base02}; }}
            .{TextBase01} {{ color: {Palette.Base01}; }}
            .{TextBase00} {{ color: {Palette.Base00}; }}
            .{TextBase0}  {{ color: {Palette.Base0}; }}
            .{TextBase1}  {{ color: {Palette.Base1}; }}
            .{TextBase2}  {{ color: {Palette.Base2}; }}
            .{TextBase3}  {{ color: {Palette.Base3}; }}

            /* ── Text color — accents ────────────────────────────────────────── */
            .{TextBlue}    {{ color: {Palette.Blue}; }}
            .{TextCyan}    {{ color: {Palette.Cyan}; }}
            .{TextYellow}  {{ color: {Palette.Yellow}; }}
            .{TextOrange}  {{ color: {Palette.Orange}; }}
            .{TextGreen}   {{ color: {Palette.Green}; }}
            .{TextRed}     {{ color: {Palette.Red}; }}
            .{TextMagenta} {{ color: {Palette.Magenta}; }}
            .{TextViolet}  {{ color: {Palette.Violet}; }}

            /* ── Text color — special ────────────────────────────────────────── */
            .{TextDefault} {{ color: {_unityDefaultTextColor}; }}

            /* ── Font ────────────────────────────────────────────────────────── */
            .{FontBold}   {{ -unity-font-style: bold; }}
            .{FontNormal} {{ -unity-font-style: normal; }}
            .{TextSm}     {{ font-size: 11px; }}

            /* ── Layout ──────────────────────────────────────────────────────── */
            .{FlexRow}  {{ flex-direction: row; }}
            .{FlexCol}  {{ flex-direction: column; }}
            .{FlexGrow} {{ flex-grow: 1; }}

            /* ── Spacing ─────────────────────────────────────────────────────── */
            .{Pa1} {{ padding: 4px; }}
            .{Pa2} {{ padding: 8px; }}

            /* ── Component — container ───────────────────────────────────────── */
            .{VContainer} {{
                padding: 4px;
            }}

            /* ── Component — row ─────────────────────────────────────────────── */
            .{VRow} {{
                flex-direction: row;
                margin: 0px -4px;
            }}

            /* ── Component — col ─────────────────────────────────────────────── */
            .{VCol} {{
                flex-direction: column;
                flex-grow: 1;
                padding: 4px;
            }}

            /* ── Component — card ────────────────────────────────────────────── */
            .{VCard} {{
                background-color:    {Palette.Base02};
                border-radius:       {OriginalUnity.Palette.BorderRadiusRounded}px;
                border-top-width:    1px;
                border-bottom-width: 1px;
                border-left-width:   1px;
                border-right-width:  1px;
                border-top-color:    {Palette.Base01};
                border-bottom-color: {Palette.Base01};
                border-left-color:   {Palette.Base01};
                border-right-color:  {Palette.Base01};
                padding:             8px;
                margin-bottom:       4px;
            }}

            /* ── Component — divider ─────────────────────────────────────────── */
            .{VDivider} {{
                height:           1px;
                background-color: {Palette.Base01};
                margin-top:       4px;
                margin-bottom:    4px;
            }}

            /* ── Component — border ──────────────────────────────────────────── */
            .{VBorder} {{
                border-top-color:    {Palette.Base01};
                border-bottom-color: {Palette.Base01};
                border-left-color:   {Palette.Base01};
                border-right-color:  {Palette.Base01};
                border-top-width:    1px;
                border-bottom-width: 1px;
                border-left-width:   1px;
                border-right-width:  1px;
            }}

            /* ── Component — button ──────────────────────────────────────────── */
            .{VBtn} {{
                background-color:    {Palette.Base02};
                color:               {Palette.Base0};
                border-radius:       {OriginalUnity.Palette.BorderRadiusRounded}px;
                border-top-color:    {Palette.Base01};
                border-bottom-color: {Palette.Base01};
                border-left-color:   {Palette.Base01};
                border-right-color:  {Palette.Base01};
                border-top-width:    1px;
                border-bottom-width: 1px;
                border-left-width:   1px;
                border-right-width:  1px;
								overflow: Visible;
								white-space: Normal;
								flex-grow: 1;
            }}

            .{VBtn}:hover {{
                background-color: {Palette.Base01};
                color:            {Palette.Base1};
            }}


            /* ── Component — field ───────────────────────────────────────────── */
            /* Bare .{VField} intentionally carries no border/background — it's also used as a
               plain wrapper around PropertyField, and a wrapper + its generated inner field
               both matching would double up the border. Border/background only apply to the
               concrete field type, whether VField is on that field directly (Sol*Field() self-
               applies it — compound type.class selector) or on an ancestor wrapper around a
               PropertyField-generated field (descendant selector). */
            TextField.{VField},
            FloatField.{VField},
            IntegerField.{VField},
            Vector3Field.{VField},
            Vector3IntField.{VField},
            ObjectField.{VField},
            EnumField.{VField},
            Foldout.{VField},
            ListView.{VField},
            .{VField} TextField,
            .{VField} FloatField,
            .{VField} IntegerField,
            .{VField} Vector3Field,
            .{VField} Vector3IntField,
            .{VField} ObjectField,
            .{VField} EnumField,
            .{VField} Foldout,
            .{VField} ListView {{
                color:         {_unityDefaultTextColor};
                border-radius: {OriginalUnity.Palette.BorderRadiusRounded}px;
            }}

            /* FloatField intentionally excluded — no background tint on top of the base surface. */
            TextField.{VField},
            IntegerField.{VField},
            Vector3Field.{VField},
            Vector3IntField.{VField},
            ObjectField.{VField},
            EnumField.{VField},
            Foldout.{VField},
            ListView.{VField},
            .{VField} TextField,
            .{VField} IntegerField,
            .{VField} Vector3Field,
            .{VField} Vector3IntField,
            .{VField} ObjectField,
            .{VField} EnumField,
            .{VField} Foldout,
            .{VField} ListView {{
                background-color: {Palette.Base02};
            }}

            /* Foldout keeps its border — applies only to the top-level Foldout element itself,
               not the field/value rows nested inside it. */
            Foldout.{VField},
            .{VField} Foldout {{
                border-top-color:    {Palette.Base01};
                border-bottom-color: {Palette.Base01};
                border-left-color:   {Palette.Base01};
                border-right-color:  {Palette.Base01};
                border-top-width:    1px;
                border-bottom-width: 1px;
                border-left-width:   1px;
                border-right-width:  1px;
            }}

            /* ── Component — list border ─────────────────────────────────────── */
            /* Own class, not VField — VField's compound/descendant selectors were matching
               ListView's internal Unity-built rows/elements downstream, not just the outer
               ListView itself. SolList() applies this directly alongside VField. */
            .{VList} {{
                border-top-color:    {Palette.Base01};
                border-bottom-color: {Palette.Base01};
                border-left-color:   {Palette.Base01};
                border-right-color:  {Palette.Base01};
                border-top-width:    1px;
                border-bottom-width: 1px;
                border-left-width:   1px;
                border-right-width:  1px;
            }}

            .{VField} Label {{
                color:       {_unityDefaultTextColor};
                min-width:   0;
                flex-shrink: 1;
            }}

            /* ── Component — switch ──────────────────────────────────────────── */
            .{VSwitch} {{
                color: {_unityDefaultTextColor};
            }}

            .{VSwitch} > .unity-toggle__checkmark {{
                background-color:    {Palette.Base02};
                border-top-color:    {Palette.Base01};
                border-bottom-color: {Palette.Base01};
                border-left-color:   {Palette.Base01};
                border-right-color:  {Palette.Base01};
                border-top-width:    1px;
                border-bottom-width: 1px;
                border-left-width:   1px;
                border-right-width:  1px;
            }}

            .{VSwitch}:checked > .unity-toggle__checkmark {{
                background-color: {Palette.Blue};
            }}

            /* ── Component — boolean label ───────────────────────────────────── */
            .{VBooleanLabel} {{
               	border-radius: {OriginalUnity.Palette.BorderRadiusRounded}px;
            }}

			.{BorderRadiusRounded} {{
		        border-radius: {OriginalUnity.Palette.BorderRadiusRounded}px;
			}}

            /* ── Component — foldout ─────────────────────────────────────────── */
            .{VFoldout} > .unity-toggle {{
                margin-left: 0;
            }}

            .{VContainer} .unity-foldout > .unity-toggle {{
                margin-left: 0;
            }}
        ";

		// ── Utility ───────────────────────────────────────────────────────────

		[MenuItem("Tools/Helpers/Theming/Generate Solarized Dark USS")]
		public static void GenerateUSSFile()
		{
			var path = USSPath;

			if (path == null) return;

			var directory = Path.GetDirectoryName(path);

			if (!Directory.Exists(directory)
					&& directory != null)
				Directory.CreateDirectory(directory);

			File.WriteAllText(path, BuildUSS());

			AssetDatabase.ImportAsset(path);
			AssetDatabase.Refresh();

			Debug.Log($"USS written to {path}");
		}
	}
}