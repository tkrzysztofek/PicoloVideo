/*
+-------------------------------- DISCLAIMER ---------------------------------+
|                                                                             |
| This application program is provided to you free of charge as an example.   |
| Despite the considerable efforts of Euresys personnel to create a usable    |
| example, you should not assume that this program is error-free or suitable  |
| for any purpose whatsoever.                                                 |
|                                                                             |
| EURESYS does not give any representation, warranty or undertaking that this |
| program is free of any defect or error or suitable for any purpose. EURESYS |
| shall not be liable, in contract, in torts or otherwise, for any damages,   |
| loss, costs, expenses or other claims for compensation, including those     |
| asserted by third parties, arising out of or in connection with the use of  |
| this program.                                                               |
|                                                                             |
+-----------------------------------------------------------------------------+
*/

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Threading;
using Euresys.MultiCam;
using System.Timers;
using System.IO;
using System.Collections.Generic;
using AForge.Imaging.Filters;
using AForge.Video.FFMPEG;
using System.Diagnostics;
using System.Text;
using AviFile;


//using iTextSharp.text;
//using iTextSharp.text.pdf;


namespace PicoloVideo
{
    /// <summary>
    /// Summary description for PrintingWindowForm.
    /// </summary>
    /// 

    public class MainForm : System.Windows.Forms.Form
    {
        Int32 globalWidthOfImage, globalHeightOfImage;
        Bitmap NewBitmap = null;
        Bitmap Img_zapis = null;
        VideoFileWriter writer = new VideoFileWriter();
        
        bool triggerHardwerowyOn = false;
        bool triggerFirstClick = false;
        //bool nagrywanieON = false;

        List<Image> selectedImages = new List<Image>();
        List<Bitmap> listaZdjeciaVideo = new List<Bitmap>();

        Bitmap tempAcquiredBitmap;

        string configFilePath = Environment.CurrentDirectory + "/" + "config.txt";

        int BrightnessValue = 0;
        int contrastValue = 0;
        float saturationValue = 0;

        PictureBox[] imgBoxesList = new PictureBox[23];
        TextBox[] seletedImgMarkerList = new TextBox[23];
        Image[] imagesToPrint = new Image[9];

        int actualImgNumber = 0;
        //int selectedImgNumber = 0;
        bool ekranGlownyOn = false;
        //private static Timer aTimer;

        // Creation of an event for asynchronous call to paint function
        public delegate void PaintDelegate(Graphics g);
        public delegate void UpdateStatusBarDelegate(String text);

        // The object that will contain the acquired image
        private Bitmap acquiredBitmap = null;

        // The Mutex object that will protect image objects during processing
        private static Mutex imageMutex = new Mutex();

        // The MultiCam object that controls the acquisition
        UInt32 channel;

        // The MultiCam object that contains the acquired buffer
        private UInt32 currentSurface;

        // Track if acquisition is ongoing
        private volatile bool channelActive;

        MC.CALLBACK multiCamCallback;

        private System.Windows.Forms.MainMenu mainMenu;
        private PictureBox mainScreen;
        private Button buttonVideoStart;
        private Button buttonVideoStop;
        private PictureBox picture1;
        private PictureBox picture2;
        private PictureBox picture3;
        private PictureBox picture4;
        private PictureBox picture5;
        private PictureBox picture11;
        private PictureBox picture10;
        private PictureBox picture9;
        private PictureBox picture8;
        private PictureBox picture7;
        private PictureBox picture6;
        private PictureBox picture13;
        private PictureBox picture12;
        private PictureBox picture15;
        private PictureBox picture14;
        private PictureBox picture17;
        private PictureBox picture16;
        private Button buttonZlapKlatke;
        private Button buttonZlapKlatki;
        private Button buttonKasujKlatki;
        private System.Windows.Forms.Timer timerZlapKlatki;

        private SaveFileDialog saveFileDialog1;
        private TrackBar TrackbarJasnosc;
        private Label label1;
        private Label label2;
        private Button buttonZapiszJpg;
        private TextBox textBoxNazwaPlikuJpgBmp;
        private TextBox textBoxSciezkaZapisu;
        private Label label3;
        private Label label4;
        private Button buttonDrukowanie;
        private TextBox tbox1;
        private TextBox tbox2;
        private TextBox tbox3;
        private TextBox tbox4;
        private TextBox tbox5;
        private TextBox tbox6;
        private TextBox tbox12;
        private TextBox tbox7;
        private TextBox tbox13;
        private TextBox tbox8;
        private TextBox tbox14;
        private TextBox tbox9;
        private TextBox tbox15;
        private TextBox tbox10;
        private TextBox tbox16;
        private TextBox tbox11;
        private TextBox tbox17;
        private TextBox selectedImgNumbersTextBox;
        private Button buttonClipboard;
        private Label label5;
        private TrackBar TrackbarKontrast;
        private Button buttonZapiszUstawienia;
        private Label label6;
        private TrackBar TrackbarNasycenie;
        private Button ZapiszBmpBtn;
        private Label label7;
        private Button buttonStartKlatki;
        private Button buttonZlapKlatkiStop;
        private TextBox textBoxNasycenieNumber;
        private TextBox textBoxKontrastNumber;
        private TextBox textBoxJasnoscNumber;
        private ErrorProvider errorProvider1;
        private Button buttonAnuluj;
        private RadioButton radioButtonV3;
        private RadioButton radioButtonV2;
        private RadioButton radioButtonV1;
        private Button buttonNagrywaj;
        private System.Windows.Forms.Timer timerNagrywanie;
        private Button buttonStopNagrywanie;
        private Label label8;
        private TextBox textBoxNazwaPlikuAvi;
        private TextBox tbox23;
        private TextBox tbox22;
        private TextBox tbox21;
        private TextBox tbox20;
        private TextBox tbox19;
        private TextBox tbox18;
        private PictureBox picture19;
        private PictureBox picture18;
        private PictureBox picture21;
        private PictureBox picture20;
        private PictureBox picture23;
        private PictureBox picture22;
        private System.ComponentModel.IContainer components;

        public MainForm()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();
            loadConfigurationFromFile();
            prepereImageBoxes();
        }

        private void loadConfigurationFromFile()
        {
            try
            {
                using (StreamReader sr = new StreamReader(configFilePath))
                {
                    TrackbarJasnosc.Value = int.Parse(sr.ReadLine());
                    TrackbarKontrast.Value = int.Parse(sr.ReadLine());
                    TrackbarNasycenie.Value = int.Parse(sr.ReadLine());
                    textBoxSciezkaZapisu.Text = sr.ReadLine();
                    textBoxNazwaPlikuJpgBmp.Text = sr.ReadLine();
                    textBoxNazwaPlikuAvi.Text = sr.ReadLine();
                }
            }
            catch
            {
                MessageBox.Show("Nie wykryto pliku config.txt");
                TrackbarJasnosc.Value = 0;
                TrackbarKontrast.Value = 0;
                TrackbarNasycenie.Value = 0;
                textBoxSciezkaZapisu.Text = "c://zdjecia/";
                textBoxNazwaPlikuJpgBmp.Text = "zdjecie1";
                textBoxNazwaPlikuAvi.Text = "video";
            }

            saturationValue = (float)TrackbarNasycenie.Value / 100;
            textBoxNasycenieNumber.Text = TrackbarNasycenie.Value.ToString();

            contrastValue = TrackbarKontrast.Value;
            textBoxKontrastNumber.Text = contrastValue.ToString();

            BrightnessValue = TrackbarJasnosc.Value;
            textBoxJasnoscNumber.Text = BrightnessValue.ToString();
        }

