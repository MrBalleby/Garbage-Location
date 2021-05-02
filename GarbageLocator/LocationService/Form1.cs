using gps;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LocationService
{
    public partial class Form1 : Form
    {
        public gps.parser.Nmea parser = new gps.parser.Nmea();
        public gps.parser.MinimalNmeaPositionNotifier mn = new gps.parser.MinimalNmeaPositionNotifier();

        public SerialPort serialPort1 = new SerialPort();
        Timer timer2 = new Timer();
        public Form1()
        {
            InitializeComponent();
            serialPort1.PortName = "COM3"; //USB GPS PORT
            serialPort1.BaudRate = 9600;
            serialPort1.NewLine = "\r\n";
            serialPort1.ReceivedBytesThreshold = 500000;
            serialPort1.ReadBufferSize = 1048576;
            timer2.Enabled = true;
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            timer2.Interval = 10;
            timer2.Tick += Timer2_Tick;
            timer2.Start();
        }

        //Timer der kører hvert 10 millisekund for at fange data fra stream (serial port (usb port) )
        private void Timer2_Tick(object sender, EventArgs e)
        {
            int bytestoread = 0;
            //stop timer for at læse data
            timer2.Stop();
            try
            {
                //gem data i bytestoread
                bytestoread = serialPort1.BytesToRead;
            }

            catch (InvalidOperationException ex)
            {
                if ((uint)ex.HResult == 0x80131509)
                {
                    timer2.Stop();
                }
                richTextBox1.Text = ex.Message;
            }

            //Hvis serial porten er åben
            if (serialPort1.IsOpen)
            {
                if (bytestoread != 0)
                {
                    //Lav et temp byte array for at gemme data deri
                    byte[] temp = new byte[bytestoread];

                    if (serialPort1.IsOpen)
                        serialPort1.Read(temp, 0, bytestoread); //Gemmer data i arrayet

                    try
                    {
                        //GPS koden der går ind og konveterer bytes til noget vi kan læse/forstå
                        Gps2Coords gps = new Gps2Coords();
                        //g.parser.NewMessage += new gps.parser.Nmea.NewMessageEventHandler(HandleNewMessage);
                        gps.mn.NewGspPosition += new gps.parser.MinimalNmeaPositionNotifier.NewGspPositionEventHandler(NewGspPosition);
                        gps.mn.Init(gps.parser);

                        //Her læser den data fra temp som der blev gemt før, og starter konveteringen:
                        using (BufferedStream bf = new BufferedStream(new MemoryStream(temp)))
                        {
                            gps.parser.Source = bf;
                            gps.parser.Start();
                            gps.parser.WaitDone();
                        }
                    }
                    catch (Exception ex)
                    {
                        richTextBox1.Text = "Error: " + ex.Message;
                    }

                }
                timer2.Start();
            }
        }
        private List<string> gpsData = new List<string>();

        //Tilføjer de konveterede værdier til en liste, som kan bruges senere::
        //Til start bliver de sendt ud til tekstboksen
        private void NewGspPosition(gps.parser.GpsPosition pos)
        {
            gpsData.Add(pos.x.ToString().Replace(',', '.'));
            gpsData.Add(pos.y.ToString().Replace(',', '.'));
            if (gpsData.Count > 0)
            {
                foreach (var item in gpsData)
                {
                    richTextBox1.Text += item + Environment.NewLine;
                }
            }
        }

    }
}
