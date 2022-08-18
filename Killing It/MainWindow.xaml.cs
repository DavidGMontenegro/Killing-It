using System;
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

        Random rand = new Random();

        bool moveUp, moveDown, moveLeft, moveRight, spawnedAmmo = false;
        string facing = "right";
        double playerSpeed = 5;
        double backgroundSpeed = 4;
        int maxAmmo = 30;
        int actualAmmo, ammoBoxBonus = 5;
        int posX, posY;

        public MainWindow()
        {
            InitializeComponent();
            actualAmmo = 20;
            ammo.Content = actualAmmo;

            spawnAmmoTimer.Tick += spawnAmmo;
            spawnAmmoTimer.Interval = TimeSpan.FromSeconds(10);
            spawnAmmoTimer.Start();
        }

        private void spawnAmmo(object sender, EventArgs e)
        {
            if (actualAmmo < maxAmmo && !spawnedAmmo)
            {
                liveProgressBar.Value -= 25;
                Image ammoImage = new Image();
                ammoImage.Source = new BitmapImage(new Uri("\\Images\\ammo Box.png", UriKind.RelativeOrAbsolute));

                ammoImage.Height = 45;
                ammoImage.Width = 45;

                posX = rand.Next((int)Canvas.GetLeft(background) + 10, (int)(Canvas.GetLeft(background) + background.ActualWidth - 10));
                posY = rand.Next((int)Canvas.GetTop(background) + 10, (int)(Canvas.GetTop(background) + background.ActualHeight - 10));

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
                    }
                    spawnedAmmo = false;
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
            if (moveDown && Canvas.GetTop(player) + player.ActualHeight < backgroundCanvas.ActualHeight - 15)
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
                            if (x.Tag != null && (x.Tag.Equals("box") || x.Tag.Equals("ammoBox") || x.Tag.Equals("bullet")))
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
                            if (x.Tag != null && (x.Tag.Equals("box") || x.Tag.Equals("ammoBox") || x.Tag.Equals("bullet")))
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
                            if (x.Tag != null && (x.Tag.Equals("box") || x.Tag.Equals("ammoBox") || x.Tag.Equals("bullet")))
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
                            if (x.Tag != null && (x.Tag.Equals("box") || x.Tag.Equals("ammoBox") || x.Tag.Equals("bullet")))
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
