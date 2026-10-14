# Kumiko

**Kumiko（組子）** は、UnityのuGUI向けに開発されたUIインタラクションライブラリです。

ゲームパッド、マウス・キーボード、タッチ操作に対応し、使用している入力種別に応じてUIの表示状態を自動的に切り替えます。

Unity標準の`Selectable`のように複数の機能を1つのコンポーネントにまとめるのではなく、機能を小さなコンポーネントに分離しています。

必要なコンポーネントを同じGameObjectにアタッチすることで、UIの機能を組み合わせて拡張できます。

## 特徴

- **入力種別に応じた状態管理**  
  ゲームパッド・キーボード、マウス、タッチ操作を区別し、それぞれに適した表示状態へ自動的に切り替えます。

- **コンポーネントによる機能拡張**  
  ボタン、方向ナビゲーション、ダブルクリック、クールダウンなどの機能を、必要に応じて追加できます。

- **入力処理と見た目の分離**  
  UIの表示状態をイベントとして通知します。色、スケール、Animatorなどによる演出は、利用側で自由に実装できます。

- **Unity標準UIとの連携**  
  `KumikoSelectableBridge`を使用することで、SliderやToggleなどの標準Selectableと組み合わせられます。

- **外部ライブラリに依存しないコア**  
  基本機能はR3などの外部ライブラリに依存しません。R3が導入されている場合は、専用の拡張メソッドを利用できます。

## 動作環境

| 項目 | 対応環境 |
|---|---|
| Unity | Unity 6以降 |
| UIシステム | uGUI |
| 入力システム | Input System |
| UI入力モジュール | Input System UI Input Module |
| R3 | 任意（拡張機能のみ） |

※ R3は必須ではありません。基本機能はR3を導入せずに使用できます。

## インストール

### Git URLからインストール

UnityのPackage Managerからインストールできます。

1. Unity Editorで`Window > Package Manager`を開きます。
2. 左上の`+`メニューから`Install package from git URL...`を選択します。
3. 次のURLを入力してインストールします。

```text
https://github.com/SoraneDev/Kumiko.git?path=/Assets/Kumiko
```

Git URLの`?path=`パラメータを使用し、リポジトリ内のKumikoパッケージのみをインストールします。

インストール後は、Input Systemと、シーン内のEventSystemに設定されたInput System UI Input Moduleを使用してください。

## クイックスタート

### 1. ボタンを作成する

Hierarchyの右クリックメニューから、`Kumiko > Button`を選択すると、Kumiko用のボタンを作成できます。

既存のGameObjectから作成する場合は、`KumikoButton`を追加してください。

マウスやタッチで操作するには、Raycastを受け取れるGraphicと、Canvas側のGraphic Raycasterが必要です。

### 2. ボタンのイベントを購読する

`KumikoButton`の`Confirmed`イベントを購読することで、マウスのクリック、タッチ、ゲームパッド・キーボードのSubmit入力を共通の処理で受け取れます。

