namespace GameDemo;

using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

[Meta, StateDiagram]
public abstract partial record GameLogicState : LogicBlockState;
