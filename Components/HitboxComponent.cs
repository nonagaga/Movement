using Godot;
using System;
using System.Linq;

[Tool]
public partial class HitboxComponent : Area2D
{
	public struct AttackData
	{
		public Vector2 AttackDirection;
		public int damage;
		public int knockbackStrength;
	}

	[Export]
	public RayCast2D attackDirectionRay
	{
		get
		{
			return attackDirectionRay;
		}

		set
		{
			attackDirectionRay = value;

			if (Engine.IsEditorHint())
			{
				UpdateConfigurationWarnings();
			}
		}
	}


	[Export]
	public int damage = 10;

	[Export]
	public int knockbackStrength = 10;

    public override string[] _GetConfigurationWarnings()
    {
		string[] warnings = base._GetConfigurationWarnings();
        if (attackDirectionRay == null)
		{
			warnings.Append("no attack direction is attached");
		}

		return warnings;
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		attackDirectionRay = GetNode<RayCast2D>("AttackDirection");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public AttackData GetAttackData()
	{
		var attackDirection = Raycast2DToVector2(attackDirectionRay);
		return new AttackData
		{
			AttackDirection = attackDirection,
			damage = damage,
			knockbackStrength = knockbackStrength,
		};
	}

	private Vector2 Raycast2DToVector2(RayCast2D rayCast)
	{
		var x = rayCast.TargetPosition.X;
		//invert y, since raycasts use pixel coordinates
		var y = rayCast.TargetPosition.Y * -1;

		return new Vector2(x, y).Normalized();
	}
}
