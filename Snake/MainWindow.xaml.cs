using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Snake
{
    public class Point
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    public partial class MainWindow : Window
    {
        private Point point;
        private Point food;
        private int count = 3;
        private int score = 0;
        DispatcherTimer timer;
        private string side = "up";
        private List<Point> list = new List<Point>();
        private int speedValue;
        private Random rnd = new Random();

        public MainWindow()
        {
            InitializeComponent();
        }        

        private void StartSnake()
        {           
            point = GetRandomCoord();
            for (int i = 0; i < count; i++)
                list.Add(new Point(point.X + 10 * i, point.Y));
            food = GetRandomCoordFood();
            Draw();
        }

        private void CreateNewGame()
        {
            count = 3;
            list.Clear();
            score = 0;
            speedValue = 200;
            StartSnake();
            CreateTimer();
        }

        private void Displacement()
        {
            for (int i = count - 1; i > 0; i--)
                list[i] = new Point(list[i - 1].X, list[i - 1].Y);
        }

        private Point GetRandomCoord()
        {
            var x = rnd.Next((int)Canvas.Width - count * 10);
            var y = rnd.Next((int)Canvas.Height - count * 10);
            return new Point(x / 10 * 10, y / 10 * 10);
        }

        private Point GetRandomCoordFood()
        {
            var point = GetRandomCoord();
            for (int i = 0; i < count; i++)
            {
                if (point.X == list[i].X && point.Y == list[i].Y)
                    point = GetRandomCoord();
            }
            return point;
        }

        #region Draw
                
        private void Draw()
        {
            for (int i = 0; i < count; i++)
                Paint(list[i].X, list[i].Y, "Snake");
            Paint(food.X, food.Y, "Food");
            Score.Text = score.ToString();
        }

        private void Paint(double x, double y, string name)
        {
            Ellipse rec = new Ellipse();
            Canvas.SetTop(rec, x);
            Canvas.SetLeft(rec, y);
            rec.Width = 10;
            rec.Height = 10;
            if (name == "Snake")
                rec.Fill = new SolidColorBrush(Colors.Red);
            else if (name == "Food")
                rec.Fill = new SolidColorBrush(Colors.Blue);
            else if (name == "FoodClear")
                rec.Fill = new SolidColorBrush(Colors.White);
            Canvas.Children.Add(rec);
        }

        #endregion
        #region Timer

        private void CreateTimer()
        {
            timer?.Stop();
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, speedValue);
            timer.Tick += new EventHandler(TimerTick);
            timer.Start();            
        }

        private void TimerTick(object sender, EventArgs e)
        {
            Canvas.Children.Clear();
            Displacement();
            if (side == "up")
                list[0] = new Point(list[0].X - 10, list[0].Y);
            else if (side == "left")
                list[0] = new Point(list[0].X, list[0].Y - 10);
            else if (side == "right")
                list[0] = new Point(list[0].X, list[0].Y + 10);
            else if (side == "down")
                list[0] = new Point(list[0].X + 10, list[0].Y);

            CheckBorder();

            if (list[0].X == food.X && list[0].Y == food.Y)
                EatFood();
        }

        private void KeysDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left)
            {
                if (side == "down" || side == "up")
                {
                    side = "left";
                }
            }
            else if (e.Key == Key.Right)
            {
                if (side == "down" || side == "up")
                {
                    side = "right";
                }
            }
            else if (e.Key == Key.Up)
            {
                if (side == "left" || side == "right")
                {
                    side = "up";
                }
            }
            else if (e.Key == Key.Down)
            {
                if (side == "left" || side == "right")
                {
                    side = "down";
                }
            }
        }

        #endregion
        #region Verification

        private void CheckBorder()
        {
            if(list[0].X < 0)
            {
                list[0] = new Point((int)Canvas.Width - 10, list[0].Y);                
            }
            else if(list[0].X >= (int)Canvas.Width)
            {
                list[0] = new Point(0, list[0].Y);
            }
            else if(list[0].Y < 0)
            {
                list[0] = new Point(list[0].X, (int)Canvas.Height - 10);
            }
            else if (list[0].Y >= (int)Canvas.Height)
            {
                list[0] = new Point(list[0].X, 0);                
            }

            if (Smashed())
                CreateNewGame();

            Draw();
        }

        private bool Smashed()
        {
            for(int i = 1; i < count; i++)
            {
                if (list[0].X == list[i].X && list[0].Y == list[i].Y)
                {
                    MessageBox.Show("Game over");                    
                    return true;
                }
            }
            return false;
        }

        private void EatFood()
        {
            if (list[0].X == food.X && list[0].Y == food.Y)
            {
                var temp = list[count - 1];
                ++count;
                score += 50;
                list.Add(temp);
            }
            food = GetRandomCoordFood();
        }

        #endregion
        #region Events Handlers

        private void CreateNewGame(object sender, RoutedEventArgs e)
        {
            CreateNewGame();
        }

        private void RadioButtonEasy(object sender, RoutedEventArgs e) => speedValue = 500;

        private void RadioButtonNormal(object sender, RoutedEventArgs e) => speedValue = 300;

        private void RadioButtonHard(object sender, RoutedEventArgs e) => speedValue = 100;


        #endregion
        
    }
}
