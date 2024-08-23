using Godot;
using System;

public partial class HealthBar : ProgressBar
{
	[Export]
	private HealthComponent _healthComponent;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		MaxValue = _healthComponent.maxHealth;
		Value = MaxValue;
        _healthComponent.Damaged += OnDamaged;
	}

    private void OnDamaged(int currentHealth)
    {
        Value = currentHealth;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
	}
}
