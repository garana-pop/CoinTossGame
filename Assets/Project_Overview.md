# Project Overview: Coin Toss Battle

## 1. Project Description
This project is a physics-based "Coin Toss" battle game where players launch coins into a vessel using mouse wheel gestures. The core experience revolves around the mechanical satisfaction of high-velocity coin launching combined with strategic power-up choices. The game is designed for PC, utilizing the New Input System to map mouse wheel velocity to physical force.

## 2. Gameplay Flow / User Loop
1.  **Wave Start**: The system initializes wave-specific enemy stats and UI.
2.  **Player Phase (Launching)**: The player has a limited time (`GameConstants.TURN_TIME_LIMIT`) to launch as many coins as possible into the vessel using the mouse wheel.
3.  **Judging Phase**: The system waits for physical simulations to settle and counts how many coins remain inside the `Vessel`.
4.  **Damage Phase**: Total damage is calculated based on coin count and dealt to the enemy.
5.  **Enemy Phase**: If the enemy survives, it deals a fixed amount of damage to the player.
6.  **PowerUp Phase**: Upon defeating an enemy, the player chooses from three upgrades (Efficiency, Shape Change, or Friction).
7.  **Wave Transit**: The game resets the vessel and moves to the next wave, or ends if the player's HP reaches zero.

## 3. Architecture
The project follows a **Centralized Manager with Event-Driven Communication** pattern.
*   `GameManager`: Acts as the orchestrator using a Coroutine-based state machine to handle the turn-based loop.
*   `GameEventBus`: A static messaging hub that decouples systems (e.g., `Vessel` doesn't need to know about `EnemyManager`).
*   `GameStateMachine`: A simple state container used by the manager to track current phases.

Location: `Assets/Scripts/Core`

## 4. Game Systems & Domain Concepts

### Combat & Turn System
*   `GameManager`: Manages the `IEnumerator GameLoop` which transitions between phases like `Launching`, `Judging`, and `DamagePhase`.
*   `GameConstants`: Centralized data file containing turn limits, damage values, and physics thresholds.
*   **Extension**: New phases (e.g., "Critical Hit Phase") can be added by updating the `GameStateMachine` enum and adding a new state block in `GameManager.GameLoop`.
Location: `Assets/Scripts/Core`

### Physics Interaction System
*   `CoinLauncher`: Converts Mouse Wheel velocity into `Rigidbody.AddForce`. It uses a peak-velocity detection algorithm.
*   `Vessel`: Uses a `HashSet<Collider>` and `OnTriggerEnter/Exit` to accurately track coins. It applies a `dampeningFactor` to incoming coins to assist with "landing."
*   `Coin`: Prefabs with `Rigidbody` and physical materials that determine bounciness and friction.
*   **Extension**: To add new projectile types, create a prefab with the "Coin" tag and update `CoinLauncher` via the `PowerUpUI`.
Location: `Assets/Scripts/Gameplay`

### Entity Management
*   `PlayerManager`: Tracks player health and handles damage via `GameEventBus`.
*   `EnemyManager`: Tracks enemy health and triggers the `OnEnemyDefeated` event.
Location: `Assets/Scripts/Systems`

## 5. Scene Overview
*   **SampleScene**: The primary gameplay scene. It contains the 3D physics environment (Launcher, Vessel, Enemy) and the UI Canvas.
*   **Scene Flow**: Currently, the game operates in a single-scene loop where waves are reset programmatically rather than reloading the scene.

Location: `Assets/Scenes`

## 6. UI System
The project uses **UGUI** with **TextMesh Pro**.
*   `BattleUI`: Displays real-time stats like Player/Enemy HP, the turn timer, and the current phase name.
*   `PowerUpUI`: A modal overlay that appears between waves, allowing players to select upgrades.
*   `ScoreUI`: Handles the display of the current coin count and cumulative score.
*   **Binding**: UI components subscribe to `GameEventBus` in `OnEnable` to react to state changes without direct references to gameplay logic.

Location: `Assets/Scripts/UI`

## 7. Asset & Data Model
*   **Prefabs**: `Coin.prefab` and `CubeCoin.prefab` define the physical behavior of projectiles.
*   **Physics Materials**: `CoinPhysicsMaterial` and `VesselPhysicsMaterial` are tuned to prevent excessive sliding.
*   **Data Classes**: `GameConstants.cs` serves as the "Source of Truth" for all balance parameters (HP, damage, time).
*   **Models**: Custom meshes like `ConcaveContainer.asset` are used for the `Vessel` to provide accurate collision for the coin-stacking mechanic.

Location: `Assets/Prefabs`, `Assets/Materials`, `Assets/Scripts/Data`

## 8. Notes, Caveats & Gotchas
*   **Wheel Velocity Threshold**: `CoinLauncher` will not fire unless the wheel velocity exceeds `GameConstants.WHEEL_VELOCITY_THRESHOLD`. This prevents accidental firing during slow scrolling.
*   **Coin Tagging**: The `Vessel` strictly checks for the "Coin" tag. Any new projectile prefabs must be tagged correctly to be counted.
*   **Physics Stability**: High-speed coins might tunnel through the vessel. The `Vessel` script attempts to mitigate this by setting `CollisionDetectionMode.Continuous` on entry.
*   **Input**: Uses the New Input System. Ensure `InputSystem_Actions` is correctly linked in the project settings.