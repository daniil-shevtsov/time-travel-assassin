using Godot;
using System;

public partial class Game : Node2D
{
	Player player;
	Target target;

	Weapon weapon;

	bool isSwinging = false;

	float playerSpeed = 500;

	private Vector2 lastMousePosition = Vector2.Zero;

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

		target.LookAt(player.GlobalPosition);
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
		lastMousePosition = eventMouseMotion.Position;
		if (isSwinging)
		{
			return;
		}
		var armLength = 450f - 30f;
		var sensitivity = 0.75f;
		var distanceChange = eventMouseMotion.Relative * 0.5f;
		var playerMouseVector = player.GlobalPosition.DistanceTo(eventMouseMotion.Position);
		weapon.LookAt(eventMouseMotion.Position);
		GD.Print($"real rotation mouse={lastMousePosition} hand={player.hand.GlobalPosition} final rotation = {weapon.RotationDegrees}");

		// weapon.RotationDegrees += 90f;
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
		var overSwing = 0f;
		if (weapon.RotationDegrees <= -45f)
		{
			overSwing = 25f;
		}
		else
		{
			overSwing = -25f;
		}
		var targetAngle = -45f + overSwing;


		var tween = CreateTween();
		tween.TweenProperty(weapon, new NodePath("rotation_degrees"), targetAngle, 0.05f).SetTrans(Tween.TransitionType.Spring);
		await ToSignal(tween, "finished");
		tween = CreateTween();
		var lastRotation = weapon.RotationDegrees;
		weapon.LookAt(lastMousePosition);
		var lookAtRotation = weapon.RotationDegrees;
		weapon.RotationDegrees = lastRotation;
		GD.Print($"mouse={lastMousePosition} hand={player.hand.GlobalPosition} weapon={weapon.GlobalPosition} weaponRotationDegrees={weapon.RotationDegrees} newRotation={lookAtRotation}");
		tween.TweenProperty(weapon, new NodePath("rotation_degrees"), lookAtRotation, 0.15f).SetTrans(Tween.TransitionType.Spring);
		await ToSignal(tween, "finished");
		GD.Print($"mouse={lastMousePosition} hand={player.hand.GlobalPosition} final rotation = {weapon.RotationDegrees}");

		isSwinging = false;
	}
}
