using Godot;
using System;

public partial class Game : Node2D
{
	Player player;
	Target target;

	Weapon weapon;

	float playerSpeed = 500;

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
		Vector2 inputDir = Input.GetVector("left", "right", "forward", "backward").Normalized() * playerSpeed * (float)delta;
		player.MoveAndCollide(inputDir);

		weapon.GlobalPosition = player.hand.GlobalPosition;
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseMotion eventMouseMotion)
		{
			RotateWeapon(eventMouseMotion);
		}
	}

	private void RotateWeapon(InputEventMouseMotion eventMouseMotion)
	{
		var armLength = 450f - 30f;
		var sensitivity = 0.75f;
		var distanceChange = eventMouseMotion.Relative * 0.5f;
		var playerMouseVector = player.GlobalPosition.DistanceTo(eventMouseMotion.Position);
		weapon.LookAt(eventMouseMotion.Position);
		weapon.RotationDegrees += 90f;
		// if (newDistance <= armLength)
		// {
		//     var collision = Gun.MoveAndCollide(distanceChange);
		//     if (collision != null)
		//     {
		//         HandleCollision(collision);
		//     }
		//     else
		//     {
		//         Gun.GlobalPosition += distanceChange;
		//     }
		// }
	}
}
