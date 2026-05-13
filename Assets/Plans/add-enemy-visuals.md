# プロジェクト概要
- ゲームタイトル: CoinTossGame
- コンセプト: 物理ベースのコイン投げバトルゲーム。器にコインを貯めて敵にダメージを与える。
- プレイヤー: シングルプレイヤー
- インスピレーション: なし
- トーン / アートディレクション: 3D物理演算ベース
- ターゲットプラットフォーム: PC (Windows)
- 画面方向 / 解像度: 横画面 (Landscape)
- レンダーパイプライン: URP (PC_RPAsset)

# ゲームメカニクス
## コアゲームプレイループ
1. ウェーブ開始 -> 2. プレイヤーフェーズ（コイン投擲） -> 3. 判定フェーズ -> 4. ダメージフェーズ -> 5. 敵フェーズ（今回のフォーカス対象） -> 6. パワーアップ選択 -> 次のウェーブへ。
## 操作方法
- マウスホイールでコインを投擲。

# UI
- BattleUI: HPやフェーズ情報の表示。
- ScoreUI: 器に乗ったコイン枚数の表示。

# 主要アセットと文脈
- `EnemyVisuals.cs`: 敵の画像切り替えと表示位置の管理。
- `EnemyCameraController.cs`: 敵攻撃時のカメラフォーカス制御。
- `GameEventBus.cs`: 敵攻撃開始・終了イベントの追加。
- `GameManager.cs`: 敵攻撃フェーズのイベント発行処理の追加。

# 実装ステップ
## 1. GameEventBusの拡張
- `GameEventBus.cs`に `OnEnemyAttackStarted` および `OnEnemyAttackFinished` イベントを追加します。
- それぞれのPublishメソッドを実装します。

## 2. GameManagerのロジック更新
- `GameManager.cs` の `GameLoop` 内、`Enemy Phase` の開始直前で `PublishEnemyAttackStarted()` を呼び出します。
- `Enemy Phase` の待機（`WaitForSeconds`）終了後に `PublishEnemyAttackFinished()` を呼び出します。

## 3. 敵ビジュアルコンポーネントの作成
- `EnemyVisuals.cs` を新規作成します。
- `idleSprite`（待機時）, `attackSprite`（攻撃時）, `hitSprite`（被弾時）をインスペクターから設定可能にします。
- `SpriteRenderer` を介して画像を切り替えます。
- 以下のイベントを購読します：
    - `OnWaveStarted`: アイドル画像にリセット。
    - `OnEnemyDamaged`: 被弾画像に切り替え（一定時間後にアイドルに戻す）。
    - `OnEnemyAttackStarted`: 攻撃画像に切り替え。
    - `OnEnemyAttackFinished`: アイドル画像に戻す。

## 4. カメラフォーカス制御の実装
- `EnemyCameraController.cs` を新規作成します。
- `CameraRig`（カメラの親オブジェクト）を制御対象とします。
- `OnEnemyAttackStarted` で敵の位置に合わせて `CameraRig` を移動・回転させます。
- `OnEnemyAttackFinished` で元の位置に戻します。
- `smoothTime`（Lerp等）を用いて滑らかな移動を実現します。

## 5. シーンセットアップ
- `Main Camera` を親オブジェクト `CameraRig` の子にします（既存の `CameraShake` との干渉を避けるため）。
- 新規GameObject `Enemy` を作成します。
    - `SpriteRenderer` をアタッチ。
    - `EnemyVisuals` をアタッチ。
    - `EnemyCameraController` をアタッチ。
    - `Enemy` の Transform を `CoinVessel` の奥（例: Z=8付近）に配置します。
- インスペクターで各種スプライトを設定します。

# 検証とテスト
- 敵フェーズ開始時にカメラが敵にフォーカスし、敵の画像が「攻撃時」に切り替わるか。
- 敵にダメージを与えた際に「被弾時」の画像に切り替わるか。
- 敵フェーズ終了後にカメラが元の位置に戻り、敵の画像が「アイドル時」に戻るか。
- `CameraShake`（コイン発射時の揺れ）が正しく動作し続けているか。
