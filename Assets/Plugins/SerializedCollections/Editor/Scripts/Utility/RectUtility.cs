using UnityEngine;

namespace AYellowpaper.SerializedCollections.Editor
{
    public static class RectUtility
    {

        public static Rect WithX(this Rect rect, float x)
        {
            return new Rect(
                    x,
                    rect.y,
                    rect.width,
                    rect.height
                );
        }

        public static Rect WithY(this Rect rect, float y)
        {
            return new Rect(
                    rect.x,
                    y,
                    rect.width,
                    rect.height
                );
        }

        public static Rect WithWidth(this Rect rect, float width)
        {
            return new Rect(
                    rect.x,
                    rect.y,
                    width,
                    rect.height
                );
        }

        public static Rect WithHeight(this Rect rect, float height)
        {
            return new Rect(
                    rect.x,
                    rect.y,
                    rect.width,
                    height
                );
        }

        public static Rect WithPosition(this Rect rect, Vector2 position)
        {
            return new Rect(position, rect.size);
        }

        public static Rect WithPosition(this Rect rect, float x, float y)
        {
            return new Rect(new Vector2(x, y), rect.size);
        }

        public static Rect WithSize(this Rect rect, Vector2 size)
        {
            return new Rect(rect.position, size);
        }

        public static Rect WithSize(this Rect rect, float width, float height)
        {
            return new Rect(rect.position, new Vector2(width, height));
        }

        public static Rect WithXAndWidth(this Rect rect, float x, float width)
        {
            return new Rect(
                    x,
                    rect.y,
                    width,
                    rect.height
                );
        }

        public static Rect WithYAndHeight(this Rect rect, float y, float height)
        {
            return new Rect(
                    rect.x,
                    y,
                    rect.width,
                    height
                );
        }

        public static Rect AppendRight(this Rect rect, float width)
        {
            return new Rect(
                    rect.x + rect.width,
                    rect.y,
                    width,
                    rect.height
                );
        }

        public static Rect AppendRight(this Rect rect, float width, float space)
        {
            return new Rect(
                    rect.x + rect.width + space,
                    rect.y,
                    width,
                    rect.height
                );
        }

        public static Rect AppendLeft(this Rect rect, float width)
        {
            return new Rect(
                    rect.x - width,
                    rect.y,
                    width,
                    rect.height
                );
        }

        public static Rect AppendLeft(this Rect rect, float width, float space)
        {
            return new Rect(
                    rect.x - space - width,
                    rect.y,
                    width,
                    rect.height
                );
        }

        public static Rect AppendUp(this Rect rect, float height)
        {
            return new Rect(
                    rect.x,
                    rect.y - height,
                    rect.width,
                    height
                );
        }

        public static Rect AppendUp(this Rect rect, float height, float space)
        {
            return new Rect(
                    rect.x,
                    rect.y - space - height,
                    rect.width,
                    height
                );
        }

        public static Rect AppendDown(this Rect rect, float height)
        {
            return new Rect(
                    rect.x,
                    rect.y + rect.height,
                    rect.width,
                    height
                );
        }

        public static Rect AppendDown(this Rect rect, float height, float space)
        {
            return new Rect(
                    rect.x,
                    rect.y + rect.height + space,
                    rect.width,
                    height
                );
        }

        public static Rect CutLeft(this Rect rect, float width)
        {
            return new Rect(
                    rect.x + width,
                    rect.y,
                    rect.width - width,
                    rect.height
                );
        }

        public static Rect CutRight(this Rect rect, float width)
        {
            return new Rect(
                    rect.x,
                    rect.y,
                    rect.width - width,
                    rect.height
                );
        }

        public static Rect CutTop(this Rect rect, float height)
        {
            return new Rect(
                    rect.x,
                    rect.y + height,
                    rect.width,
                    rect.height - height
                );
        }

        public static Rect CutBottom(this Rect rect, float height)
        {
            return new Rect(
                    rect.x,
                    rect.y,
                    rect.width,
                    rect.height - height
                );
        }

        public static Rect CutHorizontal(this Rect rect, float leftAndRight)
        {
            return rect.CutHorizontal(leftAndRight, leftAndRight);
        }

        public static Rect CutHorizontal(this Rect rect, float left, float right)
        {
            return new Rect(
                    rect.x + left,
                    rect.y,
                    rect.width - left - right,
                    rect.height
                );
        }

        public static Rect CutVertical(this Rect rect, float topAndBottom)
        {
            return rect.CutVertical(topAndBottom, topAndBottom);
        }

        public static Rect CutVertical(this Rect rect, float top, float bottom)
        {
            return new Rect(
                    rect.x,
                    rect.y + top,
                    rect.width,
                    rect.height - top - bottom
                );
        }

        public static Rect Cut(this Rect rect, float topBottom, float leftRight)
        {
            return rect.Cut(
                    topBottom,
                    leftRight,
                    topBottom,
                    leftRight
                );
        }

        public static Rect Cut(this Rect rect, float top, float right, float bottom, float left)
        {
            return new Rect(
                    rect.x + left,
                    rect.y + top,
                    rect.width - left - right,
                    rect.height - top - bottom
                );
        }

    }
}