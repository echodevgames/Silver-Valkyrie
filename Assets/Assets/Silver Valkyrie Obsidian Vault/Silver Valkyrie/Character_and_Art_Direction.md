# Character and Art Direction

## Visual Style Philosophy

_Silver Valkyrie_ combines **16-bit pixel art aesthetics** with the bold visual language of **Heavy Metal magazine** and **1980s Bally/Williams fantasy pinball**. The result is chunky, colorful sprites with unapologetic heroic proportions and maximum visual impact.

### Core Principles

- **Pixel Art Aesthetic**: Pre-rendered 3D models and hand-drawn sprites at low resolution
- **Bold Silhouettes**: Characters readable at small sizes, distinct shapes
- **Saturated Colors**: Primary and vivid complementary colors that pop on screen
- **Stylized Anatomy**: Heroic proportions—powerful, exaggerated, iconic
- **Limited Palette per Element**: Cohesive color groups for visual clarity
- **Animated Everything**: Enemies react, targets respond, the playfield lives

### Style References

**Pixel Art**:
- Donkey Kong Country (pre-rendered 3D → sprites)
- Baldur's Gate (painted sprites at low resolution)
- Shovel Knight (modern pixel art with classic sensibility)

**Fantasy Pinball**:
- Devil's Crush (animated creatures on playfield)
- Demon's Tilt (modern dark fantasy pinball aesthetic)

**Comic/Pulp Art**:
- Heavy Metal magazine
- 1980s Bally backglass art (*Fathom*, *Centaur*, *Flash Gordon*)

---

## Art Pipeline

### 3D to Pixel Workflow

1. **Model in Blender** - Existing 3D models from earlier development
2. **Pixel Art Render** - Use pixel art addon/shader to render at low resolution
3. **Touch Up in Aseprite** - Clean edges, adjust colors, add detail
4. **Animate** - Create sprite sheets for movement and reactions
5. **Export** - PNG sprite sheets for Unity import

### Hand-Drawn Elements

Some elements benefit from direct pixel art creation:
- UI elements and text
- Special effects (explosions, magic, impacts)
- Small decorative details
- Touch-ups on rendered sprites

### Commissioned Art Usage

Curt Sibling's professional character art serves as:
- **Reference** for pixel art sprite creation
- **Splash screens** for mode starts and victories
- **Title screen elements** (adapted/integrated)
- **Promotional material**

---

## Color Palette

### Primary Palette
- **Silver/Chrome**: Sasha's armor, the ball, UI highlights
- **Ice Blue**: Troll realm, cold environments, magic effects
- **Deep Blue**: Sasha's cape, backgrounds, shadows

### Accent Colors
- **Golden Yellow**: Sasha's hair, victory effects, treasure
- **Blood Red**: Damage, ogre realm, danger indicators
- **Bone White**: Draugr, skulls, ancient elements
- **Forest Green**: Troll skin, ogre elements

### Per-Enemy Palettes
- **Ice Trolls**: Blue-white skin, green hair/moss, yellow eyes
- **Giant Ogre**: Blue-grey skin, green accents, red blood
- **Draugr**: Bone white, rusted metal, ghostly blue glow

---

## Main Character: Sasha the Silver Valkyrie

### Character Concept

Sasha is a Valkyrie—a chooser of the slain—who has descended from Valhalla to battle the mythological horrors threatening the mortal realm. She is represented in gameplay by the **silver pinball** itself, with her full character appearing in animations, mode screens, and victory sequences.

### Visual Design (Established by Curt Sibling)

- **Armor**: Polished silver with blue accents, form-fitting but practical
- **Hair**: Golden blonde, flowing and dynamic
- **Cape**: Deep blue, adds motion and silhouette interest
- **Helmet**: Winged silver helm with golden crown piece
- **Weapons**: Sword, axe, bardiche (varies by enemy/mode)
- **Expression**: Confident, fierce, never uncertain
- **Proportions**: Classic pinball heroic style—powerful and iconic

### Weapon Loadouts (Per Mode)

| Enemy | Weapon | Reason |
|-------|--------|--------|
| Ice Trolls | Sword & Shield | Close combat defense |
| Giant Ogre | Bardiche | Reach for the big boy |
| Draugr Horde | Dual Swords | Crowd control |
| Berserk Mode | Dual Axes | Maximum carnage |
| Wizard Mode | All weapons | Ultimate warrior |

### Animation Needs (Sprites)

- Idle/ready stance
- Attack swing (multiple weapons)
- Victory pose
- Taking damage / block
- Charging/leaping
- Berserk rage state

---

## Enemy Designs

### Ice Trolls

**Concept**: A mated pair of maneating trolls, territorial and savage.

