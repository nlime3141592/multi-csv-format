using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace nl
{
    public class Program
    {
        private static void TryPrint<T>(MultiCSVReader reader)
        {
            List<T> list;

            if (reader.TryParseContents<T>(out list))
            {
                foreach (T element in list)
                {
                    Console.WriteLine($"{element.ToString()}");
                }
            }

            reader.TryParseEndOfLine();
        }

        private static void Main(string[] args)
        {
            string path3 = @"D:\Programming\multi-csv-format\multicsv\test3.multicsv";

            FileStream fs = new FileStream(path3, FileMode.Open, FileAccess.Read, FileShare.Read);
            MultiCSVReader rd = new MultiCSVReader(fs);

            fs.Position = 0;

            TryPrint<Player>(rd);
            TryPrint<Item>(rd);
            TryPrint<Achivement>(rd);

            Console.WriteLine("Program Ends.");
        }

        private static void Main3(string[] args)
        {
            string path1 = @"D:\Programming\multi-csv-format\multicsv\test1.multicsv";
            string path2 = @"D:\Programming\multi-csv-format\multicsv\test2.multicsv";

            FileStream fs = new FileStream(path1, FileMode.Open, FileAccess.Read, FileShare.Read);

            byte[] buffer = new byte[fs.Length];
            int rdLength = fs.Read(buffer, 0, buffer.Length);
            fs.Close();

            char[] chars = Decoding.ToUnicodeFromUtf8(buffer);

            fs = new FileStream(path2, FileMode.Create, FileAccess.Write);
            StreamWriter wr = new StreamWriter(fs, Encoding.UTF8);
            wr.Write(chars);
            wr.Close();
            fs.Close();
        }

        private static void Main2(string[] args)
        {
            byte[] utf8 = new byte[] { 0xEC, 0x95, 0x88, 0xEB, 0x85, 0x95, 0xED, 0x95, 0x98, 0xEC, 0x84, 0xB8, 0xEC, 0x9A, 0x94 };
            byte[] utf16le = new byte[] { 0x48, 0xC5, 0x55, 0xB1, 0x58, 0xD5, 0x38, 0xC1, 0x94, 0xC6 };
            byte[] utf16be = new byte[] { 0xC5, 0x48, 0xB1, 0x55, 0xD5, 0x58, 0xC1, 0x38, 0xC6, 0x94 };

            // char[] message1 = Decoding.ToUnicodeFromUtf8(utf8);
            // char[] message2 = Decoding.ToUnicodeFromUtf16Le(utf16le);
            // char[] message3 = Decoding.ToUnicodeFromUtf16Be(utf16be);
            // Console.WriteLine(message1);
            // Console.WriteLine(message2);
            // Console.WriteLine(message3);

            int length1 = Encoding.UTF8.GetCharCount(utf8, 0, utf8.Length);
            Console.WriteLine(length1);
        }

        private static void Main1(string[] args)
        {
            string path1 = @"D:\Programming\multi-csv-format\multicsv\test1.multicsv";
            string path2 = @"D:\Programming\multi-csv-format\multicsv\test2.txt";

            string message = "안녕하세요";

            FileStream fs = new FileStream(path2, FileMode.Create, FileAccess.Write);
            char[] arraya = message.ToCharArray();
            byte[] arrayb = Encoding.UTF8.GetBytes(arraya);
            byte[] arrayc = Encoding.Unicode.GetBytes(arraya);
            fs.Write(arrayb, 0, arrayb.Length);
            fs.Write(arrayc, 0, arrayc.Length);
            Console.WriteLine($"Length of A == {arraya.Length}, Lenght of B == {arrayb.Length}, Length of C == {arrayc.Length}");
            fs.Close();
        }
    }
}