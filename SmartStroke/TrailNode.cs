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
    // TrailNode is simply a node for the Trails test.
    // It works with both versions a and b. The node is
    // and ellipse, with a fill color, a position, a letter,
    // or a number (to accomodate the two versions of the test.

    // Usage: TrailNode()
    // Usage: TrailNode(int, Point, Canvas, bool)
    // Usage: TrailNode(char, Point, Canvas, bool)
    public sealed class TrailNode : Shape
    {
        // Number or letter to display inside the node
        int number;
        char letter;

        // Position of the Ellipse on the screen
        Point position;
        Ellipse e;

        // TextBlock to place inside the node to display the 
        // number or character
        TextBlock text;

        // Constant width of 60 pixels for each node
        int size;

        // Whether the node has been connected correctly or not
        bool completed = false;

        // Color to fill the Ellipse in
        SolidColorBrush fill = new SolidColorBrush(Colors.White);
        
        // Whether to rotate the text in the Ellipse or not (for horizontal
        // versions of the test)
        private bool rotate;

        public TrailNode() { size = 60; }                           // Default constructor. Should not use
        public TrailNode(int n, Point p, Canvas c, bool r = true)   // Create a node for trails test A
        {
            size = 60;
            number = n;
            letter = '0';
            position = p;
            rotate = r;
            createShapes(number.ToString(), position, c);
        }

        public TrailNode(char l, Point p, Canvas c, bool r = true) // Create a node for trails test B
        {
            size = 60;
            number = 0;
            letter = l;
            position = p;
            rotate = r;
            createShapes(letter.ToString(), position, c);
        }

        public TrailNode(string s, Point p, bool r = true) // Create a node from the replay file
        {
            number = 0;
            letter = '0';
            int n = 0;
            bool res = Int32.TryParse(s, out n);
            // Is a number
            if (res)
                number = n;
            else
                letter = Convert.ToChar(s);
            position = p;
            rotate = r;
        }

        // Return whether or not the node has been completed.
        // Useful for determining if the user has collided with
        // a new node or an old node. If old, the color should
        // not change.
        public bool getCompleted() { return completed; }
        
        // Return the node's number value.  0 if it has a char instead. 
        public int getNumber() { return number; }

        // Return the node's char value. '0' if it has an int instead.
        public char getLetter() { return letter; }

        // Return the location of the node on the screen. Will be useful
        // when comparing left-right impairments.
        public Point getLocation() { return position; }

        // Return the actual Ellipse.
        public Ellipse getEllipse() { return e; }

        // Create the Ellipse, the TextBox, and display it on the Canvas.
        private void createShapes(string display, Point p, Canvas c)
        {
            // Create the Ellipse around the point
            // Set the point to the passed in point. 
            // The width and the height will be the radius size (thus a Circle)
            // Create a default color - CornflowerBlue
            // Add the Ellipse to the Canvas
            e = new Ellipse();
            e.Width = size;
            e.Height = size;
            e.Margin = new Thickness(p.X, p.Y, 0,0);
            e.Stroke = new SolidColorBrush(Colors.Black);
            e.Fill = new SolidColorBrush(Colors.CornflowerBlue);
            e.StrokeThickness = 3.0;
            c.Children.Add(e);

            // Create a TextBox inside the Ellipse
            // The text should display either a number or a character
            text = new TextBlock();
            text.Text = display;

            // If the length of the text is greater than one, I need to move
            // the location of the TextBlock inside the Ellipse to the left some.
            // If the text should be rotated, I need to move it up instead.
            if (text.Text.Length > 1)
            {
                // The top left corner of the TextBlock should actually be above and to the
                // right of the center of the ellipse because we want to rotate the text.
                if (rotate)
                    text.Margin = new Thickness(p.X + size / 3 + size / 2, p.Y + size / 4 - 5, 0, 0);      // Correction factor - must add another size/2 to account for rotation
                else
                    text.Margin = new Thickness(p.X + size / 3, p.Y + size / 4 - 5, 0, 0);
            }
            else
            {
                if (rotate)
                    text.Margin = new Thickness(p.X + size / 3 + size / 2, p.Y + size / 4, 0, 0);      // Correction factor - must add another size/2 to account for rotation
                else
                    text.Margin = new Thickness(p.X + size / 3, p.Y + size / 4, 0, 0);
            }

            // Set the font size, width, and height of the TextBlock
            text.FontSize = 30;
            text.Width = size;
            text.Height = size;

            text.HorizontalAlignment = HorizontalAlignment.Right;

            if (rotate)
            {
                // Rotate the text 90 degree
                RotateTransform r = new RotateTransform();
                r.Angle = 90.0;
                text.RenderTransform = r;
            }
            // Do not allow the user to hit the text and have anything happen.
            text.IsHitTestVisible = false;
            
            // Add the text to the Canvas.
            c.Children.Add(text);
        }

        // Set the fill color of the node. Update the actual Ellipse color and the color in the
        // class, which will be used for ease of use for error purposes.
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

        // Convert the trail node to a string. This is used for when an error is made and
        // the error is captured inside the TestReplay object and saved to the file. We want
        // to save the number/letter of the node as well as the actual point (where it is on
        // the screen)        
        // Save: number/char tab Point
        // Save: number/char "     " x y
        public string convertToString()
        {
            string converted = "";

            // The node actually contains a letter.
            if (number == 0)
                converted += letter.ToString();
            else
                converted += number.ToString();

            converted += "\t";

            converted += position.X.ToString();
            converted += "\t";
            converted += position.Y.ToString();
            converted += "\t";

            return converted;
        }

        // Determine whether two TrailNode objects are equal or not
        public static bool operator ==(TrailNode a, TrailNode b)
        {
            return a.getLetter() == b.getLetter() &&
                   a.getNumber() == b.getNumber() &&
                   a.getLocation() == b.getLocation() &&
                   a.getCompleted() == b.getCompleted();
        }
        public static bool operator !=(TrailNode a, TrailNode b)
        {
            return !(a == b);
        }
    };
}