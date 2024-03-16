namespace GameDemo;

using Chickensoft.LogicBlocks;

public partial class GameLogic {
  public partial record State {
    public record MenuBackdrop : State,
    IGet<Input.Start>, IGet<Input.Initialize> {
      public MenuBackdrop() {
        this.OnEnter(() => Get<IGameRepo>().SetIsMouseCaptured(false));

        OnAttach(() => Get<IAppRepo>().GameEntered += OnGameEntered);
        OnDetach(() => Get<IAppRepo>().GameEntered -= OnGameEntered);
      }

      public void OnGameEntered() => Input(new Input.Start());

      public IState On(in Input.Start input) => new Playing();

      public IState On(in Input.Initialize input) {
        Get<IGameRepo>().SetNumCoinsAtStart(input.NumCoinsInWorld);
        return this;
      }
    }
  }
}
