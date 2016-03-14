﻿using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Nabbix
{
    class QueryHandler
    {
        internal static void Run(TcpClient client)
        {
            var stream = client.GetStream();

            try {
                do
                {
                    String request = GetRequest(stream);

                    String response = new Random().Next().ToString();
                    if (request == "agent.ping")
                    {
                        response = "1";
                    }

                    // For now the response is always a random integer
                    SendResponse(stream, response);

                } while (stream.DataAvailable);
            }
            catch(Exception e)
            {

            }
        }

        private static string GetRequest(NetworkStream stream)
        {
            StringBuilder builder = new StringBuilder();

            int oneByte;
            do
            {
                oneByte = stream.ReadByte();

                builder.Append((char)oneByte);

            } while (oneByte != -1 && oneByte != 10);

            return builder.ToString();
        }

        private static void SendResponse(NetworkStream stream, String response)
        {
            BufferedStream output = new BufferedStream(stream);
            //output.WriteByte()
            // Write header
            // https://www.zabbix.com/documentation/1.8/protocols
            // "ZBXD\x01" (5 bytes)
            output.WriteByte((byte)'Z');
            output.WriteByte((byte)'B');
            output.WriteByte((byte)'X');
            output.WriteByte((byte)'D');
            output.WriteByte(1);

            long length = response.Length;
            for (int i = 0; i < 8; i++)
            {
                output.WriteByte((byte)(int)(length & 0xFF));
                length >>= 8;
            }

            for (int i = 0; i < response.Length; i++)
            {
                output.WriteByte((byte)response[i]);
            }

            output.Flush();
        }
    }
}