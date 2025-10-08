# Keylogger Uygulaması - Eğitim Amaçlı

⚠️ **ÖNEMLİ UYARI**: Bu uygulama sadece eğitim ve araştırma amaçlıdır. Başkasının bilgisayarında izinsiz kullanmak yasadışıdır ve ciddi yasal sonuçlar doğurur.

## Özellikler

- Windows düşük seviye klavye hook'u kullanarak tuş kayıtları
- Log dosyalarına kayıt
- Belirli aralıklarla otomatik email gönderimi
- Konsol tabanlı, kolay kullanım

## Kurulum ve Çalıştırma

### 1. Projeyi Aç
Visual Studio'da `KeyloggerApp.csproj` dosyasını açın.

### 2. Email Ayarlarını Yapılandır

`App.config` dosyasını düzenleyin:

```xml
<add key="SenderEmail" value="sizin-email@gmail.com" />
<add key="SenderPassword" value="uygulama-sifreniz" />
<add key="ReceiverEmail" value="sizin-email@gmail.com" />
```

#### Gmail için Uygulama Şifresi Alma:
1. Google Hesabınıza gidin (https://myaccount.google.com/)
2. Güvenlik sekmesine tıklayın
3. "2 Adımlı Doğrulama"yı etkinleştirin (eğer değilse)
4. "Uygulama şifreleri" bölümüne gidin
5. "Posta" için yeni bir uygulama şifresi oluşturun
6. Oluşturulan 16 haneli şifreyi `App.config` dosyasına yapıştırın

### 3. Çalıştırma

Visual Studio'da **F5** tuşuna basarak uygulamayı çalıştırın.

Veya komut satırından:
```
dotnet run
```

### 4. Durdurma

Konsol ekranında **Q** tuşuna basın.

## Nasıl Çalışır?

1. Uygulama başlatıldığında Windows klavye hook'u kurulur
2. Her tuş basımı yakalanır ve kaydedilir
3. Kayıtlar hem dosyaya yazılır hem bellekte tutulur
4. Her 5 dakikada bir (varsayılan), biriken kayıtlar email ile gönderilir
5. Log dosyaları `Logs/` klasöründe saklanır

## Email Gönderim Süresi

`KeyloggerService.cs` dosyasında 74. satırda değiştirebilirsiniz:

```csharp
_emailTimer = new System.Timers.Timer(300000); // 5 dakika (milisaniye cinsinden)
```

- 1 dakika = 60000
- 5 dakika = 300000
- 10 dakika = 600000

## Dosya Yapısı

```
KeyloggerApp/
├── Program.cs              # Ana giriş noktası
├── KeyloggerService.cs     # Klavye hook ve log yönetimi
├── EmailSender.cs          # Email gönderim servisi
├── App.config              # Konfigürasyon dosyası
└── Logs/                   # Log dosyalarının saklandığı klasör
```

## Sorun Giderme

### Email Gönderilmiyor
- App.config'deki email ve şifre bilgilerini kontrol edin
- Gmail için "Uygulama Şifresi" kullandığınızdan emin olun
- 2 Adımlı Doğrulama'nın açık olduğundan emin olun
- İnternet bağlantınızı kontrol edin

### Tuşlar Kaydedilmiyor
- Uygulamayı **Yönetici olarak çalıştırın**
- Antivirüs yazılımınız engelliyor olabilir (geçici olarak devre dışı bırakın)

## Yasal Uyarı

Bu yazılım sadece eğitim ve kişisel test amaçlıdır. Kullanıcı, bu yazılımı kullanarak doğabilecek tüm yasal sorumluluğu kabul eder. Geliştirici, yasadışı kullanımdan sorumlu tutulamaz.

**SADECE KENDİ BİLGİSAYARINIZDA KULLANIN!**


