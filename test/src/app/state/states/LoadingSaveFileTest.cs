namespace GameDemo.Tests;

using Chickensoft.GoDotTest;
using Chickensoft.LogicBlocks;
using Godot;
using Moq;
using Shouldly;

public class LoadingSaveFileTest : TestClass
{
  private IFakeContext _context = default!;
  private Mock<IAppRepo> _appRepo = default!;
  private AppLogic.State.LoadingSaveFile _state = default!;
  private AppLogic.Data _data = default!;

  public LoadingSaveFileTest(Node testScene) : base(testScene) { }

  [Setup]
  public void Setup()
  {
    _state = new();
    _appRepo = new();
    _data = new();
    _context = _state.CreateFakeContext();
    _context.Set(_appRepo.Object);
    _context.Set(_data);
  }

  [Test]
  public void Enters()
  {
    _state.Enter();

    _context.Outputs.ShouldBe(
      [new AppLogic.Output.StartLoadingSaveFile()]
    );
  }

  [Test]
  public void GoesToInGameOnSaveFileLoaded()
  {
    var next = _state.On(new AppLogic.Input.SaveFileLoaded());

    next.State.ShouldBeOfType<AppLogic.State.InGame>();
  }
}
