# 🕹 GameDemo

[![Chickensoft Badge][chickensoft-badge]][chickensoft-website] [![Discord][discord-badge]][discord] [![Read the docs][read-the-docs-badge]][docs] ![branch coverage][branch-coverage]

The Chickensoft Game Demo — a fully tested, third-person 3D game built with Godot and C#. _Now with [saving and loading][save-load], including full
game state preservation!_

> [!IMPORTANT]
> Be sure to run [`git lfs pull`][lfs] to resolve the binary files.

---

<p align="center">
<img alt="Cardboard Box with Chickensoft Logo" src="docs/preview.jpg" width="100%">
</p>

## ⏯️ Gameplay

<https://github.com/user-attachments/assets/2b9f55bc-e5c7-45b4-8e93-eb05945f1890>

> Most 3D assets are from the fine folks at [GDQuest] — go check out their stuff and support them! You'll notice that this project is loosely inspired by GDQuest's [ThirdPersonController], but completely rewritten using Chickensoft's best practice recommendations in C# (and with state machines)!
>
> Music from [FreePD] — go check them out, too!
> Other sound effects are licensed under CC0.

## 🎮 How to Play

You'll need to setup your [Godot C# development environment][setup-docs]. This should work with .NET 8 and Godot 4.3.

Use WASD to move around. Bounce with spacebar. Hold down spacebar while falling to bounce as soon as you hit the ground. Jump on the mushrooms, collect all the coins, and don't fall off the world!

## 🏆 Game Architecture

Chickensoft's packages are designed to make building games easier while following an opinionated architecture that takes a lot of work out of the decision-making. Following a highly opinionated architecture has a few advantages: mainly, the code is easier to learn, fully testable, more consistent, and other people can learn the architecture rules and get up to speed fairly quickly.

On the other hand, it does result in a little bit of boilerplate, but that's going to be true whenever you make code modular enough to be fully unit-tested. Personally, I believe the benefits outweigh the little bit of additional boilerplate, especially for studios that have more than one person contributing to the codebase.

This project is a result of two and a half years of learning, [the other dozen or so open source Chickensoft packages][chickensoft-website], and a ton of help and support I've received from the Godot community.

- ✅ Saving and loading using [Serialization], [Serialization.Godot], and [SaveFileBuilder]. You can read all about how to implement a save system like this one over at [Serialization for C# Games](https://chickensoft.games/blog/serialization-for-csharp-games).

- ✅ Testing with [GoDotTest]. GoDotTest is designed for use with CI/CD, as well as local testing and compatibility with VSCode's debug launch profiles, allowing us to easily hook into and debug tests during development.

- ✅ Node mixins using [Introspection]: we can add additional code to node scripts at build time using C# source generation, which makes up for the lack of mixin support in C#.

  Using mixins is a [data-driven technique][mixins-ecs]. By combining data-driven techniques with object-oriented programming, we can leverage the best of both worlds to make that code is exceptionally clean and easy to maintain.

- ✅ Dependency provisioning via [AutoInject]: node dependencies are resolved in a tree-based manner by looking through a node's ancestors until it finds one that provides what the dependent node is looking for.

  Supplying dependencies in a tree-based manner mirrors Godot's node-based  approach and solves the age-old problem of needing something higher-up in the scene tree before the parent node has had a chance to initialize it. Under the hood, AutoInject temporarily subscribes to the parent and calls the child back once the dependency is available.
  
  Finally, AutoInject provides mechanisms for faking dependencies easily in unit tests.

- ✅ Two-phase initialization, accomplished with AutoInject's [Enhanced Lifecycles].

  Splitting a node's initialization up into two steps make it easier to write testable node scripts. The first phase allows the node to create the values it wants to use, and the second phase allows it to consume those values for initial setup.
  
  AutoInject adds a property to each node script it's used on, `IsTesting`, that allows it to discern whether or not it's running in-game or in a test environment, skipping the first phase during testing so that mock or fake objects can be used instead.

- ✅ Faking node trees during unit tests using [GodotNodeInterfaces].
  
  GodotNodeInterfaces provides generated interfaces and adapters for every object in GodotSharp that extends `GodotObject`, allowing us to access these objects in a way that makes it easy to mock for unit tests. Note that AutoInject requires that we add GodotNodeInterfaces to our project, even if we don't actually use it (and most games won't ever need to use it — this game just happens to be fully unit tested).
  
  GodotNodeInterfaces also provides alternative methods for manipulating a node's children that accept interfaces rather than concrete node types, allowing us to use a fake scene tree during unit tests for nodes that need to be attached to the scene tree during testing.

