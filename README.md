# Kumiko（組子）

Unity（uGUI）向けの、ゲームパッド・マウス＆キーボード・タッチに対応した UI インタラクションライブラリです。
使っている入力の種類に合わせて UI の表示状態を自動で切り替え、ダブルクリック・クールダウン・方向入力による選択移動などの機能を、小さなコンポーネントの組み合わせで追加できます。

*A UI interaction library for Unity uGUI that supports gamepad, mouse & keyboard, and touch.*

### 名前の由来

**組子（くみこ）** は、釘を使わずに細かく加工した木の部材を組み合わせ、繊細な模様を作り出す日本の伝統的な木工技術です。
一つひとつは小さな部材でも、組み合わせ方しだいでさまざまな模様が生まれます。小さなコンポーネントを組み合わせて UI の機能を作るこのライブラリの考え方を、その技術に重ねて名付けました。

---

## 特徴

- **入力の種類に応じた表示状態**：ゲームパッド・キーボード、マウス、タッチを区別し、それぞれに合った表示状態へ自動で切り替えます。マウスで操作している間はホバー、ゲームパッドで操作している間はフォーカスを表示する、といった使い分けができます。
- **小さなコンポーネントの組み合わせ**：Unity 標準の `Selectable` のように 1 つのコンポーネントへ機能をまとめず、決定入力・ダブルクリック・クールダウン・方向ナビゲーションなどを別々のコンポーネントに分けています。必要なものだけを同じ GameObject に追加して組み合わせます。
- **決定入力の共通化**：マウスのクリック、タッチのタップ、ゲームパッド・キーボードの Submit を、同じ `Confirmed` イベントで受け取れます。
- **入力処理と見た目の分離**：表示状態はイベントで通知します。色・スケール・Animator などによる演出は、利用側で自由に実装できます。
- **CanvasGroup に対応**：親の CanvasGroup の `interactable` を、Unity 標準の Selectable と同じ判定方法で反映します。
- **Kumiko だけで作れる基本の UI**：ボタン・トグル・スライダーを、Unity 標準の Selectable を使わずに作れます。
- **自動ナビゲーション**：方向入力の先にある要素を自動で探して、選択を移します。必要な方向だけ、移動先を手動で指定することもできます。
- **Unity 標準 UI との連携**：Kumiko にまだない UI（InputField など）は、`KumikoSelectableBridge` で標準の Selectable と組み合わせられます。
- **R3 に依存しないコア**：基本機能は外部ライブラリに依存しません。R3 を導入している場合は、専用の拡張メソッドを使えます。

## 動作環境

- Unity 6 以降
- uGUI
- Input System 1.18.0 以降（シーンの EventSystem に Input System UI Input Module を設定します）
- R3（任意。拡張メソッドを使う場合のみ）

---

## 導入

### Package Manager（Git URL）

`Window > Package Manager` の `+` ボタンから `Install package from git URL...` を選び、次の URL を入力します。

```
https://github.com/SoraneDev/Kumiko.git?path=/Assets/Kumiko
```

`?path=` を付けることで、リポジトリ内の Kumiko パッケージだけを導入します。

Kumiko は Input System を使用します。Input System は依存パッケージとして自動で導入されます（1.18.0 以降）。

### 手動

このリポジトリの `Assets/Kumiko` フォルダを、プロジェクトの `Packages` 以下に配置します。
`Assets` 以下に配置すると、Hierarchy の作成メニュー（`Kumiko > Button` など）がプレハブを見つけられなくなるため、`Packages` 以下に配置してください。

---

## クイックスタート

### 1. 入力の設定を確認する

シーンの EventSystem に `Input System UI Input Module` が設定されていることを確認します。
マウスやタッチで操作するには、Raycast を受け取れる Graphic（Image など）と、Canvas の `Graphic Raycaster` も必要です。

### 2. ボタンを作る

Hierarchy の右クリックメニューから `Kumiko > Button` を選ぶと、Kumiko のボタンを作成できます。
既存の GameObject をボタンにする場合は、`KumikoButton` コンポーネントを追加します。

