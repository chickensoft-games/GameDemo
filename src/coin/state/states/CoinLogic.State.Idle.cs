namespace GameDemo;

public partial class CoinLogic {
  public partial record State {
    public interface IIdle : IState;

    public record Idle : State, IIdle, IGet<Input.StartCollection> {
      public IState On(in Input.StartCollection input) => new Collecting(
        input.Target
      );
    }
  }
}
