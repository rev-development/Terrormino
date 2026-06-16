using UnityEngine.UIElements;

namespace Helpers.Editor.Theming.SolarizedDark
{
    public static class UIToolkitWrapper
    {

        public static Label SolLabel(string text, bool emphasized = false) {
            var label = new Label(text);
            label.AddToClassList(emphasized ? StyleHelper.ClassTextEmphasis : StyleHelper.ClassTextBody);

            return label;
        }

        public static Label SolSecondaryLabel(string text) {
            var label = new Label(text);
            label.AddToClassList(StyleHelper.ClassTextSecondary);

            return label;
        }

        public static Button SolButton(string text, System.Action onClick) {
            var button = new Button(onClick)
                         {
                             text = text
                         };

            button.AddToClassList(StyleHelper.ClassButton);

            return button;
        }

        public static Button SolPrimaryButton(string text, System.Action onClick) {
            var button = new Button(onClick)
                         {
                             text = text
                         };

            button.AddToClassList(StyleHelper.ClassButtonPrimary);

            return button;
        }

        public static VisualElement SolCard() {
            var card = new VisualElement();
            card.AddToClassList(StyleHelper.ClassCard);

            return card;
        }

        public static VisualElement SolDivider() {
            var divider = new VisualElement();
            divider.AddToClassList(StyleHelper.ClassDivider);

            return divider;
        }

        public static VisualElement SolRow(bool highlighted = false) {
            var row = new VisualElement();
            row.style.flexDirection = FlexDirection.Row;
            row.style.alignItems = Align.Center;

            row.AddToClassList(highlighted ? StyleHelper.ClassBgHighlight : StyleHelper.ClassBackground);

            return row;
        }

        // Root container — call this instead of manually applying the stylesheet
        public static VisualElement SolRoot() {
            var root = new VisualElement();
            StyleHelper.ApplyTo(root);
            root.AddToClassList(StyleHelper.ClassBackground);
            root.style.flexGrow = 1;

            return root;
        }

    }
}