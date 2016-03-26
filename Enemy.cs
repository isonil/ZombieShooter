using System;
using System.Collections.Generic;
using SFML.Window;

namespace Game
{

public class Enemy : Creature
{
    // constants
    private const float AttackDelay = 550f;

    // working vars
    private float attackTimer;

    public Enemy(CreatureType creatureType, Vector2f position)
        : base(creatureType, position)
    {
    }

    public bool Update(float deltaTime, Player player, List<Enemy> enemies)
    {
        if (Dead)
        {
            DeathSound.Play();
            return false;
        }

        float newRotationRad = (float)Math.Atan2(player.Position.Y - Position.Y, player.Position.X - Position.X);
        Rotation = newRotationRad * 180f / (float)Math.PI + 90f;

        var prevPosition = new Vector2f(Position.X, Position.Y);

        Position.X += (float)Math.Cos(newRotationRad) * deltaTime * CreatureType.MovementSpeed;
        Position.Y += (float)Math.Sin(newRotationRad) * deltaTime * CreatureType.MovementSpeed;

        if (Collides(enemies, player))
            Position = prevPosition;

        attackTimer += deltaTime;

        float playerDistanceSq = (player.Position.X - Position.X) * (player.Position.X - Position.X) +
            (player.Position.Y - Position.Y) * (player.Position.Y - Position.Y);

        const float MeleeAttackRadiusSq = (2f * MeleeAttackRadius) * (2f * MeleeAttackRadius);

        if (attackTimer > AttackDelay && playerDistanceSq < MeleeAttackRadiusSq)
        {
            attackTimer = 0f;
            player.MeleeAttacked();
        }

        return true;
    }

    public void GotShot()
    {
        Health -= BulletShotDamage;
        BulletHitSound.Play();
    }

    public bool Collides(List<Enemy> enemies, Player player)
    {
        const float CollisionRadiusSq = (2f * CollisionRadius) * (2f * CollisionRadius);

        foreach (var enemy in enemies)
        {
            if (enemy == this)
                continue;

            float distanceSq = (enemy.Position.X - Position.X) * (enemy.Position.X - Position.X) +
                (enemy.Position.Y - Position.Y) * (enemy.Position.Y - Position.Y);

            if (distanceSq < CollisionRadiusSq)
                return true;
        }

        float playerDistanceSq = (player.Position.X - Position.X) * (player.Position.X - Position.X) +
                (player.Position.Y - Position.Y) * (player.Position.Y - Position.Y);

        if (playerDistanceSq < CollisionRadiusSq)
            return true;

        return false;
    }
}

}
