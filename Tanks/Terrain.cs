using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Numerics;

public class Terrain
{
    private List<int> heights = new();
    private int columns;
    private int columnWidth;
    private int screenHeight;

    public Terrain(int screenWidth, int screenHeight, int columnWidth = 5)
    {
        this.screenHeight = screenHeight;
        this.columnWidth = columnWidth;
        columns = screenWidth / columnWidth;
        GenerateRandomHeights();
    }

    private void GenerateRandomHeights()
    {
        var rand = new Random();
        int h = screenHeight / 2;
        for (int i = 0; i < columns; i++)
        {
            h += rand.Next(-4, 5);
            h = Math.Clamp(h, 200, screenHeight);
            heights.Add(h);
        }
    }

    public void Piirrä()
    {
        for (int i = 0; i < columns; i++)
        {
            int x = i * columnWidth;
            int h = heights[i];
            Raylib.DrawRectangle(x, h, columnWidth, screenHeight - h, Color.DarkGreen);
        }
    }

    public int GetHeightAt(float x)
    {
        int idx = Math.Clamp((int)(x / columnWidth), 0, columns - 1);
        return heights[idx];
    }

    // Returns true if point is below the current terrain
    public bool OnkoOsuma(Vector2 sijainti)
    {
        if (sijainti.X < 0 || sijainti.X >= columns * columnWidth)
            return true;
        return sijainti.Y >= GetHeightAt(sijainti.X);
    }

    /// <summary>
    /// Lowers the terrain within a circular explosion.
    /// </summary>
    public void DestroyAt(Vector2 center, float radius)
    {
        // Determine which columns could be affected
        int leftIdx = Math.Max(0, (int)((center.X - radius) / columnWidth));
        int rightIdx = Math.Min(columns - 1, (int)((center.X + radius) / columnWidth));

        for (int i = leftIdx; i <= rightIdx; i++)
        {
            // Column center x
            float colX = i * columnWidth + columnWidth / 2f;
            // Horizontal distance from explosion center
            float dx = Math.Abs(colX - center.X);
            if (dx > radius) continue;

            // Compute vertical drop: yDist = sqrt(r^2 - dx^2)
            float yDist = (float)Math.Sqrt(radius * radius - dx * dx);
            // New height = max(old height, explosion bottom)
            int explosionBottom = (int)(center.Y + yDist);
            // We want to lower the terrain: heights increase downward
            heights[i] = Math.Min(heights[i], explosionBottom);
        }
    }
}
