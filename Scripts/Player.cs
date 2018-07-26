using Godot;
using System;

public class Player : KinematicBody2D {
    
    // outbound signals
    [Signal] delegate void LifeUp();
    [Signal] delegate void LifeDown();
    [Signal] delegate void CoinUp();

    // configuration parameters
    const float SPEED = 750f;
    const float GRAVITY = 3600;
    const float JUMP_SPEED = -1750f;
    const float JUMP_BOOST = 2f;
    const int LEVEL_HEIGHT = 2500;  // in pixels
    const float FRICTION = 0.2f;
    Vector2 FLOOR_DIRECTION = new Vector2(0, -1);
     
    // private instance variables for state
    Vector2 motion = new Vector2();
    int coinCount = 0;
    int coinTarget = 20;
    int lives = 3;

    public override void _PhysicsProcess(float delta) {
        UpdateMotion(delta);
        bool fallOffWorld = GetPosition().y > LEVEL_HEIGHT;
        if (fallOffWorld) {
		    EndGame();
        }
    }

    private void UpdateMotion(float delta) {
        Fall(delta);  // fall to floor first, frame-rate independent
        Run();
        Jump();
        MoveAndSlide(motion, FLOOR_DIRECTION);
    }

    public void RequestDamage() {
	    motion.y = JUMP_SPEED;
        EmitSignal(nameof(LifeDown));  // :-)
        //(GetNode("Pain_sfx") as AudioStreamPlayer).Play();
	    lives -= 1;

        if (lives < 0) {
           EndGame();
        }
	}

    void Hurt()
    {
        motion.y = JUMP_SPEED;
    }

    private void EndGame() {
        GetTree().ChangeScene("res://Scenes/Levels/GameOver.tscn");
    }

    public void OnCoinPickup() {
        coinCount += 1;
        //(GetNode("Coin_sfx") as AudioStreamPlayer).Play();
        EmitSignal(nameof(CoinUp));  // note is refactorable with nameof
        if (coinCount >= coinTarget) {
            GrantExtraLife();
        }
    }

    private void GrantExtraLife() {
        lives += 1; // Why not just a signal?
        coinCount = 0;
        EmitSignal(nameof(LifeUp));
    }

    private void Fall(float delta) {
        if (IsOnFloor()) {
            motion.y = 0f;
        }
        else {
            motion.y += GRAVITY * delta;  // note accelerate down
        }
    }

    private void Jump() {
        if (IsOnFloor() && Input.IsActionPressed("ui_up")) {
            GD.Print("Jump");
            motion.y = JUMP_SPEED;
           // (GetNode("Jump_sfx") as AudioStreamPlayer).Play();
        }
    }

    private void Run() {
        if (Input.IsActionPressed("ui_right") && !Input.IsActionPressed("ui_left")) {
            motion.x = SPEED;
        }
        else if (Input.IsActionPressed("ui_left") && !Input.IsActionPressed("ui_right")) {
            motion.x = -SPEED;
        }
        else {
            motion.x = Mathf.Lerp(motion.x, 0f, FRICTION);
        }
    }
}