### 3. 決定を受け取る

`KumikoButton` の `Confirmed` イベントを購読すると、クリック・タップ・Submit を同じ処理で受け取れます。

```csharp
using Kumiko;
using UnityEngine;

public sealed class ButtonExample : MonoBehaviour
{
    [SerializeField] private KumikoButton _button;


    private void OnEnable()
    {
        _button.Confirmed += OnConfirmed;
    }


    private void OnDisable()
    {
        _button.Confirmed -= OnConfirmed;
    }


    private void OnConfirmed()
    {
        Debug.Log("決定されました。");
    }
}
```

### 4. 見た目を切り替える

`KumikoElement` の `StateChanged` イベントで、表示状態に合わせて見た目を切り替えます。

```csharp
using Kumiko;
using UnityEngine;
using UnityEngine.UI;

public sealed class ButtonColorView : MonoBehaviour
{
    [SerializeField] private KumikoElement _element;
    [SerializeField] private Image _image;


    private void OnEnable()
    {
        _element.StateChanged += OnStateChanged;

        // 購読しただけでは通知されないため、現在の状態を反映しておく
        OnStateChanged(_element.State);
    }


    private void OnDisable()
    {
        _element.StateChanged -= OnStateChanged;
    }


    private void OnStateChanged(InteractionState state)
    {
        _image.color = state switch
        {
            InteractionState.Hovered => Color.cyan,
            InteractionState.Focused => Color.yellow,
            InteractionState.Pressed => Color.gray,
            InteractionState.Disabled => new Color(1f, 1f, 1f, 0.5f),
            _ => Color.white,
        };
    }
}
```

リポジトリの `Assets/Demo` には、色・スケール・Animator の Trigger を使った演出の例があります。

---

## コンポーネント構成

Kumiko では、基本となる UI 要素に、必要な機能のコンポーネントを追加して使います。

| コンポーネント | 役割 |
|---|---|
| `KumikoElement` | 入力イベント・操作可否・表示状態を管理する基本のコンポーネント。継承して独自の UI 要素を作れる |
| `KumikoButton` | 決定入力を受け、Gate の判定結果をイベントで通知するボタン |
| `KumikoToggle` | 決定入力でオンとオフを切り替えるトグル |
| `KumikoSlider` | 範囲内の値を、ドラッグや方向入力で変えるスライダー |
| `DoubleClickGate` | 2 回の決定入力で決定とする Gate |
| `CooldownGate` | 決定から一定時間、次の決定を受け付けない Gate |
| `DirectionalNavigator` | 上下左右の方向入力で、指定した要素、または自動で探した要素へ選択を移す |
| `KumikoSelectableBridge` | Unity 標準の Selectable と、操作できるかどうかを同期する |

Gate は `KumikoButton`、ほかのコンポーネントは `KumikoElement`（またはその派生）と同じ GameObject に追加します。

Hierarchy の作成メニューで作れる UI と、付いているコンポーネントは次のとおりです。

| メニュー | 付いているコンポーネント |
|---|---|
| `Kumiko > Button` | `KumikoButton`、`DirectionalNavigator` |
| `Kumiko > Slider` | `KumikoSlider`、`DirectionalNavigator` |
| `Kumiko > Toggle` | `KumikoToggle`、`DirectionalNavigator` |

コンポーネントではありませんが、現在の入力の種類は `InputModalityTracker` が管理しています。`InputModalityTracker.Current` からアクセスできます。

---

## 使い方

### 表示状態

`KumikoElement` は、次のいずれか 1 つの表示状態（`InteractionState`）を持ちます。現在の状態は `State` で取得でき、変化したときは `StateChanged` で通知されます。

| 状態 | 説明 |
|---|---|
| `Normal` | 通常の状態 |
| `Hovered` | マウスなどのポインタが重なっている状態 |
| `Focused` | EventSystem で選択されている状態 |
| `Pressed` | 押されている状態 |
| `Disabled` | 操作できない状態 |

