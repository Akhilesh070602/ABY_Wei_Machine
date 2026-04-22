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

        private static readonly object _lock = new object();

        public WeightResponse ReadWeight()

        {

            lock (_lock) // 🔥 prevents multiple access

            {

                try

                {

                    using (SerialPort port = new SerialPort("COM1", 9600, Parity.None, 8, StopBits.One))

                    {

                        port.ReadTimeout = 5000;

                        port.Open();

                        string buffer = "";

                        string finalWeight = "";

                        for (int i = 0; i < 15; i++)

                        {

                            buffer += port.ReadExisting();

                            if (!string.IsNullOrEmpty(buffer))

                            {

                                var match = Regex.Match(buffer, @"\d+(\.\d+)?");

                                if (match.Success)

                                {

                                    finalWeight = match.Value;

                                    break;

                                }

                            }

                            Thread.Sleep(200);

                        }

                        if (!string.IsNullOrEmpty(finalWeight))

                        {

                            return new WeightResponse

                            {

                                Success = true,

                                Port = "COM1",

                                Data = finalWeight,

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

                catch (Exception ex)

                {
                    return new WeightResponse
                    {
                        Success = false,
                        Message = ex.ToString() // 🔥 Full details
                    };

                }

            }

        }

    }

}

