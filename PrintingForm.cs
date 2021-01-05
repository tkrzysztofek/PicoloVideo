using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Drawing.Imaging;
using System.Linq;

namespace PicoloVideo
{
    public partial class PrintingWindowForm : Form
    {
        string pathPrintingSettings = Environment.CurrentDirectory + "/" + "druk1.txt";
        string pathPrinterSettings = Environment.CurrentDirectory + "/" + "printerSettings.txt";

        int odstepMiedzyObrazkami;

        public PrintingWindowForm()
        {
            InitializeComponent();

            try
            {
                loadPrintingSettingsFromFile(pathPrintingSettings);
            }

            catch
            {
                textBoxNaglowek.Text = "Centrum Medyczne EVITA";
                textBoxPodpis.Text = "Wydruk z badań:";
                textBoxodstepMiedzyObrazkami.Text = "50";
                textBoxWysObrazka.Text = "200";
                textBoxSzerObrazka.Text = "250";
                textBoxRozmiarCzcionki.Text = "16";
                textBoxSciezkaPDF.Text = "c://zdjecia/";
                textBoxNazwaPDF.Text = "dokument";
            }
        }

        public void loadPrintingSettingsFromFile(String path)
        {
            using (StreamReader sr = new StreamReader(path))
            {
                textBoxNaglowek.Text = sr.ReadLine();
                textBoxPodpis.Text = sr.ReadLine();
                textBoxodstepMiedzyObrazkami.Text = sr.ReadLine();
                textBoxWysObrazka.Text = sr.ReadLine();
                textBoxSzerObrazka.Text = sr.ReadLine();
                textBoxRozmiarCzcionki.Text = sr.ReadLine();
                textBoxSciezkaPDF.Text = sr.ReadLine();
                textBoxNazwaPDF.Text = sr.ReadLine();
            }
        }

        private void buttonDrukuj_Click(object sender, EventArgs e)
        {
            try
            {
                PrintDialog printDialog = new PrintDialog();
                PrintDocument printDoc = new PrintDocument();

                loadPrinterSettingsFromFile(printDoc);

                printDoc.PrintPage += Doc_PrintPage;

                printDialog.Document = printDoc;
                if (printDialog.ShowDialog() == DialogResult.OK)
                {
                    printDoc.Print();
                }
            }
            catch
            {
            }
        }

        public void loadPrinterSettingsFromFile(PrintDocument document)
        {
            PageSettings pageSettings = new PageSettings();
            string zasobnik;
            try
            {
                using (StreamReader sr = new StreamReader(pathPrinterSettings))
                {
                    zasobnik = sr.ReadLine();
                }
            }
            catch
            {
                return;
            }
            string nrZasobnika = new String(zasobnik.Where(Char.IsDigit).ToArray());
            document.DefaultPageSettings.PaperSource = pageSettings.PrinterSettings.PaperSources[Int32.Parse(nrZasobnika)];
        }

