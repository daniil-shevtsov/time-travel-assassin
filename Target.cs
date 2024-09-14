using Godot;
using System;

public partial class Target : StaticBody2D
{

	public Node2D spriteContainer;
	public Sprite2D eyeRight;
	public Sprite2D eyeLeft;
	public RectangleShape2D shape;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		spriteContainer = GetNode<Node2D>("EnemySpriteContainer");
		eyeRight = spriteContainer.GetNode<Sprite2D>("EyeRight");
		eyeLeft = spriteContainer.GetNode<Sprite2D>("EyeLeft");
		shape = (RectangleShape2D)GetNode<CollisionShape2D>("CollisionShape2D").Shape;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
