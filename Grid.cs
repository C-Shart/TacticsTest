using Godot;
using System;

[GlobalClass]
public partial class Grid : Resource
{
    private int _cols = 37;
    private int _rows = 24;
    private int _tileSize = 32;

    // Grid size in rows and columns
    [Export]
    Vector2 Size { get; set; } = new Vector2(37, 24);

    // Size of individual cells/tiles
    [Export]
    Vector2 CellSize { get; set; } = new Vector2(32, 32);


    // Parameterless constructor test
    public Grid() : this(0, 0, 0) {}

    public Grid(int cols, int rows, int tileSize)
    {
        Size = new Vector2(cols, rows);
        CellSize = new Vector2(tileSize, tileSize);
    }

    // Get pixel pos from grid coords
    public Vector2 CalculateMapPosition(Vector2 gridPosition)
    {
        Vector2 _halfCellSize = CellSize / 2;
        return gridPosition * CellSize + _halfCellSize;
    }

    // Get grid coords from pixel pos
    public Vector2 CalculateGridCoordinates(Vector2 mapPosition)
    {
        return (mapPosition / CellSize).Floor();
    }

    public bool IsWithinBounds(Vector2 cellCoordinates)
    {
        bool inside_x = cellCoordinates.X >= 0 && cellCoordinates.X < Size.X;
        bool inside_y = cellCoordinates.Y >= 0 && cellCoordinates.Y < Size.Y;
        return inside_x && inside_y;
    }

    public Vector2 GridClamp(Vector2 gridPosition)
    {
        Vector2 clampedPosition = gridPosition;
        clampedPosition.X = Mathf.Clamp(clampedPosition.X, 0, Size.X - 1);
        clampedPosition.Y = Mathf.Clamp(clampedPosition.Y, 0, Size.Y - 1);
        return clampedPosition;
    }

    public int AsIndex(Vector2 cell)
    {
        return (int)(cell.X + Size.X * cell.Y);
    }

}