using System;
using Agile.GPS;
using System.Windows.Forms;
using Agile.GPS.Models;

namespace LocationService
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
           
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                GpsData gpsData = null;
                GpsService gpsService = new GpsService();
                gpsService.Initialize("COM3");

                while (gpsData == null)
                {
                    if (gpsService.ReadData() != null)
                    {
                        gpsData = new GpsData();
                        gpsData = gpsService.ReadData();
                        if (gpsData != null)
                        {
                            richTextBox1.Text = "lat:\t" + gpsData.Latitude + "\n" + "lon:\t" + gpsData.Longitude + "\n";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                richTextBox1.Text = ex.Message;
            }
        }


    }
}
