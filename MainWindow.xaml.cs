using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Lab3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        WriteableBitmap wb;
        Point position;
        Color color = Colors.Black;
        bool lineHelper = false;
        string applyHelper;
        WriteableBitmap pattern;
        public MainWindow()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (Style != null)
            {
                Button b1 = (Button)this.Template.FindName("btnClose", this);
                b1.Click += (sender1, e1) => Close();
                Button b2 = (Button)this.Template.FindName("btnMinimize", this);
                b2.Click += (sender1, e1) =>
                {
                    if (WindowState != WindowState.Minimized)
                    {
                        WindowState = WindowState.Minimized;
                    };
                };
                Button b3 = (Button)this.Template.FindName("btnMaximize", this);
                b3.Click += (sender1, e1) =>
                {
                    if (WindowState != WindowState.Maximized)
                    {
                        WindowState = WindowState.Maximized;
                    }
                    else
                    {
                        WindowState = WindowState.Normal;
                    }
                };
                Grid grid = (Grid)this.Template.FindName("gridMove", this);
                grid.MouseLeftButtonDown += (sender1, e1) => DragMove();
            }
            wb = BitmapFactory.New((int)canv.ActualWidth, (int)canv.ActualHeight);
            Random rnd = new Random();

            pattern = BitmapFactory.New(10,10);
            for (int i = 0; i < pattern.Height; i++)
            {
                for (int j = 0; j < pattern.Width; j++)
                {
                    //pattern.SetPixel(i, j, Color.FromRgb((byte)(i*60), (byte)(j * 10), (byte)(i*20 + j*10)));
                    if(i==j)
                        pattern.SetPixel(i, j, Color.FromRgb((byte)(i * 60), (byte)(j * 10), (byte)(i * 20 + j * 10)));
                    else
                        pattern.SetPixel(i, j, Colors.LightGray);
                }
            }
            
            Image img = new Image()
            {
                Source = wb,
            };
            canv.Children.Add(img);            
        }

        #region MouseHandlers
        private void Pen_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var cursorPosition = e.GetPosition(canv);
            wb.SetPixel((int)cursorPosition.X, (int)cursorPosition.Y, color);
        }
        private void Line_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var cursorPosition = e.GetPosition(canv);
            if (lineHelper)
            {
                Line((int)position.X, (int)position.Y, (int)cursorPosition.X, (int)cursorPosition.Y, color);
                lineHelper = false;
            }
            else
                lineHelper = true;
            position = cursorPosition;
        }
        private void Fill1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Fill(e.GetPosition(canv), Colors.Red, color);           
        }
        private void Fill2_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {         
            FillWithPattern(e.GetPosition(canv), color);
        }
        private void Ellipse_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var point = e.GetPosition(canv);
            Ellipse((int)point.X, (int)point.Y, Convert.ToInt32(tb1.Text), color);
        }
        private void EllipseNative_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var point = e.GetPosition(canv);
            EllipseNative((int)point.X, (int)point.Y, Convert.ToInt32(tb1.Text), color);
        }
        private void CircleNative_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var point = e.GetPosition(canv);
            CircleNative((int)point.X, (int)point.Y, Convert.ToInt32(tb1.Text), color);
        }
        #endregion

        #region Methods
        public void Line(int x1, int y1, int x2, int y2, Color color)
        {
            int lengthX = Math.Abs(x2 - x1);
            int lengthY = Math.Abs(y2 - y1);

            int dx = (x2 - x1 >= 0 ? 1 : -1);
            int dy = (y2 - y1 >= 0 ? 1 : -1);

            int length = Math.Max(lengthX, lengthY);

            if (length == 0)
            {
                wb.SetPixel(x1, y1, color);
            }
            if (lengthY <= lengthX)
            {
                int x = x1;
                int y = y1;
                int d = -lengthX;
                length++;
                while (length > 0)
                {
                    wb.SetPixel(x, y, color);
                    x += dx;
                    d += 2 * lengthY;
                    if (d > 0)
                    {
                        d -= 2 * lengthX;
                        y += dy;
                    }
                    length--;
                }
            }
            else
            {
                int x = x1;
                int y = y1;
                int d = -lengthY;
                
                length++;
                while (length > 0)
                {
                    wb.SetPixel(x, y, color);
                    y += dy;
                    d += 2 * lengthX;
                    if (d > 0)
                    {
                        d -= 2 * lengthY;
                        x += dx;
                    }
                    length--;
                }
            }
        }
        public void Ellipse(int x0, int y0, int R, Color color)
        {
            int x = 0;
            int y = R;
            int d = 3 - 3 * R;
            while(y >= x)
            {
                Draw8Pixels(x, y, x0, y0, color);
                if (d <= 0)
                {
                    d += 4 * x + 6;
                }
                else
                {
                    d += 4 * (x - y) + 10;
                    y--;
                }
                x++;   
            }
        }
        public void EllipseNative(int x0, int y0, int R, Color color)
        {
            int x = x0, y = y0;
            for (int alfa = 1; alfa <= 360; alfa++)
            {
                x = (int)(x0 + R * Math.Cos(alfa * Math.PI / 180));
                y = (int)(y0 + R * Math.Sin(alfa * Math.PI / 180));
                wb.SetPixel(x, y, color);
            }
        }
        public void CircleNative(int x0, int y0, int R, Color color)
        {
            int x = x0, y = y0;
            int xPrev = (int)(x0 + R * Math.Cos(0));
            int yPrev = (int)(y0 + R * Math.Sin(0));
            for (int alfa = 1; alfa <= 360; alfa++)
            {
                x = (int)(x0 + R * Math.Cos(alfa * Math.PI / 180));
                y = (int)(y0 + R * Math.Sin(alfa * Math.PI / 180));
                Line(xPrev, yPrev, x, y, color);
                xPrev = x;
                yPrev = y;
            }
        }
        public void Draw8Pixels(int x, int y, int x0, int y0, Color color)
        {
            wb.SetPixel(x + x0, y + y0, color);
            wb.SetPixel(x + x0, -y + y0, color);
            wb.SetPixel(-x + x0, y + y0, color);
            wb.SetPixel(-x + x0, -y + y0, color);
            wb.SetPixel(y + x0, x + y0, color);
            wb.SetPixel(y + x0, -x + y0, color);
            wb.SetPixel(-y + x0, x + y0, color);
            wb.SetPixel(-y + x0, -x + y0, color);
        }
        public void Fill(Point p, Color col, Color borderBrush)
        {
            if (wb.GetPixel((int)(p.X + 1), (int)p.Y) != col && wb.GetPixel((int)(p.X + 1), (int)p.Y) != borderBrush)
            {
                wb.SetPixel((int)(p.X + 1), (int)p.Y, col);
                Fill(new Point(p.X + 1, p.Y), col, borderBrush);
            }
            if (wb.GetPixel((int)(p.X - 1), (int)p.Y) != col && wb.GetPixel((int)(p.X - 1), (int)p.Y) != borderBrush)
            {
                wb.SetPixel((int)(p.X - 1), (int)p.Y, col);
                Fill(new Point(p.X - 1, p.Y), col, borderBrush);
            }
            if (wb.GetPixel((int)(p.X), (int)p.Y + 1) != col && wb.GetPixel((int)(p.X), (int)p.Y + 1) != borderBrush)
            {
                wb.SetPixel((int)(p.X), (int)p.Y + 1, col);
                Fill(new Point(p.X, p.Y + 1), col, borderBrush);
            }
            if (wb.GetPixel((int)(p.X), (int)p.Y - 1) != col && wb.GetPixel((int)(p.X), (int)p.Y - 1) != borderBrush)
            {
                wb.SetPixel((int)(p.X), (int)p.Y - 1, col);
                Fill(new Point(p.X, p.Y - 1), col, borderBrush);
            }
        }
        //pattern.GetPixel((int)(p1.X % pattern.Width), (int)(p1.Y % pattern.Height))
        public void FillWithPattern(Point p, Color borderBrush)
        {
            if (p.X > 0 && p.X < canv.ActualWidth && p.Y > 0 && p.Y < canv.ActualHeight && wb.GetPixel((int)p.X, (int)p.Y) != borderBrush 
                && wb.GetPixel((int)p.X, (int)p.Y) != pattern.GetPixel((int)(p.X % pattern.Width), (int)(p.Y % pattern.Height)))
            {
                Point p1 = new Point(p.X, p.Y);
                Color c = borderBrush;
                while (p1.X > 0 && p1.X < canv.ActualWidth && p1.Y > 0 && p1.Y < canv.ActualHeight && wb.GetPixel((int)p1.X, (int)p1.Y) != borderBrush 
                    && wb.GetPixel((int)p1.X, (int)p1.Y) != c)
                {
                    c = pattern.GetPixel((int)(p1.X % pattern.Width), (int)(p1.Y % pattern.Height));
                    wb.SetPixel((int)p1.X, (int)p1.Y, c);
                    p1.X -= 1;
                }
                Point pl = p1;
                pl.X++;
                p1.X = p.X + 1;
                while (p1.X > 0 && p1.X < canv.ActualWidth && p1.Y > 0 && p1.Y < canv.ActualHeight && wb.GetPixel((int)p1.X, (int)p1.Y) != borderBrush 
                    && wb.GetPixel((int)p1.X, (int)p1.Y) != c)
                {
                    c = pattern.GetPixel((int)(p1.X % pattern.Width), (int)(p1.Y % pattern.Height));
                    wb.SetPixel((int)p1.X, (int)p1.Y, c);
                    p1.X += 1;
                }
                Point pr = p1;
                pr.X--;
                //под строкой 
                pl.Y += 1;
                pr.Y += 1;
                Point point = pr;
                while (point.X > pl.X)
                {
                    while (point.X > pl.X && wb.GetPixel((int)point.X, (int)point.Y) == borderBrush)
                    {
                        point.X--;
                    }
                    FillWithPattern(point, borderBrush);
                    while (point.X > pl.X && wb.GetPixel((int)point.X, (int)point.Y) != borderBrush)
                    {
                        point.X--;
                    }
                }
                //над строкой 
                pl.Y = p.Y - 1;
                pr.Y = p.Y - 1;
                Point point2 = pr;
                while (point2.X > pl.X)
                {
                    while (point2.X > pl.X && wb.GetPixel((int)point2.X, (int)point2.Y) == borderBrush)
                    {
                        point2.X--;
                    }
                    FillWithPattern(point2, borderBrush);
                    while (point2.X > pl.X && wb.GetPixel((int)point2.X, (int)point2.Y) != borderBrush)
                    {
                        point2.X--;
                    }
                }
            }
        }     
        #endregion

        #region ButtonHandlers
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if(comboBox.SelectedIndex != -1)
            {
                var type = (comboBox.SelectedItem as ComboBoxItem).Content;
                               
                switch (type as string)
                {
                    case "Pen":
                        {
                            if (applyHelper != "Pen")
                            {
                                canv.MouseLeftButtonDown -= Fill2_MouseLeftButtonDown;
                                canv.MouseLeftButtonDown -= Ellipse_MouseLeftButtonDown;
                                canv.MouseLeftButtonDown -= EllipseNative_MouseLeftButtonDown;
                                canv.MouseLeftButtonDown -= Fill1_MouseLeftButtonDown;
                                canv.MouseLeftButtonDown -= Line_MouseLeftButtonDown;
                                canv.MouseLeftButtonDown -= CircleNative_MouseLeftButtonDown;
                                canv.MouseLeftButtonDown += Pen_MouseLeftButtonDown;
                                applyHelper = "Pen";
                            }
                            break;
                        }
                    case "Line":
                        {
                            if (applyHelper != "Line")
                            {
                                canv.MouseLeftButtonDown -= Fill2_MouseLeftButtonDown;
                                canv.MouseLeftButtonDown -= Ellipse_MouseLeftButtonDown;
                                canv.MouseLeftButtonDown -= EllipseNative_MouseLeftButtonDown;
                                canv.MouseLeftButtonDown -= Fill1_MouseLeftButtonDown;
                                canv.MouseLeftButtonDown -= Pen_MouseLeftButtonDown;
                                canv.MouseLeftButtonDown -= CircleNative_MouseLeftButtonDown;
                                canv.MouseLeftButtonDown += Line_MouseLeftButtonDown;
                                applyHelper = "Line";
                            }
                            break;
                        }
                    case "Fill":
                        {
                            if (applyHelper != "Fill")
                            {
                                canv.MouseLeftButtonDown -= Fill2_MouseLeftButtonDown;
                                canv.MouseLeftButtonDown -= Ellipse_MouseLeftButtonDown;
                                canv.MouseLeftButtonDown -= EllipseNative_MouseLeftButtonDown;
                                canv.MouseLeftButtonDown -= Line_MouseLeftButtonDown;
                                canv.MouseLeftButtonDown -= Pen_MouseLeftButtonDown;
                                canv.MouseLeftButtonDown -= CircleNative_MouseLeftButtonDown;
                                canv.MouseLeftButtonDown += Fill1_MouseLeftButtonDown;                               
                                applyHelper = "Fill";
                            }
                            break;
                        }
                    case "Ellipse":
                        {
                            if (applyHelper != "Ellipse")
                            {
                                canv.MouseLeftButtonDown -= Fill2_MouseLeftButtonDown;
                                canv.MouseLeftButtonDown -= Fill1_MouseLeftButtonDown;
                                canv.MouseLeftButtonDown -= Pen_MouseLeftButtonDown;
                                canv.MouseLeftButtonDown -= Line_MouseLeftButtonDown;
                                canv.MouseLeftButtonDown -= EllipseNative_MouseLeftButtonDown;
                                canv.MouseLeftButtonDown -= CircleNative_MouseLeftButtonDown;
                                canv.MouseLeftButtonDown += Ellipse_MouseLeftButtonDown;
                                
                                applyHelper = "Ellipse";
                            }
                            break;
                        }
                    case "EllipseNative":
                        {
                            if (applyHelper != "EllipseNative")
                            {
                                canv.MouseLeftButtonDown -= Fill2_MouseLeftButtonDown;
                                canv.MouseLeftButtonDown -= Fill1_MouseLeftButtonDown;
                                canv.MouseLeftButtonDown -= Pen_MouseLeftButtonDown;
                                canv.MouseLeftButtonDown -= Line_MouseLeftButtonDown;
                                canv.MouseLeftButtonDown -= Ellipse_MouseLeftButtonDown;
                                canv.MouseLeftButtonDown -= CircleNative_MouseLeftButtonDown;
                                canv.MouseLeftButtonDown += EllipseNative_MouseLeftButtonDown;
                                applyHelper = "EllipseNative";
                            }
                            break;
                        }
                    case "CircleNative":
                        {
                            if (applyHelper != "CircleNative")
                            {
                                canv.MouseLeftButtonDown -= Fill2_MouseLeftButtonDown;
                                canv.MouseLeftButtonDown -= Ellipse_MouseLeftButtonDown;
                                canv.MouseLeftButtonDown -= EllipseNative_MouseLeftButtonDown;
                                canv.MouseLeftButtonDown -= Fill1_MouseLeftButtonDown;
                                canv.MouseLeftButtonDown -= Line_MouseLeftButtonDown;
                                canv.MouseLeftButtonDown -= Pen_MouseLeftButtonDown;
                                canv.MouseLeftButtonDown += CircleNative_MouseLeftButtonDown;
                                applyHelper = "CircleNative";
                            }
                            break;
                        }
                    case "FillWithPattern":
                        {
                            if (applyHelper != "FillWithPattern")
                            {
                                canv.MouseLeftButtonDown -= Fill1_MouseLeftButtonDown;
                                canv.MouseLeftButtonDown -= Pen_MouseLeftButtonDown;
                                canv.MouseLeftButtonDown -= Line_MouseLeftButtonDown;
                                canv.MouseLeftButtonDown -= Ellipse_MouseLeftButtonDown;
                                canv.MouseLeftButtonDown -= EllipseNative_MouseLeftButtonDown;
                                canv.MouseLeftButtonDown -= CircleNative_MouseLeftButtonDown;
                                canv.MouseLeftButtonDown += Fill2_MouseLeftButtonDown;
                                applyHelper = "FillWithPattern";
                            }
                            break;
                        }
                    case "null":
                        {
                            canv.MouseLeftButtonDown -= Fill2_MouseLeftButtonDown;
                            canv.MouseLeftButtonDown -= Ellipse_MouseLeftButtonDown;
                            canv.MouseLeftButtonDown -= Fill1_MouseLeftButtonDown;
                            canv.MouseLeftButtonDown -= Pen_MouseLeftButtonDown;
                            canv.MouseLeftButtonDown -= Line_MouseLeftButtonDown;
                            canv.MouseLeftButtonDown -= EllipseNative_MouseLeftButtonDown;
                            canv.MouseLeftButtonDown -= CircleNative_MouseLeftButtonDown;
                            applyHelper = "null";
                            break;
                        }
                }
            }                       
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            wb.Clear();
        }
        #endregion
        [Obsolete]
        private void CombineCanvasElements()
        {            
            Path myPath = new Path();
            var cg = new GeometryGroup();
            foreach (Shape element in canv.Children)
            {
                if (element is Rectangle)
                {
                    var el = new RectangleGeometry(new Rect(Canvas.GetLeft(element), Canvas.GetTop(element),
                        element.Width, element.Height));
                    cg.Children.Add(el);
                }
                else
                {
                    cg.Children.Add(element.RenderedGeometry);
                }                
            }
            cg.FillRule = FillRule.Nonzero;
            canv.Children.Clear();
            myPath.Data = cg;
            myPath.Fill = Brushes.Black;
            canv.Children.Add(myPath);
        }

        private void canv_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (wb != null)
            {
                var wbNew = BitmapFactory.New((int)e.NewSize.Width, (int)e.NewSize.Height);
                using (wbNew.GetBitmapContext())
                {
                    using (wb.GetBitmapContext())
                    {
                        Rect rec = new Rect(0, 0, e.PreviousSize.Width, e.PreviousSize.Height);
                        wbNew.Blit(rec, wb, rec, WriteableBitmapExtensions.BlendMode.Additive);
                    }
                }
                wb = wbNew;
                Image img = new Image()
                {
                    Source = wb,
                };
                canv.Children.Clear();
                canv.Children.Add(img);
            }                   
        }
    }
}
