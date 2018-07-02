using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;
using System.IO;

namespace PicoloVideo
{
    public partial class Form1 : Form
    {
        string path = Environment.CurrentDirectory + "/" + "druk1.txt";

        int od;

        public Form1()
        {
            InitializeComponent();            

            try
            {
                using (StreamReader sr = new StreamReader(path))
                {
                    textBox1.Text = sr.ReadLine();
                    textBox2.Text = sr.ReadLine();
                    textBox_odstep.Text = sr.ReadLine();
                    textBoxWysObrazka.Text = sr.ReadLine();
                    textBoxSzerObrazka.Text = sr.ReadLine();
                    textBoxRozmiarCzcionki.Text = sr.ReadLine();                   
                }
            }

            catch
            {
                textBox1.Text = "Centrum Medyczne EVITA";
                textBox2.Text = "Wydruk z badań:";
                textBox_odstep.Text = "50";
                textBoxWysObrazka.Text = "200";
                textBoxSzerObrazka.Text = "250";
                textBoxRozmiarCzcionki.Text = "16";
            }

        }

        private void BtnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                PrintDialog pd = new PrintDialog();
                PrintDocument doc = new PrintDocument();
                doc.PrintPage += Doc_PrintPage;

                pd.Document = doc;
                if (pd.ShowDialog() == DialogResult.OK)
                    doc.Print();
            }
            catch { }
        }

        private void Doc_PrintPage(object sender, PrintPageEventArgs e)
        {
            //throw new NotImplementedException();
            Bitmap bm = new Bitmap(pic11.Width * 3, pic11.Height * 3);

            //pic11.DrawToBitmap(bm, new Rectangle(0, 0, pic11.Width, pic11.Height));
            //pic12.DrawToBitmap(bm, new Rectangle(pic11.Width, 0, pic11.Width, pic11.Height));
            //pic13.DrawToBitmap(bm, new Rectangle(pic11.Width * 2, 0, pic11.Width, pic11.Height));
            //pic21.DrawToBitmap(bm, new Rectangle(0, pic11.Height, pic11.Width, pic11.Height));
            //pic22.DrawToBitmap(bm, new Rectangle(pic11.Width, pic11.Height, pic11.Width, pic11.Height));
            //pic23.DrawToBitmap(bm, new Rectangle(pic11.Width * 2, pic11.Height, pic11.Width, pic11.Height));
            //pic31.DrawToBitmap(bm, new Rectangle(0, pic11.Height * 2, pic11.Width, pic11.Height));
            //pic32.DrawToBitmap(bm, new Rectangle(pic11.Width, pic11.Height * 2, pic11.Width, pic11.Height));
            //pic33.DrawToBitmap(bm, new Rectangle(pic11.Width * 2, pic11.Height * 2, pic11.Width, pic11.Height));

            int x = int.Parse(textBoxSzerObrazka.Text);//pic11.Width + 5*(pic11.Width / 7) ;// 110;
            int y = int.Parse(textBoxWysObrazka.Text);//pic11.Height+ 5*(pic11.Height / 7) ;// 110;

            od = int.Parse(textBox_odstep.Text);
            if (od < 50) od = 50;

            if (pic11.Image !=null) e.Graphics.DrawImage(pic11.Image, 10, od, x, y);
            if (pic12.Image !=null) e.Graphics.DrawImage(pic12.Image, 10 + x+10, od, x, y);
            if (pic13.Image !=null) e.Graphics.DrawImage(pic13.Image, 10 + 2*x+20, od, x, y);

            if (pic21.Image !=null) e.Graphics.DrawImage(pic21.Image, 10, od+y+10, x, y);
            if (pic22.Image !=null) e.Graphics.DrawImage(pic22.Image, 10 + x + 10, od+y+10, x, y);
            if (pic23.Image !=null) e.Graphics.DrawImage(pic23.Image, 10 + 2 * x + 20, od+y+10, x, y);
            
            if (pic31.Image !=null) e.Graphics.DrawImage(pic31.Image, 10, od + 2*y + 20, x, y);
            if (pic32.Image !=null) e.Graphics.DrawImage(pic32.Image, 10 + x + 10, od + 2*y + 20, x, y);
            if (pic33.Image !=null) e.Graphics.DrawImage(pic33.Image, 10 + 2 * x + 20, od + 2*y + 20, x, y);

           // e.Graphics.DrawImage(bm, 10, 60, 830, 600);
            e.Graphics.DrawString(textBox1.Text, new Font("Tahoma", int.Parse(textBoxRozmiarCzcionki.Text)), Brushes.Black, 0, od-50);
            e.Graphics.DrawString(textBox2.Text, new Font("Tahoma", int.Parse(textBoxRozmiarCzcionki.Text)), Brushes.Black, 0, od+3*y+40);
            bm.Dispose();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
           
            path = Environment.CurrentDirectory + "/" + "druk1.txt";
            try
            {
                using (StreamReader sr = new StreamReader(path))
                {
                    textBox1.Text = sr.ReadLine();
                    textBox2.Text = sr.ReadLine();
                    textBox_odstep.Text = sr.ReadLine();
                    textBoxWysObrazka.Text = sr.ReadLine();
                    textBoxSzerObrazka.Text = sr.ReadLine();
                }
            }
            catch
            {
                textBox1.Text = "Centrum Medyczne EVITA";
                textBox2.Text = "Wydruk z badań:";
                textBox_odstep.Text = "50";
                textBoxWysObrazka.Text = "200";
                textBoxSzerObrazka.Text = "250";
            }            
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            path = Environment.CurrentDirectory + "/" + "druk2.txt";
            try
            {
                using (StreamReader sr = new StreamReader(path))
                {
                    textBox1.Text = sr.ReadLine();
                    textBox2.Text = sr.ReadLine();
                    textBox_odstep.Text = sr.ReadLine();
                    textBoxWysObrazka.Text = sr.ReadLine();
                    textBoxSzerObrazka.Text = sr.ReadLine();
                }
            }
            catch
            {
                textBox1.Text = "Gastroskopia";
                textBox2.Text = "Wyniki:";
                textBox_odstep.Text = "200";
                textBoxWysObrazka.Text = "200";
                textBoxSzerObrazka.Text = "250";
            }
            
        }

        private void pic11_Click(object sender, EventArgs e)
        {
            ;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!File.Exists(path))
                File.CreateText(path);

            try
            {
                using (StreamWriter sw = new StreamWriter(path))
                {
                    sw.WriteLine(textBox1.Text);
                    sw.WriteLine(textBox2.Text);
                    sw.WriteLine(textBox_odstep.Text);
                    sw.WriteLine(textBoxWysObrazka.Text);
                    sw.WriteLine(textBoxSzerObrazka.Text);
                    sw.WriteLine(textBoxRozmiarCzcionki.Text);
                }
            }
            catch { };

        }
    }
}
