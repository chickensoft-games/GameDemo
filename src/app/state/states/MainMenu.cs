namespace GameDemo;

partial class AppLogic {
  partial record State {
    public record MainMenu : State, IGet<Input.StartGame> {
      public MainMenu() {
        OnEnter<MainMenu>(
          previous => {
            Context.Output(new Output.LoadGame());
            Get<IAppRepo>().OnMainMenuEntered();
            Context.Output(new Output.ShowMainMenu());
          }
        );
      }

      public IState On(Input.StartGame input) => new LeavingMenu();
    }
  }
}
