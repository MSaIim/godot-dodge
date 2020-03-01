using Godot;
using System;

public class Player : Area2D
{
    [Signal]
    public delegate void Hit();

    [Export]
    public int speed = 400;         // How fast the player will move (pixels/sec).

    private Vector2 screenSize;    // Size of the game window.
    private Vector2 velocity = new Vector2();

    // Called when node enters scene
    public override void _Ready()
    {
        this.screenSize = this.GetViewport().Size;
        this.Hide();
    }

    public void Start(Vector2 pos)
    {
        this.Position = pos;
        this.Show();
        GetNode<CollisionShape2D>("CollisionShape2D").Disabled = false;
    }

    // Called every frame. delta time is not constant
    public override void _Process(float delta)
    {
        // Reset
        this.velocity.x = 0;
        this.velocity.y = 0;

        // Check what direction key was pressed
        this.Move();

        // Play the animation
        this.Animate();

        // Clamp
        this.Clamp(delta);
    }

    public void OnPlayerBodyEntered(PhysicsBody2D body)
    {
        this.Hide(); // Player disappears after being hit.
        this.EmitSignal("Hit");

        // Disable the player's collision so we don't trigger Hit more than once.
        // Godot waits until it's safe to do so. Don't want to disable in the middle
        // of processing the physics.
        GetNode<CollisionShape2D>("CollisionShape2D").SetDeferred("disabled", true);
    }

    private void Move()
    {
        if (Input.IsActionPressed("ui_right"))
        {
            this.velocity.x += 1;
        }

        if (Input.IsActionPressed("ui_left"))
        {
            this.velocity.x -= 1;
        }

        if (Input.IsActionPressed("ui_up"))
        {
            this.velocity.y -= 1;
        }

        if (Input.IsActionPressed("ui_down"))
        {
            this.velocity.y += 1;
        }
    }

    private void Animate()
    {
        AnimatedSprite sprite = GetNode<AnimatedSprite>("AnimatedSprite");

        // Normalize diagonal movement
        if (this.velocity.Length() > 0)
        {
            this.velocity = this.velocity.Normalized() * this.speed;
            sprite.Play();
        }
        else
        {
            sprite.Stop();
        }

        // Change sprite animation
        if (this.velocity.x != 0)
        {
            sprite.Animation = "right";
            sprite.FlipV = false;
            sprite.FlipH = this.velocity.x < 0;
        }
        else if (this.velocity.y != 0)
        {
            sprite.Animation = "up";
            sprite.FlipV = this.velocity.y > 0;
        }
    }

    private void Clamp(float delta)
    {
        this.Position += this.velocity * delta;
        this.Position = new Vector2(
            x: Mathf.Clamp(this.Position.x, 0, this.screenSize.x),
            y: Mathf.Clamp(this.Position.y, 0, this.screenSize.y)
        );
    }
}
