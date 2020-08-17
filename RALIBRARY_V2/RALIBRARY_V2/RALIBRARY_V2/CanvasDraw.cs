using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Shapes;

namespace RALIBRARY_V2
{
    public class CanvasDraw
    {
        public void AddimgToCanvasPath(Canvas canvas, string path)
        {
            Image image = new Image();
            var bitmapImage = new BitmapImage();
            image.Source = bitmapImage;
            bitmapImage.UriSource = new Uri(path, UriKind.RelativeOrAbsolute);

            canvas.Children.Clear();

            var bounds = Window.Current.Bounds;
            double height = bounds.Height;
            double width = bounds.Width;

            Canvas.SetLeft(image, 0);
            Canvas.SetTop(image, 0);
            canvas.Children.Add(image);
        }

        public void CreateCircle(Canvas canvas, PointerRoutedEventArgs e, double size, Color cor)
        {
            Ellipse myEllipse = new Ellipse();
            //myEllipse.StrokeThickness = 2; // tamanho borda
            //myEllipse.Stroke = Brushes.White; // cor borda
            myEllipse.Stroke = new SolidColorBrush(cor);
            myEllipse.Fill = new SolidColorBrush(cor);
            myEllipse.Width = size;
            myEllipse.Height = size;
            Canvas.SetLeft(myEllipse, e.GetCurrentPoint(canvas).Position.X - (size / 2));
            Canvas.SetTop(myEllipse, e.GetCurrentPoint(canvas).Position.Y - (size / 2));
            canvas.Children.Add(myEllipse);
        }

        public void CreateCirclePosition(Canvas canvas, PointerRoutedEventArgs e, double size, Color cor, double x, double y)
        {
            Ellipse myEllipse = new Ellipse();
            //myEllipse.StrokeThickness = 2; // tamanho borda
            //myEllipse.Stroke = Brushes.White; // cor borda
            myEllipse.Stroke = new SolidColorBrush(cor);
            myEllipse.Fill = new SolidColorBrush(cor);
            myEllipse.Width = size;
            myEllipse.Height = size;
            Canvas.SetLeft(myEllipse, x - (size / 2));
            Canvas.SetTop(myEllipse, y - (size / 2));
            canvas.Children.Add(myEllipse);
        }

        public void CreateSquare(Canvas canvas, PointerRoutedEventArgs e, double size, Color cor)
        {
            Rectangle myrectangle = new Rectangle();
            myrectangle.Width = size;
            myrectangle.Height = size;
            myrectangle.Stroke = new SolidColorBrush(cor);
            myrectangle.Fill = new SolidColorBrush(cor);
            Canvas.SetLeft(myrectangle, e.GetCurrentPoint(canvas).Position.X - (size / 2));
            Canvas.SetTop(myrectangle, e.GetCurrentPoint(canvas).Position.Y - (size / 2));
            canvas.Children.Add(myrectangle);
        }

        public void CreateSquarePosition(Canvas canvas, PointerRoutedEventArgs e, double size, Color cor, double x, double y)
        {
            Rectangle myrectangle = new Rectangle();
            myrectangle.Width = size;
            myrectangle.Height = size;
            myrectangle.Stroke = new SolidColorBrush(cor);
            myrectangle.Fill = new SolidColorBrush(cor);
            Canvas.SetLeft(myrectangle, x - (size / 2));
            Canvas.SetTop(myrectangle, y - (size / 2));
            canvas.Children.Add(myrectangle);
        }

        public bool first_pick { get; set; }
        private double x_first_pick = 40, y_first_pick = 40;

        public void CreateLine(Canvas canvas, double size, PointerRoutedEventArgs e, Color cor)
        {
            first_pick = !first_pick;
            if (!first_pick)
            {
                Line line = new Line();
                canvas.Children.Remove(line);
                line.X1 = x_first_pick;
                line.X2 = e.GetCurrentPoint(canvas).Position.X;
                line.Y1 = y_first_pick;
                line.Y2 = e.GetCurrentPoint(canvas).Position.Y;
                line.StrokeThickness = size;
                line.Stroke = new SolidColorBrush(cor);
                line.Fill = new SolidColorBrush(cor);
                canvas.Children.Add(line);
            }
            else
            {
                x_first_pick = e.GetCurrentPoint(canvas).Position.X;
                y_first_pick = e.GetCurrentPoint(canvas).Position.Y;
            }
        }

        public void CreateLinePosition(Canvas canvas, double size, PointerRoutedEventArgs e, Color cor, double x1, double x2, double y1, double y2)
        {
            Line line = new Line();
            canvas.Children.Remove(line);
            line.X1 = x1;
            line.X2 = x2;
            line.Y1 = y1;
            line.Y2 = y2;
            line.StrokeThickness = size;
            line.Stroke = new SolidColorBrush(cor);
            line.Fill = new SolidColorBrush(cor);
            canvas.Children.Add(line);
        }

        public void CreateText(string val, Canvas canvas, double size, PointerRoutedEventArgs e, Color cor, double x, double y)
        {
            TextBlock label = new TextBlock();
            label.HorizontalAlignment = HorizontalAlignment.Stretch;
            label.FontSize = size;
            label.Text = val;
            label.VerticalAlignment = VerticalAlignment.Stretch;
            label.Foreground = new SolidColorBrush(cor);
            Canvas.SetLeft(label, x);
            Canvas.SetTop(label, y);
            canvas.Children.Add(label);
        }


