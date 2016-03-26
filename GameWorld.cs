using System;
using System.Collections.Generic;
using SFML.Audio;
using SFML.Graphics;
using SFML.Window;

namespace Game
{

public class GameWorld
{
	// resources
	private static readonly Texture BoundaryTexture = new Texture("Data/Misc/boundary.png");
	private static readonly Texture GrassTexture = new Texture("Data/Ground/grass.png");
	private static readonly Texture RainTexture = new Texture("Data/Misc/rain.png");
	private static readonly Music Rain = new Music("Data/Sounds/rain.ogg");

	// constants
	public const float MapSize = 4000f;
	private const float NewRaindropDelay = 8f;
	private const float RaindropMovementSpeed = 1.9f;
	private const int TreeCount = 200;
	private const float BoundarySize = 20f;
	private const float BaseEnemySpawnDelay = 1000f;
	private const float EnemySpawnPositionOffset = 100f;

	// working vars
    private List<Enemy> enemies = new List<Enemy>();
    private List<Vector2f> raindrops = new List<Vector2f>();
    private List<Object> objects = new List<Object>();
    private List<Bullet> bullets = new List<Bullet>();
    private Sprite grassSprite = new Sprite();
	private Sprite raindropSprite = new Sprite();
	private Random random = new Random();
	private float elapsedMilliseconds;
	private float raindropTimer;
	private float enemySpawnTimer;

	// properties
	public IEnumerable<Enemy> Enemies { get { return enemies; } }
	private float EnemySpawnDelay
	{
		get
		{
			int elapsed5SecsIntervals = (int)(elapsedMilliseconds / 5000f);
			return BaseEnemySpawnDelay * (float)Math.Pow(0.9f, elapsed5SecsIntervals);
		}
	}

    public GameWorld()
    {
		AddInitialEnemies();

        for (int i = 0; i < TreeCount; ++i)
        {
            int rand = random.Next() % 4;
            float x = (float)random.NextDouble() * MapSize;
            float y = (float)random.NextDouble() * MapSize;
            var pos = new Vector2f(x, y);

            if (rand == 0)
				objects.Add(new Object(HardcodedObjectTypes.Tree1, pos));
            else if (rand == 1)
				objects.Add(new Object(HardcodedObjectTypes.Tree2, pos));
            else if (rand == 2)
				objects.Add(new Object(HardcodedObjectTypes.Tree3, pos));
            else if (rand == 3)
				objects.Add(new Object(HardcodedObjectTypes.Tree4, pos));
        }

		grassSprite.Texture = GrassTexture;
		raindropSprite.Texture = RainTexture;
		Rain.Loop = true;
		Rain.Play();
    }

    public void Update(float deltaTime, RenderWindow window, Player player)
    {
		elapsedMilliseconds += deltaTime;
        UpdateRaindrops(deltaTime, window);
        UpdateEnemies(deltaTime, player);
        UpdateBullets(deltaTime);
    }

    public void Draw(RenderWindow window, Player player)
    {
        DrawGrass(window, player);
        DrawBoundaries(window);
        DrawEnemies(window);
        player.Draw(window);
        DrawBullets(window);
        DrawObjects(window);
        DrawRaindrops(window);
    }

    public void Restart()
    {
        enemies.Clear();
		elapsedMilliseconds = 0f;
		enemySpawnTimer = 0f;
		AddInitialEnemies();
    }

    public void AddShot(Vector2f position, float rotation)
    {
        float newRotation = rotation + (float)random.NextDouble() * Bullet.RecoilAngle - Bullet.RecoilAngle / 2f;
        bullets.Add(new Bullet(position, newRotation));
		Bullet.ShotSound.Play();
    }
        
    private void UpdateRaindrops(float deltaTime, RenderWindow window)
    {
        for (int i = raindrops.Count - 1; i >= 0; --i)
        {
			float newX = raindrops[i].X + deltaTime * RaindropMovementSpeed;
			float newY = raindrops[i].Y + deltaTime * RaindropMovementSpeed;

            if (newX > window.Size.X || newY > window.Size.Y)
                raindrops.RemoveAt(i);
            else
                raindrops[i] = new Vector2f(newX, newY);
        }

        raindropTimer += deltaTime;
        while (raindropTimer > NewRaindropDelay)
        {
			raindropTimer -= NewRaindropDelay;
            float x = (float)random.NextDouble() * window.Size.X * 2f - window.Size.X;
            raindrops.Add(new Vector2f(x, 0f));
        }
    }

    private void UpdateEnemies(float deltaTime, Player player)
    {
        for (int i = enemies.Count - 1; i >= 0; --i)
        {
            bool isAlive = enemies[i].Update(deltaTime, player, enemies);

            if (!isAlive)
            {
                player.Score += 1;
                enemies.RemoveAt(i);
            }
        }

		enemySpawnTimer += deltaTime;
        if (enemySpawnTimer > EnemySpawnDelay)
        {
			enemySpawnTimer = 0f;

            int count = random.Next() % 3 + 1;

            for (int i = 0; i < count; ++i)
			{
				SpawnEnemy(player);
			}
        }
    }

