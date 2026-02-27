# Core Gameplay Loop

_Silver Valkyrie_ challenges players to take up arms as the legendary warrior herself, battling through mythic realms of ice, fire, and shadow. The gameplay loop is driven by **precision shooting**, **mode progression**, and **combo chaining**—rewarding aggressive play, shot accuracy, and timing.

> **Format**: Devil's Crush-style scrolling playfield (3 screens tall)  
> **Perspective**: Top-down with slight angle  
> **Ball = Sasha**: The silver ball represents the Valkyrie herself

---

## Primary Game Flow

### 1. Qualify Modes → Battle Enemies → Achieve Victory

**Mode Qualification:**

| Action | Result |
|--------|--------|
| Complete **T-R-O-L-L** drop targets | Light **Ice Trolls** mode |
| Hit **Ogre Head** bash target 3x | Light **Giant Ogre** mode |
| Hit **Pop Bumpers** 50x | Light **Draugr Horde** mode |
| Spell **B-E-R-S-E-R-K** | Light **Multiball** |
| Hit **Longhouse** 5x (no active modes) | Light **Feast Hurry-Up** |

**Mode Execution:** Each mode offers unique objectives and scoring opportunities:

- **Ice Trolls**: Navigate to the protected saucer twice to defeat the maneating pair
- **Giant Ogre**: Stun with bash target hits, then land the killing blow
- **Draugr Horde**: Pop bumper frenzy with escalating multipliers
- **Berserk Multiball**: 2-ball multiball with doubled scoring for 30 seconds
- **Feast Hurry-Up**: Rapid countdown bonus collection at the Longhouse

### 2. Build Toward Ultimate Challenge

**Progression Path:**

- Complete individual modes for victory points and bonuses
- Collect **S-L-A-Y** letters via top rollovers and inlanes for scoring multipliers
- Master all six major modes to unlock **Master Slayer Mode** (wizard mode)

**Master Slayer Mode:**

- 3-ball multiball featuring all previously defeated foes returning
- 60-second timer with extensions for completing objectives
- Massive point bonuses and exclusive Valhalla sequence
- The ultimate test—and the ultimate reward

### 3. Strategic Scoring Elements

**Multiplier System:**

- **SLAY** completion activates playfield-wide scoring multipliers (2x → 3x → 4x → 5x)
- **Orbit spinners** build combo chains and momentum
- **RUNE** rollover completion triggers **magnet effects** in pop bumper area

**Risk/Reward Decisions:**

- Pursue high-value shots guarded by challenging angles
- Balance mode completion versus multiplier collection
- Manage multiball effectively to maximize 30-second scoring window
- Choose when to go for the wizard mode qualifier vs. safe scoring

---

## Playfield Flow (Devil's Crush Style)

Unlike realistic pinball with a single static view, Silver Valkyrie's playfield **scrolls vertically** across three connected screens. The camera follows the ball, creating a larger play space with distinct zones.

```
┌─────────────────────────────┐
│      UPPER PLAYFIELD        │
│  ┌─────┐         ┌─────┐    │
│  │TROLL│ ORBITS  │SLAY │    │
│  │DROPS│    ↺    │ROLLS│    │
│  └──┬──┘         └─────┘    │
│     ↓                       │
│  [ICE TROLLS SAUCER]        │
├─────────────────────────────┤
│      MIDDLE PLAYFIELD       │
│         (●)(●)(●)           │  ← Pop Bumpers
│            ⬡                │  ← Magnet
│     ┌───────────┐           │
│     │OGRE HEAD  │ [FLIPPER] │  ← Upper Left Flipper
│     │(bash tgt) │           │
│     └───────────┘           │
│  [AXE DROPS]  [LONGHOUSE]   │
├─────────────────────────────┤
│      LOWER PLAYFIELD        │
│                             │
│    \  SLAY INLANES  /       │
│     \      ↓       /        │
│  [====FLIPPERS====]         │
│         ╲   ╱               │
│          ╲ ╱                │
│           ▽ DRAIN           │
└─────────────────────────────┘
```

### Zone Purposes

**Upper Playfield (Ice Realm)**
- TROLL drop targets gate access to Ice Trolls mode
- Orbit lanes build spinner combos
- SLAY top rollovers (L and A letters)
- RUNE rollovers for magnet activation

**Middle Playfield (Battle Zone)**
- Pop bumper triangle with central magnet
- Giant Ogre bash target (animated head)
- Longhouse scoop (mode starts, ball locks)
- AXE drop targets for combo building
- Upper left flipper for tactical control

**Lower Playfield (Home Base)**
- Main flipper pair
- Slingshots (sword and axe themed)
- SLAY inlanes (S and Y letters)
- Drain with ball save opportunities

---

## Player Incentive Structure

### Immediate Rewards (Every Hit Matters)

- **Drop target hits**: Satisfying visual/audio feedback, letter lights up
- **Bash target impacts**: Screen shake, enemy reaction animation
- **Spinner rips**: Rising pitch sound, combo counter builds
- **Bumper chaos**: Rapid-fire hits, score climbing, visual frenzy

### Medium-term Goals (Mode Mastery)

- **Mode completion**: Victory animation, bonus points, progression tracked
- **SLAY multipliers**: Whole-playfield scoring boost
- **Multiball survival**: Intense 30-second scoring window
- **Combo chains**: Orbit-to-orbit shots for escalating bonuses

### Long-term Achievement (The Journey)

- **Master Slayer Mode**: Ultimate challenge requiring all modes completed
- **High score pursuit**: Leaderboard competition
- **Complete narrative arc**: From individual battles to storming Valhalla
- **Exclusive wizard mode content**: See Sasha's ultimate victory

---

## Session Structure

### Game Start
1. Attract mode with animated playfield preview
2. Player presses start
3. Ball launches from plunger lane
4. "SASHA RIDES!" voice callout

### Standard Play
- 3 balls per game (standard pinball structure)
- Modes persist across balls (progress saved)
- Ball save active for first few seconds after launch
- Bonus countdown between balls

### Game End
1. Final ball drains
2. Bonus calculation and display
3. High score check
4. "VALHALLA AWAITS... NEXT TIME" (or victory if wizard mode completed)

---

## Difficulty Tuning

The game should feel **challenging but fair**:

- **Ball physics**: Slightly floatier than real pinball (more reaction time)
- **Drain frequency**: Enough to create tension, not frustration
- **Mode timers**: Tight but achievable
- **Wizard mode**: Genuinely difficult—a real accomplishment

### Accessibility Considerations

- Visual ball trail for tracking
- Audio cues for important events
- Colorblind-friendly UI options
- Adjustable ball speed (options menu)

---

## Design Philosophy

_Silver Valkyrie_ balances **accessible entry** with **deep mastery**. 

New players can:
- Enjoy the spectacle of hitting targets and watching enemies react
- Complete simple modes and feel progression
- Experience the full playfield and art

Experienced players pursue:
- Complex shot sequences for maximum combos
- Optimal multiplier timing before mode starts
- Wizard mode mastery and high score competition
- Perfect multiball management

Every mechanical element serves both immediate feedback and long-term progression. No dead moments. Always something to shoot for.
