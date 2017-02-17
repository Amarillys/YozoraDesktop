using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yozora
{
    class Initer
    {
        static public void Start()
        {
            //PrintVisibleWindowHandles(2);
            Int32 width = Int32.Parse(System.Configuration.ConfigurationManager.AppSettings["width"]);
            Int32 height = Int32.Parse(System.Configuration.ConfigurationManager.AppSettings["height"]);
            Int32 frame = Int32.Parse(System.Configuration.ConfigurationManager.AppSettings["framelimit"]);
            float stars = float.Parse(System.Configuration.ConfigurationManager.AppSettings["stars"]);
            float dots = float.Parse(System.Configuration.ConfigurationManager.AppSettings["dots"]);

            IntPtr progman = W32.FindWindow("Progman", null);
            IntPtr result = IntPtr.Zero;
            W32.SendMessageTimeout(progman,
                                   0x052C,
                                   new IntPtr(0),
                                   IntPtr.Zero,
                                   W32.SendMessageTimeoutFlags.SMTO_NORMAL,
                                   1000,
                                   out result);
            IntPtr workerw = IntPtr.Zero;

            W32.EnumWindows(new W32.EnumWindowsProc((tophandle, topparamhandle) =>
            {
                IntPtr p = W32.FindWindowEx(tophandle,
                                            IntPtr.Zero,
                                            "SHELLDLL_DefView",
                                            IntPtr.Zero);

                if (p != IntPtr.Zero)
                {
                    // Gets the WorkerW Window after the current one.
                    workerw = W32.FindWindowEx(IntPtr.Zero,
                                               tophandle,
                                               "WorkerW",
                                               IntPtr.Zero);
                }

                return true;
            }), IntPtr.Zero);

            IntPtr dc = W32.GetDCEx(workerw, IntPtr.Zero, (W32.DeviceContextValues)0x403);
            if (dc != IntPtr.Zero)
            {
                // Create a Graphics instance from the Device Context
                using (Graphics g = Graphics.FromHdc(dc))
                {

                    // Use the Graphics instance to draw a white rectangle in the upper 
                    // left corner. In case you have more than one monitor think of the 
                    // drawing area as a rectangle that spans across all monitors, and 
                    // the 0,0 coordinate beeing in the upper left corner.
                    DotMgr dg = new DotMgr();
                    dg.Init(g, width, height, frame, stars, dots);
                    dg.DrawStar();
                }
                // make sure to release the device context after use.
                W32.ReleaseDC(workerw, dc);
            }
        }
    }
}
