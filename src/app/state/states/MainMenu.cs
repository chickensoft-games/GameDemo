namespace GameDemo;

public partial class AppLogic {
  public partial record State {
    public record MainMenu : State, IGet<Input.StartGame> {
      public MainMenu(IContext context) : base(context) {
        var appRepo = Context.Get<IAppRepo>();
        OnEnter<MainMenu>(
          (previous) => {
            appRepo.OnMainMenuEntered();
            Context.Output(new Output.LoadGame());
            Context.Output(new Output.ShowMainMenu());
          }
        );
      }

      public IState On(Input.StartGame input) => new LeavingMenu(Context);
    }
  }
}
