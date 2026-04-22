//using System;
//using System.IO.Ports;
//using System.Threading;
//using WeightmentAPI.Models;

//namespace WebApplicationTest.Service
//{
//    public class WeightService
//    {
//        private readonly string _portName = "COM1";

//        public WeightResponse ReadWeight()
//        {
//            try
//            {
//                using (SerialPort port = new SerialPort(_portName))
//                {
//                    port.BaudRate = 9600;
//                    port.DataBits = 8;
//                    port.Parity = Parity.None;
//                    port.StopBits = StopBits.One;
//                    port.Handshake = Handshake.None;

//                    port.ReadTimeout = 3000;

//                    port.Open();

//                    Thread.Sleep(2000); // allow device to send

//                    string data = "";

//                    // 🔥 READ MULTIPLE TIMES (IMPORTANT)
//                    for (int i = 0; i < 5; i++)
//                    {
//                        data += port.ReadExisting();
//                        Thread.Sleep(300);
//                    }

//                    port.Close();

//                    if (!string.IsNullOrWhiteSpace(data))
//                    {
//                        return new WeightResponse
//                        {
//                            Success = true,
//                            Port = _portName,
//                            Data = data.Trim(),
//                            Message = "Weight read successfully"
//                        };
//                    }

//                    return new WeightResponse
//                    {
//                        Success = false,
//                        Message = "No data received from machine"
//                    };
//                }
//            }
//            catch (Exception ex)
//            {
//                return new WeightResponse
//                {
//                    Success = false,
//                    Message = ex.Message
//                };
//            }
//        }
//    }
//}

using System;
using System.IO.Ports;
using System.Text.RegularExpressions;
using System.Threading;
using WeightmentAPI.Models;

namespace WebApplicationTest.Service
{
    public class WeightService
    {
        private readonly string _portName = "COM1";

        public WeightResponse ReadWeight()
        {
            SerialPort port = null;

            try
            {
                port = new SerialPort(_portName);

                // ✅ MATCH DEVICE SETTINGS
                port.BaudRate = 1200;
                port.DataBits = 7;
                port.Parity = Parity.None;
                port.StopBits = StopBits.One;
                port.Handshake = Handshake.None;

                port.ReadTimeout = 3000;

                port.Open();

                Console.WriteLine("✅ Port opened");

                Thread.Sleep(2000);

                string finalWeight = "";

                for (int i = 0; i < 5; i++)
                {
                    string raw = port.ReadExisting();

                    Console.WriteLine("RAW: " + raw);

                    var match = Regex.Match(raw, @"\d+(\.\d+)?");

                    if (match.Success)
                    {
                        finalWeight = match.Value;
                    }

                    Thread.Sleep(500);
                }

                if (!string.IsNullOrEmpty(finalWeight))
                {
                    return new WeightResponse
                    {
                        Success = true,
                        Port = _portName,
                        Data = finalWeight,
                        Message = "Weight read successfully"
                    };
                }

                return new WeightResponse
                {
                    Success = false,
                    Message = "No valid weight found"
                };
            }
            catch (UnauthorizedAccessException)
            {
                return new WeightResponse
                {
                    Success = false,
                    Message = "❌ COM1 is being used by another application"
                };
            }
            catch (Exception ex)
            {
                return new WeightResponse
                {
                    Success = false,
                    Message = ex.Message
                };
            }
            finally
            {
                if (port != null && port.IsOpen)
                    port.Close();
            }
        }
    }
}