# Playfield Elements

This document details every interactive element on the Silver Valkyrie playfield, organized by zone. Unlike a physical pinball table, these elements exist in a **scrolling video game playfield** and can include animated enemies, reactive environments, and effects impossible on real hardware.

---

## Playfield Overview

```
┌────────────────────────────────────────┐
│           UPPER PLAYFIELD              │
│  ┌─────────┐              ┌─────────┐  │
│  │ T-R-O-L │   ORBIT      │ SLAY    │  │
│  │   -L    │   LANES      │ (L)(A)  │  │
│  │ (drops) │     ↺        │ ROLLS   │  │
│  └────┬────┘              └─────────┘  │
│       ↓                                │
│  [ICE TROLLS SAUCER]    [RUNE ROLLS]   │
│       ◉                   (R)(U)(N)(E) │
├────────────────────────────────────────┤
│           MIDDLE PLAYFIELD             │
│                                        │
│         (●)   (●)   (●)                │  ← Pop Bumpers
│              ⬡                         │  ← Magnet
│   ┌──────────────┐                     │
│   │  OGRE HEAD   │      [FLIPPER]      │  ← Upper Left
│   │  (bash tgt)  │         ╱           │
│   └──────────────┘                     │
│                                        │
│   [A-X-E DROPS]      [LONGHOUSE]       │
│                          ◉             │
├────────────────────────────────────────┤
│           LOWER PLAYFIELD              │
│                                        │
│    ╲ SLAY (S)              SLAY (Y) ╱  │  ← Inlanes
│     ╲   ╲                    ╱   ╱     │
│      ╲   ╲                  ╱   ╱      │
│       ╲   ╲                ╱   ╱       │
│    [SLING] [====FLIPPERS====] [SLING]  │
│             ╲            ╱             │
│              ╲   DRAIN  ╱              │
│               ╲        ╱               │
│                ╲______╱                │
└────────────────────────────────────────┘
```

---

## Drop Target Systems

### TROLL Drop Targets (Upper Playfield)

**Configuration**: 5 inline targets in a protective formation  
**Spelling**: T - R - O - L - L  
**Location**: Upper left, guarding Ice Trolls saucer  

**Function**: 
- Clearing all 5 targets opens access to the saucer behind them
- Completing the bank lights **Ice Trolls mode**
- Targets reset after mode completion

**Visual Design**:
- Ice/frost themed target faces
- Each letter carved in runic style
- Frozen debris effect when hit
- Satisfying "crack" and tumble animation

**Scoring**:
- Individual target: 10,000 points
- Bank completion: 50,000 points + mode light

---

### AXE Drop Targets (Middle Playfield)

**Configuration**: 3 inline targets  
**Spelling**: A - X - E  
**Location**: Center-left, beneath pop bumper area

**Function**:
- Increase combo value multiplier
- Contribute to BERSERK letter collection
- Feed ball toward spinner area when cleared

**Visual Design**:
- Battle axe motif on each target
- Metal clang and spark effect on hit
- Targets "chop" down when struck

**Scoring**:
- Individual target: 5,000 points
- Bank completion: 25,000 points + combo multiplier boost

---

## Pop Bumper Complex (Middle Playfield)

### Pop Bumpers (×3)

**Configuration**: Triangle formation  
**Location**: Upper-middle playfield  

**Function**:
- Cumulative hits (50) light **Draugr Horde mode**
- During Draugr mode, bumpers have 5x scoring
- Each hit contributes to RUNE activation

**Visual Design**:
- Skull-topped bumpers
- Runic symbols on the caps
- Flash and pulse on contact
- During Draugr mode: ghostly blue glow

**Scoring**:
- Standard hit: 1,000 points
- During Draugr mode: 5,000 points
- RUNE active: 2,000 points

---

### Central Magnet System

**Location**: Center of pop bumper triangle  
**Trigger**: Complete R-U-N-E rollovers

**Behavior**:
- 1 RUNE letter: Magnet pulses slowly (subtle pull)
- 2-3 letters: Pulses faster (noticeable influence)
- All 4 letters: Rapid pulse, brief strong activation
- Resets after activation window expires

