using Godot;
using System;

public partial class Player : StaticBody2D
{

	public Marker2D hand;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		hand = GetNode<Marker2D>("Hand");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