- ✅ A consistent approach to state management using [LogicBlocks] and domain-driven design.

  For nodes that require state, we separate their state from their presentation by creating a hierarchical state machine (i.e., a logic block) for the node and consuming it through a simple binding system, ensuring our view is always synchronized with its state.
  
  Also, LogicBlocks also generates pictures of the state diagrams, so that's reason enough — scroll down to see the state diagrams in this project!

  Using LogicBlocks for all logic everywhere sometimes results in views with very simple states, but the additional effort makes it very consistent: i.e., if a node does anything more than simple visualization, you know you can find the dirty details in its logic block. Plus, separating visualization from state makes it super easy to test the logic block in isolation.

  Additionally, following domain-driven design means our logic blocks can share and use domain repository objects and react to changes in the domain model, keeping all the active logic blocks synchronized to changes occurring across the entire game, similar to an event-bus model.

- ✅ Simplified reactive utilities to power domain-driven modeling using Chickensoft's [Collections] package.

  To accomplish reactive, easy to maintain game domain models, we use some super simplified rx-style observable objects. The API is similar to an observable, but with a few tweaks for ergonomics and a simplified implementation to keep it lightweight using C#'s vanilla events.

- ✅ Consistent, automatically enforced coding style via the Chickensoft [EditorConfig]. Combined with the project settings included from [GodotGame], VSCode will automatically format your code whenever you save. And, it'll flag code-style issues, keeping everything annoyingly consistent.

I've written an entire treatise about the architecture behind this demo. You can read about it over at [Enjoyable Game Architecture][game-arch].

## 💁 Getting Help

_Found something wrong or need help?_ Please join us in the [Chickensoft Discord server][discord] to let us know!

> 💡 This game was generated from the [Chickensoft GodotGame Template][GodotGame]. The Godot Game Template README contains documentation about how how the tests, code coverage, CI/CD, app entrypoint, debug profiles, and versioning work.

---

## 📐State Diagrams

Since we're using [LogicBlocks], here's some of the more interesting state diagrams in the project, generated from the code.

### Application State

![Application State Diagram](docs/app.png)

### Game State

![Game State Diagram](docs/game.png)

### Player State

![Player State Diagram](docs/player.png)

### Coin State

![Coin State Diagram](docs/coin.png)

### Jumpshroom State

![Jumpshroom State Diagram](docs/jumpshroom.png)

---

🐣 Package generated from a 🐤 Chickensoft Template — <https://chickensoft.games>

<!-- Links -->

<!-- Header -->
[chickensoft-badge]: https://raw.githubusercontent.com/chickensoft-games/chickensoft_site/main/static/img/badges/chickensoft_badge.svg
[chickensoft-website]: https://chickensoft.games
[discord-badge]: https://raw.githubusercontent.com/chickensoft-games/chickensoft_site/main/static/img/badges/discord_badge.svg
[discord]: https://discord.gg/gSjaPgMmYW
[read-the-docs-badge]: https://raw.githubusercontent.com/chickensoft-games/chickensoft_site/main/static/img/badges/read_the_docs_badge.svg
[docs]: https://chickensoft.games/docs
[branch-coverage]: badges/branch_coverage.svg

<!-- Article -->
[GoDotTest]: https://github.com/chickensoft-games/go_dot_test
[setup-docs]: https://chickensoft.games/docs/setup
[Godotgame]: https://github.com/chickensoft-games/GodotGame
[GDQuest]: https://www.gdquest.com/
[ThirdPersonController]: https://www.gdquest.com/news/2022/12/godot-4-third-person-controller/
[FreePD]: https://freepd.com/
[AutoInject]: https://github.com/chickensoft-games/AutoInject
[Enhanced Lifecycles]: https://github.com/chickensoft-games/AutoInject?tab=readme-ov-file#-enhanced-lifecycle
[mixins-ecs]: https://en.wikipedia.org/wiki/Entity_component_system#Is_ECS_a_useful_concept?
[Introspection]: https://github.com/chickensoft-games/Introspection
[GodotNodeInterfaces]: https://github.com/chickensoft-games/GodotNodeInterfaces
[LogicBlocks]: https://github.com/chickensoft-games/LogicBlocks
[EditorConfig]: https://github.com/chickensoft-games/EditorConfig
[game-arch]: https://chickensoft.games/blog/game-architecture
[lfs]: https://git-lfs.com/
[save-load]: https://chickensoft.games/blog/serialization-for-csharp-games
[Serialization]: https://github.com/chickensoft-games/Serialization
[Serialization.Godot]: https://github.com/chickensoft-games/Serialization.Godot
[Collections]: https://github.com/chickensoft-games/Collections
[SaveFileBuilder]: https://github.com/chickensoft-games/SaveFileBuilder
