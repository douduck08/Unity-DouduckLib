# Coroutine 協程工具箱

協程工具箱（Coroutine Toolkit）提供了一系列輕量、高效且安全的協程控制工具，用以簡化非同步邏輯、並行等待與時間線序列控制，同時將垃圾回收配置（GC Allocations）降至最低。

## 工具類型與選擇指南

### 1. `CoroutineUtil` (全域協程管理器)
- **定位**：跨場景的全域協程執行器與快速延遲呼叫入口。
- **特點**：
  - **跨場景執行**：提供 `StartCoroutineOnDontDestroy` 用於執行不隨場景切換而中斷的協程。
  - **高效延遲**：提供 `RunDelaySeconds` 與 `RunDelayFrames`。內部實作了 `WaitForSeconds` 快取機制，避免重複創建產生的 GC 記憶體垃圾。
  - **非同步安全**：確保 `RunDelayFrames(0)` 也會強制在下一影格執行，保證非同步執行的順序一致性。
- **參數習慣**：回呼函式（callback / delegate）統一置於參數列表的最後一個，便於使用 Lambda 多行運算式。

### 2. `CoroutineSequence` (鏈式協程播放器)
- **定位**：結構化的時間線協程播放器（類似 DOTween 的 Sequence）。
- **特點**：
  - **鏈式 API**：支援 `Append`（順序播放）、`Joint`（與上一步並行播放）、`Insert`（自訂時間點播入）。
  - **精確並行等待**：當 `Append` 步驟中含有 `Joint` 的子協程時，會完整等待主協程與所有 Joint 協程皆執行完畢後，才推進到下一步驟，確保時序正確。
  - **安全 Yield 阻斷**：`StartCoroutine()` 回傳自訂的唯讀 `CoroutineSequenceYieldInstruction` 結構，既可直接在其他協程中進行 `yield return` 等待，又防範了外部呼叫 `StopCoroutine(coroutine)` 導致狀態損壞，強迫使用內建的 `sequence.StopCoroutine()`。
- **限制**：每個 Sequence 實例均為一次性使用（無法重複啟動），若重複呼叫 `StartCoroutine()` 會發出警告。

### 3. `CoroutineHandle` (底層協程輔助工廠)
- **定位**：提供底層 `IEnumerator` 的多種控制工廠（如 Delay、Parallel、Queue 等）。
- **特點**：
  - 提供並行等待 `ParallelEnumerator` 與佇列等待 `QueueEnumerator`。
  - 內部使用自訂 `WaitForCounter` 產量指令，消除了傳統 `WaitUntil` 帶來的 Lambda 委派與閉包記憶體配置。

---

## 使用範例

### 1. 全域延遲呼叫 (`CoroutineUtil`)

```csharp
using UnityEngine;
using DouduckLib;

public class ExampleComponent : MonoBehaviour
{
    void Start()
    {
        // 延遲 1.5 秒執行（Lambda 位於最後一個參數，可讀性高）
        CoroutineUtil.RunDelaySeconds(1.5f, () =>
        {
            Debug.Log("1.5 seconds have passed!");
        });

        // 延遲 3 影格執行
        CoroutineUtil.RunDelayFrames(3, () =>
        {
            Debug.Log("3 frames have passed!");
        });
    }
}
```

### 2. 鏈式協程播放 (`CoroutineSequence`)

使用 `CoroutineSequence` 可以非常直觀地串接多個協程、時間延遲與 Action 回呼：

```csharp
using System.Collections;
using UnityEngine;
using DouduckLib;

public class SequenceExample : MonoBehaviour
{
    void Start()
    {
        var seq = new CoroutineSequence(this);

        seq.Append(CoroutineA())            // 1. 順序執行協程 A
           .Append(() => Debug.Log("A"))    // 2. 執行回呼印出 A
           .AppendInterval(0.5f)            // 3. 延遲 0.5 秒
           .Append(CoroutineB())            // 4. 順序執行協程 B
           .Joint(CoroutineC())             // 5. 與協程 B 並行執行協程 C (會等待兩者皆完成才前進)
           .Insert(0.2f, CoroutineD())      // 6. 在 Sequence 啟動 0.2 秒時插入執行協程 D
           .OnComplete(() =>
           {
               Debug.Log("Sequence Completed!"); // 7. 結束回呼
           });

        // 啟動序列
        seq.StartCoroutine();
    }

    IEnumerator CoroutineA() { yield return new WaitForSeconds(1f); }
    IEnumerator CoroutineB() { yield return new WaitForSeconds(0.5f); }
    IEnumerator CoroutineC() { yield return new WaitForSeconds(1.2f); } // 比 B 長，會被同步等待
    IEnumerator CoroutineD() { yield return new WaitForSeconds(0.3f); }
}
```

### 3. 在協程中等待序列完成

```csharp
IEnumerator MainFlowRoutine()
{
    var seq = new CoroutineSequence(this);
    seq.Append(MyRoutine());

    // 啟動並等待 Sequence 完成
    yield return seq.StartCoroutine();

    Debug.Log("Sequence finished. Now running the rest of MainFlow...");
}
```

### 4. 正確停止序列

若要中途手動停止 `CoroutineSequence`，請務必呼叫 `sequence.StopCoroutine()`，以確保所有子協程與並行協程都被一併清理：

```csharp
var seq = new CoroutineSequence(this);
seq.Append(InfiniteRoutine()).StartCoroutine();

// 中途正確停止
seq.StopCoroutine();
```
