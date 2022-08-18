using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Killing_It
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer spawnAmmoTimer = new DispatcherTimer();
        DispatcherTimer spawnZombieTimer = new DispatcherTimer();

        Random rand = new Random();


        private List<Image> bloodStains = new List<Image>();
        private DispatcherTimer bloodStainRemoveTimer = new DispatcherTimer();

        bool moveUp, moveDown, moveLeft, moveRight, spawnedAmmo = false;
        string facing = "right";
        double playerSpeed = 4;
        double backgroundSpeed = 3;
        double zombieSpeed = 1;
        int maxAmmo = 30;
        int actualAmmo, ammoBoxBonus = 5;
        int posX, posY;
        int score = 0;

        public MainWindow()
        {
            InitializeComponent();
            actualAmmo = maxAmmo;
            ammo.Content = actualAmmo;

            spawnAmmoTimer.Tick += spawnAmmo;
            spawnAmmoTimer.Interval = TimeSpan.FromSeconds(5);

            spawnZombieTimer.Tick += spawnZombie;
            spawnZombieTimer.Interval = TimeSpan.FromSeconds(2);

            spawnZombieTimer.Start();
            spawnAmmoTimer.Start();
        }

        private void spawnZombie(object sender, EventArgs e)
        {
            DispatcherTimer zombieMoveTimer = new DispatcherTimer();
            Image zombie = new Image();
            int posX, posY;

            zombie.Source = new BitmapImage(new Uri("\\Images\\zombie.gif", UriKind.RelativeOrAbsolute));

            zombie.Height = 60;
            zombie.Width = 60;
            zombie.RenderTransformOrigin = new Point(0.5, 0.5);

            posX = rand.Next(0, (int)backgroundCanvas.ActualWidth - (int)zombie.ActualWidth);
            posY = rand.Next(0, (int)backgroundCanvas.ActualHeight - (int)zombie.ActualHeight);


            Canvas.SetLeft(zombie, posX);
            Canvas.SetTop(zombie, posY);

            zombie.Tag = "zombie";

            backgroundCanvas.Children.Add(zombie);

            zombieMoveTimer.Tick += moveZombies;
            zombieMoveTimer.Interval = TimeSpan.FromMilliseconds(30);
            zombieMoveTimer.Start();

        }

        private void moveZombies(object sender, EventArgs e)
        {
            foreach (var zombie in backgroundCanvas.Children.OfType<Image>())
            {
                if (zombie.Tag != null && zombie.Tag.Equals("zombie"))
                {
                    RotateTransform rotateTransform = new RotateTransform();
                    if (Canvas.GetLeft(zombie) > Canvas.GetLeft(player))
                    {
                        rotateTransform.Angle = 0;
                        Canvas.SetLeft(zombie, Canvas.GetLeft(zombie) - zombieSpeed);
                        zombie.RenderTransform = rotateTransform;
                        rotateTransform.Angle = 180;
                        zombie.RenderTransform = rotateTransform;
                    }

                    if (Canvas.GetLeft(zombie) < Canvas.GetLeft(player))
                    {
                        rotateTransform.Angle = 0;
                        Canvas.SetLeft(zombie, Canvas.GetLeft(zombie) + zombieSpeed);
                        zombie.RenderTransform = rotateTransform;
                    }

                    if (Canvas.GetTop(zombie) > Canvas.GetTop(player))
                    {
                        rotateTransform.Angle = 0;
                        Canvas.SetTop(zombie, Canvas.GetTop(zombie) - zombieSpeed);
                        zombie.RenderTransform = rotateTransform;
                        rotateTransform.Angle = -90;
                        zombie.RenderTransform = rotateTransform;
                    }

                    if (Canvas.GetTop(zombie) < Canvas.GetTop(player))
                    {
                        rotateTransform.Angle = 0;
                        Canvas.SetTop(zombie, Canvas.GetTop(zombie) + zombieSpeed);
                        zombie.RenderTransform = rotateTransform;
                        rotateTransform.Angle = 90;
                        zombie.RenderTransform = rotateTransform;
                    }


                    foreach (var bullet in backgroundCanvas.Children.OfType<Image>())
                    {
                        if (bullet.Tag != null && bullet.Tag.Equals("bullet") && bullet != null)
                        {
                            Rect zombieRect = new Rect(Canvas.GetLeft(zombie), Canvas.GetTop(zombie), zombie.Width, zombie.Height);
                            Rect bulletRect = new Rect(Canvas.GetLeft(bullet), Canvas.GetTop(bullet), bullet.Width, bullet.Height);

                            if (bulletRect.IntersectsWith(zombieRect))
                            {
                                score++;
                                Score.Content = "Score: " + score;

                                zombie.Source = new BitmapImage(new Uri("\\Images\\blood stain.png", UriKind.RelativeOrAbsolute));
                                zombie.Tag = "blood stain";
                                bloodStains.Add(zombie);

                                bullet.Source = null;

                                DispatcherTimer bloodStainRemoveTimer = new DispatcherTimer();

                                bloodStainRemoveTimer.Tick += removeBloodStain;
                                bloodStainRemoveTimer.Interval = TimeSpan.FromMilliseconds(150);
                                bloodStainRemoveTimer.Start();
                            }
                        }
                    }
                }
            }
        }

        private void removeBloodStain(object sender, EventArgs e)
        {
            int bloodStainNumber = bloodStains.Count;
            for (int i = 0; i < bloodStainNumber; i++)
            {
                if (bloodStains[i].Opacity != 0)
                {
                    bloodStains[i].Opacity -= 0.2;
                }
                else
                {
                    backgroundCanvas.Children.Remove(bloodStains[i]);
                    bloodStains.Remove(bloodStains[i]);
                    bloodStainRemoveTimer.Stop();
                }
            }
        }

        private void spawnAmmo(object sender, EventArgs e)
        {
            if (actualAmmo < maxAmmo && !spawnedAmmo)
            {
                Image ammoImage = new Image();
                ammoImage.Source = new BitmapImage(new Uri("\\Images\\ammo Box.png", UriKind.RelativeOrAbsolute));

                ammoImage.Height = 30;
                ammoImage.Width = 30;

                posX = rand.Next((int)Canvas.GetLeft(background) + 20, (int)Canvas.GetLeft(background) + (int)background.ActualWidth - (int)ammoImage.ActualWidth - 50);
                posY = rand.Next((int)Canvas.GetTop(background) + 20, (int)Canvas.GetTop(background) + (int)background.ActualHeight - (int)ammoImage.ActualHeight - 50);


                Canvas.SetLeft(ammoImage, posX);
                Canvas.SetTop(ammoImage, posY);
                ammoImage.Tag = "ammoBox";

                backgroundCanvas.Children.Add(ammoImage);
                spawnedAmmo = true;
            }
        }

        private void movePlayer()
        {
            foreach (var x in backgroundCanvas.Children.OfType<Image>())
            {
                if (x.Tag != null && x.Tag.Equals("box"))
                {
                    Rect playerHitBox = new Rect(Canvas.GetLeft(player), Canvas.GetTop(player), player.Width, player.Height);
                    Rect boxHitBox = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);
                    /*
                    if (playerHitBox.IntersectsWith(boxHitBox))
                    {
                        if (moveUp)
                        {
                            moveUp = false;
                            Canvas.SetTop(player, Canvas.GetTop(player) + playerSpeed);
                        }
                        else if (moveDown)
                        {
                            moveDown = false;
                            Canvas.SetTop(player, Canvas.GetTop(player) - playerSpeed);
                        }
                        else if (moveLeft)
                        {
                            moveLeft = false;
                            Canvas.SetLeft(player, Canvas.GetLeft(player) + playerSpeed);
                        }
                        else if (moveRight)
                        {
                            moveRight = false;
                            Canvas.SetLeft(player, Canvas.GetLeft(player) - playerSpeed);
                        }
                    }
                    */
                }

                if (x.Tag != null && x.Tag.Equals("ammoBox"))
                {

                    Rect playerHitBox = new Rect(Canvas.GetLeft(player), Canvas.GetTop(player), player.Width, player.Height);
                    Rect boxHitBox = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);

                    if (playerHitBox.IntersectsWith(boxHitBox))
                    {
                        if (actualAmmo + ammoBoxBonus > maxAmmo)
                        {
                            actualAmmo = maxAmmo;
                            ammo.Content = actualAmmo;
                        }
                        else
                        {
                            actualAmmo += ammoBoxBonus;
                            ammo.Content = actualAmmo;
                        }
                        backgroundCanvas.Children.Remove(x);
                        spawnedAmmo = false;
                    }

                    break;
                }
            }

            RotateTransform rotateTransform = new RotateTransform(0);
            if (moveUp && Canvas.GetTop(player) > 5)
            {
                Canvas.SetTop(player, Canvas.GetTop(player) - playerSpeed);
                player.RenderTransform = rotateTransform;
                rotateTransform.Angle = -90;
                player.RenderTransform = rotateTransform;
                facing = "up";
                moveBackground("down");
            }
            if (moveDown && Canvas.GetTop(player) + player.ActualHeight < backgroundCanvas.ActualHeight - 25)
            {
                Canvas.SetTop(player, Canvas.GetTop(player) + playerSpeed);
                player.RenderTransform = rotateTransform;
                rotateTransform.Angle = 90;
                player.RenderTransform = rotateTransform;
                facing = "down";
                moveBackground("up");
            }
            if (moveLeft && Canvas.GetLeft(player) > 5)
            {
                Canvas.SetLeft(player, Canvas.GetLeft(player) - playerSpeed);
                player.RenderTransform = rotateTransform;
                rotateTransform.Angle = 180;
                player.RenderTransform = rotateTransform;
                facing = "left";
                moveBackground("right");
            }
            if (moveRight && Canvas.GetLeft(player) + player.ActualWidth < backgroundCanvas.ActualWidth - 5)
            {
                Canvas.SetLeft(player, Canvas.GetLeft(player) + playerSpeed);
                player.RenderTransform = rotateTransform;
                facing = "right";
                moveBackground("left");
            }
        }

        private void moveBackground(string direction)
        {
            switch (direction)
            {
                case "up":
                    if (Canvas.GetTop(background) >= -315)
                    {
                        Canvas.SetTop(background, Canvas.GetTop(background) - backgroundSpeed);
                        foreach (var x in backgroundCanvas.Children.OfType<Image>())
                        {
                            if (x.Tag != null && (x.Tag.Equals("box") || x.Tag.Equals("ammoBox") || x.Tag.Equals("bullet") || x.Tag.Equals("zombie") || x.Tag.Equals("blood stain")))
                            {
                                Canvas.SetTop(x, Canvas.GetTop(x) - backgroundSpeed);
                            }
                        }
                    }
                    break;

                case "down":
                    if (Canvas.GetTop(background) <= -2.5)
                    {
                        Canvas.SetTop(background, Canvas.GetTop(background) + backgroundSpeed);
                        foreach (var x in backgroundCanvas.Children.OfType<Image>())
                        {
                            if (x.Tag != null && (x.Tag.Equals("box") || x.Tag.Equals("ammoBox") || x.Tag.Equals("bullet") || x.Tag.Equals("zombie") || x.Tag.Equals("blood stain")))
                            {
                                Canvas.SetTop(x, Canvas.GetTop(x) + backgroundSpeed);
                            }
                        }
                    }
                    break;

                case "right":
                    if (Canvas.GetLeft(background) < -2.5)
                    {
                        Canvas.SetLeft(background, Canvas.GetLeft(background) + backgroundSpeed);
                        foreach (var x in backgroundCanvas.Children.OfType<Image>())
                        {
                            if (x.Tag != null && (x.Tag.Equals("box") || x.Tag.Equals("ammoBox") || x.Tag.Equals("bullet") || x.Tag.Equals("zombie") || x.Tag.Equals("blood stain")))
                            {
                                Canvas.SetLeft(x, Canvas.GetLeft(x) + backgroundSpeed);
                            }
                        }
                    }
                    break;

                case "left":
                    if (Canvas.GetLeft(background) > -350)
                    {
                        Canvas.SetLeft(background, Canvas.GetLeft(background) - backgroundSpeed);
                        foreach (var x in backgroundCanvas.Children.OfType<Image>())
                        {
                            if (x.Tag != null && (x.Tag.Equals("box") || x.Tag.Equals("ammoBox") || x.Tag.Equals("bullet") || x.Tag.Equals("zombie") || x.Tag.Equals("blood stain")))
                            {
                                Canvas.SetLeft(x, Canvas.GetLeft(x) - backgroundSpeed);
                            }
                        }
                    }
                    break;
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Up:
                    moveUp = true;
                    break;

                case Key.Down:
                    moveDown = true;
                    break;

                case Key.Left:
                    moveLeft = true;
                    break;

                case Key.Right:
                    moveRight = true;
                    break;

                case Key.Space:
                    shoot(facing);
                    break;

            }

            movePlayer();
        }

        public void shoot(string direction)
        {
            if (actualAmmo > 0)
            {
                actualAmmo--;
                ammo.Content = actualAmmo;

                Bullet bullet = new Bullet();
                bullet.direction = direction;
                bullet.bulletLeft = (int)(Canvas.GetLeft(player) + (player.ActualWidth / 2));
                bullet.bulletTop = (int)(Canvas.GetTop(player) + (player.ActualHeight / 2));
                bullet.MakeBullet(backgroundCanvas);
            }
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Up:
                    moveUp = false;
                    break;

                case Key.Down:
                    moveDown = false;
                    break;

                case Key.Left:
                    moveLeft = false;
                    break;

                case Key.Right:
                    moveRight = false;
                    break;

            }
        }
    }
}
