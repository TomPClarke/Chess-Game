using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessPro
{
    public class Game
    {
        const int WHITE = 1;
        const int BLACK = -1;
        const int EMPTY_SPACE = 0;
        const int PAWN = 1;
        const int ROOK = 2;
        const int KNIGHT = 3;
        const int BISHOP = 4;
        const int QUEEN = 5;
        const int KING = 6;
        const int PAWN_MOVED = 7;
        const int KING_MOVED = 8;
        const int ROOK_MOVED = 9;
        const int PAWN_ENPASS = 10;
        /*TODO LIST
         * - CASTLING
         * - Make Server Less finicky
         * - Main Menu
         * - STALEMATE
         * - Allow ports to be added with IP address.
         */
        bool whitecheckmate = false;
        int pawnx;
        public int whitetime = 1800;
        public int blacktime = 1800;
        int pawny;
        public bool whitego = true;
        public bool Promotion_Active = false;
        int[] lastpawnpos = { 0, 0 };
        public int[] inhand = { 0, 0 };
        public Client client = new Client();
        int[,] gameboard = new int[,]
        {
            {-2,-3,-4,-5,-6,-4,-3,-2},
            {-1,-1,-1,-1,-1,-1,-1,-1},
            {0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0},
            {1,1,1,1,1,1,1,1},
            {2,3,4,5,6,4,3,2}
        };
        public Game()
        {

        }
        public bool connect() // connects to client
        {
             return client.connect();
        }
        public void reload() // resets gameboard
        {
            whitego = true;
            gameboard = new int[,]
        {
            {-2,-3,-4,-5,-6,-4,-3,-2},
            {-1,-1,-1,-1,-1,-1,-1,-1},
            {0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0},
            {0,0,0,0,0,0,0,0},
            {1,1,1,1,1,1,1,1},
            {2,3,4,5,6,4,3,2}
        };
        }
        public bool check(int prefix = -1, int alty = -1, int altx = -1, int[,] board = null)
        {
            if(board == null) { board = gameboard; }
            bool incheck = false;
            int[] pos = { alty, altx };
            if (alty + altx == -2) { pos = findking(prefix == -1); }
            prefix *= -1; //prefix refers to piece which can take you.
            if(pos == null) { return false; }
            int y = pos[0];
            int x = pos[1];
            // gameboard[y, x]
            if (IsOccupied(y + prefix, x + 1, prefix*(PAWN_MOVED)) || IsOccupied(y + 1, x + 1, prefix*(PAWN)) || IsOccupied(y + prefix, x + 1, prefix*(PAWN_ENPASS)))
            {
                incheck = true; goto end; // or 11 or 1
            }
            if (IsOccupied(y + prefix, x - 1, prefix * (PAWN_MOVED)) || IsOccupied(y + 1, x - 1, prefix * (PAWN)) || IsOccupied(y+ prefix, x- 1, prefix * (PAWN_ENPASS)))
            {
                incheck = true; goto end;
            } //Can pawns take the king.
            bool take = false;
            for(int u = 0; u < 8; u++)
            {
                int piece = gameboard[y, u];
                if(piece == prefix *ROOK || piece == prefix * QUEEN)
                {
                    take = true;
                    if(u > x) { incheck = true; goto end; }
                }
                else if(piece != 0 && piece != (prefix * KING * -1))
                {
                    if (u > x) { break; }
                    take = false;
                }
            }
            if (take) { incheck = true; goto end; }
            for (int u = 0; u < 8; u++)
            {
                int piece = gameboard[u, x];
                if (piece == prefix * ROOK || piece == prefix * QUEEN)
                {
                    take = true;
                    if (u > y) { incheck = true; goto end; }
                }
                else if (piece != 0 && piece != (prefix * KING * -1))
                {
                    if (u > y) { break; }
                    take = false;
                }
            }
            if (take) { return true; }
            { //knights
                if (IsOccupied(y - 2, x + 1,prefix *3)) { incheck = true; goto end; }
                if (IsOccupied(y - 2, x - 1,prefix *3)) { incheck = true; goto end; }
                if (IsOccupied(y + 2, x - 1,prefix *3)) { incheck = true; goto end; }
                if (IsOccupied(y + 2, x + 1,prefix * 3)) { incheck = true; goto end; }
                if (IsOccupied(y + 1, x - 2,prefix * 3)) { incheck = true; goto end; }
                if (IsOccupied(y + 1, x + 2,prefix * 3)) { incheck = true; goto end; }
                if (IsOccupied(y - 1, x + 2,prefix * 3)) { incheck = true; goto end; }
                if (IsOccupied(y - 1, x - 2,prefix * 3)) { incheck = true; goto end; }
            }// bishops and queens
            for (int i = 1; i < 8; i++)
            {
                if (IsOccupied(y + i, x - i, prefix * BISHOP)) { incheck = true; goto end; }
                if (IsOccupied(y + i, x - i, prefix * QUEEN)) { incheck = true; goto end; }
                else if (IsOccupied(y + i, x - i)) { break; }
            }
            for (int i = 1; i < 8; i++)
            {
                if (IsOccupied(y - i, x + i, prefix * BISHOP)) { incheck = true; goto end; }
                if (IsOccupied(y - i, x + i, prefix * QUEEN)) { incheck = true; goto end; }
                else if (IsOccupied(y - i, x + i)) { break; }
            }
            for (int i = 1; i < 8; i++)
            {
                if (IsOccupied(y + i, x + i, prefix * BISHOP)) { incheck = true; goto end; }
                if (IsOccupied(y + i, x + i, prefix * QUEEN)) { incheck = true; goto end; }
                else if (IsOccupied(y + i, x + i)) { break; }
            }
            for (int i = 1; i < 8; i++)
            {
                if (IsOccupied(y - i, x - i, prefix * BISHOP)) { incheck = true; goto end; }
                if (IsOccupied(y - i, x - i, prefix * QUEEN)) { incheck = true; goto end; }
                else if (IsOccupied(y - i, x - i)) { break; }
            }
            //king
            if (IsOccupied(y - 1, x + 1,prefix * 6)) { incheck = true; goto end; }
            if (IsOccupied(y - 1, x - 1,prefix * 6)) { incheck = true; goto end; }
            if (IsOccupied(y - 1, x,prefix * 6)) { incheck = true; goto end; }
            if (IsOccupied(y + 1, x + 1,prefix *6)) { incheck = true; goto end; }
            if (IsOccupied(y + 1, x - 1,prefix * 6)) { incheck = true; goto end; }
            if (IsOccupied(y + 1, x,prefix * 6)) { incheck = true; goto end; }
            if (IsOccupied(y , x + 1,prefix *6)) { incheck = true; goto end; }
            if (IsOccupied(y , x - 1,prefix *6)) { incheck = true; goto end; }
        end:
            return incheck;

        }
        public bool Checkmate(int colour)
        {
            int tx = inhand[0];
            int ty = inhand[1];
            int[,] temp = new int[8, 8];
            temp = backup(temp);
            gameboard = Deselect_All(gameboard);
            whitecheckmate = true;
            for (int i = 0; i < 8; i++)
            {
                for (int k = 0; k < 8; k++)
                {
                    if (gameboard[i,k] * colour > 0)
                    {
                        clicked(k, i);
                        if(whitecheckmate == false)
                        {
                            gameboard = temp;
                            loadhand(tx, ty);
                            return false;
                        }
                    } 
                }
            }
            loadhand(tx, ty);
            gameboard = temp;
            return whitecheckmate;
        }
        public void clicked(int x, int y) //decides piece on square and calls function for it
        {
            int piece = gameboard[y, x];
            gameboard = Deselect_All(gameboard);
            if(whitego == false && (piece > 0 && piece < 11)) { return; }
            if(whitego == true && (piece < 0)) { return; }
            switch (Math.Abs(piece))
            {
                case 0: //free space
                    break;
                case PAWN: // pawn
                    loadhand(x, y);
                    Pawn(true, x, y, (piece < 0));
                    break;
                case ROOK: // rook
                    loadhand(x, y);
                    Rook(x , y);
                    break;
                case KNIGHT: // knight
                    loadhand(x, y);
                    Knight(x, y);
                    break;
                case BISHOP: // bishop
                    loadhand(x, y);
                    Bishop(x, y);
                    break;
                case QUEEN: // Queen
                    loadhand(x, y);
                    Bishop(x, y);
                    Rook(x, y);
                    break;
                case KING: // King
                    loadhand(x, y);
                    King(x, y);
                   break;
                case PAWN_MOVED: //Pawnmoved
                case PAWN_ENPASS: // en passant
                    loadhand(x , y);
                    Pawn(false, x, y, (piece < 0));
                    break;
                case KING_MOVED: //Kingmoved
                    loadhand(x, y);
                    King(x, y);
                    break;
                case ROOK_MOVED: //rookmoved
                        //return "rook";
                default: //selected space
                    Selected(x, y, piece);
                    break;
            }
        }
        private bool allowmove(int x, int y, bool complex = false)
        {
            if (y < 0 || x < 0 || y > 7 || x > 7) { return false; } //Checks if square is out of bounds
            int spot = gameboard[x, y] * gameboard[inhand[1], inhand[0]]; //Checks square is either free or taken by another piece.
            if (spot <= 0)
            {
                if (Self_Check(y, x, complex)) { goto end; }
                gameboard[x, y] += 50; // "Hashes" highlighted square.
                whitecheckmate = false;
                if (complex) 
                {
                    gameboard[x, y] = 12; //Special case for en passant.
                }
                return (spot == 0);
            }
            return false;
        end:
            return true;
        }
        private bool Self_Check(int x, int y, bool complex)
        {
            bool incheck = false;
            int colour = BLACK;
            if (whitego) { colour = WHITE; }
            int[,] temp = new int[8, 8];
            temp = backup(temp);
            //Deselect_All(temp);
            gameboard = Deselect_All(gameboard);
            gameboard[y, x] = gameboard[inhand[1], inhand[0]];
            gameboard[inhand[1], inhand[0]] = 0;
            if (complex)
            {
                gameboard[y + colour, x] = 0;
            }
            if (whitego)
            {
                incheck = check(WHITE, -1, -1);
            }
            else
            {
                incheck = check(BLACK, -1, -1);
            }
            gameboard = temp;
            return incheck;
        }
        private void loadhand(int x, int y) 
        {
            inhand[0] = x;  
            inhand[1] = y;
        }
        private void King( int x , int y)
        {
            allowmove(y - 1, x);
            allowmove(y - 1, x - 1);
            allowmove(y - 1, x + 1);
            allowmove(y, x + 1);
            allowmove(y, x - 1);
            allowmove(y + 1, x);
            allowmove(y + 1, x - 1);
            allowmove(y + 1, x + 1);
            // Castling
        }
        private void Bishop(int x, int y)
        {
                for (int i = 1; i < 8; i++)
                {
                    if(!allowmove(y + i, x + i)) { break; }
                }
                for (int i = 1; i < 8; i++)
                {
                    if (!allowmove(y + i, x - i)) { break; }
                }
                for (int i = 1; i < 8; i++)
                {
                    if (!allowmove(y - i, x + i)) { break; }
                }
            for (int i = 1; i < 8; i++)
            {
                if (!allowmove(y - i, x - i)) { break; }
            }

        }
        private void Knight(int x, int y)
        {
            allowmove(y -2 , x + 1); 
            allowmove(y -2, x - 1); 
            allowmove(y + 2, x - 1); 
            allowmove(y + 2, x + 1); 
            allowmove(y + 1, x - 2); 
            allowmove(y + 1, x + 2); 
            allowmove(y - 1, x + 2); 
            allowmove(y - 1, x - 2);
        }
        private void Rook(int x, int y)
        {
            for (int i = x + 1; i < 8; i++)
            {
                if (!allowmove(y, i)) { break; }
            }
            for (int i = x -1; i > -1; i--)
            {
                if (!allowmove(y, i)) { break; }
            }
            for (int i = y + 1; i < 8; i++)
            {
                if (!allowmove(i, x)) { break; }
            }
            for (int i = y -1; i > -1; i--)
            {
                if (!allowmove(i, x)) { break; }
            }
        }
        private bool IsOccupied(int x, int y, int piece = 0)
        {
            
            if (y >= 0 && x >= 0 && y <= 7 && x <= 7)
            {
                if (piece != 0)
                {
                    return (gameboard[x, y] == piece);
                }
                return (gameboard[x, y] != 0);
            }
            return false;
        }
        private void Pawn(bool first, int x, int y, bool black)
        {
            int direction = -1;
            if (black) { direction += 2; } //Gets Direction that pawn is going
            if (IsOccupied(y + direction, x - 1))
            {
                allowmove(y + direction, x - 1);
            }
            if (IsOccupied(y + direction, x + 1))
            {
                allowmove(y + direction, x + 1); // Handles taking pieces diagonally
            }
            if (IsOccupied(y, x- 1, direction *10)) { 
                allowmove(y + direction, x -1, true); }
            if (IsOccupied(y, x +1, direction * 10)) { 
                allowmove(y + direction, x + 1, true); } // Checks for en passant
            if (IsOccupied(y + direction, x)) { return; } //Checks piece above for obstructions
            if(!allowmove(y + direction , x)) { return; } //Highlight piece aboce it, ends routine if blocked
            if (first)
            {
                if (IsOccupied(y + direction * 2, x)) { return; } //Checks two pieces up for obstruction
                allowmove(y + (2 * direction), x); //Highlights piece above it.
            }

        }
        private int[,] backup(int[,] temp)
        {
            for (int i = 0; i < 8; i++)
            {
                for (int k = 0; k < 8; k++)
                {
                    temp[i, k] = gameboard[i, k];
                }
            }
            return temp;
        }
        private void Selected(int x, int y, int select)
        {
            /*int[,] temp = new int[8,8];
            temp = backup(temp);
            temp = Deselect_All(temp);*/
            bool endturn = true;
            Remove_EnPassant(); // If square was eligible for en passant deselect it - since it is the end of the turn.
            int piece_inhand = gameboard[inhand[1], inhand[0]];
            switch (piece_inhand)
            {
                case -1:
                case 1:
                    if (Math.Abs(y - inhand[1]) == 2) 
                    { 
                      piece_inhand *= 10; // select the piece for en passant IF (It is a pawn) && (It has moved two places).
                      lastpawnpos = new int[] { x, y};
                    }
                    else 
                    { 
                        piece_inhand *= 7;   //else set it to a regular moved pawn.
                    }
                    break;
                case 7:
                case -7:
                    if(y == 0 || y == 7)
                    {
                        endturn = false; //If a pawn is at the end of the board, it needs to be promoted ergo the turn is not over yet.
                    }
                    break;
                default:
                  break;

            }
            gameboard[y, x] = piece_inhand;
            gameboard[inhand[1], inhand[0]] = 0;
            if (select == 12)
            {
                gameboard[y + (piece_inhand / 7), x] = 0;
            }
            if (select == 13)
            {

            }
            //make so if move puts board in check for own player move is disallowed.
            /*if (whitego)
            {
                if (check(1))
                {
                    gameboard = temp;
                    //whitego = !whitego;
                    return;
                }
            }
            else
            {
                if (check(-1)) 
                {
                    gameboard = temp;
                    //whitego = !whitego;
                    return; }
            }*/
            if (endturn)
            {
                whitego = !whitego;
                if (client.found)
                {
                }
                if (select == 12)
                {
                    client.send(x, y, inhand[1], inhand[0], 2 * piece_inhand);
                }
                else
                {
                    client.send(x, y, inhand[1], inhand[0], piece_inhand);
                }
            }
            else
            {
                Promotion_Active = true;
                pawnx = x;
                pawny = y;
            }
        }
        public void Pawnpromo(int piece)
        {
            piece *= (gameboard[pawny, pawnx] / 7);
            gameboard[pawny, pawnx] = piece;
            client.send(pawnx, pawny, inhand[1], inhand[0], piece);
            Promotion_Active = false;
            gameboard[inhand[1], inhand[0]] = 0;
            whitego = !whitego; 
        }
        private void Remove_EnPassant()
        {
            for (int i = 0; i < 8; i++)
            {
                for (int k = 0; k < 8; k++)
                {
                    if (gameboard[i, k] == 10)
                    {
                        gameboard[i, k] = 7;
                    }
                    if (gameboard[i, k] == -10)
                    {
                        gameboard[i, k] = -7;
                    }
                }
            }
        }
        public bool IsItMyGo()
        {
            if (client.found)
            {
                int[,] newboard = new int[8, 8];
                newboard = client.Checkforboard(); //Change, so one doesn't need knowledge of how client.Checkforboard works
                if (newboard != null)
                {
                    gameboard = newboard;
                    whitego = !whitego;
                    return true;
                }
                return false;
            }
            return false;
        }
        private int[] findking(bool black)
        {
            int king = 6;
            int[] pos = new int[2];
            if (black) { king *= -1; }
            for (int i = 0; i < 8; i++)
            {
                for (int k = 0; k < 8; k++)
                {
                    if (gameboard[i, k] == king)
                    {
                        pos[0] = i;
                        pos[1] = k;
                        return pos;
                    }
                }
            }
            return null;
        }
        public int returnpiece(int x, int y)
        {
            return gameboard[y, x];
        }
        private int[,] Deselect_All(int[,] board)
        {
            for(int i = 0; i< 8; i++)
            {
                for(int k = 0; k < 8; k++)
                {
                    if (Math.Abs(board[i,k]) > 13) // A Selected number: 50 + piece_num
                    {
                        board[i, k] -= 50;
                    }
                    if(board[i,k] == 12) // Handles highlightedd squares under en passant.
                    {
                        board[i, k] = 0;
                    }
                }
            }
            return gameboard;
        }
    }
}