    private void UpdateBullets(float deltaTime)
    {
        for (int i = bullets.Count - 1; i >= 0; --i)
        {
            if (!bullets[i].Update(deltaTime, enemies))
                bullets.RemoveAt(i);
        }
    }

    private void DrawGrass(RenderWindow window, Player player)
    {
        // drawing only visible grass,
		// for some reason I couldn't display a single sprite with repeated texture

        float viewCenterX = window.GetView().Center.X;
        float viewCenterY = window.GetView().Center.Y;
		int grassCountX = (int)(window.GetView().Size.X / GrassTexture.Size.X);
		int grassCountY = (int)(window.GetView().Size.Y / GrassTexture.Size.Y);
		int fromX = (int)(viewCenterX / GrassTexture.Size.X) - grassCountX / 2 - 2;
		int toX = (int)(viewCenterX / GrassTexture.Size.X) + grassCountX / 2 + 1;
		int fromY = (int)(viewCenterY / GrassTexture.Size.Y) - grassCountY / 2 - 2;
		int toY = (int)(viewCenterY / GrassTexture.Size.Y) + grassCountY / 2 + 1;

        var position = new Vector2f(0f, 0f);

        for (int i = fromX; i <= toX; ++i)
        {
            for (int j = fromY; j <= toY; ++j)
            {
				position.X = i * GrassTexture.Size.X;
				position.Y = j * GrassTexture.Size.Y;
                grassSprite.Position = position;
                grassSprite.Draw(window, RenderStates.Default);
            }
        }
    }

    private void DrawBoundaries(RenderWindow window)
    {
        var shape = new RectangleShape();
		shape.Texture = BoundaryTexture;

        shape.Position = new Vector2f(0f, 0f);
		shape.Size = new Vector2f(MapSize, BoundarySize);
        window.Draw(shape);

        shape.Position = new Vector2f(0f, MapSize);
		shape.Size = new Vector2f(MapSize + BoundarySize, BoundarySize);
        window.Draw(shape);

        shape.Position = new Vector2f(0f, 0f);
		shape.Size = new Vector2f(BoundarySize, MapSize);
        window.Draw(shape);

        shape.Position = new Vector2f(MapSize, 0f);
		shape.Size = new Vector2f(BoundarySize, MapSize);
        window.Draw(shape);
    }

	private void AddInitialEnemies()
	{
		enemies.Add(new Enemy(HardcodedCreatureTypes.Zombie, new Vector2f(100f, 100f)));
		enemies.Add(new Enemy(HardcodedCreatureTypes.Zombie, new Vector2f(200f, 200f)));
		enemies.Add(new Enemy(HardcodedCreatureTypes.HeavyZombie, new Vector2f(300f, 300f)));
		enemies.Add(new Enemy(HardcodedCreatureTypes.FastZombie, new Vector2f(400f, 400f)));
	}

    private void SpawnEnemy(Player player)
    {
        float x = 0f, y = 0f;

        int spawnSide = random.Next() % 4;

        if (spawnSide == 0)
        {
            x = (float)random.NextDouble() * MapSize;
			y = -EnemySpawnPositionOffset;
        }
        else if (spawnSide == 1)
        {
            x = (float)random.NextDouble() * MapSize;
			y = MapSize + EnemySpawnPositionOffset;
        }
        else if (spawnSide == 2)
        {
			x = -EnemySpawnPositionOffset;
            y = (float)random.NextDouble() * MapSize;
        }
        else if (spawnSide == 3)
        {
			x = MapSize + EnemySpawnPositionOffset;
            y = (float)random.NextDouble() * MapSize;
        }

        var pos = new Vector2f(x, y);
        int randType = random.Next() % 100;
        Enemy newEnemy = null;

        if (randType < 70)
			newEnemy = new Enemy(HardcodedCreatureTypes.Zombie, pos);
        else if (randType < 85)
			newEnemy = new Enemy(HardcodedCreatureTypes.HeavyZombie, pos);
        else
			newEnemy = new Enemy(HardcodedCreatureTypes.FastZombie, pos);

        while (newEnemy.Collides(enemies, player))
		{
			newEnemy.Position.X += Creature.CollisionRadius;
		}

        enemies.Add(newEnemy);
    }

    private void DrawEnemies(RenderWindow window)
    {
        foreach (var enemy in enemies)
        {
            enemy.Draw(window);
        }
    }

    private void DrawBullets(RenderWindow window)
    {
        foreach (var bullet in bullets)
        {
            bullet.Draw(window);
        }
    }

    private void DrawObjects(RenderWindow window)
    {
        foreach (var obj in objects)
        {
            obj.Draw(window);
        }
    }

    private void DrawRaindrops(RenderWindow window)
    {
        var newView = window.GetView();
        newView.Center = new Vector2f(window.Size.X / 2f, window.Size.Y / 2f);
        newView.Size = new Vector2f(window.Size.X, window.Size.Y);
        window.SetView(newView);

        foreach (var raindrop in raindrops)
        {
            raindropSprite.Position = raindrop;
            raindropSprite.Draw(window, RenderStates.Default);
        }
    }
}

}
