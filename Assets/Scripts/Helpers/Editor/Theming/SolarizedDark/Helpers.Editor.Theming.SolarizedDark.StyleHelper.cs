// Editor/Theming/SolarizedDarkStyleHelper.cs

using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Helpers.Editor.Theming.SolarizedDark
{
    public static class StyleHelper
    {

        // ── USS class name constants ──────────────────────────────────────────
        // Avoids magic strings throughout your tool code

        public const string ClassBackground = "sol-bg";

        public const string ClassBgHighlight = "sol-bg-highlight";

        public const string ClassTextBody = "sol-text-body";

        public const string ClassTextEmphasis = "sol-text-emphasis";

        public const string ClassTextSecondary = "sol-text-secondary";

        public const string ClassAccentBlue = "sol-accent-blue";

        public const string ClassAccentCyan = "sol-accent-cyan";

        public const string ClassAccentYellow = "sol-accent-yellow";

        public const string ClassAccentGreen = "sol-accent-green";

        public const string ClassAccentRed = "sol-accent-red";

        public const string ClassCard = "sol-card";

        public const string ClassDivider = "sol-divider";

        public const string ClassButton = "sol-button";

        public const string ClassButtonPrimary = "sol-button-primary";

        // ── RuntimeStyleSheet generation ─────────────────────────────────────
        // Generates a StyleSheet from USS string at runtime — avoids needing
        // a .uss asset on disk, useful for package/plugin distribution.

        private static StyleSheet _cachedSheet;

        public static string USSPath = "Assets/Scripts/Helpers/Editor/Theming/SolarizedDark.uss";
        // ── Inline style application ──────────────────────────────────────────

        public static void ApplyBackground(VisualElement el, bool highlighted = false) {
            el.style.backgroundColor = ParseColor(highlighted ? Palette.Base02 : Palette.Base03);
        }

        public static void ApplyBodyText(VisualElement el, bool emphasized = false) {
            el.style.color = ParseColor(emphasized ? Palette.Base1 : Palette.Base0);
        }

        public static void ApplySecondaryText(VisualElement el) {
            el.style.color = ParseColor(Palette.Base01);
        }

        public static void ApplyAccent(VisualElement el, string hexColor) {
            el.style.color = ParseColor(hexColor);
        }

        public static void ApplyBorder(VisualElement el, string hexColor = null, float width = 1f) {
            var color = ParseColor(hexColor ?? Palette.Base01);
            el.style.borderTopColor = color;
            el.style.borderBottomColor = color;
            el.style.borderLeftColor = color;
            el.style.borderRightColor = color;
            el.style.borderTopWidth = width;
            el.style.borderBottomWidth = width;
            el.style.borderLeftWidth = width;
            el.style.borderRightWidth = width;
        }

        public static StyleSheet GetStyleSheet() {
            if (_cachedSheet != null) return _cachedSheet;

            _cachedSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(USSPath);

            if (_cachedSheet == null)
                UnityEngine.Debug.LogWarning(
                        "SolarizedDark.uss not found — run Tools/Theming/Generate Solarized Dark USS"
                    );

            return _cachedSheet;
        }

        public static void ApplyTo(VisualElement root) {
            root.styleSheets.Add(GetStyleSheet());
        }

        // ── USS string builder ────────────────────────────────────────────────

        private static string BuildUSS() {
            return $@"
            .{ClassBackground} {{
                background-color: {Palette.Base03};
            }}

            .{ClassBgHighlight} {{
                background-color: {Palette.Base02};
            }}

            .{ClassTextBody} {{
                color: {Palette.Base0};
                -unity-font-style: normal;
            }}

            .{ClassTextEmphasis} {{
                color: {Palette.Base1};
                -unity-font-style: bold;
            }}

            .{ClassTextSecondary} {{
                color: {Palette.Base01};
                font-size: 11px;
            }}

            .{ClassAccentBlue}    {{ color: {Palette.Blue}; }}
            .{ClassAccentCyan}    {{ color: {Palette.Cyan}; }}
            .{ClassAccentYellow}  {{ color: {Palette.Yellow}; }}
            .{ClassAccentGreen}   {{ color: {Palette.Green}; }}
            .{ClassAccentRed}     {{ color: {Palette.Red}; }}

            .{ClassCard} {{
                background-color: {Palette.Base02};
                border-radius: 4px;
                border-top-width: 1px;
                border-bottom-width: 1px;
                border-left-width: 1px;
                border-right-width: 1px;
                border-top-color: {Palette.Base01};
                border-bottom-color: {Palette.Base01};
                border-left-color: {Palette.Base01};
                border-right-color: {Palette.Base01};
                padding: 8px;
                margin-bottom: 4px;
            }}

            .{ClassDivider} {{
                height: 1px;
                background-color: {Palette.Base01};
                margin-top: 4px;
                margin-bottom: 4px;
            }}

            .{ClassButton} {{
                background-color: {Palette.Base02};
                color: {Palette.Base0};
                border-radius: 3px;
                border-top-color: {Palette.Base01};
                border-bottom-color: {Palette.Base01};
                border-left-color: {Palette.Base01};
                border-right-color: {Palette.Base01};
                border-top-width: 1px;
                border-bottom-width: 1px;
                border-left-width: 1px;
                border-right-width: 1px;
            }}

            .{ClassButton}:hover {{
                background-color: {Palette.Base01};
                color: {Palette.Base1};
            }}

            .{ClassButtonPrimary} {{
                background-color: {Palette.Blue};
                color: {Palette.Base03};
                border-top-width: 0;
                border-bottom-width: 0;
                border-left-width: 0;
                border-right-width: 0;
                border-radius: 3px;
            }}

            .{ClassButtonPrimary}:hover {{
                background-color: {Palette.Cyan};
            }}
        ";
        }

        // ── Utility ───────────────────────────────────────────────────────────

        public static Color ParseColor(string hex) {
            ColorUtility.TryParseHtmlString(hex, out var color);

            return color;
        }

        [MenuItem("Tools/Helpers/Theming/Generate Solarized Dark USS")]
        public static void GenerateUSSFile() {
            var path = USSPath;
            var directory = Path.GetDirectoryName(path);

            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

            File.WriteAllText(path, BuildUSS());

            // Tell Unity to pick it up
            AssetDatabase.ImportAsset(path);
            AssetDatabase.Refresh();

            UnityEngine.Debug.Log($"USS written to {path}");
        }

        private static StyleSheet StyleSheetFromUSS(string uss) {
            var sheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(USSPath);

            return sheet;
        }

    }
}