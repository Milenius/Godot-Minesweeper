using Godot;
using System;
using System.ComponentModel.Design;

public partial class Field : Node2D
{
	[Export]
	public FieldResource FieldResource;

    [Signal]
    public delegate void FieldRevealedEventHandler(int x, int y);

    [Signal]
    public delegate void FlagPlantedEventHandler();

    [Signal]
    public delegate void FlagRemovedEventHandler();

    [Signal]
    public delegate void BombHitEventHandler();

	Sprite2D hiddenSprite;
	Sprite2D flagSprite;
	Sprite2D numberSprite;
    Sprite2D explosionSprite;

    Area2D area;

    public override void _Ready()
	{
		hiddenSprite = GetNode<Sprite2D>("HiddenSprite");
		flagSprite = GetNode<Sprite2D>("FlagSprite");
		numberSprite = GetNode<Sprite2D>("NumberSprite");
        explosionSprite = GetNode<Sprite2D>("ExplosionSprite");

        area = GetNode<Area2D>("Area2D");
        area.InputEvent += Area_InputEvent;
	}

    private void Area_InputEvent(Node viewport, InputEvent @event, long shapeIdx)
    {
        if (@event is InputEventMouseButton eventMouseButton && eventMouseButton.IsPressed())
        {
            if (!FieldResource.IsHidden)
                return;

            if (eventMouseButton.ButtonIndex == MouseButton.Left)
            {
                FieldResource.IsHidden = false;
                EmitSignal(SignalName.FieldRevealed, FieldResource.position.X, FieldResource.position.Y);

                if (FieldResource.HasBomb)
                    EmitSignal(SignalName.BombHit);

            } else
            {
                FieldResource.HasFlag = !FieldResource.HasFlag;

                if (FieldResource.HasFlag)
                    EmitSignal(SignalName.FlagPlanted);
                else
                    EmitSignal(SignalName.FlagRemoved);
            }
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{	
		if (FieldResource is FieldResource)
		{
            if (FieldResource.IsHidden)
                hiddenSprite.Visible = true;
            else hiddenSprite.Visible = false;

            if (FieldResource.IsHidden && FieldResource.HasFlag)
                flagSprite.Visible = true;
            else flagSprite.Visible = false;

            if (!FieldResource.IsHidden && FieldResource.HasBomb)
                explosionSprite.Visible = true;


            if (!FieldResource.IsHidden && FieldResource.AdjacentBombs != 0)
            {
                numberSprite.Visible = true;
                string adjacentBombs = FieldResource.AdjacentBombs.ToString();
                string spriteFile = "number_" + adjacentBombs + ".png";

                numberSprite.Texture = (Texture2D)GD.Load("res://assets/" + spriteFile);
            }
        }
	}
}
