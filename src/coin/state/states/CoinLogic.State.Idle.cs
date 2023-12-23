namespace GameDemo;

public partial class CoinLogic {
  public partial record State {
    public interface IIdle : IState { }

    public record Idle : State, IIdle, IGet<Input.StartCollection> {
      public IState On(Input.StartCollection input) => new Collecting(
        input.Target
      );
    }
  }
}
