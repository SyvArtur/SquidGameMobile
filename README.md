# SquidGame Mobile
It's a 3D multiplayer casual party game for Android. This game was created using various design patterns (such as factory method, observer) and trying to follow the principles of OOP and SOLID.

## About project
Squidgame Mobile is a 3D third-person competitive multiplayer game. You go through several different deadly challenges to earn points.

<img src="https://drive.google.com/uc?export=view&id=1rDzHSygIoEx4OzLej0tae4nPzO2J19bz" width="600px">

### Frameworks
 - **Mirror** for creating a multiplayer game
 - **ML-Agents** for training NPC in 1 mini-game

### Briefly about the mini-games
In the first mini-game, players start running forward, but must immediately stop when a doll begins to turn towards them, otherwise they will be killed. To win, players must reach the finish line before the time runs out.

<img src="https://drive.google.com/uc?export=view&id=1Hl9Mv7Jlm_GbMS5vtgcOBJ4eU9nGWXYa" width="600px">

In the second mini-game, players must step onto the correct colored platform before time runs out, otherwise they fall and lose the game. The time for reaction decreases with each subsequent round.

<img src="https://drive.google.com/uc?export=view&id=1wOcltTfW636kVwbR-u7KPf5gJoEvXTgZ" width="600px">

In the third mini-game you need to remember the location of fruit images on 5x5 platforms. After a set time,  the pictures disappear, and images of a specific fruit are displayed on the walls. Players must choose the correct platform,  otherwise they fall into the lava and lose the game. The time to memorize decreases with each round.

<img src="https://drive.google.com/uc?export=view&id=1WyjYQaDOsj3rC5jlcW9aa-R3r69ydJ3b" width="600px">
<img src="https://drive.google.com/uc?export=view&id=1EIfTh9T0XBhgRQlZOJxXdL2KxwhPTtL7" width="600px">

There is no multiplayer in this fourth mini-game. The player is given the opportunity to choose the type of camera to control his car.  The car itself has a complex drift system, multiple gears and corresponding sound effects.  The player must shoot down NPCs running away from him, controlled by neural networks, demonstrating his superiority over them. NPCs are trained to evade the player's car.

<img src="https://drive.google.com/uc?export=view&id=1w0NvEc89WnYXFt1yRWvSVDG8OmKdH3Xp" width="600px">
<img src="https://drive.google.com/uc?export=view&id=1EG9B_seoC3hhwsB6TmMTVkXNVh59Zc6E" width="600px">
