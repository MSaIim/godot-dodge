using Godot;
using System;

public class Game : Node2D
{
    [Export]
    public PackedScene mob;

    private int score;

    private Random random = new Random();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // Play background music
        GetNode<AudioStreamPlayer>("Music").Play();
    }

    public override void _Process(float delta)
    {
        if (Input.IsActionPressed("ui_esc"))
        {
            GetTree().Quit();
        }
    }

    public void NewGame()
    {
        this.score = 0;

        // Update the HUD with messages
        var hud = GetNode<HUD>("HUD");
        hud.UpdateScore(this.score);
        hud.ShowMessage("Get Ready!");

        // Set player position and start player logic
        var player = GetNode<Player>("Player");
        var startPosition = GetNode<Position2D>("StartPosition");
        player.Start(startPosition.Position);

        // Start the game
        GetNode<Timer>("StartTimer").Start();
    }

    public void GameOver()
    {
        // Play death sound effect
        GetNode<AudioStreamPlayer>("Death").Play();

        // Stop the timers
        GetNode<Timer>("MobTimer").Stop();
        GetNode<Timer>("ScoreTimer").Stop();

         // Show gameover screen
        GetNode<HUD>("HUD").ShowGameOver();
    }

    public void OnStartTimerTimeout()
    {
        GetNode<Timer>("MobTimer").Start();
        GetNode<Timer>("ScoreTimer").Start();
    }

    public void OnScoreTimerTimeout()
    {
        this.score++;
        GetNode<HUD>("HUD").UpdateScore(this.score);
    }

    public void OnMobTimerTimeout()
    {
        // Choose random location on Path2D
        var mobSpawnLocation = GetNode<PathFollow2D>("MobPath/MobSpawnLocation");
        mobSpawnLocation.Offset = this.random.Next();

        // Create Mob instance and add it to the scene
        var mobInstance = (RigidBody2D)mob.Instance();
        this.AddChild(mobInstance);

        // Set the mob's direction perpendicular to the path direction
        float direction = mobSpawnLocation.Rotation + Mathf.Pi / 2;

        // Set the mob's positin to a random location
        mobInstance.Position = mobSpawnLocation.Position;

        // Add some randomnexx to the direction
        direction += this.RandomRange(-Mathf.Pi / 4, Mathf.Pi / 4);
        mobInstance.Rotation = direction;

        // Set the velocity
        mobInstance.LinearVelocity = new Vector2(this.RandomRange(150f, 250f), 0).Rotated(direction);
    }

    private float RandomRange(float min, float max)
    {
        return (float)this.random.NextDouble() * (max - min) + min;
    }
}
