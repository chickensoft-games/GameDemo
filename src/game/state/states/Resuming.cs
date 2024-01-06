namespace GameDemo;

partial class GameLogic {
  partial record State {
    public record Resuming : State, IGet<Input.PauseMenuTransitioned> {
      public Resuming() {
        OnEnter<Resuming>(previous => Get<IGameRepo>().Resume());
        OnExit<Resuming>(
          next => Context.Output(new Output.HidePauseMenu())
        );
      }

      public IState On(Input.PauseMenuTransitioned input) =>
        new Playing();
    }
  }
}
