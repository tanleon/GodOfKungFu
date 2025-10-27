# God of Kung Fu - Technical Design Document

[Download the Project Document](https://raw.githubusercontent.com/tanleon/GodOfKungFu/main/project_%20GodOfKungFu.pdf)

## Table of Contents
- [Game Introduction](#game-introduction)
- [Tools and Technologies](#tools-and-technologies)
- [Game Development](#game-development)
- [Game Assets](#game-assets)
- [Level Design](#level-design)
- [Appendix](#appendix)

## Game Introduction

**Title:** God of Kung Fu  
**Genre:** 2D Action-Platformer / Martial Arts Adventure  
**Inspiration:** Classic martial arts beat 'em ups, Metroidvania-style progression, and narrative-driven combat games

### Premise
In *God of Kung Fu*, you play as a young martial arts disciple whose master, Guang, falls victim to the Shadow Lord, a malevolent force corrupting the greatest warriors of the land. Before dying, Master Guang reveals that his three siblings have been enslaved by the Shadow Lord's dark influence.

Your journey takes you across treacherous landscapes where you must battle corrupted warriors, free them from the Shadow Lord's grip, and inherit their techniques. Only by mastering their skills can you enter the Cave of Reflection and confront your own shadow.

### Gameplay Features
- **Combat System:** Fluid kung fu strikes, combos, and special moves with animation-driven feedback
- **Movement & Platforming:** Precision jumps, wall interactions, and skill-based mobility
- **Progression & Abilities:** Metroidvania-style ability unlocks and energy management
- **Narrative & Dialogue:** Story-driven quests with immersive dialogue system
- **UI & Feedback:** Minimalist HUD with clear visual feedback

## Tools and Technologies

### Development Stack
- **Game Engine:** Unity (2021+)
- **Scripting Language:** C#
- **Input System:** Unity Input System
- **Text Rendering:** TextMeshPro
- **Graphics Pipeline:** Universal Render Pipeline (URP)

### Development Tools
- **Art Generation:** Google ImageFX (dialogue portraits)
- **Code Editor:** Visual Studio Code
- **Version Control:** Git

## Game Development

### Core Scripts Overview

#### Character Control
- `PlayerController.cs` - Main player movement, input, and ability logic
- `TouchingDirections.cs` - Contact detection for ground, walls, and ceilings
- `Damageable.cs` - Health management system for all entities
- `Attack.cs` - Combat and hit detection system

#### AI Systems
- `FlyingEye.cs`, `Knight.cs` - Basic enemy behaviors
- `HuoFeng.cs`, `ShuiLian.cs`, `LeiShan.cs` - Boss-specific AI
- `DetectionZone.cs` - AI trigger and awareness system

#### UI Management
- `UIManager.cs` - Central UI controller
- `HealthBar.cs`, `BossHealthBar.cs` - Health display systems
- `PauseManager.cs` - Game state management
- `MainMenuUI.cs` - Menu navigation

#### Dialogue System
- `DialogueManager.cs` - Dialogue flow control
- `DialogueTrigger.cs` - Interaction-based dialogue initiation
- `Dialogue.cs` - Data structure for dialogue content

### Key Implementation Methods

#### Player Movement & Abilities
```csharp
// Example ability unlock system
void UnlockAbilitiesBasedOnScene()
{
    string sceneName = SceneManager.GetActiveScene().name;
    // Parse scene name to determine available abilities
}
```

#### Combat System
- Event-driven health updates
- Invincibility frames for damage prevention
- Animation-triggered attack sequences

#### Optimization Strategies
- Event-driven architecture reduces polling
- Component-based design for modularity
- Object pooling for frequent instantiations

## Game Assets

### Graphical Assets
- **Player Sprite:** RVROS Adventurer
- **Enemies:** Monsters and Creatures Fantasy, Martial Hero packs
- **Environment:** Forest, Dark Forest, Jungle, and Cave tilesets
- **UI Elements:** RPG Icon Pack, custom health bars and menus

### Audio Assets
- **Background Music:** 8-bit style battle themes
- **Sound Effects:** RPG Essentials SFX pack

### UI System
- **Main Menu:** Start game, level selection, exit options
- **Pause Menu:** Resume, tutorials, return to main menu
- **In-Game UI:** Health bars, ability indicators, dialogue boxes
- **Feedback:** Damage popups, tutorial prompts

## Level Design

### Level 1: Forest Realm
- **Boss:** Huo Feng (Master of Speed)
- **Player Skills:** Basic attacks only
- **Enemies:** Knights, Flying Eyes
- **Mechanics:** Introduction to combat and movement

### Level 2: Dark Forest
- **Boss:** Shui Lian (Master of Soaring Leap)
- **Player Skills:** Speed Boost unlocked
- **New Mechanics:** Moving obstacles (slimes)
- **Challenge:** Platforming with environmental hazards

### Level 3: Jungle Temple
- **Boss:** Lei Shan (Master of Chi)
- **Player Skills:** High Jump unlocked
- **New Mechanics:** Collapsing platforms and traps
- **Challenge:** Precision platforming with new abilities

### Level 4: Cave of Reflection
- **Boss:** Dark Self (Player's Shadow)
- **Player Skills:** All abilities available
- **Mechanics:** Mirror match with all learned techniques
- **Final Challenge:** Mastery of all combat skills

## Appendix

### Core Story Dialogue
The game features an extensive dialogue system with:
- Character-specific portraits and speaking styles
- Tutorial integration within narrative context
- Branching dialogue options for player engagement
- Lore delivery through character interactions

### Technical Specifications
- **Target Resolution:** 1920x1080
- **Input Methods:** Keyboard and Gamepad support
- **Performance Targets:** 60 FPS on mid-range systems
- **Build Platforms:** Windows, MacOS, Linux


---

*God of Kung Fu* combines traditional martial arts themes with modern game development practices, creating an engaging 2D action-platformer that emphasizes skill mastery and narrative depth.
