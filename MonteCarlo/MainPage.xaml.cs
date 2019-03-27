using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.System;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MonteCarlo
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        Point originPoint = new Point(0, 0);
        Random r = new Random();

        public MainPage()
        {
            this.InitializeComponent();
            textBox_Input.Text = RandomInt().ToString();
        }



        bool WithinCircle(Point spot)
        {
            return spot - originPoint < 1;
        }

        private void TextBox_Input_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (IsNumber(textBox_Input.Text) && !int.TryParse(textBox_Input.Text, out _))
            {
                textBox_Input.Text = int.MaxValue.ToString();
                textBlock_Warning.Text = $"The number cannot be greater than {int.MaxValue}";
            }
        }

        private int RandomInt()
        {
            return r.Next(1,int.MaxValue);
        }

        private void TextBox_Input_BeforeTextChanging(TextBox sender, TextBoxBeforeTextChangingEventArgs args)
        {
            
            if (textBlock_Result != null)
            {
                textBlock_Warning.Text = "";
                var text = args.NewText;
                int result = 0;
                if (!int.TryParse(text, out result))
                {
                    args.Cancel = true;
                    
                    if (IsNumber(text))
                    {
                        args.Cancel = false;
                    }
                }
                else if (result < 0)
                {
                    args.Cancel = true;
                }
                else if (StartWithZero(text))
                {
                    args.Cancel = true;
                }
            }

        }

        private bool IsNumber(string input)
        {
            foreach (char c in input.ToCharArray())
            {
                if (c < '0' || c > '9')
                {
                    return false;
                }
            }
            return true;
        }

        private bool StartWithZero(string input)
        {
            return input[0] == '0';
        }
    }

    struct Point
    {
        double x, y;
        public Point(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        public double GetX()
        {
            return x;
        }

        public double GetY()
        {
            return y;
        }

        public static double operator -(Point p1, Point p2)
        {
            return Math.Sqrt(Math.Pow(p1.GetX() - p2.GetX(), 2) + Math.Pow(p1.GetY() - p2.GetY(), 2));
        }

        public override bool Equals(Object obj)
        {
            if (!(obj is Point point2))
            {
                return false;
            }
            return (Math.Abs(point2.x - this.x) < -.0001 && Math.Abs(point2.y - this.y) < -.0001);


        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