```csharp
using Kumiko;
using UnityEngine;

public class ButtonExample : MonoBehaviour
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

### 3. 必要な機能を追加する

同じGameObjectにコンポーネントを追加することで、ボタンの機能を拡張できます。

- `DoubleClickGate`：2回の決定入力を条件にする
- `CooldownGate`：決定後、一定時間の再入力を制限する
- `DirectionalNavigator`：方向入力による選択先を設定する

例えば、`KumikoButton`に`DoubleClickGate`と`CooldownGate`を追加することで、ダブルクリックによる決定とクールダウンを組み合わせられます。

## コンポーネント構成

Kumikoでは、基本となるUI要素に必要な機能を追加して使用します。

| コンポーネント | 役割 |
|---|---|
| `KumikoElement` | UIの入力イベント、操作可否、表示状態を管理する基本コンポーネント |
| `KumikoButton` | 決定入力を処理し、結果をイベントとして通知する |
| `KumikoModule` | UI要素に機能を追加するための抽象基底クラス |
| `DoubleClickGate` | 2回の決定入力を認識する |
| `CooldownGate` | 決定入力にクールダウンを設定する |
| `DirectionalNavigator` | 上下左右の入力に応じた選択移動を行う |
| `KumikoSelectableBridge` | Unity標準のSelectableと操作可能状態を同期する |

### InputModalityTracker

`InputModalityTracker`はコンポーネントではなく、現在の入力種別を管理するクラスです。

`InputModalityTracker.Current`からアクセスできます。

## 入力種別と表示状態

### 入力種別

KumikoはUIイベントから入力種別を判定し、現在の入力に適した表示状態へ切り替えます。

| 入力種別 | 対象 | 主な表示状態 |
|---|---|---|
| `Navigation` | キーボード・ゲームパッド | Focused |
| `Pointer` | マウス・ペン | Hovered |
| `Touch` | タッチ操作 | Normal |

いずれの入力種別でも、押下中は`Pressed`、操作できない場合は`Disabled`が優先されます。

入力種別は`InputModalityTracker`によって管理され、入力操作に応じて更新されます。

### 表示状態

`KumikoElement`は内部的に複数の入力状態を保持し、表示用に次のいずれかの`InteractionState`を決定します。

| 状態 | 説明 |
|---|---|
| `Normal` | 通常状態 |
| `Hovered` | ポインタが重なっている状態 |
| `Focused` | EventSystemで選択されている状態 |
| `Pressed` | 押下状態 |
| `Disabled` | 操作不可能な状態 |

表示状態は、次の優先順位に従って決定されます。

**Disabled → Pressed → 入力種別に対応した状態 → Normal**

例えば、ボタンが選択されている状態でも、マウス操作中はHovered、ゲームパッド操作中はFocusedを表示できます。

### 入力種別を固定する

入力種別を一時的に固定したい場合は、`Lock()`を使用します。

```csharp
InputModalityTracker.Current.Lock(InputModality.Navigation);
```

固定中も入力種別の検知は継続しますが、表示に使用する入力種別は変更されません。

固定を解除すると、最後に検知した入力種別へ戻ります。

```csharp
InputModalityTracker.Current.Unlock();
```

## 表示状態に応じた見た目の変更

Kumikoは、UIの入力状態と見た目の変更処理を分離しています。

現在の表示状態は`KumikoElement.State`から取得でき、状態が変化したときは`StateChanged`イベントで通知されます。

```csharp
using Kumiko;
using UnityEngine;
using UnityEngine.UI;

public class ButtonColorView : MonoBehaviour
{
    [SerializeField] private KumikoElement _element;
    [SerializeField] private Image _image;