        private void Doc_PrintPage(object sender, PrintPageEventArgs e)
        {
            //   Bitmap bm = new Bitmap(pic11.Width * 3, pic11.Height * 3);

            int x = int.Parse(textBoxSzerObrazka.Text); //pic11.Width + 5*(pic11.Width / 7) ;// 110;
            int y = int.Parse(textBoxWysObrazka.Text); //pic11.Height+ 5*(pic11.Height / 7) ;// 110;

            odstepMiedzyObrazkami = int.Parse(textBoxodstepMiedzyObrazkami.Text);
            if (odstepMiedzyObrazkami < 50) odstepMiedzyObrazkami = 50;

            if (pic11.Image != null) e.Graphics.DrawImage(pic11.Image, 10, odstepMiedzyObrazkami, x, y);
            if (pic12.Image != null) e.Graphics.DrawImage(pic12.Image, 10 + x + 10, odstepMiedzyObrazkami, x, y);
            if (pic13.Image != null) e.Graphics.DrawImage(pic13.Image, 10 + 2 * x + 20, odstepMiedzyObrazkami, x, y);

            if (pic21.Image != null) e.Graphics.DrawImage(pic21.Image, 10, odstepMiedzyObrazkami + y + 10, x, y);
            if (pic22.Image != null)
                e.Graphics.DrawImage(pic22.Image, 10 + x + 10, odstepMiedzyObrazkami + y + 10, x, y);
            if (pic23.Image != null)
                e.Graphics.DrawImage(pic23.Image, 10 + 2 * x + 20, odstepMiedzyObrazkami + y + 10, x, y);

            if (pic31.Image != null) e.Graphics.DrawImage(pic31.Image, 10, odstepMiedzyObrazkami + 2 * y + 20, x, y);
            if (pic32.Image != null)
                e.Graphics.DrawImage(pic32.Image, 10 + x + 10, odstepMiedzyObrazkami + 2 * y + 20, x, y);
            if (pic33.Image != null)
                e.Graphics.DrawImage(pic33.Image, 10 + 2 * x + 20, odstepMiedzyObrazkami + 2 * y + 20, x, y);

            e.Graphics.DrawString(textBoxNaglowek.Text,
                new System.Drawing.Font("Tahoma", int.Parse(textBoxRozmiarCzcionki.Text)), Brushes.Black, 0,
                odstepMiedzyObrazkami - 50);
            e.Graphics.DrawString(textBoxPodpis.Text,
                new System.Drawing.Font("Tahoma", int.Parse(textBoxRozmiarCzcionki.Text)), Brushes.Black, 0,
                odstepMiedzyObrazkami + 3 * y + 40);
        }

        private void radioButtonTekst1_CheckedChanged(object sender, EventArgs e)
        {
            pathPrintingSettings = Environment.CurrentDirectory + "/" + "druk1.txt";
            try
            {
                loadPrintingSettingsFromFile(pathPrintingSettings);
            }
            catch
            {
                textBoxNaglowek.Text = "Centrum Medyczne EVITA";
                textBoxPodpis.Text = "Wydruk z badań:";
                textBoxodstepMiedzyObrazkami.Text = "50";
                textBoxWysObrazka.Text = "200";
                textBoxSzerObrazka.Text = "250";
                textBoxRozmiarCzcionki.Text = "16";
                textBoxSciezkaPDF.Text = "c://zdjecia/";
                textBoxNazwaPDF.Text = "dokument";
            }
        }

        private void radioButtonTekst2_CheckedChanged(object sender, EventArgs e)
        {
            pathPrintingSettings = Environment.CurrentDirectory + "/" + "druk2.txt";
            try
            {
                loadPrintingSettingsFromFile(pathPrintingSettings);
            }
            catch
            {
                textBoxNaglowek.Text = "Gastroskopia";
                textBoxPodpis.Text = "Wyniki:";
                textBoxodstepMiedzyObrazkami.Text = "200";
                textBoxWysObrazka.Text = "200";
                textBoxSzerObrazka.Text = "250";
                textBoxRozmiarCzcionki.Text = "16";
                textBoxSciezkaPDF.Text = "c://zdjecia/";
                textBoxNazwaPDF.Text = "dokument";
            }
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            ;
        }

        private void buttonZapisUstawien_Click(object sender, EventArgs e)
        {
            if (!File.Exists(pathPrintingSettings))
                File.CreateText(pathPrintingSettings);

            try
            {
                using (StreamWriter sw = new StreamWriter(pathPrintingSettings))
                {
                    sw.WriteLine(textBoxNaglowek.Text);
                    sw.WriteLine(textBoxPodpis.Text);
                    sw.WriteLine(textBoxodstepMiedzyObrazkami.Text);
                    sw.WriteLine(textBoxWysObrazka.Text);
                    sw.WriteLine(textBoxSzerObrazka.Text);
                    sw.WriteLine(textBoxRozmiarCzcionki.Text);
                    sw.WriteLine(textBoxSciezkaPDF.Text);
                    sw.WriteLine(textBoxNazwaPDF.Text);
                }
            }
            catch
            {
                MessageBox.Show("Błąd zapisu pliku configuracyjnego");
            }
        }


