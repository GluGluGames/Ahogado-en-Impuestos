using UnityEngine;


namespace GGG.Components.Buildings {
    public static class HexUtilities {
        public static Vector3Int OffsetToCube(Vector2Int offset) {
            var q = offset.x - (offset.y + (offset.y % 2)) / 2;
            var r = offset.y;

            return new Vector3Int(q, r, -q - r);
        }

        public static Vector3 GetPositionForHexFromCoordinate(Vector2Int initialOffset, Vector2Int coord, float size, bool isFlatTopped) {
            int column = coord.x;
            int row = coord.y;

            int realCol = coord.x + initialOffset.x;
            int realRow = coord.y + initialOffset.y;
            float width;
            float height;
            float xPosition;
            float yPosition;
            bool shouldOffset;
            float horizontalDistance;
            float verticalDistance;
            float offset;
            float radius = size;

            if (!isFlatTopped) {
                shouldOffset = (row % 2) == 0;
                width = Mathf.Sqrt(3) * radius;
                height = 2f * radius;

                horizontalDistance = width;
                verticalDistance = height * (3f / 4f);

                offset = (shouldOffset) ? width / 2 : 0;

                xPosition = (realCol * (horizontalDistance)) + offset;
                yPosition = (realRow * verticalDistance);
            } else {
                shouldOffset = (column % 2) == 0;
                width = 2f * radius;
                height = Mathf.Sqrt(3) * radius;

                horizontalDistance = width * (3f / 4f);
                verticalDistance = height;

                offset = shouldOffset ? height / 2 : 0;
                xPosition = (column * (horizontalDistance));
                yPosition = (row * verticalDistance) - offset;
            }

            return new Vector3(xPosition, 0, -yPosition);
        }
    }
}


