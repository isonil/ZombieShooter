using System;
using SFML.Audio;
using SFML.Graphics;
using SFML.Window;

namespace Game
{

public class Player : Creature
{
	// resources
	private static readonly Sound StepSound = new Sound(new SoundBuffer("Data/Sounds/step.ogg"));

	// constants
	private const float StepSoundDelay = 400f;
	private const float ShotDelay = 100f;
	private const float GunOffsetX = 11f;
	private const float GunOffsetY = 42f;

	// working vars
	private float stepTimer;
	private float shotTimer;

	// properties
	public static Vector2f StartPos { get { return new Vector2f(GameWorld.MapSize / 2f, GameWorld.MapSize / 2f); } }
	public int Score { get; set; }

	static Player()
	{
		StepSound.Volume = 20f;
	}

    public Player(CreatureType creatureType, Vector2f position)
        : base(creatureType, position)
    {
    }

    public void Update(float deltaTime, InputState inputState, GameWorld gameWorld)
    {
        if (Dead)
			return;

        bool moving = false;
        float speed = CreatureType.MovementSpeed;

        var prevPosition = new Vector2f(Position.X, Position.Y);

        if (inputState.IsKeyPressed[(int)Keyboard.Key.A])
		{
			Position.X -= deltaTime * speed;
			moving = true;
		}

        if (inputState.IsKeyPressed[(int)Keyboard.Key.D])
		{
			Position.X += deltaTime * speed;
			moving = true;
		}

        if (inputState.IsKeyPressed[(int)Keyboard.Key.W])
		{
			Position.Y -= deltaTime * speed;
			moving = true;
		}

        if (inputState.IsKeyPressed[(int)Keyboard.Key.S])
		{
			Position.Y += deltaTime * speed;
			moving = true;
		}

        bool collides = false;
        const float CollisionRadiusSq = (2f * CollisionRadius) * (2f * CollisionRadius);

        foreach (var enemy in gameWorld.Enemies)
        {
            float distanceSq = (enemy.Position.X - Position.X) * (enemy.Position.X - Position.X) +
				(enemy.Position.Y - Position.Y) * (enemy.Position.Y - Position.Y);

            if (distanceSq < CollisionRadiusSq)
            {
                collides = true;
                break;
            }
        }

        if (Position.X < 0f)
			collides = true;

        if (Position.X > GameWorld.MapSize)
			collides = true;

        if (Position.Y < 0f)
			collides = true;

        if (Position.Y > GameWorld.MapSize)
			collides = true;

        if (collides)
			Position = prevPosition;

        if (moving)
        {
            stepTimer += deltaTime;

            if (stepTimer > StepSoundDelay)
            {
                stepTimer = 0f;
				StepSound.Play();
            }
        }
        else
			stepTimer = StepSoundDelay - 50f;

        float newRotationRad = (float)Math.Atan2(inputState.MousePositionFromCenter.Y, inputState.MousePositionFromCenter.X);
        float newRotation = newRotationRad * 180f / (float)Math.PI;
        Rotation = newRotation + 90f;

		shotTimer += deltaTime;
        if (inputState.IsLMBPressed && shotTimer > ShotDelay)
        {
			shotTimer = 0f;
                
            // calculating gun offset relative to the world space
            float gunOffsetX2 = (float)Math.Cos(newRotationRad + Math.PI / 2f) * GunOffsetX;
            float gunOffsetY2 = (float)Math.Sin(newRotationRad + Math.PI / 2f) * GunOffsetX;
            gunOffsetX2 += (float)Math.Cos(newRotationRad) * GunOffsetY;
            gunOffsetY2 += (float)Math.Sin(newRotationRad) * GunOffsetY;
            var bulletPos = new Vector2f(Position.X + gunOffsetX2, Position.Y + gunOffsetY2);
            gameWorld.AddShot(bulletPos, newRotation);
        }
    }

    public void MeleeAttacked()
    {
		if (!Dead )
		{
			Health -= MeleeAttackDamage;

			if (Health < 0)
				Health = 0;

			MeleeAttackSound.Play();
		}
    }

    public void Resurrect()
    {
        Score = 0;
        Health = CreatureType.MaxHealth;
		Position = StartPos;
    }
}

}
