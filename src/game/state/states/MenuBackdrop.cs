namespace GameDemo;

public partial class GameLogic {
  public partial record State {
    public record MenuBackdrop : State, IGet<Input.StartGame>, IGet<Input.Initialize> {
      public MenuBackdrop() {
        OnEnter<MenuBackdrop>(_ => {
          Get<IGameRepo>().SetIsMouseCaptured(false);
        });

        OnAttach(() => Get<IAppRepo>().GameStarting += OnGameStarting);
        OnDetach(() => Get<IAppRepo>().GameStarting -= OnGameStarting);
      }

      private void OnGameStarting() => Context.Input(new Input.StartGame());

      public IState On(Input.StartGame input) => new Playing();

      public IState On(Input.Initialize input) {
        Get<IGameRepo>().OnNumCoinsAtStart(input.NumCoinsInWorld);
        return this;
      }
    }
  }
}