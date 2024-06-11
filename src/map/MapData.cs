namespace GameDemo;

using System.Collections.Generic;
using Chickensoft.Introspection;
using Chickensoft.Serialization;

[Meta, Id("map_data")]
public partial record MapData {
  [Save("coins_being_collected")]
  public required Dictionary<string, CoinData> CoinsBeingCollected {
    get; init;
  }
  [Save("collected_coin_ids")]
  public required HashSet<string> CollectedCoinIds { get; init; }
}
