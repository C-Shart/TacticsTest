// Code originally from https://tweakgame.com/godot-engine-build-custom-grid-system-using-csharp/

using Godot;
using System;
using System.Collections.Generic;

public class CustomGrid<TItem>
{
    public delegate void GridItemChangedDelegate(int coordinateX, int coordinateY, TItem obj);

    public event GridItemChangedDelegate GridItemChanged;

    public int Height { get; private set; }
    public int CellSize { get; private set; }
    public int Width { get; private set; }

    public TItem[,] GridCellCollection { get; private set; }

    public List<Vector2> GridPositions { get; private set; }

    private readonly Func<Vector2> _originPosition;
    public CustomGrid(int width, int height, int cellSize, Func<Vector2> originPosition = null)
    {
        _originPosition = originPosition;
        Height = height;
        CellSize = cellSize;
        Width = width;
        GridCellCollection = new TItem[width, height];
        GridPositions = new();
        // Initialize grid cells and positions
        for (int i = 0; i < Width; i++)
        {
            for (int j = 0; j < Height; j++)
            {

                GridCellCollection[i, j] = default;
                GridPositions.Add(GetWorldPosition(i, j));
            }
        }
    }
    private void CreateGrid()
    {
        for (int i = 0; i < Width; i++)
        {
            for (int j = 0; j < Height; j++)
            {
                //each cell data
            }
        }
    }


    private Vector2 GetParentPosition() => _originPosition?.Invoke() ?? Vector2.Zero;

    private bool TryGetGridCoordinate(Vector2 pos, Vector2 parentPos, out (int x, int y) coordinate)
    {
        var x = Mathf.FloorToInt((pos.X - parentPos.X) / (CellSize));
        var y = Mathf.FloorToInt((pos.Y - parentPos.Y) / (CellSize));
        coordinate = (x, y);
        return x >= 0 && x < Width && y >= 0 && y < Height;
    }

    private bool IsCellEmpty(int x, int y)
    {
        var obj = GridCellCollection[x, y];
        return obj == null || EqualityComparer<TItem>.Default.Equals(obj, default);
    }

    private bool IsCellEmpty(int x, int y, out TItem item)
    {
        item = GridCellCollection[x, y];
        return GridCellCollection[x, y] == null || EqualityComparer<TItem>.Default.Equals(GridCellCollection[x, y], default);
    }

    public bool IsCellEmpty(Vector2 pos)
    {
        return TryGetGridCoordinate(pos, GetParentPosition(), out var coordinate) && IsCellEmpty(coordinate.x, coordinate.y);
    }

    private Vector2 GetWorldPosition(int x, int y) => new Vector2(x * CellSize, y * CellSize) + GetParentPosition();

    public bool TrySetItem(Vector2 pos, TItem obj, out Vector2 positionToSet)
    {
        if (obj == null)
        {
            positionToSet = Vector2.Zero;
            return false;
        }
        var parentPos = GetParentPosition();
        if (TryGetGridCoordinate(pos, parentPos, out var coordinate) && IsCellEmpty(coordinate.x, coordinate.y))
        {
            GridCellCollection[coordinate.x, coordinate.y] = obj;
            positionToSet = new Vector2(coordinate.x * CellSize, coordinate.y * CellSize) + parentPos;
            GridItemChanged?.Invoke(coordinate.x, coordinate.y, obj);
            return true;
        }
        positionToSet = Vector2.Zero;
        return false;
    }
    public void TriggerItemChangedEvent(int x, int y)
    {
        GridItemChanged?.Invoke(x, y, GridCellCollection[x, y]);
    }

    public bool TryGetItem(Vector2 pos, out TItem obj)
    {
        if (TryGetGridCoordinate(pos, GetParentPosition(), out var coordinate))
        {
            if (!IsCellEmpty(coordinate.x, coordinate.y))
            {
                obj = GridCellCollection[coordinate.x, coordinate.y];
                return true;
            }
        }
        obj = default;
        return false;
    }

    public bool TryRemoveItem(Vector2 pos, out TItem removedItem)
    {
        if (TryGetGridCoordinate(pos, GetParentPosition(), out var coordinate))
        {
            if (IsCellEmpty(coordinate.x, coordinate.y, out removedItem))
            {
                return false;
            }
            GridCellCollection[coordinate.x, coordinate.y] = default;
            return true;
        }
        removedItem = default;
        return false;
    }
}