namespace RoleDomain.Common.Import
{
 
    using System;
    using System.Data;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Text;

    public class IOHelper
    {
        /// <summary>
        /// 比较两个byte[]是否相同
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool Compare_byte(byte[] a, byte[] b) {
            if (a.Length == b.Length) {
                for (int i = 0; i < a.Length; i++) {

                    if (a[i] != b[i]) {
                        return false;
                    }
                }
                return true;
            }

            return false;
        }

        public static byte[] bitmap2byte(Bitmap b)
        {
            byte[] bytes = null;
            using (MemoryStream ms = new MemoryStream())
            {
                b.Save(ms, ImageFormat.Jpeg);
                bytes = ms.GetBuffer();
                ms.Close();
            }
            return bytes;
        }

        public static Bitmap byte2bitmap(byte[] bytes)
        {
            byte[] bytelist = bytes;
            Bitmap bm = null;
            using (MemoryStream ms1 = new MemoryStream(bytelist))
            {
                bm = (Bitmap) Image.FromStream(ms1);
                ms1.Close();
            }
            return bm;
        }

        public static byte[] file_reader(FileInfo file)
        {
            using (FileStream fs = new FileStream(file.FullName, FileMode.Open, FileAccess.Read))
            {
                int nBytes = (int) fs.Length;
                byte[] byteArray = new byte[nBytes];
                int nBytesRead = fs.Read(byteArray, 0, nBytes);
                fs.Close();
               
                return byteArray;
            }
        }

        public static string file_reader(FileInfo file, Encoding encoding)
        {
            string text = "";

            using (StreamReader sr = new StreamReader(file.FullName, encoding, true, 4096))
            {
                text = sr.ReadToEnd();

            }
            return text;
        }

        /// <summary>
        /// 自动识别UTF-8或者GBK编码的文件读取
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static string file2text(FileInfo file)
        {
            string text = "";

            using (StreamReader sr = new StreamReader(file.FullName, EncodingType.GetType(file.FullName), true, 4096))
            {
                text = sr.ReadToEnd();

            }
            return text;
        }



        public static byte[] file_reader(string file)
        {
            return file_reader(new FileInfo(file));
        }

        public static string file_readtext(FileInfo file)
        {
            return file_reader(file,Encoding.UTF8);
        }

        public static string file_readtext_gbk(FileInfo file)
        {
            return Encoding.GetEncoding("gbk").GetString(file_reader(file));
        }

        public static string file_readtext(string file)
        {
            return Encoding.UTF8.GetString(file_reader(file));
        }

        public static string file_readtext(FileInfo file, Encoding encoding)
        {
            return encoding.GetString(file_reader(file));
        }

        public static string file_readtext(string file, Encoding encoding)
        {
            return encoding.GetString(file_reader(file));
        }

        public static bool file_write(FileInfo file, string content)
        {
            byte[] Bin = Encoding.UTF8.GetBytes(content);
            return file_write(file, Bin);
        }

        public static bool file_write(FileInfo file, byte[] Bin)
        {
            using (FileStream fs = new FileStream(file.FullName, FileMode.Create, FileAccess.Write))
            {
                fs.Write(Bin, 0, Bin.Length);
            }
            return true;
        }

        public static byte[] stream2byte(Stream stream)
        {
            byte[] CS10000;
            long originalPosition = 0;
            if (stream.CanSeek)
            {
                originalPosition = stream.Position;
                stream.Position = 0;
            }
            try
            {
                int bytesRead;
                byte[] readBuffer = new byte[0x1000];
                int totalBytesRead = 0;
                while ((bytesRead = stream.Read(readBuffer, totalBytesRead, readBuffer.Length - totalBytesRead)) > 0)
                {
                    totalBytesRead += bytesRead;
                    if (totalBytesRead == readBuffer.Length)
                    {
                        int nextByte = stream.ReadByte();
                        if (nextByte != -1)
                        {
                            byte[] temp = new byte[readBuffer.Length * 2];
                            Buffer.BlockCopy(readBuffer, 0, temp, 0, readBuffer.Length);
                            Buffer.SetByte(temp, totalBytesRead, (byte) nextByte);
                            readBuffer = temp;
                            totalBytesRead++;
                        }
                    }
                }
                byte[] buffer = readBuffer;
                if (readBuffer.Length != totalBytesRead)
                {
                    buffer = new byte[totalBytesRead];
                    Buffer.BlockCopy(readBuffer, 0, buffer, 0, totalBytesRead);
                }
                CS10000 = buffer;
            }
            finally
            {
                if (stream.CanSeek)
                {
                    stream.Position = originalPosition;
                }
            }
            return CS10000;
        }