**Visual Design**:
- Glowing runic symbol in center
- Pulse intensity matches activation level
- Magical particle effects during full activation

**Thematic Purpose**: Represents supernatural forces—Sasha channeling ancient magic to control the battlefield.

---

## Bash Target System

### Giant Ogre Head (Middle Playfield)

**Type**: Animated bash target  
**Location**: Middle-right playfield

**Visual Design**:
- Large ogre head (takes up significant screen space)
- Animated idle: drooling, eyes moving, breathing
- Blue-grey skin, green horns
- Based on Curt Sibling art

**Behavior**:
- Bobs and shakes on impact
- 3 quick hits = **Giant Ogre mode** starts
- During mode: takes damage, expressions change
- Defeat animation: eyes roll back, tongue out, collapse

**Mode Integration**:
- Qualification: 3 hits in quick succession
- During mode: Continue hitting to stun, then "mouth shot" to kill
- Victory: Decapitation animation (per the commissioned art)

**Scoring**:
- Standard hit: 25,000 points
- During mode (stun phase): 50,000 points
- Killing blow: 500,000 points

---

## Spinner Systems

### Orbit Spinners (×2)

**Location**: Left and right orbit entrances  
**Type**: Blade-style spinners

**Function**:
- Build combo chains and momentum
- Enhanced value during Berserk mode (2x)
- Contribute to orbit completion bonuses

**Visual Design**:
- Sword blade appearance
- "Whoosh" blur effect during spin
- Spark trail at high speeds

**Scoring**:
- Per spin: 500 points
- Combo multiplier applies
- Berserk mode: 1,000 per spin

---

## Saucer and Scoop Systems

### Ice Trolls Saucer (Upper Playfield)

**Location**: Behind TROLL drop target bank  
**Access**: Only available after clearing all TROLL targets

**Function**:
- Primary objective during Ice Trolls mode
- First hit: Wound one troll (animation + callout)
- Second hit: Slay the second troll (victory)

**Visual Design**:
- Cave entrance appearance
- Ice formations around the opening
- Troll eyes visible in darkness (during mode)
- Blood splatter effect on hits

**Mode Behavior**:
- Hit 1: *"ARRRGH! You'll pay for that!"* - wounded troll animation
- Hit 2: *"NOOO! My mate!"* - death animation, mode victory

---

### Longhouse Scoop (Middle Playfield)

**Location**: Center-right  
**Type**: Ball capture scoop with eject

**Functions**:
- Mode start location (shoot to begin qualified modes)
- Ball lock for Berserk Multiball
- Feast Hurry-Up trigger (5 hits, no active mode)
- Jackpot collection point

**Visual Design**:
- Norse longhouse entrance
- Wooden beam construction
- Firelight glow from within
- Smoke particles

**Thematic Purpose**: Sasha's mead hall—the center of Viking warrior culture and her base of operations.

---

## Flipper Configuration

### Main Flippers (Lower Playfield)

**Quantity**: 2 (standard left/right pair)  
**Coverage**: Full lower playfield, most shots accessible

**Visual Design**:
- Sword-shaped flipper bats
- Silver with blue inlay
- Swing animation with motion blur
- Impact flash on ball contact

### Upper Left Flipper (Middle Playfield)

**Quantity**: 1  
**Location**: Left side of middle playfield  
**Coverage**: Upper-middle and left targets, surgical TROLL shots

**Strategic Value**:
- Essential for consistent Ice Trolls saucer access
- Allows skilled players to control upper playfield
- Risk/reward positioning (ball can drain from here)

---

## Lane Systems

### SLAY Letter Collection

**Distribution**:

| Letter | Location | How to Collect |
|--------|----------|----------------|
| S | Left inlane | Ball rolls through |
| L | Left top rollover | Orbit shot |
| A | Right top rollover | Orbit shot |
| Y | Right inlane | Ball rolls through |

**Function**: Complete S-L-A-Y to activate scoring multipliers

**Multiplier Progression**:
- 1st completion: 2x scoring
- 2nd completion: 3x scoring  
- 3rd completion: 4x scoring
- 4th completion: 5x scoring (maximum)

**Visual Feedback**:
- Letters light up in sequence
- Completion triggers golden flash across playfield
- Multiplier indicator prominently displayed