        private Shape cursor;
        public void Cursor_Circle(Canvas canvas, double size, PointerRoutedEventArgs e, Windows.UI.Color mysolidColorBrush)
        {
            if (canvas.Children.Contains(cursor)) canvas.Children.Remove(cursor); if (canvas.Children.Contains(fallower_line)) canvas.Children.Remove(fallower_line);
            cursor = new Ellipse();
            cursor.Width = size;
            cursor.Height = size;
            cursor.Fill = new SolidColorBrush(mysolidColorBrush);
            cursor.Stroke = new SolidColorBrush(mysolidColorBrush);
            cursor.StrokeThickness = 2;
            Canvas.SetLeft(cursor, e.GetCurrentPoint(canvas).Position.X - (size / 2));
            Canvas.SetTop(cursor, e.GetCurrentPoint(canvas).Position.Y - (size / 2));
            canvas.Children.Add(cursor);
        }

        public void Cursor_Square(Canvas canvas, double size, PointerRoutedEventArgs e, Color mysolidColorBrush)
        {
            if (canvas.Children.Contains(cursor)) canvas.Children.Remove(cursor); if (canvas.Children.Contains(fallower_line)) canvas.Children.Remove(fallower_line);
            cursor = new Rectangle();
            cursor.Width = size;
            cursor.Height = size;
            cursor.Fill = new SolidColorBrush(mysolidColorBrush);
            cursor.Stroke = new SolidColorBrush(mysolidColorBrush);
            cursor.StrokeThickness = 2;
            Canvas.SetLeft(cursor, e.GetCurrentPoint(canvas).Position.X - (size / 2));
            Canvas.SetTop(cursor, e.GetCurrentPoint(canvas).Position.Y - (size / 2));
            canvas.Children.Add(cursor);
        }

        Line fallower_line = new Line();

        public void Line_Cursor(Canvas canvas, double size, PointerRoutedEventArgs e, Color mysolidColorBrush)
        {
            if (canvas.Children.Contains(cursor)) canvas.Children.Remove(cursor); if (canvas.Children.Contains(fallower_line)) canvas.Children.Remove(fallower_line);

            if (first_pick)
            {
                fallower_line.X1 = x_first_pick;
                fallower_line.X2 = e.GetCurrentPoint(canvas).Position.X;
                fallower_line.Y1 = y_first_pick;
                fallower_line.Y2 = e.GetCurrentPoint(canvas).Position.Y;
                fallower_line.Stroke = new SolidColorBrush(mysolidColorBrush);
                fallower_line.StrokeThickness = size;
                canvas.Children.Add(fallower_line);
            }
        }

        public void DisableClick()
        {
            if (first_pick) first_pick = !first_pick;
        }

        public void Create_Rectangle(Canvas canvas, PointerRoutedEventArgs e, Color cor)
        {
            if (first_pick == true)
            {
                Rectangle myrectangle = new Rectangle();
                double x_size, y_size;
                if (e.GetCurrentPoint(canvas).Position.X - x_first_pick >= 0)
                {
                    x_size = e.GetCurrentPoint(canvas).Position.X - x_first_pick;
                    Canvas.SetLeft(myrectangle, e.GetCurrentPoint(canvas).Position.X - (x_size));
                }
                else
                {
                    x_size = x_first_pick - e.GetCurrentPoint(canvas).Position.X;
                    Canvas.SetLeft(myrectangle, e.GetCurrentPoint(canvas).Position.X);
                }
                if (e.GetCurrentPoint(canvas).Position.Y - y_first_pick >= 0)
                {
                    y_size = e.GetCurrentPoint(canvas).Position.Y - y_first_pick;
                    Canvas.SetTop(myrectangle, e.GetCurrentPoint(canvas).Position.Y - (y_size));
                }
                else
                {
                    y_size = y_first_pick - e.GetCurrentPoint(canvas).Position.Y;
                    Canvas.SetTop(myrectangle, e.GetCurrentPoint(canvas).Position.Y);
                }

                myrectangle.Width = x_size;
                myrectangle.Height = y_size;
                myrectangle.Stroke = new SolidColorBrush(cor);
                myrectangle.Fill = new SolidColorBrush(cor);
                canvas.Children.Add(myrectangle);
                first_pick = false;
            }
            else if (!first_pick)
            {
                x_first_pick = e.GetCurrentPoint(canvas).Position.X;
                y_first_pick = e.GetCurrentPoint(canvas).Position.Y;
                first_pick = true;
            }
        }

        public void Undo_Draw(Canvas canvas)
        {
            if (canvas.Children.Count > 0) canvas.Children.RemoveAt(canvas.Children.Count - 1);
        }

        public void HideComponent(params UIElement[] component)
        {
            for (int i = 0; i < component.Length; i++)
                component[i].Visibility = Visibility.Collapsed;
        }
        public void VisibleComponent(params UIElement[] component)
        {
            for (int i = 0; i < component.Length; i++)
                component[i].Visibility = Visibility.Visible;
        }
        
    }
}
