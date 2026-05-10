# プロジェクト概要
- ゲームタイトル: CoinTossGame
- コンセプト: マウスホイールでコインを飛ばし、器（Vessel）に入った枚数で敵にダメージを与える物理ベースのバトルゲーム。
- プレイヤー: シングルプレイヤー
- ターゲットプラットフォーム: PC (Windows)
- レンダーパイプライン: URP

# ゲームメカニクス
## コアゲームプレイループ
プレイヤーはマウスホイールの操作でコインを射出します。コインは器に向かって飛び、判定フェーズ終了時に器の中にあるコインがダメージとして計算されます。
## ステージ改良
左右・上・奥に壁を設置し、コインが跳弾（リバウンド）するようにします。これにより、壁を利用してコインを器に導くといった戦略的なプレイが可能になります。

# UI
- 現状のHP、タイマー、フェーズ表示UIは維持します。今回の改良に伴うUIの変更はありません。

# 主要アセットとコンテキスト
- `StageWallPhysicsMaterial.physicsMaterial`: 壁とコインに使用する跳ね返り設定用フィジックスマテリアル。
- `StageConfig.cs`: ステージ全体の物理設定（反発係数など）を制御するスクリプト。
- `Coin.prefab`: 跳ね返りやすいようにフィジックスマテリアルを設定します。
- `BattleScene`: 壁オブジェクトを追加し、ステージを構成します。

# 実装手順
1. **フィジックスマテリアルの作成**:
   - `Assets/Materials/BouncyPhysicsMaterial.physicsMaterial` を作成。
   - Bounciness（反発係数）を 0.6 程度に初期設定。
2. **StageConfig スクリプトの実装**:
   - `Assets/Scripts/Gameplay/StageConfig.cs` を作成。
   - インスペクターから `[Range(0, 1)] float bounciness` を調整可能にする。
   - `BouncyPhysicsMaterial` を参照し、値を同期させる機能を実装（`OnValidate` および `Update`）。
3. **ステージの構築**:
   - `BattleScene` に空の GameObject `Stage` を作成。
   - 4つの Cube（`LeftWall`, `RightWall`, `TopWall`, `BackWall`）を配置。
   - **配置パラメータ（目安）**:
     - `LeftWall`: 位置(-5, 5, -6.5), スケール(0.5, 10, 21)
     - `RightWall`: 位置(5, 5, -6.5), スケール(0.5, 10, 21)
     - `TopWall`: 位置(0, 10, -6.5), スケール(10, 0.5, 21)
     - `BackWall`: 位置(0, 5, 4), スケール(10, 10, 0.5)
   - すべての壁に `BouncyPhysicsMaterial` を割り当て。
4. **コインプレハブの更新**:
   - `Assets/Prefabs/Coin.prefab` を開き、Collider に `BouncyPhysicsMaterial` を設定。
5. **シーンへの組み込み**:
   - `Stage` オブジェクトに `StageConfig` コンポーネントを追加し、マテリアルを紐付ける。

# 検証とテスト
- **目視確認**: 壁が器と発射地点を囲うように正しく配置されているか。
- **物理挙動の確認**: コインを発射し、左右や上の壁で跳ね返るか。
- **奥の壁の確認**: 奥の壁に当たったコインが器の方へ戻ってくるか。
- **調整機能の確認**: `StageConfig` のスライダーを動かして、リアルタイムに跳ね返り具合が変わるか。
