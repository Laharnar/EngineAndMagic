# EngineAndMagic
Basic idea: *Linear combat RPG game in a city of modern age and magic.*

It should contain a short story and simple combat with teammates.

## Summary
Project contains:
- multiple character types with different behaviours, fighting
- player
- implemented A* pathfinding
- waypoint pathfinding for crowds
- tiled and drawn sprites for background

Below are listed a few important scenes.

### scene1

Scene "scene1" contains a demo with player and 2 groups, police and few vikings fighting. There is 1 NPC wandering around(using A*).
Ai is implemented with behaviour trees.
You can talk to few NPCs.

You can also fight as a player.
Controls

WASD:move
J: attack
K: defend, TODO
L: dash

Note: units don't die, probably broken collisions.

Characters are made from rectangle sprites, made and animated directly in unity.

Background is a mix of tiles and quickly painter images, made with Tiled and photoshop.

### plaza

I tried to make and interesting crowded location, like a market or something, where NPC walk around and do stuff.
I ended up with a bunch of NPCs that move between the stalls by using waypoints, some collision avoidance between characters and random timers. 

TODO: level collision avoidance.
