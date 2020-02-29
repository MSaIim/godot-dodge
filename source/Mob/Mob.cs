using Godot;
using System;

public class Mob : RigidBody2D
{
    private static Random random = new Random();

    [Export]
    public int minSpeed = 150;

    [Export]
    public int maxSpeed = 250;

    private string[] mobTypes = { "walk", "swim", "fly" };

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        var sprite = GetNode<AnimatedSprite>("AnimatedSprite");
        sprite.Animation = this.mobTypes[random.Next(0, this.mobTypes.Length)];
        sprite.Play();
    }

    public void OnVisibilityScreenExited()
    {
        QueueFree();
    }
}
