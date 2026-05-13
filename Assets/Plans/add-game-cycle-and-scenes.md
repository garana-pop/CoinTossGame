# Project Overview
- **Game Title**: CoinTossGame (拡張版)
- **High-Level Concept**: 物理ベースのコイン投入バトルに、ローグライクなマップ進行要素を追加したゲーム。
- **Players**: シングルプレイヤー
- **Target Platform**: PC (Stand-alone Windows)
- **Render Pipeline**: URP

# Game Mechanics
## Core Gameplay Loop
1. **MapScene**: ノードを選択して進行方向を決める。
2. **ActionScene (Battle/Store/Event)**:
    - **Battle**: 敵と戦い、勝利して報酬（お金・アイテム）を得る。
    - **Store**: お金を消費してアイテムや回復を購入する。
    - **Event**: 選択肢によってメリット・デメリットが発生する。
3. **Loop**: 全てのノードを攻略し、最終ボス撃破を目指す。HPが0になるとゲームオーバー。

## Controls and Input Methods
- **Map**: マウスクリックでノードを選択。
- **Battle**: マウスホイールでコインを投擲（既存）。
- **Store/Event**: マウスクリックで項目を選択。

# UI
- **Map UI**: 分岐するノードの可視化。現在の位置と進行可能ルートの表示。
- **Store UI**: 商品リスト、価格表示、所持金表示。
- **Event UI**: イベントテキスト、選択肢ボタン。
- **Global HUD**: 現在のHP、所持金、所持アイテムの常時表示（DontDestroyOnLoadで管理）。

# Key Asset & Context
- `RunManager.cs`: ゲーム全体の進行状況（HP、所持金、進行度、所持アイテム）を管理するシングルトン。
- `MapManager.cs`: マップの生成と遷移の制御。
- `StoreManager.cs`: ショップの品揃えと購入処理。
- `EventManager.cs`: イベントの表示と選択結果の適用。
- `MapNodeData.cs` (ScriptableObject): ノードの種類や中身の定義。
- `ItemData.cs` (ScriptableObject): アイテムの効果（攻撃力UP、弾丸変更など）の定義。
- `EventData.cs` (ScriptableObject): イベントのテキストと選択肢の定義。

# Implementation Steps

## 1. 永続的なデータ管理システムの構築 (RunManager)
- **[New] Assets/Scripts/Core/RunManager.cs**:
    - `DontDestroyOnLoad` で動作し、全シーンで共通のデータ（HP、Money、Inventory）を保持。
    - シーン遷移関数（`LoadMap`, `LoadBattle`, etc.）を実装。
- **[Modify] Assets/Scripts/Systems/PlayerManager.cs**:
    - 起動時に `RunManager` から現在のHPを同期するように変更。
- **[Modify] Assets/Scripts/Core/GameEventBus.cs**:
    - `OnMoneyChanged`, `OnItemObtained` などの新規イベントを追加。

## 2. BattleScene の調整 (1戦完結型への変更)
- **[Modify] Assets/Scripts/Core/GameManager.cs**:
    - 無限ウェーブ形式から「1体の敵を倒したらクリア」の形式に変更。
    - 勝利後、リザルトを表示して `MapScene` に戻る処理を追加。
    - 敗北時、`GameOver` 画面へ遷移。
- **[Modify] Assets/Scripts/Systems/EnemyManager.cs**:
    - `RunManager` の進行度に応じたHP補正を適用。

## 3. MapScene の実装
- **[New] Assets/Scenes/MapScene.unity**: マップ画面用シーン。
- **[New] Assets/Scripts/Gameplay/MapNode.cs**: 各ノードの挙動（クリックされたら指定のシーンへ）。
- **[New] Assets/Scripts/Systems/MapManager.cs**: ノードの接続関係と解放状態の管理。
- **[New] UI/MapCanvas**: ノードを配置するUI。

## 4. StoreScene の実装
- **[New] Assets/Scenes/StoreScene.unity**: ショップ画面。
- **[New] Assets/Scripts/Systems/StoreManager.cs**:
    - ランダムなアイテム（HP回復、弾変更アイテムなど）を表示。
    - 購入時に `RunManager` のお金を減らし、効果を適用。

## 5. EventScene の実装
- **[New] Assets/Scenes/EventScene.unity**: イベント画面。
- **[New] Assets/Scripts/Systems/EventManager.cs**:
    - `EventData` からテキストと選択肢を読み込む。
    - 選択肢クリック時に「HP±X」「お金±Y」などの効果を実行。

## 6. 特殊アイテムの実装 (弾の形状変更)
- **[Modify] Assets/Scripts/Gameplay/CoinLauncher.cs**:
    - `RunManager` が持つ「現在の弾薬Prefab」を参照するように変更。
- **[New] アイテム「特殊な弾」**: 取得時に `RunManager` の `CurrentBulletPrefab` を上書きする。

# Verification & Testing
- **Map -> Battle -> Map**: 敵を倒した後にマップに戻り、次のノードが選べるか。
- **Persistence**: バトルで減ったHPや獲得したお金が Store/Event シーンに正しく引き継がれるか。
- **Store Purchase**: お金が足りない時に購入できないか。購入後にアイテムの効果（弾の見た目など）が変わるか。
- **Event Consequence**: 選択肢に応じたHP増減やアイテム獲得が正しく行われるか。
