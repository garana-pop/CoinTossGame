# プロジェクト概要
- ゲームタイトル: CoinTossGame
- ハイレベルコンセプト: 容器（Vessel）にコインを投げ入れ、敵にダメージを与える物理ベースのバトルゲーム。
- プレイヤー: シングルプレイヤー
- インスピレーション / 参照ゲーム: アーケードのコインプッシャー / 物理演算バトル
- トーン / アートディレクション: 3D、物理演算重視
- ターゲットプラットフォーム: PC
- 画面の向き / 解像度: 横画面（Landscape）
- レンダーパイプライン: URP

# ゲームメカニクス
## コアゲームプレイループ
プレイヤーはマウスホイールを使用してコインを発射します。コインが容器に入ると、一定時間後にカウントされ、敵へのダメージとなります。
## 操作と入力方法
コイン発射のためのマウスホイールスクロール。

# UI
- フェーズ表示とウェーブ情報のバトルUI。
- コイン数を表示するスコアUI。
- アップグレードを選択するパワーアップUI。

# 主要アセットとコンテキスト
- `CoinLauncher.cs`: コインの発射を制御するスクリプト。
- `LaunchPoint`: コインが生成される、CoinLauncherの子オブジェクト（Transform）。
- `Assets/Sprites/MuzzleFlash.prefab`: 新しく作成するマズルフラッシュ用のプレハブ。

# 実装ステップ
1. **`CoinLauncher.cs` の修正**:
    - `CoinLauncher` クラスに `[SerializeField] private ParticleSystem muzzleFlash;` を追加します。
    - `PlayLaunchEffects(float force)` メソッド内に、マズルフラッシュを再生するコードを追加します: `if (muzzleFlash != null) muzzleFlash.Play();`。
2. **パーティクルシステムアセットの作成**:
    - 新しい Particle System GameObject を作成します。
    - 「火花（Sparks）」のように見えるよう設定します:
        - **Main**: Duration 1.0, Looping False, Start Lifetime 0.1-0.3, Start Speed 5-10, Start Size 0.05-0.1, Gravity Modifier 0.5.
        - **Emission**: Burst (Count: 15-25).
        - **Shape**: Cone (Angle 25, Radius 0.1).
        - **Size over Lifetime**: 終わりに近づくにつれて小さくなる曲線。
        - **Color over Lifetime**: 黄色/オレンジから透明に変化するグラデーション。
        - **Renderer**: 
            - Material: `Assets/GabrielAguiarProductions/FreeQuickEffectsVol1/Materials/Flare00_AB.mat` を使用。
            - Sorting Order: 100 (他のゲームオブジェクトの手前に表示されるように設定)。
    - これを `MuzzleFlash` という名前で `Assets/Sprites` フォルダにプレハブとして保存します。
3. **シーンへの統合**:
    - `BattleScene` 内の `CoinLauncher` の下にある `LaunchPoint` オブジェクトの子として `MuzzleFlash` プレハブを追加します。
    - ローカルの Position と Rotation をリセットします。
    - `CoinLauncher` コンポーネントの `muzzleFlash` フィールドに、追加した子オブジェクトの `ParticleSystem` をアサインします。

# 検証とテスト
1. **動作確認**: ゲームを再生し、マウスホイールを回してコインを発射します。
2. **視覚的確認**: コインが発射されるたびに、発射位置に黄色/オレンジの火花パーティクルが表示されるか確認します。
3. **描画確認**: パーティクルが他のオブジェクトに埋もれず、意図通り手前に表示されているか確認します。
