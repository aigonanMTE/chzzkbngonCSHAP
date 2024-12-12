using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace chzzkbangonallramTEST
{
    static class Program
    {
        static Mutex mutex = new Mutex(true, "Global\\UniqueApplicationName");

        [STAThread]
        static void Main()
        {
            // 프로그램 중복 실행 확인
            if (!mutex.WaitOne(TimeSpan.Zero, true))
            {
                // 이미 실행 중인 창을 찾아서 활성화
                IntPtr hWnd = NativeMethods.FindWindow(null, "치지직뱅온알람");

                if (hWnd != IntPtr.Zero)
                {
                    // 실행 중인 창을 활성화
                    NativeMethods.SetForegroundWindow(hWnd);
                }
                else
                {
                    // MessageBox.Show("실행 중인 창을 찾을 수 없습니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                MessageBox.Show("이미 프로그램을 실행 중 입니다. 트레이 메뉴를 확인 하세요");

                return; // 중복 실행 시 종료
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());

            GC.KeepAlive(mutex); // Mutex가 가비지 컬렉션되지 않도록 유지
        }

        public class NativeMethods
        {
            [DllImport("user32.dll")]
            public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool SetForegroundWindow(IntPtr hWnd);
        }
    }
}
