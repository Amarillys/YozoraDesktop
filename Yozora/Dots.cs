using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Yozora
{
    public struct Dot
    {
        public float X, Y;
        public float Vx, Vy;
        public float Radius;
        public Color Color;       

        static Random random = new Random((int)DateTime.Now.ToBinary());

        public Dot(int width, int height, Color color)
        {
            X = (float)random.NextDouble() * width;
            Y = (float)random.NextDouble() * height;
            Vx = -0.5f + (float)random.NextDouble();
            Vy = -0.5f + (float)random.NextDouble();
            Radius = (float)random.NextDouble() * 2;
            Color = color;
        }
    }

    public class DotMgr
    {
        private Dot[] dot_array_;
        private Graphics g_;
        //width
        private Int32 w_ = 1920;
        //height
        private Int32 h_ = 1200;
        //the limit of fps
        private Int32 f_ = 24;
        //the quanlity of stars
        private float s_ = 0.2f;
        //the quanlity of dots
        private float d_ = 0.2f;
        //the lines of stars
        private Int32 l_ = 5;
        //the width of lines
        private Int32 b_ = 1;

        public Inf Init(Graphics g, Int32 width, Int32 height, Int32 frame, float stars, float dots)
        {
            g_ = g;
            w_ = width;
            h_ = height;
            f_ = frame;
            s_ = stars;
            d_ = dots;
            return new Inf(0, "success init.");
        }

        public void CreateDots()
        {

        }

        public void DrawStar()
        {
            Int32 x, y, r1, r2;
            Int32 angle = 180 / l_;
            Int32 l = l_ * 2;
            Point[] star = new Point[l+1];
            Color color;
            //bool isFill;
            Pen p;
            Random rnd = new Random();
            
            for (Int32 i = 0; i < w_ * s_; ++i)
            {
                x = rnd.Next(10, w_ - 10);
                y = rnd.Next(10, h_ - 10);
                // ?
                r1 = rnd.Next(4, 7);
                r2 = rnd.Next(8, 12);
                color = Color.FromArgb(240, rnd.Next(1, 254), rnd.Next(1, 254), rnd.Next(1, 254));
                p = new Pen(color, b_);
                for (Int32 j = 0; j < l+1; ++j)
                {
                    star[j].X = Convert.ToInt32(Math.Round(j % 2 == 0 ? (x + r1 * Math.Cos(j * 3.14159 / l_)) : (x + r2 * Math.Cos(j * 3.14159 / l_)), 2, MidpointRounding.AwayFromZero));
                    star[j].Y = Convert.ToInt32(Math.Round(j % 2 == 0 ? (y + r1 * Math.Sin(j * 3.14159 / l_)) : (y + r2 * Math.Sin(j * 3.14159 / l_)), 2, MidpointRounding.AwayFromZero));
                }
                g_.DrawLines(p, star);

                if (rnd.Next(0, 100) > 50)
                    g_.FillPolygon(new SolidBrush(color), star);
            }
        }

        public void test()
        {
            g_.FillRectangle(new SolidBrush(Color.FromArgb(0x78ff0000)), 0, 0, w_, h_);
        }
    }
}
