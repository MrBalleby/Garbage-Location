using System;
using System.IO;
using gps.parser;

namespace gps
{
    public class Gps2Coords
    {
        public gps.parser.Nmea parser = new gps.parser.Nmea();
        public gps.parser.MinimalNmeaPositionNotifier mn = new gps.parser.MinimalNmeaPositionNotifier();

        [STAThread]
        public void GetData(string[] args)
        {
            try
            {
                if ((args == null) || (args.Length < 1))
                {
                    throw new Exception("Usage: gsp2coords gps-nmea-file");
                }
                Gps2Coords gps = new Gps2Coords();
                //g.parser.NewMessage += new gps.parser.Nmea.NewMessageEventHandler(HandleNewMessage);
                gps.mn.NewGspPosition += new gps.parser.MinimalNmeaPositionNotifier.NewGspPositionEventHandler(gps.NewGspPosition);
                gps.mn.Init(gps.parser);
                using(BufferedStream bf = new BufferedStream(new FileStream(args[0], FileMode.Open, FileAccess.Read)))
                {
                    gps.outFile = new System.IO.StreamWriter(@"C:\temp\gpsTest" + ".coords.txt");
                    gps.parser.Source = bf;
                    gps.parser.Start();
                    gps.parser.WaitDone();
                    if (gps.outFile != null)
                    {
                        gps.outFile.Close();
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        private StreamWriter outFile = null;
        private void NewGspPosition(gps.parser.GpsPosition pos)
        {
            outFile.WriteLine(pos.x.ToString().Replace(',', '.'));
            outFile.WriteLine(pos.y.ToString().Replace(',', '.'));
        }
    }//EOC

}//EON