---

### RUNE Rollovers (Upper Playfield)

**Configuration**: 4 flat rollover switches  
**Spelling**: R - U - N - E  
**Location**: Upper playfield, near orbit returns

**Function**: Progressive magnet system activation

**Behavior**:
- Lane change (flipper buttons swap which rollover is lit)
- Completing all 4 activates central magnet
- Brief activation window, then reset

---

### Orbit Lanes

**Configuration**: Left and right full orbits  
**Features**:
- Spinners at entry points
- One-way gates at top (prevent backflow)
- SLAY letter rollovers (L and A) along path
- Feed to opposite flipper or upper playfield

**Visual Design**:
- Carved stone/metal lane walls
- Runic inscriptions along the path
- Speed lines during fast orbit shots

---

## Slingshot System

### Weapon-Themed Slingshots

**Configuration**: 2 slingshots (left and right)  
**Location**: Above main flippers, standard positions

**Visual Design**:
- Left: Sword motif
- Right: Axe motif
- Metal clang sound on activation
- Spark/clash visual effect

**Function**: Standard slingshot behavior—fast rebounds, controlled chaos

---

## Ball Lock System (Berserk Multiball)

### Virtual Lock Mechanism

**Type**: Letter collection (no physical ball trap)  
**Collection**: Spell B-E-R-S-E-R-K across various targets

**Letter Locations**:

| Letter | Target |
|--------|--------|
| B | Left orbit |
| E | AXE targets |
| R | Pop bumpers (10 hits) |
| S | SLAY inlane (either) |
| E | Right orbit |
| R | RUNE completion |
| K | Longhouse scoop |

**Multiball Start**:
1. All 7 letters collected
2. Shoot Longhouse scoop
3. Ball captured briefly
4. Second ball auto-launches
5. *"SASHA GOES BERSERK!"*
6. 30-second 2x scoring window

**Design Rationale**: Virtual locks maintain fast gameplay flow without extended ball holds. Inspired by *Attack from Mars* and *Deadpool* multiball systems.

---

## Gate System

### Orbit Gates (×2)

**Location**: Top of each orbit lane  
**Type**: One-way hinged gates

**Function**:
- Allow forward orbit completion
- Prevent ball backflow
- Ensure clean orbit-to-flipper feeds

**Visual Design**:
- Minimal visual presence (gameplay element, not featured)
- Subtle "click" sound when passed

---

## Environmental Elements (Video Pinball Exclusive)

These elements are only possible because this is a video game, not a physical table:

### Animated Enemy Presence

- **Ice Trolls**: Appear behind their targets during mode, animate and react
- **Giant Ogre**: Full animated head, not just a static target
- **Draugr**: Rise from the playfield background during wizard mode

### Dynamic Playfield Changes

- Ice formations grow/spread during Ice Trolls mode
- Blood pools appear after Ogre defeat
- Ghostly blue fog during Draugr Horde
- Golden light during wizard mode

### Background Animation

- Aurora borealis in the sky
- Distant battles and fires
- Weather effects (snow in Ice Trolls mode)

---

## Element Priority (For Development)

### Phase 1: Core Playable
1. Ball physics
2. Main flippers
3. Basic walls and lanes
4. Pop bumpers
5. One drop target bank (TROLL)

### Phase 2: Full Playfield
6. All drop targets (AXE)
7. Slingshots
8. Spinners
9. Saucers and scoops
10. Upper flipper
11. Orbit lanes with gates

### Phase 3: Polish
12. Ogre bash target (animated)
13. Magnet system
14. All visual effects
15. Environmental animations

---

## Technical Notes for Jesse

**Physics Considerations**:
- Ball should feel slightly floatier than real pinball (more forgiving)
- Flipper response must be instant (no input lag)
- Bumper knockback should be consistent
- Spinner momentum should feel satisfying

**Collision Priorities**:
- Flippers > Ball > Bumpers > Targets > Walls
- No ball clipping through elements
- Consistent bounce angles

**Animation Sync**:
- Target hits should trigger immediate visual feedback
- Sound and visuals must be synchronized
- No perceived delay between input and response
