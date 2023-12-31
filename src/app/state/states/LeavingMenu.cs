namespace GameDemo;

public partial class AppLogic {
  public partial record State {
    public record LeavingMenu : State, IGet<Input.FadeOutFinished> {
      public LeavingMenu() {
        OnEnter<LeavingMenu>(
          previous => Context.Output(new Output.FadeOut())
        );
      }

      public IState On(Input.FadeOutFinished input) =>
        new InGame();
    }
  }
}
