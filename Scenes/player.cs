using Godot;
using System;

public partial class player : CharacterBody2D
{
    [Export]
    public string PlayerIndex = "0";

    // exports
    [Export]
    public float FallStunVelocityMin = 600f;

    [Export]
    public float FallStunVelocityMax = 1000f;

    [Export]
    public int JumpBufferFrameWindow = 5;

    [Export]
    public float GroundAcceleration = 50f;

    [Export]
    public float AirAcceleration = 10f;

    [Export]
    public float Friction = 100f;

    [Export]
    public float UpwardsGravity = 500f;

    [Export]
    public float DownwardsGravity = 980f;

    [Export]
    public int CoyoteFrames = 5;

    public const float MaxSpeed = 300.0f;
    private float Speed = MaxSpeed;

    public const float JumpVelocity = -400.0f;

    // Get the gravity from the project settings to be synced with RigidBody nodes.
    public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

    private int jumpBufferFrame = 0;
    private bool isJumpBuffered = false;

    private int framesSinceGround = 0;
    private Vector2 lastVelocity = Vector2.Zero;

    public override void _Ready()
    {

    }
    public override void _PhysicsProcess(double delta)
    {
        Vector2 velocity = Velocity;

        // Handle Jump.
        velocity = HandleJump();

        // Get the input direction and handle the movement/deceleration.
        // As good practice, you should replace UI actions with custom gameplay actions.
        Vector2 direction = Input.GetVector("move_left" + PlayerIndex, "move_right" + PlayerIndex, "move_up" + PlayerIndex, "move_down" + PlayerIndex);
        //if inputting direction
        if (direction != Vector2.Zero)
        {
            if (IsOnFloor())
            {
                //if input opposite to velocity
                if (velocity.X * direction.X < 0)
                {
                    //turn around quickly using strong friction
                    velocity.X = Mathf.MoveToward(Velocity.X, direction.X * Speed, Friction);
                }
                else
                {
                    //don't turn around, move with normal acceleration
                    velocity.X = Mathf.MoveToward(Velocity.X, direction.X * Speed, GroundAcceleration);
                }

            }
            else
            {
                //if input opposite to velocity
                if (velocity.X * direction.X < 0)
                {
                    velocity.X = Mathf.MoveToward(Velocity.X, direction.X * Speed, AirAcceleration * 2);
                }
                else
                {
                    velocity.X = Mathf.MoveToward(Velocity.X, direction.X * Speed, AirAcceleration);
                }
            }
               
        }
        else
        {
            if (IsOnFloor())
            {
                velocity.X = Mathf.MoveToward(Velocity.X, 0, Friction);
            }
            else
            {
                velocity.X = Mathf.MoveToward(Velocity.X, 0, AirAcceleration);
            }

        }

        // Add the gravity.
        if (!IsOnFloor())
            velocity.Y += gravity * (float)delta;

        Velocity = velocity;
        lastVelocity = velocity;
        MoveAndSlide();
    }

    private Vector2 HandleJump()
    {
        Vector2 velocity = Velocity;

        //if we're on the floor...
        if (IsOnFloor())
        {
            //reset our frames since ground count
            framesSinceGround = 0;

            //if we've buffered a jump...
            if (isJumpBuffered)
            {
                //and it's within the window...
                if (jumpBufferFrame < JumpBufferFrameWindow)
                {
                    velocity = Jump();
                }
                else
                {
                    //if we were too late, let's reset
                    isJumpBuffered = false;
                    jumpBufferFrame = 0;
                }
            }

            //if we press jump...
            if (Input.IsActionJustPressed("jump" + PlayerIndex))
            {
                //duh
                velocity = Jump();
            }
        }
        else
        //we must be in the air if we're not on the floor
        {
            //that's one more frame off the ground.
            framesSinceGround++;

            if (Input.IsActionJustPressed("jump" + PlayerIndex))
            {
                //if we're within the coyote window...
                if (framesSinceGround < CoyoteFrames)
                {
                    //allow the jump anyways!
                    velocity = Jump();
                }

                //buffer the jump
                isJumpBuffered = true;
                jumpBufferFrame = 0;
            }
        }

        //if we've released jump for any reason...
        if (Input.IsActionJustReleased("jump" + PlayerIndex))
        {
            gravity = DownwardsGravity;
        }

        //if we're currently buffering a jump, make sure we track
        //how many frames we've passed without jumping
        if (isJumpBuffered)
        {
            jumpBufferFrame++;
        }

        return velocity;
    }

    private Vector2 Jump()
    {
        var velocity = Velocity;
        velocity.Y = JumpVelocity;
        isJumpBuffered = false;
        jumpBufferFrame = 0;
        gravity = UpwardsGravity;

        return velocity;
    }
}
