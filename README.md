# Flavorful Story

## Об игре
- Платформа: PC, Nintendo Switch

- Жанр: Аркадный социальный симулятор, Cozy, Sym RPG

- Стиль: 3D, Low-poly, PS2

- Движок: Unity

---

░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░▄▄█▀▀█▄░░░░░░░░░░░░░
░░░░░░░░░░░░░░░░░░░░░░░░░░░░▄█▀░▄▄▄░▀█░░░░░░░░░░░░
░░░░░░░░▄██▄▄▄▄░░░░░░░░░░▄▄█▀░▄█▀░██░█▄░░░░░░░░░░░
░░░░░░░██████████▄▄▄▄▄▄▀▀▀▀░░░█▄▄░░█░░█░░░░░░░░░░░
░░░░░░░██░░░░▀██████░░░░░░░░░░░░▀▀▀░░▄█░░░░░░░░░░░
░░░░░░░▀█▄░░░░██████▄░░░░░░░░░░░░░░░░▀█░░░░░░░░░░░
░░░░░░░░██▄░░▄███████░░░░░░░░░░░░░░░░░▀█░░░░░░░░░░
░░░░░░░░▀███████████▀░░░░░░░░░░░░░░░░░░▀█░░░░░░░░░
░░░░░░░░░███████████░░░░░░░░░░░░░░░░░░░░█░░░░░░░░░
░░░░░░░░░░█████████░░░░░░░░░░░░░▄▀▀▀░░░░█░░░░░░░░░
░░░░░░░░▄█████████░░░░░░░░░░░░░░░░░░▄▄░░█░░░░░░░░░
░░░░░░░▄▀░░░░░▀██░░░█▀▀░░░░▄▄▄▄░░░░░▄░░░█░░░░░░░░░
░░░░░░▄░░░░░░░░░▀█░░░░░░░░░███░░░░░░░▄▄▄█░░░░░░░░░
░░░░░░█░░░░░░▄░▀▄░█░▀░░░░░░░░█▄░▄█░░░░░██▄░░░░░░░░
░░░░░░█░░█░▀░░░░░░█░▀▀░░░█▄▄▄▀▀▀░░░░░▄█░░░█▄░░░░░░
░░░░░░░░░░▀▄▄░░░▄█▀░░▀░░░░░░░░░░░░░▄██░░░▄██▄░░░░░
░░░░░░░░░░░░░▀▀▀▀▀█▄▄░░░░░░░░░▄▄▄████░░░░████▄░░░░
░░░░░░░░░░░░░░░░░░░▀█████████▀▀░░░░░▀█░░░▀████░░░░
░░░░░░░░░░░░░░░░░░░░░░▀▀▀▀██░░░░░░░░░░█░░░░▀██░░░░

---

# Руководство для программистов

Ниже представлены описания ключевых систем игры, необходимые для работы в проекте.

## SavingSystem

Система позволяет сохранять и восстанавливать:
- 📦 сценовые объекты (`MonoBehaviour`) через `SaveableEntity`
- ⚙️ Zenject-сервисы через `ISaveableService`
  
Состояния сохраняются в `.sav` файлы.

---

### 📄 Основные интерфейсы

#### `ISaveable`

```csharp
public interface ISaveable
{
    object CaptureState();
    
    void RestoreState(object state);
}
```

Реализуется каждым компонентом, который хочет сохранять и восстанавливать своё состояние.

---

#### `ISaveableService : ISaveable`

```csharp
public interface ISaveableService : ISaveable
{
    string UniqueIdentifier => GetType().FullName;
}
```

Реализуется любым Zenject-сервисом, который должен сохраняться.

---

### 🏗 Сценовые объекты (SaveableEntity)

#### Шаг 1. Добавь `SaveableEntity` на объект

```csharp
// GameObject
- SaveableEntity (проставит GUID автоматически)
- PlayerInventory (реализует ISaveable)
```

#### Шаг 2. Реализуй `ISaveable` в компоненте

```csharp
public class PlayerInventory : MonoBehaviour, ISaveable
{
    [SerializeField] private List<Item> _items;

    public object CaptureState() => _items.Select(i => i.Id).ToList();

    public void RestoreState(object state)
    {
        var itemIds = state as List<string>;
        _items = itemIds.Select(ItemDatabase.GetItemById).ToList();
    }
}
```

---

### ⚙️ Zenject-сервисы (ISaveableService)

#### Шаг 1. Реализуй интерфейс

```csharp
public class PlayerProgressService : ISaveableService
{
    private int _level;
    private int _xp;

    [Serializable]
    private struct State { public int Level, Xp; }

    public object CaptureState() => new State { Level = _level, Xp = _xp };

    public void RestoreState(object state)
    {
        if (state is State s)
        {
            _level = s.Level;
            _xp = s.Xp;
        }
    }
}
```