        public static DataSet xml_reader(FileInfo file)
        {
            DataSet ds = new DataSet();
            using (MemoryStream ms = new MemoryStream(file_reader(file.FullName)))
            {
                ds.ReadXml(ms);
                ms.Close();
                ms.Dispose();
            }
            return ds;
        }

        public static DataSet xml_reader(string file)
        {
            return xml_reader(new FileInfo(file));
        }

        /// <summary>
        /// 返回指示文件是否已被其它程序使用的布尔值
        /// </summary>
        /// <param name="fileFullName">文件的完全限定名，例如：“C:\MyFile.txt”。</param>
        /// <returns>如果文件已被其它程序使用，则为 true；否则为 false。</returns>
        public static Boolean FileIsUsed(String fileFullName)
        {
            Boolean result = false;

            //判断文件是否存在，如果不存在，直接返回 false
            if (!System.IO.File.Exists(fileFullName))
            {
               // result = false;
                throw new Exception("文件不存在！");
            }//end: 如果文件不存在的处理逻辑
            else
            {//如果文件存在，则继续判断文件是否已被其它程序使用

                //逻辑：尝试执行打开文件的操作，如果文件已经被其它程序使用，则打开失败，抛出异常，根据此类异常可以判断文件是否已被其它程序使用。
                System.IO.FileStream fileStream = null;
                try
                {
                    fileStream = System.IO.File.Open(fileFullName, System.IO.FileMode.Open, System.IO.FileAccess.ReadWrite, System.IO.FileShare.None);

                    result = false;
                }
                catch (System.IO.IOException ioEx)
                {
                    result = true;
                }
                catch (System.Exception ex)
                {
                    result = true;
                }
                finally
                {
                    if (fileStream != null)
                    {
                        fileStream.Close();
                    }
                }

            }//end: 如果文件存在的处理逻辑

            //返回指示文件是否已被其它程序使用的值
            return result;

        }//end method FileIsUsed


        // 首先引用API 函数  

        [DllImport("kernel32.dll")]
        private static extern IntPtr _lopen(string lpPathName, int iReadWrite);
        [DllImport("kernel32.dll")]
        private static extern bool CloseHandle(IntPtr hObject);
        private const int OF_READWRITE = 2;
        private const int OF_SHARE_DENY_NONE = 0x40;
        private static readonly IntPtr HFILE_ERROR = new IntPtr(-1);


        /// <summary>   
        /// 检查文件是否已经打开   
        /// </summary>   
        /// <param name="strfilepath">要检查的文件路径</param>          
        /// <returns>-1文件不存在,1文件已经打开,0文件可用未打开</returns>   
        public static int VerifyFileIsOpen(string strfilepath)
        {
            string vFileName = strfilepath;

            // 先检查文件是否存在,如果不存在那就不检查了   
            if (!File.Exists(vFileName))
            {
                return -1;
            }

            // 打开指定文件看看情况   
            IntPtr vHandle = _lopen(vFileName, OF_READWRITE | OF_SHARE_DENY_NONE);
            if (vHandle == HFILE_ERROR)
            { // 文件已经被打开                   
                return 1;
            }
            CloseHandle(vHandle);

            // 说明文件没被打开，并且可用  

            return 0;
        }



        public static DataSet Xml2DataSet(string xml,Encoding enc) {
            DataSet ds = new DataSet();
            using (MemoryStream mStream = new MemoryStream(enc.GetBytes(xml)))
            {
                ds.ReadXml(mStream);
                mStream.Close();
            }

            return ds;
        }
    }
}

