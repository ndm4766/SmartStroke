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
    class TrailNode
    {
        int number;
        char letter;
        Point position;
        Ellipse e;
        TextBlock text;
        int size = 50;
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

        private void createShapes(string display, Point p, Canvas c)
        {
            // Create the Ellipse around the point
            e = new Ellipse();
            //e.Fill = new SolidColorBrush(Color.FromArgb(255,255,0,0));
            e.Width = size;
            e.Height = size;
            e.Margin = new Thickness(p.X, p.Y, 0,0);
            e.Stroke = new SolidColorBrush(Colors.White);
            c.Children.Add(e);

            // Create a TextBox inside the Ellipse
            text = new TextBlock();
            text.Text = display;
            text.Margin = new Thickness(p.X + size/3, p.Y + size/4, 0, 0);      // Correction factor
            text.FontSize = 25;
            text.Width = size;
            text.Height = size;
            c.Children.Add(text);
        }
        public void setFillColor(SolidColorBrush b)
        {
            fill = b;
            e.Fill = fill;
        }
        
    };
}
