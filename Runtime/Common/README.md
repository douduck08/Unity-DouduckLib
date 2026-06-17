# DIContainer & FactoryContainer (依賴注入與工廠容器)

本文件說明 `Runtime/Common` 根目錄下的兩個輕量容器工具。

## DIContainer (依賴注入容器)

**目的：自動管理與注入類別之間的依賴關係，以降低物件之間的耦合度。**

`DIContainer` 是一個輕量型的 IoC 容器，支援構造函數注入（Constructor Injection）、建構子快取（減少反射效能開銷）、指定注入標記與循環依賴防護。

### 核心功能
- **構造函數注入**：自動解析構造函數參數並遞迴注入依賴。
- **快取機制**：自動快取各實作類別的建構子與參數資訊，避免重複反射。
- **注入標記 (`[Inject]`)**：若類別擁有多個構造函數，可使用 `[Inject]` 標記指定特定的建構子；若無標記則預設使用第一個。
- **循環依賴防禦**：若解析過程中偵測到循環依賴，會主動拋出 `InvalidOperationException` 防止 `StackOverflow`。

### 生命週期 (Singleton 與 Transient)
- **Transient (瞬態，只註冊型別)**：使用 `Register<TContract, TImplementation>()`。每次呼叫 `Resolve` 均會建構並回傳**全新**的實體。
- **Singleton (單例，註冊現成實體)**：使用 `Register<TContract, TImplementation>(instance)`。容器會快取該實體，後續重複呼叫 `Resolve` 時將永遠回傳**同一個**實體。

### 使用範例

```csharp
using DouduckLib;

public interface ISomeService { }
public class SomeService : ISomeService { }

public class SomeClass
{
    readonly ISomeService _service;

    // 若有多個建構子，可使用 [Inject] 指定特定一個
    [Inject]
    public SomeClass(ISomeService service)
    {
        _service = service;
    }
}

// 容器的註冊與解析
public class InitExample
{
    void Awake()
    {
        var container = new DIContainer();
        
        // --- 方法 A：Transient 註冊 ---
        // 註冊型別映射，後續 Resolve 時會自動 new 新實體
        container.Register<ISomeService, SomeService>();
        container.Register<SomeClass, SomeClass>();
        
        // --- 方法 B：Singleton 註冊 ---
        // 註冊現有實體，後續重複 Resolve 會永遠返回此同一個物件
        // SomeService existingService = new SomeService();
        // container.Register<ISomeService, SomeService>(existingService);

        // 解析實例（將自動解析建構子並注入對應的 SomeService 實例）
        SomeClass instance = container.Resolve<SomeClass>();
    }
}
```

## FactoryContainer (工廠容器)

**目的：統一管理與呼叫各個物件工廠，將物件的建立邏輯與使用端解耦。**

`FactoryContainer` 提供了一個將工廠類別統一註冊與管理的容器。支援強型別泛型呼叫，提供最高 4 個參數的無反射工廠建立。

### 核心功能
- **泛型工廠介面**：提供 `IFactory<TValue>` 以及帶有 1 至 4 個參數的 `IFactory<TParam1, ..., TValue>` 介面。
- **強型別零反射**：透過泛型多載方法在編譯期進行型別安全檢查，呼叫過程無反射與 GC 開銷。

### 使用範例

```csharp
using DouduckLib;

public class SomeClass { }

// 實作工廠介面
public class SomeFactory : IFactory<SomeClass>
{
    public SomeClass Create()
    {
        return new SomeClass();
    }
}

// 註冊與生成
public class FactoryExample
{
    void Start()
    {
        var container = new FactoryContainer();
        
        // 註冊工廠
        container.Register<SomeClass>(new SomeFactory());

        // 強型別無反射生成
        SomeClass instance = container.Create<SomeClass>();
    }
}
```
