﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

namespace PptxPlayer
{
    public class Library
    {
        private bool rotating = false;
        private Storyboard rotaion = new Storyboard();
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
                animation.BeginTime = TimeSpan.FromSeconds(1);
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
    }
}
