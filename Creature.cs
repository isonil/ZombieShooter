using SFML.Audio;
using SFML.Graphics;
using SFML.Window;

namespace Game
{

public class Creature
{
	// resources
    protected static readonly Sound MeleeAttackSound = new Sound(new SoundBuffer("Data/Sounds/meleeAttack.ogg"));
    protected static readonly Sound BulletHitSound = new Sound(new SoundBuffer("Data/Sounds/bulletHit.ogg"));
    protected static readonly Sound DeathSound = new Sound(new SoundBuffer("Data/Sounds/death.ogg"));

	// constants
	public const float CollisionRadius = 20f;
	protected const float MeleeAttackRadius = 35f;
	protected const int MeleeAttackDamage = 5;
	protected const int BulletShotDamage = 25;

	// working vars
    private Sprite sprite = new Sprite();
    private Vector2f spriteOrigin = new Vector2f(0f, 0f);

	// properties
	public Vector2f Position; // should be a property
	public float Rotation { get; set; }
	public int Health { get; protected set; }
	public bool Dead { get { return Health <= 0; } }
	protected CreatureType CreatureType { get; private set; }

    public Creature(CreatureType creatureType, Vector2f position)
    {
        CreatureType = creatureType;
        Position = position;
        Health = creatureType.MaxHealth;
    }

    public void Draw(RenderWindow window)
    {
        sprite.Texture = CreatureType.Texture;
		sprite.Origin = new Vector2f(sprite.Texture.Size.X / 2f, sprite.Texture.Size.Y / 2f);
        sprite.Position = Position;
        sprite.Rotation = Rotation;
        sprite.Draw(window, RenderStates.Default);
    }
}

}
