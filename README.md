# Match-3 Game - Technical Documentation

## 📋 Project Overview
A Unity-based Match-3 puzzle game with multiple game modes and enhanced gameplay mechanics. Built with C# and DOTween for smooth animations.

---

## 🎯 Development Tasks

### Task 1: Visual Redesign (Re-skin)
**Objective:** Update game visual assets
- Replace sprites in Item prefabs with new artwork
- Maintain consistent sprite dimensions and pivot points
- Update sprite references in all related prefabs
- Adjust **Pixels Per Unit** (PPU) in sprite import settings for


**Files Modified:**
- `Assets/Resources/Prefabs/Items/*`

---

### Task 2: Core Gameplay Implementation

#### 2.1 Item Generation System
**Location:** `Board.cs` → `Fill()` method

**Implementation Details:**
- Generate items in sets of 3 for each type to ensure solvability
- Store all items in a list before placement
- Shuffle the list using Fisher-Yates algorithm
- Fill board cells sequentially from the shuffled list

**Algorithm:**
```csharp
1. Create list of items (each type x 3)
2. Shuffle list randomly
3. Assign items to board cells in order
```

**Benefits:**
- Guarantees minimum matchable combinations
- Prevents unsolvable board states
- Maintains game balance

---

#### 2.2 Bottom Cell Collection System
**Location:** `CellCollectedController.cs`

**Event Flow:**
1. Player selects item on board → Event fired via `EventManager`
2. `CellCollectedController` receives event
3. `HandleItemCollected()` checks for empty cells
4. Smart placement prioritizes grouping items of same type
5. `CheckMatch()` validates for sets of 3
6. Auto-remove matched sets and compress remaining cells

**Special Case - Time Attack Mode:**
- When item removed from cell → `FixCells()` called
- Remaining cells shift to fill gap
- Maintains clean collection area

**Key Methods:**
- `HandleItemCollected()` - Main collection logic
- `CheckMatch()` - Match detection and removal
- `FixCells()` - Cell compression after removal

---

#### 2.3 Win/Lose Condition System
**Location:** Multiple level classes

**Implementation:**
1. **UI Setup**
   - Create Win Panel prefab with congratulations message
   - Create Lose Panel prefab with retry button
   - Add corresponding UI scripts: `WinPanel.cs`, `LosePanel.cs`
   - Register panels with `UIMainManager`

2. **Logic Refactoring**
   - Refactor win/lose logic across 4 game modes:
  - `LevelMoves.cs`
  - `LevelTime.cs`
  - `LevelAutoWin.cs`
  - `LevelAutoLose.cs`
- Add `GAME_WIN` state to `GameManager.eStateGame` enum
- Fire `OnConditionComplete()` event when win/lose conditions met
- Update UI panel display based on game state

**State Machine:**
```
GAME_STARTED → [Condition Met] → OnConditionComplete() → GAME_WIN / GAME_OVER
```

---

#### 2.4 Auto-Win Logic
**Location:** `LevelAutoWin.cs`

**Algorithm:**
1. Linear scan through all cells in board matrix
2. Store `name` of first item found
3. Search for 2 additional matching items
4. Execute match sequence
5. Repeat until board is cleared
6. Use Coroutine with delays between steps for visual clarity

**Implementation Pattern:**
```csharp
IEnumerator AutoWinSequence()
{
    foreach (cell in board)
    {
        Find 3 matching items
        Execute match
        yield return delay
    }
}
```

---

#### 2.5 Auto-Lose Logic
**Location:** `LevelAutoLose.cs`

**Algorithm:**
1. Linear scan through all board cells
2. Maintain `List<string>` of previously seen item names
3. Place items in bottom cells
4. Only add new item types (not in list) to ensure fastest loss
5. Use Coroutine with delays for step visualization

**Strategy:** Maximize item type diversity in bottom cells to prevent matches

---

### Task 3: Gameplay Polish & Enhancement

#### 3.1 Animation Improvements
**Location:** `Item.cs` - DOTween configurations

**Tuning Parameters:**
- **Movement Duration:** Reduce to 0.15-0.2s for snappier response
- **Easing Curves:** Use `Ease.OutBack` for satisfying bounces
- **Scale Animations:** 
  - Appear: Scale from 0.1 to 1.0 in 0.15s
  - Explode: Scale to 0.1 in 0.1s with rotation
  - Hint: Punch scale by 0.1x every 0.1s (looped)
- **Delay Timings:** Keep delays < 0.3s to maintain pace

---

#### 3.2 Time Attack Mode Enhancements
**Location:** `GameSettings.asset`, `Item.cs`, `LevelTime.cs`

**Changes Implemented:**

1. **Extended Play Time**
   - Update `GameSettings.asset` → time = 60 seconds

2. **Item Return Functionality**
   - Add `originCell` field to `Item` class
   - Store cell reference on item spawn
   - Enable items to return to original position

3. **Revised Lose Condition**
   - Previous: Game ends when bottom cells full
   - New: Game ends only when timer reaches 0
   - Remove full-cell loss condition from `LevelTime.cs`

4. **Selection State Tracking**
   - Add `IsSelected` boolean to `Item` class
   - First click: `IsSelected = true` → Move to bottom cell
   - Second click (if selected): Return to `originCell`
   - Enables strategic item management

**Gameplay Flow:**
```
Item Click → Check IsSelected
├─ false → Move to bottom, IsSelected = true
└─ true  → Return to originCell, IsSelected = false
```

---

## 📁 Project Structure

### Core Classes
- **`Board.cs`** - Board generation and management
- **`Item.cs`** - Base item class with animations
- **`NormalItem.cs`** - Standard match-3 items
- **`Cell.cs`** - Individual cell logic
- **`GameManager.cs`** - Game state management
- **`BoardController.cs`** - User input and board interactions
- **`CellCollectedController.cs`** - Bottom collection area logic

### Level Modes
- **`LevelMoves.cs`** - Limited moves mode
- **`LevelTime.cs`** - Time attack mode
- **`LevelAutoWin.cs`** - Auto-play victory demonstration
- **`LevelAutoLose.cs`** - Auto-play defeat demonstration
