using Godot;
using System;

public partial class Main : Node2D
{
	private int FieldSize = 16;
	private int HorizontalFields = 24;
	private int VerticalFields = 24;

	private int BombPercentage = 15;

    private int AmountFlags = 0;
    private int AmountBombs = 0;
    private bool GameLost = false;

    private bool GameOver  = false;

    private Field[,] FieldState;
	private Random RandomGen = new Random();

	private NinePatchRect Background;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		int viewPortWidth = FieldSize * VerticalFields;
		int viewPortHeight = FieldSize * HorizontalFields;
		Background = GetNode<NinePatchRect>("NinePatchRect");

		GetViewport().GetWindow().ContentScaleSize = new Vector2I(viewPortWidth+6, viewPortHeight+6);
		Background.Size = new Vector2(viewPortWidth+6, viewPortHeight+6);

		FieldState = new Field[HorizontalFields, VerticalFields];

        PackedScene fieldScene = GD.Load<PackedScene>("res://scenes/Field.tscn");
		
		for (int i = 0; i < HorizontalFields; i++)
		{
			for (int j = 0; j < VerticalFields; j++)
			{
				FieldState[i, j] = fieldScene.Instantiate() as Field;
				FieldState[i, j].GlobalPosition = (new Vector2(i,j) * FieldSize) + new Vector2(3,3);
				AddChild(FieldState[i, j]);

				// FieldState[i, j].FieldResource.IsHidden = false;
                FieldState[i, j].FieldResource.position = new Vector2(i,j);
          
                FieldState[i, j].FieldRevealed += WaveReveal;
                FieldState[i, j].FlagPlanted += () => AmountFlags++;
                FieldState[i, j].FlagRemoved += () => AmountFlags--;
                FieldState[i, j].BombHit += () => { GameLost = true; };


                if (RandomGen.Next(100) < BombPercentage)
                {
					FieldState[i, j].FieldResource.HasBomb = true;
                    AmountBombs++;
                }
            }
		}

		UpdateAdjacentValues();
    }

    public override void _Process(double delta)
    {
        if (AmountBombs == AmountFlags && !GameLost && !GameOver)
        {
            GD.Print("You Won!!! :)");
            GameOver = true;
        }
        else if (GameLost && !GameOver)
        {
            GD.Print("You Lost!!! :C");
            GameOver = true;
        }
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("reset_key"))
		{
			GD.Print("Reset Key");
			GetTree().ChangeSceneToPacked(GD.Load<PackedScene>("res://scenes/Main.tscn"));
		}
    }

    public bool FieldExists(int x, int y)
    {
        return x >= 0 && y >= 0 && x < HorizontalFields && y < VerticalFields;
    }

	public void WaveReveal(int x, int y)
	{
        if (!FieldExists(x, y)) return;
        
        Vector2I[] checkPositions = {   new Vector2I(x - 1, y), new Vector2I(x + 1, y), new Vector2I(x, y - 1), new Vector2I(x, y + 1),
                                        new Vector2I(x - 1, y - 1), new Vector2I(x + 1, y + 1), new Vector2I(x + 1, y - 1), new Vector2I(x - 1, y + 1) };

        FieldState[x, y].FieldResource.IsHidden = false;
        
        if (FieldState[x,y].FieldResource.AdjacentBombs > 0) return;

        foreach (Vector2I pos in checkPositions)
        {
            if (FieldExists(pos.X, pos.Y))
            {
                if (FieldState[pos.X, pos.Y].FieldResource.IsHidden)
                    WaveReveal(pos.X, pos.Y);              
            }
        }
    }

    public void UpdateAdjacentValues()
	{
        for (int i = 0; i < HorizontalFields; i++)
        {
            for (int j = 0; j < VerticalFields; j++)
            {
				FieldState[i,j].FieldResource.AdjacentBombs = CalcAdjacentValue(i, j);
            }
        }
    }

	public int CalcAdjacentValue(int x, int y)
	{
		int tempSum = 0;

        for (int i = x-1; i <= x+1; i++)
        {
            for (int j = y-1; j <= y+1; j++)
            {
                
                if ((i == x && j == y) || i < 0 || j < 0 || i > HorizontalFields-1 || j > VerticalFields-1)
				{
					;
                } else
				{
                    tempSum += Convert.ToInt32(FieldState[i, j].FieldResource.HasBomb);
                }
            }
        }
		return tempSum;
    }
}
