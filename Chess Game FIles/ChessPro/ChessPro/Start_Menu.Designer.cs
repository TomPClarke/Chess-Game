namespace ChessPro
{
    partial class Start_Menu
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.StartGame = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // StartGame
            // 
            this.StartGame.AutoSize = true;
            this.StartGame.BackColor = System.Drawing.Color.LightGray;
            this.StartGame.Cursor = System.Windows.Forms.Cursors.Hand;
            this.StartGame.Font = new System.Drawing.Font("Lucida Sans", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.StartGame.Location = new System.Drawing.Point(12, 9);
            this.StartGame.Name = "StartGame";
            this.StartGame.Size = new System.Drawing.Size(268, 55);
            this.StartGame.TabIndex = 87;
            this.StartGame.Text = "Play Local";
            this.StartGame.Click += new System.EventHandler(this.StartGame_Click);
            // 
            // Start_Menu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.StartGame);
            this.Name = "Start_Menu";
            this.Text = "Start_Menu";
            this.Load += new System.EventHandler(this.Start_Menu_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label StartGame;
    }
}