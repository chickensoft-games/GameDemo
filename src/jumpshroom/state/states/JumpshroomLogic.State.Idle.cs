namespace GameDemo;

using Chickensoft.Introspection;

public partial class JumpshroomLogic
{
  public partial record State
  {
    [Meta]
    public partial record Idle : State, IGet<Input.Hit>
    {
      public Transition On(in Input.Hit input)
      {
        var target = input.Target;
        return To<Loading>()
        .With(state => ((Loading)state).Target = target);
      }
    }
  }
}
