namespace GameDemo;

using System.Collections.Generic;
using Chickensoft.Introspection;

public partial class MapLogic
{
  [Meta, Id("map_logic_data")]
  public partial record Data
  {
    // Node names of coins actively being collected.
    public List<string> CoinsBeingCollected { get; set; } = [];
    // Node names of coins that were collected. We save the list of collected
    // coins since the map deserializes with coins pre-populated. Then, all we
    // have to do when loading the map is remove the coins that were collected
    // :)
    public HashSet<string> CollectedCoinIds { get; set; } = [];
  }
}
