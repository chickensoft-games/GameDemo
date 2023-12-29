namespace GameDemo;

using System;
using System.Collections.Generic;
using Chickensoft.GoDotTest;
using Godot;
using Shouldly;

public class LogicSerializationTest : TestClass {
  public LogicSerializationTest(Node testScene) : base(testScene) { }

  [Test]
  public void MapsTypesCorrectly() {
    var types = LogicSerialization.GetDerivedTypesForLogicState(typeof(PlayerLogic.State));

    types.SetEquals(new HashSet<Type>() {
      typeof(PlayerLogic.State),
      typeof(PlayerLogic.State.Disabled),
      typeof(PlayerLogic.State.Alive),
      typeof(PlayerLogic.State.Dead),
      typeof(PlayerLogic.State.Grounded),
      typeof(PlayerLogic.State.Idle),
      typeof(PlayerLogic.State.Moving),
      typeof(PlayerLogic.State.Airborne),
      typeof(PlayerLogic.State.Jumping),
      typeof(PlayerLogic.State.Liftoff),
      typeof(PlayerLogic.State.Falling)
    }).ShouldBeTrue();
  }
}