> ⚠️ `UniqueIdentifier` не нужно реализовывать — он по умолчанию возвращает `GetType().FullName`

---

#### Шаг 2. Зарегистрируй в Zenject через `SaveableServiceBinder<T>`

```csharp
public class GameplayInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.BindInterfacesAndSelfTo<PlayerProgressService>().AsSingle();
        Container.BindInterfacesTo<SaveableServiceBinder<PlayerProgressService>>().AsSingle();
    }
}
```

### 📁 Как работает

- Все `SaveableEntity` объекты собираются автоматически
- Все `ISaveableService`, зарегистрированные в `SaveRegistry`, также участвуют в сохранении
- Состояния сохраняются системой сохранения

---

### 🔁 Порядок вызова системы сохранения

Ниже представлено описание порядка вызовов системы сохранения во время загрузки и сохранения данных, с учётом `Unity Lifecycle` и `Zenject`.

---

#### 📦 Порядок при загрузке сохранения

1. `Awake()` (Unity)
- Вызывается у всех `MonoBehaviour` в новой сцене
- Zenject начинает `InstallBindings()` в `MonoInstaller`'ах

2. `InstallBindings()` (Zenject)
- Биндятся все `ISaveableService` и их зависимости
- Биндится `SaveableServiceBinder<T>`
- Вызов `SaveableServiceBinder.Initialize()` → `SaveRegistry.Register(...)`

✅ Теперь все Zenject-сервисы находятся в SaveRegistry

3. `SavingSystem.LoadLastSceneAsync(...)`
```csharp
var state = LoadFile("save1");
await SceneManager.LoadSceneAsync(...);
RestoreState(state);
```

4. `RestoreState(...)`
- Вызывается сразу после загрузки сцены
- Последовательно:
  - Все `SaveableEntity` → `RestoreState(state)`
  - Все `ISaveableService` из `SaveRegistry` → `RestoreState(state)`

> 💡 Всё это происходит ДО вызова `Start()`

5. `Start()` (Unity)
- Вызывается у всех `MonoBehaviour`
- Все данные уже восстановлены

6. `SavingSystem.OnLoadCompleted`
- Вызывается в самом конце
- Можно подписаться для пост-обработки:

```csharp
SavingSystem.OnLoadCompleted += () =>
{
    // Все данные восстановлены
};
```

---

#### 💾 Порядок при сохранении

```csharp
SavingSystem.Save("save1");
```

1. `GetAllSaveables()` собирает:
  - Все `SaveableEntity` в сцене
  - Все `ISaveableService` из `SaveRegistry`

2. Каждый объект вызывает `CaptureState()`

3. Состояние сериализуется и сохраняется в файл

---

#### 🧠 Резюме

| Этап                                  | Когда вызывается                           | Кто участвует                        |
|---------------------------------------|--------------------------------------------|--------------------------------------|
| `Awake()`                             | При загрузке сцены                         | Все `MonoBehaviour`                  |
| `InstallBindings()`                   | Сразу после `Awake()`                      | Zenject Installers                   |
| `SaveableServiceBinder.Initialize()`   | В момент создания Zenject-сервиса          | Все `ISaveableService`               |
| `SceneManager.LoadSceneAsync()`       | Из `LoadLastSceneAsync()`                  | Unity загружает сцену               |
| `RestoreState()`                      | Сразу после загрузки сцены                 | Все `ISaveable`, включая сервисы     |
| `Start()`                             | После `RestoreState()`                     | Все `MonoBehaviour`                  |
| `OnLoadCompleted`                     | В конце `RestoreState()`                   | Ваш код может на него подписаться    |
| `CaptureState()`                      | При `Save(...)`                            | Все `ISaveable`                      |

---

## Требования к программистам

- [Требования по структуре папок](https://docs.google.com/document/d/1tSARu2g-VNt6iJd2riVN6J3Bq1rkdgX5/edit?usp=sharing&ouid=105076722265519793362&rtpof=true&sd=true)

- Требования по структуре папки Scripts:

  Папка должна разделяться на подпапки по ```namespace```. Каждый namespace должен представлять собой отдельную систему или модуль. Например, папка ```Saving``` включает в себя все классы / интерфейсы, связанные с системой сохранения.

  Все классы / интерфейсы должны быть включены в соответствующий namespace.

  Правило именования namespace:

  ```ProjectName.Folder.[Subfolder].[FeatureName]```

  Например, ```namespace FlavorfulStory.Saving```

---