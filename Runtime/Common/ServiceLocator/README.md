# ServiceLocator 服務定位器模式

服務定位器（Service Locator）是一種設計模式，用於將服務的使用端與具體實作端解耦。它提供一個集中的註冊與獲取點，避免類別之間產生直接的依賴。

## 主要類別與結構

### 1. `IServiceLocator` 與 `ServiceLocatorFunction`
- `IServiceLocator` 定義了定位器的基礎介面，持有服務字典與生命週期回呼列表。
- `ServiceLocatorFunction` 提供了服務的註冊、建立與查詢擴充方法（Extension Methods）：
  - `HasService<T>()`
  - `GetService<T>()`
  - `AddService<T>(T service)`
  - `CreateService<T>()`
  - `CreateServiceComponent<T>()`

### 2. `GlobalServiceLocator`
- 全域跨場景的服務定位器。
- 繼承自 `GlobalSingletonComponent`，在遊戲啟動後會以 `DontDestroyOnLoad` 跨場景保留。

### 3. `LocalServiceLocator`
- 場景層級（Scene-local）的服務定位器。
- 繼承自 `LocalSingletonComponent`，每個 Unity 場景可擁有一個獨立的定位器。

## 支援非 MonoBehaviour 的生命週期驅動

本模組內置了三個介面：
- `IUpdatable`：定義 `OnUpdate()`
- `IFixedUpdatable`：定義 `OnFixedUpdate()`
- `ILateUpdatable`：定義 `OnLateUpdate()`

當你註冊一個**純 C# 類別（非 MonoBehaviour）**作為服務時，如果它實作了上述任一介面，服務定位器會自動將其加入輪詢列表。在 ServiceLocator 自身的 `Update`、`FixedUpdate` 或 `LateUpdate` 中，會自動觸發這些服務的生命週期方法。

這讓你能在不使用 `MonoBehaviour` 的情況下，仍然享有 Unity 生命週期的更新驅動。

## 使用範例

### 1. 定義服務

```csharp
using UnityEngine;
using DouduckLib;

// Component 服務
public class SomeComponentService : MonoBehaviour
{
    public void DoSomething() { }
}

// 純 C# 類別服務，實作 IUpdatable
public class SomeService : IUpdatable
{
    public void OnUpdate()
    {
        // 自動被 ServiceLocator 的 Update 呼叫
    }
}
```

### 2. 註冊服務

```csharp
using UnityEngine;
using DouduckLib;

public class SomeClass : MonoBehaviour
{
    void Awake()
    {
        var locator = GlobalServiceLocator.Get();

        // 註冊純 C# 服務
        locator.CreateService<SomeService>();

        // 註冊 Component 服務
        locator.CreateServiceComponent<SomeComponentService>();
    }
}
```

### 3. 獲取與使用服務

```csharp
using UnityEngine;
using DouduckLib;

public class AnotherClass : MonoBehaviour
{
    void Update()
    {
        // 獲取並呼叫服務
        var service = GlobalServiceLocator.Get().GetService<SomeComponentService>();
        service?.DoSomething();
    }
}
```

## 常用 API 說明

以擴充方法形式提供（需引入 `using DouduckLib;`）：

| 方法名稱 | 說明 |
| :--- | :--- |
| `HasService<T>()` | 檢查定位器中是否已存在型別為 `T` 的服務。 |
| `GetService<T>()` | 取得型別為 `T` 的服務。若不存在則返回 `null`。 |
| `AddService<T>(T service)` | 將已實例化的服務 `service` 註冊至定位器。若該型別已註冊則會返回 `false`。 |
| `CreateService<T>()` | 實例化一個 `new T()` 並註冊。 |
| `CreateServiceComponent<T>()` | 在定位器物件上掛載 Component `T` 並註冊。 |