    private void OnEnable()
    {
        _element.StateChanged += OnStateChanged;
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
            InteractionState.Normal => Color.white,
            InteractionState.Hovered => Color.cyan,
            InteractionState.Focused => Color.yellow,
            InteractionState.Pressed => Color.gray,
            InteractionState.Disabled => Color.darkGray,
            _ => Color.white
        };
    }
}
```

状態に応じた見た目の変更方法は自由です。

リポジトリの`Assets/Demo`には、以下の実装例があります。

- 色を変化させるアニメーション
- スケールを変化させるアニメーション
- AnimatorのTriggerを使用したアニメーション

なお、Submit入力ではPressedと解除の状態が連続して通知されます。押下アニメーションを一定時間表示したい場合は、演出側で表示時間を制御してください。

## Gateによる決定入力の制御

`KumikoButton`は、同じGameObjectに追加されたGateコンポーネントを使用して、決定入力に条件を付けられます。

### DoubleClickGate

最初の決定入力を記録し、指定した時間内に2回目の決定入力を受けた場合に通過します。

初期設定の判定間隔は`0.4`秒です。

### CooldownGate

決定入力が成立した後、指定した時間の再入力を拒否します。

初期設定のクールダウンは`1`秒です。

### 複数のGateの組み合わせ

複数のGateを追加した場合、Kumikoは各Gateの判定結果をまとめて最終的な結果を決定します。

Gateの判定は、以下の2段階に分かれています。

1. `Evaluate()`：内部状態を変更せずに条件を評価する
2. `Commit()`：最終結果に応じて内部状態を更新する

これにより、複数の入力条件を組み合わせた場合でも、最終的な決定結果を考慮して状態を更新できます。

### 決定結果の通知

| 結果 | 動作 |
|---|---|
| `Passed` | `Confirmed`イベントを通知 |
| `Pending` | 判定途中のためイベントを通知しない |
| `Rejected` | `ConfirmRejected`イベントを通知 |

`ConfirmRejected`は、拒否音やエラー演出などに使用できます。

## 方向ナビゲーション

`DirectionalNavigator`を追加することで、ゲームパッドやキーボードの方向入力によるUIの選択移動を設定できます。

Inspectorから以下の遷移先を指定できます。

- Up
- Down
- Left
- Right

遷移先には`KumikoElement`を指定します。

対象が非アクティブ、または操作不可能な場合は選択を移動しません。

現在のバージョンは、遷移先を手動で設定する方式です。自動ナビゲーションには対応していません。

## Unity標準のSelectableとの連携

`KumikoSelectableBridge`を使用することで、Unity標準のSelectableをKumikoと組み合わせられます。

例えば、SliderやToggleなど、Unity標準コンポーネントの機能を利用しながら、Kumikoによる操作可否の管理や表示状態の通知を使用できます。

`KumikoSelectableBridge`は、標準SelectableのNavigationとTransitionを無効化し、Kumikoの操作可能状態をSelectableに反映します。

そのため、見た目の切り替えや選択移動はKumiko側で管理してください。

## R3との連携（任意）

KumikoのコアはR3に依存していません。

プロジェクトに対応するR3パッケージが導入されている場合は、`Kumiko.R3`の拡張メソッドを利用できます。

**`KUMIKO_R3`をScripting Define Symbolsに手動で追加する必要はありません。**

`Kumiko.R3.asmdef`に設定された`versionDefines`によって、対応するR3パッケージが存在する場合に`KUMIKO_R3`が自動的に定義され、拡張機能が有効になります。

※ R3自体はKumikoのインストール時に自動導入されません。R3連携を利用する場合のみ、別途R3をインストールしてください。

### 使用例

```csharp
using Kumiko;
using Kumiko.R3;
using R3;
using UnityEngine;

public class ButtonR3Example : MonoBehaviour
{
    [SerializeField] private KumikoButton _button;

    private void Start()
    {
        _button.OnConfirmAsObservable()
            .Subscribe(_ => Debug.Log("決定されました。"))
            .AddTo(this);
    }
}
```

### 利用可能な拡張メソッド

| メソッド | 内容 |
|---|---|
| `StateAsObservable()` | UIの表示状態をObservableとして取得 |
| `OnConfirmAsObservable()` | 決定入力の成立通知を取得 |
| `OnConfirmRejectedAsObservable()` | 決定入力の拒否通知を取得 |
| `ModalityAsObservable()` | 入力種別の変更通知を取得 |

`StateAsObservable()`と`ModalityAsObservable()`は、購読開始時に現在の値を通知します。

## 注意事項

- `StateChanged`などの公開イベントは、購読した時点では現在の状態を通知しません。初期状態は`State`などのプロパティから取得してください。
- イベント購読後は、利用側で適切に購読解除を行ってください。
- 公開イベントの購読者から例外が伝播した場合、同じイベントに登録されている後続の購読者への通知は保証されません。
- Gateの評価順序には`ConfirmGateStage`が使用されます。

## 既知の制限

- **Gateの実行時追加・削除には未対応**  
  `KumikoButton`の`Awake()`で取得したGateが対象です。

- **選択復帰機能は未実装**  
  選択中の要素が操作不可能になった場合、選択が解除されます。ほかの要素への自動的な選択移動や復帰には対応していません。

- **入力種別の判定はInput System UI Input Moduleを前提とする**  
  ほかのInput Moduleでは、タッチ入力もPointerとして分類される場合があります。

- **Sliderとの方向入力の競合**  
  水平方向の入力で値を変更するSliderに対して、`DirectionalNavigator`の左右の遷移先を設定すると、値の変更と選択移動が同時に発生する場合があります。

- **一部の標準UIとの組み合わせは制限あり**  
  Dropdown、ScrollView、InputFieldなどは、選択遷移や操作の調整が複雑なため、現在のバージョンでは積極的な利用を推奨していません。今後の改善対象としています。

## ライセンス

このライブラリはMITライセンスの下で公開されています。
