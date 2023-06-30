using System.Runtime.InteropServices;
using System.Collections;

namespace Chess.GamePlay
{
    public class IncompleteChessModel : IChessModel
    {
        /// ----------------------------------------------------------------------------------
        /// ----------- No need to edit this introductory code -------------------------------
        /// ----------- (scroll down to see code you need to edit) ---------------------------
        /// ----------------------------------------------------------------------------------

        public IncompleteChessModel() { }

        public bool IsPieceOwnedByPlayer(char piece, Player player)
        {
            GridCharacter[] allWhitePieces = {
                GridCharacter.WhiteQueen,
                GridCharacter.WhiteKing,
                GridCharacter.WhiteBishop,
                GridCharacter.WhiteRook,
                GridCharacter.WhiteKnight,
                GridCharacter.WhitePawn
            };
            GridCharacter[] allBlackPieces = {
                GridCharacter.BlackQueen,
                GridCharacter.BlackKing,
                GridCharacter.BlackBishop,
                GridCharacter.BlackRook,
                GridCharacter.BlackKnight,
                GridCharacter.BlackPawn
            };
            GridCharacter[] allowedPieces = player == Player.White ? allWhitePieces : allBlackPieces;
            return allowedPieces.Contains((GridCharacter)piece);
        }

        public void MovePiece(char[][] board, Move move)
        {
            char sourcePiece = board[move.fromRow][move.fromColumn];
            board[move.fromRow][move.fromColumn] = (char)GridCharacter.Empty;
            board[move.toRow][move.toColumn] = sourcePiece;
        }

        private bool IsIndexInRange(int index)
        {
            return index >= 0 && index < 8;
        }

        public bool IsMoveWithinBoard(Move move)
        {
            return IsIndexInRange(move.fromRow) && IsIndexInRange(move.fromColumn) && IsIndexInRange(move.toRow) && IsIndexInRange(move.toColumn);
        }

        public bool IsMoveFromOwnPiece(char[][] board, Move move, Player player)
        {
            char fromPiece = board[move.fromRow][move.fromColumn];
            return IsPieceOwnedByPlayer(fromPiece, player);
        }

        public bool IsMoveToValidTile(char[][] board, Move move, Player player)
        {
            char toPiece = board[move.toRow][move.toColumn];
            return !IsPieceOwnedByPlayer(toPiece, player);
        }

        public bool IsMoveLegal(char[][] board, Move move, Player turn)
        {
            return IsMoveWithinBoard(move)
                && IsMoveFromOwnPiece(board, move, turn)
                && IsMoveToValidTile(board, move, turn)
                && IsPieceMoveLegal(board, move, turn)
                && !IsMoveIntoCheck(board, move, turn);
        }

        private bool IsPieceMoveLegal(char[][] board, Move move, Player turn)
        {
            char fromPiece = board[move.fromRow][move.fromColumn];
            switch (fromPiece.ToString().ToLower())
            {
                case "k":
                    return IsValidMovementForKing(board, move, turn);
                case "n":
                    return IsValidMovementForKnight(board, move, turn);
                case "p":
                    return IsValidMovementForPawn(board, move, turn);
                case "r":
                    return IsValidMovementForRook(board, move, turn);
                case "b":
                    return IsValidMovementForBishop(board, move, turn);
                case "q":
                    return IsValidMovementForQueen(board, move, turn);
                default:
                    return false;
            }
        }

        public bool IsValidMovementForKing(char[][] board, Move move, Player player)
        {
            int rowDifference = move.fromRow - move.toRow;
            int columnDifference = move.fromColumn - move.toColumn;

            // A valid move must not start and end at the same place
            if (rowDifference == 0 && columnDifference == 0)
            {
                return false;
            }

            // A valid move must not move a distance of 2+ in either direction
            else if (rowDifference < -1 || rowDifference > 1 || columnDifference < -1 || columnDifference > 1)
            {
                return false;
            }

            // Everything else (within 1 square) is legal
            else return true;
        }


        /// ----------------------------------------------------------------------------------
        /// ----------- Edit from here for Part 1: Piece Logic -------------------------------
        /// ----------------------------------------------------------------------------------

        private bool UnoccupiedPathCheck(char[][] board, int startRow, int startColumn, int moveLength, int incrementRow, int incrementColumn) 
        { //checks all spaces in the path are clear. *not including destination square, this requires extra logic for capture / not capture
            for(int i = 1; i < moveLength; i++) 
            {
                if (board[startRow + incrementRow * i][startColumn + incrementColumn * i] != '.') return false;
            }
            return true;
        }
        public bool IsValidMovementForKnight(char[][] board, Move move, Player player)
        { //no path for knight, check only size of move is correct, again destination square availabilty is checked elsewhere
            int rowDifference = move.toRow - move.fromRow;
            int columnDifference = move.toColumn - move.fromColumn;

            if (Math.Abs(rowDifference) == 1 && Math.Abs(columnDifference) == 2) return true;
            if (Math.Abs(rowDifference) == 2 && Math.Abs(columnDifference) == 1) return true;

            return false;
        }

        public bool IsValidMovementForRook(char[][] board, Move move, Player player)
        { //check direction and path there is clear
            int rowDifference = move.toRow - move.fromRow;
            int columnDifference = move.toColumn - move.fromColumn;

            if (rowDifference == 0 && columnDifference != 0)
            {
                return UnoccupiedPathCheck(board, move.fromRow, move.fromColumn, Math.Abs(columnDifference), 0, Math.Clamp(columnDifference, -1, 1));
            }
            if(rowDifference != 0 && columnDifference == 0)
            {
                return UnoccupiedPathCheck(board, move.fromRow, move.fromColumn, Math.Abs(rowDifference), Math.Clamp(rowDifference, -1, 1), 0);
            }
            return false;
        }

