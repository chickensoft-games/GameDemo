namespace GameDemo;

public partial class GameLogic {
  public partial record State {
    public record MenuBackdrop : State, IGet<Input.StartGame> {
      public MenuBackdrop() {
        OnEnter<MenuBackdrop>(_ => {
          Context.Output(new Output.CaptureMouse(false));
          Get<IAppRepo>().GameStarting += OnGameStarting;
        });
      }

      private void OnGameStarting() => Context.Input(new Input.StartGame());

      public IState On(Input.StartGame input) => new PlayingGame();
    }
  }
}
