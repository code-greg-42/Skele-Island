# Skele Island!

**Skele Island** is a simple and fun third-person shooter where the goal is to survive waves of skeletons without falling off the edge!

## Play Now!
Experience **Skele Island** on Unity Play:

[Play Skele Island on Unity Play](https://play.unity.com/mg/other/builds-f9-14)

*play in full screen for best experience (default resolution 1920x1080)*

## Features
- **Third-Person Aiming and Movement**: The player character moves with standard third-person combat functionality, and is able to strafe, roll, and jump.
- **AI Enemies**: The enemies are able to correctly move around obstacles and follow the player. They attack when in range.
- **Player Abilities**: The player is able to use two abilities. A simple damage buff, and an ability that draws all enemies in range towards a tornado that the player places on the ground. Each ability requires charges to be used, and additional charges can be obtained by collecting pickups off of dead enemies.
- **Boss Wave**: A much stronger enemy spawns after defeating the standard waves. The boss has an enlarged size, range, health, and does not get knocked back when struck with an attack by the player.
- **Pickups**: Some enemies drop pickup items. These pickups can increase the player's charges of abilities, heal the player to full health, or increase the player's overall damage and speed. The pickup drop rate is different on each difficulty.
- **UI Components**: Player health, boss health, and a player castbar (for casting main attacks) are all featured on the UI in this game. They are updated in accordance to the events in the game in a smooth manner.

## How to Play
- **Movement**: WASD to move forward, left, backwards, and right, respectively. Shift makes the player roll, and space makes the player jump. Use WASD when hitting shift to aim the roll.
- **Attacking**: Hold left-click to charge up a fireball. A castbar will appear at the bottom of the screen. The castbar will turn green after 0.5 seconds, letting you know that the fireball is ready to be released. Releasing left-click will cast the fireball in the direction of the crosshair. If you hold down left-click longer than 0.5 seconds, a larger fireball will be cast. The castbar will continue to grow to represent that. Once the castbar has filled, the maximum fireball size has been reached.
- **Kill Enemies**: Enemy skeletons start with 100 health. Based on how long you charge your attacks, they may take a different number of hits before dying.
- **Pickup Items**: Some enemies spawn a pickup, represented by a glowing yellow orb. Run over it to instantly gain the buff. These buffs can be very beneficial and are crucial to defeating all the waves.
- **Avoid Enemies**: Each skeleton has a sword, which causes damage when it hits the player. Run from enemies to avoid this at all costs. The amount of hits it takes an enemy to kill the player depends on the difficulty.

## Tips
- **Hit Enemies to Stay Alive**: The enemies cannot move while taking damage, and thus hitting them prevents a lot of damage to the player.
- **Use the Roll Ability**: The roll ability covers great distances, but can be dangerous if used near the edge. Find instances to safely roll away from the enemies, then begin casting from a safe distance.
- **Roll Through Enemies**: You can use the roll ability to roll through a pack of enemies, which can be very useful to gather pickup items. If you cast an attack at the front of the pack, causing them to take damage, you can then safely roll through and gather items.
- **Make Sure to Pickup Items**: The pickup items are incredibly valuable, but have a limited lifetime once spawned. Running too far from packs of enemies can cause you to miss pickup items, so try to pay attention to when a pickup item drops.
- **Combo Abilities**: Using the damage buff ability followed by a force pull on a big group of enemies is a deadly combo. Use it when you have built up charges to take out the whole pack.

## Development
For my second microgame, I wanted to build upon what I learned from my first game, while incorporating new things that I know and love from being a gamer myself. I started by learning how to setup a third-person controller, and adjusting it to where I felt the movement controls were fun and energetic. This meant making sure there was a roll ability, as that has been an element of most of my favorite games. Next, I had to setup a working animator controller, which correctly responded to all of the movements. I encountered plenty of issues in getting a fully working player character, but I am proud of what the final result is. For the enemies, I wanted to use the NavMeshAgent AI that seems to be a standard, and I found some issues with implementing it. Mainly, with issues arising from physics interactions on the NavMeshAgent. This created some adversity, as in my previous game, the Force Pull ability had been done using rigidbodies. Alas, I was able to adjust it accordingly (by disabling and re-enabling the NavMeshAgent), and I'm pleased with the final result of the Force Pull ability in this game. I also managed to use object pooling in this project, as well as a centralized and encapsulated UI Manager system. I learned a lot about Third-person controllers, animator controllers, AI enemies, object pooling, and singletons through making this game. I hope to carry this knowledge into future games I create or work on, and I look forward to the next project!

## Credits
- **Unity Learn Junior Programmer Pathway**: Following this course has been tremendously helpful and educational, and is a big reason I was able to make this game.
- **Unity Asset Store**: The asset store was very helpful to be able to find 3D models and animations to work with.
- **Testers**: Special thanks to the few testers in my circle who were able to provide feedback as I created the game.

## Feedback
I would love to hear any feedback, advice, or general recommendations for creating games as I will be continuing to learn Unity and Game Development, and aspire to join the Game Development Industry. Please use the GitHub Issues for this repository to contact me. I appreciate any and all input. Thanks!
