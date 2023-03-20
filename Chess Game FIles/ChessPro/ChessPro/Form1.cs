using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChessPro
{
    public partial class Chess : Form
    {
        Game game = new Game();
        public Chess()
        {
            InitializeComponent();
            this.Size = new Size(300, 250);
            MainMenu_Background.BringToFront();
            StartGame.BringToFront();
            checkBox1.BringToFront();
            checkBox2.BringToFront();
            DrawBoard(); // Draw new board
            Show_Menu_Pawn_Promotion(false); //Hide pawn promotion menu Show_Menu_Pawn_Promotion()
        }
        public void Show_Menu_Pawn_Promotion(bool Show) // Shows pawn promo menu Show_Menu_Pawn_Promotion
        {
            if (Show)
            {
                Q5.Show();
                K3.Show();
                R2.Show();
                B4.Show();
                Pawn_Menu_Background.Show();
            }
            else
            {
                Pawn_Menu_Background.Hide();
                Q5.Hide();
                K3.Hide();
                R2.Hide();
                B4.Hide();
            }
        }
        private void pictureBox_Click(object sender, EventArgs e) //Handles selecting square // rename
        {
            call_check(game.whitego);
            if (game.client.found)
            {
                if (game.whitego && game.client.name == "black") { return; }
                if (!game.whitego && game.client.name == "white") { return; }
            }
            PictureBox me = sender as PictureBox;
            int x = Int32.Parse(me.Name[1].ToString());
            int y = Int32.Parse(me.Name[2].ToString());
            if (!game.Promotion_Active) { game.clicked(x, y); }
            if (game.Promotion_Active)
            {
                Show_Menu_Pawn_Promotion(true);
                return;
            }
            game.clicked(x, y);
            DrawBoard();
            call_check(game.whitego);
        }
        private void Win()
        {

        }
        private void Lose()
        {

        }
        private void Draw()
        {

        }
        private void call_check(bool forwhite)
        {
            bool incheck = false;
            string name = "";
            int colour = 0;
            if (forwhite)
            {
                name = "White";
                colour = 1;
            }
            else
            {
                name = "Black";
                colour = -1;
            }
            //BLACK
            if (game.check(colour))
            {
                label2.Text = name + " Check";
                incheck = true;
            }
            else { label2.Text = "No Check"; }
            if (game.Checkmate(colour))
            {
                label2.Location = new Point(200, 3);
                if (incheck)
                {
                    label2.Text = name + " Checkmate";
                    if (game.client.found)
                    {
                        if(game.client.name == name.ToLower())
                        {
                            Lose();
                        }
                        else
                        {
                            Win();
                        }
                    }
                    else
                    {
                        Win();
                    }
                }
                else { 
                    label2.Text = name + " Stalemate";
                    Draw();
                }
            }
        }
        private void timer1_Tick(object sender, EventArgs e) //Rename- make clear what timer1 ticks for
        {
            if (game.IsItMyGo())
            {
                DrawBoard();
                //timer1.Enabled = false;
                call_check(game.whitego);
            }
        }
        private PictureBox getbox(int[] coord) //return a picturebox given location - finds by name.
        {
            if (coord == null) { return null; }
            string name = "p" + coord[0].ToString() + coord[1].ToString();
            PictureBox a = this.Controls.Find(name, true).First() as PictureBox;
            return a;

        }
        private void DrawBoard() // Draws new board. Rename
        {
            string piece = "";
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    int piecenum = game.returnpiece(x, y);
                    switch (Math.Abs(piecenum))
                    {
                        case 7:
                            piece = "pawn";
                            break;
                        case 10: // En Passant
                        case 1:
                            piece = "pawn";
                            break;
                        case 9:
                        case 2:
                            piece = "rook";
                            break;
                        case 3:
                            piece = "knight";
                            break;
                        case 4:
                            piece = "bishop";
                            break;
                        case 5:
                            piece = "queen";
                            break;
                        case 8:
                        case 6:
                            piece = "king";
                            break;
                        case 0:
                            piece = "clear";
                            break;
                        default:
                            piece = "selected";
                            break;
                    }
                    if (piecenum < 0) { piece += "b.png"; }
                    else { piece += ".png"; }
                    if (((x + y) % 2 == 0) && !piece.Contains("selected")) { piece = "light" + piece; }
                    int[] box = { x, y };
                    Drawtobox(getbox(box), piece);
                }
            }
        }
        private void Drawtobox(PictureBox set, string filename) // Changes images on box
        {
            if (set == null)
            {
                return;
            }
            PictureBox draw = set as PictureBox;
            draw.Load("art/" + filename);
        }
        private void button1_Click(object sender, EventArgs e) // connects to server
        {
            if (TextBoxIP.Text[0].ToString() != "E") { game.client.serverip = TextBoxIP.Text; }
            game.reload();
            game.whitetime = 1801;
            game.blacktime = 1801;
            DrawBoard();
            if (game.connect()) { label1.Text = "Connected: " + game.client.name; button1.Text = "Disconnect"; }
            else { label1.Text = "Server was unable to find another player."; }

        }
        private void pictureBox2_Click(object sender, EventArgs e) // handles pawn promotion
        {
            PictureBox me = sender as PictureBox;
            game.Pawnpromo(Int32.Parse(me.Name[1].ToString()));
            Show_Menu_Pawn_Promotion(false);
            DrawBoard();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            timer1.Enabled = (!timer1.Enabled);
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            if (game.whitego)
            {
                Black_Clock.ForeColor = Color.White;
                White_Clock.ForeColor = Color.Red;
                game.whitetime--;
                string seconds = (game.whitetime % 60).ToString();
                if (seconds.Length == 1) { seconds = "0" + seconds; }
                White_Clock.Text = (game.whitetime / 60).ToString() + ":" + seconds;
            }
        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            if (!game.whitego)
            {
                Black_Clock.ForeColor = Color.Red;
                White_Clock.ForeColor = Color.White;
                game.blacktime--;
                string seconds = (game.blacktime % 60).ToString();
                if (seconds.Length == 1) { seconds = "0" + seconds; }
                Black_Clock.Text = (game.blacktime / 60).ToString() + ":" + seconds;
            }
        }
        private void StartGame_Click(object sender, EventArgs e)
        {
            timer2.Enabled = true;
            if (!checkBox1.Checked)
            {
                this.Size = new Size(655, 720);
                if (checkBox2.Checked)
                {
                    this.Size = new Size(655, 800);
                }
            }
            else
            {
                this.AutoScroll = true;
            }
            checkBox1.Dispose();
            checkBox2.Dispose();
            StartGame.Dispose();
            MainMenu_Background.Dispose();
            this.Location = new Point(50, 50);
        }
        private void TextBoxIP_Click(object sender, EventArgs e)
        {
            TextBoxIP.Text = "";
        }
    }
}
