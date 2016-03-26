using SFML.Graphics;

namespace Game
{

public class ObjectType
{
    public Texture Texture { get; private set; }

    public ObjectType(string texturePath)
    {
        Texture = new Texture(texturePath);
    }
}

public class HardcodedObjectTypes
{
    public static readonly ObjectType Tree1 = new ObjectType("Data/Objects/tree1.png");
    public static readonly ObjectType Tree2 = new ObjectType("Data/Objects/tree2.png");
    public static readonly ObjectType Tree3 = new ObjectType("Data/Objects/tree3.png");
    public static readonly ObjectType Tree4 = new ObjectType("Data/Objects/tree4.png");
}

}
