using System;
using System.IO.Ports;
using System.Threading;
using WeightmentAPI.Models;

namespace WebApplicationTest.Service
{
    public class WeightService
    {
        private readonly string _portName = "COM1"; // ✅ FIXED PORT

        public WeightResponse ReadWeight()
        {
            try
            {
                using (SerialPort port = new SerialPort(_portName))
                {
                    // ✅ MATCH YOUR DEVICE SETTINGS
                    port.BaudRate = 9600;
                    port.DataBits = 8;
                    port.Parity = Parity.None;
                    port.StopBits = StopBits.One;
                    port.Handshake = Handshake.None;

                    port.ReadTimeout = 3000;

                    port.Open();

                    // ✅ wait for machine to send data
                    Thread.Sleep(2000);

                    string data = port.ReadLine(); // 🔥 IMPORTANT

                    port.Close();

                    if (!string.IsNullOrWhiteSpace(data))
                    {
                        return new WeightResponse
                        {
                            Success = true,
                            Port = _portName,
                            Data = data.Trim(),
                            Message = "Weight read successfully"
                        };
                    }

                    return new WeightResponse
                    {
                        Success = false,
                        Message = "No data received"
                    };
                }
            }
            catch (TimeoutException)
            {
                return new WeightResponse
                {
                    Success = false,
                    Message = "Timeout: No data from machine"
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
        }
    }
}
//using System.IO.Ports;
//using WebApplicationTest.Models;
//using WeightmentAPI.Models;

//namespace WebApplicationTest.Service
//{
//    public class WeightService
//    {
//        // 👉 BEST PRACTICE: Use fixed COM port (change if needed)
//        private readonly string _portName = "COM3";

//        public WeightResponse ReadWeight()
//        {
//            try
//            {
//                using (SerialPort port = new SerialPort(_portName))
//                {
//                    // ✅ Proper configuration
//                    port.BaudRate = 9600;
//                    port.Parity = Parity.None;
//                    port.DataBits = 8;
//                    port.StopBits = StopBits.One;
//                    port.Handshake = Handshake.None;

//                    port.ReadTimeout = 5000;
//                    port.WriteTimeout = 2000;

//                    port.Open();

//                    // ✅ Give device time
//                    Thread.Sleep(2000);

//                    string data = port.ReadLine();

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
//                        Message = "No data received from device"
//                    };
//                }
//            }
//            catch (TimeoutException)
//            {
//                return new WeightResponse
//                {
//                    Success = false,
//                    Message = "Timeout: No response from weighing machine"
//                };
//            }
//            catch (Exception ex)
//            {
//                return new WeightResponse
//                {
//                    Success = false,
//                    Message = $"Error: {ex.Message}"
//                };
//            }
//        }
//    }
//}