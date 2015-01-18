using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public int[,] input = new int[3, 5];       // ustalamy rozmiar tablice 3 na 5
        Neuron NW1;

        private void button1_Click(object sender, EventArgs e)  // w przypadku nacisnięcia "Otworz"
        {
            listBox1.Items.Clear();
            button2.Enabled = true;
            openFileDialog1.Title = "Wybierz testujemy plik";
            openFileDialog1.ShowDialog();
           
            pictureBox1.Image = Image.FromFile(openFileDialog1.FileName);
            Bitmap im = pictureBox1.Image as Bitmap;
            for (var i = 0; i <= 5; i++) listBox1.Items.Add(" ");

            for (var x = 0; x <= 2; x++)
            {
                for (var y = 0; y <= 4; y++)
                {
                    // listBox1.Items.Add(Convert.ToString(im.GetPixel(x, y).R));
                    int n = (im.GetPixel(x, y).R);
                    if (n >= 250) n = 0;
                    else n = 1;
                    listBox1.Items[y] = listBox1.Items[y] + "  " + Convert.ToString(n);
                    input[x, y] = n;                            //    Przypisujemy odpowiednie znaczenie każdrj części wejściowych danych
                    //if (n == 0) input[x, y] = 1;
                }

            }

            recognize();
        }

        // Rozpoznajemy symbol, wywołując opisane niżej metode klasy
        public void recognize()
        {
            NW1.sygnal_n();
            NW1.Sum();
            if (NW1.Porownanie()) listBox1.Items.Add(" - True, Sum = "+Convert.ToString(NW1.sum));
            else listBox1.Items.Add( " - False, Sum = "+Convert.ToString(NW1.sum));
        }

        class Neuron
        {
            public int[,] sygnal;        // tutaj bedziemy przechowywac sygnaly
            public int[,] waga;     // zbiór dla przechowywania wag
            public int[,] input;      // wejsciowa informacja
            public int limit = 9;     // granica 
            public int sum;           // tutaj zachowamy sume sygnalow

            public Neuron(int sizex, int sizey, int[,] inP)       // Zadajemy właściwości podczas tworzenia obiektu
            {
                waga = new int[sizex, sizey];                // ustalamy rozmiar tablicy
                sygnal = new int[sizex, sizey];

                input = new int[sizex, sizey];
                input = inP;                                   // otrzymujemy dane wejsciowe
            }

            // sprawdzanie sygnalow
            public void sygnal_n()
            {
                for (int x = 0; x <= 2; x++)                    // sprawdzamy kazdy akson(sygnal z kazdej pikseli)
                {
                    for (int y = 0; y <= 4; y++)
                    {
                        sygnal[x, y] = input[x, y] * waga[x, y];  // Mnożymy sygnał na 0 lub 1 i zapisujemy w tablice.
                                                             
                    }
                }
            }

            public void Sum()         // sumujemy sygnaly
            {
                sum = 0;
                for (int x = 0; x <= 2; x++)
                {
                    for (int y = 0; y <= 4; y++)
                    {
                        sum += sygnal[x, y];
                    }
                }
            }

            public bool Porownanie()        // porownujemy sume sygnalow z limitem
            {
                if (sum >= limit)
                    return true;
                else return false;
            }
            public void incW(int[,] inP)
            {
                for (int x = 0; x <= 2; x++)
                {
                    for (int y = 0; y <= 4; y++)
                    {
                        waga[x, y] += inP[x, y];
                    }
                }
            }
            public void decW(int[,] inP)
            {
                for (int x = 0; x <= 2; x++)
                {
                    for (int y = 0; y <= 4; y++)
                    {
                        waga[x, y] -= inP[x, y];
                    }
                }
            }

        }


        // zachowujemy znaczenie w plik tekstowy
        private void Form1_Load(object sender, EventArgs e)
        {
           

            NW1 = new Neuron(3, 5,input);                       // nowy egzemplarz dla tablicy

            openFileDialog1.Title = "Wybierz plik wagi";
            openFileDialog1.ShowDialog();
            string s = openFileDialog1.FileName;
            StreamReader sr = File.OpenText(s);                 // pobieramy plik wagi
            string line;
            string[] s1;
            int k = 0;
            while ((line = sr.ReadLine()) != null)
            {
               
                s1 = line.Split(' ');
                for (int i = 0; i < s1.Length; i++)
                {
                    listBox1.Items.Add("");
                    if (k < 5)
                    {
                        NW1.waga[i, k] = Convert.ToInt32(s1[i]);                    // Przeznaczamy każdemu związkowi jej zapisaną wczeszniej wagę
                        listBox1.Items[k] += Convert.ToString(NW1.waga[i, k]);      // Wyprowadzamy wagi
                    }

                }
                k++;

            }
            sr.Close();
        }

        private void button2_Click(object sender, EventArgs e)   // w przypadku nacisnięcia "nie prawda"
        {
            button2.Enabled = false;

                if (NW1.Porownanie() == false)
                    NW1.incW(input);                 
                else NW1.decW(input);
               
                //zapisujemy zmiany w plik tablicy
                  string s="";
                  string[] s1 = new string[5];
                  System.IO.File.Delete("wagi.txt");
                  FileStream FS = new FileStream("wagi.txt", FileMode.OpenOrCreate);
                  StreamWriter SW = new StreamWriter(FS);

                for (int y = 0; y <= 4; y++)
                {

                    s = Convert.ToString(NW1.waga[0, y]) + " " + Convert.ToString(NW1.waga[1, y]) + " " + Convert.ToString(NW1.waga[2, y]) ;
                        

                    s1[y] = s;
                   
                    SW.WriteLine(s);

                }
                SW.Close();


            
        }
    }

}
