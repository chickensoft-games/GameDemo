namespace GameDemo;

using Chickensoft.LogicBlocks;

public partial class CoinLogic {
  public partial record State {
    public interface ICollecting : IState {
    }

    public record Collecting : State, ICollecting, IGet<Input.PhysicsProcess> {
      public ICoinCollector Target { get; }
      private double _elapsedTime;

      public Collecting(ICoinCollector target) {
        Target = target;

        this.OnEnter(() => Get<IGameRepo>().StartCoinCollection(Get<ICoin>()));
      }

      public IState On(in Input.PhysicsProcess input) {
        var settings = Get<Settings>();
        var collectionTime = settings.CollectionTimeInSeconds;

        _elapsedTime += (float)input.Delta;

        if (_elapsedTime >= collectionTime) {
          Output(new Output.SelfDestruct());

          var coin = Get<ICoin>();
          var gameRepo = Get<IGameRepo>();

          gameRepo.OnFinishCoinCollection(coin);
        }

        var nextPosition = input.GlobalPosition.Lerp(
          Target.CenterOfMass, (float)(_elapsedTime / collectionTime)
        );

        Output(new Output.Move(nextPosition));
        return this;
      }
    }
  }
}
