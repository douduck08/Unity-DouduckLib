# DouduckLib.Save 存檔系統

通用、極致解耦、與 Unity Inspector 深度整合的事件導向存檔/讀檔架構。

## 系統特色

1. **事件導向的中央集中組裝 (Centralized Event Assembly)**
   - 控制器不介入如何複製與合併資料的細節，亦不需要繼承任何接口（Interface-free）。
   - 提供 `OnPreSave` 與 `OnPostLoad` 事件。由外部的管理器（例如 `SaveManager`）集中進行資料組裝，順序完全由您掌控，杜絕多系統註冊可能發生的隱性欄位覆蓋 Bug。

2. **完全無狀態化 (Stateless) 的格子存檔**
   - `SlotSaveController` 不持有任何 `CurrentSlot` 執行期狀態，避免了「刪除/檢查存檔時意外更改目前遊玩槽」而導致存檔意外覆蓋的隱形 Bug。

3. **智慧反射拷貝 (SaveMapper)**
   - `SaveMapper.Copy` 具備智慧名稱清洗對應功能（剔除底線前綴 `_`、忽略大小寫），能夠無縫將 `private int _money` 與 `public int money` 完成自動配對並拷貝，免去手寫屬性對照的痛苦。

4. **Unity Inspector 深度整合與唯讀預覽**
   - 控制器的內部 `TSaveData _data` 欄位被標記為 `[SerializeField, ReadOnly]`。當控制器被作為序列化變數暴露在管理器上時，其當前記憶體中的資料會直接呈現在 Inspector 中供唯讀監看除錯，不需額外宣告預覽欄位。

5. **高度可客製化的接口**
   - **`GetDefaultData()`**：當無存檔時被調用，子類別可覆寫此方法以自訂特殊的初始預設值（例如 Config 預設值）。
   - **`GetFileName(index)`**：格子存檔子類別可覆寫此方法，自訂您專案專屬的存檔檔名規則。

## 核心類別說明

* `SaveMapper`：反射資料拷貝工具。
* `SaveController<TSaveData>`：單一檔案的存檔控制器（例如 `config.save` 或 `global.save`）。
* `SlotSaveController<TSaveData>`：多存檔格子的控制器（例如 `save01.save`, `save02.save` 等）。

## 呼叫範例

### 1. 宣告控制器子類別
```csharp
[Serializable]
public class GameSlotSaveController : SlotSaveController<GameSave>
{
    public GameSlotSaveController() : base("save{0:00}.save", "auto.save", 30) { }
}
```

### 2. 在管理器中集中處理事件
```csharp
public class SaveManager : MonoBehaviour
{
    [SerializeField] GameSlotSaveController _gameSaveController = new();
    
    // 遊戲執行期狀態，無需繼承任何存檔框架介面
    [SerializeField] PlayerState _playerState = new();
    [SerializeField] DayState _dayState = new();

    void Start()
    {
        // 訂閱事件完成中央集中組裝
        _gameSaveController.OnPreSave += OnGameSave;
        _gameSaveController.OnPostLoad += OnGameLoad;
        
        // 載入 Slot 1
        _gameSaveController.LoadSlot(1);
    }

    void OnGameSave(GameSave data)
    {
        // 資料流方向極其透明，拷貝順序可完全自訂
        SaveMapper.Copy(_playerState, data);
        SaveMapper.Copy(_dayState, data);
        
        // 如果有欄位衝突，也可以在此手動處理與合併：
        // data.money = _playerState.Money + _bonusState.Money;
    }

    void OnGameLoad(GameSave data)
    {
        SaveMapper.Copy(data, _playerState);
        SaveMapper.Copy(data, _dayState);
    }
}
```
