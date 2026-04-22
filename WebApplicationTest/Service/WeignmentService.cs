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

using WeightmentAPI.Models;

namespace WebApplicationTest.Service

{

    public class WeightService

    {

        private readonly string _portName = "COM1";

        public WeightResponse ReadWeight()

        {

            try

            {

                using (SerialPort port = new SerialPort(_portName, 9600, Parity.None, 8, StopBits.One))

                {

                    port.Handshake = Handshake.None;

                    port.ReadTimeout = 5000;

                    port.NewLine = "\r\n"; // adjust if needed

                    port.Open();

                    // ✅ Read ONE full line instead of random chunks

                    string rawData = port.ReadLine();

                    Console.WriteLine("RAW: " + rawData);

                    // ✅ Extract numeric weight

                    var match = Regex.Match(rawData, @"\d+(\.\d+)?");

                    if (match.Success)

                    {

                        return new WeightResponse

                        {

                            Success = true,

                            Port = _portName,

                            Data = match.Value,

                            Message = "Weight read successfully"

                        };

                    }

                    return new WeightResponse

                    {

                        Success = false,

                        Message = "No valid weight found in response"

                    };

                }

            }

            catch (TimeoutException)

            {

                return new WeightResponse

                {

                    Success = false,

                    Message = "⏱ Timeout: No data received from device"

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

        }

    }

}