表示状態は、次の優先順位で決まります。

```
Disabled → Pressed → 入力の種類に応じた状態 → Normal
```

### 入力の種類

Kumiko は UI のイベントから入力の種類（`InputModality`）を判定し、表示する状態を切り替えます。

| 入力の種類 | 対象 | 表示する状態 |
|---|---|---|
| `Navigation` | キーボード・ゲームパッド | 選択されていれば `Focused` |
| `Pointer` | マウス・ペン | ポインタが重なっていれば `Hovered` |
| `Touch` | タッチ | `Normal`（押下中は `Pressed`） |

ボタンが選択されている状態でも、マウスで操作している間は `Hovered`、ゲームパッドで操作している間は `Focused` を表示できます。

現在の入力の種類は `InputModalityTracker.Current.Modality` で取得でき、変化したときは `ModalityChanged` で通知されます。

### 入力の種類を固定する

一時的に入力の種類を固定したい場合は、`Lock` を使います。

```csharp
InputModalityTracker.Current.Lock(InputModality.Navigation);
```

固定している間も入力の種類の判定は続きますが、表示には反映されません。`Unlock` で固定を解除すると、最後に判定した入力の種類に戻ります。

```csharp
InputModalityTracker.Current.Unlock();
```

### 操作できるかどうか

`KumikoElement.Interactable` で、操作できるかどうかを切り替えます。

```csharp
element.Interactable = false;
```

- 実際に操作できるかどうかは `IsInteractable()` で取得します。親の CanvasGroup の `interactable` も考慮されます。
- 選択中の要素が操作できなくなると、選択が外れます。

### トグル

`KumikoToggle` は、決定入力（クリック・タップ・Submit）のたびにオンとオフを切り替えます。

```csharp
toggle.IsOnChanged += OnIsOnChanged;   // 不要になったら解除する

toggle.IsOn = true;                    // IsOnChanged が通知される
toggle.SetIsOnWithoutNotify(false);    // 通知せずに切り替える
```

- Inspector の `Is On Graphic` にチェックマークなどの Graphic を設定すると、オンのときだけ表示します。
- 切り替えの演出を自分で作る場合は、`Is On Graphic` を設定せずに `IsOnChanged` を購読してください。

### スライダー

`KumikoSlider` は、範囲内の値をドラッグや方向入力で変えます。

```csharp
slider.ValueChanged += OnValueChanged;  // 不要になったら解除する

slider.Value = 0.5f;                   // ValueChanged が通知される
slider.SetValueWithoutNotify(0.2f);    // 通知せずに変える
```

- つまみ以外を押すと、押した位置へ値が移ります。つまみを押した場合は、値を飛ばさずにドラッグできます。
- `Direction` の軸と同じ向きの方向入力で、`Step Size` ずつ値が変わります。その方向では `DirectionalNavigator` による選択の移動は起きません。
- 塗り（`Fill Rect`）とつまみ（`Handle Rect`）は、値に合わせてアンカーが書き換えられます。塗りの Image を Filled にした場合は、`fillAmount` が書き換えられます。
- Inspector で `Direction` を変えると、Unity 標準の Slider と同じように、自身と子のレイアウトも自動で変わります。横と縦が入れ替わる場合は大きさや位置の横と縦を入れ替え、逆向きになる場合は配置を反転します。この変更は Undo で元に戻せます。
- 親に合わせて伸縮するアンカーの場合は、アンカーも入れ替えるので、幅や高さが 0 になって消えることはありません。
- Inspector では、`Min Value` が `Max Value` を超える値は反映されません。`Value` は範囲内のスライダーで設定できます。
- スクリプトから方向を変える場合は、`SetDirection(direction, flipsLayout: true)` でレイアウトも合わせて変更できます。値だけを変える場合は `flipsLayout: false` を指定します。

```csharp
slider.SetDirection(SlideDirection.BottomToTop, flipsLayout: true);
```

### 決定入力に条件を付ける（Gate）

