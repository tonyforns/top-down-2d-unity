# Top-Down 2D (Unity)

Proyecto Unity 2D top-down con sistemas modulares: diálogos, interacción, inventario, crafteo y puzzles.

---

## Estructura de scripts (`Assets/Scripts`)

| Carpeta        | Descripción                          |
|----------------|--------------------------------------|
| `Interfaces/`  | Contratos (IInteractable, IPickUp, etc.) |
| `Dialogue/`    | Sistema de diálogos                  |
| `Interaction/` | Detección de objetos interactuables y prompt en mundo |
| `Inventory/`   | Inventario con diccionario y pickups |
| `Crafting/`    | Recetas y crafteo                    |
| `Puzzle/`      | Puzzles con estados (palancas, etc.) |
| `Utilitys/`    | Singleton y utilidades                |

---

## Interfaces (`Interfaces/`)

- **IInteractable**: Objetos con los que el jugador interactúa (tecla E).
  - `void Interact()`
  - `string GetPromptText()` (por defecto `"Interactuar"`)
  - `int GetInteractionPriority()` (por defecto `0`; mayor = más prioridad cuando hay varios en rango)
- **IPickUp**: Contrato para recoger ítems (método `PickUp()`).
- **IDamageable**, **IDragable**, **IWeapon**: Para combate, arrastre y armas.

---

## Sistema de interacción (`Interaction/`)

Objetos interactuables en la escena: el jugador detecta los que están en rango y al pulsar E se ejecuta `Interact()` en el elegido (prioridad, luego distancia).

### Componentes

| Script                 | Uso |
|------------------------|-----|
| **InteractionDetector** | En el **jugador**. Requiere `Collider2D` con **isTrigger = true** (ej. CircleCollider2D). Mantiene una lista de `IInteractable` en rango; al pulsar la tecla (por defecto E) llama `Interact()` al objetivo actual. Expone `CurrentTarget` y evento `OnTargetChanged`. |
| **InteractablePromptUI** | Muestra el texto del prompt (ej. "E - Hablar") en **World Space** encima del objeto interactuable. Debe estar en un **Canvas con Render Mode = World Space**. Referencias: `InteractionDetector`, `TMP_Text`, opcional `panel`. Campo `worldOffset` (ej. `(0, 1.2, 0)`) para la altura sobre el objeto. |
| **InteractableBridge** | Para NPCs/objetos que ya usan **DialogueTrigger**: añade este componente al mismo GameObject para que la interacción sea con E (IInteractable) y al interactuar se llame `StartDialogue()`. |

### Configuración rápida

1. Jugador: añadir **InteractionDetector** + un **Collider2D** isTrigger (radio de interacción).
2. Objeto interactuable: implementar **IInteractable** en su script y tener **Collider2D** para que el detector lo registre.
3. Prompt en mundo: Canvas World Space con **InteractablePromptUI** (referencia al detector del jugador y al texto).

---

## Sistema de diálogos (`Dialogue/`)

Diálogos con líneas, retratos y animación de escritura (typewriter).

### ScriptableObjects y datos

- **DialogueData**: ScriptableObject con la secuencia de líneas.
- **DialogueLine**: Una línea (texto, nombre, retrato, velocidad de escritura).

### Componentes

- **DialogueManager** (Singleton): Orquesta el flujo; inicia diálogo, avanza con tecla (por defecto Space) o clic.
- **DialogueUI**: Muestra el texto (TMP), retrato y nombre; animación typewriter; eventos al completar línea.
- **DialogueTrigger**: En un GameObject con Collider2D; al entrar el jugador (tag "Player") inicia el diálogo. Opción "Use Once". También se puede llamar `StartDialogue()` desde **InteractableBridge** para activar con E.

---

## Sistema de inventario (`Inventory/`)

Inventario con slots fijos respaldado por un **Dictionary** (solo se guardan slots no vacíos).

### Datos

- **ItemData** (ScriptableObject): Define un tipo de ítem (Create → Scriptable Objects → Item Data).  
  Campos: `itemId`, `displayName`, `description`, `icon`, `isStackable`, `maxStackSize`.
- **ItemStack**: Struct en runtime: `ItemData` + `Amount`.

### Lógica

