using Godot;
using System;

public partial class Game : Node2D
{
	Player player;
	Target target;

	Weapon weapon;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		player = GetNode<Player>("Player");
		target = GetNode<Target>("Target");
		weapon = GetNode<Weapon>("Weapon");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Vector2 inputDir = Input.GetVector("left", "right", "forward", "backward");
		player.MoveAndCollide(inputDir);
	}
}
