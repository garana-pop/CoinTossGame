# CoinTossGame Technical Overview

## 1. Project Description
**CoinTossGame** is a physics-based "coin pusher" battle game where players launch coins into a target vessel using a mouse wheel "flick" gesture. The project is designed as a wave-based survival experience where coin accuracy directly translates to damage against enemies. It is built for **Windows Standalone** using **Unity 6 (6000.4.4f1)** and the **Universal Render Pipeline (URP)**.

### Core Pillars
- **Physical Mastery:** Launching coins with the mouse wheel requires timing and speed control.
- **Strategic Accumulation:** Maximizing the number of coins held in the vessel during a limited player phase.
- **Progression:** Choosing power-ups between waves to survive escalating enemy difficulty.

---

## 2. Gameplay Flow / User Loop
The experience follows a rigid state-based loop controlled by a Central Game State Machine:

1.  **Wave Start:** System initializes the enemy for the current wave (scaling HP based on wave number).
2.  **Player Phase (Launching):** The player has 10 seconds to launch as many coins as possible into the vessel using the mouse wheel.
3.  **Judging Phase:** A brief pause (2 seconds) allows physics to settle.
4.  **Damage Phase:** The number of coins in the vessel is counted. Each coin deals damage (10 dmg/coin) to the enemy.
5.  **Enemy Phase:** If the enemy survives, the player takes fixed damage.
6.  **PowerUp Phase:** Upon enemy defeat, the player selects from randomly generated upgrades.
7.  **Wave Transit:** The arena resets, and the next wave begins.
8.  **Game Over:** Triggered when Player HP reaches zero.

---

## 3. Architecture
The project employs a **Centralized Management** pattern combined with a **Static Event Bus** for decoupled communication.

### Core Architecture Components
*   `GameManager`: The central brain. Orchestrates the high-level game loop using Coroutines and manages transitions between states.
*   `GameStateMachine`: A pure C# class that defines the valid transitions between game phases (Idle, Launching, Judging, etc.).
*   `GameEventBus`: A static class containing C# `Action` events. This allows systems like the UI or Score Manager to react to gameplay events (e.g., `OnCoinLanded`, `OnEnemyDamaged`) without direct references to the source.
*   `GameConstants`: A static class containing all magic numbers (gravity, forces, HP scales, durations) for easy balancing.

**Location:** `Assets/Scripts/Core`

---

## 4. Game Systems & Domain Concepts

### Launching System (Input to Physics)
Translates hardware mouse wheel velocity into physical impulse.
*   `CoinLauncher`: Captures scroll delta, tracks peak velocity, and applies a scaled force to coin prefabs. It includes jitter/randomization to prevent perfect repetition.
*   `CoinLifespan`: Automatically destroys coins that fall out of bounds or fail to enter the vessel after a timeout.

**Location:** `Assets/Scripts/Gameplay`

### Vessel & Scoring System
Handles the "Target" logic and coin counting.
*   `Vessel`: Uses a `HashSet<Collider>` with `OnTriggerEnter/Exit` to accurately track coins. It modifies coin physics (dampening) upon entry to help them stay within the container.
*   `ScoreManager`: Listens to `GameEventBus.OnCoinLanded` to update the current score/damage potential.

**Location:** `Assets/Scripts/Gameplay` and `Assets/Scripts/Systems`

### Entity Management
Manages health and life cycles of the player and enemies.
*   `PlayerManager`: Tracks player health.
*   `EnemyManager`: Tracks enemy health and scales difficulty per wave.

**Location:** `Assets/Scripts/Systems`

---

## 5. Scene Overview
The project is designed to run primarily within a single scene with dynamic state transitions.
*   `BattleScene.unity`: The main gameplay scene containing the arena, vessel, launcher, and UI overlay.
*   `0.unity` (Recovery): A fallback/temp scene.

**Scene Flow Rules:**
- The `GameManager` starts the `GameLoop` coroutine immediately on `Start()`.
- UI overlays (`BattleUI`, `PowerUpUI`) are toggled based on the `GameStateMachine` state.

**Location:** `Assets/Scenes`

---

## 6. UI System
The project uses **UGUI** and **TextMesh Pro** for its interface.
*   `BattleUI`: Displays the current game phase (e.g., "PLAYER PHASE"), turn timer, and current score.
*   `ScoreUI`: A dedicated component for showing real-time coin counts and damage.
*   `PowerUpUI`: Managed as a modal screen that appears during the `PowerUp` state. It uses the `GameEventBus.OnPowerUpSelected` event to signal the `GameManager` to resume the loop.

**Location:** `Assets/Scripts/UI`

---

## 7. Asset & Data Model
*   **Prefabs:** The `Coin` prefab is central, containing a `Rigidbody`, `Collider`, and `CoinLifespan` script.
*   **Physics Materials:** `BouncyPhysicsMaterial` and `VesselPhysicsMaterial` are used to fine-tune coin interactions (low friction/high bounce for coins).
*   **Scriptable Data:** While many constants are in `GameConstants.cs`, `StageConfig.cs` is used for per-level or per-arena settings.
*   **Input:** Uses the **New Input System** (`InputSystem_Actions.inputactions`).

**Location:** `Assets/Prefabs`, `Assets/Materials`, `Assets/Scripts/Data`

---

## 8. Notes, Caveats & Gotchas
*   **Physics Scaling:** The `CoinLauncher` scales force based on `GameConstants.WHEEL_VELOCITY_TO_FORCE_SCALE`. Changing this requires recalibrating the vessel distance.
*   **Continuous Collision:** The `Vessel` script forces `CollisionDetectionMode.Continuous` on coins entering the trigger to prevent them from "tunneling" through the mesh at high speeds.
*   **Coroutine Dependency:** The entire game loop is a single large Coroutine in `GameManager`. If this script is disabled or the object destroyed, the game state will hang.
*   **Mouse Wheel Direction:** The code specifically filters for `scroll.y > 0` (forward scroll) for launching. Backwards scrolling is ignored.