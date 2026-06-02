namespace GameDemo;

public partial class JumpshroomLogic
{
  public record Data(float ImpulseStrength)
  {
    public IPushEnabled? Target { get; set; }
  }
}
