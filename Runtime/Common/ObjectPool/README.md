# ObjectPool 對象池模式

這是一個高效且類型安全的泛型對象池實現，旨在重複使用頻繁建立（`Instantiate`）與摧毀（`Destroy`）的物件，以減少記憶體與垃圾回收（GC Alloc）帶來的效能消耗。

本對象池支援 Unity 的 `GameObject` / `Component`，亦可用於池化任何純 C# 的 Class 物件。

---

## 繼承結構與職責分工

本模組採用 **「鏈式繼承 (Chained Inheritance)」** 結構，以實現最大程度的代碼重用與清晰的 API 隔離：

1. **`ObjectPoolBase<TObject>`** (純 C# 核心)
   - 負責核心的對象池狀態管理（`Stack`、`HashSet` 容器操作）。
   - 提供無參數的 `Spawn()` 核心演算法。
   - 內建 **Double Despawn (重複回收) 防禦機制** 與 **無 GC 的 `DespawnAll` 批次回收優化**。
   - 完全與 Unity 引擎解耦，可用於一般 C# Class 的池化。

2. **`GameObjectPool<TObject>`** (Unity 專用 - 無參數版)
   - 繼承自 `ObjectPoolBase<TObject>`，約束 `where TObject : Component`。
   - 實作與 Unity 相關的實例化（`Instantiate`）、銷毀（`Destroy`）與啟用邏輯（`SetActive`）。
   - 對外提供乾淨的無參數 `Spawn()` 接口。

3. **`GameObjectPool<TObject, TData>`** (Unity 專用 - 帶參數版)
   - 繼承自 `GameObjectPool<TObject>`。
   - 繼承了所有 Unity 相關的實作，額外擴充強型別的資料傳遞 API。
   - 支援 `Spawn(TData spawnParam)` 與 `OnSpawned(Action<TObject, TData>)` 事件。

---

## 核心安全與效能優化

### 1. 防禦性重複回收檢查 (Double Despawn Defense)
在呼叫 `Despawn` 時，若偵測到物件並不處於啟用（Active）狀態，或不屬於本對象池，將會自動攔截並拋出警告（`Debug.LogWarning`），防止池內 Stack 狀態損壞，避免產生重複取出的遊戲 Bug。

### 2. 無 GC 批次回收 (No-GC DespawnAll)
`DespawnAll()` 內部引入了快取緩衝機制，遍歷回收時不會產生任何的 Enumerator 建立與銷毀，將批次回收的效能優化至 $O(N)$ 且達成零運行時 GC Alloc。

### 3. Inspector 序列化支援
所有對象池類別均已標記為 `[System.Serializable]`。您可以在 `MonoBehaviour` 中直接序列化具體子類別，並在 Unity 編輯器 Inspector 中配置 `prefab`、`initialSize`、`maxSize` 以及 `instantiateParent`。

---

## C# 標準事件與生命週期回呼

對象池支援 C# 原生的事件機制（多播委派），便於您在物件生命週期的各個階段進行監聽，且支援 `+=` 註冊與 `-=` 取消訂閱，能有效防範記憶體洩漏：

* **`OnCreated`** (`Action<TObject>`)：當新物件被實例化時觸發。
* **`OnSpawned`** (`Action<TObject>`)：當物件被取出（Spawn）並啟用時觸發。
* **`OnSpawned`** (`Action<TObject, TData>`)：*(僅限帶參數對象池)* 物件被取出並啟用時觸發，同時傳遞強型別參數資料。
* **`OnDespawned`** (`Action<TObject>`)：當物件被回收（Despawn）並停用時觸發。
* **`OnReleased`** (`Action<TObject>`)：當物件被對象池釋放（因超出 `maxSize` 限制或釋放池）即將銷毀前觸發。

---

## 使用範例

### 範例 1：無參數對象池 (簡化版)
適用於不需要傳遞額外 Spawn 資料的物件。

```csharp
using UnityEngine;
using DouduckLib;

public class SomeSpawner : MonoBehaviour
{
    [SerializeField] SomeComponent _prefab;
    GameObjectPool<SomeComponent> _pool;

    void Awake()
    {
        _pool = new GameObjectPool<SomeComponent>();
        
        // 初始化：指定 prefab、父節點、初始容量 5、最大閒置限制 10
        _pool.InitializePool(_prefab, transform, 5, 10);

        // 訂閱事件
        _pool.OnSpawned += OnItemSpawned;
    }

    void OnItemSpawned(SomeComponent item)
    {
        // 初始化物件狀態
    }

    void SpawnItem()
    {
        // 借出物件（無參數）
        SomeComponent item = _pool.Spawn();
    }

    void RecycleItem(SomeComponent item)
    {
        // 歸還物件
        _pool.Despawn(item);
    }

    void OnDestroy()
    {
        if (_pool != null)
        {
            // 取消訂閱以防止記憶體洩漏
            _pool.OnSpawned -= OnItemSpawned;
            _pool.ReleasePool();
        }
    }
}
```

### 範例 2：帶參數對象池 (序列化與動態初始化)
適用於生成時需要傳遞特定參數資料進行配置的物件。

```csharp
using UnityEngine;
using System;
using DouduckLib;

public struct SomeData
{
    public Vector3 targetPosition;
    public int parameterValue;
}

public class SomeComponent : MonoBehaviour
{
    public void Setup(SomeData data)
    {
        // 根據傳入的資料進行初始化
    }
}

public class SomeSpawnerWithData : MonoBehaviour
{
    [SerializeField] SomeComponent _prefab;
    
    // 方案 A: 透過繼承宣告可序列化的對象池，可直接在 Inspector 中配置參數
    [SerializeField] SomeComponentPool _serializedPool;

    [Serializable]
    public class SomeComponentPool : GameObjectPool<SomeComponent, SomeData> {}

    // 方案 B: 動態初始化
    GameObjectPool<SomeComponent, SomeData> _dynamicPool;

    void Awake()
    {
        // 方案 A 初始化與事件訂閱
        _serializedPool.InitializePool();
        _serializedPool.OnSpawned += (item, data) => item.Setup(data);

        // 方案 B 初始化與事件訂閱
        _dynamicPool = new GameObjectPool<SomeComponent, SomeData>();
        _dynamicPool.InitializePool(_prefab, transform, 10);
        _dynamicPool.OnSpawned += (item, data) => item.Setup(data);
    }

    void SpawnItem(SomeData data)
    {
        // 從對象池中借出物件，並傳遞 SomeData
        SomeComponent item = _dynamicPool.Spawn(data);
    }

    void OnDestroy()
    {
        _serializedPool.ReleasePool();
        _dynamicPool.ReleasePool();
    }
}
```

---

## 常用 API 列表

| 方法名稱 | 說明 |
| :--- | :--- |
| `InitializePool(TObject prefab, int initialSize)` | 初始化對象池，預先生成 `initialSize` 個物件（容量無上限）。 |
| `InitializePool(TObject prefab, int initialSize, int maxSize)` | 初始化對象池，並設定閒置容量上限為 `maxSize`。超出上限的物件在回收時會被直接銷毀。 |
| `InitializePool(TObject prefab, Transform parent, int initialSize)` | *(限 GameObjectPool)* 指定生成時的父級節點。 |
| `Spawn()` | 從對象池取出一個物件。若無閒置物件則會自動建立，並觸發 `OnSpawned` 原生事件。 |
| `Spawn(TData spawnParam)` | *(限帶參數版)* 從對象池取出一個物件，並將資料傳遞至 `OnSpawned` 事件。 |
| `Despawn(TObject item)` | 將使用完畢的物件回收至池中。會自動防範重複回收，並觸發 `OnDespawned` 原生事件。 |
| `DespawnAll()` | 將目前所有處於啟用狀態（Active）的物件全部回收（無 GC 高效實作）。 |
| `ReleasePool()` | 釋放對象池。回收並銷毀池內所有物件，重置池狀態。 |
| `ActiveObjectNumber` | *(屬性)* 目前正在外部使用的啟用中物件數量。 |
| `InactiveObjectNumber` | *(屬性)* 目前在池內閒置中的物件數量。 |
| `TotalObjectNumber` | *(屬性)* 目前池中管理的所有物件總數（啟用中 + 閒置中）。 |
