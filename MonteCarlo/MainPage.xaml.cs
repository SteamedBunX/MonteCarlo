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
using System.Diagnostics;
using Windows.UI.ViewManagement;
using System.Runtime.InteropServices;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI;
using System.Threading.Tasks;

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
        int defaultSeed;
        WriteableBitmap visualized;

        public MainPage()
        {
            this.InitializeComponent();
            ApplicationView.PreferredLaunchViewSize = new Size(800, 480);
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
            textBox_Input.Text = RandomInt().ToString();
            defaultSeed = r.Next();
            r = new Random(defaultSeed);
            textBox_RandomSeed.PlaceholderText = $"{defaultSeed}";
        }

        private async void Button_Generate_Click(object sender, RoutedEventArgs e)
        {
            await GenerateNewResult();
        }

        private async Task GenerateNewResult()
        {
            // Arrenge display
            button_Generate.IsEnabled = false;
            progressBar.Opacity = 1;
            progressBar.Value = 0;
            picture_result.Source = null;
            textBlock_Result.Text = "";

            // Start the process
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            int amount = int.Parse(textBox_Input.Text);
            int inside = 0;
            int outside = 0;
            
            //Set the random seed
            if(string.IsNullOrEmpty(textBox_RandomSeed.Text))
            {
                r = new Random(defaultSeed);
            }else
            {
                r = new Random(int.Parse(textBox_RandomSeed.Text));
            }

            // Calculation for the result
            var progressIndicator = new Progress<int>(ReportProgress);
            int[,] dots = await Task.Run(() => GenerateDots(amount, ref inside, progressIndicator));
            outside = amount - inside;

            // Print Result
            textBlock_Result.Text = GenerateResult(amount, inside, outside);

            // Visualize result
            visualized = VisualizeResult(dots);

            // Make progress bar invisible
            progressBar.Opacity = 0;

            // Display Image
            picture_result.Source = visualized;
            picture_result.InvalidateMeasure();

            // Print Timer
            stopWatch.Stop();
            long totalTime = stopWatch.ElapsedMilliseconds;
            long second = totalTime / 1000;
            long millisecond = totalTime % 1000;
            textBlock_Result.Text += $"\nTotal Time Elapsed : {second}s {millisecond}ms ";
            button_Generate.IsEnabled = true;
        }

        private int[,] GenerateDots(int amount, ref int inside, IProgress<int> progress)
        {
            int showProgress = 0;
            int[,] dots = new int[400, 400];
            for (int i = 0; i < amount; i++)
            {
                Point dot = GenerateDot();
                if (dot - originPoint <= 1)
                {
                    inside++;
                }
                dots[(int)(dot.GetX() * 400), (int)(dot.GetY() * 400)]++;
                int liveProgress = (int)(i / (double)amount * 100);
                if (showProgress < liveProgress)
                {
                    showProgress = liveProgress;
                    progress.Report(showProgress);
                }
            }

            return dots;
        }

        void ReportProgress(int value)
        {
            progressBar.Value = value;
        }

        WriteableBitmap VisualizeResult(int[,] dots)
        {
            WriteableBitmap result = new WriteableBitmap(400, 400);
            int maxValue = 0;
            int minValue = int.MaxValue;
            foreach (int value in dots)
            {
                if (value > maxValue)
                {
                    maxValue = value;
                }
                if (value < minValue)
                {
                    minValue = value;
                }
            }
            WriteableBitmap writeableBmp = BitmapFactory.New(400, 400);
            using (writeableBmp.GetBitmapContext())
            {
                for (int x = 0; x < 400; x++)
                {
                    for (int y = 0; y < 400; y++)
                    {
                        double brightness = 1 - (double)(dots[x, y] - minValue) / (maxValue - minValue);
                        brightness = brightness < 0 ? (byte)0.0 : brightness;
                        if (new Point(x / 400.0, y / 400.0) - originPoint < 1)
                        {
                            writeableBmp.SetPixel(x, y, (byte)(brightness * 255), (byte)(brightness * 255), 255);
                        }
                        else
                        {
                            writeableBmp.SetPixel(x, y, 255, (byte)(165 + (brightness * 90)), (byte)(brightness * 255));
                        }
                    }
                }
            }
            return writeableBmp;
        }

        string GenerateResult(int amount, int inside, int outside)
        {
            string result = "";
            double estimatedPI = (double)inside * 4 / amount;
            double difference = Math.Abs(estimatedPI - Math.PI);
            result += $"Total points generated : {amount}\n";
            result += $"Total points landed inside the fan : {inside}\n";
            result += $"Total points landed outside the fan : {outside}\n";
            result += $"Estimated value of PI : {estimatedPI: 0.0000000}\n";
            result += $"PI Actuall : {Math.PI: 0.0000000}\n";
            result += $"Difference : {difference: #.####E+0}\n";
            result += $"Difference in % : {difference / Math.PI: 0.0000%}";
            return result;
        }

        Point GenerateDot()
        {
            return new Point(r.NextDouble(), r.NextDouble());
        }

        int RandomInt()
        {
            return r.Next(1, int.MaxValue);
        }

        bool IsNumber(string input)
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

        bool WithinCircle(Point spot)
        {
            return spot - originPoint < 1;
        }

        bool StartWithZero(string input)
        {
            return input[0] == '0';
        }

        void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox.Text == string.Empty)
            {
                textBox.Text = "0";
            }
            else if (IsNumber(textBox.Text) && !int.TryParse(textBox.Text, out _))
            {
                textBox.Text = int.MaxValue.ToString();
                textBlock_Warning.Text = $"The number cannot be greater than {int.MaxValue}";
            }
        }

        void TextBox_BeforeTextChanging(TextBox sender, TextBoxBeforeTextChangingEventArgs args)
        {
            TextBox textBox = sender as TextBox;
            if (textBlock_Result != null)
            {
                textBlock_Warning.Text = "";
                var text = args.NewText;
                int result = 0;
                if (text.Contains(" ") || text.Contains("\n"))
                {
                    args.Cancel = true;
                    textBlock_Warning.Text = "Please Only Enter Numbers";
                }
                else if (!int.TryParse(text, out result))
                {
                    if (!IsNumber(text))
                    {
                        args.Cancel = true;
                        textBlock_Warning.Text = "Please Only Enter Numbers";
                    }
                }
                else if (result < 0)
                {
                    textBlock_Warning.Text = "Input Cannot Be Negative";
                    args.Cancel = true;
                }
                else if (text == "0")
                {

                }
                else if (StartWithZero(text))
                {
                    textBlock_Warning.Text = "Number Cannot Start With 0";
                    args.Cancel = true;
                }
            }

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
            return (Math.Abs(point2.x - this.x) < .0001 && Math.Abs(point2.y - this.y) < .0001);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
