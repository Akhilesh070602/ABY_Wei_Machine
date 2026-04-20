//using System.IO.Ports;
//using WebApplicationTest.Models;
//using WeightmentAPI.Models;

//namespace WebApplicationTest.Service
//{
//    public class WeightService
//    {
//        public WeightResponse ReadWeightAuto()
//        {
//            Console.WriteLine("=== START WEIGHT READ ===");

//            var ports = SerialPort.GetPortNames();

//            if (ports.Length == 0)
//            {
//                Console.WriteLine("❌ No COM ports found");

//                return new WeightResponse
//                {
//                    Success = false,
//                    Message = "No COM ports found (USB/RS232 not connected)"
//                };
//            }

//            foreach (var portName in ports)
//            {
//                try
//                {
//                    Console.WriteLine($"Trying {portName}");

//                    using (SerialPort port = new SerialPort(portName, 9600))
//                    {
//                        port.Open();
//                        Thread.Sleep(1500);

//                        string data = port.ReadExisting();

//                        port.Close();

//                        Console.WriteLine($"Data from {portName}: {data}");

//                        if (!string.IsNullOrWhiteSpace(data))
//                        {
//                            return new WeightResponse
//                            {
//                                Success = true,
//                                Port = portName,
//                                Data = data,
//                                Message = "Weight read successfully"
//                            };
//                        }
//                    }
//                }
//                catch (Exception ex)
//                {
//                    Console.WriteLine($"Error on {portName}: {ex.Message}");
//                }
//            }

//            return new WeightResponse
//            {
//                Success = false,
//                Message = "No valid data received"
//            };
//        }
//    }
//}
using System.IO.Ports;
using WebApplicationTest.Models;
using WeightmentAPI.Models;

namespace WebApplicationTest.Service
{
    public class WeightService
    {
        // 👉 BEST PRACTICE: Use fixed COM port (change if needed)
        private readonly string _portName = "COM3";

        public WeightResponse ReadWeight()
        {
            try
            {
                using (SerialPort port = new SerialPort(_portName))
                {
                    // ✅ Proper configuration
                    port.BaudRate = 9600;
                    port.Parity = Parity.None;
                    port.DataBits = 8;
                    port.StopBits = StopBits.One;
                    port.Handshake = Handshake.None;

                    port.ReadTimeout = 5000;
                    port.WriteTimeout = 2000;

                    port.Open();

                    // ✅ Give device time
                    Thread.Sleep(2000);

                    string data = port.ReadLine();

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
                        Message = "No data received from device"
                    };
                }
            }
            catch (TimeoutException)
            {
                return new WeightResponse
                {
                    Success = false,
                    Message = "Timeout: No response from weighing machine"
                };
            }
            catch (Exception ex)
            {
                return new WeightResponse
                {
                    Success = false,
                    Message = $"Error: {ex.Message}"
                };
            }
        }
    }
}