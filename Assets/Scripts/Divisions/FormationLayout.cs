using GameStudio.HunterGatherer.Utilities;
using System.Collections.Generic;
using UnityEngine;

namespace GameStudio.HunterGatherer.Divisions
{
    /// <summary> Methods for generating formation layouts </summary>
    public class FormationLayout
    {
        /// <summary>
        /// Generates the layout of the division when placing a movement order.
        /// </summary>
        /// <param name="formationSize">The size of the formation</param>
        /// <param name="pivot">The point around with the division should form</param>
        /// <param name="direction">The direction the division formation should point towards</param>
        /// <param name="ratio">MUST BE ONE OR HIGHER, incrementing this value decreases rows and increases columns in the formation</param>
        /// <param name="radius">Distance between units</param>
        public static List<Vector3> GeneratePositions(int formationSize, Vector3 pivot, Vector3 direction, float ratio = 1f, float radius = 2f)
        {
            List<Vector3> formationPositions = new List<Vector3>();
            Vector3 dirRight = Quaternion.Euler(0f, 90f, 0f) * direction.normalized;
            Vector3 dirBack = -direction.normalized;

            // Incrementing R decreases rows and increases columns.
            int maxRow = Mathf.CeilToInt(formationSize / ratio);
            int maxColumn = (int)ratio;
            int currentMaxColumn = maxColumn;

            // Calculate the pivot offset to ensure unit location matches decal position
            float columnOffset = radius * (maxColumn / 2f - 0.5f);
            float rowOffset = radius * (maxRow / 2f - 0.5f);
            Vector3 pivotOffset = pivot - dirRight * columnOffset - dirBack * rowOffset;

            for (int row = 0; row < maxRow; row++)
            {
                // If it's the last row, and the formation divided by maxrow isn't an integer, obtain the remainder of units
                if (row == maxRow - 1 && !(formationSize / (float)maxRow).IsInteger())
                {
                    currentMaxColumn = formationSize - (Mathf.FloorToInt(formationSize / maxColumn) * maxColumn);
                    columnOffset = radius * (currentMaxColumn / 2f - 0.5f);
                    pivotOffset = pivot - dirRight * columnOffset - dirBack * rowOffset;
                }

                for (int column = 0; column < currentMaxColumn; column++)
                {
                    formationPositions.Add(pivotOffset + (dirRight * radius * column) + (dirBack * radius * row));
                }
            }
            return formationPositions;
        }

        /// <summary> Generates a rectangle big enough to contain the entire formation </summary>
        public static Rect GenerateRect(int formationSize, float ratio = 1f, float radius = 2f)
        {
            float unitRadius = 0.5f;

            // Incrementing R decreases rows and increases columns.
            int maxRow = Mathf.CeilToInt(Mathf.Sqrt(formationSize / ratio));
            int maxColumn = Mathf.CeilToInt(formationSize / (float)maxRow);

            return new Rect
            {
                width = (radius * (maxColumn - 1) + unitRadius * 2),
                height = (radius * (maxRow - 1) + unitRadius * 2),
            };
        }
    }
}