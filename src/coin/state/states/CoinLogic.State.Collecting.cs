namespace GameDemo;

using Chickensoft.Collections;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

public partial class CoinLogic {
  public partial record State {
    [Meta, Id("coin_logic_state_collecting")]
    public partial record Collecting : State, IGet<Input.PhysicsProcess> {
      public Collecting() {
        this.OnEnter(() => Get<IGameRepo>().StartCoinCollection(Get<ICoin>()));
      }

      public Transition On(in Input.PhysicsProcess input) {
        var settings = Get<Settings>();
        var data = Get<Data>();
        var entityTable = Get<EntityTable>();
        var collectionTime = settings.CollectionTimeInSeconds;

        data.ElapsedTime += (float)input.Delta;

        if (data.ElapsedTime >= collectionTime) {
          Output(new Output.SelfDestruct());

          var coin = Get<ICoin>();
          var gameRepo = Get<IGameRepo>();

          gameRepo.OnFinishCoinCollection(coin);
        }

        if (entityTable.Get<ICoinCollector>(data.Target) is { } target) {
          var nextPosition = input.GlobalPosition.Lerp(
            target.CenterOfMass, (float)(data.ElapsedTime / collectionTime)
          );

          Output(new Output.Move(nextPosition));
        }

        return ToSelf();
      }
    }
  }
}
