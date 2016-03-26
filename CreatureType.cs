using SFML.Graphics;

namespace Game
{

public class CreatureType
{
	public Texture Texture { get; private set; }
	public float MovementSpeed { get; private set; }
	public int MaxHealth { get; private set; }

    public CreatureType(string texturePath, float movementSpeed, int maxHealth)
    {
        Texture = new Texture(texturePath);
        MovementSpeed = movementSpeed;
        MaxHealth = maxHealth;
    }
}

public class HardcodedCreatureTypes
{
    public static readonly CreatureType Player = new CreatureType("Data/Creatures/player.png", 0.4f, 100);
    public static readonly CreatureType Zombie = new CreatureType("Data/Creatures/zombie.png", 0.35f, 75);
    public static readonly CreatureType HeavyZombie = new CreatureType("Data/Creatures/heavyZombie.png", 0.25f, 200);
    public static readonly CreatureType FastZombie = new CreatureType("Data/Creatures/fastZombie.png", 0.55f, 50);
}

}
