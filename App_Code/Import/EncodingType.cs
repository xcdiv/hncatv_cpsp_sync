using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace RoleDomain.Common
{
    /// <summary> 
    /// 获取文件的编码格式 
    /// </summary> 
    public class EncodingType
    {
        // 根据网页的HTML内容提取网页的Encoding  
        public static Encoding GetEncoding(string html)
        {
            string pattern = @"(?i)\bcharset=(?<charset>[\S]+)";
            string charset = Regex.Match(html, pattern).Groups["charset"].Value;
            string _charset = "utf-8";

            if (charset != null && charset.Trim().Length > 0)
            {
                _charset = charset.Replace("\"", "").Replace("'", "").Replace("=", "").Replace("/", "").Replace("<", "").Replace(">", "");

            }


            try
            {
                return Encoding.GetEncoding(_charset);
            }
            catch (ArgumentException)
            {
                return null;
            }
        }
        /// <summary> 
        /// 给定文件的路径，读取文件的二进制数据，判断文件的编码类型 
        /// </summary> 
        /// <param name=“FILE_NAME“>文件路径</param> 
        /// <returns>文件的编码类型</returns> 
        public static System.Text.Encoding GetType(string FILE_NAME)
        {
            Encoding r;

            using (FileStream fs = new FileStream(FILE_NAME, FileMode.Open, FileAccess.Read))
            {
                r = GetType(fs);

                fs.Close();
            }
            return r;
        }

        public static System.Text.Encoding GetTypeByBytes(byte[] fs)
        {
            byte[] Unicode = new byte[] { 0xFF, 0xFE, 0x41 };
            byte[] UnicodeBIG = new byte[] { 0xFE, 0xFF, 0x00 };
            byte[] UTF8 = new byte[] { 0xEF, 0xBB, 0xBF }; //带BOM 
            Encoding reVal = Encoding.Default;

            using (BinaryReader r = new BinaryReader(new MemoryStream(fs), System.Text.Encoding.Default))
            {
                int i;
                int.TryParse(fs.Length.ToString(), out i);
                byte[] ss = r.ReadBytes(i);
                if (IsUTF8Bytes(ss) || (ss[0] == 0xEF && ss[1] == 0xBB && ss[2] == 0xBF))
                {
                    reVal = Encoding.UTF8;
                }
                else if (ss[0] == 0xFE && ss[1] == 0xFF && ss[2] == 0x00)
                {
                    reVal = Encoding.BigEndianUnicode;
                }
                else if (ss[0] == 0xFF && ss[1] == 0xFE && ss[2] == 0x41)
                {
                    reVal = Encoding.Unicode;
                }
                r.Close();
            }
            return reVal;

        }

        /// <summary> 
        /// 通过给定的文件流，判断文件的编码类型 
        /// </summary> 
        /// <param name=“fs“>文件流</param> 
        /// <returns>文件的编码类型</returns> 
        public static System.Text.Encoding GetType(FileStream fs)
        {
            byte[] Unicode = new byte[] { 0xFF, 0xFE, 0x41 };
            byte[] UnicodeBIG = new byte[] { 0xFE, 0xFF, 0x00 };
            byte[] UTF8 = new byte[] { 0xEF, 0xBB, 0xBF }; //带BOM 
            Encoding reVal = Encoding.Default;

            using (BinaryReader r = new BinaryReader(fs, System.Text.Encoding.Default))
            {
                int i;
                int.TryParse(fs.Length.ToString(), out i);
                byte[] ss = r.ReadBytes(i);
                if (IsUTF8Bytes(ss) || (ss[0] == 0xEF && ss[1] == 0xBB && ss[2] == 0xBF))
                {
                    reVal = Encoding.UTF8;
                }
                else if (ss[0] == 0xFE && ss[1] == 0xFF && ss[2] == 0x00)
                {
                    reVal = Encoding.BigEndianUnicode;
                }
                else if (ss[0] == 0xFF && ss[1] == 0xFE && ss[2] == 0x41)
                {
                    reVal = Encoding.Unicode;
                }
                r.Close();
            }
            return reVal;

        }

        /// <summary> 
        /// 判断是否是不带 BOM 的 UTF8 格式 
        /// </summary> 
        /// <param name=“data“></param> 
        /// <returns></returns> 
        private static bool IsUTF8Bytes(byte[] data)
        {
            int charByteCounter = 1; //计算当前正分析的字符应还有的字节数 
            byte curByte; //当前分析的字节. 
            for (int i = 0; i < data.Length; i++)
            {
                curByte = data[i];
                if (charByteCounter == 1)
                {
                    if (curByte >= 0x80)
                    {
                        //判断当前 
                        while (((curByte <<= 1) & 0x80) != 0)
                        {
                            charByteCounter++;
                        }
                        //标记位首位若为非0 则至少以2个1开始 如:110XXXXX...........1111110X 
                        if (charByteCounter == 1 || charByteCounter > 6)
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    //若是UTF-8 此时第一位必须为1 
                    if ((curByte & 0xC0) != 0x80)
                    {
                        return false;
                    }
                    charByteCounter--;
                }
            }
            if (charByteCounter > 1)
            {
                throw new Exception("非预期的byte格式");
            }
            return true;
        }



        public static string GetText(byte[] buff)
        {
            string strReslut = string.Empty;
            if (buff.Length > 3)
            {
                if (buff[0] == 239 && buff[1] == 187 && buff[2] == 191)
                {// utf-8
                    strReslut = Encoding.UTF8.GetString(buff);
                }
                else if (buff[0] == 254 && buff[1] == 255)
                {// big endian unicode
                    strReslut = Encoding.BigEndianUnicode.GetString(buff);
                }
                else if (buff[0] == 255 && buff[1] == 254)
                {// unicode
                    strReslut = Encoding.Unicode.GetString(buff);
                }
                else if (IsUTF8Bytes(buff) || (buff[0] == 0xEF && buff[1] == 0xBB && buff[2] == 0xBF))
                {
                    strReslut = Encoding.UTF8.GetString(buff);
                }
                else if (buff[0] == 0xFE && buff[1] == 0xFF && buff[2] == 0x00)
                {
                    strReslut = Encoding.Unicode.GetString(buff);
                }
                else if (buff[0] == 0xFF && buff[1] == 0xFE && buff[2] == 0x41)
                {
                    strReslut = Encoding.Unicode.GetString(buff);
                }
                else if (isUtf8(buff))
                {// utf-8
                    strReslut = Encoding.UTF8.GetString(buff);
                }
                else
                {// ansi
                    strReslut = Encoding.Default.GetString(buff);
                }
            }
            return strReslut;
        }
        // 110XXXXX, 10XXXXXX
        // 1110XXXX, 10XXXXXX, 10XXXXXX
        // 11110XXX, 10XXXXXX, 10XXXXXX, 10XXXXXX
        private static bool isUtf8(byte[] buff)
        {
            for (int i = 0; i < buff.Length; i++)
            {
                if ((buff[i] & 0xE0) == 0xC0) // 110x xxxx 10xx xxxx
                {
                    if ((buff[i + 1] & 0x80) != 0x80)
                    {
                        return false;
                    }
                }
                else if ((buff[i] & 0xF0) == 0xE0) // 1110 xxxx 10xx xxxx 10xx xxxx
                {
                    if ((buff[i + 1] & 0x80) != 0x80 || (buff[i + 2] & 0x80) != 0x80)
                    {
                        return false;
                    }
                }
                else if ((buff[i] & 0xF8) == 0xF0) // 1111 0xxx 10xx xxxx 10xx xxxx 10xx xxxx
                {
                    if ((buff[i + 1] & 0x80) != 0x80 || (buff[i + 2] & 0x80) != 0x80 || (buff[i + 3] & 0x80) != 0x80)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        // news.sohu.com
        private static bool isGBK(byte[] buff)
        {
            return false;
        }
    }
}
