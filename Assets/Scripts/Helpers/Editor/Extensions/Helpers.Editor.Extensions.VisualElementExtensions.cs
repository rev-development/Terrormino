using UnityEngine.UIElements;

namespace Helpers.Editor.Extensions
{
    public static class VisualElementExtensions
    {

        public static T WithChildren<T>(this T parent, params VisualElement[] children)
            where T : VisualElement {
            foreach (var child in children) parent.Add(child);

            // Usage
            // root.Add(SolarizedUI.Card().WithChildren(
            //         SolarizedUI.Label("Title", emphasized: true),
            //         SolarizedUI.Divider(),
            //         SolarizedUI.PrimaryButton("Go", () => { })
            //     ));

            return parent;
        }

        public static T WithClass<T>(this T el, string className)
            where T : VisualElement {
            el.AddToClassList(className);

            return el;
        }

        public static T WithStyle<T>(this T el, System.Action<IStyle> styleAction)
            where T : VisualElement {
            styleAction(el.style);

            return el;
        }

    }
}