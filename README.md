# sueca-solver
This solution provides an artificial player for *Sueca*, a Portuguese imperfect information card game.
The chosen algorithm for this agent's main procedure is PIMC, which solves the hidden information with determinization.
Additionally, the computation of each perfect information game is obtained with a Min-Max search.


## shared-files
The shared-files project contains all the reusable files that code the basic game concepts, and also the definition of our artificial players.
We ended up with several final players due to different possible parametrizations of the algorithm, except for the RandomPlayer and the RuleBasedPlayer.

#### RandomPlayer
It chooses a completely random move between the possible ones. 

#### RuleBasedPlayer
it tries to reproduce the deliberation process of a non-profecional player (me) with predefinied rules.

#### TrickPlayer
The TrickPlayer samples a large number of distributions (N = 1000), although exploring shallower game tree in the MinMax (depth limit = 1 trick).

#### SmartPlayer
The SmartPlayer explores deeper game tree but sampling at least 20 distributions.

#### SmartestPlayer
The SmartestPlayer does the same as the previous one, altough limiting the main loop with time instead of iterations (N).

#### ElephantPlayer
The ElephantPlayer was an attempt to introduce a caching memory to speed up the MinMax and allow deeper explorations.

## single-game
The single-game program uses a text-based interface on Terminal and produces a game for three humans and one agent. this was the first created program to perceive the competence of the player.

## test

## war

## sueca-player

## unity-emulator

## results