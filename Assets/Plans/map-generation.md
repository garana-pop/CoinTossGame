# プロジェクト概要
- ゲームタイトル: CoinTossGame
- ハイレベルコンセプト: コインを器に投げ入れる物理アクションを核としたローグライクデッキビルド風アーケードゲーム。
- プレイヤー: シングルプレイヤー。
- インスピレーション: Slay the Spire（マップシステム）。
- ターゲットプラットフォーム: PC (Windows)。
- レンダリングパイプライン: URP (PC_RPAsset)。

# ゲームメカニクス
## コアゲームプレイループ
プレイヤーは分岐するマップ構造を進み、ノードを選択してゲームを進行させます。各ノードはバトル、イベント、商店などの遭遇イベントに繋がります。最終的な目標は、マップの最上部にいるボスを倒すことです。

## 操作と入力方法
- マウスクリック: マップノードの選択。
- マウスホイール: マップのスクロール（画面の高さを超える場合）。

# UI
- **MapScene**:
  - 自動生成されたマップを表示するスクロールエリア。
  - ノードは遭遇タイプ（バトル、商店、イベント等）を表す。
  - 線（コネクター）でノード間を繋ぎ、進行可能なルートを表示する。
  - ノードの状態（ロック中、到達可能、訪問済み）に応じて見た目を変更する。

# 主要アセットとコンテキスト
- `Assets/Scripts/Data/MapGraphData.cs`: 永続的なマップグラフを保存するための新しいデータモデル。
- `Assets/Scripts/Systems/MapGenerator.cs`: ランダムグラフを生成するための新しいユーティリティ。
- `Assets/Scripts/Core/RunManager.cs`: `MapGraphData` を保持するように修正。
- `Assets/Scripts/Systems/MapManager.cs`: ノードの生成と接続線の描画を行うように修正。
- `Assets/Scripts/Gameplay/MapNode.cs`: インタラクション状態（到達可能性など）を処理するように修正。
- `Assets/Settings/BossNode.asset`, `EliteNode.asset`, `TreasureNode.asset`, `RestNode.asset`: 新しいデータアセット。

# 実装ステップ
## 1. データモデルと永続化
1. `MapNodeInstance` と `MapGraphData` クラスを `Assets/Scripts/Data/MapGraphData.cs` に作成。ノードの座標、タイプ、接続インデックスを保存。
2. `RunManager.cs` に `MapGraphData` プロパティと `InitializeMap()` メソッドを追加。
3. `MapGraphData` をシリアル化し、シーンを跨いで保存・読み込みができるようにする。

## 2. マップ生成ロジック
1. `MapGenerator.cs` に階層ベースの生成アプローチを実装：
   - 階層数を定義（15〜18階層）。
   - 各階層に3〜5個のノードをランダムなタイプで生成。
   - 階層 `i` のノードを階層 `i+1` のノードに接続。
   - 最初の階層を「スタート地点」、最後の階層を「単一のボスノード」に設定。
   - すべてのノードが下から上への有効なパスに含まれることを保証。

## 3. アセットの準備
1. `Assets/Settings/` に不足している `MapNodeData` ScriptableObject を作成：
   - `BossNode.asset`, `EliteNode.asset`, `TreasureNode.asset`, `RestNode.asset`
2. それぞれに名称とアイコンを設定。

## 4. UIセットアップとプレハブ作成
1. `MapScene` の `MapCanvas` に `ScrollRect` を追加。
2. 既存のシーンオブジェクトを参考に `MapNode` プレハブを作成。
3. 2点間を繋ぐ「線」のプレハブ（シンプルなUI Image）を作成。
4. `MapNode.cs` を更新し、状態（ロック中など）に応じた見た目の変化を処理できるようにする。

## 5. マップの可視化 (MapManager)
1. `MapManager.cs` を更新：
   - `Start` 時に既存のマップUIをクリア。
   - `RunManager` から `MapGraphData` を取得。
   - 生成された座標に `MapNode` プレハブを生成。
   - 接続されたノード間に「線」画像を配置し、回転・スケーリングで接続を表現。
   - `RunManager` の現在地に基づき、各ノードのクリック可否を更新。

## 6. ロジックの統合
1. `MapNode.OnNodeClicked` を更新：
   - 選択されたノードのインデックスを `RunManager` に記録。
   - ノードを「訪問済み」にマーク。
   - 対応するシーンへ遷移。

# 検証とテスト
- **生成テスト**: 15〜18階層で生成され、最後にボスが配置されているか。
- **接続テスト**: 親子ノードが線で正しく繋がっているか。
- **到達可能テスト**: ノードクリア後、接続された次のノードのみがクリック可能になるか。
- **永続化テスト**: バトルからマップに戻った際、マップの状態（クリア状況、現在地）が維持されているか。
