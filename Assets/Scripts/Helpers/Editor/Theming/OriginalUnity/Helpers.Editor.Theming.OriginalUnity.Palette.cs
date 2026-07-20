// Unity Pro (dark skin) reference colors.
// Approximated from Unity's built-in editor skin — minor variation across Unity versions is expected.
// Sample via EditorGUIUtility.GetBuiltinSkin(EditorSkin.Scene) or pixel-pick in the editor to verify.

namespace Helpers.Editor.Theming.OriginalUnity
{
	public static class Palette
	{
		// ── Backgrounds ──────────────────────────────────────────────────────

		public const string DarkBackground = "#282828"; // Scene/Game view, darkest panels

		public const string WindowBackground = "#3C3C3C"; // Standard window / panel background

		public const string InspectorBackground = "#383838"; // Inspector, Project window rows

		public const string ToolbarBackground = "#3C3C3C"; // Top toolbar strip

		public const string HoverBackground = "#464646"; // Hovered list row or button

		// ── Text ─────────────────────────────────────────────────────────────

		public const string DefaultText = "#C4C4C4"; // Primary label color (EditorStyles.label)

		public const string DisabledText = "#808080"; // Grayed-out / disabled controls

		public const string PlaceholderText = "#656565"; // Input field placeholder / hint text

		// ── Controls ─────────────────────────────────────────────────────────

		public const string InputBackground = "#2A2A2A"; // TextField, IntField, FloatField background

		public const string ButtonBackground = "#585858"; // Standard button face

		public const string ButtonBorder = "#303030"; // Button border / outline

		// ── Selection & Focus ─────────────────────────────────────────────────

		public const string SelectionBackground = "#2D5F8B"; // Selected row in Hierarchy/Project

		public const string FocusBlue = "#0078D7"; // Keyboard focus ring

		// ── Borders & Separators ─────────────────────────────────────────────

		public const string Separator = "#232323"; // Horizontal rule, panel dividers

		public const string Border = "#1A1A1A"; // Hard border (e.g. between docked panels)

		// ── CSS Properties ─────────────────────────────────────────────

		public const int BorderRadiusRounded = 3; // Typical border-radius for UI elements with rounded corners
	}
}