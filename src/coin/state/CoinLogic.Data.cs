namespace GameDemo;

using Chickensoft.Introspection;
using Chickensoft.Serialization;

public partial class CoinLogic {
  [Meta, Id("coin_logic_data")]
  public partial class Data {
    /// <summary>Id of the entity that is collecting us, if any.</summary>
    [Save("target")]
    public string? Target { get; set; }

    [Save("elapsed_time")]
    public double ElapsedTime { get; set; }
  }
}
