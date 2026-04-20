using System.IO.Ports;
using WebApplicationTest.Models;
using WeightmentAPI.Models;

namespace WebApplicationTest.Service
{
    public class WeightService
    {
        public WeightResponse ReadWeightAuto()
        {
            Console.WriteLine("=== START WEIGHT READ ===");

            var ports = SerialPort.GetPortNames();

            if (ports.Length == 0)
            {
                Console.WriteLine("❌ No COM ports found");

                return new WeightResponse
                {
                    Success = false,
                    Message = "No COM ports found (USB/RS232 not connected)"
                };
            }

            foreach (var portName in ports)
            {
                try
                {
                    Console.WriteLine($"Trying {portName}");

                    using (SerialPort port = new SerialPort(portName, 9600))
                    {
                        port.Open();
                        Thread.Sleep(1500);

                        string data = port.ReadExisting();

                        port.Close();

                        Console.WriteLine($"Data from {portName}: {data}");

                        if (!string.IsNullOrWhiteSpace(data))
                        {
                            return new WeightResponse
                            {
                                Success = true,
                                Port = portName,
                                Data = data,
                                Message = "Weight read successfully"
                            };
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error on {portName}: {ex.Message}");
                }
            }

            return new WeightResponse
            {
                Success = false,
                Message = "No valid data received"
            };
        }
    }
}