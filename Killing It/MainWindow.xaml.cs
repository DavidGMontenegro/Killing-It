using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Threading;

namespace Killing_It
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer gameTimer = new DispatcherTimer();
        RotateTransform rotateTransform = new RotateTransform();

        Random rand = new Random();
        bool goUp, goLeft, goRight, goDown;
        int playerSpeed = 4;
        int backgroundSpeed = 8;
        double zombieSpeed = 2;
        int ammo = 40;
        int score = 0;
        int actualAllowedZombies = 5;
        int zombieNumber = 0;
        int spawnedAmmo = 0;
        int spawnedLiveKits = 0;

        List<Image> toDelete = new List<Image>();


        public MainWindow()
        {
            InitializeComponent();
            spawnZombies();
            zombieNumber += 1;

            gameTimer.Tick += GameTimerEvent;
            gameTimer.Interval = TimeSpan.FromMilliseconds(20);
            gameTimer.Start();
        }

        private void GameTimerEvent(object sender, EventArgs e)
        {
            AmmoLabel.Content = ammo;
            ScoreLabel.Content = "Score: " + score;

            if (liveProgressBar.Value == 0)
            {
                gameTimer.Stop();
                MessageBoxResult result = MessageBox.Show("  GAME OVER  ", "Game over", MessageBoxButton.OK);
                return;
            }
            else if (liveProgressBar.Value < 75 && spawnedLiveKits < 4)
            {
                spawnLive();

            }

            if (goLeft == true && Canvas.GetLeft(player) > Canvas.GetLeft(background))
            {
                Canvas.SetLeft(player, Canvas.GetLeft(player) - playerSpeed);
                rotateTransform.Angle = 0;
                player.RenderTransform = rotateTransform;
                rotateTransform.Angle = 180;
                player.RenderTransform = rotateTransform;
                if (!moveBackground("right"))
                {
                    Canvas.SetLeft(player, Canvas.GetLeft(player) - playerSpeed);
                }

                foreach (var x in backgroundCanvas.Children.OfType<Image>())
                {
                    if (x.Tag != null)
                    {
                        if (x.Tag.Equals("box"))
                        {
                            Rect boxRect = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);
                            Rect playerRect = new Rect(Canvas.GetLeft(player), Canvas.GetTop(player), player.Width, player.Height);
                            if (boxRect.IntersectsWith(playerRect))
                            {
                                Canvas.SetLeft(player, Canvas.GetLeft(player) + 2 * playerSpeed);
                                moveBackground("left");
                                break;
                            }
                        }
                    }
                }
            }
            if (goRight == true && Canvas.GetLeft(player) + player.ActualWidth < Canvas.GetLeft(background) + background.ActualWidth)
            {
                Canvas.SetLeft(player, Canvas.GetLeft(player) + playerSpeed);
                rotateTransform.Angle = 0;
                player.RenderTransform = rotateTransform;
                if (!moveBackground("left"))
                {
                    Canvas.SetLeft(player, Canvas.GetLeft(player) + playerSpeed);
                }

                foreach (var x in backgroundCanvas.Children.OfType<Image>())
                {
                    if (x.Tag != null)
                    {
                        if (x.Tag.Equals("box"))
                        {
                            Rect boxRect = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);
                            Rect playerRect = new Rect(Canvas.GetLeft(player), Canvas.GetTop(player), player.Width, player.Height);
                            if (boxRect.IntersectsWith(playerRect))
                            {
                                Canvas.SetLeft(player, Canvas.GetLeft(player) - 2 * playerSpeed);
                                moveBackground("right");
                                break;
                            }
                        }
                    }
                }
            }
            if (goUp == true && Canvas.GetTop(player) > Canvas.GetTop(background))
            {
                Canvas.SetTop(player, Canvas.GetTop(player) - playerSpeed);
                rotateTransform.Angle = 0;
                player.RenderTransform = rotateTransform;
                rotateTransform.Angle = -90;
                player.RenderTransform = rotateTransform;
                if (!moveBackground("down"))
                {
                    Canvas.SetTop(player, Canvas.GetTop(player) - playerSpeed);
                }

                foreach (var x in backgroundCanvas.Children.OfType<Image>())
                {
                    if (x.Tag != null)
                    {
                        if (x.Tag.Equals("box"))
                        {
                            Rect boxRect = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);
                            Rect playerRect = new Rect(Canvas.GetLeft(player), Canvas.GetTop(player), player.Width, player.Height);
                            if (boxRect.IntersectsWith(playerRect))
                            {
                                Canvas.SetTop(player, Canvas.GetTop(player) + 2 * playerSpeed);
                                moveBackground("up");
                                break;
                            }
                        }
                    }
                }
            }
            if (goDown == true && Canvas.GetTop(player) + player.Height < Canvas.GetTop(background) + background.ActualHeight)
            {
                Canvas.SetTop(player, Canvas.GetTop(player) + playerSpeed);
                rotateTransform.Angle = 0;
                player.RenderTransform = rotateTransform;
                rotateTransform.Angle = 090;
                player.RenderTransform = rotateTransform;
                if (!moveBackground("up"))
                {
                    Canvas.SetTop(player, Canvas.GetTop(player) + playerSpeed);
                }

                foreach (var x in backgroundCanvas.Children.OfType<Image>())
                {
                    if (x.Tag != null)
                    {
                        if (x.Tag.Equals("box"))
                        {
                            Rect boxRect = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);
                            Rect playerRect = new Rect(Canvas.GetLeft(player), Canvas.GetTop(player), player.Width, player.Height);
                            if (boxRect.IntersectsWith(playerRect))
                            {
                                Canvas.SetTop(player, Canvas.GetTop(player) - 2 * playerSpeed);
                                moveBackground("down");
                                break;
                            }
                        }
                    }
                }
            }

            foreach (var x in backgroundCanvas.Children.OfType<Image>())
            {
                if (x.Tag != null)
                {
                    if (x.Tag.Equals("ammo box"))
                    {
                        Rect playerRect = new Rect(Canvas.GetLeft(player), Canvas.GetTop(player), player.Width, player.Height);
                        Rect ammoBoxRect = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width - 10, x.Height - 10);

                        if (playerRect.IntersectsWith(ammoBoxRect))
                        {
                            x.Tag = null;
                            toDelete.Add(x);
                            ammo += 15;
                            spawnedAmmo--;
                        }
                    }
                    else if (x.Tag.Equals("first aid kit"))
                    {
                        Rect playerRect = new Rect(Canvas.GetLeft(player), Canvas.GetTop(player), player.Width, player.Height);
                        Rect kitRect = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width - 10, x.Height - 10);

                        if (playerRect.IntersectsWith(kitRect))
                        {
                            x.Tag = null;
                            toDelete.Add(x);
                            liveProgressBar.Value += 10;
                            spawnedLiveKits--;
                        }
                    }
                    else if (x.Tag.Equals("blood stain"))
                    {
                        x.Opacity -= 0.01;
                    }
                    else if (x.Tag.Equals("zombie"))
                    {
                        Rect playerRect = new Rect(Canvas.GetLeft(player), Canvas.GetTop(player), player.Width - 10, player.Height - 10);
                        Rect zombieRect = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width - 20, x.Height - 20);

                        if (playerRect.IntersectsWith(zombieRect))
                        {
                            liveProgressBar.Value -= 1;
                        }

                        RotateTransform rotateTransform = new RotateTransform();
                        if (Canvas.GetLeft(x) < Canvas.GetLeft(player))
                        {
                            rotateTransform.Angle = 0;
                            Canvas.SetLeft(x, Canvas.GetLeft(x) + zombieSpeed);
                            x.RenderTransform = rotateTransform;
                            foreach (var box in backgroundCanvas.Children.OfType<Image>())
                            {
                                if (box.Tag != null)
                                {
                                    if (box.Tag.Equals("box"))
                                    {
                                        Rect boxRect = new Rect(Canvas.GetLeft(box), Canvas.GetTop(box), box.Width, box.Height);
                                        if (boxRect.IntersectsWith(zombieRect))
                                        {
                                            Canvas.SetLeft(x, Canvas.GetLeft(x) - 3 * zombieSpeed);
                                            break;
                                        }
                                    }
                                }
                            }
                        }

                        if (Canvas.GetLeft(x) > Canvas.GetLeft(player))
                        {
                            rotateTransform.Angle = 0;
                            Canvas.SetLeft(x, Canvas.GetLeft(x) - zombieSpeed);
                            x.RenderTransform = rotateTransform;
                            rotateTransform.Angle = 180;
                            x.RenderTransform = rotateTransform;
                            foreach (var box in backgroundCanvas.Children.OfType<Image>())
                            {
                                if (box.Tag != null)
                                {
                                    if (box.Tag.Equals("box"))
                                    {
                                        Rect boxRect = new Rect(Canvas.GetLeft(box), Canvas.GetTop(box), box.Width, box.Height);
                                        if (boxRect.IntersectsWith(zombieRect))
                                        {
                                            Canvas.SetLeft(x, Canvas.GetLeft(x) + 3 * zombieSpeed);
                                            break;
                                        }
                                    }
                                }
                            }
                        }

                        if (Canvas.GetTop(x) > Canvas.GetTop(player))
                        {
                            rotateTransform.Angle = 0;
                            Canvas.SetTop(x, Canvas.GetTop(x) - zombieSpeed);
                            x.RenderTransform = rotateTransform;
                            rotateTransform.Angle = -90;
                            x.RenderTransform = rotateTransform;
                            foreach (var box in backgroundCanvas.Children.OfType<Image>())
                            {
                                if (box.Tag != null)
                                {
                                    if (box.Tag.Equals("box"))
                                    {
                                        Rect boxRect = new Rect(Canvas.GetLeft(box), Canvas.GetTop(box), box.Width, box.Height);
                                        if (boxRect.IntersectsWith(zombieRect))
                                        {
                                            Canvas.SetTop(x, Canvas.GetTop(x) + 3 * zombieSpeed);
                                            break;
                                        }
                                    }
                                }
                            }
                        }

                        if (Canvas.GetTop(x) < Canvas.GetTop(player))
                        {
                            rotateTransform.Angle = 0;
                            Canvas.SetTop(x, Canvas.GetTop(x) + zombieSpeed);
                            x.RenderTransform = rotateTransform;
                            rotateTransform.Angle = 90;
                            x.RenderTransform = rotateTransform;
                            foreach (var box in backgroundCanvas.Children.OfType<Image>())
                            {
                                if (box.Tag != null)
                                {
                                    if (box.Tag.Equals("box"))
                                    {
                                        Rect boxRect = new Rect(Canvas.GetLeft(box), Canvas.GetTop(box), box.Width, box.Height);
                                        if (boxRect.IntersectsWith(zombieRect))
                                        {
                                            Canvas.SetLeft(x, Canvas.GetLeft(x) - 3 * zombieSpeed);
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else if (x.Tag.Equals("bullet"))
                    {
                        foreach (var y in backgroundCanvas.Children.OfType<Image>())
                        {
                            if (y.Tag != null && x.Tag != null)
                            {
                                if (y.Tag.Equals("zombie"))
                                {
                                    Rect zombieRect = new Rect(Canvas.GetLeft(y), Canvas.GetTop(y), y.Width, y.Height);
                                    Rect bulletRec = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);

                                    if (bulletRec.IntersectsWith(zombieRect))
                                    {
                                        score++;

                                        y.Source = new BitmapImage(new Uri("\\Images\\blood stain.png", UriKind.RelativeOrAbsolute));
                                        y.Tag = "blood stain";
                                        toDelete.Add(y);
                                        zombieNumber--;
                                        x.Tag = null;
                                        toDelete.Add(x);
                                    }
                                }
                                else if (y.Tag.Equals("box"))
                                {
                                    Rect boxRect = new Rect(Canvas.GetLeft(y), Canvas.GetTop(y), y.Width, y.Height);
                                    Rect bulletRec = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);

                                    if (bulletRec.IntersectsWith(boxRect))
                                    {
                                        x.Tag = null;
                                        toDelete.Add(x);
                                    }
                                }

                            }
                        }
                    }
                }
            }

            int numToDelete = toDelete.Count;
            for (int i = 0; i < numToDelete; i++)
            {
                if (toDelete[i].Tag != null)
                {
                    if (toDelete[i].Tag.Equals("blood stain"))
                    {
                        if (zombieNumber < actualAllowedZombies)
                        {
                            spawnZombies();
                            spawnZombies();
                            spawnZombies();
                            spawnZombies();
                            zombieNumber += 4;
                        }

                        if (toDelete[i].Opacity == 0)
                        {
                            backgroundCanvas.Children.Remove(toDelete[i]);
                        }
                    }
                }
                else
                {
                    backgroundCanvas.Children.Remove(toDelete[i]);
                }
            }

            if (score / 2 > actualAllowedZombies)
            {
                actualAllowedZombies += 1;
            }
        }

        private void spawnZombies()
        {
            Image zombie = new Image();
            int posX, posY;

            zombie.Tag = "zombie";
            zombie.Source = new BitmapImage(new Uri("\\Images\\zombie.gif", UriKind.RelativeOrAbsolute));
            zombie.RenderTransformOrigin = new Point(0.5, 0.5);

            zombie.Height = 60;
            zombie.Width = 60;

            do
            {
                posX = rand.Next(0, (int)backgroundCanvas.ActualWidth);
                posY = rand.Next(0, (int)backgroundCanvas.ActualHeight);
            } while (checkSpawnPoint(posX, posY, zombie));


            Canvas.SetLeft(zombie, posX);
            Canvas.SetTop(zombie, posY);

            backgroundCanvas.Children.Add(zombie);
        }

        private void spawnAmmo()
        {
            Image ammoBox = new Image();
            int posX, posY;

            ammoBox.Tag = "ammo box";
            ammoBox.Source = new BitmapImage(new Uri("\\Images\\ammo box.png", UriKind.RelativeOrAbsolute));

            ammoBox.Height = 40;
            ammoBox.Width = 40;

            do
            {
                posX = rand.Next(0, (int)backgroundCanvas.ActualWidth);
                posY = rand.Next(0, (int)backgroundCanvas.ActualHeight);
            } while (checkSpawnPoint(posX, posY, ammoBox));
            Canvas.SetLeft(ammoBox, posX);
            Canvas.SetTop(ammoBox, posY);

            backgroundCanvas.Children.Add(ammoBox);
            spawnedAmmo++;
        }

        private void spawnLive()
        {
            Image live = new Image();
            int posX, posY;

            live.Tag = "first aid kit";
            live.Source = new BitmapImage(new Uri("\\Images\\first aid kit.png", UriKind.RelativeOrAbsolute));

            live.Height = 40;
            live.Width = 40;

            do
            {
                posX = rand.Next(0, (int)backgroundCanvas.ActualWidth);
                posY = rand.Next(0, (int)backgroundCanvas.ActualHeight);
            } while (checkSpawnPoint(posX, posY, live));
            Canvas.SetLeft(live, posX);
            Canvas.SetTop(live, posY);

            backgroundCanvas.Children.Add(live);
            spawnedLiveKits++;
        }


        private bool moveBackground(string direction)
        {

            switch (direction)
            {
                case "up":
                    if (Canvas.GetTop(background) >= (backgroundCanvas.ActualHeight - background.ActualHeight))
                    {
                        Canvas.SetTop(background, Canvas.GetTop(background) - backgroundSpeed);
                        foreach (var x in backgroundCanvas.Children.OfType<Image>())
                        {
                            if (x.Tag != null && (x.Tag.Equals("box") || x.Tag.Equals("ammo box") || x.Tag.Equals("zombie") || x.Tag.Equals("first aid kit") || x.Tag.Equals("blood stain")))
                            {
                                Canvas.SetTop(x, Canvas.GetTop(x) - backgroundSpeed);
                            }
                        }
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                case "down":
                    if (Canvas.GetTop(background) <= -2.5)
                    {
                        Canvas.SetTop(background, Canvas.GetTop(background) + backgroundSpeed);
                        foreach (var x in backgroundCanvas.Children.OfType<Image>())
                        {
                            if (x.Tag != null && (x.Tag.Equals("box") || x.Tag.Equals("ammo box") || x.Tag.Equals("zombie") || x.Tag.Equals("first aid kit") || x.Tag.Equals("blood stain")))
                            {
                                Canvas.SetTop(x, Canvas.GetTop(x) + backgroundSpeed);
                            }
                        }
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                case "right":
                    if (Canvas.GetLeft(background) < -2.5)
                    {
                        Canvas.SetLeft(background, Canvas.GetLeft(background) + backgroundSpeed);
                        foreach (var x in backgroundCanvas.Children.OfType<Image>())
                        {
                            if (x.Tag != null && (x.Tag.Equals("box") || x.Tag.Equals("ammo box") || x.Tag.Equals("zombie") || x.Tag.Equals("first aid kit") || x.Tag.Equals("blood stain")))
                            {
                                Canvas.SetLeft(x, Canvas.GetLeft(x) + backgroundSpeed);
                            }
                        }
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                case "left":
                    if (Canvas.GetLeft(background) > (backgroundCanvas.ActualWidth - background.ActualWidth))
                    {
                        Canvas.SetLeft(background, Canvas.GetLeft(background) - backgroundSpeed);
                        foreach (var x in backgroundCanvas.Children.OfType<Image>())
                        {
                            if (x.Tag != null && (x.Tag.Equals("box") || x.Tag.Equals("ammo box") || x.Tag.Equals("zombie") || x.Tag.Equals("first aid kit") || x.Tag.Equals("blood stain")))
                            {
                                Canvas.SetLeft(x, Canvas.GetLeft(x) - backgroundSpeed);
                            }
                        }
                        return true;
                    }
                    else
                    {
                        return false;
                    }
            }
            return false;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.W:
                    goUp = true;
                    break;

                case Key.S:
                    goDown = true;
                    break;

                case Key.A:
                    goLeft = true;
                    break;

                case Key.D:
                    goRight = true;
                    break;
            }
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.W:
                    goUp = false;
                    break;

                case Key.S:
                    goDown = false;
                    break;

                case Key.A:
                    goLeft = false;
                    break;

                case Key.D:
                    goRight = false;
                    break;

            }
        }

        private void Window_MouseLeftButtonUp(object sender, MouseButtonEventArgs mouse)
        {
            if (ammo > 0)
            {
                Point mousePosition = mouse.GetPosition(this);
                Shoot((int)mousePosition.X, (int)mousePosition.Y);
            }

            if (ammo < 15 && spawnedAmmo < 6)
            {
                spawnAmmo();
                spawnAmmo();
            }
        }

        public void Shoot(int posX, int posY)
        {
            ammo--;

            Bullet bullet = new Bullet();
            bullet.mousePosX = posX;
            bullet.mousePosY = posY;

            double radians = Math.Atan2(posY - Canvas.GetTop(player), posX - Canvas.GetLeft(player));

            double angleBetween = radians * (180 / Math.PI);

            RotateTransform rotateTransform = new RotateTransform();
            rotateTransform.Angle = angleBetween;
            player.RenderTransform = rotateTransform;
            bullet.angleBetween = angleBetween;

            bullet.bulletLeft = (int)(Canvas.GetLeft(player) + (player.ActualWidth / 2));
            bullet.bulletTop = (int)(Canvas.GetTop(player) + (player.ActualHeight / 2));
            bullet.MakeBullet(backgroundCanvas);
        }

        public bool checkSpawnPoint(int posX, int posY, Image toCheck)
        {
            Rect toCheckRect = new Rect(posX, posY, toCheck.Width, toCheck.Height);
            Rect playerRect = new Rect(Canvas.GetLeft(player), Canvas.GetTop(player), player.Width, player.Height);
            foreach (var box in backgroundCanvas.Children.OfType<Image>())
            {
                if (box.Tag != null)
                {
                    Rect bulletRec = new Rect(Canvas.GetLeft(box), Canvas.GetTop(box), box.Width, box.Height);
                    if (box.Tag.Equals("box"))
                    {
                        Rect boxRect = new Rect(Canvas.GetLeft(box), Canvas.GetTop(box), box.Width, box.Height);
                        if (boxRect.IntersectsWith(toCheckRect) || boxRect.IntersectsWith(playerRect))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }
}
