using twodog.xunit;
using Xunit;

namespace App1.Tests;

[CollectionDefinition("GodotHeadless", DisableParallelization = true)]
public class GodotHeadlessCollection : ICollectionFixture<GodotHeadlessFixture>;