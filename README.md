# ARCANOID

The game project was created in approx. 5 days for the recruitment task. The game was created in a multi scene workflow - I divided the project into UI, Level and managers.
Level information, such as block color, level constants, etc., is included in the ScriptableObject [LevelData](Assets/Natoniewski_Arkanoid/Scripts/GameLevel/LevelData.cs) file at any time.

The game uses the [ObjectPool](https://docs.unity3d.com/ScriptReference/Pool.ObjectPool_1.html) data structure offered by unity. Objects created by the pool are immediately moved to the scene that exists only during the game, called "Factory", as dictated by [Factory.cs](Assets/Natoniewski_Arkanoid/Scripts/PersistantGame/Factory.cs).

The game was created in the 4:5 aspect.

![](Images/Gra.png)

![](Images/Gameplay.gif)

## Generator

The layout of the levels in each session is identical depending on the seed. The generator has been written in a way that allows tweaking the elements of interest to the user.
The arrangement of levels can be symmetrical or completely random. There were no custom editors in this project.

## Save & Load

In order to save and read the game state, I used BinaryFormatter. I chose this type of serialization because of the control it offers.
The state of the game is saved at every stage of the game. You can open the menu at any time by pressing the ESC key and clicking the Save button.

The file is saved to Application.persistentDataPath.

High score is kept at each stage of the game. After losing all life points, the player receives a prompt to save the result after having previously entered a 3-letter name.
All results saved in the file are visible on the final screen.

![](Images/SaveLoad.gif)

The results are inserted into the ScrollView.

## State machine

The whole game has been divided into three states - Pause, Start, Play

### Pause

The state the player is in when interacting with the UI.

### Start

The state the player is in at the moment of entering the next levels. In this state, the ball is at rest.
The ability to click ESC to stop the game is disabled there.

### Play

The state the player is in when the game is played.

## UI

I used LeanTween to tween the UI

## Misc

Known bug:

power-ups move in every state. The solution is to write a power up manager. This one would have a list of power ups and a factory, and like [BallManager](Assets/Natoniewski_Arkanoid/Scripts/PersistantGame/BallManager.cs) it would move powerups in the OnPlayUpdate() method. Thus, it solves the problem of lazy instancing of power ups by [Brick](Assets/Natoniewski_Arkanoid/Scripts/GameLevel/Bricks/Brick.cs). I did not do it because I did it many times in this assignment and therefore felt that I no longer needed to prove that I was able to do it.

# ASSETS USED

- [Arkanoid asset pack](https://kronbits.itch.io/matriax-free-assets) by Kronbits
- [Pixel Art GUI Elements](https://mounirtohami.itch.io/pixel-art-gui-elements) by Mounir Tohami
