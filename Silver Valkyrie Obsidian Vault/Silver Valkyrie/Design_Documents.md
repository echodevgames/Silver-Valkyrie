# Silver Valkyrie - Design Documents

> **Project Type**: Devil's Crush-style video game pinball  
> **Engine**: Unity 2D  
> **Art Style**: Pixel art / pre-rendered sprites

These are the core design documents for **Silver Valkyrie**, a pixel art video pinball game featuring Norse mythology, Heavy Metal magazine aesthetics, and mode-based progression.

---

## Document Index

- [[Project_Overview]] - Team, tech specs, project status
- [[Core_Gameplay_Loop]] - Primary game flow and player incentives
- [[Objectives_and_Modes]] - Detailed mode breakdowns
- [[Playfield_Elements]] - Targets, bumpers, flippers, lanes
- [[Character_and_Art_Direction]] - Visual style, Sasha, enemies
- [[Master_Slayer_Mode]] - Wizard mode details
- [[Lighting_and_Effects]] - Visual feedback systems
- [[Sound_and_Music]] - Audio direction

---

## Design DNA

### NOT a Realistic Simulation

Silver Valkyrie is **not** trying to replicate a physical pinball table. It's a *video game* that uses pinball mechanics as its core gameplay—closer to Devil's Crush than Visual Pinball.

This means:
- **Scrolling playfield** spanning multiple screens vertically
- **Animated enemies** that react, attack, and die on the playfield itself
- **Boss encounters** integrated into the table
- **Bonus rooms** and alternate play areas
- **Fantasy physics** tuned for fun, not realism
- **No cabinet** - this is a screen-native experience

### Mechanical Inspiration: 1981-1986 Bally/Williams

The *feel* of the gameplay draws from classic solid-state pinball:

- **Inline drop targets** (like TROLL) - straight from *Centaur*
- **Heavy use of saucers, spinners, pop bumpers** - transitional era hallmarks
- **Tight but tactical shots** - every shot matters

### Structural Inspiration: 1993-1999 WPC "SuperPin" Era

The *progression* draws from modern pinball design:

- Multiple **named modes** tied to character actions (*Monster Bash*, *Medieval Madness*)
- **Objectives tied to drop targets, rollovers, and orbit shots**
- **Wizard mode** (MASTER SLAYER MODE) as ultimate goal
- Narrative layered onto fast, brutal gameplay

### Visual Inspiration: Heavy Metal Fantasy

The *aesthetic* draws from pulp fantasy and arcade art:

- **1977-1984 Bally fantasy era**: *Fathom*, *Xenon*, *Centaur*, *Flash Gordon*
- **Heavy Metal magazine**: Bold, unapologetic, heroic
- **16-bit pixel art**: Pre-rendered sprite style of *Donkey Kong Country*, *Baldur's Gate*

---

## TL;DR

**Silver Valkyrie = Devil's Crush format + Monster Bash depth + Heavy Metal attitude**

She's got the scrolling playfield of *Alien Crush*, the mode structure of *Monster Bash*, and the visual soul of a 1983 Bally backglass—rendered in chunky, glorious pixels.

> "It plays like the best of the '80s with the brains of the '90s—and the attitude of now."

---

## Playfield Structure (Devil's Crush Style)

Unlike a single-screen realistic table, Silver Valkyrie features a **vertically scrolling playfield** divided into distinct zones:

### Lower Playfield (Screen 1)
- Main flippers (2)
- Slingshots (sword and axe themed)
- Drain and ball save area
- SLAY inlanes (S and Y letters)

### Middle Playfield (Screen 2)
- Pop bumper complex (3 bumpers + central magnet)
- Giant Ogre bash target (animated head)
- Longhouse scoop
- AXE drop targets
- Secondary flipper (upper left)

### Upper Playfield (Screen 3)
- TROLL drop target bank
- Ice Trolls saucer (protected behind TROLL targets)
- Orbit lanes with spinners
- SLAY top rollovers (L and A letters)
- RUNE rollovers

### Bonus Rooms (Accessed via specific shots)
- Ice Trolls Lair (mode-specific arena)
- Ogre's Cave (bash target battle room)
- Valhalla (wizard mode exclusive)

---

## Core Design Pillars

1. **Accessible but Deep** - Easy to understand, difficult to master
2. **Always Something Happening** - No dead moments, constant feedback
3. **Earned Progression** - Wizard mode requires real skill and completion
4. **Visual Spectacle** - Every hit, mode, and victory should look incredible
5. **Respects the Player's Time** - Games are intense but not endless