`KumikoButton` と同じ GameObject に Gate を追加すると、決定入力に条件を付けられます。

- **DoubleClickGate**：1 回目の決定入力を記録し、指定した時間内に 2 回目の決定入力があったときに決定とします。1 回目では `Confirmed` を通知しません。
- **CooldownGate**：決定してから指定した時間、次の決定入力を拒否します。

複数の Gate を追加すると、すべての Gate を通過したときだけ決定になります。Gate の判定は次の 2 段階に分かれているため、ほかの Gate の結果を考慮して内部の状態を更新できます。

1. `Evaluate`：内部の状態を変えずに、通過させてよいかを判定する
2. `Commit`：最終的な結果が決まった後に、内部の状態を更新する

たとえば `DoubleClickGate` と `CooldownGate` を組み合わせた場合、クールダウン中の 2 回目の入力は拒否され、クールダウンは決定したときだけ始まります。

最終的な結果は、次のように通知されます。

| 結果 | 通知 |
|---|---|
| `Passed` | `Confirmed` を通知する |
| `Pending` | 判定の途中のため、何も通知しない（ダブルクリックの 1 回目など） |
| `Rejected` | `ConfirmRejected` を通知する。拒否の効果音などに使える |

複数の Gate が通過以外の結果を返した場合は、`Stage` が `Recognition`（入力の形の判定）の Gate を、`Restriction`（回数や頻度の制限）の Gate より優先します。同じ `Stage` の場合は、Inspector で上にある Gate の結果を採用します。
無効になっている Gate は判定に使われません。

### 独自の Gate を作る

`KumikoGate` を継承すると、独自の条件を作れます。

```csharp
using Kumiko;
using UnityEngine;

public sealed class UseLimitGate : KumikoGate
{
    [SerializeField, Min(1)] private int _maxUseCount = 1;


    private int _useCount;


    public override ConfirmGateStage Stage => ConfirmGateStage.Restriction;


    public override ConfirmGateResult Evaluate()
    {
        return _useCount < _maxUseCount ? ConfirmGateResult.Passed : ConfirmGateResult.Rejected;
    }


    public override void Commit(ConfirmGateResult ownResult, ConfirmGateResult finalResult)
    {
        // 決定したときだけ回数を数える
        if (finalResult == ConfirmGateResult.Passed) _useCount++;
    }


    public override void ResetState()
    {
        // コンポーネントが無効になったときに呼ばれる。回数は残すため何もしない
    }
}
```

### 独自の UI 要素を作る

`KumikoElement` を継承すると、操作可能な状態で受けた入力に応じた処理を追加できます。表示状態と操作可否の管理は基底クラスが行います。

| メソッド | 呼ばれるタイミング |
|---|---|
| `OnConfirmInput()` | 決定入力（クリック・タップ・Submit）を受けたとき |
| `OnPointerDownInput(eventData)` | ポインタの押下（マウスの左ボタン・タッチなど）を受けたとき |
| `UsesMoveInput(direction)` | 方向入力を要素自身の操作に使うかを問い合わせるとき。`true` を返した方向では `DirectionalNavigator` が選択を移さない |
| `OnMoveInput(direction)` | `UsesMoveInput` で `true` を返した方向入力を受けたとき |

### 方向ナビゲーション

`DirectionalNavigator` を追加すると、ゲームパッドやキーボードの方向入力で選択を移せます。移動先は、手動で指定することも、自動で探すこともできます。

- **手動の移動先**：Inspector の `Select On Up / Down / Left / Right` に、移動先の `KumikoElement` を設定します。設定した方向では、自動で探すよりも優先します。
- **自動で探す**：`Mode` で、移動先が未設定（または選択できない）方向を、自動で探すかどうかを決めます。

| Mode | 自動で探す方向 |
|---|---|
| `Manual` | 探さない。設定した移動先へだけ選択を移す |
| `Automatic` | 上下左右 |
| `Horizontal` | 左右だけ |
| `Vertical` | 上下だけ |

