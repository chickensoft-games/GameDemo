namespace GameDemo;

partial class AppLogic {
  partial record State {
    public record LeavingMenu : State, IGet<Input.FadeOutFinished> {
      public LeavingMenu() {
        OnEnter<LeavingMenu>(
          previous => Context.Output(new Output.FadeToBlack())
        );
      }

      public IState On(Input.FadeOutFinished input) =>
        new InGame();
    }
  }
}
