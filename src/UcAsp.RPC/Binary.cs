﻿/***************************************************
*创建人:TecD02
*创建时间:2016/7/30 18:52:56
*功能说明:<Function>
*版权所有:<Copyright>
*Frameworkversion:4.0
*CLR版本：4.0.30319.42000
***************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
namespace UcAsp.RPC
{   /// <summary>
    /// 二进制数据
    /// </summary>
    /// 
    [Serializable]
    public class Binary
    {
        private byte[] _buffer;

        /// <summary>
        /// 基础数据
        /// </summary>
        public byte[] Buffer
        {
            get
            {
                byte[] gbuffer = null;
                using (MemoryStream dms = new MemoryStream())
                {
                    using (MemoryStream cms = new MemoryStream(this._buffer))
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
                return gbuffer;
            }
            set
            {
                byte[] gizpbytes = null;
                using (MemoryStream cms = new MemoryStream())
                {
                    using (System.IO.Compression.GZipStream gzip = new System.IO.Compression.GZipStream(cms, System.IO.Compression.CompressionMode.Compress))
                    {
                        //将数据写入基础流，同时会被压缩
                        gzip.Write(value, 0, value.Length);
                    }
                    gizpbytes = cms.ToArray();
                }
                this._buffer = gizpbytes;
            }
        }

        /// <summary>
        /// 二进制数据
        /// </summary>
        /// <param name="buffer">数据</param>
        public Binary(byte[] buffer)
        {
            byte[] gizpbytes = null;
            using (MemoryStream cms = new MemoryStream())
            {
                using (System.IO.Compression.GZipStream gzip = new System.IO.Compression.GZipStream(cms, System.IO.Compression.CompressionMode.Compress))
                {
                    //将数据写入基础流，同时会被压缩
                    gzip.Write(buffer, 0, buffer.Length);
                }
                gizpbytes = cms.ToArray();
            }
            this._buffer = gizpbytes;

          //  this._buffer = buffer;
        }
    }
}