自動で探す場合は、方向入力の先にある要素のうち、近くて向きのずれが小さい要素を選びます。評価方法は Unity 標準の Selectable と同じです。

- 対象になるのは、`DirectionalNavigator` を持ち、同じルートの Canvas にある、選択できる要素だけです。選択を移した先から、さらに方向入力で移動できるようにするためです。
- 開いていないメニューの要素は、CanvasGroup の `interactable` をオフにすると対象から外れます。
- `Wrap Around` をオンにすると、`Horizontal` と `Vertical` で、端から反対側の端へ回り込みます。回り込む先は、反対側の端のうち、自身と同じ行（または列）にある要素です。
- 選択を移す先は、`FindTarget(direction)` で取得できます。

次の場合は、選択を移しません。

- 移動先が見つからない場合や、手動の移動先が選択できず、自動で探さない場合。
- スライダーの値を変える方向など、要素自身が操作に使う方向。

### Unity 標準の Selectable との連携

`KumikoSelectableBridge` を使うと、Kumiko にまだない標準 Selectable（InputField など）の機能を使いながら、操作できるかどうかの管理と表示状態の通知を Kumiko で行えます。トグルとスライダーは、Kumiko のコンポーネントを使ってください。

- `KumikoSelectableBridge` は、標準 Selectable の Navigation と Transition を `None` に設定し、Kumiko の操作可否を Selectable に反映します。
- 見た目の切り替えと選択の移動は、Kumiko 側で管理してください。

### R3 との連携（任意）

プロジェクトに R3 を導入している場合は、`Kumiko.R3` の拡張メソッドを使えます。

```csharp
using Kumiko;
using Kumiko.R3;
using R3;
using UnityEngine;

public sealed class ButtonR3Example : MonoBehaviour
{
    [SerializeField] private KumikoButton _button;


    private void Start()
    {
        _button.ConfirmAsObservable()
            .Subscribe(_ => Debug.Log("決定されました。"))
            .AddTo(this);
    }
}
```

| メソッド | 内容 |
|---|---|
| `StateAsObservable()` | 表示状態の変化。購読した時点で現在の状態も流す |
| `ConfirmAsObservable()` | 決定の通知 |
| `ConfirmRejectedAsObservable()` | 決定入力が拒否されたときの通知 |
| `IsOnAsObservable()` | トグルのオンとオフの切り替わり。購読した時点で現在の状態も流す |
| `ValueAsObservable()` | スライダーの値の変化。購読した時点で現在の値も流す |
| `ModalityAsObservable()` | 入力の種類の変化。購読した時点で現在の入力の種類も流す |

`OnConfirmAsObservable()` と `OnConfirmRejectedAsObservable()` は非推奨です。`ConfirmAsObservable()` と `ConfirmRejectedAsObservable()` を使ってください。

拡張メソッドは、R3 の Unity 向けパッケージ（`com.cysharp.r3`）1.3.1 以降を導入すると自動で有効になります。Scripting Define Symbols を手動で追加する必要はありません。R3 は Kumiko と一緒には導入されないため、使う場合は別途導入してください。

---

## 設定項目

### KumikoElement / KumikoButton

| 項目 | 説明 | 初期値 |
|---|---|---|
| Interactable | 操作できるかどうか | オン |

### DoubleClickGate

| 項目 | 説明 | 初期値 |
|---|---|---|
| Double Click Interval | 1 回目の決定入力から、2 回目として受け付ける時間（秒） | 0.4 |

### CooldownGate

| 項目 | 説明 | 初期値 |
|---|---|---|
| Cooldown Duration | 決定してから、次の決定入力を受け付けるまでの時間（秒） | 1 |

### KumikoToggle

| 項目 | 説明 | 初期値 |
|---|---|---|
| Interactable | 操作できるかどうか | オン |
| Is On | オンかどうか | オフ |
| Is On Graphic | オンのときだけ表示する Graphic（チェックマークなど）。未設定でもよい | なし |

### KumikoSlider

