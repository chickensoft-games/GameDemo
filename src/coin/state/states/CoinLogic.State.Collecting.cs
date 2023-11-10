namespace GameDemo;

public partial class CoinLogic {
  public partial record State {
    public interface ICollecting : IState { }

    public record Collecting : State, ICollecting, IGet<Input.PhysicsProcess> {
      public ICoinCollector Target { get; }
      private double _elapsedTime;

      public Collecting(IContext context, ICoinCollector target) :
        base(context) {
        var coin = Context.Get<ICoin>();
        var appRepo = Context.Get<IAppRepo>();

        Target = target;

        OnEnter<Collecting>((previous) => appRepo.StartCoinCollection(coin));
      }

      public IState On(Input.PhysicsProcess input) {
        var settings = Context.Get<Settings>();
        var collectionTime = settings.CollectionTimeInSeconds;

        _elapsedTime += (float)input.Delta;

        if (_elapsedTime >= collectionTime) {
          Context.Output(new Output.SelfDestruct());

          var coin = Context.Get<ICoin>();
          var appRepo = Context.Get<IAppRepo>();

          appRepo.OnFinishCoinCollection(coin);
        }

        var nextPosition = input.GlobalPosition.Lerp(
          Target.CenterOfMass, (float)(_elapsedTime / collectionTime)
        );

        Context.Output(new Output.Move(nextPosition));
        return this;
      }
    }
  }
}