        private void buttonTestZapisu_Click(object sender, EventArgs e)
        {
            Bitmap scalonyObraz = new Bitmap(pic11.Width * 5 + 40, pic11.Height * 9);
            Graphics g = Graphics.FromImage(scalonyObraz);
            g.Clear(Color.White);

            int x = int.Parse(textBoxSzerObrazka.Text); //pic11.Width + 5*(pic11.Width / 7) ;// 110;
            int y = int.Parse(textBoxWysObrazka.Text); //pic11.Height+ 5*(pic11.Height / 7) ;// 110;

            odstepMiedzyObrazkami = int.Parse(textBoxodstepMiedzyObrazkami.Text);
            if (odstepMiedzyObrazkami < 50) odstepMiedzyObrazkami = 50;

            if (pic11.Image != null) g.DrawImage(pic11.Image, 0, odstepMiedzyObrazkami, x, y);
            if (pic12.Image != null) g.DrawImage(pic12.Image, 0 + x + 10, odstepMiedzyObrazkami, x, y);
            if (pic13.Image != null) g.DrawImage(pic13.Image, 0 + 2 * x + 20, odstepMiedzyObrazkami, x, y);

            if (pic21.Image != null) g.DrawImage(pic21.Image, 0, odstepMiedzyObrazkami + y + 10, x, y);
            if (pic22.Image != null) g.DrawImage(pic22.Image, 0 + x + 10, odstepMiedzyObrazkami + y + 10, x, y);
            if (pic23.Image != null) g.DrawImage(pic23.Image, 0 + 2 * x + 20, odstepMiedzyObrazkami + y + 10, x, y);

            if (pic31.Image != null) g.DrawImage(pic31.Image, 0, odstepMiedzyObrazkami + 2 * y + 20, x, y);
            if (pic32.Image != null) g.DrawImage(pic32.Image, 0 + x + 10, odstepMiedzyObrazkami + 2 * y + 20, x, y);
            if (pic33.Image != null) g.DrawImage(pic33.Image, 0 + 2 * x + 20, odstepMiedzyObrazkami + 2 * y + 20, x, y);

            g.DrawString(textBoxNaglowek.Text,
                new System.Drawing.Font("Tahoma", int.Parse(textBoxRozmiarCzcionki.Text)), Brushes.Black, 0,
                odstepMiedzyObrazkami - 50);
            g.DrawString(textBoxPodpis.Text, new System.Drawing.Font("Tahoma", int.Parse(textBoxRozmiarCzcionki.Text)),
                Brushes.Black, 0, odstepMiedzyObrazkami + 3 * y + 40);


            Document doc = new Document(PageSize.A3);

            if (!Directory.Exists(textBoxSciezkaPDF.Text))
                Directory.CreateDirectory(textBoxSciezkaPDF.Text);
            try
            {
                String pathPdf = textBoxSciezkaPDF.Text + textBoxNazwaPDF.Text + ".pdf";
                PdfWriter.GetInstance(doc, new FileStream(pathPdf, FileMode.Create));
                doc.Open();
                iTextSharp.text.Image pdfImage = iTextSharp.text.Image.GetInstance(scalonyObraz,
                    System.Drawing.Imaging.ImageFormat.Bmp);
                doc.Add(pdfImage);
            }
            catch
            {
                MessageBox.Show("Blad zapisu PDF \nSprawdź czy plik " + textBoxNazwaPDF.Text +
                                ".pdf nie jest otwarty w innym programie");
            }

            doc.Close();
            g.Dispose();
        }
    }
}