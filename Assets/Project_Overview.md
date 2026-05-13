# Project Overview: CoinTossGame

## 1. Project Description
**CoinTossGame** is a physics-based battle game where the player launches coins into a "vessel" (container) to deal damage to enemies. The game combines rhythmic physical interaction (mouse wheel scrolling) with RPG elements like wave progression and power-ups.
- **Core Experience**: Precision-based coin launching using physics, managing turn-based combat phases, and scaling difficulty through waves.
- **Target Audience**: Casual players enjoying physical interaction and roguelike-lite progression.

## 2. Gameplay Flow / User Loop
1.  **Wave Start**: A new enemy is spawned with scaled HP.
2.  **Player Phase (Launching)**: The player has a limited time (`GameConstants.TURN_TIME_LIMIT`) to launch as many coins as possible into the vessel using the mouse wheel.
3.  **Judging Phase**: A brief pause (`GameConstants.JUDGE_DURATION`) to let the physics settle.
4.  **Damage Phase**: Damage is calculated based on the count of coins inside the vessel and applied to the enemy.
5.  **Enemy Phase**: If the enemy survives, it deals fixed damage to the player.
6.  **PowerUp Phase**: Triggered after defeating an enemy; the player chooses an upgrade.
7.  **GameOver**: Triggered if the player's HP reaches zero.

## 3. Architecture
The project follows a centralized state-driven architecture centered around a `GameManager` and an event-driven communication pattern.

### Core Architecture
- `GameManager`: The central brain of the game loop. It manages transitions between phases using a Coroutine-based loop and communicates with other systems.
- `GameStateMachine`: A state machine implementation that governs the valid transitions between phases (e.g., `Launching` -> `Judging`).
- `GameEventBus`: A static event hub using C# `Action` delegates. Most systems (UI, Managers, VFX) decouple from the core logic by subscribing to these events.

### Patterns
- **State Pattern**: Used in `GameStateMachine` to control the game flow logic.
- **Event Bus Pattern**: Used in `GameEventBus` to handle decoupling between logic (e.g., `EnemyManager`) and presentation (e.g., `BattleUI`).
- **Manager Pattern**: Systems like `PlayerManager` and `EnemyManager` encapsulate specific domain data and logic.

`Location: Assets/Scripts/Core`

## 4. Game Systems & Domain Concepts

### Combat System
The combat is a cycle of launching physical objects into a target area to convert them into damage.
- `EnemyManager`: Manages enemy HP scaling per wave and damage reception.
- `PlayerManager`: Manages player health and game-over conditions.
- `ScoreManager`: Converts the count of coins in the vessel into an attack score/damage value.

`Location: Assets/Scripts/Systems`

### Physics Interaction System
- `CoinLauncher`: Detects mouse wheel velocity. If the velocity exceeds `GameConstants.WHEEL_VELOCITY_THRESHOLD`, it instantiates coin prefabs with proportional impulse.
- `Vessel`: Uses a `Trigger` and `HashSet<Collider>` to track coins currently inside the container. It applies linear/angular velocity dampening to coins that enter to keep them stable.
- `KillZone`: Cleans up coins that fall off the stage or miss the vessel.

`Location: Assets/Scripts/Gameplay`

### Progression System
- `PowerUpUI`: Displays choices for the player after a wave.
- `GameConstants`: A static data holder for balancing parameters (HP, damage, thresholds).

## 5. Scene Overview
The project is built around a single primary scene:
- `BattleScene`: Contains the physical stage, the vessel, the coin launcher, and the UI canvas. This is the entry point and the main gameplay arena.

`Location: Assets/Scenes`

## 6. UI System
The project uses **UGUI** with **TextMesh Pro** for text rendering.
- `BattleUI`: Displays HP bars (text-based), the turn timer, and current phase status. It subscribes to `GameEventBus` to update values automatically.
- `PowerUpUI`: Manages the selection interface after a wave victory.
- `ScoreUI`: Specifically handles the display of the current coin count/score during the judging phase.

`Location: Assets/Scripts/UI`

## 7. Asset & Data Model
- **Prefabs**: `Coin` prefabs are instantiated by the launcher. They require a `Rigidbody` and a `CoinLifespan` component.
- **Physics Materials**: `BouncyPhysicsMaterial` and others are used to tune the "feel" of coin collisions.
- **Constants**: `GameConstants.cs` acts as the primary source of truth for game balancing.
- **Universal Render Pipeline (URP)**: The project uses URP with specific settings for Mobile and PC, utilizing `VolumeProfiles` for post-processing effects like Bloom or Camera Shake.

`Location: Assets/Scripts/Data, Assets/Settings`

## 8. Notes, Caveats & Gotchas
- **Physics Instability**: Coins rely on `CollisionDetectionMode.Continuous` inside the vessel to prevent tunneling through the mesh at high velocities.
- **Input System**: Uses the **New Input System**; the `CoinLauncher` specifically reads from `Mouse.current.scroll`.
- **Coin Management**: The `Vessel` handles coin "ownership". If a coin exits the trigger, its lifespan timer (destruction) is resumed.
- **Memory/Performance**: Old coins are destroyed via `KillZone` or `CoinLifespan` to prevent excessive physics objects from impacting performance.
- **Scale Influence**: Non-uniform scaling on the `Vessel` or its parent can cause physics artifacts; currently, coins are managed via physical forces rather than parenting to avoid these issues.