# Project Technical Overview: CoinTossGame

## 1. Project Description
**CoinTossGame** is a physics-based arcade experience where players attempt to toss coins into a target container (vessel). The core experience centers on a unique "Scroll-to-Launch" mechanic, where the speed of the mouse wheel directly correlates to the force applied to the coin. The project is designed as a modular, event-driven system built with Unity 6, focusing on extensible gameplay loops and decoupled communication.

## 2. Gameplay Flow / User Loop
1.  **Launching State**: The player scrolls the mouse wheel. The system tracks the peak scroll velocity.
2.  **Toss Mechanic**: Upon releasing/stopping the scroll, if the peak velocity exceeds a threshold, a coin is instantiated and launched from the `launchPoint`.
3.  **Judging State**: The game transitions to a judging state while the coin is in flight.
4.  **Scoring**: If a coin enters the `Vessel` trigger, the score is updated. Physics dampening is applied to the coin to help it settle.
5.  **Progression**: The game loop supports waves and power-up phases (managed by the `GameStateMachine`), allowing for future expansion into a "rogue-like" or "wave-based" structure.

## 3. Architecture
The project follows a **Decoupled Event-Driven Architecture** combined with a **State Machine** pattern.

### Core Architecture
*   `GameEventBus`: A static event hub that facilitates communication between systems without direct references. It defines events for coin tosses, landing, score changes, and game state transitions.
*   `GameManager`: The central coordinator that initializes the state machine and listens to the `GameEventBus` to drive state transitions.
*   `GameStateMachine`: A logic-only class that manages the current game state (`Launching`, `Judging`, `ShowingScore`, `PowerUp`, etc.) and enforces valid transition rules.

`Location: Assets/Scripts/Core`

## 4. Game Systems & Domain Concepts

### Launcher System
Uses the **Unity Input System** to monitor mouse wheel movement.
*   `CoinLauncher`: Calculates the peak velocity of the mouse scroll. It maps this velocity to an impulse force using constants defined in `GameConstants`.
*   **Extension**: New launch types (e.g., keyboard charging, touch flicking) can be added by creating new launcher classes that call `GameEventBus.PublishCoinThrown()`.

`Location: Assets/Scripts/Gameplay`

### Detection & Physics System
Handles the interaction between the coin and the goal.
*   `Vessel`: Uses a `HashSet<Collider>` to track coins currently inside the target area. It applies physical dampening (velocity reduction) to help coins "stick" inside the container.
*   **Extension**: Different vessel types with varying point multipliers or physical properties can be created by extending the trigger logic.

`Location: Assets/Scripts/Gameplay`

### Scoring & State System
*   `ScoreManager`: Listens for `OnCoinLanded` events and calculates the total score based on the number of coins successfully held in the vessel.
*   `GameConstants`: A centralized data class containing tuning values like `WHEEL_VELOCITY_THRESHOLD` and `SCORE_PER_COIN`.

`Location: Assets/Scripts/Systems` and `Assets/Scripts/Data`

## 5. Scene Overview
*   **SampleScene**: The primary gameplay scene containing the `GameManager`, `Vessel` (target), and the `CoinLauncher` (camera-linked).
*   **Environment**: Uses Universal Render Pipeline (URP) with specific profiles for PC and Mobile rendering.
*   **Physics**: Custom `PhysicMaterial` assets (`CoinPhysicsMaterial`, `VesselPhysicsMaterial`) are used to control bounciness and friction, critical for the "toss and land" feel.

`Location: Assets/Scenes`

## 6. UI System
The project uses **TextMesh Pro (TMP)** for high-quality text rendering.
*   `ScoreUI`: A decoupled UI component that subscribes to `GameEventBus.OnScoreChanged`. It has no knowledge of the scoring logic, only updating the visual display when notified.
*   **Binding**: Logic is purely event-based; there is no direct coupling between the `ScoreManager` and the `ScoreUI`.

`Location: Assets/Scripts/UI`

## 7. Asset & Data Model
*   **Prefabs**: The `Coin` prefab is the central gameplay entity, requiring a `Rigidbody` and the "Coin" tag for detection.
*   **Data Driven**: Gameplay parameters are stored in `GameConstants.cs` rather than hardcoded in systems.
*   **Models**: Custom meshes like `ConcaveContainer.asset` are used to provide the physical shape of the target vessel.
*   **Rendering**: Universal Render Pipeline (URP) assets are split between `PC_RPAsset` and `Mobile_RPAsset` for cross-platform optimization.

## 8. Notes, Caveats & Gotchas
*   **Scroll Direction**: Currently, the `CoinLauncher` detects absolute scroll speed. A known task exists to restrict launching to "forward" scrolls only.
*   **Physics Stability**: The `Vessel` script sets `CollisionDetectionMode.Continuous` on coins that enter the trigger to prevent them from "tunneling" through the container at high speeds.
*   **Event Cleanup**: All event subscriptions in `OnEnable` are strictly mirrored with removals in `OnDisable` to prevent memory leaks and "MissingReference" exceptions when switching scenes.
*   **Scale Warning**: The `Vessel` script contains a warning about parenting coins to the vessel; doing so might cause mesh distortion due to non-uniform scaling of the container. Physics-based management is preferred.