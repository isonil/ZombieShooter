using System;
using System.Diagnostics;
using SFML.Graphics;
using SFML.Window;

namespace Game
{

public class Core
{
  private enum GameState
  {
    Menu,
    Game,
    Closing
  };

  // game components
    private Settings settings;
    private GameWorld gameWorld;
    private Player player;
    private InputState inputState;
    private GameState gameState = GameState.Menu;

  // misc components
    private RenderWindow window;
    private View view;
    private Stopwatch stopwatch;
    private Font font;
    private Text GUIText;

    public void Run()
    {
        Init();

        float lastTime = 0f;
    float currentTime = 0f;

        while (window.IsOpen())
        {
            currentTime = stopwatch.ElapsedMilliseconds;
            float deltaTime = currentTime - lastTime;
            lastTime = currentTime;

            window.DispatchEvents();

      switch (gameState)
      {
      case GameState.Closing:
        window.Close();
        break;

      case GameState.Menu:
                // no menu implemented
                gameState = GameState.Game;
        break;

      case GameState.Game:
        player.Update(deltaTime, inputState, gameWorld);
        UpdateView();
        gameWorld.Update(deltaTime, window, player);
        gameWorld.Draw(window, player);
        DrawGUI();
        break;
      }

      if (!window.IsOpen())
        break;

            window.Display();
            window.Clear(Color.Black);
        }
    }

    private void Init()
    {
        settings = new Settings("settings.ini");
        gameWorld = new GameWorld();
        player = new Player(HardcodedCreatureTypes.Player, Player.StartPos);
        inputState = new InputState();
        gameState = GameState.Menu;

        window = new RenderWindow(
            new VideoMode(settings.WindowSize.X, settings.WindowSize.Y),
            "Game",
            settings.Fullscreen ? Styles.Fullscreen : Styles.Default
        );
        window.SetVerticalSyncEnabled(true);
        window.Closed += new EventHandler(OnClosed);
        window.KeyPressed += new EventHandler<KeyEventArgs>(OnKeyPressed);
        window.KeyReleased += new EventHandler<KeyEventArgs>(OnKeyReleased);
        window.MouseMoved += new EventHandler<MouseMoveEventArgs>(OnMouseMoved);
        window.MouseButtonPressed += new EventHandler<MouseButtonEventArgs>(OnMouseButtonPressed);
        window.MouseButtonReleased += new EventHandler<MouseButtonEventArgs>(OnMouseButtonReleased);

        view = new View(new Vector2f(0f, 0f), new Vector2f(settings.WindowSize.X, settings.WindowSize.Y));
        view.Zoom(1f / (settings.WindowSize.X / 2000f));
        stopwatch = new Stopwatch();
        stopwatch.Start();
        font = new Font("Data/Fonts/arial.ttf");
        GUIText = new Text();
    }

    private void OnClosed(object obj, EventArgs e)
    {
        gameState = GameState.Closing;
    }

    private void OnKeyPressed(object obj, KeyEventArgs e)
    {
        if (e.Code == Keyboard.Key.Escape)
      gameState = GameState.Closing;

        if (e.Code == Keyboard.Key.P)
      RestartGame();

    int keyCode = (int)e.Code;

        if (keyCode >= 0 && keyCode < (int)Keyboard.Key.KeyCount)
            inputState.IsKeyPressed[keyCode] = true;
    }

    private void OnKeyReleased(object obj, KeyEventArgs e)
  {
    int keyCode = (int)e.Code;

        if (keyCode >= 0 && keyCode < (int)Keyboard.Key.KeyCount)
            inputState.IsKeyPressed[keyCode] = false;
    }

    private void OnMouseMoved(object obj, MouseMoveEventArgs e)
    {
    inputState.MousePosition = new Vector2f(e.X, e.Y);

        inputState.MousePositionFromCenter = new Vector2f(inputState.MousePosition.X - settings.WindowSize.X / 2,
      inputState.MousePosition.Y - settings.WindowSize.Y / 2);
    }

    private void OnMouseButtonPressed(object obj, MouseButtonEventArgs e)
    {
        if (e.Button == Mouse.Button.Left)
            inputState.IsLMBPressed = true;
    }

    private void OnMouseButtonReleased(object obj, MouseButtonEventArgs e)
    {
        if (e.Button == Mouse.Button.Left)
            inputState.IsLMBPressed = false;
    }

    private void UpdateView()
    {
        var tmp = new Vector2f(player.Position.X, player.Position.Y);
        tmp.X += inputState.MousePositionFromCenter.X / settings.WindowSize.X * 100f;
        tmp.Y += inputState.MousePositionFromCenter.Y / settings.WindowSize.Y * 100f;
        view.Center = tmp;
        window.SetView(view);
    }

    private void DrawGUI()
    {
        GUIText.Font = font;
        GUIText.Position = new Vector2f(20f, 20f);
        GUIText.Scale = new Vector2f(0.5f, 0.5f);

        GUIText.DisplayedString = "Health: " + player.Health + " / " +
            HardcodedCreatureTypes.Player.MaxHealth + " Score: " +
            player.Score + (player.Dead ? " You are dead. Press P to restart." : "");

        GUIText.Color = Color.White;
        window.Draw(GUIText);
    }

    private void RestartGame()
    {
        player.Resurrect();
        gameWorld.Restart();
    }
}

}
