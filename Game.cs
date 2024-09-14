using Godot;

public partial class Game : Node2D
{
	Player player;
	Target target;

	Weapon weapon;

	bool isSwinging = false;

	static float playerSpeed = 500f;

	static float enemySpeed = playerSpeed / 4f;

	private Vector2 lastMousePosition = Vector2.Zero;

	static float hitBase = 100f;
	static float hitReduction = 10f;
	private Vector2 hitImpulse = Vector2.Zero;

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

		var eyeOffset = calculateEyeOffset(player.GlobalPosition, target.GlobalPosition, target.shape.Size);
		target.eyeLeft.Position = eyeOffset;
		target.eyeRight.Position = eyeOffset;

		target.GlobalPosition = target.GlobalPosition.MoveToward(player.GlobalPosition, enemySpeed * (float)delta);

		if (hitImpulse > Vector2.Zero)
		{
			var amountToMove = hitImpulse * hitReduction * (float)delta;
			target.GlobalPosition += amountToMove;
			hitImpulse -= amountToMove;
			GD.Print($" amountToMove {amountToMove} newPosition {target.GlobalPosition} hitImpulse left {hitImpulse}");
		}
	}

	private Vector2 calculateEyeOffset(Vector2 targetPosition, Vector2 enemyPosition, Vector2 enemySize)
	{
		return new Vector2(
		Mathf.Sign(targetPosition.X - Mathf.Clamp(targetPosition.X, enemyPosition.X - enemySize.X / 2, enemyPosition.X + enemySize.X / 2)),
		Mathf.Sign(targetPosition.Y - Mathf.Clamp(targetPosition.Y, enemyPosition.Y - enemySize.Y / 2, enemyPosition.Y + enemySize.Y / 2))
		);
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
		// GD.Print($"real rotation mouse={lastMousePosition} hand={player.hand.GlobalPosition} final rotation = {weapon.RotationDegrees}");

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
		var targetAngle = 0f;
		var minAngle = -85f;
		var maxAngle = 85f;


		if (weapon.RotationDegrees >= 0f)
		{
			targetAngle = minAngle;
		}
		else
		{
			targetAngle = maxAngle;
		}

		var maxSwing = maxAngle - minAngle;
		var swingArc = Mathf.Abs(weapon.RotationDegrees - targetAngle);
		var swingPercent = Mathf.Min(1f, swingArc / maxSwing);

		var distance = player.hand.GlobalPosition.DistanceTo(target.GlobalPosition);
		var weaponRange = weapon.shape.Size.X;
		var distanceFactor = Mathf.Clamp(1f - (distance / weaponRange), 0f, 1f);
		var hitAmount = hitBase * swingPercent * (1f + distanceFactor * 2f);
		var hitDirection = Vector2.Right;

		if (distance <= weaponRange)
		{
			hitImpulse = hitAmount * hitDirection;
			GD.Print($"HIT base {hitBase} * swing percent {swingPercent} * distance factor {distanceFactor} = amount {hitAmount} * direction {hitDirection} = final {hitImpulse}");

		}

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
