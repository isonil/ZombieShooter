using SFML.Graphics;
using SFML.Window;

namespace Game
{

public class Object
{
    private ObjectType objectType;
    private Vector2f position;
    private Sprite sprite = new Sprite();

    public Object(ObjectType objectType, Vector2f position)
    {
        this.objectType = objectType;
        this.position = position;
    }

    public void Draw(RenderWindow window)
    {
        sprite.Texture = objectType.Texture;
		sprite.Origin = new Vector2f(sprite.Texture.Size.X / 2f, sprite.Texture.Size.Y / 2f);
        sprite.Position = position;
        sprite.Draw(window, RenderStates.Default);
    }
}

}
