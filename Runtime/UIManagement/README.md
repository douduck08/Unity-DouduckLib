# DouduckLib.UIManagement

UI 管理系統（UI Management）是一個針對 Unity UGUI 設計的頁面堆疊管理器。基於 Sub-Canvas（子畫布）機制設計，提供高效能、低開銷且生命週期完整的 UI 頁面（Page）控制框架。

---

## 🌟 核心特色

1. **基於 Sub-Canvas 的渲染隔離**
   每個 `UIPage` 都必須擁有自己的 `Canvas` 組件。作為 `UIManager` Canvas 的子畫布，任何子頁面的 UI 更新與重組（Rebuild）只會侷限於該子畫布內部，徹底隔離渲染，避免觸發整個 UI 系統的大範圍重組。

2. **極致的 UGUI 顯示與輸入優化**
   頁面的隱藏與顯示採用停用 `Canvas.enabled` 與 `GraphicRaycaster.enabled` 的方式，而非直接停用 GameObject。此設計能保留頁面的佈局狀態與網格，完全消除 GameObject 啟用/停用時的 CPU Spikes 與記憶體垃圾回收（GC Alloc）。

3. **健壯的頁面生命週期（Lifecycle）**
   - **`OnInitialized`**：頁面載入/建立後初始化時觸發（僅執行一次）。
   - **`OnPushed`**：頁面被推入堆疊、顯示在螢幕上時觸發。
   - **`OnCovered(UIPage coveringPage)`**：當新的頁面被推送到此頁面之上時觸發，並傳入造成覆蓋的頁面對象。
   - **`OnUncovered(UIPage uncoveringPage)`**：當此頁面上方的頁面被關閉（Pop/Destroy）使此頁面重回最上層時觸發，並傳入解除覆蓋的頁面對象。
   - **`OnPopped`**：頁面被彈出堆疊、隱藏時觸發。

4. **防禦性堆疊安全機制**
   採用 `List` 模擬堆疊。當頁面被非預期銷毀（例如手動 `Destroy` 或場景切換）時，會在 `OnDestroy()` 中自動把自己從 `UIManager` 中移除並觸發 `OnPopped()`。這能保證堆疊清潔，防止後續 Pop 時引發 `MissingReferenceException`。

---

## 🛠️ 快速開始

### 1. 繼承 `UIPage` 建立頁面

```csharp
using UnityEngine;
using DouduckLib.UIManagement;

public class MainMenuPage : UIPage
{
    protected override void OnInitialized()
    {
        // 頁面初始化邏輯（快取組件、註冊本地 UI 事件）
    }

    protected override void OnPushed()
    {
        // 頁面顯示時（例如播放進場動畫、播放背景音樂）
    }

    protected override void OnCovered(UIPage coveringPage)
    {
        // 被上層頁面遮擋時（例如關閉射線檢測以防止誤觸底層按鈕，若上層為全螢幕則可考慮暫時關閉 Canvas.enabled）
        SetInputEnabled(false);
    }

    protected override void OnUncovered(UIPage uncoveringPage)
    {
        // 重新回到最上層時（例如重新開啟射線檢測、刷新數據）
        SetInputEnabled(true);
    }

    protected override void OnPopped()
    {
        // 頁面關閉時（例如播放出場動畫、註銷臨時事件）
    }
}
```

### 2. 使用 `UIManager` 進行推拉控制

```csharp
using UnityEngine;
using DouduckLib.UIManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] MainMenuPage _mainMenuPagePrefab;
    [SerializeField] SettingsPage _settingsPagePrefab;

    void Start()
    {
        // 取得當前場景的 UIManager 並推送首頁
        var manager = UIManager.Get(gameObject);

        // 如果傳入 Prefab，會自動實例化（Clone）為子物件並初始化
        manager.Push(_mainMenuPagePrefab);
    }

    public void OpenSettings()
    {
        var manager = UIManager.Get(gameObject);
        // 推送設定頁面，此時主選單頁面會自動觸發 OnCovered
        manager.Push(_settingsPagePrefab);
    }

    public void CloseTopPage()
    {
        var manager = UIManager.Get(gameObject);
        // 關閉最上層頁面，底下的頁面會自動觸發 OnUncovered 重新取得焦點
        manager.Pop();
    }
}
```