        private void prepereImageBoxes()
        {
            imgBoxesList[0] = picture1;
            imgBoxesList[1] = picture2;
            imgBoxesList[2] = picture3;
            imgBoxesList[3] = picture4;
            imgBoxesList[4] = picture5;
            imgBoxesList[5] = picture6;
            imgBoxesList[6] = picture7;
            imgBoxesList[7] = picture8;
            imgBoxesList[8] = picture9;
            imgBoxesList[9] = picture10;
            imgBoxesList[10] = picture11;
            imgBoxesList[11] = picture12;
            imgBoxesList[12] = picture13;
            imgBoxesList[13] = picture14;
            imgBoxesList[14] = picture15;
            imgBoxesList[15] = picture16;
            imgBoxesList[16] = picture17;
            imgBoxesList[17] = picture18;
            imgBoxesList[18] = picture19;
            imgBoxesList[19] = picture20;
            imgBoxesList[20] = picture21;
            imgBoxesList[21] = picture22;
            imgBoxesList[22] = picture23;

            seletedImgMarkerList[0] = tbox1;
            seletedImgMarkerList[1] = tbox2;
            seletedImgMarkerList[2] = tbox3;
            seletedImgMarkerList[3] = tbox4;
            seletedImgMarkerList[4] = tbox5;
            seletedImgMarkerList[5] = tbox6;
            seletedImgMarkerList[6] = tbox7;
            seletedImgMarkerList[7] = tbox8;
            seletedImgMarkerList[8] = tbox9;
            seletedImgMarkerList[9] = tbox10;
            seletedImgMarkerList[10] = tbox11;
            seletedImgMarkerList[11] = tbox12;
            seletedImgMarkerList[12] = tbox13;
            seletedImgMarkerList[13] = tbox14;
            seletedImgMarkerList[14] = tbox15;
            seletedImgMarkerList[15] = tbox16;
            seletedImgMarkerList[16] = tbox17;
            seletedImgMarkerList[17] = tbox18;
            seletedImgMarkerList[18] = tbox19;
            seletedImgMarkerList[19] = tbox20;
            seletedImgMarkerList[20] = tbox21;
            seletedImgMarkerList[21] = tbox22;
            seletedImgMarkerList[22] = tbox23;

            for (int i = 0; i < seletedImgMarkerList.Length; i++)
            {
                seletedImgMarkerList[i].Visible = false;
            }
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.mainMenu = new System.Windows.Forms.MainMenu(this.components);
            this.mainScreen = new System.Windows.Forms.PictureBox();
            this.buttonVideoStart = new System.Windows.Forms.Button();
            this.buttonVideoStop = new System.Windows.Forms.Button();
            this.picture1 = new System.Windows.Forms.PictureBox();
            this.picture2 = new System.Windows.Forms.PictureBox();
            this.picture3 = new System.Windows.Forms.PictureBox();
            this.picture4 = new System.Windows.Forms.PictureBox();
            this.picture5 = new System.Windows.Forms.PictureBox();
            this.picture11 = new System.Windows.Forms.PictureBox();
            this.picture10 = new System.Windows.Forms.PictureBox();
            this.picture9 = new System.Windows.Forms.PictureBox();
            this.picture8 = new System.Windows.Forms.PictureBox();
            this.picture7 = new System.Windows.Forms.PictureBox();
            this.picture6 = new System.Windows.Forms.PictureBox();
            this.picture13 = new System.Windows.Forms.PictureBox();
            this.picture12 = new System.Windows.Forms.PictureBox();
            this.picture15 = new System.Windows.Forms.PictureBox();
            this.picture14 = new System.Windows.Forms.PictureBox();
            this.picture17 = new System.Windows.Forms.PictureBox();
            this.picture16 = new System.Windows.Forms.PictureBox();
            this.buttonZlapKlatke = new System.Windows.Forms.Button();
            this.buttonZlapKlatki = new System.Windows.Forms.Button();
            this.buttonKasujKlatki = new System.Windows.Forms.Button();
            this.timerZlapKlatki = new System.Windows.Forms.Timer(this.components);
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.TrackbarJasnosc = new System.Windows.Forms.TrackBar();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonZapiszJpg = new System.Windows.Forms.Button();
            this.textBoxNazwaPlikuJpgBmp = new System.Windows.Forms.TextBox();
            this.textBoxSciezkaZapisu = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.buttonDrukowanie = new System.Windows.Forms.Button();
            this.tbox1 = new System.Windows.Forms.TextBox();
            this.tbox2 = new System.Windows.Forms.TextBox();
            this.tbox3 = new System.Windows.Forms.TextBox();
            this.tbox4 = new System.Windows.Forms.TextBox();
            this.tbox5 = new System.Windows.Forms.TextBox();
            this.tbox6 = new System.Windows.Forms.TextBox();
            this.tbox12 = new System.Windows.Forms.TextBox();
            this.tbox7 = new System.Windows.Forms.TextBox();
            this.tbox13 = new System.Windows.Forms.TextBox();
            this.tbox8 = new System.Windows.Forms.TextBox();
            this.tbox14 = new System.Windows.Forms.TextBox();
            this.tbox9 = new System.Windows.Forms.TextBox();
            this.tbox15 = new System.Windows.Forms.TextBox();
            this.tbox10 = new System.Windows.Forms.TextBox();
            this.tbox16 = new System.Windows.Forms.TextBox();
            this.tbox11 = new System.Windows.Forms.TextBox();
            this.tbox17 = new System.Windows.Forms.TextBox();
            this.selectedImgNumbersTextBox = new System.Windows.Forms.TextBox();
            this.buttonClipboard = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.TrackbarKontrast = new System.Windows.Forms.TrackBar();
            this.buttonZapiszUstawienia = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.TrackbarNasycenie = new System.Windows.Forms.TrackBar();
            this.ZapiszBmpBtn = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.buttonStartKlatki = new System.Windows.Forms.Button();
            this.buttonZlapKlatkiStop = new System.Windows.Forms.Button();
            this.textBoxNasycenieNumber = new System.Windows.Forms.TextBox();
            this.textBoxKontrastNumber = new System.Windows.Forms.TextBox();
            this.textBoxJasnoscNumber = new System.Windows.Forms.TextBox();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.buttonAnuluj = new System.Windows.Forms.Button();
            this.radioButtonV1 = new System.Windows.Forms.RadioButton();
            this.radioButtonV2 = new System.Windows.Forms.RadioButton();
            this.radioButtonV3 = new System.Windows.Forms.RadioButton();
            this.buttonNagrywaj = new System.Windows.Forms.Button();
            this.timerNagrywanie = new System.Windows.Forms.Timer(this.components);
            this.buttonStopNagrywanie = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.textBoxNazwaPlikuAvi = new System.Windows.Forms.TextBox();
            this.tbox23 = new System.Windows.Forms.TextBox();
            this.tbox22 = new System.Windows.Forms.TextBox();
            this.tbox21 = new System.Windows.Forms.TextBox();
            this.tbox20 = new System.Windows.Forms.TextBox();
            this.tbox19 = new System.Windows.Forms.TextBox();
            this.tbox18 = new System.Windows.Forms.TextBox();
            this.picture19 = new System.Windows.Forms.PictureBox();
            this.picture18 = new System.Windows.Forms.PictureBox();
            this.picture21 = new System.Windows.Forms.PictureBox();
            this.picture20 = new System.Windows.Forms.PictureBox();
            this.picture23 = new System.Windows.Forms.PictureBox();
            this.picture22 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.mainScreen)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picture1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picture2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picture3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picture4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picture5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picture11)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picture10)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picture9)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picture8)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picture7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picture6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picture13)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picture12)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picture15)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picture14)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picture17)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picture16)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.TrackbarJasnosc)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.TrackbarKontrast)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.TrackbarNasycenie)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picture19)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picture18)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picture21)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picture20)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picture23)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picture22)).BeginInit();
            this.SuspendLayout();
            // 
            // mainScreen
            // 
            this.mainScreen.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.mainScreen.Location = new System.Drawing.Point(155, 1);
            this.mainScreen.Margin = new System.Windows.Forms.Padding(0);
            this.mainScreen.Name = "mainScreen";
            this.mainScreen.Size = new System.Drawing.Size(765, 579);
            this.mainScreen.TabIndex = 1;
            this.mainScreen.TabStop = false;
            // 
            // buttonVideoStart
            // 
            this.buttonVideoStart.Location = new System.Drawing.Point(971, 153);
            this.buttonVideoStart.Name = "buttonVideoStart";
            this.buttonVideoStart.Size = new System.Drawing.Size(75, 23);
            this.buttonVideoStart.TabIndex = 2;
            this.buttonVideoStart.Text = "Video start";
            this.buttonVideoStart.UseVisualStyleBackColor = true;
            this.buttonVideoStart.Click += new System.EventHandler(this.buttonVideoStart_Click);
            // 
            // buttonVideoStop
            // 
            this.buttonVideoStop.Location = new System.Drawing.Point(1049, 153);
            this.buttonVideoStop.Name = "buttonVideoStop";
            this.buttonVideoStop.Size = new System.Drawing.Size(75, 23);
            this.buttonVideoStop.TabIndex = 3;
            this.buttonVideoStop.Text = "Video stop";
            this.buttonVideoStop.UseVisualStyleBackColor = true;
            this.buttonVideoStop.Click += new System.EventHandler(this.buttonVideoStop_Click);
            // 
            // picture1
            // 
            this.picture1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picture1.Location = new System.Drawing.Point(2, 1);
            this.picture1.Margin = new System.Windows.Forms.Padding(0);
            this.picture1.Name = "picture1";
            this.picture1.Size = new System.Drawing.Size(153, 115);
            this.picture1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picture1.TabIndex = 4;
            this.picture1.TabStop = false;
            this.picture1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.image_Click);
            // 
            // picture2
            // 
            this.picture2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picture2.Location = new System.Drawing.Point(2, 117);
            this.picture2.Margin = new System.Windows.Forms.Padding(0);
            this.picture2.Name = "picture2";
            this.picture2.Size = new System.Drawing.Size(153, 115);
            this.picture2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picture2.TabIndex = 6;
            this.picture2.TabStop = false;
            this.picture2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.image_Click);
            // 
            // picture3
            // 
            this.picture3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picture3.Location = new System.Drawing.Point(2, 233);
            this.picture3.Margin = new System.Windows.Forms.Padding(0);
            this.picture3.Name = "picture3";
            this.picture3.Size = new System.Drawing.Size(153, 115);
            this.picture3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picture3.TabIndex = 8;
            this.picture3.TabStop = false;
            this.picture3.MouseDown += new System.Windows.Forms.MouseEventHandler(this.image_Click);
            // 
            // picture4
            // 
            this.picture4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picture4.Location = new System.Drawing.Point(2, 349);
            this.picture4.Margin = new System.Windows.Forms.Padding(0);
            this.picture4.Name = "picture4";
            this.picture4.Size = new System.Drawing.Size(153, 115);
            this.picture4.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picture4.TabIndex = 10;
            this.picture4.TabStop = false;
            this.picture4.MouseDown += new System.Windows.Forms.MouseEventHandler(this.image_Click);
            // 
            // picture5
            // 
            this.picture5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picture5.Location = new System.Drawing.Point(2, 465);
            this.picture5.Margin = new System.Windows.Forms.Padding(0);
            this.picture5.Name = "picture5";
            this.picture5.Size = new System.Drawing.Size(153, 115);
            this.picture5.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picture5.TabIndex = 12;
            this.picture5.TabStop = false;
            this.picture5.MouseDown += new System.Windows.Forms.MouseEventHandler(this.image_Click);
            // 
            // picture11
            // 
            this.picture11.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picture11.Location = new System.Drawing.Point(767, 581);
            this.picture11.Margin = new System.Windows.Forms.Padding(0);
            this.picture11.Name = "picture11";
            this.picture11.Size = new System.Drawing.Size(153, 115);
            this.picture11.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picture11.TabIndex = 15;
            this.picture11.TabStop = false;
            this.picture11.MouseDown += new System.Windows.Forms.MouseEventHandler(this.image_Click);
            // 
            // picture10
            // 
            this.picture10.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picture10.Location = new System.Drawing.Point(614, 581);
            this.picture10.Margin = new System.Windows.Forms.Padding(0);
            this.picture10.Name = "picture10";
            this.picture10.Size = new System.Drawing.Size(153, 115);
            this.picture10.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picture10.TabIndex = 14;
            this.picture10.TabStop = false;
            this.picture10.MouseDown += new System.Windows.Forms.MouseEventHandler(this.image_Click);
            // 
            // picture9
            // 
            this.picture9.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picture9.Location = new System.Drawing.Point(461, 581);
            this.picture9.Margin = new System.Windows.Forms.Padding(0);
            this.picture9.Name = "picture9";
            this.picture9.Size = new System.Drawing.Size(153, 115);
            this.picture9.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picture9.TabIndex = 17;
            this.picture9.TabStop = false;
            this.picture9.MouseDown += new System.Windows.Forms.MouseEventHandler(this.image_Click);
            // 
            // picture8
            // 
            this.picture8.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picture8.Location = new System.Drawing.Point(308, 581);
            this.picture8.Margin = new System.Windows.Forms.Padding(0);
            this.picture8.Name = "picture8";
            this.picture8.Size = new System.Drawing.Size(153, 115);
            this.picture8.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picture8.TabIndex = 16;
            this.picture8.TabStop = false;
            this.picture8.MouseDown += new System.Windows.Forms.MouseEventHandler(this.image_Click);
            // 
            // picture7
            // 
            this.picture7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picture7.Location = new System.Drawing.Point(155, 581);
            this.picture7.Margin = new System.Windows.Forms.Padding(0);
            this.picture7.Name = "picture7";
            this.picture7.Size = new System.Drawing.Size(153, 115);
            this.picture7.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picture7.TabIndex = 19;
            this.picture7.TabStop = false;
            this.picture7.MouseDown += new System.Windows.Forms.MouseEventHandler(this.image_Click);
            // 
            // picture6
            // 
            this.picture6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picture6.Location = new System.Drawing.Point(2, 581);
            this.picture6.Margin = new System.Windows.Forms.Padding(0);
            this.picture6.Name = "picture6";
            this.picture6.Size = new System.Drawing.Size(153, 115);
            this.picture6.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picture6.TabIndex = 18;
            this.picture6.TabStop = false;
            this.picture6.MouseDown += new System.Windows.Forms.MouseEventHandler(this.image_Click);
            // 
            // picture13
            // 
            this.picture13.BackColor = System.Drawing.SystemColors.Control;
            this.picture13.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picture13.Location = new System.Drawing.Point(155, 697);
            this.picture13.Margin = new System.Windows.Forms.Padding(0);
            this.picture13.Name = "picture13";
            this.picture13.Size = new System.Drawing.Size(153, 115);
            this.picture13.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picture13.TabIndex = 25;
            this.picture13.TabStop = false;
            this.picture13.MouseDown += new System.Windows.Forms.MouseEventHandler(this.image_Click);
            // 
            // picture12
            // 
            this.picture12.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picture12.Location = new System.Drawing.Point(2, 697);
            this.picture12.Margin = new System.Windows.Forms.Padding(0);
            this.picture12.Name = "picture12";
            this.picture12.Size = new System.Drawing.Size(153, 115);
            this.picture12.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picture12.TabIndex = 24;
            this.picture12.TabStop = false;
            this.picture12.MouseDown += new System.Windows.Forms.MouseEventHandler(this.image_Click);
            // 
            // picture15
            // 
            this.picture15.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picture15.Location = new System.Drawing.Point(461, 697);
            this.picture15.Margin = new System.Windows.Forms.Padding(0);
            this.picture15.Name = "picture15";
            this.picture15.Size = new System.Drawing.Size(153, 115);
            this.picture15.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picture15.TabIndex = 23;
            this.picture15.TabStop = false;
            this.picture15.MouseDown += new System.Windows.Forms.MouseEventHandler(this.image_Click);
            // 
            // picture14
            // 
            this.picture14.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picture14.Location = new System.Drawing.Point(308, 697);
            this.picture14.Margin = new System.Windows.Forms.Padding(0);
            this.picture14.Name = "picture14";
            this.picture14.Size = new System.Drawing.Size(153, 115);
            this.picture14.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picture14.TabIndex = 22;
            this.picture14.TabStop = false;
            this.picture14.MouseDown += new System.Windows.Forms.MouseEventHandler(this.image_Click);
            // 
            // picture17
            // 
            this.picture17.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picture17.Location = new System.Drawing.Point(767, 697);
            this.picture17.Margin = new System.Windows.Forms.Padding(0);
            this.picture17.Name = "picture17";
            this.picture17.Size = new System.Drawing.Size(153, 115);
            this.picture17.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picture17.TabIndex = 21;
            this.picture17.TabStop = false;
            this.picture17.MouseDown += new System.Windows.Forms.MouseEventHandler(this.image_Click);
            // 
            // picture16
            // 
            this.picture16.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picture16.Location = new System.Drawing.Point(614, 697);
            this.picture16.Margin = new System.Windows.Forms.Padding(0);
            this.picture16.Name = "picture16";
            this.picture16.Size = new System.Drawing.Size(153, 115);
            this.picture16.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picture16.TabIndex = 20;
            this.picture16.TabStop = false;
            this.picture16.MouseDown += new System.Windows.Forms.MouseEventHandler(this.image_Click);
            // 
            // buttonZlapKlatke
            // 
            this.buttonZlapKlatke.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.buttonZlapKlatke.Location = new System.Drawing.Point(971, 8);
            this.buttonZlapKlatke.Name = "buttonZlapKlatke";
            this.buttonZlapKlatke.Size = new System.Drawing.Size(153, 52);
            this.buttonZlapKlatke.TabIndex = 26;
            this.buttonZlapKlatke.Text = "Zlap klatke";
            this.buttonZlapKlatke.UseVisualStyleBackColor = true;
            this.buttonZlapKlatke.Click += new System.EventHandler(this.buttonZlapKlatke_Click);
            // 
            // buttonZlapKlatki
            // 
            this.buttonZlapKlatki.Location = new System.Drawing.Point(971, 66);
            this.buttonZlapKlatki.Name = "buttonZlapKlatki";
            this.buttonZlapKlatki.Size = new System.Drawing.Size(75, 23);
            this.buttonZlapKlatki.TabIndex = 27;
            this.buttonZlapKlatki.Text = "Zlap klatki";
            this.buttonZlapKlatki.UseVisualStyleBackColor = true;
            this.buttonZlapKlatki.Click += new System.EventHandler(this.buttonZlapKlatki_Click);
            // 
            // buttonKasujKlatki
            // 
            this.buttonKasujKlatki.Location = new System.Drawing.Point(1049, 95);
            this.buttonKasujKlatki.Name = "buttonKasujKlatki";
            this.buttonKasujKlatki.Size = new System.Drawing.Size(75, 23);
            this.buttonKasujKlatki.TabIndex = 28;
            this.buttonKasujKlatki.Text = "Kasuj klatki";
            this.buttonKasujKlatki.UseVisualStyleBackColor = true;
            this.buttonKasujKlatki.Click += new System.EventHandler(this.buttonResetZdjec_Click);
            // 
            // timerZlapKlatki
            // 
            this.timerZlapKlatki.Interval = 500;
            this.timerZlapKlatki.Tick += new System.EventHandler(this.TimerZlapKlatki_Tick);
            // 
            // TrackbarJasnosc
            // 
            this.TrackbarJasnosc.LargeChange = 10;
            this.TrackbarJasnosc.Location = new System.Drawing.Point(926, 383);
            this.TrackbarJasnosc.Maximum = 100;
            this.TrackbarJasnosc.Minimum = -100;
            this.TrackbarJasnosc.Name = "TrackbarJasnosc";
            this.TrackbarJasnosc.Size = new System.Drawing.Size(192, 45);
            this.TrackbarJasnosc.SmallChange = 5;
            this.TrackbarJasnosc.TabIndex = 32;
            this.TrackbarJasnosc.TickFrequency = 10;
            this.TrackbarJasnosc.Scroll += new System.EventHandler(this.trackBar_Scroll);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(938, 367);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(46, 13);
            this.label1.TabIndex = 33;
            this.label1.Text = "Jasnoœæ";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label2.Location = new System.Drawing.Point(1034, 930);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(90, 16);
            this.label2.TabIndex = 34;
            this.label2.Text = "www.arco.pl";
            // 
            // buttonZapiszJpg
            // 
            this.buttonZapiszJpg.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.buttonZapiszJpg.Location = new System.Drawing.Point(971, 536);
            this.buttonZapiszJpg.Name = "buttonZapiszJpg";
            this.buttonZapiszJpg.Size = new System.Drawing.Size(153, 56);
            this.buttonZapiszJpg.TabIndex = 35;
            this.buttonZapiszJpg.Text = "Zapisz JPG";
            this.buttonZapiszJpg.UseVisualStyleBackColor = true;
            this.buttonZapiszJpg.Click += new System.EventHandler(this.buttonZapisJPG_Click);
            // 
            // textBoxNazwaPlikuJpgBmp
            // 
            this.textBoxNazwaPlikuJpgBmp.Location = new System.Drawing.Point(1055, 449);
            this.textBoxNazwaPlikuJpgBmp.Name = "textBoxNazwaPlikuJpgBmp";
            this.textBoxNazwaPlikuJpgBmp.Size = new System.Drawing.Size(69, 20);
            this.textBoxNazwaPlikuJpgBmp.TabIndex = 36;
            // 
            // textBoxSciezkaZapisu
            // 
            this.textBoxSciezkaZapisu.Location = new System.Drawing.Point(1055, 423);
            this.textBoxSciezkaZapisu.Name = "textBoxSciezkaZapisu";
            this.textBoxSciezkaZapisu.Size = new System.Drawing.Size(69, 20);
            this.textBoxSciezkaZapisu.TabIndex = 37;
            this.textBoxSciezkaZapisu.Text = "c://selectedImages/";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(1005, 426);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(46, 13);
            this.label3.TabIndex = 38;
            this.label3.Text = "œcie¿ka:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(934, 452);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(117, 13);
            this.label4.TabIndex = 39;
            this.label4.Text = "nazwa pliku JPG/BMP:";
            // 
            // buttonDrukowanie
            // 
            this.buttonDrukowanie.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.buttonDrukowanie.Location = new System.Drawing.Point(971, 872);
            this.buttonDrukowanie.Name = "buttonDrukowanie";
            this.buttonDrukowanie.Size = new System.Drawing.Size(153, 56);
            this.buttonDrukowanie.TabIndex = 40;
            this.buttonDrukowanie.Text = "Drukowanie";
            this.buttonDrukowanie.UseVisualStyleBackColor = true;
            this.buttonDrukowanie.Click += new System.EventHandler(this.buttonDrukowanie_Click);
            // 
            // tbox1
            // 
            this.tbox1.Location = new System.Drawing.Point(135, 1);
            this.tbox1.Name = "tbox1";
            this.tbox1.Size = new System.Drawing.Size(20, 20);
            this.tbox1.TabIndex = 41;
            this.tbox1.Visible = false;
            // 
            // tbox2
            // 
            this.tbox2.Location = new System.Drawing.Point(135, 117);
            this.tbox2.Name = "tbox2";
            this.tbox2.Size = new System.Drawing.Size(20, 20);
            this.tbox2.TabIndex = 42;
            this.tbox2.Visible = false;
            // 
            // tbox3
            // 
            this.tbox3.Location = new System.Drawing.Point(135, 233);
            this.tbox3.Name = "tbox3";
            this.tbox3.Size = new System.Drawing.Size(20, 20);
            this.tbox3.TabIndex = 43;
            this.tbox3.Visible = false;
            // 
            // tbox4
            // 
            this.tbox4.Location = new System.Drawing.Point(135, 349);
            this.tbox4.Name = "tbox4";
            this.tbox4.Size = new System.Drawing.Size(20, 20);
            this.tbox4.TabIndex = 44;
            this.tbox4.Visible = false;
            // 
            // tbox5
            // 
            this.tbox5.Location = new System.Drawing.Point(135, 465);
            this.tbox5.Name = "tbox5";
            this.tbox5.Size = new System.Drawing.Size(20, 20);
            this.tbox5.TabIndex = 45;
            this.tbox5.Visible = false;
            // 
            // tbox6
            // 
            this.tbox6.Location = new System.Drawing.Point(135, 581);
            this.tbox6.Name = "tbox6";
            this.tbox6.Size = new System.Drawing.Size(20, 20);
            this.tbox6.TabIndex = 46;
            this.tbox6.Visible = false;
            // 
            // tbox12
            // 
            this.tbox12.Location = new System.Drawing.Point(135, 697);
            this.tbox12.Name = "tbox12";
            this.tbox12.Size = new System.Drawing.Size(20, 20);
            this.tbox12.TabIndex = 47;
            this.tbox12.Visible = false;
            // 
            // tbox7
            // 
            this.tbox7.Location = new System.Drawing.Point(288, 581);
            this.tbox7.Name = "tbox7";
            this.tbox7.Size = new System.Drawing.Size(20, 20);
            this.tbox7.TabIndex = 48;
            this.tbox7.Visible = false;
            // 
            // tbox13
            // 
            this.tbox13.Location = new System.Drawing.Point(288, 697);
            this.tbox13.Name = "tbox13";
            this.tbox13.Size = new System.Drawing.Size(20, 20);
            this.tbox13.TabIndex = 49;
            this.tbox13.Visible = false;
            // 
            // tbox8
            // 
            this.tbox8.Location = new System.Drawing.Point(441, 581);
            this.tbox8.Name = "tbox8";
            this.tbox8.Size = new System.Drawing.Size(20, 20);
            this.tbox8.TabIndex = 50;
            this.tbox8.Visible = false;
            // 
            // tbox14
            // 
            this.tbox14.Location = new System.Drawing.Point(441, 697);
            this.tbox14.Name = "tbox14";
            this.tbox14.Size = new System.Drawing.Size(20, 20);
            this.tbox14.TabIndex = 51;
            this.tbox14.Visible = false;
            // 
            // tbox9
            // 
            this.tbox9.Location = new System.Drawing.Point(594, 581);
            this.tbox9.Name = "tbox9";
            this.tbox9.Size = new System.Drawing.Size(20, 20);
            this.tbox9.TabIndex = 52;
            this.tbox9.Visible = false;
            // 
            // tbox15
            // 
            this.tbox15.Location = new System.Drawing.Point(594, 697);
            this.tbox15.Name = "tbox15";
            this.tbox15.Size = new System.Drawing.Size(20, 20);
            this.tbox15.TabIndex = 53;
            this.tbox15.Visible = false;
            // 
            // tbox10
            // 
            this.tbox10.Location = new System.Drawing.Point(747, 581);
            this.tbox10.Name = "tbox10";
            this.tbox10.Size = new System.Drawing.Size(20, 20);
            this.tbox10.TabIndex = 54;
            this.tbox10.Visible = false;
            // 
            // tbox16
            // 
            this.tbox16.Location = new System.Drawing.Point(747, 697);
            this.tbox16.Name = "tbox16";
            this.tbox16.Size = new System.Drawing.Size(20, 20);
            this.tbox16.TabIndex = 55;
            this.tbox16.Visible = false;
            // 
            // tbox11
            // 
            this.tbox11.Location = new System.Drawing.Point(900, 581);
            this.tbox11.Name = "tbox11";
            this.tbox11.Size = new System.Drawing.Size(20, 20);
            this.tbox11.TabIndex = 56;
            this.tbox11.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbox11.Visible = false;
            // 
            // tbox17
            // 
            this.tbox17.Location = new System.Drawing.Point(900, 697);
            this.tbox17.Name = "tbox17";
            this.tbox17.Size = new System.Drawing.Size(20, 20);
            this.tbox17.TabIndex = 57;
            this.tbox17.Visible = false;
            // 
            // selectedImgNumbersTextBox
            // 
            this.selectedImgNumbersTextBox.ForeColor = System.Drawing.Color.Red;
            this.selectedImgNumbersTextBox.Location = new System.Drawing.Point(1101, 713);
            this.selectedImgNumbersTextBox.Name = "selectedImgNumbersTextBox";
            this.selectedImgNumbersTextBox.Size = new System.Drawing.Size(23, 20);
            this.selectedImgNumbersTextBox.TabIndex = 58;
            // 
            // buttonClipboard
            // 
            this.buttonClipboard.Location = new System.Drawing.Point(1049, 124);
            this.buttonClipboard.Name = "buttonClipboard";
            this.buttonClipboard.Size = new System.Drawing.Size(75, 23);
            this.buttonClipboard.TabIndex = 59;
            this.buttonClipboard.Text = "Clipboard";
            this.buttonClipboard.UseVisualStyleBackColor = true;
            this.buttonClipboard.Click += new System.EventHandler(this.buttonClipboard_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(938, 307);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(46, 13);
            this.label5.TabIndex = 61;
            this.label5.Text = "Kontrast";
            // 
            // TrackbarKontrast
            // 
            this.TrackbarKontrast.LargeChange = 10;
            this.TrackbarKontrast.Location = new System.Drawing.Point(926, 323);
            this.TrackbarKontrast.Maximum = 100;
            this.TrackbarKontrast.Minimum = -100;
            this.TrackbarKontrast.Name = "TrackbarKontrast";
            this.TrackbarKontrast.Size = new System.Drawing.Size(192, 45);
            this.TrackbarKontrast.SmallChange = 5;
            this.TrackbarKontrast.TabIndex = 60;
            this.TrackbarKontrast.TickFrequency = 10;
            this.TrackbarKontrast.Scroll += new System.EventHandler(this.trackBar_Scroll);
            // 
            // buttonZapiszUstawienia
            // 
            this.buttonZapiszUstawienia.Location = new System.Drawing.Point(1049, 627);
            this.buttonZapiszUstawienia.Name = "buttonZapiszUstawienia";
            this.buttonZapiszUstawienia.Size = new System.Drawing.Size(75, 37);
            this.buttonZapiszUstawienia.TabIndex = 62;
            this.buttonZapiszUstawienia.Text = "Zapisz ustawienia";
            this.buttonZapiszUstawienia.UseVisualStyleBackColor = true;
            this.buttonZapiszUstawienia.Click += new System.EventHandler(this.buttonZapiszUstawienia_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(938, 243);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(57, 13);
            this.label6.TabIndex = 64;
            this.label6.Text = "Nasycenie";
            // 
            // TrackbarNasycenie
            // 
            this.TrackbarNasycenie.LargeChange = 1;
            this.TrackbarNasycenie.Location = new System.Drawing.Point(926, 259);
            this.TrackbarNasycenie.Minimum = -10;
            this.TrackbarNasycenie.Name = "TrackbarNasycenie";
            this.TrackbarNasycenie.Size = new System.Drawing.Size(192, 45);
            this.TrackbarNasycenie.TabIndex = 63;
            this.TrackbarNasycenie.Scroll += new System.EventHandler(this.trackBar_Scroll);
            // 
            // ZapiszBmpBtn
            // 
            this.ZapiszBmpBtn.Location = new System.Drawing.Point(1049, 598);
            this.ZapiszBmpBtn.Name = "ZapiszBmpBtn";
            this.ZapiszBmpBtn.Size = new System.Drawing.Size(75, 23);
            this.ZapiszBmpBtn.TabIndex = 65;
            this.ZapiszBmpBtn.Text = "Zapisz BMP";
            this.ZapiszBmpBtn.UseVisualStyleBackColor = true;
            this.ZapiszBmpBtn.Click += new System.EventHandler(this.buttonZapisBmp_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(1039, 716);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(61, 13);
            this.label7.TabIndex = 66;
            this.label7.Text = "wybranych:";
            // 
            // buttonStartKlatki
            // 
            this.buttonStartKlatki.Location = new System.Drawing.Point(1049, 182);
            this.buttonStartKlatki.Name = "buttonStartKlatki";
            this.buttonStartKlatki.Size = new System.Drawing.Size(75, 23);
            this.buttonStartKlatki.TabIndex = 67;
            this.buttonStartKlatki.Text = "Start Klatki";
            this.buttonStartKlatki.UseVisualStyleBackColor = true;
            this.buttonStartKlatki.Click += new System.EventHandler(this.buttonStartKlatki_Click);
            // 
            // buttonZlapKlatkiStop
            // 
            this.buttonZlapKlatkiStop.Location = new System.Drawing.Point(1049, 66);
            this.buttonZlapKlatkiStop.Name = "buttonZlapKlatkiStop";
            this.buttonZlapKlatkiStop.Size = new System.Drawing.Size(75, 23);
            this.buttonZlapKlatkiStop.TabIndex = 69;
            this.buttonZlapKlatkiStop.Text = "Stop";
            this.buttonZlapKlatkiStop.UseVisualStyleBackColor = true;
            this.buttonZlapKlatkiStop.Click += new System.EventHandler(this.buttonStopVideo_Click);
            // 
            // textBoxNasycenieNumber
            // 
            this.textBoxNasycenieNumber.Location = new System.Drawing.Point(1089, 243);
            this.textBoxNasycenieNumber.Name = "textBoxNasycenieNumber";
            this.textBoxNasycenieNumber.Size = new System.Drawing.Size(29, 20);
            this.textBoxNasycenieNumber.TabIndex = 70;
            // 
            // textBoxKontrastNumber
            // 
            this.textBoxKontrastNumber.Location = new System.Drawing.Point(1089, 307);
            this.textBoxKontrastNumber.Name = "textBoxKontrastNumber";
            this.textBoxKontrastNumber.Size = new System.Drawing.Size(29, 20);
            this.textBoxKontrastNumber.TabIndex = 71;
            // 
            // textBoxJasnoscNumber
            // 
            this.textBoxJasnoscNumber.Location = new System.Drawing.Point(1089, 367);
            this.textBoxJasnoscNumber.Name = "textBoxJasnoscNumber";
            this.textBoxJasnoscNumber.Size = new System.Drawing.Size(29, 20);
            this.textBoxJasnoscNumber.TabIndex = 72;
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // buttonAnuluj
            // 
            this.buttonAnuluj.Location = new System.Drawing.Point(1049, 670);
            this.buttonAnuluj.Name = "buttonAnuluj";
            this.buttonAnuluj.Size = new System.Drawing.Size(75, 37);
            this.buttonAnuluj.TabIndex = 73;
            this.buttonAnuluj.Text = "Anuluj wybrane";
            this.buttonAnuluj.UseVisualStyleBackColor = true;
            this.buttonAnuluj.Click += new System.EventHandler(this.buttonAnulujWybrane_Click);
            // 
            // radioButtonV1
            // 
            this.radioButtonV1.AutoSize = true;
            this.radioButtonV1.Checked = true;
            this.radioButtonV1.Location = new System.Drawing.Point(941, 214);
            this.radioButtonV1.Name = "radioButtonV1";
            this.radioButtonV1.Size = new System.Drawing.Size(62, 17);
            this.radioButtonV1.TabIndex = 74;
            this.radioButtonV1.TabStop = true;
            this.radioButtonV1.Text = "S-Video";
            this.radioButtonV1.UseVisualStyleBackColor = true;
            // 
            // radioButtonV2
            // 
            this.radioButtonV2.AutoSize = true;
            this.radioButtonV2.Location = new System.Drawing.Point(1006, 214);
            this.radioButtonV2.Name = "radioButtonV2";
            this.radioButtonV2.Size = new System.Drawing.Size(46, 17);
            this.radioButtonV2.TabIndex = 75;
            this.radioButtonV2.Text = "Vid1";
            this.radioButtonV2.UseVisualStyleBackColor = true;
            // 
            // radioButtonV3
            // 
            this.radioButtonV3.AutoSize = true;
            this.radioButtonV3.Location = new System.Drawing.Point(1071, 214);
            this.radioButtonV3.Name = "radioButtonV3";
            this.radioButtonV3.Size = new System.Drawing.Size(46, 17);
            this.radioButtonV3.TabIndex = 76;
            this.radioButtonV3.Text = "Vid2";
            this.radioButtonV3.UseVisualStyleBackColor = true;
            // 
            // buttonNagrywaj
            // 
            this.buttonNagrywaj.Location = new System.Drawing.Point(971, 507);
            this.buttonNagrywaj.Name = "buttonNagrywaj";
            this.buttonNagrywaj.Size = new System.Drawing.Size(75, 23);
            this.buttonNagrywaj.TabIndex = 77;
            this.buttonNagrywaj.Text = "Nagrywanie";
            this.buttonNagrywaj.UseVisualStyleBackColor = true;
            this.buttonNagrywaj.Click += new System.EventHandler(this.buttonNagrywaj_Click);
            // 
            // timerNagrywanie
            // 
            this.timerNagrywanie.Interval = 25;
            this.timerNagrywanie.Tick += new System.EventHandler(this.timerNagrywanie_Tick);
            // 
            // buttonStopNagrywanie
            // 
            this.buttonStopNagrywanie.Location = new System.Drawing.Point(1049, 507);
            this.buttonStopNagrywanie.Name = "buttonStopNagrywanie";
            this.buttonStopNagrywanie.Size = new System.Drawing.Size(75, 23);
            this.buttonStopNagrywanie.TabIndex = 78;
            this.buttonStopNagrywanie.Text = "Zatrzymaj";
            this.buttonStopNagrywanie.UseVisualStyleBackColor = true;
            this.buttonStopNagrywanie.Click += new System.EventHandler(this.buttonStopNagrywanie_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(965, 478);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(86, 13);
            this.label8.TabIndex = 80;
            this.label8.Text = "nazwa pliku AVI:";
            // 
            // textBoxNazwaPlikuAvi
            // 
            this.textBoxNazwaPlikuAvi.Location = new System.Drawing.Point(1055, 475);
            this.textBoxNazwaPlikuAvi.Name = "textBoxNazwaPlikuAvi";
            this.textBoxNazwaPlikuAvi.Size = new System.Drawing.Size(69, 20);
            this.textBoxNazwaPlikuAvi.TabIndex = 79;
            // 
            // tbox23
            // 
            this.tbox23.Location = new System.Drawing.Point(900, 813);
            this.tbox23.Name = "tbox23";
            this.tbox23.Size = new System.Drawing.Size(20, 20);
            this.tbox23.TabIndex = 92;
            this.tbox23.Visible = false;
            // 
            // tbox22
            // 
            this.tbox22.Location = new System.Drawing.Point(747, 813);
            this.tbox22.Name = "tbox22";
            this.tbox22.Size = new System.Drawing.Size(20, 20);
            this.tbox22.TabIndex = 91;
            this.tbox22.Visible = false;
            // 
            // tbox21
            // 
            this.tbox21.Location = new System.Drawing.Point(594, 813);
            this.tbox21.Name = "tbox21";
            this.tbox21.Size = new System.Drawing.Size(20, 20);
            this.tbox21.TabIndex = 90;
            this.tbox21.Visible = false;
            // 
            // tbox20
            // 
            this.tbox20.Location = new System.Drawing.Point(441, 813);
            this.tbox20.Name = "tbox20";
            this.tbox20.Size = new System.Drawing.Size(20, 20);
            this.tbox20.TabIndex = 89;
            this.tbox20.Visible = false;
            // 
            // tbox19
            // 
            this.tbox19.Location = new System.Drawing.Point(288, 813);
            this.tbox19.Name = "tbox19";
            this.tbox19.Size = new System.Drawing.Size(20, 20);
            this.tbox19.TabIndex = 88;
            this.tbox19.Visible = false;
            // 
            // tbox18
            // 
            this.tbox18.Location = new System.Drawing.Point(135, 813);
            this.tbox18.Name = "tbox18";
            this.tbox18.Size = new System.Drawing.Size(20, 20);
            this.tbox18.TabIndex = 87;
            this.tbox18.Visible = false;
            // 
            // picture19
            // 
            this.picture19.BackColor = System.Drawing.SystemColors.Control;
            this.picture19.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picture19.Location = new System.Drawing.Point(155, 813);
            this.picture19.Margin = new System.Windows.Forms.Padding(0);
            this.picture19.Name = "picture19";
            this.picture19.Size = new System.Drawing.Size(153, 115);
            this.picture19.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picture19.TabIndex = 86;
            this.picture19.TabStop = false;
            this.picture19.MouseDown += new System.Windows.Forms.MouseEventHandler(this.image_Click);
            // 
            // picture18
            // 
            this.picture18.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picture18.Location = new System.Drawing.Point(2, 813);
            this.picture18.Margin = new System.Windows.Forms.Padding(0);
            this.picture18.Name = "picture18";
            this.picture18.Size = new System.Drawing.Size(153, 115);
            this.picture18.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picture18.TabIndex = 85;
            this.picture18.TabStop = false;
            this.picture18.MouseDown += new System.Windows.Forms.MouseEventHandler(this.image_Click);
            // 
            // picture21
            // 
            this.picture21.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picture21.Location = new System.Drawing.Point(461, 813);
            this.picture21.Margin = new System.Windows.Forms.Padding(0);
            this.picture21.Name = "picture21";
            this.picture21.Size = new System.Drawing.Size(153, 115);
            this.picture21.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picture21.TabIndex = 84;
            this.picture21.TabStop = false;
            this.picture21.MouseDown += new System.Windows.Forms.MouseEventHandler(this.image_Click);
            // 
            // picture20
            // 
            this.picture20.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picture20.Location = new System.Drawing.Point(308, 813);
            this.picture20.Margin = new System.Windows.Forms.Padding(0);
            this.picture20.Name = "picture20";
            this.picture20.Size = new System.Drawing.Size(153, 115);
            this.picture20.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picture20.TabIndex = 83;
            this.picture20.TabStop = false;
            this.picture20.MouseDown += new System.Windows.Forms.MouseEventHandler(this.image_Click);
            // 
            // picture23
            // 
            this.picture23.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picture23.Location = new System.Drawing.Point(767, 813);
            this.picture23.Margin = new System.Windows.Forms.Padding(0);
            this.picture23.Name = "picture23";
            this.picture23.Size = new System.Drawing.Size(153, 115);
            this.picture23.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picture23.TabIndex = 82;
            this.picture23.TabStop = false;
            this.picture23.MouseDown += new System.Windows.Forms.MouseEventHandler(this.image_Click);
            // 
            // picture22
            // 
            this.picture22.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picture22.Location = new System.Drawing.Point(614, 813);
            this.picture22.Margin = new System.Windows.Forms.Padding(0);
            this.picture22.Name = "picture22";
            this.picture22.Size = new System.Drawing.Size(153, 115);
            this.picture22.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picture22.TabIndex = 81;
            this.picture22.TabStop = false;
            this.picture22.MouseDown += new System.Windows.Forms.MouseEventHandler(this.image_Click);
            // 
            // MainForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(1137, 971);
            this.Controls.Add(this.tbox18);
            this.Controls.Add(this.tbox19);
            this.Controls.Add(this.tbox23);
            this.Controls.Add(this.tbox22);
            this.Controls.Add(this.tbox21);
            this.Controls.Add(this.tbox20);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.textBoxNazwaPlikuAvi);
            this.Controls.Add(this.buttonStopNagrywanie);
            this.Controls.Add(this.buttonNagrywaj);
            this.Controls.Add(this.radioButtonV3);
            this.Controls.Add(this.radioButtonV2);
            this.Controls.Add(this.radioButtonV1);
            this.Controls.Add(this.buttonAnuluj);
            this.Controls.Add(this.textBoxJasnoscNumber);
            this.Controls.Add(this.textBoxKontrastNumber);
            this.Controls.Add(this.textBoxNasycenieNumber);
            this.Controls.Add(this.buttonZlapKlatkiStop);
            this.Controls.Add(this.buttonStartKlatki);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.ZapiszBmpBtn);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.TrackbarNasycenie);
            this.Controls.Add(this.buttonZapiszUstawienia);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.TrackbarKontrast);
            this.Controls.Add(this.buttonClipboard);
            this.Controls.Add(this.selectedImgNumbersTextBox);
            this.Controls.Add(this.tbox17);
            this.Controls.Add(this.tbox11);
            this.Controls.Add(this.tbox16);
            this.Controls.Add(this.tbox10);
            this.Controls.Add(this.tbox15);
            this.Controls.Add(this.tbox9);
            this.Controls.Add(this.tbox14);
            this.Controls.Add(this.tbox8);
            this.Controls.Add(this.tbox13);
            this.Controls.Add(this.tbox7);
            this.Controls.Add(this.tbox12);
            this.Controls.Add(this.tbox6);
            this.Controls.Add(this.tbox5);
            this.Controls.Add(this.tbox4);
            this.Controls.Add(this.tbox3);
            this.Controls.Add(this.tbox2);
            this.Controls.Add(this.tbox1);
            this.Controls.Add(this.buttonDrukowanie);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBoxSciezkaZapisu);
            this.Controls.Add(this.textBoxNazwaPlikuJpgBmp);
            this.Controls.Add(this.buttonZapiszJpg);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.TrackbarJasnosc);
            this.Controls.Add(this.buttonKasujKlatki);
            this.Controls.Add(this.buttonZlapKlatki);
            this.Controls.Add(this.buttonZlapKlatke);
            this.Controls.Add(this.picture13);
            this.Controls.Add(this.picture12);
            this.Controls.Add(this.picture15);
            this.Controls.Add(this.picture14);
            this.Controls.Add(this.picture17);
            this.Controls.Add(this.picture16);
            this.Controls.Add(this.picture7);
            this.Controls.Add(this.picture6);
            this.Controls.Add(this.picture9);
            this.Controls.Add(this.picture8);
            this.Controls.Add(this.picture11);
            this.Controls.Add(this.picture10);
            this.Controls.Add(this.picture5);
            this.Controls.Add(this.picture4);
            this.Controls.Add(this.picture3);
            this.Controls.Add(this.picture2);
            this.Controls.Add(this.picture1);
            this.Controls.Add(this.buttonVideoStop);
            this.Controls.Add(this.buttonVideoStart);
            this.Controls.Add(this.mainScreen);
            this.Controls.Add(this.picture23);
            this.Controls.Add(this.picture22);
            this.Controls.Add(this.picture21);
            this.Controls.Add(this.picture20);
            this.Controls.Add(this.picture19);
            this.Controls.Add(this.picture18);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Menu = this.mainMenu;
            this.Name = "MainForm";
            this.Text = "PicoloVideo";
            this.Closing += new System.ComponentModel.CancelEventHandler(this.MainForm_Closing);
            this.Closed += new System.EventHandler(this.MainForm_Closed);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.MainForm_Paint);
            ((System.ComponentModel.ISupportInitialize)(this.mainScreen)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picture1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picture2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picture3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picture4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picture5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picture11)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picture10)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picture9)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picture8)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picture7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picture6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picture13)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picture12)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picture15)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picture14)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picture17)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picture16)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.TrackbarJasnosc)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.TrackbarKontrast)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.TrackbarNasycenie)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picture19)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picture18)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picture21)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picture20)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picture23)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picture22)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// 
        private static Mutex mutex = null;  // !-!

        [STAThread]
        static void Main()
        {
            // !-!
            const string appName = "MyAppName";
            bool isAppCreated;

            mutex = new Mutex(true, appName, out isAppCreated);

            if (!isAppCreated)
            {
                DialogResult dialogResult = MessageBox.Show("Wykryto uruchomiony program, czy napewno kontynuowaæ?", "UWAGA !", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    //do something
                }
                else if (dialogResult == DialogResult.No)
                {
                    return;
                }
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }

        private void startChannel()
        {
           
            MC.OpenDriver(); 

            // Enable error logging
            MC.SetParam(MC.CONFIGURATION, "ErrorLog", "error.log"); 

            // Create a channel and associate it with the first connector on the first board
            MC.Create("CHANNEL", out channel); 
            try
            {
                MC.SetParam(MC.BOARD, "BoardTopology", "1_01_2");
            }
            catch(Euresys.MultiCamException exc) {
                MessageBox.Show("Problem topologii \n \n" + exc.Message);
            };

            try
            {
                MC.SetParam(channel, "DriverIndex", 0);

                if (radioButtonV1.Checked)
                    MC.SetParam(channel, "Connector", "YC");
                else if (radioButtonV2.Checked)
                    MC.SetParam(channel, "Connector", "VID1");
                else
                    MC.SetParam(channel, "Connector", "VID2");

                // Choose the video standard
                MC.SetParam(channel, "CamFile", "PAL");
                // Choose the pixel color format
                MC.SetParam(channel, "ColorFormat", "RGB24");

                // Choose the acquisition mode
                MC.SetParam(channel, "AcquisitionMode", "VIDEO");
            }
            catch (Euresys.MultiCamException exc)
            {
                MessageBox.Show("Problem nr. e1 \n \n" + exc.Message);
            }; 
        }
        
        private void MainForm_Load(object sender, System.EventArgs e)
        {
            Boolean uruchomienieBezKarty = true;
            if (uruchomienieBezKarty)
            {
                return;
            }

            // + PicoloVideo Sample Program
            
            try
            { 
                startChannel();

                // Choose the acquisition mode
                MC.SetParam(channel, "AcquisitionMode", "VIDEO");

                MC.SetParam(channel, "TrigMode", "COMBINED");
                // Choose the triggering mode for subsequent acquisitions
                MC.SetParam(channel, "NextTrigMode", "REPEAT");
                // Choose the number of images to acquire
                MC.SetParam(channel, "SeqLength_Fr", 1);
                MC.SetParam(channel, "TrigLineIndex", 1);
                MC.SetParam(channel, "TrigEdge", "GOLOW");

                // Register the callback function
                multiCamCallback = new MC.CALLBACK(MultiCamCallback);
                MC.RegisterCallback(channel, multiCamCallback, channel);

                // Enable the signals corresponding to the callback functions
                MC.SetParam(channel, MC.SignalEnable + MC.SIG_SURFACE_PROCESSING, "ON");
                MC.SetParam(channel, MC.SignalEnable + MC.SIG_ACQUISITION_FAILURE, "ON");
                MC.SetParam(channel, MC.SignalEnable + MC.SIG_END_CHANNEL_ACTIVITY, "ON");
                MC.SetParam(channel, "BufferPitch", 4096);
                ////channelActive = false;

                MC.SetParam(channel, "ChannelState", "ACTIVE"); // 
                Refresh();                                      //       AUT OSTART PRZY URUCHOMIENIU
                channelActive = true;                         //                
                Refresh();//

                //Prepare the channel in order to minimize the acquisition sequence startup latency
                MC.SetParam(channel, "ChannelState", "READY");
                MC.SetParam(channel, "ChannelState", "ACTIVE");

            }
            catch (Euresys.MultiCamException exc)
            {
                // An exception has occurred in the try {...} block. 
                // Retrieve its description and display it in a message box.
                MessageBox.Show("NIE WYKRYTO KARTY \n \n" + exc.Message, "MultiCam Exception");

                Close();
            }  

            // - PicoloVideo Sample Program
        }

        private void MultiCamCallback(ref MC.SIGNALINFO signalInfo)
        {
            switch (signalInfo.Signal)
            {
                case MC.SIG_SURFACE_PROCESSING:
                    ProcessingCallback(signalInfo);
                    break;
                case MC.SIG_ACQUISITION_FAILURE:
                    AcqFailureCallback(signalInfo);
                    break;
                case MC.SIG_END_CHANNEL_ACTIVITY:
                    try
                    {
                        if (triggerHardwerowyOn)
                        {
                            triggerFirstClick = true;
                            MC.SetParam(channel, "ChannelState", "ACTIVE");
                            actualImgNumber++;
                            if (actualImgNumber > imgBoxesList.Length - 1) actualImgNumber = 0;
                        }
                    }
                    catch (Euresys.MultiCamException exc)
                    {
                        MessageBox.Show("Problem nr. e2 \n \n" + exc.Message);
                    };
                    break;
                default:
                    throw new Euresys.MultiCamException("Unknown signal");
            }
        }

        private void ProcessingCallback(MC.SIGNALINFO signalInfo)
        {
            UInt32 currentChannel = (UInt32)signalInfo.Context;

            //statusBar.Text = "Processing";
            currentSurface = signalInfo.SignalInfo;

            // + PicoloVideo Sample Program

            try
            {
                // Update the image with the acquired image buffer data 
                Int32 width, height, bufferPitch;
                IntPtr bufferAddress;
                MC.GetParam(currentChannel, "ImageSizeX", out width);
                MC.GetParam(currentChannel, "ImageSizeY", out height);
                MC.GetParam(currentChannel, "BufferPitch", out bufferPitch);
                MC.GetParam(currentSurface, "SurfaceAddr", out bufferAddress);

                try
                {
                    FiltersSequence filter = new FiltersSequence(
                    new BrightnessCorrection(BrightnessValue),
                    new ContrastCorrection(contrastValue),
                    new SaturationCorrection(saturationValue)
                    );
                    // apply the filter                  

                    imageMutex.WaitOne();

                    globalHeightOfImage = height;
                    globalWidthOfImage = width;
                    tempAcquiredBitmap = new Bitmap(width, height, bufferPitch, PixelFormat.Format24bppRgb, bufferAddress);
                    acquiredBitmap = filter.Apply(tempAcquiredBitmap);

                }
                finally
                {
                    imageMutex.ReleaseMutex();
                }

                // Retrieve the frame rate
                Double frameRate_Hz;
                MC.GetParam(channel, "PerSecond_Fr", out frameRate_Hz);

                // Retrieve the channel state
                String channelState;
                MC.GetParam(channel, "ChannelState", out channelState);

                // Display the new image
                this.BeginInvoke(new PaintDelegate(Redraw), new object[1] { CreateGraphics() });
            }
            catch (Euresys.MultiCamException exc)
            {
                MessageBox.Show(exc.Message, "MultiCam Exception");
            }
            catch (System.Exception exc)
            {
                MessageBox.Show(exc.Message, "System Exception");
            }
        }

        private void AcqFailureCallback(MC.SIGNALINFO signalInfo)
        {
            UInt32 currentChannel = (UInt32)signalInfo.Context;

            // + PicoloVideo Sample Program

            try
            {
                // Display frame rate and channel state
                //statusBar.Text = String.Format("Acquisition Failure, Channel State: IDLE");
                this.BeginInvoke(new PaintDelegate(Redraw), new object[1] { CreateGraphics() });
            }
            catch (System.Exception exc)
            {
                MessageBox.Show(exc.Message, "System Exception");
            }

            // - PicoloVideo Sample Program
        }

        void Redraw(Graphics g)
        {
            // + PicoloVideo Sample Program
            try
            {
                imageMutex.WaitOne();

                if (acquiredBitmap != null && triggerHardwerowyOn && triggerFirstClick)
                    imgBoxesList[actualImgNumber].Image = new Bitmap(acquiredBitmap);

                if (acquiredBitmap != null && ekranGlownyOn)
                    mainScreen.Image = acquiredBitmap; // wyswietla video na glownym ekranie
            }
            catch (System.Exception exc)
            {
                MessageBox.Show(exc.Message, "System Exception");
            }
            finally
            {
                imageMutex.ReleaseMutex();
            }
        }

        private void MainForm_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            Redraw(e.Graphics);
        }

        private void MainForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Application.Exit();

            try
            {
                if (channel != 0)
                    MC.SetParam(channel, "ChannelState", "IDLE");
                channelActive = false;

                // Whait that the channel has finished the last acquisition
                while (channelActive == true)
                {
                    Thread.Sleep(10);
                }

                // Delete the channel
                if (channel != 0)
                {
                    MC.Delete(channel);
                    channel = 0;
                }
            }
            catch (Euresys.MultiCamException exc)
            {
                MessageBox.Show(exc.Message, "MultiCam Exception");
            }
        }

        // ZLAP HARDWEROWO
        private void buttonStartKlatki_Click(object sender, System.EventArgs e)
        {
            try
            {
                triggerFirstClick = false;
                triggerHardwerowyOn = false;
                //Thread.Sleep(100);
                KanalTrigger();
                // Start an acquisition sequence by activating the channel
                String channelState;
                MC.GetParam(channel, "ChannelState", out channelState);
                if (channelState != "ACTIVE")
                    MC.SetParam(channel, "ChannelState", "ACTIVE");
                Refresh();
                channelActive = true;
                ekranGlownyOn = true;
                triggerHardwerowyOn = true;
            }
            catch { }
        }

        private void MainForm_Closed(object sender, System.EventArgs e)
        {
            try
            {
                Application.Exit();
                //Close MultiCam driver                
                MC.CloseDriver();
            }
            catch (Euresys.MultiCamException exc)
            {
                MessageBox.Show(exc.Message, "MultiCam Exception");
            }
        }

        ///////////////////////////////////////////////////////////
        // ZLAP KLATKE
        private void buttonZlapKlatke_Click(object sender, EventArgs e)
        {
            try
            {
                KanalVideo();
                timerZlapKlatki.Stop();
                triggerFirstClick = false;
                triggerHardwerowyOn = false;
                //Thread.Sleep(100);
                KanalTrigger();
                // Start an acquisition sequence by activating the channel
                String channelState;
                MC.GetParam(channel, "ChannelState", out channelState);
                if (channelState != "ACTIVE")
                    MC.SetParam(channel, "ChannelState", "ACTIVE");
                Refresh();
                channelActive = true;
                ekranGlownyOn = true;
                triggerHardwerowyOn = true;

            }
            catch (System.NullReferenceException exc)
            {
                MessageBox.Show("Problem nr. e3 \n \n" + exc.Message);
            }

            for (int i = 0; i < 3; i++)
            {
                try
                {
                    mainScreen.Image = new Bitmap(acquiredBitmap);
                }
                catch (System.NullReferenceException exc)
                {
                    MessageBox.Show("Problem nr. e4 \n \n" + exc.Message);
                }
            }
        }

        // RESET ZDJEC 
        private void buttonResetZdjec_Click(object sender, EventArgs e)
        {
            triggerHardwerowyOn = false;
            timerZlapKlatki.Stop();
            actualImgNumber = 0;
            //selectedImgNumber = 0;
            selectedImages.Clear();

            for (int i = 0; i < imgBoxesList.Length; i++)
            {
                imgBoxesList[i].Image = null;
                imgBoxesList[i].BorderStyle = BorderStyle.FixedSingle;
                seletedImgMarkerList[i].Visible = false;
            }
        }

        // ZLAP KLATKI
        private void buttonZlapKlatki_Click(object sender, EventArgs e)
        {
            triggerHardwerowyOn = false;
            KanalVideo();
            timerZlapKlatki.Start();
        }

        // TIMER TICK
        private void TimerZlapKlatki_Tick(object sender, EventArgs e)
        {
            triggerHardwerowyOn = false;
            try
            {
                imgBoxesList[actualImgNumber].Image = new Bitmap(acquiredBitmap);
                actualImgNumber++;
                if (actualImgNumber > imgBoxesList.Length-1) actualImgNumber = 0;
            }
            catch
            {
                MessageBox.Show("Nie wykryto kamery");
            }
        }

        // VIDEO ON
        private void buttonVideoStart_Click(object sender, EventArgs e)
        {
            videoStart();
        }

        private void videoStart()
        {
            triggerHardwerowyOn = false;
            KanalVideo();
            ekranGlownyOn = true;
        }

        // VIDEO OFF
        private void buttonVideoStop_Click(object sender, EventArgs e)
        {
            videoStop();
        }

        private void videoStop()
        {
            triggerHardwerowyOn = false;
            ekranGlownyOn = false;
            mainScreen.Image = null;
        }

        //  CLICK LEWY / PRAWY        
        private void image_Click(object sender, MouseEventArgs e)
        {
            triggerHardwerowyOn = false;
            PictureBox clickedImg = (PictureBox)sender;

            switch (MouseButtons)
            {
                case MouseButtons.Left:

                    if (clickedImg.BorderStyle == BorderStyle.FixedSingle)
                    {
                        clickedImg.BorderStyle = BorderStyle.Fixed3D;
                        selectedImages.Add(clickedImg.Image);
                    }
                    else
                    {
                        clickedImg.BorderStyle = BorderStyle.FixedSingle;
                        selectedImages.Remove(clickedImg.Image);
                    }

                    for (int i = 0; i < seletedImgMarkerList.Length; i++)
                        if (imgBoxesList[i].BorderStyle == BorderStyle.Fixed3D)
                        {
                            seletedImgMarkerList[i].Visible = true;
                            seletedImgMarkerList[i].TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
                            seletedImgMarkerList[i].Text = "V";
                            seletedImgMarkerList[i].ForeColor = System.Drawing.Color.Green;
                        }
                        else seletedImgMarkerList[i].Visible = false;

                    selectedImgNumbersTextBox.Text = selectedImages.Count.ToString();
                    if (selectedImages.Count > imagesToPrint.Length)
                    {
                        buttonDrukowanie.Enabled = false;
                        selectedImgNumbersTextBox.ForeColor = System.Drawing.Color.Red;
                        foreach (var seletedImgMarker in seletedImgMarkerList)
                        {
                            seletedImgMarker.ForeColor = Color.Red;
                            seletedImgMarker.Text = "X";
                        }
                    }

                    else
                    {
                        buttonDrukowanie.Enabled = true;
                        selectedImgNumbersTextBox.ForeColor = System.Drawing.Color.Black;
                    }
                    break;


                case MouseButtons.Right:
                    mainScreen.Image = clickedImg.Image;
                    //NewBitmap = new Bitmap(pic.Image);
                    NewBitmap = (Bitmap)clickedImg.Image;
                    Img_zapis = NewBitmap;
                    TrackbarJasnosc.Enabled = true;
                    TrackbarKontrast.Enabled = true;
                    TrackbarNasycenie.Enabled = true;
                    ekranGlownyOn = false;
                    break;
            }
        }

        // ZAPIS DO PLIKU .JPG
        private void buttonZapisJPG_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists(textBoxSciezkaZapisu.Text))
                Directory.CreateDirectory(textBoxSciezkaZapisu.Text);

            int fileSuffixNumber = 0;
            String sciezkaZapisuJpeg = (textBoxSciezkaZapisu.Text + textBoxNazwaPlikuJpgBmp.Text + fileSuffixNumber + ".jpg");

            if (mainScreen.Image != null)
            {
                if (!File.Exists(sciezkaZapisuJpeg))
                {
                    mainScreen.Image.Save(sciezkaZapisuJpeg, ImageFormat.Jpeg);
                }
                else
                {
                    while (File.Exists(sciezkaZapisuJpeg))
                    {
                        fileSuffixNumber++;
                        sciezkaZapisuJpeg = (textBoxSciezkaZapisu.Text + textBoxNazwaPlikuJpgBmp.Text + fileSuffixNumber + ".jpg");
                    }
                    mainScreen.Image.Save(sciezkaZapisuJpeg, ImageFormat.Jpeg);
                }
            }
        }

        // ZAPIS DO PLIKU .BMP
        private void buttonZapisBmp_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists(textBoxSciezkaZapisu.Text))
                Directory.CreateDirectory(textBoxSciezkaZapisu.Text);

            int fileSuffixNumber = 0;
            String sciezkaZapisuBmp = (textBoxSciezkaZapisu.Text + textBoxNazwaPlikuJpgBmp.Text + fileSuffixNumber.ToString() + ".bmp");

            if (mainScreen.Image != null)
            {
                if (!File.Exists(sciezkaZapisuBmp))
                {
                    mainScreen.Image.Save(sciezkaZapisuBmp, ImageFormat.Bmp);
                }
                else
                {
                    while (File.Exists(sciezkaZapisuBmp))
                    {
                        fileSuffixNumber++;
                        sciezkaZapisuBmp = (textBoxSciezkaZapisu.Text + textBoxNazwaPlikuJpgBmp.Text + fileSuffixNumber.ToString() + ".bmp");
                    }
                    mainScreen.Image.Save(sciezkaZapisuBmp, ImageFormat.Bmp);
                }
            }
            }

        // DRUKOWANIE
        private void buttonDrukowanie_Click(object sender, EventArgs e)
        {
            try
            {
                PrintingWindowForm printingForm = new PrintingWindowForm();
                printingForm.Show();

                for (int i = 0; i < imagesToPrint.Length; i++)
                {
                    imagesToPrint[i] = null;
                }

                selectedImages.CopyTo(imagesToPrint);

                if (imagesToPrint[0] != null)
                    printingForm.pic11.Image = imagesToPrint[0];
                if (imagesToPrint[1] != null)
                    printingForm.pic12.Image = imagesToPrint[1];
                if (imagesToPrint[2] != null)
                    printingForm.pic13.Image = imagesToPrint[2];
                if (imagesToPrint[3] != null)
                    printingForm.pic21.Image = imagesToPrint[3];
                if (imagesToPrint[4] != null)
                    printingForm.pic22.Image = imagesToPrint[4];
                if (imagesToPrint[5] != null)
                    printingForm.pic23.Image = imagesToPrint[5];
                if (imagesToPrint[6] != null)
                    printingForm.pic31.Image = imagesToPrint[6];
                if (imagesToPrint[7] != null)
                    printingForm.pic32.Image = imagesToPrint[7];
                if (imagesToPrint[8] != null)
                    printingForm.pic33.Image = imagesToPrint[8];
            }
            catch { }
        }


        private void buttonClipboard_Click(object sender, EventArgs e)
        {
            if (mainScreen.Image != null)
            {
                Clipboard.SetImage(mainScreen.Image);
            }
        }

        // TRACKBARY
        private void trackBar_Scroll(object sender, EventArgs e)
        {
            saturationValue = (float)TrackbarNasycenie.Value / 100;
            textBoxNasycenieNumber.Text = TrackbarNasycenie.Value.ToString();

            contrastValue = TrackbarKontrast.Value;
            textBoxKontrastNumber.Text = contrastValue.ToString();

            BrightnessValue = TrackbarJasnosc.Value;
            textBoxJasnoscNumber.Text = BrightnessValue.ToString();
        }

        private void buttonZapiszUstawienia_Click(object sender, EventArgs e)
        {
            if (!File.Exists(configFilePath))
                File.CreateText(configFilePath);

            try
            {
                using (StreamWriter sw = new StreamWriter(configFilePath))
                {
                    sw.WriteLine(TrackbarJasnosc.Value);
                    sw.WriteLine(TrackbarKontrast.Value);
                    sw.WriteLine(TrackbarNasycenie.Value);
                    sw.WriteLine(textBoxSciezkaZapisu.Text);
                    sw.WriteLine(textBoxNazwaPlikuJpgBmp.Text);
                    sw.WriteLine(textBoxNazwaPlikuAvi.Text);
                }
            }
            catch
            {
                MessageBox.Show("B³¹d zapisu pliku configuracyjnego");
            }
        }

        private void KanalVideo()
        { 
            triggerHardwerowyOn = false;
            string tryb;

            MC.GetParam(channel, "TrigMode", out tryb);

            if (tryb == "COMBINED")
            {
                MC.SetParam(channel, "ChannelState", "IDLE");
                //MC.CloseDriver();
                MC.Delete(channel);
                channel = 0;

                Thread.Sleep(10);

                startChannel();

                // Choose the way the first acquisition is triggered
                MC.SetParam(channel, "TrigMode", "IMMEDIATE");
                // Choose the triggering mode for subsequent acquisitions
                MC.SetParam(channel, "NextTrigMode", "REPEAT");
                // Choose the number of images to acquire
                MC.SetParam(channel, "SeqLength_Fr", MC.INDETERMINATE);

                // Register the callback function
                multiCamCallback = new MC.CALLBACK(MultiCamCallback);
                MC.RegisterCallback(channel, multiCamCallback, channel);

                // Enable the signals corresponding to the callback functions
                MC.SetParam(channel, MC.SignalEnable + MC.SIG_SURFACE_PROCESSING, "ON");
                MC.SetParam(channel, MC.SignalEnable + MC.SIG_ACQUISITION_FAILURE, "ON");
                MC.SetParam(channel, MC.SignalEnable + MC.SIG_END_CHANNEL_ACTIVITY, "ON");
                MC.SetParam(channel, "BufferPitch", 4096);
                channelActive = false;

                MC.SetParam(channel, "ChannelState", "ACTIVE"); // 
                Refresh();                                      //       AUT OSTART PRZY URUCHOMIENIU
                channelActive = true;                           //

                Refresh();
            } 
        }

        private void KanalTrigger()
        {
            triggerHardwerowyOn = false;
            string tryb;

            MC.GetParam(channel, "TrigMode", out tryb);

            if (tryb != "COMBINED")
            {

                MC.SetParam(channel, "ChannelState", "IDLE");
                channelActive = false;
                MC.Delete(channel);
                channel = 0;

                Thread.Sleep(10);

                startChannel();

                MC.SetParam(channel, "TrigMode", "COMBINED");
                // Choose the triggering mode for subsequent acquisitions
                MC.SetParam(channel, "NextTrigMode", "REPEAT");
                // Choose the number of images to acquire
                MC.SetParam(channel, "SeqLength_Fr", 1);
                MC.SetParam(channel, "TrigLineIndex", 1);
                MC.SetParam(channel, "TrigEdge", "GOLOW");

                // Register the callback function
                multiCamCallback = new MC.CALLBACK(MultiCamCallback);
                MC.RegisterCallback(channel, multiCamCallback, channel);

                // Enable the signals corresponding to the callback functions
                MC.SetParam(channel, MC.SignalEnable + MC.SIG_SURFACE_PROCESSING, "ON");
                MC.SetParam(channel, MC.SignalEnable + MC.SIG_ACQUISITION_FAILURE, "ON");
                MC.SetParam(channel, MC.SignalEnable + MC.SIG_END_CHANNEL_ACTIVITY, "ON");
                MC.SetParam(channel, "BufferPitch", 4096);
            }
        }

        private void buttonStopVideo_Click(object sender, EventArgs e)
        {
            timerZlapKlatki.Stop();
        }

        private void buttonAnulujWybrane_Click(object sender, EventArgs e)
        {
            selectedImages.Clear();
            for (int i = 0; i < imgBoxesList.Length; i++)
            {
                imgBoxesList[i].BorderStyle = BorderStyle.FixedSingle;
                selectedImgNumbersTextBox.Text = selectedImages.Count.ToString();
                buttonDrukowanie.Enabled = true;
                selectedImgNumbersTextBox.ForeColor = System.Drawing.Color.Black;
                seletedImgMarkerList[i].Visible = false;
            }
        }

        private void buttonNagrywaj_Click(object sender, EventArgs e)
        {
            videoStart();
            if (!Directory.Exists(textBoxSciezkaZapisu.Text))
                Directory.CreateDirectory(textBoxSciezkaZapisu.Text);

            int fileSuffixNumber = 0;
            string aviPath = textBoxSciezkaZapisu.Text + textBoxNazwaPlikuAvi.Text + ".avi";

            if (!File.Exists(aviPath))
            {
                disableButtons();

                ekranGlownyOn = false;
                triggerHardwerowyOn = false;
                KanalVideo();
                int width = globalWidthOfImage;
                int height = globalHeightOfImage;

                try
                {
                    writer.Open(aviPath, width, height, 25, VideoCodec.MPEG4, 1000000);
                }
                catch { }

                timerNagrywanie.Start();
                buttonNagrywaj.BackColor = Color.Green;
            }
            else
            {
                while (File.Exists(aviPath))
                {
                    fileSuffixNumber++;
                    aviPath = (textBoxSciezkaZapisu.Text + textBoxNazwaPlikuAvi.Text + fileSuffixNumber + ".avi");
                }
                disableButtons();
                ekranGlownyOn = false;
                triggerHardwerowyOn = false;
                KanalVideo();
                int width = globalWidthOfImage;
                int height = globalHeightOfImage;

                try
                {
                    writer.Open(aviPath, width, height, 25, VideoCodec.MPEG4, 1000000);
                }
                catch { }

                timerNagrywanie.Start();
                buttonNagrywaj.BackColor = Color.Green;

            }          
        }

        private void disableButtons()
        {
            buttonZlapKlatke.Enabled = false;
            buttonZlapKlatki.Enabled = false;
            buttonZlapKlatkiStop.Enabled = false;
            buttonKasujKlatki.Enabled = false;
            buttonClipboard.Enabled = false;
            buttonVideoStart.Enabled = false;
            buttonVideoStop.Enabled = false;
            buttonStartKlatki.Enabled = false;
        }

        private void enableButtons()
        {
            buttonZlapKlatke.Enabled = true;
            buttonZlapKlatki.Enabled = true;
            buttonZlapKlatkiStop.Enabled = true;
            buttonKasujKlatki.Enabled = true;
            buttonClipboard.Enabled = true;
            buttonVideoStart.Enabled = true;
            buttonVideoStop.Enabled = true;
            buttonStartKlatki.Enabled = true;
        }

        private void timerNagrywanie_Tick(object sender, EventArgs e)
        {                       
            
            try
            {
                writer.WriteVideoFrame(acquiredBitmap);                
            }
            catch
            { }
        }

        private void buttonStopNagrywanie_Click(object sender, EventArgs e)
        {
            timerNagrywanie.Stop();
            writer.Close();            
            enableButtons();
            buttonNagrywaj.BackColor = Color.White;
            videoStop();
        }              
                

        private void makeAvi(List<Bitmap> maps)
        {
            triggerHardwerowyOn = false;
            KanalVideo();
            timerNagrywanie.Start();
            AviManager aviManager = new AviManager("local.avi", false);

            //false means do not show the diag of the Compression 
            //21 means the fps of the video
            //maplist[0] cover of the video  the maplist is the val you should insert 
            VideoStream avistream = aviManager.AddVideoStream(false, 21, maps[0]);

            for (int i = 1; i < maps.Count; i++)
            {
                avistream.AddFrame(maps[i]);
            }

            aviManager.Close();
            MessageBox.Show("AddOk");
        }

       

        private void makeAviStop()
        {
            timerNagrywanie.Stop();
            makeAvi(listaZdjeciaVideo);

            if (!Directory.Exists(textBoxSciezkaZapisu.Text))
                Directory.CreateDirectory(textBoxSciezkaZapisu.Text);

            int fileSuffixNumber = 0;
            String sciezkaZapisuJpeg = (textBoxSciezkaZapisu.Text + textBoxNazwaPlikuJpgBmp.Text + fileSuffixNumber + ".jpg");

            foreach (Image tempImage in listaZdjeciaVideo)
            {
                while (File.Exists(sciezkaZapisuJpeg))
                {
                    fileSuffixNumber++;
                    sciezkaZapisuJpeg = (textBoxSciezkaZapisu.Text + textBoxNazwaPlikuJpgBmp.Text + fileSuffixNumber + ".jpg");
                }
                tempImage.Save(sciezkaZapisuJpeg, ImageFormat.Jpeg);

            }

        }
    }


}
