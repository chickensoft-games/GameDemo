namespace GameDemo;

public partial class AppLogic {
  public partial record State {
    public record MainMenu : State, IGet<Input.StartGame> {
      public MainMenu() {
        OnEnter<MainMenu>(
          (previous) => {
            Get<IAppRepo>().OnMainMenuEntered();
            Context.Output(new Output.LoadGame());
            Context.Output(new Output.ShowMainMenu());
          }
        );
      }

      public IState On(Input.StartGame input) => new LeavingMenu();
    }
  }
}
