using System;
using System.Timers;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using SharpDX;

using D2DFactory = SharpDX.Direct2D1.Factory;
using D2DHwndRenderTargetProperties = SharpDX.Direct2D1.HwndRenderTargetProperties;
using D2DRenderTargetProperties = SharpDX.Direct2D1.RenderTargetProperties;
using D2DPixelFormat = SharpDX.Direct2D1.PixelFormat;
using D2DHwndRenderTarget = SharpDX.Direct2D1.WindowRenderTarget;

namespace Yozora
{
    public partial class LineDotPanel : UserControl
    {
        float lineWidth = 0.3f;
        double k;
        double dots_distance = 100;
        double dots_d_radius = 150;
        List<Dot> dots_array;
        Random random;
        System.Timers.Timer animateTimer;
        D2DFactory factory;
        D2DHwndRenderTargetProperties hwndRenderProps;
        D2DHwndRenderTarget renderTarget;
        D2DRenderTargetProperties renderProps = new D2DRenderTargetProperties
        {
            PixelFormat = new D2DPixelFormat(SharpDX.DXGI.Format.B8G8R8A8_UNorm, SharpDX.Direct2D1.AlphaMode.Premultiplied),
            Usage = SharpDX.Direct2D1.RenderTargetUsage.None,
            Type = SharpDX.Direct2D1.RenderTargetType.Default
        };


        public LineDotPanel()
        {
            InitializeComponent();
            

            factory = new D2DFactory();

            hwndRenderProps = new SharpDX.Direct2D1.HwndRenderTargetProperties
            {
                Hwnd = Handle,
                PixelSize = new SharpDX.Size2(Width, Height),
                PresentOptions = SharpDX.Direct2D1.PresentOptions.None
            };

            renderTarget = new D2DHwndRenderTarget(factory, renderProps, hwndRenderProps)
            {
                AntialiasMode = SharpDX.Direct2D1.AntialiasMode.PerPrimitive,
                StrokeWidth = lineWidth
            };

            k = Math.Max(Width, Height) * 0.15;
            if (k > 900.0)
            {
                k = 900.0;
            }
            random = new Random((int)DateTime.Now.ToBinary());
            dots_array = new List<Dot>();
            CreateDots();
            animateTimer = new System.Timers.Timer()
            {
                Interval = 16,
                AutoReset = true,
                Enabled = true
            };
            animateTimer.Elapsed += AnimateIt;
            animateTimer.Start();
        }

        private void AnimateIt(object sender, ElapsedEventArgs e)
        {
            AnimateDots();
        }

        private Color ColorValue(int min)
        {
            return Color.FromArgb(
                204,
                random.Next(min, 255),
                random.Next(min, 255),
                random.Next(min, 255)
            );
        }

        private double MixComponent(double comp1, double weight1, double comp2, double weight2)
        {
            return (comp1 * weight1 + comp2 * weight2) / (weight1 + weight2);
        }

        private Color AverageColor(Dot dot1, Dot dot2)
        {
            return Color.FromArgb(
                204, 
                (int)Math.Floor(MixComponent(dot1.Color.R, dot1.Radius, dot2.Color.R, dot2.Radius)),
                (int)Math.Floor(MixComponent(dot1.Color.G, dot1.Radius, dot2.Color.G, dot2.Radius)),
                (int)Math.Floor(MixComponent(dot1.Color.B, dot1.Radius, dot2.Color.B, dot2.Radius))
            );
        }

        private void CreateDots()
        {
            for(int i=0;i<k;i++)
            {
                dots_array.Add(
                    new Dot(
                        Width, 
                        Height, 
                        Color.FromArgb(
                            204, 
                            ColorValue(150)
                        )
                    )
                );
            }
        }

        private void MoveDots()
        {
            foreach (Dot dot in dots_array)
            {
                if (dot.Y < 0 || dot.Y > Height)
                {
                    dot.Vy = -dot.Vy;
                }
                else if (dot.X < 0 || dot.X > Width)
                {
                    dot.Vx = -dot.Vx;
                }
                dot.X += dot.Vx;
                dot.Y += dot.Vy;
            }
        }

        private void ConnectDots(Point mousePosition)
        {
            long count = 0;
            foreach (Dot dot1 in dots_array)
            {
                foreach (Dot dot2 in dots_array)
                {
                    if ((dot1.X - dot2.X) < dots_distance && 
                        (dot1.Y - dot2.Y) < dots_distance &&
                        (dot1.X - dot2.X) > -dots_distance && 
                        (dot1.Y - dot2.Y) > -dots_distance)
                    {
                        if ((dot1.X - mousePosition.X) < dots_d_radius && 
                            (dot1.Y - mousePosition.Y) < dots_d_radius &&
                            (dot1.X - mousePosition.X) > -dots_d_radius && 
                            (dot1.Y - mousePosition.Y) > -dots_d_radius)
                        {
                            count++;
                            Color c = AverageColor(dot1, dot2);
                            SharpDX.Direct2D1.SolidColorBrush brush = new SharpDX.Direct2D1.SolidColorBrush(renderTarget, ColorToRawColor4(c), new SharpDX.Direct2D1.BrushProperties() { Opacity=0.8f });
                            renderTarget.DrawLine(new SharpDX.Mathematics.Interop.RawVector2(dot1.X, dot1.Y), new SharpDX.Mathematics.Interop.RawVector2(dot2.X, dot2.Y), brush);
                            brush.Dispose();
                        }
                    }
                }
            }
        }

        private SharpDX.Mathematics.Interop.RawColor4 ColorToRawColor4(Color c)
        {
            return new SharpDX.Mathematics.Interop.RawColor4((float)c.R / 256f, (float)c.G / 256f, (float)c.B / 256f, (float)c.A / 256f);
        }

        private void DrawDots()
        {
            foreach (Dot dot in dots_array)
            {
                Color c = dot.Color;
                SharpDX.Direct2D1.SolidColorBrush brush = new SharpDX.Direct2D1.SolidColorBrush(renderTarget, ColorToRawColor4(c), new SharpDX.Direct2D1.BrushProperties() { Opacity = 0.8f });
                renderTarget.DrawEllipse(new SharpDX.Direct2D1.Ellipse(new SharpDX.Mathematics.Interop.RawVector2(dot.X, dot.Y), dot.Radius, dot.Radius), brush);
                brush.Dispose();
            }
        }

        private void AnimateDots()
        {
            renderTarget.BeginDraw();
            renderTarget.Clear(ColorToRawColor4(Color.Transparent));
            MoveDots();
            ConnectDots(MousePosition);
            DrawDots();
            renderTarget.EndDraw();
        }

        private void LineDot_Load(object sender, EventArgs e)
        {
            
        }
    }

    public class Dot
    {
        private float x, y;
        private float vx, vy;
        private float radius;
        private Color color;

        static Random random = new Random((int)DateTime.Now.ToBinary());


        public float X { get { return x; } set { x = value; } }
        public float Vx { get { return vx; } set { vx = value; } }
        public float Radius { get { return radius; } set { radius = value; } }
        public Color Color { get { return color; } set { color = value; } }
        public float Y { get { return y; } set { y = value; } }
        public float Vy { get { return vy; } set { vy = value; } }


        public Dot(int width, int height, Color color)
        {
            x = (float)random.NextDouble() * width;
            y = (float)random.NextDouble() * height;
            vx = -0.5f + (float)random.NextDouble();
            vy = -0.5f + (float)random.NextDouble();
            radius = (float)random.NextDouble() * 2;
            this.color = color;
        }
    }
}
