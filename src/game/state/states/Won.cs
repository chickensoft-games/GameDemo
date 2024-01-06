namespace GameDemo;

partial class GameLogic {
  partial record State {
    public record Won : State, IGet<Input.GoToMainMenu> {
      public Won() {
        OnEnter<Won>(
          previous => Context.Output(new Output.ShowWonScreen())
        );
      }

      public IState On(Input.GoToMainMenu input) {
        Get<IAppRepo>().OnExitGame(PostGameAction.GoToMainMenu);
        return this;
      }
    }
  }
}
