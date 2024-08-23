using Godot;
using System;

public partial class HurtboxComponent : Area2D
{
	[Export]
	public HealthComponent Health;
	[Export]
	public int InvincibilityFrames = 5;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        AreaEntered += OnHurtboxAreaEntered;
	}

	//
    private void OnHurtboxAreaEntered(Area2D area)
    {
		var hitbox = area as HitboxComponent;
		var attackData = hitbox.GetAttackData();
		Health.TakeDamage(attackData.damage);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
	}
}
