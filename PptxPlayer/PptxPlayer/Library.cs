using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

namespace PptxPlayer
{
    public class Library
    {
        private bool rotating = false;
        private Storyboard rotaion = new Storyboard();
        private Image pptview = null;
        private Image ppt_prev = null;
        private Image ppt_next = null;
        private String [] m_urlArray = null;

        public void setViewer(Image viewer, Image prev, Image next)
        {
            pptview = viewer;
            ppt_prev = prev;
            ppt_next = next;
        }
        public void Rotate(String axis, ref Image target)
        {
            var bounds = ApplicationView.GetForCurrentView().VisibleBounds;
            var scaleFactor = DisplayInformation.GetForCurrentView().RawPixelsPerViewPixel;
            var size = new Size(bounds.Width * scaleFactor, bounds.Height * scaleFactor);
            

            if (rotating)
            {
                rotaion.Stop();
                rotating = false;
            }
            else
            {
                DoubleAnimation animation = new DoubleAnimation();
                animation.From = 0.0;
                animation.To = 360;
                animation.BeginTime = TimeSpan.FromSeconds(0);
                animation.RepeatBehavior = RepeatBehavior.Forever;
                Storyboard.SetTarget(animation, target);
                Storyboard.SetTargetProperty(animation, "(UIElement.Projection).(PlaneProjection.Rotation" + axis + ")");
                rotaion.Children.Clear();
                rotaion.Children.Add(animation);
                rotaion.Begin();
                rotating = true;


            }
        }

        public void Transition(String axis, ref Image target)
        {
            var bounds = ApplicationView.GetForCurrentView().VisibleBounds;
            var scaleFactor = DisplayInformation.GetForCurrentView().RawPixelsPerViewPixel;
            var size = new Size(bounds.Width * scaleFactor, bounds.Height * scaleFactor);

            if (rotating)
            {
                rotaion.Stop();
                rotating = false;
            }
            else
            {
                DoubleAnimation animation = new DoubleAnimation();
                animation.From = 0.0;
                if (axis == "X")
                    animation.To = size.Width;
                else
                    animation.To = size.Height;

                animation.BeginTime = TimeSpan.FromSeconds(1);
                animation.RepeatBehavior = RepeatBehavior.Forever;
                Storyboard.SetTarget(animation, target);
                Storyboard.SetTargetProperty(animation, "(UIElement.RenderTransform).(TranslateTransform." + axis + ")");
                rotaion.Children.Clear();
                rotaion.Children.Add(animation);
                rotaion.Begin();
                rotating = true;
            }
        }

        public void PrevTransition(String axis, ref Image target, ref Image Prev, ref Image Next, String[] url_array)
        {
            if (rotating)
            {
                stopAnimation(url_array, 0);
                return;
            }

            rotating = true;
            Prev.Visibility = Visibility.Visible;
            ppt_next.Source = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri(url_array[1]));

            var bounds = ApplicationView.GetForCurrentView().VisibleBounds;
            var scaleFactor = DisplayInformation.GetForCurrentView().RawPixelsPerViewPixel;
            var size = new Size(bounds.Width * scaleFactor, bounds.Height * scaleFactor);

            DoubleAnimation animation = new DoubleAnimation();
            animation.From = 0.0;
            if (axis == "X")
                animation.To = size.Width;
            else
                animation.To = size.Height;

            animation.BeginTime = TimeSpan.FromSeconds(0);

            animation.Completed += (sender, eArgs) =>
            {
                stopAnimation(url_array, 0);
            };

            Storyboard.SetTarget(animation, target);
            Storyboard.SetTargetProperty(animation, "(UIElement.RenderTransform).(TranslateTransform." + axis + ")");
            //rotaion.Children.Clear();
            //rotaion.Children.Add(animation);
            //rotaion.Begin();

            DoubleAnimation prev_animation = new DoubleAnimation();
            prev_animation.To = 0;
            if (axis == "X")
                prev_animation.From = -size.Width;
            else
                prev_animation.From = -size.Height;

            prev_animation.BeginTime = TimeSpan.FromSeconds(0);

            Storyboard.SetTarget(prev_animation, Prev);
            Storyboard.SetTargetProperty(prev_animation, "(UIElement.RenderTransform).(TranslateTransform." + axis + ")");
            rotaion.Children.Clear();
            rotaion.Children.Add(animation);
            rotaion.Children.Add(prev_animation);
            rotaion.Begin();
        }

        public void NextTransition(String axis, ref Image target, ref Image Prev, ref Image Next, String[] url_array)
        {
            if (rotating)
            {
                stopAnimation(url_array, 1);
                return;
            }

            ppt_prev.Source = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri(url_array[1]));

            rotating = true;
            Next.Visibility = Visibility.Visible;

            var bounds = ApplicationView.GetForCurrentView().VisibleBounds;
            var scaleFactor = DisplayInformation.GetForCurrentView().RawPixelsPerViewPixel;
            var size = new Size(bounds.Width * scaleFactor, bounds.Height * scaleFactor);

            DoubleAnimation animation = new DoubleAnimation();
            animation.From = 0.0;
            if (axis == "X")
                animation.To = -size.Width;
            else
                animation.To = -size.Height;

            animation.BeginTime = TimeSpan.FromSeconds(0);
            
            animation.Completed += (sender, eArgs) =>
            {
                stopAnimation(url_array, 1);
            };
            
            Storyboard.SetTarget(animation, target);
            Storyboard.SetTargetProperty(animation, "(UIElement.RenderTransform).(TranslateTransform." + axis + ")");
            //rotaion.Children.Clear();
            //rotaion.Children.Add(animation);
            //rotaion.Begin();

            DoubleAnimation next_animation = new DoubleAnimation();
            next_animation.From = size.Width;
            if (axis == "X")
                next_animation.To = 0;
            else
                next_animation.To = 0;

            next_animation.BeginTime = TimeSpan.FromSeconds(0);

            Storyboard.SetTarget(next_animation, Next);
            Storyboard.SetTargetProperty(next_animation, "(UIElement.RenderTransform).(TranslateTransform." + axis + ")");
            rotaion.Children.Clear();
            rotaion.Children.Add(animation);
            rotaion.Children.Add(next_animation);
            rotaion.Begin();
        }

        private void stopAnimation(String [] url_array, int dir)
        {
            if( dir == 0 )  // prev
            {
                ppt_prev.Visibility = Visibility.Collapsed;
                ppt_next.Visibility = Visibility.Visible;                
            }

            if (dir == 1)  // next
            {
                ppt_prev.Visibility = Visibility.Visible;
                ppt_next.Visibility = Visibility.Collapsed;               
            }

            m_urlArray = url_array;
            
            rotaion.Stop();

            pptview.Visibility = Visibility.Collapsed;
            
            pptview.Source = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri(url_array[1]));
            pptview.ImageOpened += Pptview_ImageOpened;
            
            rotating = false;
            return;
        }

        private void Pptview_ImageOpened(object sender, RoutedEventArgs e)
        {
            ppt_prev.Visibility = Visibility.Collapsed;
            ppt_next.Visibility = Visibility.Collapsed;
            pptview.Visibility = Visibility.Visible;
            ppt_prev.Source = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri(m_urlArray[0]));
            ppt_next.Source = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri(m_urlArray[2]));
        }
    }
}
