namespace GameDemo;

using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

public partial class GameLogic {
  public partial record State {
    [Meta]
    public partial record MenuBackdrop : State,
    IGet<Input.Start>, IGet<Input.Initialize> {
      public MenuBackdrop() {
        this.OnEnter(() => Get<IGameRepo>().SetIsMouseCaptured(false));

        OnAttach(() => Get<IAppRepo>().GameEntered += OnGameEntered);
        OnDetach(() => Get<IAppRepo>().GameEntered -= OnGameEntered);
      }

      public void OnGameEntered() => Input(new Input.Start());

      public Transition On(in Input.Start input) => To<Playing>();

      public Transition On(in Input.Initialize input) {
        Get<IGameRepo>().SetNumCoinsAtStart(input.NumCoinsInWorld);
        return ToSelf();
      }
    }
  }
}