**Visual Design**:
- Blue-white skin with warts and texture
- Green mossy hair
- Yellow predatory eyes
- Jagged teeth, drooling
- Hunched, powerful builds

**Personality**: Predatory, taunting, hungry
- *"Tonight we dine on Viking Soup!"*
- *"Person-meat! My favorite!"*

**Animation Needs**:
- Idle lurking
- Taunting/roaring
- Taking damage
- Death sequence (one, then the other)

### Giant Ogre

**Concept**: A massive, brutish creature—emphasis on size contrast with Sasha.

**Visual Design** (Established by Curt Sibling):
- Blue-grey skin
- Green horns/spikes
- Massive head (bash target)
- Drooling, stupid expression
- Scarred and battle-worn

**Personality**: Brutish but overconfident
- *"I'll crush you like a walnut!"*
- *"My head... it's spinning..."*
- *"Noooo! My beautiful face!"*

**Animation Needs**:
- Head bobbing/idle
- Impact reaction (stunned)
- Rage state
- Decapitation defeat

### Draugr Horde

**Concept**: Undead Norse warriors rising endlessly from frozen earth.

**Visual Design**:
- Skeletal with remnants of flesh
- Rusted, broken armor and weapons
- Glowing blue eyes
- Varied poses and equipment
- Emphasis on *numbers*

**Personality**: Silent, relentless, terrifying in mass

**Animation Needs**:
- Rising from ground
- Shambling advance
- Attack swing
- Destruction/collapse

---

## Playfield Art Direction

### Background Layers

The scrolling playfield uses parallax background layers:

1. **Far Background**: Mountains, sky, aurora borealis
2. **Mid Background**: Norse architecture, standing stones, trees
3. **Playfield Surface**: The actual play area with targets and lanes
4. **Foreground Elements**: Occasional overlapping decorative pieces

### Zone Theming

| Zone | Theme | Colors |
|------|-------|--------|
| Lower (Flippers) | Battlefield entrance | Earth tones, blood red accents |
| Middle (Bumpers/Ogre) | Ogre's territory | Greens, browns, bone |
| Upper (Trolls) | Frozen troll lair | Ice blue, white, grey |
| Bonus Rooms | Varies by mode | Mode-specific palettes |
| Wizard Mode | Valhalla | Gold, white, divine light |

### Playfield Elements (Sprite Needs)

- Flippers (sword-shaped, animated swing)
- Pop bumpers (skull or rune themed)
- Drop targets (TROLL letters, ice/frost themed)
- Slingshots (weapon themed—sword and axe)
- Saucers (cave entrance for trolls)
- Spinners (blade/wind themed)
- Lanes and rails
- Insert lights (rune symbols)

---

## UI Art Direction

### Score Display

- Large, readable pixel font
- Runic styling on numbers
- Animated scoring popups for big hits

### Mode Indicators

- Icon-based mode status
- TROLL/BERSERK/SLAY letter displays
- Timer bars for active modes

### Title Screen

- **Artist**: [Girlfriend]
- **Elements**: Sasha heroic pose, Silver Valkyrie logo, Norse decorative framing
- **Vibe**: Metal album cover meets arcade attract screen

### Pause/Menu Screens

- Consistent runic/Norse decorative borders
- Pixel art buttons and selections
- Character art integration where appropriate

---

## Animation Priorities

### Must Have (Core Game Feel)

1. Ball (subtle trail/glow effect)
2. Flippers (smooth swing animation)
3. Pop bumpers (hit reaction)
4. Drop targets (falling animation)
5. Slingshots (hit flash)

### High Priority (Mode Experience)

1. Ice Trolls (full character animation)
2. Giant Ogre head (idle, hit, defeat)
3. Mode start splash screens
4. Victory animations

### Nice to Have (Polish)

1. Draugr rising/falling (wizard mode)
2. Background animations (aurora, flames)
3. Sasha celebration animations
4. Environmental destruction

---

## Audio-Visual Sync Notes

Visual effects should sync with audio for maximum impact:

- **Flipper hit**: Flash + thwack sound
- **Bumper hit**: Pulse + boing/thud
- **Drop target**: Fall animation + clunk
- **Mode start**: Screen flash + fanfare
- **Ogre hit**: Screen shake + grunt
- **Troll death**: Freeze frame + death cry
- **Wizard mode**: Everything goes maximum

---

## Reference Materials

### Commissioned Art (Curt Sibling)
- Sasha ready stance (sword + axe)
- Sasha victorious (bardiche + ogre head)
- Sasha charging (dual swords)

### Mood Boards
- [[Sasha_the_Valkyrie.canvas]]
- [[Enemies.canvas]]
- [[DMD_Sprite_References.canvas]]

### External References
- Devil's Crush gameplay footage
- Demon's Tilt art style
- Heavy Metal magazine covers
- 1980s Bally backglass art
