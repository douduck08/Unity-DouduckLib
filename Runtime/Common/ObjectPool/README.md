# ObjectPool 對象池模式

這是一個基於泛型的對象池實現，旨在重複使用頻繁建立（`Instantiate`）與摧毀（`Destroy`）的物件，以減少記憶體與垃圾回收（GC Alloc）帶來的效能消耗。

## 主要類別與結構

### 1. `ObjectPoolBase<TObject, TData>`
對象池的泛型抽象基底類別。
- `TObject`：被池化的物件類型。
- `TData`：生成物件時所傳遞的參數資料型別。
- **必須由子類實作的抽象方法**：
  - `InstantiateObject(TObject prefab)`：定義如何實例化新物件。
  - `ReleaseObject(TObject item)`：定義如何釋放/銷毀物件。
  - `SetObjectActive(TObject item, bool active)`：定義如何啟用或停用物件。

### 2. `GameObjectPool<TObject, TData>`
繼承自 `ObjectPoolBase`，專門用於處理 Unity Component 物件的對象池實作。
- `TObject` 限制為 `Component`。
- 自動處理 `Instantiate`、`Destroy` 以及 `SetActive`。
- 支援指定生成的父級節點（`Transform instantiateParent`）。

## 鏈式 API (Fluent API) 支援

對象池支援流式接口（Fluent API）來設定物件在各生命週期階段的事件回呼（Callbacks）：
- `OnCreated(Action<TObject> callback)`：當新物件被實例化時觸發。
- `OnSpawned(Action<TObject, TData> callback)`：當物件從池中被取出（Spawn）並啟用時觸發。
- `OnDespawned(Action<TObject> callback)`：當物件被回收（Despawn）並停用時觸發。
- `OnReleased(Action<TObject> callback)`：當對象池被釋放，物件即將被銷毀時觸發。

## 使用範例

### 1. 宣告被池化的元件與資料結構

```csharp
using UnityEngine;

public struct SomeData
{
    // 生成物件時所需的參數資料
}

public class SomeComponent : MonoBehaviour
{
    public void Setup(SomeData data)
    {
        // 初始化物件邏輯
    }
}
```

### 2. 建立與管理對象池

```csharp
using UnityEngine;
using DouduckLib;

public class SomeClass : MonoBehaviour
{
    [SerializeField] SomeComponent _prefab;
    GameObjectPool<SomeComponent, SomeData> _pool;

    void Awake()
    {
        _pool = new GameObjectPool<SomeComponent, SomeData>();
        
        _pool.InitializePool(_prefab, transform, 10)
             .OnSpawned((item, data) => 
             {
                 item.Setup(data);
             });
    }

    void SpawnItem(SomeData data)
    {
        // 從池中取出物件
        SomeComponent item = _pool.Spawn(data);
    }

    void RecycleItem(SomeComponent item)
    {
        // 將物件回收至池中
        _pool.Despawn(item);
    }

    void OnDestroy()
    {
        // 釋放對象池
        _pool.ReleasePool();
    }
}
```

## 常用 API 說明

| 方法名稱 | 說明 |
| :--- | :--- |
| `InitializePool(TObject prefab, int initialSize)` | 初始化對象池，預先生成 `initialSize` 個物件（容量無上限）。 |
| `InitializePool(TObject prefab, int initialSize, int maxSize)` | 初始化對象池，預先生成 `initialSize` 個物件，並設定閒置容量上限為 `maxSize`（大於 0 生效，超出時 Despawn 會直接銷毀物件）。 |
| `InitializePool(TObject prefab, Transform instantiateParent, int initialSize)` | *(限 GameObjectPool)* 初始化對象池，指定 `Transform` 父節點，預先生成 `initialSize` 個物件（容量無上限）。 |
| `InitializePool(TObject prefab, Transform instantiateParent, int initialSize, int maxSize)` | *(限 GameObjectPool)* 同上，並設定閒置容量上限為 `maxSize`。 |
| `Spawn(TData spawnParam)` | 從對象池取出一個物件。若無閒置物件則會自動建立。取出後會將物件啟用，並觸發 `OnSpawned` 回呼。 |
| `Despawn(TObject item)` | 將使用完畢的物件回收至池中。會將物件停用，並觸發 `OnDespawned` 回呼。若已達 `maxSize` 上限，則會直接銷毀。 |
| `DespawnAll()` | 將目前所有處於啟用狀態（Active）的物件全部回收。 |
| `ReleasePool()` | 釋放對象池。回收所有啟用中的物件，並將池內所有物件銷毀，同時觸發 `OnReleased` 回呼，重置池狀態。 |
| `ActiveObjectNumber` | *(屬性)* 目前正在外部使用的啟用中物件數量。 |
| `InactiveObjectNumber` | *(屬性)* 目前在池內閒置中的物件數量。 |
| `TotalObjectNumber` | *(屬性)* 目前池中管理的所有物件總數（啟用中 + 閒置中）。 |
