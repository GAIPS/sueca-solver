# sueca-solver
This solution provides an artificial player for *Sueca*, a Portuguese imperfect information card game.
The chosen algorithm for this agent's main procedure is PIMC, which solves the hidden information with determinization.
Additionally, the computation of each perfect information game is obtained with a Min-Max search.


### shared-files
The shared-files project contains all the reusable files that code the basic game concepts, and also the definition of our artificial players.
We ended up with several final players due to different possible parametrizations of the algorithm, except for the RandomPlayer and the RuleBasedPlayer.

##### RandomPlayer
It chooses a completely random move between the possible ones. 

##### RuleBasedPlayer
it tries to reproduce the deliberation process of a non-profecional player (me) with predefinied rules.

##### TrickPlayer
The TrickPlayer samples a large number of distributions (N = 1000), although exploring shallower game tree in the MinMax (depth limit = 1 trick).

##### SmartPlayer
The SmartPlayer explores deeper game tree but sampling at least 20 distributions.

##### SmartestPlayer
The SmartestPlayer does the same as the previous one, altough limiting the main loop with time instead of iterations (N).

##### ElephantPlayer
The ElephantPlayer was an attempt to introduce a caching memory to speed up the MinMax and allow deeper explorations.

## single-game
The single-game program uses a text-based interface on Terminal and produces a game for three humans and one agent. this was the first created program to perceive the competence of the player.

### test
The test program purpose was just debugging specific method calls...

### war
The war program computes extensive series of games between 4 players. This program is not yet parameterizable in run-time :( However, the initial assignments to the main variables allow to control the number of games, the type of palyers, if the program is paralled or not, if it should save results and to where... At the end, it return the rate of won games.

### sueca-player
This program was integrated as a communicating module of a complex system where a social robot plays *Sueca* with three humans using physical cards over a touch-table. This social robot must be able to play the game properly and also interact socially other players.
The sueca-player is only the AI component that chooses a card to play, or computes the current rewards of the cards being played. Therefore, it has to communicate that information to other modules resposible for the sociability of the robot. This communication is achieved using Thalamus Framework.

### unity-emulator
The unity-emulator is a testing program that emulates Thalamus behaviors from the touch-table game interface. It sends messages to reproduce game events and consequently test the AI answers to those messages.

### results
This folder includes logs of computed game sessions by **war**, which were used on matlab to extract a linear regression of the final points. The final obtained linear regression was a poor prediction of the final points but allowed us to create three categories to classify the initial conditions of a team (easy, medium or hard). 