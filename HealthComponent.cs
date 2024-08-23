using Godot;
using System;

public partial class HealthComponent : Node2D
{
	[Signal]
	public delegate void DeadEventHandler();

	[Signal]
	public delegate void DamagedEventHandler(int currentHealth);

	[Export]
	public int maxHealth;

	private int health;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		health = maxHealth;
	}

	public void TakeDamage(int damage) {

		health -= damage;
		Math.Clamp(health, 0, maxHealth);

		EmitSignal(nameof(Damaged),health);

		if(health == 0)
		{
			EmitSignal(nameof(Dead));
		}
	}

	public int GetHealth()
	{
		return health;
	}
}
