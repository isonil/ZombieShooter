using System;
using System.Collections.Generic;
using SFML.Audio;
using SFML.Graphics;
using SFML.Window;

namespace Game
{

public class Bullet
{
    // resources
    public static readonly Sound ShotSound = new Sound(new SoundBuffer("Data/Sounds/shot.ogg"));
    private static readonly Texture BulletTexture = new Texture("Data/Misc/bullet.png");

    // constants
    public const float RecoilAngle = 10f;
    private const float MovementSpeed = 4f;
    private const float DisappearAfter = 1500f;

    // working vars
    private Vector2f position;
    private float rotation;
    private Sprite bulletSprite;
    private float disappearTimer;

    static Bullet()
    {
        ShotSound.Volume = 20f;
    }

    public Bullet(Vector2f position, float rotation)
    {
        this.position = position;
        this.rotation = rotation;
        bulletSprite = new Sprite(BulletTexture);
    }

    public bool Update(float deltaTime, List<Enemy> enemies)
    {
        const float CollisionRadiusSq = (2f * Creature.CollisionRadius) * (2f * Creature.CollisionRadius);

        foreach (var enemy in enemies)
        {
            float distanceSq = (enemy.Position.X - position.X) * (enemy.Position.X - position.X) +
                (enemy.Position.Y - position.Y) * (enemy.Position.Y - position.Y);

            if (distanceSq < CollisionRadiusSq)
            {
                enemy.GotShot();
                return false;
            }
        }

        disappearTimer += deltaTime;

        if (disappearTimer > DisappearAfter)
            return false;

        float radians = rotation / 180f * (float)Math.PI;
        position.X += (float)Math.Cos(radians) * deltaTime * MovementSpeed;
        position.Y += (float)Math.Sin(radians) * deltaTime * MovementSpeed;

        return true;
    }

    public void Draw(RenderWindow window)
    {
        bulletSprite.Position = position;
        bulletSprite.Rotation = rotation + 90f;
        bulletSprite.Draw(window, RenderStates.Default);
    }
}

}
