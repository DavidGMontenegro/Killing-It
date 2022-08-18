using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Threading;

namespace Killing_It
{
    internal class Zombie
    {
        public int playerPosX, playerPosY;
        Canvas canvas;
        Random rand = new Random();

        private int speed = 4;
        private Image zombie = new Image();
        private DispatcherTimer zombieTimer = new DispatcherTimer();

        public void MakeZombie(Canvas canvas)
        {
            int posX, posY;
            this.canvas = canvas;
            zombie.Source = new BitmapImage(new Uri("\\Images\\zombie.gif", UriKind.RelativeOrAbsolute));

            zombie.Height = 45;
            zombie.Width = 45;

            posX = rand.Next(0, (int)canvas.ActualWidth - (int)zombie.ActualWidth);
            posY = rand.Next(0, (int)canvas.ActualHeight - (int)zombie.ActualHeight);


            Canvas.SetLeft(zombie, posX);
            Canvas.SetTop(zombie, posY);
            zombie.Tag = "zombie";

            canvas.Children.Add(zombie);

            zombieTimer.Tick += moveZombie;
            zombieTimer.Interval = TimeSpan.FromSeconds(0.25);
            zombieTimer.Start();

        }

        private void moveZombie(object sender, EventArgs e)
        {
            RotateTransform rotateTransform = new RotateTransform(0);
            if (Canvas.GetLeft(zombie) > playerPosX)
            {
                Canvas.SetLeft(zombie, Canvas.GetLeft(zombie) - speed);
                zombie.RenderTransform = rotateTransform;
                rotateTransform.Angle = 180;
                zombie.RenderTransform = rotateTransform;
            }

            if (Canvas.GetLeft(zombie) < playerPosX)
            {
                rotateTransform.Angle = 0;
                Canvas.SetLeft(zombie, Canvas.GetLeft(zombie) + speed);
                zombie.RenderTransform = rotateTransform;
            }

            if (Canvas.GetTop(zombie) > playerPosY)
            {
                rotateTransform.Angle = 0;
                Canvas.SetTop(zombie, Canvas.GetTop(zombie) - speed);
                zombie.RenderTransform = rotateTransform;
                rotateTransform.Angle = -90;
                zombie.RenderTransform = rotateTransform;
            }

            if (Canvas.GetLeft(zombie) < playerPosY)
            {
                rotateTransform.Angle = 0;
                Canvas.SetLeft(zombie, Canvas.GetLeft(zombie) + speed);
                zombie.RenderTransform = rotateTransform;
                rotateTransform.Angle = 90;
                zombie.RenderTransform = rotateTransform;
            }


        }
    }
}
