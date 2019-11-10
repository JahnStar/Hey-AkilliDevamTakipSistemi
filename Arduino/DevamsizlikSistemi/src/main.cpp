// Developed by Halil Emre Yildiz
#include <Arduino.h>
#include <SPI.h>
#include <SD.h>
#include <MFRC522.h>
#include <avr/wdt.h>
#include <virtuabotixRTC.h>

int rfidRST = 8;
int rfidSS = 10;

int sdCS = 9;
File dosya;

int uyariLed = 5;
int led = 6;
int zil = 7;

int saat = 4;
int saatData = 3;
int saatRst = 2;

MFRC522 okuyucu(rfidSS,rfidRST);
virtuabotixRTC Saat(saat, saatData, saatRst);

void ModulDegistir(int modulPin)
{
    //Tum moduller (kosullar) elle girilmelidir!
    if (modulPin == rfidSS)
    {
        digitalWrite(sdCS, 1);
        digitalWrite(rfidSS, 0);
    }
    else if (modulPin == sdCS)
    {
        digitalWrite(rfidSS, 1);
        digitalWrite(sdCS, 0);
    }
}
String KartVeriPaketle(byte kimlik0, byte kimlik1, byte kimlik2, byte kimlik3)
{
    return "#KartKimlik(" + String(kimlik0) + "?" + String(kimlik1) + "?" + String(kimlik2) + "?" + String(kimlik3)  + ")";
}
bool IslemiKaydet(String kartKimlik)
{
    bool kaydedildi = false;
    ModulDegistir(sdCS);
    dosya = SD.open("KAYITLAR.TXT", FILE_WRITE);
    if (dosya)
    {   
        String sdYaz_Veri = "(" + kartKimlik + ")" + Saat.dayofmonth + "." + Saat.month + "." + Saat.year + "?" + Saat.hours + ":" + Saat.minutes + ")";
        dosya.println(sdYaz_Veri);
        dosya.flush();
        dosya.close();
        kaydedildi = true;

    }
    return kaydedildi;
}

