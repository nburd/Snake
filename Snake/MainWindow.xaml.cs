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
        DispatcherTimer timer;
        private string side = "up";
        private List<Point> list = new List<Point>();
        private List<Point> foods = new List<Point>();
        private int speedValue = 800;
        private Random rnd = new Random();

        public MainWindow()
        {
            InitializeComponent();
            StartSnake();
        }        

        private void StartSnake()
        {
            point = GetRandomCoord();
            for (int i = 0; i < count; i++)
                list.Add(new Point(point.X + 10 * i, point.Y));
            food = GetRandomCoord();
            Draw();
        }

        private void Displacement()
        {
            for (int i = count - 1; i > 0; i--)
                list[i] = new Point(list[i - 1].X, list[i - 1].Y);
        }

        private Point GetRandomCoord()
        {
            rnd = new Random();
            var x = rnd.Next((int)Canvas.Width - count * 10);
            var y = rnd.Next((int)Canvas.Height - count * 10);
            return new Point(x / 10 * 10, y / 10 * 10);
        }

        #region Draw

        private void DrawUp()
        {
            Displacement();
            list[0] = new Point(list[0].X - 10, list[0].Y);
            //if (list[0] == food)
               // EatFood();
            CheckToSmash();
            CheckBorder();            
        }

        private void DrawDown()
        {
            Displacement();
            list[0] = new Point(list[0].X + 10, list[0].Y);
            //if (list[0] == food)
              //  EatFood();
            CheckToSmash();
            CheckBorder();
        }

        private void DrawLeft()
        {
            Displacement();
            list[0] = new Point(list[0].X, list[0].Y - 10);
            CheckToSmash();
            CheckBorder();
        }

        private void DrawRight()
        {
            Displacement();
            list[0] = new Point(list[0].X, list[0].Y + 10);
            CheckToSmash();
            CheckBorder();
        }

        private void Draw()
        {
            for (int i = 0; i < count; i++)
                Paint(list[i].X, list[i].Y, "Snake");
            Paint(food.X, food.Y, "Food");
        }

        private void Paint(double x, double y, string name)
        {
            Ellipse rec = new Ellipse();
            Canvas.SetTop(rec, x);
            Canvas.SetLeft(rec, y);
            rec.Width = 10;
            rec.Height = 10;
            if(name == "Snake")
                rec.Fill = new SolidColorBrush(Colors.Red);
            else if(name == "Food")
                rec.Fill = new SolidColorBrush(Colors.Blue);
            Canvas.Children.Add(rec);
        }

        #endregion
        #region Timer

        private void CreateTimer()
        {
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, speedValue);
            timer.Tick += new EventHandler(TimerTick);
            timer.Start();            
        }

        private void TimerTick(object sender, EventArgs e)
        {
            Canvas.Children.Clear();            
            if (side == "up")
                DrawUp();
            else if (side == "left")
                DrawLeft();
            else if (side == "right")
                DrawRight();
            else if (side == "down")
                DrawDown();
        }

        private void KeysDown(object sender, KeyEventArgs e)
        {
            timer.Stop();
            if (e.Key == Key.Left)
            {
                if (side == "down" || side == "up")
                {
                    side = "left";
                    CreateTimer();
                }
                else CreateTimer();
            }
            else if (e.Key == Key.Right)
            {
                if (side == "down" || side == "up")
                {
                    side = "right";
                    CreateTimer();
                }
                else CreateTimer();
            }
            else if (e.Key == Key.Up)
            {
                if (side == "left" || side == "right")
                {
                    side = "up";
                    CreateTimer();
                }
                else CreateTimer();
            }
            else if (e.Key == Key.Down)
            {
                if (side == "left" || side == "right")
                {
                    side = "down";
                    CreateTimer();
                }
                else CreateTimer();
            }
        }

        #endregion
        #region Verification

        private void CheckBorder()
        {
            if(list[0].X < 0)
            {
                list[0] = new Point((int)Canvas.Height - 10, list[0].Y);
                CheckToSmash();
                Draw();
            }
            else if(list[0].X >= Canvas.Height)
            {
                list[0] = new Point(0, list[0].Y);
                CheckToSmash();
                Draw();
            }
            else if(list[0].Y < 0)
            {
                list[0] = new Point(list[0].X, (int)Canvas.Width - 10);
                CheckToSmash();
                Draw();
            }
            else if (list[0].Y >= Canvas.Width)
            {
                list[0] = new Point(list[0].X, 0);
                CheckToSmash();
                Draw();
            }
            Draw();
        }

        private void CheckToSmash()
        {
            for(int i = 1; i < count; i++)
            {
                if (list[0].X == list[i].X && list[0].Y == list[i].Y)
                {
                    Canvas.Children.Clear();
                    var result = MessageBox.Show("Игра закончена! Начать новую игру?", "New Game", MessageBoxButton.YesNo);
                    if (result == MessageBoxResult.Yes)
                    {
                        list.Clear();
                        StartSnake();
                        CreateTimer();
                        break;
                    }
                    else
                        Close();
                }
            }
        }

        //private void EatFood()
        //{
        //    if(list[0].X == food.X && list[0].Y == food.Y)
        //    {
        //        var temp = list[count - 1];
        //        ++count;
        //        list.Add(temp);
        //    }
        //}

        #endregion
        #region Events Handlers

        private void CreateNewGame(object sender, RoutedEventArgs e)
        {
            list.Clear();
            StartSnake();
            //CreateTimer();
        }

        private void RadioButtonEasy(object sender, RoutedEventArgs e) => speedValue = 500;

        private void RadioButtonNormal(object sender, RoutedEventArgs e) => speedValue = 300;

        private void RadioButtonHard(object sender, RoutedEventArgs e) => speedValue = 100;


        #endregion
        
        private Point GetCoordFood()
        {
            var point = GetRandomCoord();
            if (point.Y == list[0].Y)
                point = GetRandomCoord();
            return point;
        }
    }
}
