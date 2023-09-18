using Godot;
using System;

public partial class FieldResource : Resource
{
    [Export]
    public bool IsHidden { get; set; }
    [Export]
    public bool HasBomb { get; set; }
    [Export]
    public bool HasFlag { get; set; }
    [Export]
    public int AdjacentBombs { get; set; }

    public Vector2 position { get; set; }


    public FieldResource() : this(true, false, false, 0) { }

    public FieldResource(bool isHidden=true, bool hasBomb=false, bool hasFlag=false, int adjacentBombs=0)
    {
        this.IsHidden = isHidden;
        this.HasBomb = hasBomb;
        this.HasFlag = hasFlag;
        this.AdjacentBombs = adjacentBombs;
    }
}
