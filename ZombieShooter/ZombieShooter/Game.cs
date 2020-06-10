﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ZombieShooter
{
    public partial class Game : Form
    {
        Player igrach;
        bool goLeft, goRight, goUp, goDown;
        bool gameOver = false;
        string facing = "up";
        int playerHealth = 100;
        int speed = 10;
        int ammo = 10;
        int zombieSpeed = 3;
        int kills = 0;
        Random newRnd = new Random();
        List<PictureBox> zombies = new List<PictureBox>();

        public Game(string playername)
        {
            igrach = new Player(playername);
            InitializeComponent();
            Timer.Start();
            Initial_Spawn();
        }

        private void Initial_Spawn ()
        {
            for (int i = 0; i < 3; i++)
                MakeZombies();
        }

        private void Game_KeyDown(object sender, KeyEventArgs e) // dokolku se pritisne kopce da doznaeme na koja strana treba da odime
        {
            if (gameOver == true)
            {
                return;
            }

            if (e.KeyCode == Keys.Left)
            {
                goLeft = true;
                facing = "left";
                Player.Image = Properties.Resources.left;
            }
            
            if (e.KeyCode == Keys.Right)
            {
                goRight = true;
                facing = "right";
                Player.Image = Properties.Resources.right;
            }

            if (e.KeyCode == Keys.Up)
            {
                goUp = true;
                facing = "up";
                Player.Image = Properties.Resources.up;
            }

            if (e.KeyCode == Keys.Down)
            {
                goDown = true;
                facing = "down";
                Player.Image = Properties.Resources.down;
            }

        }

        private void Game_KeyUp(object sender, KeyEventArgs e) // dokolku se otpusti kopceto da se prekine so dvizenje
        {
            if (gameOver == true)
            {
                return;
            }

            if (e.KeyCode == Keys.Up)
            {
                goUp = false;
            }

            if (e.KeyCode == Keys.Down)
            {
                goDown = false;
            }

            if (e.KeyCode == Keys.Right)
            {
                goRight = false;
            }

            if (e.KeyCode == Keys.Left)
            {
                goLeft = false;
            }

            if (e.KeyCode == Keys.Space && ammo > 0)
            {
                ammo--;
                ShootBullet(facing);

                if (ammo == 1)
                {
                    dropAmmo();
                }
            }

        }

        private void Timer_Tick(object sender, EventArgs e) // na sekoja sekunda od timerot da se pravi update na ammo, pozicija i sl.
        {
            if (playerHealth > 1)
            {
                pbHealth.Value = playerHealth;
            }
            else
            {
                gameOver = true;
                Player.Image = Properties.Resources.dead;
                Timer.Stop();
            }
            if (playerHealth < 40) pbHealth.SetState(2);

            lblMadeKills.Text = igrach.Points.ToString();
            lblTotalAmmo.Text = ammo.ToString();

            if (goLeft == true && Player.Left > 0)
            {
                Player.Left -= speed;
            }
            if (goRight == true && Player.Left + Player.Width < this.ClientSize.Width - 15)
            {
                Player.Left += speed;
            }
            if (goUp == true && Player.Top > 45)
            {
                Player.Top -= speed;
            }
            if (goDown == true && Player.Top + Player.Height < this.ClientSize.Height - 15)
            {
                Player.Top += speed;
            }

            foreach (Control x in this.Controls) // sobiranje na ammo, dvizenje na zombies i presmetuvanje udari
            {
                if (x is PictureBox && (string)x.Tag == "ammo")
                {
                    if (Player.Bounds.IntersectsWith(x.Bounds))
                    {
                        this.Controls.Remove(x);
                        ((PictureBox)x).Dispose();
                        ammo += 5;
                    }
                }

                if (x is PictureBox && (string)x.Tag == "zombie")
                {
                    if (x.Left > Player.Left)
                    {
                        x.Left -= zombieSpeed;
                        ((PictureBox)x).Image = Properties.Resources.zleft;
                    }

                    if (x.Left < Player.Left)
                    {
                        x.Left += zombieSpeed;
                        ((PictureBox)x).Image = Properties.Resources.zright;
                    }

                    if (x.Top < Player.Top)
                    {
                        x.Top += zombieSpeed;
                        ((PictureBox)x).Image = Properties.Resources.zdown;
                    }

                    if (x.Top > Player.Top)
                    {
                        x.Top -= zombieSpeed;
                        ((PictureBox)x).Image = Properties.Resources.zup;
                    }

                    if (Player.Bounds.IntersectsWith(x.Bounds))
                    {
                        playerHealth -= 2;
                    }
                }

                foreach (Control j in this.Controls)
                {
                    if (j is PictureBox && (string)j.Tag == "bullet" && x is PictureBox && (string)x.Tag == "zombie")
                    {
                        if (j.Bounds.IntersectsWith(x.Bounds))
                        {
                            igrach.IncreasePoints();
                            this.Controls.Remove(x);
                            x.Dispose();
                            this.Controls.Remove(j);
                            j.Dispose();
                            MakeZombies();
                        }
                    }
                }
            }

        }

        public void MakeZombies()
        {
            PictureBox zombie = new PictureBox();
            zombie.Tag = "zombie";
            zombie.Image = Properties.Resources.zdown;
            zombie.Left = newRnd.Next(0, 900);
            zombie.Top = newRnd.Next(0, 800);
            zombie.SizeMode = PictureBoxSizeMode.AutoSize; // fit to picture
            this.Controls.Add(zombie);
            Player.BringToFront();
        }

        private void dropAmmo() // random pozicija za spawn na ammo 
        {
            PictureBox ammo = new PictureBox();
            ammo.Image = Properties.Resources.ammo_Image;
            ammo.SizeMode = PictureBoxSizeMode.AutoSize;
            ammo.Left = newRnd.Next(10, this.ClientSize.Width - ammo.Width);
            ammo.Top = newRnd.Next(10, this.ClientSize.Height - ammo.Height);
            ammo.Tag = "ammo";

            this.Controls.Add(ammo);
            Player.BringToFront();
        }

        public void ShootBullet(string direction)
        {
            Bullet shootBullet = new Bullet();
            shootBullet.direction = direction;
            shootBullet.bulletLeft = Player.Left + (Player.Width / 2); // da pocne kursumot otprilika na pola covek
            shootBullet.bulletTop = Player.Top + (Player.Height / 2);
            shootBullet.makeBullet(this);

        }
    }
}