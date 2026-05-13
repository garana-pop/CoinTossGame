# Project Overview: CoinTossGame

## 1. Project Description
**CoinTossGame** is a physics-based battle game where the player launches coins into a container (the Vessel) to deal damage to enemies. The game combines mechanical skill (mouse-wheel-based launching) with turn-based RPG elements. It is designed for PC, utilizing the high-precision input of a mouse wheel to control the velocity of coin throws. The core experience centers on maximizing the number of coins successfully landed in the vessel within a limited timeframe to defeat progressively stronger enemies.

## 2. Gameplay Flow / User Loop
1.  **Wave Start**: A new enemy is spawned with HP scaled based on the current wave number.
2.  **Player Phase (Launching)**: The player has 10 seconds (`TURN_TIME_LIMIT`) to launch as many coins as possible into the vessel using the mouse wheel.
3.  **Judging Phase**: A short pause (`JUDGE_DURATION`) allows the physics simulation to settle, determining how many coins remained in the vessel.
4.  **Damage Phase**: The enemy takes damage equal to the number of coins in the vessel multiplied by `SCORE_PER_COIN`.
5.  **Enemy Phase**: If the enemy survives, it deals a fixed amount of damage to the player.
6.  **Power-Up Phase**: Upon defeating an enemy, the player chooses from three randomized upgrades before moving to the next wave.
7.  **Game Over**: Occurs when the player's HP reaches zero.

## 3. Architecture
The project follows a **Decoupled Event-Driven Architecture** centered around a central `GameEventBus` and a State Machine.

### Core Management
*   `GameManager`: The central coordinator that manages the high-level game loop using a Coroutine-based sequence. It references all major systems and handles state transitions.
*   `GameStateMachine`: A pure C# class that defines the valid transitions between game states (e.g., `Launching` -> `Judging` -> `DamagePhase`).
*   `GameEventBus`: A static class acting as a global message broker. Systems subscribe to events (e.g., `OnCoinLanded`, `OnEnemyDefeated`) to react to game changes without direct coupling.

`Location: Assets/Scripts/Core`

## 4. Game Systems & Domain Concepts

### Launching System
Uses a unique mouse-wheel velocity detection mechanism.
*   `CoinLauncher`: Measures the scroll speed of the mouse wheel. If it exceeds a threshold, it instantiates a coin and applies force proportional to the scroll velocity.
*   `CameraShake`: Provides tactile feedback by shaking the camera based on the launch force.

`Location: Assets/Scripts/Gameplay`

### Vessel & Physics System
Manages the "Scoring Area" where coins must land.
*   `Vessel`: Uses a trigger volume and a `HashSet<Collider>` to track coins currently inside the container. It applies a `dampeningFactor` to incoming coins to help them settle.
*   `CoinLifespan`: Automatically destroys coins that fall outside the vessel after a set duration to maintain performance.
*   `KillZone`: Instantly destroys any objects falling out of the world bounds.

`Location: Assets/Scripts/Gameplay`

### Combat & Progression System
Handles the stats and health of entities.
*   `PlayerManager`: Tracks player health and triggers game over events.
*   `EnemyManager`: Manages enemy health, scaling Max HP per wave using `ENEMY_HP_SCALE_PER_WAVE`.
*   `ScoreManager`: Calculates the potential damage/score based on the current coin count in the vessel.

`Location: Assets/Scripts/Systems`

## 5. Scene Overview
*   **BattleScene**: The primary scene where gameplay occurs. It contains the `GameManager`, the 3D Vessel setup, the `CoinLauncher`, and the UI Canvas.
*   **0.unity (_Recovery)**: A backup/recovery scene.

The project is designed to be a single-scene experience where waves are handled by resetting internal states rather than reloading scenes.

`Location: Assets/Scenes`

## 6. UI System
The UI is built using **Unity UI (uGUI)** and follows a "Push" model via the `GameEventBus`.
*   `BattleUI`: Displays the current game phase (e.g., "PLAYER PHASE") and wave information.
*   `ScoreUI`: Updates the coin count and score in real-time by listening to `OnCoinLanded`.
*   `PowerUpUI`: Displays a selection screen for upgrades after defeating an enemy.

`Location: Assets/Scripts/UI`

## 7. Asset & Data Model
*   **Constants**: All magic numbers (Force, HP, Timers) are centralized in `GameConstants.cs` for easy balancing.
*   **Prefabs**: Coins (e.g., `Coin.prefab`, `CubeCoin.prefab`) are the primary dynamic objects. The `CoinLauncher` can be configured to shoot different prefabs.
*   **Materials**: Physics Materials (`BouncyPhysicsMaterial`, `CoinPhysicsMaterial`) are used to fine-tune the bouncing behavior of coins against the vessel.
*   **Input**: Uses the **New Input System** (`InputSystem_Actions.inputactions`).

`Location: Assets/Scripts/Data`, `Assets/Prefabs`, `Assets/Materials`

## 8. Notes, Caveats & Gotchas
*   **Physics Jitter**: The `Vessel` script sets `collisionDetectionMode` to `Continuous` when a coin enters the trigger to prevent it from clipping through the mesh at high speeds.
*   **Mouse Wheel Sensitivity**: Launch force is highly dependent on `WHEEL_VELOCITY_TO_FORCE_SCALE`. Changes here significantly impact the difficulty of landing coins.
*   **State Locking**: The `CoinLauncher` is explicitly enabled/disabled by the `GameManager` to prevent players from throwing coins during the Enemy or Power-Up phases.
*   **Cleanup**: `vessel.ResetCoins()` is called at the end of every turn/wave to clear the physical coins and prevent performance degradation.