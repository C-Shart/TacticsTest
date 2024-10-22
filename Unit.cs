using Godot;
using GColl = Godot.Collections;
using System;

[Tool]
public partial class Unit : Path2D
{
    public GColl.Array<Vector2> points = new GColl.Array<Vector2>();

    [Signal]
    public delegate void WalkFinishedEventHandler();

    private Sprite2D _sprite;
    private AnimationPlayer _animPlayer;
    private PathFollow2D _pathFollow;

    [Export]
    public Resource Grid { get; set; }
    // public Grid Grid { get; set; }
    // public Resource Grid { get; set; } = GD.Load<Grid>("Grid.tres") as Grid;

    [Export]
    public int MoveRange { get; set; } = 6;

    [Export]
    public double MoveSpeed { get; set; } = 400.0;

    [Export]
    public Texture Skin { get; set; }

    [Export]
    public Vector2 SkinOffset { get; set; }

    public Vector2 cell;
    public Vector2 position;
    public bool isSelected;

    private bool _isWalking;

    public override void _Ready()
    {
        _sprite = GetNode<Sprite2D>("Sprite");
        _animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        _pathFollow = GetNode<PathFollow2D>("PathFollow2D");

        Skin = _sprite.Texture;
        SkinOffset = _sprite?.Position ?? Vector2.Zero;

        isSelected = false;
        _isWalking = false;

        SetProcess(false);
        _pathFollow.Rotates = false;

        if (Grid is Grid grid)
        {
            cell = grid.CalculateGridCoordinates(position);
            position = grid.CalculateMapPosition(cell);
        }
        // cell = Grid.CalculateGridCoordinates(position);
        // position = Grid.CalculateMapPosition(cell);

        if (!Engine.IsEditorHint())
        {
            Curve = new Curve2D();
        }


        // TESTING STUFF, DELETE
        Vector2 item1 = new Vector2(2, 6);
        Vector2 item2 = new Vector2(2, 17);
        Vector2 item3 = new Vector2(3, 2);
        Vector2 item4 = new Vector2(6, 4);
        Vector2 item5 = new Vector2(9, 5);
        Vector2 item6 = new Vector2(10, 5);

        points.Insert(0, item1);
        points.Insert(1, item2);
        points.Insert(2, item3);
        points.Insert(3, item4);
        points.Insert(4, item5);
        points.Insert(5, item6);

    }

    public override void _Process(double delta)
    {
        _pathFollow.Progress += (float)MoveSpeed * (float)delta;

        if (_pathFollow.ProgressRatio >= 1.0)
        {
            _isWalking = false;
            // Setting this value to 0.0 causes a Zero Length Interval error
            _pathFollow.Progress = 0.00001f;
            if (Grid is Grid grid) {position = grid.CalculateMapPosition(cell);}
            Curve.ClearPoints();
            EmitSignal(SignalName.WalkFinished); // TODO: Include signals
        }
    }

    public void WalkAlong(Vector2[] path)
    {
        if (path.IsEmpty())
        {
            return;
        }

        Curve.AddPoint(Vector2.Zero);
        foreach (Vector2 point in path)
        {
            if (Grid is Grid grid) {Curve.AddPoint(grid.CalculateMapPosition(point) - position);}
        }
        cell = path[^1];
        _isWalking = true;
    }

}
