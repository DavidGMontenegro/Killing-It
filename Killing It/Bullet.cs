using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Killing_It
{
    internal class Bullet
    {
        public string direction;
        public int bulletLeft, bulletTop;
        Canvas canvas;

        private int speed = 6;
        private Image bullet = new Image();
        private DispatcherTimer bulletTimer = new DispatcherTimer();

        public void MakeBullet(Canvas canvas)
        {
            this.canvas = canvas;
            bullet.Source = new BitmapImage(new Uri("\\Images\\bullet.png", UriKind.RelativeOrAbsolute));

            bullet.Height = 30;
            bullet.Width = 30;

            Canvas.SetLeft(bullet, bulletLeft);
            Canvas.SetTop(bullet, bulletTop);
            bullet.Tag = "bullet";

            canvas.Children.Add(bullet);

            bulletTimer.Tick += moveBullet;
            bulletTimer.Interval = TimeSpan.FromMilliseconds(speed);
            bulletTimer.Start();

        }

        private void moveBullet(object sender, EventArgs e)
        {
            RotateTransform rotateTransform = new RotateTransform(0);

            switch (direction)
            {

                case "up":
                    bullet.RenderTransform = rotateTransform;
                    rotateTransform.Angle = -90;
                    bullet.RenderTransform = rotateTransform;
                    Canvas.SetTop(bullet, Canvas.GetTop(bullet) - speed);
                    break;

                case "down":
                    bullet.RenderTransform = rotateTransform;
                    rotateTransform.Angle = 90;
                    bullet.RenderTransform = rotateTransform;
                    Canvas.SetTop(bullet, Canvas.GetTop(bullet) + speed);
                    break;

                case "left":
                    bullet.RenderTransform = rotateTransform;
                    rotateTransform.Angle = 180;
                    bullet.RenderTransform = rotateTransform;
                    Canvas.SetLeft(bullet, Canvas.GetLeft(bullet) - speed);
                    break;

                case "right":
                    bullet.RenderTransform = rotateTransform;
                    Canvas.SetLeft(bullet, Canvas.GetLeft(bullet) + speed);
                    break;
            }

            if (Canvas.GetTop(bullet) < 0 || Canvas.GetTop(bullet) + bullet.ActualHeight > canvas.ActualHeight ||
                Canvas.GetLeft(bullet) < 0 || Canvas.GetLeft(bullet) + bullet.ActualWidth > canvas.ActualWidth)
            {
                bulletTimer.Stop();
                canvas.Children.Remove(bullet);
                bullet = null;
            }
        }
    }
}