| 項目 | 説明 | 初期値 |
|---|---|---|
| Interactable | 操作できるかどうか | オン |
| Fill Rect | 値に合わせて伸縮する塗り。親が伸縮する範囲になる | なし |
| Handle Rect | 値の位置へ移動するつまみ。親が移動する範囲になる | なし |
| Direction | 値が最小から最大へ向かう方向 | Left To Right |
| Min Value / Max Value | 値の範囲 | 0 / 1 |
| Whole Numbers | 値を整数に丸めるかどうか | オフ |
| Value | 現在の値 | 0 |
| Step Size | 方向入力 1 回で変化する量。整数のみの場合は 1 以上の整数に丸める | 0.1 |

### DirectionalNavigator

| 項目 | 説明 | 初期値 |
|---|---|---|
| Mode | 移動先が未設定、または選択できない方向で、自動で探す範囲（Manual・Automatic・Horizontal・Vertical） | Manual |
| Wrap Around | `Horizontal` と `Vertical` で、端から反対側の端へ回り込むかどうか | オフ |
| Select On Up / Down / Left / Right | 方向入力で選択を移す先の `KumikoElement`。設定した方向では、自動で探すよりも優先する | なし |

`Mode` の初期値は、以前のバージョンと同じ動きになるように `Manual` にしています。Hierarchy の作成メニューで作った UI では、`Automatic` に設定されています。

### KumikoSelectableBridge

設定項目はありません。同じ GameObject に標準の Selectable（InputField など）が必要です。

---

## 仕様と注意点

- **イベントの購読**：`StateChanged` や `ModalityChanged` は、購読した時点では通知しません。現在の値は `State` や `Modality` から取得してください。購読したイベントは、不要になったら解除してください。
- **購読者の例外**：イベントの購読者から例外が発生した場合、同じイベントに登録されている後続の購読者への通知は保証されません。
- **Submit の押下表示**：Submit には押している期間がないため、`Pressed` と解除が続けて通知されます。押下の演出を一定時間見せたい場合は、演出側で表示時間を制御してください。
- **起動時の入力の種類**：再生開始時は `Navigation` として扱います。
- **ポインタの判定**：押下とクリックは、マウスの左ボタンだけを扱います。ポインタが動いていないのに UI が動いてポインタの下に入った場合は、入力の種類を切り替えません。
- **クリックによる選択**：マウスやタッチで押した要素は、EventSystem で選択されます。後からゲームパッドに切り替えたとき、最後に触れた要素から操作を始められます。
- **時間**：Gate の判定は `Time.timeScale` の影響を受けません。
- **無効化**：コンポーネントが無効になると、ホバー・選択・押下の状態と、Gate の判定途中の状態がリセットされます。
- **アプリケーションのフォーカス**：アプリケーションがフォーカスを失ったときは、押下状態を解除します。
- **ドメインリロード**：Enter Play Mode Options でドメインリロードを無効にしていても、`InputModalityTracker.Current` は再生開始時に作り直されます。
- **値の表示**：スライダーは、つまみと塗りのアンカーを書き換えて値を表示します。書き換えるアンカーは Inspector で編集できなくなります。

---

## 既知の制限

- **Gate の実行時の追加・削除**：`KumikoButton` の `Awake` の時点で付いている Gate だけが対象です。
- **選択の復帰**：選択中の要素が操作できなくなると、選択が外れます。ほかの要素へ自動で選択を移す機能はありません。
- **自動ナビゲーションの対象**：別のルート Canvas にある要素は、自動では探しません。Canvas をまたいで移動させる場合は、移動先を手動で設定してください。
- **Input Module**：入力の種類の判定は、Input System UI Input Module を前提としています。ほかの Input Module では、タッチも `Pointer` として扱われます。
- **トグルのグループ**：複数のトグルから 1 つだけを選ぶグループ機能はありません。
- **一部の標準 UI**：Dropdown・ScrollView・InputField などは、選択の移動や操作の調整が複雑なため、現在のバージョンでは使用を推奨していません。

---

## ライセンス

[MIT License](LICENSE)
