# Singleton 單例模式

單例模式（Singleton）保證一個類別在應用程式中僅有一個實體（Instance），並提供一個全域的存取點。

## 單例類型與選擇指南

### 1. `Singleton<T>`
- **適用對象**：純 C# 類別（不繼承 `MonoBehaviour`）。
- **特點**：執行緒安全（Thread-safe），使用延遲載入（Lazy Initialization）。
- **調用方式**：`SingletonName.Get()`。

### 2. `GlobalSingletonComponent<T>`
- **適用對象**：需要在多個場景之間持續存在並繼承自 `MonoBehaviour` 的元件。
- **特點**：
  - 在 `Awake` 時會自動呼叫 `DontDestroyOnLoad(gameObject)` 跨場景保存。
  - 支援自動建立（預設）或手動擺放。
  - 當遊戲退出（Application Quit）時，會防止重新建立實例以避免殘留 GameObject。
- **調用方式**：`SingletonName.Get()`。

### 3. `LocalSingletonComponent<T>`
- **適用對象**：場景限定（Scene-local）的 `MonoBehaviour` 元件。
- **特點**：
  - 每個場景（Unity Scene）可擁有自己獨立的該單例實例。
  - 當場景被銷毀/切換時，該場景的單例實例會隨之銷毀。
- **調用方式**：`SingletonName.Get(gameObject)` 或 `SingletonName.Get(component)`（需要傳入一個物件以確定所屬場景）。

### 4. `SingletonScriptableObject<T>`
- **適用對象**：`ScriptableObject` 單例。
- **特點**：第一次存取時會在載入資源中搜尋（`Resources.FindObjectsOfTypeAll<T>()`）並快取。
- **調用方式**：`SingletonName.Get()`。

## 建立選項：`SingletonOption`

對於繼承自 `SingletonComponentBase` 的單例，可以透過第二個泛型參數傳入建立選項：
- `SingletonOption.AutoCreate`（預設）：若呼叫 `Get()` 時實例不存在，會自動在當前場景中建立該單例物件。
- `SingletonOption.NotAutoCreate`：若呼叫 `Get()` 時實例不存在，返回 `null`。這要求開發者必須在場景中手動預先擺放該單例物件。

## 使用範例

### 1. 純 C# 單例 (`Singleton<T>`)

```csharp
using DouduckLib;

public class SomeSingleton : Singleton<SomeSingleton>
{
    public SomeSingleton()
    {
        // 初始化邏輯
    }
}
```

### 2. 全域 MonoBehaviour 單例 (`GlobalSingletonComponent<T>`)

覆寫 `OnSingletonAwake()` 和 `OnSingletonDestroy()` 取代 Unity 原生的 `Awake()` 與 `OnDestroy()`：

```csharp
using UnityEngine;
using DouduckLib;

public class SomeGlobalSingleton : GlobalSingletonComponent<SomeGlobalSingleton>
{
    protected override void OnSingletonAwake()
    {
        // 單例初始化邏輯
    }

    protected override void OnSingletonDestroy()
    {
        // 銷毀邏輯
    }
}
```

### 3. 場景單例 (`LocalSingletonComponent<T>`) 與 `NotAutoCreate` 

```csharp
using UnityEngine;
using DouduckLib;

public class SomeLocalSingleton : LocalSingletonComponent<SomeLocalSingleton, SingletonOption.NotAutoCreate>
{
    protected override void OnSingletonAwake()
    {
        // 場景單例初始化邏輯
    }
}

// 於其他元件中調用：
public class SomeClass : MonoBehaviour
{
    void Start()
    {
        // 傳入自己的 gameObject 以查詢當前場景的單例
        var singleton = SomeLocalSingleton.Get(gameObject);
    }
}
```

### 4. ScriptableObject 單例 (`SingletonScriptableObject<T>`)

```csharp
using UnityEngine;
using DouduckLib;

[CreateAssetMenu(fileName = "SomeSetting", menuName = "Settings/SomeSetting")]
public class SomeScriptableObjectSingleton : SingletonScriptableObject<SomeScriptableObjectSingleton>
{
    public float SomeValue;
}
```
