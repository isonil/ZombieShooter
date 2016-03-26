using SFML.Window;

namespace Game
{

public class InputState
{
  public Vector2f MousePosition { get; set; }
  public Vector2f MousePositionFromCenter { get; set; }
  public bool[] IsKeyPressed { get; private set; }
  public bool IsLMBPressed { get; set; }

  public InputState()
  {
    IsKeyPressed = new bool[(int)Keyboard.Key.KeyCount];
  }
}

}