        public bool IsValidMovementForBishop(char[][] board, Move move, Player player)
        { //check direction and path there is clear
            int rowDifference = move.toRow - move.fromRow;
            int columnDifference = move.toColumn - move.fromColumn;

            if (Math.Abs(rowDifference) == Math.Abs(columnDifference))
            {
                return UnoccupiedPathCheck(board, move.fromRow, move.fromColumn, Math.Abs(rowDifference), Math.Clamp(rowDifference, -1, 1), Math.Clamp(columnDifference, -1, 1));
            }
            return false;
        }

        public bool IsValidMovementForQueen(char[][] board, Move move, Player player)
        { 
            if (IsValidMovementForBishop(board, move, player)) return true;
            if (IsValidMovementForRook(board, move, player)) return true;
            return false;
        }

        public bool IsValidMovementForPawn(char[][] board, Move move, Player player)
        {
            int pawnStartRank = player == Player.White ? 6 : 1; //rows are indexed top -> bottom but displayed bottom -> top
            int playerInt = player == Player.White ? -1 : 1; //inversed from displayed board for same reason as above
            int rowDifference = move.toRow - move.fromRow; 
            int columnDifference = move.toColumn - move.fromColumn;
            
            if (columnDifference == 0) //if not moved horizontally (captures caught in else if)
            {
                if (rowDifference == playerInt && board[move.toRow][move.toColumn] == '.') return true; //single space advance into open space

                if (rowDifference == 2 * playerInt && move.fromRow == pawnStartRank) //double move on first pawn move
                {
                    if (board[move.fromRow + playerInt][move.fromColumn] == '.' && board[move.fromRow + 2 * playerInt][move.fromColumn] == '.') return true; //check if path and destination is empty
                }
            }
            else if (Math.Abs(columnDifference) == 1 && rowDifference == playerInt) //if move if a capture
            {
                if (board[move.toRow][move.toColumn] != '.' && !IsPieceOwnedByPlayer(board[move.toRow][move.toColumn], player)) return true; //if the 
                return false;
            }
            return false;
        }

        /// ----------------------------------------------------------------------------------
        /// ----------- Edit from here for Part 2: End game logic ----------------------------
        /// ----------------------------------------------------------------------------------

        public bool IsInCheck(char[][] board, Player player)
        {
            //find king
            int[] kingpos = FindKingPosition(board, player);
            
            //loop through board, when found char != '.', if !isPieceOwnedByPlayer, and can validly move to the kings square, player is in check
            for(int row = 0; row < board.Length; row++) 
            {
                for(int col = 0;  col < board.Length; col++)
                {
                    if (board[row][col] != '.' && !IsPieceOwnedByPlayer(board[row][col], player))
                    {
                        if(IsPieceMoveLegal(board, new Move(row, col, kingpos[0], kingpos[1]), player )) return true;
                    }
                }
            }
            return false;
        }

        public bool IsMoveIntoCheck(char[][] board, Move move, Player player)
        {   //IsMoveLegal will fail if invalid move from the other methods, so we can assume this move was vaild
            char[][] newboard = board;
            newboard[move.toRow][move.toColumn] = newboard[move.fromRow][move.fromColumn];
            newboard[move.fromRow][move.fromColumn] = '.';
            if (IsInCheck(newboard, player)) return true;
            return false;

        }

        public bool IsGameOver(char[][] board, Player player)
        {
            //CURRENT: Program specs and tests do not account for blocking / capturing checking piece, will add that next.

            if (IsInCheck(board, player))
            {
                int[] kingPos = FindKingPosition(board, player);
                int checkingPiecesCount = 0;
                ArrayList checkingPieces = new ArrayList(); //struct: <piece>, <row>, <col>... (would've been used for blocking/capture of checking piece)

                for (int row = 0; row < board.Length; row++)
                { //find all checking pieces (not needed yet)
                    for (int col = 0; col < board.Length; col++)
                    {
                        if (IsPieceMoveLegal(board, new Move(row, col, kingPos[0], kingPos[1]), player))
                        {
                            checkingPiecesCount++;
                            checkingPieces.Add(board[row][col]);
                            checkingPieces.Add(row);
                            checkingPieces.Add(col);
                        }
                    }
                }

                //king move offsets
                int[] kingRowOffset = { -1, -1, -1, 0, 0, 0, 1, 1, 1 };
                int[] kingColOffset = { -1, 0, 1, -1, 0, 1, -1, 0, 1 };

                for (int i = 0; i < 8; i++)
                { //check for valid king moves, if one exists, and that move results in no check, game is not over
                    Move newKingMove = new Move(kingPos[0], kingPos[1], kingPos[0] + kingRowOffset[i], kingPos[1] + kingColOffset[i]);

                    if (IsMoveWithinBoard(newKingMove))
                    {
                        if (IsPieceMoveLegal(board, newKingMove, player))
                        {
                            if (!IsMoveIntoCheck(board, newKingMove, player)) return false;
                        }
                    }
                }
              
                return true;
                
            }
            return false;
        }

        private int[] FindKingPosition(char[][] board, Player player)
        {
            GridCharacter targetPiece = player == Player.White ? GridCharacter.WhiteKing : GridCharacter.BlackKing;

            for (int i = 0; i < board.Length; i++)
            {
                for (int j = 0; j < board[i].Length; j++)
                {
                    if (board[i][j] == (char)targetPiece)
                    {
                        return new int[] { i, j };
                    }
                }
            }
            return new int[] { -1, -1 };
        }
    }
}
