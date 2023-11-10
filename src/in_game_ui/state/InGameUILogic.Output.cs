namespace GameDemo;

public partial class InGameUILogic {
  public static class Output {
    public readonly record struct NumCoinsCollectedChanged(
      int NumCoinsCollected
    );

    public readonly record struct NumCoinsAtStartChanged(
      int NumCoinsAtStart
    );
  }
}
