using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Agile.GPS.Models;

namespace Agile.GPS
{
    public interface IGpsService
    {
        void Initialize(string comPort);
        GpsData ReadData();
    }

    public class GpsService : IGpsService
    {
        private SerialPort _port;

        public GpsService()
        {

        }

        // Pass the name of COM port (ie. "COM13") that the GPS unit
        // is attached to in order to initialize the unit and begin receiving data.
        public void Initialize(string comPort)
        {
            _port = new SerialPort();
            _port.BaudRate = 9600;
            _port.Parity = Parity.None;
            _port.DataBits = 8;
            _port.StopBits = StopBits.One;
            _port.ReadTimeout = SerialPort.InfiniteTimeout;
            _port.PortName = comPort;
            _port.Handshake = Handshake.None;
            _port.Open();
        }

        // Reads and returns the current location data from the GPS unit
        public GpsData ReadData()
        {
            var speedUpdated = false;
            var coordsUpdated = false;

            var gpsData = new GpsData();
            if (_port.IsOpen)
            {
                string data = _port.ReadExisting();
                string[] strArr = data.Split('$');
                foreach (var line in strArr)
                {
                    string[] lineArr = line.Split(',');
                    switch (lineArr[0])
                    {
                        case "GPGGA":
                            try
                            {
                                if (String.IsNullOrEmpty(lineArr[2]) || String.IsNullOrEmpty(lineArr[5]))
                                {
                                    return null;
                                }

                                //Latitude
                                var dLat = Convert.ToDouble(lineArr[2]);
                                dLat = dLat / 100;
                                var lat = dLat.ToString().Split('.');
                                var latitude = lineArr[3].ToString() + lat[0].ToString() + "." + ((Convert.ToDouble(lat[1]) / 60)).ToString("#####");
                                gpsData.Latitude = latitude;
                                //Longitude
                                var dLon = Convert.ToDouble(lineArr[4]);
                                dLon = dLon / 100;
                                var lon = dLon.ToString().Split('.');
                                var longitude = lineArr[5].ToString() + lon[0].ToString() + "." + ((Convert.ToDouble(lon[1]) / 60)).ToString("#####");
                                gpsData.Longitude = longitude;

                                coordsUpdated = true;
                            }
                            catch (Exception ex)
                            {
                                var err = ex.ToString();
                            }
                            break;
                        default:
                            break;
                    }

                }
            }

            if (!speedUpdated)
            {
                gpsData.Speed = "0 mph";
            }

            return coordsUpdated ? gpsData : null;
        }
    }
}
