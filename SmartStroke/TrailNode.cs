using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace SmartStroke
{
    public sealed class TrailNode : Shape
    {
        int number;
        char letter;
        Point position;
        Ellipse e;
        TextBlock text;
        int size = 50;
        bool completed = false;
        SolidColorBrush fill = new SolidColorBrush(Colors.White);

        public TrailNode() { }                             // Default constructor. Should not use
        public TrailNode(int n, Point p, Canvas c)         // Create a node for trails test A
        {
            number = n;
            letter = '0';
            position = p;
            createShapes(number.ToString(), position, c);
        }

        public TrailNode(char l, Point p, Canvas c) // Create a node for trails test B
        {
            number = 0;
            letter = l;
            position = p;
            createShapes(letter.ToString(), position, c);
        }

        public int getNumber() { return number; }
        public char getLetter() { return letter; }
        public Ellipse getEllipse()
        {
            return e;
        }

        private void createShapes(string display, Point p, Canvas c)
        {
            // Create the Ellipse around the point
            e = new Ellipse();
            e.Width = size;
            e.Height = size;
            e.Margin = new Thickness(p.X, p.Y, 0,0);
            e.Stroke = new SolidColorBrush(Colors.White);
            e.StrokeThickness = 3.0;
            c.Children.Add(e);

            // Create a TextBox inside the Ellipse
            text = new TextBlock();
            text.Text = display;
            if(text.Text.Length > 1)
                text.Margin = new Thickness(p.X + size / 3 + size/2, p.Y + size/4 - 5, 0, 0);      // Correction factor - must add another size/2 to account for rotation
            else
                text.Margin = new Thickness(p.X + size / 3 + size/2, p.Y + size / 4, 0, 0);      // Correction factor - must add another size/2 to account for rotation
            text.FontSize = 25;
            text.Width = size;
            text.Height = size;

            text.HorizontalAlignment = HorizontalAlignment.Right;
            // Rotate the text 90 degree
            RotateTransform r = new RotateTransform();
            r.Angle = 90.0;
            text.RenderTransform = r;

            text.IsHitTestVisible = false;
            
            c.Children.Add(text);
        }
        public void setFillColor(SolidColorBrush b)
        {
            fill = b;
            e.Fill = fill;
        }
        
        public void setComplete(bool val)
        {
            if (!completed)         // Cannot set a node to be not completed after it has been completed
                completed = val;
        }
    };
}