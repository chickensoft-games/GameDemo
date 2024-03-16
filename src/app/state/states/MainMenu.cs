namespace GameDemo;

using Chickensoft.LogicBlocks;

public partial class AppLogic {
  public partial record State {
    public record MainMenu : State, IGet<Input.StartGame> {
      public MainMenu() {
        this.OnEnter(
          () => {
            Output(new Output.LoadGame());
            Get<IAppRepo>().OnMainMenuEntered();
            Output(new Output.ShowMainMenu());
          }
        );
      }

      public IState On(in Input.StartGame input) => new LeavingMenu();
    }
  }
}