- **Inventory** (Singleton): Contenedor con `slotCount` slots.
  - Almacenamiento: `Dictionary<int, ItemStack>` (índice de slot → stack).
  - API: `GetSlot(index)`, `AddItem(data, amount)`, `RemoveItem(data, amount)`, `RemoveAt(slotIndex, amount)`, `SwapSlots(a, b)`, `HasItem(data, amount)`, `GetTotalCount(data)`, `CountSpaceFor(data, amount)` (para comprobar espacio sin modificar).
  - Evento: `OnInventoryChanged` para refrescar la UI.

### UI

- **InventorySlotUI**: Un slot (Image para icono, TMP_Text para cantidad). Método `Set(ItemStack)`.
- **InventoryUI**: Referencias a **Inventory** y array de **InventorySlotUI**; se suscribe a `OnInventoryChanged` y refresca todos los slots.

### Objetos recogibles

- **PickupItem**: Implementa **IInteractable**. En el Inspector: **ItemData**, **amount**, **promptText** (ej. "Recoger"), y si se destruye o desactiva al recoger. Requiere **Collider2D**. Al interactuar (E) añade el ítem al inventario.

---

## Sistema de crafteo (`Crafting/`)

Recetas que consumen ingredientes del inventario y producen un resultado.

### Datos

- **RecipeIngredient**: Struct: `ItemData` + `Amount` (para ingredientes y resultado).
- **CraftingRecipe** (ScriptableObject): Create → Scriptable Objects → Crafting Recipe.  
  Campos: `recipeName`, `description`, array **ingredients** (RecipeIngredient), **result** (RecipeIngredient).

### Lógica

- **CraftingSystem** (MonoBehaviour): Lista de **recetas** en el Inspector.
  - `CanCraft(recipe)`: Comprueba que el inventario tenga todos los ingredientes y espacio para el resultado (usa `Inventory.CountSpaceFor`; no modifica el inventario).
  - `Craft(recipe)`: Consume ingredientes y añade el resultado; dispara evento `OnCrafted`.
  - Pensado para una “mesa de crafteo” o sistema global con sus propias recetas.

### UI

- **CraftingRecipeEntryUI**: Fila de receta: nombre, icono y cantidad del resultado, botón **Craft** (solo activo si `CanCraft`). Opcional: iconos/cantidades de ingredientes. `Set(recipe, system)` y `RefreshButton()`.
- **CraftingUI**: Referencias a **CraftingSystem** y array de **CraftingRecipeEntryUI**; opcionalmente **Inventory**. En Start asigna cada receta a cada entrada; se suscribe a **OnInventoryChanged** para actualizar el estado del botón Craft.

### Integración con inventario

- En **Inventory** se añadió `CountSpaceFor(ItemData, amount)` para que el crafteo pueda comprobar espacio sin añadir/quitar ítems ni disparar `OnInventoryChanged` al solo verificar.

---

## Sistema de puzzles (`Puzzle/`)

- **PuzzleItem**: Implementa **IInteractable**; tiene estado correcto/actual; al interactuar alterna estado y dispara `OnStateChange`. `IsInCorrectState()`.
- **PuzzleItemLever**: Hereda de PuzzleItem; voltea el sprite de la palanca al interactuar.
- **Puzzle**: Agrupa varios **PuzzleItem**; se suscribe a `OnStateChange` de cada uno; cuando todos están en estado correcto dispara **OnPuzzleSolved**.
- **PuzzleDoor**: Se puede enlazar al evento para abrir/cerrar al resolver el puzzle.

---

## Sonido y utilidades

- **SoundModelSO**: ScriptableObject con nombre de sonido y `AudioClip`.
- **SoundSystem**: Reproduce sonidos (referencias a clips o SO).
- **Singleton&lt;T&gt;** (`Utilitys/Singleton.cs`): Patrón singleton para MonoBehaviours (ej. DialogueManager, Inventory).

---

## Resumen de flujos

1. **Interactuar**: Jugador con **InteractionDetector** + trigger → objetos con **IInteractable** + Collider2D → al pulsar E se llama `Interact()`. **InteractablePromptUI** (World Space) muestra "E - [GetPromptText()]" encima del objeto.
2. **Recoger ítem**: Objeto con **PickupItem** (IInteractable) → al interactuar se añade a **Inventory** y el objeto se destruye o desactiva.
3. **Crafteo**: **CraftingSystem** con recetas; **CraftingUI** muestra entradas y botón Craft; al pulsar se comprueba `CanCraft` y se llama `Craft()` (consume ingredientes y añade resultado).

---

