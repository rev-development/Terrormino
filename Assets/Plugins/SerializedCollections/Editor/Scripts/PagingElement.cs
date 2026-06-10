using UnityEditor;
using UnityEngine;

namespace AYellowpaper.SerializedCollections.Editor
{
    public class PagingElement
    {

        private const int buttonWidth = 20;

        private const int inputWidth = 20;

        private const int labelWidth = 30;

        private int _page = 1;

        private int _pageCount = 1;

        public PagingElement(int pageCount = 1)
        {
            PageCount = pageCount;
        }

        public int Page
        {
            get => _page;
            set
            {
                _page = value;
                EnsureValidPageIndex();
            }
        }
        public int PageCount
        {
            get => _pageCount;
            set
            {
                Debug.Assert(value >= 1, $"{nameof(PageCount)} needs to be 1 or larger but is {value}.");
                _pageCount = value;
                EnsureValidPageIndex();
            }
        }

        public float GetDesiredWidth()
        {
            return (buttonWidth * 2) + inputWidth + labelWidth;
        }

        public void OnGUI(Rect rect)
        {
            var leftButton = rect.WithXAndWidth(rect.x, buttonWidth);
            var inputRect = leftButton.AppendRight(inputWidth);
            var labelRect = inputRect.AppendRight(labelWidth);
            var rightButton = labelRect.AppendRight(buttonWidth);

            using (new GUIEnabledScope(Page != 1))
            {
                if (GUI.Button(leftButton, "<"))
                {
                    Page--;
                }
            }

            using (new GUIEnabledScope(Page != PageCount))
            {
                if (GUI.Button(rightButton, ">"))
                {
                    Page++;
                }
            }

            Page = EditorGUI.IntField(inputRect, Page);
            GUI.Label(labelRect, "/" + PageCount);
        }

        private void EnsureValidPageIndex()
        {
            _page = Mathf.Clamp(_page, 1, PageCount);
        }

    }
}