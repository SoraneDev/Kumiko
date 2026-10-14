# Kumiko

Unity（uGUI）向けの、ゲームパッド・マウス＆キーボード・タッチに対応したUIインタラクションライブラリです。
Selectableのように機能を1つのクラスに詰め込まず、小さなコンポーネントに分けて、アタッチするだけで機能を拡張できます。

## 特徴

- 入力種別（Navigation / Pointer / Touch）に応じて、表示する状態を自動で切り替える
- 機能ごとにコンポーネントを分け、同じGameObjectに足して拡張できる（Gate、ナビゲーションなど）
- コアは外部ライブラリに依存しない（R3は拡張として別途対応）
- Unity標準のSelectable（Sliderなど）も、Kumikoの状態管理の下で使える

## 動作環境

- Unity 6
- uGUI
- Input System（Input System UI Input Module）
- R3（任意）

## インストール

### Git URL 経由でのインストール

Git パッケージの path クエリパラメータをサポートするバージョンの Unity が必要です（Unity 2019.3.4f1 以降、または Unity 2020.1a21 以降）。パッケージマネージャーに `https://github.com/SoraneDev/Kumiko.git?path=/Assets/Kumiko` を追加できます。

## クイックスタート

1. ボタンにするGameObjectに、`KumikoButton` を追加する（クリックを受けるGraphicが必要です）
2. 必要に応じて、`DoubleClickGate` や `CooldownGate` を同じGameObjectに追加する
3. ゲームパッドで操作するUIは、`DirectionalNavigator` を追加して、上下左右の遷移先を設定する
4. スクリプトから、イベントを購読する

```csharp
using Kumiko;
using UnityEngine;

public class Example : MonoBehaviour
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

## コンポーネント

| コンポーネント | 役割 |
|---|---|
| `KumikoElement` | 基本となるコンポーネント。ホバー・選択・押下・操作可否の状態を管理し、入力種別に応じた表示状態を決める |
| `KumikoButton` | 決定入力（クリック・タップ・Submit）を通知する。Gateを付けられる |
| `DoubleClickGate` | 決定入力に、ダブルクリックの条件を付与する |
| `CooldownGate` | 決定入力に、クールダウンの条件を付与する |
| `DirectionalNavigator` | 方向入力を受けたとき、設定した要素へ選択を移す |
| `KumikoSelectableBridge` | Unity標準のSelectable（Sliderなど）を、Kumikoの状態管理の下で使えるようにする |
| `InputModalityTracker` | 入力種別の保持と固定（`Lock` / `Unlock`）を行う |

## 入力種別と表示状態

入力種別は、最後に使われた入力から自動で判定されます。種別によって、表示する状態が変わります。

| 入力種別 | 対象 | 表示する状態 |
|---|---|---|
| Navigation | キーボード・ゲームパッド | Focused / Pressed / Disabled |
| Pointer | マウス・ペン | Hovered / Pressed / Disabled |
| Touch | 指 | Pressed / Disabled |

入力種別は、`InputModalityTracker.Current.Lock(...)` で固定できます。固定を解除するには、`Unlock()` を呼びます。

## 見た目の変更

表示状態は、`KumikoElement.State` で取得し、`StateChanged` イベントで変化を購読できます。
見た目の切り替えは、これを購読して実装します。

## R3との連携

コアはR3に依存しません。R3を使う場合は、次の手順で、拡張メソッドを有効にします。

1. Scripting Define Symbolsに、`KUMIKO_R3` を追加する
2. `using Kumiko.R3;` を追加する

```csharp
using Kumiko.R3;
using R3;

_button.OnConfirmAsObservable()
    .Subscribe(_ => Debug.Log("決定されました。"))
    .AddTo(this);
```

## 注意事項

- 公開イベントの購読者は、自身の処理で発生する例外を適切に処理してください。購読者から例外が伝播した場合、同じイベントに登録された後続の購読者への通知は保証されません。
- 購読したイベントは、利用側で解除してください。

## 既知の制限

- Gateは実行時に追加・削除しないでください。`Awake` 時点でアタッチされているGateだけが対象です。
- Input System UI Input Moduleを前提としています。ほかのInputModuleでは、タッチ入力もPointerとして扱われます。
- 選択中の要素が操作不可能になると、選択が外れます。選択の復帰は未対応です。
- 水平方向の入力で値を変えるSliderには、`DirectionalNavigator` の左右の遷移先を設定しないでください。値の変更と選択の移動が、両方実行されます。
- DropdownとScrollView、InputFieldも使用可能ですが、遷移の設定が難しく、動作が安定しないため現在のバージョンの本ライブラリを使用するうえでは、使用はおすすめしません。今後のアップデートで対応予定です。

## ライセンス

このライブラリはMITライセンスの下で提供されています。