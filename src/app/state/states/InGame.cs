namespace GameDemo;

public partial class AppLogic {
  public partial record State {
    public record InGame : State;
  }
}