bool baglanildi = false;
char alinanVeri;
bool BaglantiKur(char pcdenBaglantiIstekMesaji, String pcyeOnayMesaji, char pcdenSifirlamaMesaji, unsigned long snIcindeBaglandiysa) // v2
{
    alinanVeri = Serial.read();
    if (alinanVeri == pcdenSifirlamaMesaji) // Sifirlama mesaji geldiyse
    {
        if (millis() > snIcindeBaglandiysa * 1000)
        {
            wdt_enable(WDTO_15MS); // yeniden baslat
            while(1) { }
        }
        delay(150);
    }
    if (!baglanildi && alinanVeri == pcdenBaglantiIstekMesaji) // Istek mesaji geldiyse
    {
        Serial.println(pcyeOnayMesaji); // Onay Mesaji Gonder
        baglanildi = true;
    }
    return baglanildi;
}
// Kurulum Verileri
String girisAdi = "Yonetici1";
String girisSifresi = "0000";
String girisBilgileri;
void setup()
{
    Serial.begin(9600);
    SPI.begin();

    //Saat.setDS1302Time(6, 22, 16, 2, 1, 10, 2019);

    okuyucu.PCD_Init();
    pinMode(led, OUTPUT);
    pinMode(uyariLed, OUTPUT);
    pinMode(zil, OUTPUT);

    pinMode(sdCS, OUTPUT);
    
    // Kurulum Verileri - Atama
    if (!SD.begin(sdCS)) 
    {
        digitalWrite(led, 0);
        Serial.println("SD kart takili degil yada okuyucu/SD kart calismiyor. SD karti bekleniyor...");
        while(!SD.begin(sdCS))
        {
            digitalWrite(uyariLed, 1);
            digitalWrite(zil, 1);
            tone(zil, 100);        
            delay(300);
            digitalWrite(uyariLed, 0);
            digitalWrite(zil, 0);
            noTone(zil);
            delay(350);
        }
        Serial.println("SD kart takildi. SD kart kullanima hazir.");
        delay(100);
    }

    digitalWrite(led, 1);
    ModulDegistir(sdCS);
    dosya = SD.open("HeyADTS.TXT", FILE_READ);
    girisBilgileri = "#GirisBilgileri(";
    String dosyaGirisAdi = "";
    String dosyaSifre = "";
    while (dosya.available())
    {
        char k = dosya.read();
        girisBilgileri += k; // Giris Bilgilerini Kaydet

        if (k == '/')
        {
            if (dosyaGirisAdi[0] != '>') dosyaGirisAdi = ">"; // 1. '>' okuma
            else if (dosyaSifre[0] != '>') dosyaSifre = ">"; // 2. '>' okuma
        }
        else if (dosyaGirisAdi[0] == '>') 
        {
            if (dosyaSifre[0] == '>' && k != ')') dosyaSifre += k;
            else dosyaGirisAdi += k;
        }
    }
    girisBilgileri += ")";
    dosya.close();

    if (('>' + girisAdi) == dosyaGirisAdi && ('>' + girisSifresi) == dosyaSifre) 
    {
        Serial.println("Hos Geldiniz");

        digitalWrite(led, 1);
        digitalWrite(zil, 1);
        tone(zil, 1000);        
        delay(250);
        digitalWrite(led, 0);
        digitalWrite(zil, 0);
        noTone(zil);
    }
    else while (true)
    {
        digitalWrite(uyariLed, 1);
        digitalWrite(zil, 1);
        tone(zil, 100);        
        delay(250);
        digitalWrite(uyariLed, 0);
        digitalWrite(zil, 0);
        noTone(zil);
        delay(1000);
        delay(1000);
    }
    digitalWrite(led, 0);
}
String oncekiKart_Saat;
void loop()
{
    Saat.updateTime();

    ModulDegistir(rfidSS);
    if (okuyucu.PICC_IsNewCardPresent() && okuyucu.PICC_ReadCardSerial()) // Kart okunduysa
    {
        String kartKimlik = "";
        for (int i = 0; i < 4; i++) 
        {
            if (i > 0) kartKimlik += "?"; 
            kartKimlik += String(okuyucu.uid.uidByte[i]);
        }

        // islem yapilmiyorsa ve veri alinmiyorsa kart bilgilerini gonder
        {
            Serial.println("#KartKimlik(" + kartKimlik + ")");
            Serial.flush();
        }

        if (oncekiKart_Saat == kartKimlik + Saat.minutes) // Ayni karti 1 dakikada icinde 1'den fazla kaydetmez
        {
            digitalWrite(zil, 1);
            digitalWrite(uyariLed, 1);
            tone(zil, 448);
            delay(175);
            noTone(zil);
            digitalWrite(zil, 0);
            delay(25);
            digitalWrite(zil, 1);
            tone(zil, 448);
            delay(200);
            noTone(zil);
            digitalWrite(zil, 0);
            digitalWrite(uyariLed, 0);
            delay(325);
        }
        else if (IslemiKaydet(kartKimlik)) // Sd karta islem bilgilerini kaydet
        {
            oncekiKart_Saat = kartKimlik + Saat.minutes;
            digitalWrite(led, 1);
            digitalWrite(zil, 1); // Aktif buzzer icin
            tone(zil,1024); // Pasif buzzer icin
            delay(325);
            noTone(zil); // Pasif buzzer icin
            digitalWrite(zil, 0);  // Aktif buzzer icin
            delay(250);
            digitalWrite(led, 0);
        }
        else // Kaydetme hatasi
        {
            if (millis() > 1000)
            {
                wdt_enable(WDTO_15MS); // yeniden baslat
                while(1) { }
            }
            delay(150);
        }
    }
    else if (BaglantiKur('+', "Onaylandi", '-', 1))
    {   
        digitalWrite(led, 1);

        if (alinanVeri == '*') // Giris bilgileri istek mesaji geldiyse
        {
            Serial.println(girisBilgileri);
        }
        else if (alinanVeri == '/') // Kayitlar icin istek mesaji geldiyse
        {
            ModulDegistir(sdCS);
            
            dosya = SD.open("KAYITLAR.txt",FILE_READ);
            if (dosya)
            {
                Serial.print("#Kayitlar<");
                Serial.flush(); // Gonderilmesini bekle
                String okunan = "";
                while (dosya.available()) // Her karakter icin dondur
                {
                    char karakter = (char)dosya.read();
                    if (karakter != '\n') okunan += karakter;
                    else
                    {
                        Serial.print(okunan); // Diziyi gonder
                        Serial.flush(); // Gonderilmesini bekle
                        okunan = "";
                        delay(3); // Gecikme
                    }
                } 
                Serial.println(">");
                Serial.flush(); // Gonderilmesini bekle

                digitalWrite(zil, 1);
                tone(zil,768);
                delay(250);        
                noTone(zil);
                digitalWrite(zil, 0);
            }
            dosya.close();      
        }
    }
}