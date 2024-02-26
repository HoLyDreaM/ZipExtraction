using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
//using System.IO;
using System.IO.Compression;
using System.Windows.Forms;

namespace ZipAyiklama
{
    class Program
    {
        public static List<string> ZipDosyalar = new List<string>();
        public static List<string> YeniDosyalar = new List<string>();
        static void Main(string[] args)
        {

            Console.Clear();
            Console.Write("Dosya Listesi Alınıyor..." + Environment.NewLine);

            string[] dosya;
            String yol = DosyaYolu();
            string strDosyaUzantisi = DosyaUzantisi();

            dosya = Directory.GetFiles(yol, strDosyaUzantisi);
            for (int i = 0; i < 1; i++)
            {
                for (int a = 0; a < dosya.Length; a++)
                {
                    ZipDosyalar.Add(dosya[a].ToString());
                }
            }

            Console.Write("Zip İçeriği Okunuyor..." + Environment.NewLine);

            for (int i = 0; i < ZipDosyalar.Count; i++)
            {
                bool blnZipKontrol = ValidateZipFile(ZipDosyalar[i].ToString());
                string strAramaUzantisi = AramaUzantisi();
                string strDegisimUzantisi = DegisimUzantisi();

                if (blnZipKontrol)
                {
                    try
                    {
                        using (ZipArchive archive = ZipFile.OpenRead(ZipDosyalar[i].ToString()))
                        {
                            foreach (var item in archive.Entries)
                            {
                                string strZipFullName = item.FullName.ToString();

                                if (strZipFullName.Contains(strAramaUzantisi))
                                {
                                    if (strAramaUzantisi != strDegisimUzantisi)
                                    {
                                        strZipFullName = strZipFullName.Replace(strAramaUzantisi, strDegisimUzantisi);
                                    }

                                    string[] strZipDosyaAdi = ZipDosyalar[i].ToString().Split('\\');
                                    string[] NewName = strZipFullName.Split('.');
                                    string strYeniAd = NewName[0] + "_" + i.ToString() + "." + NewName[1].ToString();
                                    string strFileName = strZipDosyaAdi[strZipDosyaAdi.Length - 1].ToString() + " # " + strYeniAd;

                                    Console.Write(strFileName + " Zip Dosya İsimleri Belirlendi." + Environment.NewLine);

                                    YeniDosyalar.Add(strFileName);
                                    break;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.Write("Hata Bulundu.Detay:\n" + ex.Message.ToString());
                    }
                }
            }

            try
            {
                Console.Write("Dosya İsimleri Değiştirilmeye Başlandı." + Environment.NewLine);

                for (int i = 0; i < YeniDosyalar.Count; i++)
                {
                    Console.Write("Dosya İsimleri Değiştirilmeye Başlandı." + Environment.NewLine);

                    string[] strOrjFileName = YeniDosyalar[i].Trim().Split('#');

                    string strDosyaYolu = "";

                    for (int a = 0; a < ZipDosyalar.Count; a++)
                    {
                        if (ZipDosyalar[a].ToString().Contains(strOrjFileName[0].ToString().Trim()))
                        {
                            strDosyaYolu = ZipDosyalar[a].ToString();
                            break;
                        }
                    }

                    FileInfo file = new FileInfo(strDosyaYolu);
                    file.Rename(strOrjFileName[1].ToString());
                }

                Console.Write("Toplam :" + YeniDosyalar.Count + " Adet Dosya İsmi Düzenlenmiştir." + Environment.NewLine);
            }
            catch (Exception ex)
            {
                Console.Write("Hata Bulundu.Detay:\n" + ex.Message.ToString());
            }
        }
        private static System.IO.Stream TestStream(string strFilePath)
        {
            System.IO.Stream fs = System.IO.File.OpenRead(strFilePath);
            return fs;
        }
        public static bool ValidateZipFile(string fileToTest)
        {
            bool result;
            try
            {
                using (ICSharpCode.SharpZipLib.Zip.ZipFile zip = new ICSharpCode.SharpZipLib.Zip.ZipFile(fileToTest))
                {
                    result = zip.TestArchive(true, ICSharpCode.SharpZipLib.Zip.TestStrategy.FindFirstError, null);
                }
            }
            catch
            {

                result = false;
            }
            
            return result;
        }
        public static string DosyaYolu()
        {
            string strPath = Application.StartupPath + "\\parameters.json";
            var json = File.ReadAllText(strPath);
            var jObject = JObject.Parse(json);
            string result = jObject["DosyaYolu"].ToString();

            return result;
        }
        public static string DosyaUzantisi()
        {
            string strPath = Application.StartupPath + "\\parameters.json";
            var json = File.ReadAllText(strPath);
            var jObject = JObject.Parse(json);
            string result = jObject["DosyaUzantisi"].ToString();

            return result;
        }
        public static string AramaUzantisi()
        {
            string strPath = Application.StartupPath + "\\parameters.json";
            var json = File.ReadAllText(strPath);
            var jObject = JObject.Parse(json);
            string result = jObject["ZipUzantisi"].ToString();

            return result;
        }
        public static string DegisimUzantisi()
        {
            string strPath = Application.StartupPath + "\\parameters.json";
            var json = File.ReadAllText(strPath);
            var jObject = JObject.Parse(json);
            string result = jObject["Degisim"].ToString();

            return result;
        }
    }
    public static class FileInfoExtensions
    {
        public static void Rename(this FileInfo fileInfo, string newName)
        {
            fileInfo.MoveTo(Path.Combine(fileInfo.Directory.FullName, newName));
        }
    }
}
