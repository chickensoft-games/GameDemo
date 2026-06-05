namespace GameDemo;

using System;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

public partial record GameLogicState
{
  [Meta]
  public partial record Saving : Paused, IGet<Input.SaveCompleted>
  {
    public Saving()
    {
      this.OnEnter(
        () =>
        {
          Output(new Output.ShowPauseSaveOverlay());
          Output(new Output.StartSaving());
        }
      );

      this.OnExit(() => Output(new Output.HidePauseSaveOverlay()));
    }

    public Type On(in Input.SaveCompleted input) => To<Paused>();

    // Make it impossible to leave the pause menu while saving
    public override Type On(in Input.PauseButtonPressed input) =>
      ToSelf();
  }
}
