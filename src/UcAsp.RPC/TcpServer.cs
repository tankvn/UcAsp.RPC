﻿/***************************************************
*创建人:TecD02
*创建时间:2016/8/4 18:12:48
*功能说明:<Function>
*版权所有:<Copyright>
*Frameworkversion:4.0
*CLR版本：4.0.30319.42000
***************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using UcAsp.RPC;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;
using log4net;
using System.IO;
namespace UcAsp.RPC
{
    public class TcpServer : ServerBase
    {
        private readonly ILog _log = LogManager.GetLogger(typeof(TcpServer));
        private Socket _server;

        public override void StartListen(int port)
        {
            _server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this._server.Bind(new IPEndPoint(IPAddress.Any, port));
            this._server.Listen(3000);
            _log.Info("开启服务：" + port);
            // Console.WriteLine(_server.LocalEndPoint);

            /// while (true)
            //{
            //Socket socket = this._server.Accept();


            ThreadPool.QueueUserWorkItem(Accept, null);

            //}


        }
        private void Accept(object obj)
        {
            while (true)
            {
                Socket socket = this._server.Accept();
                Console.WriteLine("连接：" + socket.RemoteEndPoint);
                _log.Info("连接：" + socket.RemoteEndPoint);
                ThreadPool.QueueUserWorkItem(Recive, socket);
            }
        }
        private void Recive(object obj)
        {
            Socket socket = (Socket)(obj);
            while (true)
            {
                ByteBuilder _recvBuilder = new ByteBuilder(socket.ReceiveBufferSize);
                if (socket.Connected)
                {
                    try
                    {
                        byte[] buffer = new byte[buffersize];
                        int total = 0;
                        while (true)
                        {
                            int len = socket.ReceiveBufferSize;
                            buffer = new byte[len];
                            socket.Receive(buffer);
                            byte[] gbuffer = null;
                            using (MemoryStream dms = new MemoryStream())
                            {
                                using (MemoryStream cms = new MemoryStream(buffer))
                                {

                                    using (System.IO.Compression.GZipStream gzip = new System.IO.Compression.GZipStream(cms, System.IO.Compression.CompressionMode.Decompress))
                                    {

                                        byte[] bytes = new byte[1024];
                                        int glen = 0;
                                        try
                                        {
                                            //读取压缩流，同时会被解压
                                            while ((glen = gzip.Read(bytes, 0, bytes.Length)) > 0)
                                            {
                                                dms.Write(bytes, 0, glen);
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            Console.WriteLine(ex);
                                        }
                                        gbuffer = dms.ToArray();

                                    }
                                }

                            }

                            _recvBuilder.Add(gbuffer);
                            total = _recvBuilder.GetInt32(0);
                            //Thread.Sleep(1);
                            if (_recvBuilder.Count == total)
                            { break; }
                        }
                        DataEventArgs e = DataEventArgs.Parse(_recvBuilder);

                        Call(socket, e);

                    }
                    catch (Exception ex)
                    {
                        _log.Error(ex);
                        Console.WriteLine(ex);
                        //socket.Dispose();
                        // Thread thread = Thread.CurrentThread;
                        // thread.Abort();


                    }
                }
            }


        }





    }
}
