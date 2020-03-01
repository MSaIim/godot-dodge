using Godot;
using System;

public class HUD : CanvasLayer
{
    [Signal] public delegate void StartGame();

    private bool isStarted = false;

    public override void _Ready()
    {
        GetNode<Label>("ScoreLabel").Hide();
    }

    public override void _PhysicsProcess(float delta)
    {
        if (!isStarted && Input.IsActionPressed("ui_accept"))
        {
            isStarted = true;
            GetNode<Label>("StartLabel").Hide();
            GetNode<Label>("ScoreLabel").Show();
            EmitSignal("StartGame");
        }
    }

    public void ShowMessage(string text)
    {
        var messageLabel = GetNode<Label>("MessageLabel");
        messageLabel.Text = text;
        messageLabel.Show();

        GetNode<Timer>("MessageTimer").Start();
    }

    async public void ShowGameOver()
    {
        ShowMessage("Game Over");

        var messageTimer = GetNode<Timer>("MessageTimer");
        await ToSignal(messageTimer, "timeout");

        var messageLabel = GetNode<Label>("MessageLabel");
        messageLabel.Text = "Dodge";
        messageLabel.Show();

        isStarted = false;
        GetNode<Label>("StartLabel").Show();
    }

    public void UpdateScore(int score)
    {
        GetNode<Label>("ScoreLabel").Text = score.ToString();
    }

    public void OnMessageTimerTimeout()
    {
        GetNode<Label>("MessageLabel").Hide();
    }
}
