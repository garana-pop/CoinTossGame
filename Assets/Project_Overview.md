# Project Overview: CoinTossGame

## 1. Project Description
**CoinTossGame** is a physics-based battle game where the player launches coins into a vessel using mouse wheel gestures. The core experience revolves around the "coin toss" mechanic, where the number of coins successfully landed in a vessel determines the damage dealt to an enemy. It is a roguelike-lite progression game where players survive waves of enemies, improving their coin-throwing capabilities through power-ups between rounds.

## 2. Gameplay Flow / User Loop
1.  **Wave Start**: A new enemy spawns with scaled HP based on the current wave number.
2.  **Player Phase (Launching)**: The player has a limited time (governed by `GameConstants.TURN_TIME_LIMIT`) to launch as many coins as possible into the vessel.
3.  **Judging Phase**: A short duration where physics settles, and the system counts how many coins are currently inside the vessel's trigger zone.
4.  **Damage Phase**: Total damage is calculated (Coins × Multiplier) and applied to the enemy.
5.  **Enemy Phase**: If the enemy survives, it deals a fixed amount of damage back to the player.
6.  **Power-Up Phase**: Upon defeating an enemy, the player chooses a power-up (e.g., more coins per throw, reduced bounciness).
7.  **Loop**: The cycle repeats with increasing difficulty until the player's HP reaches zero.

## 3. Architecture
The project follows a **Centralized State Machine** pattern coupled with an **Event-Driven Architecture** to decouple gameplay systems from UI and logic.

*   **GameManager**: The central orchestrator that runs the main `IEnumerator GameLoop`. It handles phase transitions and coordinates between different managers.
*   **GameStateMachine**: A standalone logic class that defines valid state transitions (e.g., `Launching` -> `Judging` -> `DamagePhase`).
*   **GameEventBus**: A static event hub using C# Actions (`Action`, `Action<int>`, etc.). This allows components like `ScoreUI` or `EnemyManager` to respond to game events without direct references to the source of the action.
*   **Managers**: Specific domain logic is encapsulated in managers (`EnemyManager`, `PlayerManager`, `ScoreManager`) that subscribe to the `GameEventBus`.

`Location: Assets/Scripts/Core`

## 4. Game Systems & Domain Concepts

### Coin Physics & Launching
The launching system translates physical user input (mouse wheel velocity) into coin trajectory.
*   `CoinLauncher`: Detects mouse wheel scroll speed, calculates launch force, and instantiates coin prefabs.
*   `CoinLifespan`: Automatically destroys coins after a set time if they are not inside the vessel, preventing memory leaks and physics clutter.
*   `KillZone`: A safety net trigger that destroys any coin falling off the stage.
*   **Extension**: Create new `GameObject` prefabs for different coin types (e.g., heavy coins, bouncy coins) and swap them in the `CoinLauncher`.

`Location: Assets/Scripts/Gameplay`

### Combat & Scoring
The battle logic converts physical placement into numerical stats.
*   `Vessel`: Uses a `HashSet<Collider>` to track coins currently inside the scoring zone. It applies a `dampeningFactor` to incoming coins to help them stay within the container.
*   `EnemyManager`: Manages enemy HP and scales difficulty per wave using constants.
*   `ScoreManager`: Calculates the player's potential damage based on `OnCoinLanded` events.

`Location: Assets/Scripts/Systems`

## 5. Scene Overview
*   **BattleScene**: The primary gameplay scene. It contains the environment (vessel, launch point), the UI Canvas, and the `GameManager`.
*   **Scene Flow**: Currently, the game is self-contained within one scene. The `GameLoop` resets the state (Wave Transit) without reloading the scene to maintain performance and smooth transitions.

`Location: Assets/Scenes`

## 6. UI System
The UI is built using **UGUI** and follows an observer pattern via the `GameEventBus`.
*   `BattleUI`: Displays the current game phase (e.g., "PLAYER ATTACK!"), enemy HP, and the turn timer.
*   `ScoreUI`: Listens to `OnScoreChanged` to update the current points/coins display.
*   `PowerUpUI`: A modal overlay that appears during the `PowerUp` state, allowing the player to select upgrades.
*   **Extension**: To add a new UI element, create a script that subscribes to the relevant `GameEventBus` action (e.g., `OnPlayerDamaged`) and update the visual state.

`Location: Assets/Scripts/UI`

## 7. Asset & Data Model
*   **ScriptableObjects**: The project uses basic references, but configuration is primarily driven by `GameConstants.cs`.
*   **Prefabs**: 
    *   `Coin`: The base physics object with a `Rigidbody` and `CoinLifespan`.
    *   `Vessel`: The container with complex colliders and a trigger for counting.
*   **GameConstants**: A static class containing balance variables like `WHEEL_VELOCITY_THRESHOLD`, `SCORE_PER_COIN`, and `ENEMY_BASE_HP`. This is the "single source of truth" for game balancing.

`Location: Assets/Scripts/Data`

## 8. Notes, Caveats & Gotchas
*   **Physics Jitter**: The `Vessel` script sets `collisionDetectionMode` to `Continuous` when a coin enters. If adding many coins, monitor the physics thread performance.
*   **Input Scaling**: The `CoinLauncher` scales mouse wheel delta by `WHEEL_VELOCITY_TO_FORCE_SCALE`. Different mouse hardwares/drivers might report scroll deltas differently; this may need normalization for cross-platform play.
*   **Phase Timing**: The `GameManager` uses `WaitForSeconds` for phase transitions. If adding animations (e.g., enemy attack animations), these delays should be synchronized with the animation duration.
*   **Trigger Stability**: The `Vessel` uses `OnTriggerExit`. If a coin is destroyed while *inside* the trigger (e.g., by a "clear all" effect), `OnTriggerExit` might not fire. The `Vessel.ResetCoins()` method manually clears the set to prevent ghost counts.