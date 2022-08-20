using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Killing_It
{
    internal class Bullet
    {
        public int mousePosX, mousePosY;
        public int bulletLeft, bulletTop;
        Canvas canvas;
        public double angleBetween = 0;


        private int speed = 4;
        private Image bullet = new Image();
        private List<Image> bloodStains = new List<Image>();
        private DispatcherTimer bulletTimer = new DispatcherTimer();
        private DispatcherTimer bloodStainRemoveTimer = new DispatcherTimer();

        public void MakeBullet(Canvas canvas)
        {
            this.canvas = canvas;
            bullet.Source = new BitmapImage(new Uri("\\Images\\bullet.png", UriKind.RelativeOrAbsolute));

            bullet.Height = 10;
            bullet.Width = 10;
            bullet.RenderTransformOrigin = new Point(0.5, 0.5);

            Canvas.SetLeft(bullet, bulletLeft);
            Canvas.SetTop(bullet, bulletTop);
            bullet.Tag = "bullet";

            RotateTransform rotateTransform = new RotateTransform();
            rotateTransform.Angle = angleBetween;
            bullet.RenderTransform = rotateTransform;

            canvas.Children.Add(bullet);

            bulletTimer.Tick += moveBullet;
            bulletTimer.Interval = TimeSpan.FromMilliseconds(2);
            bulletTimer.Start();

        }

        private void moveBullet(object sender, EventArgs e)
        {
            RotateTransform rotateTransform = new RotateTransform();
            if (Canvas.GetLeft(bullet) < mousePosX)
            {
                Canvas.SetLeft(bullet, Canvas.GetLeft(bullet) + speed);
            }

            if (Canvas.GetLeft(bullet) > mousePosX)
            {
                Canvas.SetLeft(bullet, Canvas.GetLeft(bullet) - speed);
            }

            if (Canvas.GetTop(bullet) > mousePosY)
            {
                Canvas.SetTop(bullet, Canvas.GetTop(bullet) - speed);
            }

            if (Canvas.GetTop(bullet) < mousePosY)
            {
                Canvas.SetTop(bullet, Canvas.GetTop(bullet) + speed);
            }

            if (Canvas.GetTop(bullet) < 0 || Canvas.GetTop(bullet) + bullet.ActualHeight > canvas.ActualHeight ||
                Canvas.GetLeft(bullet) < 0 || Canvas.GetLeft(bullet) + bullet.ActualWidth > canvas.ActualWidth || bullet.Source == null)
            {
                bulletTimer.Stop();
                canvas.Children.Remove(bullet);
                bullet = null;
            }
        }
    }
}
