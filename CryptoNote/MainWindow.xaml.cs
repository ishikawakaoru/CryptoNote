using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.IO;
using Microsoft.Win32;


namespace CryptoNote
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// .
    public partial class MainWindow : Window
    {
       private string pathFile;


        public MainWindow()
        {
            InitializeComponent();
        }


        private void CoderButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Всё файлы (*.*) |*.*| Файлы Shirase (*.shirase)|*.shirase";
            if (openFileDialog.ShowDialog() == true)
            {
              //  выбор дофигашки файлов
              //  this.openFileDialog1.Multiselect = true;
              //  foreach (String file in openFileDialog1.FileNames)
              //  {
              //      MessageBox.Show(file);
              //  }

                pathFile = openFileDialog.FileName;
                OpenFile.Text = pathFile.ToString();

                DisplayFile(pathFile);
            }

        }

        void DisplayFile(string strfile)
        {
            int nCols = 16;
            FileStream fs = new FileStream(strfile, FileMode.Open, FileAccess.Read);
            long nBytesRead = fs.Length;
            if (nBytesRead > 65536 / 4)
                nBytesRead = 65536 / 4;
            int nLines = (int)(nBytesRead / nCols) + 1;
            string[] lines = new string[nLines];
            int nBytesToRead = 0;

            //for (int i = 0; i < bytesAsInts.Length; i++)

            for (int i = 0; i < nLines; i++)
            {
                StringBuilder nextLine = new StringBuilder();
                nextLine.Capacity = 2 * nCols;

                for (int j = 0; j < nLines; j++)
                {
                    int nextByte = fs.ReadByte();
                    nBytesToRead++;
                    if (nextByte < 0 || nBytesToRead > 65536)
                        break;
                    char nextChar = (char)nextByte;
                    nextLine.Append(" 0x0" + string.Format("{0,1:X}", (int)nextChar));
                    //nextLine.Append((int)nextChar);

                }
                lines[i] = nextLine.ToString();
            }

            fs.Close();

            string text = "";
            foreach (string l in lines)
                text += l;

            OutputText.Text = text;
        }

        public void Copy(string inputFilePath, string outputFilePath, bool CoderMode)
		{
			int bufferSize = 1024 * 64;

			using (FileStream inStream = new FileStream(inputFilePath, FileMode.Open, FileAccess.Read))
			{

				using (FileStream fileStream = new FileStream(outputFilePath, FileMode.OpenOrCreate, FileAccess.Write))
				{

					int bytesRead = -1;
					byte[] bytes = new byte[bufferSize];
					string decodeString = key1.Text;
				if (decodeString.Count() < 18)
					{
						MessageBox.Show("Длина цифрового ключа должна быть 18 символа");
						return;

					}

					if (!char.IsDigit(key1.Text, 0))
					{

						MessageBox.Show("КЗПГ");
						return;
					}

					double decodeInt = Convert.ToDouble(decodeString);
					int ln = (int)(Math.Log(Math.Log(decodeInt, 2)));
					while ((bytesRead = inStream.Read(bytes, 0, bufferSize)) > 0)
					{
						
						int[] bytesAsInts = bytes.Select(x=>(int)x).ToArray();

						if (CoderMode == false)
						{

							for (int i = 0; i < bytesAsInts.Length; i++)
							{
								bytesAsInts[i] = ((bytesAsInts[i] - (ln * 2) - 128) * -1);
							}
						}

						else
						{
							for (int i = 0; i < bytesAsInts.Length; i++)
							{
								bytesAsInts[i] = ((bytesAsInts[i] * (-1) + ln * 2) + 128);
							}
						}

						byte[] bytes1 = bytesAsInts.Select(x=>(byte)x).ToArray();
						fileStream.Write(bytes1, 0, bytesRead);
						fileStream.Flush();
					}

					fileStream.Close();


					}
            }
		}

        public static bool IsWithin(long value, int minimum, int maximum)
        {
            return value >= minimum && value <= maximum;
        }

        /// <summary>
        /// Кодировка HEX
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CoderButton_Copy_Click(object sender, RoutedEventArgs e)
        {
            // кодировка с изменением байтов

            Copy(pathFile, OpenFile.Text + ".shirase", false);
        }
        /// <summary>
        /// Декодировка HEX
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CoderButton_Copy1_Click(object sender, RoutedEventArgs e)
        {
            // декодировка с измененеием байтов
            string newPathFile = pathFile.Replace(".shirase", "");
            Copy(pathFile, newPathFile, true);
        }

        /// <summary>
        /// Основная DEADSEC Decoder
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void deadsecDECSTKOV_Click(object sender, RoutedEventArgs e)
        {
            DeadSecDecodingSTKOV(pathFile);
        }


        
        /// <summary>
        /// Основная DEADSEC Coder
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void deadsec_Click(object sender, RoutedEventArgs e)
        {
            DeadSecCoding(pathFile);
        }

        public static void Writer(string nextbyte, string strfile)
        {

            StreamWriter sw = new StreamWriter(strfile, true);
            sw.Write(nextbyte);
            sw.Flush();
            sw.Close();
        }

        void DeadSecDecodingSTKOV(string strfile) 
        {

            string newstrFile = strfile.Replace(".gz", "");

            GZip.Decompress(strfile, newstrFile);
            File.Delete(strfile);


            using (var inputStream = new FileStream(newstrFile, FileMode.Open, FileAccess.Read))
            using (var reader = new BinaryReader(inputStream))
            using (var outputStream = new FileStream(strfile.Replace(".crypto.gz",""), FileMode.Create, FileAccess.Write))
            {
                var sb = new StringBuilder();

                while (reader.PeekChar() != -1)
                {
                    char[] chars = reader.ReadChars(1024 * 16);

                    foreach (char c in chars)
                    {
                        if (c != '@')
                            sb.Append(c);
                        else
                        {
                            int number = int.Parse(sb.ToString()) / 1234567;
                            byte b = (byte)number;
                            outputStream.WriteByte(b);
                            sb.Clear();
                        }
                    }
                }

                if (sb.Length > 0)
                {
                    int number = int.Parse(sb.ToString()) / 1234567;
                    byte b = (byte)number;
                    outputStream.WriteByte(b);
                }
            }
            File.Delete(newstrFile);
            OutputText.Text = "Декодировка завершилась успешно!";
        }


        
  void DeadSecCoding(string strfile) 
     {

           using (FileStream  fs = new FileStream(strfile, FileMode.Open, FileAccess.Read))
      {
           using (StreamWriter  sw = new StreamWriter(strfile + ".crypto", true))
           {

               long nBytesRead = fs.Length;
               int nBytesToRead = 0;

               for (int i = 0; i < nBytesRead; i++)
               {
                   int nextByte = fs.ReadByte();

                   nextByte *= 1234567;

                   sw.Write(nextByte.ToString() + '@'); 
                   nBytesToRead++;

               }
           }
           fs.Close();
      }

 

            //   Writer(nextLine.ToString());   // эта пишет стразу опкод

           GZip.Compress(strfile + ".crypto", strfile + ".crypto.gz");
           File.Delete(strfile + ".crypto");

            OutputText.Text = "ГОТОВО!";
        }


        //////////////////////////////////////////// CRYPTOTEXT ////////////////////////////////////////////////
        public string tmptext;
        public void CryptoText(bool mode)
        {


            if (mode == true)
            {
                string str = OutputText.Text;
                char[] arr;

                arr = str.ToCharArray();

                // преобразуем символ в его код и отобразим

                foreach (int i in arr)
                {


                    int c = i * (5); 
                    char b2 = (char)c;

                    // tmptext содержит кодированное сообщение
                    tmptext += (b2).ToString();

                }

                IntegetText.Text = tmptext;
            }
            else
            {
                string str = IntegetText.Text;
                char[] arr;

                arr = str.ToCharArray();

                foreach (int i in arr)
                {
                    int a = i/(5);
                    
                    char b2 = (char)a;
                    OutputText.Text += b2.ToString();
                }
            }
        }


        private void CoderButton_Copy2_Click(object sender, RoutedEventArgs e)
        {
            tmptext = "";
            CryptoText(true);
        }

        private void CoderButton_Copy3_Click(object sender, RoutedEventArgs e)
        {
            tmptext = "";
            CryptoText(false);
        }


        /////////////////////////////////////////////////////////////////////////////////////////////////////////
     


    }
}