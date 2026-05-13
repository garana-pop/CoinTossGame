# Coin Toss Roguelike - Technical Documentation

## 1. Project Description
This project is a 3D Roguelike deck-builder/action game where the core mechanic revolves around tossing coins into a vessel. Instead of traditional cards, players use a mouse-wheel-based "launch" mechanic to flick coins into a target container. The number and type of coins successfully landed determine the damage dealt to enemies. The game features a run-based structure with a branching map, shops, and event nodes, following the core pillars of physics-based skill and strategic progression.

## 2. Gameplay Flow / User Loop
1.  **Map Exploration**: The player starts at `MapScene`, selecting nodes (Battle, Store, Event) to progress through floors.
2.  **Battle Setup**: Upon entering a `BattleScene`, the `RunManager` persists player stats, and `EnemyManager` scales enemy HP based on the current floor.
3.  **The Battle Loop**:
    *   **Player Phase (Launching)**: A timer starts (managed by `GameManager`). The player flicks the mouse wheel to launch coins.
    *   **Judging Phase**: The system waits for physics to settle. `Vessel` tracks how many coins are inside its trigger.
    *   **Damage Phase**: Total damage is calculated (Coins x Damage-per-coin) and applied to the enemy.
    *   **Enemy Phase**: The enemy deals fixed damage to the player's HP.
    *   **Reset**: The vessel is cleared, and the loop repeats until one side is defeated.
4.  **Progression/Rewards**: Defeating enemies grants money and increases the floor counter. Players visit `StoreScene` to buy item upgrades or different coin prefabs.
5.  **Shutdown/Game Over**: If player HP reaches zero, the run resets via `RunManager` and returns to the initial map/title.

## 3. Architecture
The project follows a **Decoupled Event-Driven Architecture** centered around a central bus.

*   **Central Event Bus**: `GameEventBus` serves as the primary communication hub using static C# events. Components subscribe to events (e.g., `OnCoinLanded`, `OnEnemyDamaged`) to update UI or trigger state changes without direct coupling.
*   **State Management**: `GameStateMachine` defines the strict transition rules for the battle flow (Idle -> Launching -> Judging -> Damage -> Enemy -> etc.).
*   **Global Persistence**: `RunManager` is a `DontDestroyOnLoad` singleton that tracks "cross-scene" data: Current HP, Money, Inventory, and the currently equipped Coin Prefab.
*   **Main Entry Point**: `GameManager` (in BattleScene) coordinates the `GameLoop` coroutine, driving the `GameStateMachine` and interacting with specific scene systems.

**Location**: `Assets/Scripts/Core`

## 4. Game Systems & Domain Concepts

### Physics-Based Launch System
*   `CoinLauncher`: Converts mouse wheel velocity into physical impulse. It uses `GameConstants` to scale wheel speed to force.
*   `Vessel`: Uses a `HashSet<Collider>` to manage coins currently inside the scoring zone. It applies a `dampeningFactor` to incoming coins to increase the "catch" probability.
*   `StageConfig`: An `[ExecuteAlways]` utility that synchronizes `PhysicsMaterial` bounciness settings across the scene in real-time.
*   **Location**: `Assets/Scripts/Gameplay`

### Battle & Entity Management
*   `EnemyManager`: Calculates scaling HP using the formula: `BaseHP * (1 + Floor * ScaleFactor)`.
*   `PlayerManager`: Interface between the battle scene and the `RunManager` global HP.
*   `ScoreManager`: Listens to the `Vessel` and converts coin counts into score/damage values.
*   **Location**: `Assets/Scripts/Systems`

### Progress & Map System
*   `MapManager`: Handles the logic for floor progression and node visibility.
*   `MapNode`: Represents a clickable destination that triggers scene loads via `RunManager`.
*   **Location**: `Assets/Scripts/Systems` and `Assets/Scripts/Gameplay`

## 5. Scene Overview
*   **MapScene**: The hub where the player chooses the next encounter. Uses `MapNodeData` to define node types.
*   **BattleScene**: The core gameplay scene containing the physics setup, vessel, launcher, and `GameManager` loop.
*   **StoreScene / EventScene**: Utility scenes for progression (Implementation focused on UI-driven interactions with `RunManager`).
*   **_Recovery**: Contains auto-saved or backup scene versions (Internal use).

**Location**: `Assets/Scenes`

## 6. UI System
The project uses **UGUI** with **TextMesh Pro** for all interface elements.

*   `BattleUI`: Displays real-time battle stats (HP, Timer, Phase Name). It updates solely by listening to `GameEventBus`.
*   `GlobalHUD`: A persistent UI (often in Map or Store) showing player HP and Money from `RunManager`.
*   `ScoreUI` / `PowerUpUI`: Specialized overlays for showing damage numbers or reward selection.
*   **Binding**: Logic is decoupled; UI scripts subscribe to `GameEventBus` in `OnEnable` and unsubscribe in `OnDisable` to prevent memory leaks.

**Location**: `Assets/Scripts/UI`

## 7. Asset & Data Model
*   **ScriptableObjects**:
    *   `ItemData`: Defines inventory items and `coinPrefabOverride` for changing the coin's physical shape (e.g., `CubeBulletItem`).
    *   `MapNodeData`: Defines properties for map locations (Type, Icon, Name).
*   **Prefabs**:
    *   `Coin`: The standard physics object with a `Rigidbody` and `CoinLifespan`.
    *   `CubeCoin`: An alternative projectile shape used for different gameplay feel.
*   **Physics Materials**: `BouncyPhysicsMaterial` and `CoinPhysicsMaterial` are used to fine-tune the "feel" of the coin toss.

**Location**: `Assets/Scripts/Data` and `Assets/Settings`

## 8. Notes, Caveats & Gotchas
*   **Mouse Wheel Limitation**: The `CoinLauncher` is hardcoded to only detect **forward** scroll (positive Y delta). Backward scrolling is ignored.
*   **Continuous Collision**: `Vessel` dynamically switches coins to `CollisionDetectionMode.Continuous` when they enter the trigger to prevent high-speed coins from tunneling through the bottom.
*   **Singleton Dependency**: Many systems (UI, Managers) assume `RunManager.Instance` is present. If starting from `BattleScene` directly, `GameManager` will attempt to spawn a debug `RunManager`.
*   **Bounciness Warning**: `StageConfig` modifies the `PhysicsMaterial` asset directly in the project. Changes made during Play Mode will persist after exiting unless handled carefully.
*   **Physics Step**: Since the game relies on precise coin landing, ensure the Project Settings `Fixed Timestep` is consistent (default 0.02s) to avoid non-deterministic behavior.