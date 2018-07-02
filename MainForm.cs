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

namespace PicoloVideo
{
    /// <summary>
    /// Summary description for Form1.
    /// </summary>
    /// 

    public class MainForm : System.Windows.Forms.Form
    {
        // Creation of an event for asynchronous call to paint function
        public delegate void PaintDelegate(Graphics g);
        public delegate void UpdateStatusBarDelegate(String text);

        // The object that will contain the acquired image
        private Bitmap image = null;

        // The Mutex object that will protect image objects during processing
        private static Mutex imageMutex = new Mutex();

        // The MultiCam object that controls the acquisition
        UInt32 channel;

        // The MultiCam object that contains the acquired buffer
        private UInt32 currentSurface;
        
        // Track if acquisition is ongoing
        private volatile bool channelactive;

        MC.CALLBACK multiCamCallback;

        private System.Windows.Forms.MainMenu mainMenu;
        private System.Windows.Forms.StatusBar statusBar;
        private System.Windows.Forms.StatusBarPanel statusBarPanel1;
        private PictureBox main_screen;
        private Button go;
        private Button stop;
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
        private System.ComponentModel.IContainer components;

        public MainForm()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            // Show scope of the sample program
            MessageBox.Show(@"
                This program demonstrates the VIDEO Acquisition Mode on a Picolo Board.
                
                The Go! menu starts an acquisition sequence by activating the channel.
                By default, this program requires an PAL camera connected on VID1.",
                "Sample program description", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            this.statusBar = new System.Windows.Forms.StatusBar();
            this.statusBarPanel1 = new System.Windows.Forms.StatusBarPanel();
            this.main_screen = new System.Windows.Forms.PictureBox();
            this.go = new System.Windows.Forms.Button();
            this.stop = new System.Windows.Forms.Button();
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
            ((System.ComponentModel.ISupportInitialize)(this.statusBarPanel1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.main_screen)).BeginInit();
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
            this.SuspendLayout();
            // 
            // statusBar
            // 
            this.statusBar.Location = new System.Drawing.Point(0, 964);
            this.statusBar.Name = "statusBar";
            this.statusBar.Panels.AddRange(new System.Windows.Forms.StatusBarPanel[] {
            this.statusBarPanel1});
            this.statusBar.ShowPanels = true;
            this.statusBar.Size = new System.Drawing.Size(1264, 22);
            this.statusBar.TabIndex = 0;
            this.statusBar.Text = "Ready. Click on the \'GO!\' button to start.";
            // 
            // statusBarPanel1
            // 
            this.statusBarPanel1.Name = "statusBarPanel1";
            this.statusBarPanel1.Width = 300;
            // 
            // main_screen
            // 
            this.main_screen.Location = new System.Drawing.Point(387, 12);
            this.main_screen.Name = "main_screen";
            this.main_screen.Size = new System.Drawing.Size(768, 576);
            this.main_screen.TabIndex = 1;
            this.main_screen.TabStop = false;
            // 
            // go
            // 
            this.go.Location = new System.Drawing.Point(1177, 103);
            this.go.Name = "go";
            this.go.Size = new System.Drawing.Size(75, 23);
            this.go.TabIndex = 2;
            this.go.Text = "Video start";
            this.go.UseVisualStyleBackColor = true;
            this.go.Click += new System.EventHandler(this.Go_Click);
            // 
            // stop
            // 
            this.stop.Location = new System.Drawing.Point(1177, 132);
            this.stop.Name = "stop";
            this.stop.Size = new System.Drawing.Size(75, 23);
            this.stop.TabIndex = 3;
            this.stop.Text = "Video stop";
            this.stop.UseVisualStyleBackColor = true;
            this.stop.Click += new System.EventHandler(this.Stop_Click);
            // 
            // picture1
            // 
            this.picture1.Location = new System.Drawing.Point(30, 12);
            this.picture1.Name = "picture1";
            this.picture1.Size = new System.Drawing.Size(153, 115);
            this.picture1.TabIndex = 4;
            this.picture1.TabStop = false;
            // 
            // picture2
            // 
            this.picture2.Location = new System.Drawing.Point(30, 128);
            this.picture2.Name = "picture2";
            this.picture2.Size = new System.Drawing.Size(153, 115);
            this.picture2.TabIndex = 6;
            this.picture2.TabStop = false;
            // 
            // picture3
            // 
            this.picture3.Location = new System.Drawing.Point(30, 244);
            this.picture3.Name = "picture3";
            this.picture3.Size = new System.Drawing.Size(153, 115);
            this.picture3.TabIndex = 8;
            this.picture3.TabStop = false;
            // 
            // picture4
            // 
            this.picture4.Location = new System.Drawing.Point(30, 360);
            this.picture4.Name = "picture4";
            this.picture4.Size = new System.Drawing.Size(153, 115);
            this.picture4.TabIndex = 10;
            this.picture4.TabStop = false;
            // 
            // picture5
            // 
            this.picture5.Location = new System.Drawing.Point(30, 476);
            this.picture5.Name = "picture5";
            this.picture5.Size = new System.Drawing.Size(153, 115);
            this.picture5.TabIndex = 12;
            this.picture5.TabStop = false;
            // 
            // picture11
            // 
            this.picture11.Location = new System.Drawing.Point(1000, 594);
            this.picture11.Name = "picture11";
            this.picture11.Size = new System.Drawing.Size(153, 115);
            this.picture11.TabIndex = 15;
            this.picture11.TabStop = false;
            // 
            // picture10
            // 
            this.picture10.Location = new System.Drawing.Point(806, 594);
            this.picture10.Name = "picture10";
            this.picture10.Size = new System.Drawing.Size(153, 115);
            this.picture10.TabIndex = 14;
            this.picture10.TabStop = false;
            // 
            // picture9
            // 
            this.picture9.Location = new System.Drawing.Point(612, 594);
            this.picture9.Name = "picture9";
            this.picture9.Size = new System.Drawing.Size(153, 115);
            this.picture9.TabIndex = 17;
            this.picture9.TabStop = false;
            // 
            // picture8
            // 
            this.picture8.Location = new System.Drawing.Point(418, 594);
            this.picture8.Name = "picture8";
            this.picture8.Size = new System.Drawing.Size(153, 115);
            this.picture8.TabIndex = 16;
            this.picture8.TabStop = false;
            // 
            // picture7
            // 
            this.picture7.Location = new System.Drawing.Point(224, 594);
            this.picture7.Name = "picture7";
            this.picture7.Size = new System.Drawing.Size(153, 115);
            this.picture7.TabIndex = 19;
            this.picture7.TabStop = false;
            // 
            // picture6
            // 
            this.picture6.Location = new System.Drawing.Point(30, 592);
            this.picture6.Name = "picture6";
            this.picture6.Size = new System.Drawing.Size(153, 115);
            this.picture6.TabIndex = 18;
            this.picture6.TabStop = false;
            // 
            // picture13
            // 
            this.picture13.Location = new System.Drawing.Point(224, 715);
            this.picture13.Name = "picture13";
            this.picture13.Size = new System.Drawing.Size(153, 115);
            this.picture13.TabIndex = 25;
            this.picture13.TabStop = false;
            // 
            // picture12
            // 
            this.picture12.Location = new System.Drawing.Point(30, 715);
            this.picture12.Name = "picture12";
            this.picture12.Size = new System.Drawing.Size(153, 115);
            this.picture12.TabIndex = 24;
            this.picture12.TabStop = false;
            // 
            // picture15
            // 
            this.picture15.Location = new System.Drawing.Point(612, 715);
            this.picture15.Name = "picture15";
            this.picture15.Size = new System.Drawing.Size(153, 115);
            this.picture15.TabIndex = 23;
            this.picture15.TabStop = false;
            // 
            // picture14
            // 
            this.picture14.Location = new System.Drawing.Point(418, 715);
            this.picture14.Name = "picture14";
            this.picture14.Size = new System.Drawing.Size(153, 115);
            this.picture14.TabIndex = 22;
            this.picture14.TabStop = false;
            // 
            // picture17
            // 
            this.picture17.Location = new System.Drawing.Point(1000, 715);
            this.picture17.Name = "picture17";
            this.picture17.Size = new System.Drawing.Size(153, 115);
            this.picture17.TabIndex = 21;
            this.picture17.TabStop = false;
            // 
            // picture16
            // 
            this.picture16.Location = new System.Drawing.Point(806, 715);
            this.picture16.Name = "picture16";
            this.picture16.Size = new System.Drawing.Size(153, 115);
            this.picture16.TabIndex = 20;
            this.picture16.TabStop = false;
            // 
            // MainForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(1264, 986);
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
            this.Controls.Add(this.stop);
            this.Controls.Add(this.go);
            this.Controls.Add(this.main_screen);
            this.Controls.Add(this.statusBar);
            this.Menu = this.mainMenu;
            this.Name = "MainForm";
            this.Text = "PicoloVideo";
            this.Closing += new System.ComponentModel.CancelEventHandler(this.MainForm_Closing);
            this.Closed += new System.EventHandler(this.MainForm_Closed);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.MainForm_Paint);
            ((System.ComponentModel.ISupportInitialize)(this.statusBarPanel1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.main_screen)).EndInit();
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
            this.ResumeLayout(false);

        }
        #endregion

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.Run(new MainForm());
        }

        private void MainForm_Load(object sender, System.EventArgs e)
        {

            // + PicoloVideo Sample Program

            try
            {
                // Open MultiCam driver
                MC.OpenDriver();

                // Enable error logging
                MC.SetParam(MC.CONFIGURATION, "ErrorLog", "error.log");

                // Create a channel and associate it with the first connector on the first board
                MC.Create("CHANNEL", out channel);
                MC.SetParam(channel, "DriverIndex", 0);
                MC.SetParam(channel, "Connector", "VID1");

                // Choose the video standard
                MC.SetParam(channel, "CamFile", "PAL");
                // Choose the pixel color format
                MC.SetParam(channel, "ColorFormat", "RGB24");

                // Choose the acquisition mode
                MC.SetParam(channel, "AcquisitionMode", "VIDEO");
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
                channelactive = false;

                // Prepare the channel in order to minimize the acquisition sequence startup latency
                MC.SetParam(channel, "ChannelState", "READY");
            }
            catch (Euresys.MultiCamException exc)
            {
                // An exception has occurred in the try {...} block. 
                // Retrieve its description and display it in a message box.
                MessageBox.Show(exc.Message, "MultiCam Exception");
                Close();
            }

            // - PicoloVideo Sample Program
        }


        private void MultiCamCallback(ref MC.SIGNALINFO signalInfo)
        {
            switch(signalInfo.Signal)
            {
                case MC.SIG_SURFACE_PROCESSING:
                    ProcessingCallback(signalInfo);
                    break;
                case MC.SIG_ACQUISITION_FAILURE:
                    AcqFailureCallback(signalInfo);
                    break;
                case MC.SIG_END_CHANNEL_ACTIVITY:
                    channelactive = false;
                    break;
                default:
                    throw new Euresys.MultiCamException("Unknown signal");
            }
        }

        private void ProcessingCallback(MC.SIGNALINFO signalInfo)
        {
            UInt32 currentChannel = (UInt32)signalInfo.Context;

            statusBar.Text = "Processing";
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
                    imageMutex.WaitOne();

                    image = new Bitmap(width, height, bufferPitch, PixelFormat.Format24bppRgb, bufferAddress);

                    /* Insert image analysis and processing code here */
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

                // Display frame rate and channel state
                statusBar.Text = String.Format("Frame Rate: {0:f2}, Channel State: {1}", frameRate_Hz, channelState);

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
                statusBar.Text = String.Format("Acquisition Failure, Channel State: IDLE");
                this.BeginInvoke(new PaintDelegate(Redraw), new object[1] { CreateGraphics() });
            }
            catch (System.Exception exc)
            {
                MessageBox.Show(exc.Message, "System Exception");
            }

            // - PicoloVideo Sample Program
        }

        private void UpdateStatusBar(String text)
        {
            statusBarPanel1.Text = text;
        }

        void Redraw(Graphics g)
        {
            // + PicoloVideo Sample Program

            try
            {
                imageMutex.WaitOne();

                main_screen.Image = image;
                if (image != null)
                   // g.DrawImage(image, 0, 0); 
                UpdateStatusBar(statusBar.Text);
            }
            catch (System.Exception exc)
            {
                MessageBox.Show(exc.Message, "System Exception");
            }
            finally
            {
                imageMutex.ReleaseMutex();
            }

            // - PicoloVideo Sample Program
        }

        private void MainForm_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            Redraw(e.Graphics);
        }

        private void MainForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                Stop_Click(sender, e);
                // Whait that the channel has finished the last acquisition
                while (channelactive == true)
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

        private void Go_Click(object sender, System.EventArgs e)
        {
            // + PicoloVideo Sample Program


            // Start an acquisition sequence by activating the channel
            String channelState;
            MC.GetParam(channel, "ChannelState", out channelState);
            if (channelState != "ACTIVE")
                MC.SetParam(channel, "ChannelState", "ACTIVE");
            Refresh();
            channelactive = true;

            // - PicoloVideo Sample Program
        }

        private void Stop_Click(object sender, System.EventArgs e)
        {
            // + PicoloVideo Sample Program
            // Stop an acquisition sequence by deactivating the channel
            if (channel != 0)
                MC.SetParam(channel, "ChannelState", "IDLE");
            UpdateStatusBar(String.Format("Frame Rate: {0:f2}, Channel State: IDLE", 0));

            // - PicoloVideo Sample Program
        }

        private void MainForm_Closed(object sender, System.EventArgs e)
        {
            try
            {
                // Close MultiCam driver
                MC.CloseDriver();
            }
            catch (Euresys.MultiCamException exc)
            {
                MessageBox.Show(exc.Message, "MultiCam Exception");
            }
        }

        
    }
}
