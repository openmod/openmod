using System;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

#if !LINUX
namespace Rocket.Core
{
    public class Debugger : MonoBehaviour
    {

        public static IDictionary<IntPtr, string> GetOpenWindows()
        {
            IntPtr shellWindow = GetShellWindow();
            Dictionary<IntPtr, string> windows = new Dictionary<IntPtr, string>();

            EnumWindows(delegate (IntPtr hWnd, int lParam)
            {
                if (hWnd == shellWindow) return true;
                if (!IsWindowVisible(hWnd)) return true;

                int length = GetWindowTextLength(hWnd);
                if (length == 0) return true;

                StringBuilder builder = new StringBuilder(length);
                GetWindowText(hWnd, builder, length + 1);

                windows[hWnd] = builder.ToString();
                return true;

            }, 0);

            return windows;
        }

        private delegate bool EnumWindowsProc(IntPtr hWnd, int lParam);

        [DllImport("USER32.DLL")]
        private static extern bool EnumWindows(EnumWindowsProc enumFunc, int lParam);

        [DllImport("USER32.DLL")]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("USER32.DLL")]
        private static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("USER32.DLL")]
        private static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("USER32.DLL")]
        private static extern IntPtr GetShellWindow();

        private DateTime lastUpdate = DateTime.Now;

        public void Awake()
        {
            if (GetOpenWindows().Where(k => k.Value == "Select Unity Instance").Count() == 0)
            {
                R.Instance.Initialize();
                Destroy(this);
                return;
            }
            Console.Write("Waiting for debugger...");
        }

        public void FixedUpdate()
        {
            if ((DateTime.Now - lastUpdate).TotalSeconds > 3)
            {
                Console.Write(".");
                lastUpdate = DateTime.Now;
            }

            if (System.Diagnostics.Debugger.IsAttached)
            {
                Console.WriteLine("\nDebugger found, continuing...");
                R.Instance.Initialize();
                Destroy(this);
            }
        }
    }
}
#endif