using Godot;
using System;

public partial class Game : Node2D
{
	Player player;
	Target target;

	Weapon weapon;

	bool isSwinging = false;

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

		if (Input.IsActionJustReleased("swing"))
		{
			SwingWeapon();
		}
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
		if (isSwinging)
		{
			return;
		}
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

	private async void SwingWeapon()
	{
		isSwinging = true;
		var targetAngle = 45f;
		var angleToRotate = targetAngle - weapon.RotationDegrees;

		var tween = CreateTween();
		tween.TweenProperty(weapon, new NodePath("rotation_degrees"), targetAngle, 0.05f).SetTrans(Tween.TransitionType.Spring);
		await ToSignal(tween, "finished");
		isSwinging = false;
	}
}
