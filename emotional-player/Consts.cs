using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmotionalPlayer
{
    public class Consts
    {
        public static readonly string INIT = "INIT";
        public static readonly string PARTNER = "Partner";
        public static readonly string OPPONENT = "Opponent";

        public static readonly string TRICK_INCREASE_PROPERTY = "Trick(Increase)";
        public static readonly string TRICK_WINNER = "Trick(Winner)";
        public static readonly string TRICK_SCORE = "Trick(Score)";
        public static readonly string TRICK_END = "Trick(End)";
        public static readonly string END_GAME = "Game(End)";
        public static readonly string END_SESSION = "Session(End)";
        public static readonly string PLAY_INFO = "Play(Info)";
        public static readonly string CURRENT_PLAYER = "Current(PlayerID)";
        public static readonly string NEXT_PLAYER = "Next(PlayerID)";

        public static readonly string ID_PROPERTY = "My(Id)";
        public static readonly string DIALOGUE_FLOOR_PROPERTY = "Dialogue(Floor)";

        public static readonly string DEFAULT_SUBJECT = "Board";

        public static readonly string DIALOGUE_STATE_PROPERTY = "DialogueState(Board)";
        public static readonly string STATE_SESSION_START = "SessionStart";
        public static readonly string STATE_GAME_START = "GameStart";
        public static readonly string STATE_SHUFFLE = "Shuffle";
        public static readonly string STATE_CUT = "Cut";
        public static readonly string STATE_DEAL = "Deal";
        public static readonly string STATE_GAME_END = "GameEnd";
        public static readonly string STATE_SESSION_END = "SessionEnd";
        public static readonly string STATE_PLAYPARTNER = "PlayPartner";
        public static readonly string STATE_PLAYSELF = "PlaySelf";
        public static readonly string STATE_NEXT_PLAYER = "NextPlayer";
        public static readonly string STATE_TRICK_END = "TrickEnd";

        public const string AGENT_TYPE_GROUP = "Group";
        public const string AGENT_TYPE_INDIVIDUAL = "Individual";

    }
}
