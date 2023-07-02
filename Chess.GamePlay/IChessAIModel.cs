using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.GamePlay
{
    public interface IChessAIModel
    {
        public int Evaluate(char[][] board, Player player);
        //<summary>
        //Evaluates the position
        //Positive for a position in favour of the player, negitive for in favour of the oppoenent
        //This can start as a simple material count, and progress to include check bonuses etc

        public int CountMaterial(char[][] board, Player player);
        //<summary>
        //Counts the amount of material a player has in standard form
        //pawn = 1, bishop and knight = 3, rook = 5, queen = 8
    }
}
