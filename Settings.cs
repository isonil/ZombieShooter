using SFML.Graphics;
using SFML.Window;

namespace Game
{

public class Settings
{
	public Vector2u WindowSize { get; private set; }
    public bool Fullscreen { get; private set; }

    public Settings(string path)
    {
        Load(path);
    }

    public void Load(string path)
    {
        // loading config not implemented

		var desktopMode = VideoMode.DesktopMode;
		WindowSize = new Vector2u(desktopMode.Width, desktopMode.Height);
        Fullscreen = true;
    }
}

}