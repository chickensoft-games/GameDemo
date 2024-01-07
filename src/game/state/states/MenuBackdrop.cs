namespace GameDemo;

partial class GameLogic {
  partial record State {
    public record MenuBackdrop : State, IGet<Input.Start>, IGet<Input.Initialize> {
      public MenuBackdrop() {
        OnEnter<MenuBackdrop>(_ => {
          Get<IGameRepo>().SetIsMouseCaptured(false);
        });

        OnAttach(() => Get<IAppRepo>().GameEntered += OnGameEntered);
        OnDetach(() => Get<IAppRepo>().GameEntered -= OnGameEntered);
      }

      public void OnGameEntered() => Context.Input(new Input.Start());

      public IState On(Input.Start input) => new Playing();

      public IState On(Input.Initialize input) {
        Get<IGameRepo>().SetNumCoinsAtStart(input.NumCoinsInWorld);
        return this;
      }
    }
  }
}
