using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;
using System.IO;




namespace PicoloVideo
{
    public partial class Form1 : Form
    {               

        public Form1()
        {
            InitializeComponent();
            textBox1.Text = "Centrum Medyczne EVITA";
            textBox2.Text = "Wydruk z badań:";

        }

        private void BtnPrint_Click(object sender, EventArgs e)
        {
            PrintDialog pd = new PrintDialog();
            PrintDocument doc = new PrintDocument();
            doc.PrintPage += Doc_PrintPage;

            pd.Document = doc;
            if (pd.ShowDialog() == DialogResult.OK)
                doc.Print();
        }

        private void Doc_PrintPage(object sender, PrintPageEventArgs e)
        {
            //throw new NotImplementedException();
            Bitmap bm = new Bitmap(pic11.Width*3, pic11.Height*3);
            
            pic11.DrawToBitmap(bm, new Rectangle(0, 0, pic11.Width, pic11.Height));
            pic12.DrawToBitmap(bm, new Rectangle(pic11.Width, 0, pic11.Width, pic11.Height));
            pic13.DrawToBitmap(bm, new Rectangle(pic11.Width*2, 0, pic11.Width, pic11.Height));
            pic21.DrawToBitmap(bm, new Rectangle(0, pic11.Height, pic11.Width, pic11.Height));
            pic22.DrawToBitmap(bm, new Rectangle(pic11.Width, pic11.Height, pic11.Width, pic11.Height));
            pic23.DrawToBitmap(bm, new Rectangle(pic11.Width * 2, pic11.Height, pic11.Width, pic11.Height));
            pic31.DrawToBitmap(bm, new Rectangle(0, pic11.Height*2, pic11.Width, pic11.Height));
            pic32.DrawToBitmap(bm, new Rectangle(pic11.Width, pic11.Height*2, pic11.Width, pic11.Height));
            pic33.DrawToBitmap(bm, new Rectangle(pic11.Width * 2, pic11.Height*2, pic11.Width, pic11.Height));          
           
            e.Graphics.DrawImage(bm, 10, 60, 830, 600);
            e.Graphics.DrawString(textBox1.Text, new Font("Tahoma", 24), Brushes.Black, 0, 0);
            e.Graphics.DrawString(textBox2.Text, new Font("Tahoma", 24), Brushes.Black, 0, pic11.Height * 6);            
            bm.Dispose();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            textBox1.Text = "Centrum Medyczne EVITA";
            textBox2.Text = "Wydruk z badań:";
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            textBox1.Text = "Gastroskopia";
            textBox2.Text = "Wyniki:";
        }

        
        
    }
}
