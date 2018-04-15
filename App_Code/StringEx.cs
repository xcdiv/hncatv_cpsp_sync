namespace System
{
    using Microsoft.JScript;
    using RoleDomain.Common;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Linq;

    public static class StringEx
    {
        /// <summary>
        /// GB2312转换成UTF8
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string gb2312_utf8(this string text)
        {
            //声明字符集   
            System.Text.Encoding utf8, gb2312;
            //gb2312   
            gb2312 = System.Text.Encoding.GetEncoding("gb2312");
            //utf8   
            utf8 = System.Text.Encoding.GetEncoding("utf-8");
            byte[] gb;
            gb = gb2312.GetBytes(text);
            gb = System.Text.Encoding.Convert(gb2312, utf8, gb);
            //返回转换后的字符   
            return utf8.GetString(gb);
        }

        /// <summary>
        /// UTF8转换成GB2312
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string utf8_gb2312(this string text)
        {
            //声明字符集   
            System.Text.Encoding utf8, gb2312;
            //utf8   
            utf8 = System.Text.Encoding.GetEncoding("utf-8");
            //gb2312   
            gb2312 = System.Text.Encoding.GetEncoding("gb2312");
            byte[] utf;
            utf = utf8.GetBytes(text);
            utf = System.Text.Encoding.Convert(utf8, gb2312, utf);
            //返回转换后的字符   
            return gb2312.GetString(utf);
        }


        public static string Regex_clearHTML(this string html)
        {

            return Regex.Replace(html, @"<[^{><}]*>", "");
        }
        public static string Regex_clearSCRIPT(this string html)
        {
            string regexstr = @"<script[^>]*?>.*?</script>";
            html = Regex.Replace(html, regexstr, "", RegexOptions.IgnoreCase);
            html = Regex.Replace(html, @"<(\/?script.*?)>", "");

            return html;
        }
        public static string Regex_clearSTYLE(this string html)
        {
            string regexstr = @"<style[^>]*?>.*?</style>";
            html = Regex.Replace(html, regexstr, "", RegexOptions.IgnoreCase);
            html = Regex.Replace(html, @"<(\/?link.*?)>", "");

            return html;
        }
        public static string Regex_clearA(this string html)
        {
            return Regex.Replace(html, @"(?is)<a[^>]*?>.*?</a>", "", RegexOptions.IgnoreCase);
        }

        public static string Regex_clearObject(this string html)
        {
            string regexstr = @"(?i)<Object([^>])*>(/w|/W)*</Object([^>])*>";
            return Regex.Replace(html, regexstr, string.Empty);
        }

        public static string Regex_clearIframe(this string html)
        {
            string regexstr = @"(?i)<iframe([^>])*>(/w|/W)*</iframe([^>])*>";
            return Regex.Replace(html, regexstr, string.Empty);
        }

        public static string Regex_clearFrameset(this string html)
        {
            string regexstr = @"(?i)<frameset([^>])*>(/w|/W)*</frameset([^>])*>";
            return Regex.Replace(html, regexstr, string.Empty);
        }

        public static string clear_json_value(this string text)
        {
            return text.Replace(@"\", @"\\").Replace("\r\n", "").Replace("\r", "").Replace("\n", "").Replace("\t", " ");
        }

        public static string clearEditer(this string html)
        {
            html = html.Regex_clearSCRIPT();
            html = html.Regex_clearSTYLE();
            html = html.Regex_clearA();
            html = html.Regex_clearObject();
            html = html.Regex_clearIframe();
            html = html.Regex_clearFrameset();

            return html;
        }

      
        public static int float2int(this string value)
        {
            float f = float.Parse(value);

            return int.Parse(string.Format("{0:0}", f));
        }

       
        public static string substr(this string value, int length)
        {
            if ((length > 0) && (value.Length > length))
            {
                return value.Substring(0, length);
            }
            return value;
        }

        public static string substrfilling(this string value, int length)
        {
            string str = value;
            if ((length > 0) && (str.Length > length))
            {
                str = str.Substring(0, length);
            }

            if (str.Length < length) { 
                str = str.PadLeft(length, '0'); 
            } 

            return str;
        }


        public static string toEscape(this string text)
        {
            return GlobalObject.escape(text);
        }

        public static string toUnEscape(this string text)
        {
            return GlobalObject.unescape(text);
        }
        public static long GenerateIntID()
        {

            byte[] buffer = Guid.NewGuid().ToByteArray();

            return BitConverter.ToInt64(buffer, 0);

        }
        public static string GenerateStringID()
        {

            long i = 1;

            foreach (byte b in Guid.NewGuid().ToByteArray())
            {

                i *= ((int)b + 1);

            }

            return string.Format("{0:x}", i - DateTime.Now.Ticks);

        }
        /// <summary>
        /// 字符串翻转
        /// </summary>
        /// <param name="strTest"></param>
        /// <returns></returns>
        public static string DoStrRev(this string strTest)
        {
            if (strTest.Length == 1)
                return strTest;
            string strResult = strTest.Substring(strTest.Length - 1, 1);
            strResult += DoStrRev(strTest.Substring(0, strTest.Length - 1));
            return strResult;
        }
        //public static long CheckSumInt64(this string text)
        //{
        //    return CheckSum.CalculateChecksumINT(text);
        //}
        /// 转全角的函数(SBC case)
        ///
        ///任意字符串
        ///全角字符串
        ///
        ///全角空格为12288，半角空格为32
        ///其他字符半角(33-126)与全角(65281-65374)的对应关系是：均相差65248
        ///
        public static String ToSBC(this string input)
        {
            // 半角转全角：
            char[] c = input.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                if (c[i] == 32)
                {
                    c[i] = (char)12288;
                    continue;
                }
                if (c[i] < 127)
                    c[i] = (char)(c[i] + 65248);
            }
            return new String(c);
        }

        /**/
        // /
        // / 转半角的函数(DBC case)
        // /
        // /任意字符串
        // /半角字符串
        // /
        // /全角空格为12288，半角空格为32
        // /其他字符半角(33-126)与全角(65281-65374)的对应关系是：均相差65248
        // /
        public static String ToDBC(this string input)
        {
            char[] c = input.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                if (c[i] == 12288)
                {
                    c[i] = (char)32;
                    continue;
                }
                if (c[i] > 65280 && c[i] < 65375)
                    c[i] = (char)(c[i] - 65248);
            }
            return new String(c);
        }




        /// <summary>
        /// 判断是不是utf-8
        /// </summary>
        /// <param name="rawtext"></param>
        /// <returns>>80 is utf-8</returns>
        public static int utf8_probability(this byte[] rawtext)
        {
            int score = 0;
            int i, rawtextlen = 0;
            int goodbytes = 0, asciibytes = 0;

            // Maybe also use UTF8 Byte Order Mark:  EF BB BF

            // Check to see if characters fit into acceptable ranges
            rawtextlen = rawtext.Length;
            for (i = 0; i < rawtextlen; i++)
            {
                if ((rawtext[i] & (byte)0x7F) == rawtext[i])
                {  // One byte
                    asciibytes++;
                    // Ignore ASCII, can throw off count
                }
                else
                {
                    int m_rawInt0 = Convert.ToInt16(rawtext[i]);
                    int m_rawInt1 = Convert.ToInt16(rawtext[i + 1]);
                    int m_rawInt2 = Convert.ToInt16(rawtext[i + 2]);

                    if (256 - 64 <= m_rawInt0 && m_rawInt0 <= 256 - 33 && // Two bytes
                     i + 1 < rawtextlen &&
                     256 - 128 <= m_rawInt1 && m_rawInt1 <= 256 - 65)
                    {
                        goodbytes += 2;
                        i++;
                    }
                    else if (256 - 32 <= m_rawInt0 && m_rawInt0 <= 256 - 17 && // Three bytes
                     i + 2 < rawtextlen &&
                     256 - 128 <= m_rawInt1 && m_rawInt1 <= 256 - 65 &&
                     256 - 128 <= m_rawInt2 && m_rawInt2 <= 256 - 65)
                    {
                        goodbytes += 3;
                        i += 2;
                    }
                }
            }

            if (asciibytes == rawtextlen) { return 0; }

            score = (int)(100 * ((float)goodbytes / (float)(rawtextlen - asciibytes)));

            // If not above 98, reduce to zero to prevent coincidental matches
            // Allows for some (few) bad formed sequences
            if (score > 98)
            {
                return score;
            }
            else if (score > 95 && goodbytes > 30)
            {
                return score;
            }
            else
            {
                return 0;
            }

        }
         

        /// <summary>
        /// Return unique Int64 value for input string
        /// </summary>
        /// <param name="strText"></param>
        /// <returns></returns>
        public static Int64 GetInt64HashCode(this string strText)
        {
            Int64 hashCode = 0;
            if (!string.IsNullOrEmpty(strText))
            {
                //Unicode Encode Covering all characterset
                byte[] byteContents = Encoding.Unicode.GetBytes(strText);
                System.Security.Cryptography.SHA256 hash =
                new System.Security.Cryptography.SHA256CryptoServiceProvider();
                byte[] hashText = hash.ComputeHash(byteContents);
                //32Byte hashText separate
                //hashCodeStart = 0~7  8Byte
                //hashCodeMedium = 8~23  8Byte
                //hashCodeEnd = 24~31  8Byte
                //and Fold
                Int64 hashCodeStart = BitConverter.ToInt64(hashText, 0);
                Int64 hashCodeMedium = BitConverter.ToInt64(hashText, 8);
                Int64 hashCodeEnd = BitConverter.ToInt64(hashText, 24);
                hashCode = hashCodeStart ^ hashCodeMedium ^ hashCodeEnd;
            }
            return (hashCode);
        }

        public static Int64 GetInt64HashCodeMain(this string strText)
        {
            Int64 hashCode = 0;
            if (!string.IsNullOrEmpty(strText))
            {
                //Unicode Encode Covering all characterset
                byte[] byteContents = Encoding.Unicode.GetBytes(strText);
                System.Security.Cryptography.SHA1 hash =
                new System.Security.Cryptography.SHA1CryptoServiceProvider();
                byte[] hashText = hash.ComputeHash(byteContents);
                //32Byte hashText separate
                //hashCodeStart = 0~7  8Byte
                //hashCodeMedium = 8~23  8Byte
                //hashCodeEnd = 24~31  8Byte
                //and Fold
                Int64 hashCodeStart = BitConverter.ToInt64(hashText, 0);
                Int64 hashCodeMedium = BitConverter.ToInt64(hashText, 8);
                Int64 hashCodeEnd = BitConverter.ToInt64(hashText, 12);
                hashCode = hashCodeStart ^ hashCodeMedium ^ hashCodeEnd;
            }
            return (hashCode);
        }



        public static Int64 GetInt64HashCodeFast3(this string strText)
        {
            Int64 hashCode = 0;

            var s1 = strText.Substring(0, strText.Length / 2);
            var s2 = strText.Substring(strText.Length / 2);

            var x = ((long)s1.GetHashCode()) << 0x20 | s2.GetHashCode();

            return x;
        }
        
        public static Int64 GetInt64HashCodeFast2(this string strText)
        {
            Int64 hashCode = 0;
            if (!string.IsNullOrEmpty(strText))
            {
                //Unicode Encode Covering all characterset
                byte[] byteContents = Encoding.Unicode.GetBytes(strText);
                System.Security.Cryptography.SHA1Cng hash =
                new System.Security.Cryptography.SHA1Cng();
                byte[] hashText = hash.ComputeHash(byteContents);
                //32Byte hashText separate
                //hashCodeStart = 0~7  8Byte
                //hashCodeMedium = 8~23  8Byte
                //hashCodeEnd = 24~31  8Byte
                //and Fold
                Int64 hashCodeStart = BitConverter.ToInt64(hashText, 0);
                Int64 hashCodeMedium = BitConverter.ToInt64(hashText, 5);
                Int64 hashCodeEnd = BitConverter.ToInt64(hashText, 8);
                hashCode = hashCodeStart ^ hashCodeMedium ^ hashCodeEnd;
            }
            return (hashCode);
        }


        public static Int64 GetInt64HashCodeFast(this string strText)
        {
            Int64 hashCode = 0;
            if (!string.IsNullOrEmpty(strText))
            {
                //Unicode Encode Covering all characterset
                byte[] byteContents = Encoding.Unicode.GetBytes(strText);
                System.Security.Cryptography.MD5 hash =
                new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] hashText = hash.ComputeHash(byteContents);
                //32Byte hashText separate
                //hashCodeStart = 0~7  8Byte
                //hashCodeMedium = 8~23  8Byte
                //hashCodeEnd = 24~31  8Byte
                //and Fold
                Int64 hashCodeStart = BitConverter.ToInt64(hashText, 0);
                Int64 hashCodeMedium = BitConverter.ToInt64(hashText, 5);
                Int64 hashCodeEnd = BitConverter.ToInt64(hashText, 8);
                hashCode = hashCodeStart ^ hashCodeMedium ^ hashCodeEnd;
            }
            return (hashCode);
        }

        /// <summary>
        /// 字符串转List<int> ::>   List<int> intList = str.ToList<int>(',', s => int.Parse(s));
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="str"></param>
        /// <param name="split"></param>
        /// <param name="convertHandler"></param>
        /// <returns></returns>
        public static List<T> ToList<T>(this string str, char split, Converter<string, T> convertHandler)
        {
            if (string.IsNullOrEmpty(str))
            {
                return new List<T>();
            }
            else
            {
                string[] arr = str.Split(split);
                T[] Tarr = Array.ConvertAll(arr, convertHandler);
                return new List<T>(Tarr);
            }
        }

        /// <summary>
        /// 验证email
        /// </summary>
        /// <param name="emailString"></param>
        /// <returns></returns>
        public static bool valid_email(this string emailString)
        {

            string emailReg = @"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*";
            return Regex.IsMatch(emailString, emailReg);

        }

 
 


















         
    }
}

