namespace RoleDomain.Common
{
    using System;
    using System.Collections;
    using System.Configuration;
    using System.Data;
    using System.IO;
    using System.Text;
    using System.Web;
    using System.Web.UI;
    using System.Web.UI.HtmlControls;

    public class ToJson : Page
    {
        public static string checkEmpty(string str)
        {
            return ((str.ToString() != "") ? str : "0");
        }

        public static string checkEmpty(string str, string emptyText)
        {
            return ((str.ToString() != "") ? str : emptyText);
        }

        public static string checkEmptyPic(string str)
        {
            return ((str.ToString() != "") ? str : "/employee/photo/tupian.bmp");
        }

        public static string CheckPath(string picPath)
        {
            string fileName = ConfigurationManager.AppSettings["PIC"].ToString() + picPath;
            FileInfo picSrc = new FileInfo(picPath);
            if (!picSrc.Exists)
            {
                fileName = ConfigurationManager.AppSettings["PIC"].ToString() + "/employee/photo/tupian.bmp";
            }
            return fileName;
        }

        public static string checkPic(string pic)
        {
            return ((pic != "") ? pic : "/employee/photo/tupian.bmp");
        }

        public static string Date_ToShort(string str)
        {
            return ((str != "") ? Convert.ToDateTime(str).ToShortDateString() : str);
        }

        public static string DateDiff(DateTime DateTime1, DateTime DateTime2)
        {
            string dateDiff = null;
            try
            {
                TimeSpan ts1 = new TimeSpan(DateTime1.Ticks);
                TimeSpan ts2 = new TimeSpan(DateTime2.Ticks);
                dateDiff = ts1.Subtract(ts2).Duration().Days.ToString();
            }
            catch
            {
            }
            return dateDiff;
        }

        public static string EmpTypeStr()
        {
            return "<span style=\"text-align:center; width:100%\" ><img src=\"/resources/images/report/bb_cf_03.jpg\" id=\"empty\" alt=\"无数据\" /></span>";
        }

        private static ArrayList GetDistinctData(ref DataView dv, ref ArrayList al, string column_name)
        {
            bool existed = false;
            foreach (DataRowView dr in dv)
            {
                existed = false;
                for (int i = 0; i < al.Count; i++)
                {
                    if (dr[column_name].ToString() == al[i].ToString())
                    {
                        existed = true;
                    }
                }
                if (!existed)
                {
                    al.Add(dr[column_name]);
                }
            }
            return al;
        }

        public static string GetFormParam(string name, string defaultValue)
        {
            string val = HttpContext.Current.Request.Form[name] ?? defaultValue;
            if (!(val == defaultValue))
            {
                return val;
            }
            return (HttpContext.Current.Request.QueryString[name] ?? defaultValue);
        }

        public static string GetFormParamReplce(string name, string defaultValue)
        {
            return (name.ToString().Replace("\"", "") ?? defaultValue);
        }

        public static string GetFormParamUrl(string name, string defaultValue)
        {
            string val = HttpContext.Current.Request.Form[name] ?? defaultValue;
            if (val == defaultValue)
            {
                val = HttpContext.Current.Request.QueryString[name] ?? defaultValue;
            }
            return HttpContext.Current.Server.UrlDecode(val);
        }

        public static string GetStrDecoding(string str, string codingType)
        {
            return HttpUtility.UrlDecode(str, Encoding.GetEncoding(codingType));
        }

        public static string GetStrEncoding(string str, string codingType)
        {
            return HttpUtility.UrlEncode(str, Encoding.GetEncoding(codingType));
        }

        public static string GetSubString(string mText, int startIndex)
        {
            return GetSubString(mText, startIndex, (Encoding.Default.GetByteCount(mText) - startIndex) + 1, "");
        }

        public static string GetSubString(string mText, int startIndex, int byteCount, string RepliceStr)
        {
            if (byteCount < 1)
            {
                return string.Empty;
            }
            if (Encoding.Default.GetByteCount(mText) <= byteCount)
            {
                return mText;
            }
            if (startIndex == 0)
            {
                byte[] txtBytes = Encoding.Default.GetBytes(mText);
                byte[] newBytes = new byte[byteCount];
                for (int i = 0; i < byteCount; i++)
                {
                    newBytes[i] = txtBytes[i];
                }
                return Encoding.Default.GetString(newBytes);
            }
            string tmp = GetSubString(mText, 0, startIndex - 1, RepliceStr);
            mText = mText.Substring(tmp.Length) + RepliceStr;
            return GetSubString(mText, 0, byteCount, RepliceStr);
        }

        public static string getValue(string str)
        {
            return ((str.Length > 0) ? str.ToString().Remove(str.Length - 1, 1) : "");
        }

        private static string HaveChild(DataTable dt, DataRowView drv)
        {
            return ((dt.Select("[parentID] = '" + drv[0].ToString() + "'").Length > 0) ? "false" : "true");
        }

        public static string Ifram(string Urlpar)
        {
            return ("<iframe  frameborder='no' border='0' marginwidth='0' marginheight='0' ; src=" + Urlpar + " width=100% height=100%></iframe>");
        }

        public static string jsonString_Paging_ToJson(DataTable dt, string cellArray, string TableCountName, string TableName)
        {
            StringBuilder jsonString = new StringBuilder();
            if ((dt != null) && (dt.Rows.Count > 0))
            {
                jsonString.Append(string.Concat(new object[] { "{\"", TableCountName, "\":\"", dt.Rows.Count, "\",\"", TableName, "\":[" }));
                foreach (DataRow drv in dt.Rows)
                {
                    jsonString.Append("{");
                    if (cellArray.Length > 0)
                    {
                        string[] VA = cellArray.Split(new char[] { ',' });
                        if (VA.Length > 0)
                        {
                            for (int i = 0; i < VA.Length; i++)
                            {
                                jsonString.Append("\"" + VA[i].ToString().Trim() + "\":");
                                jsonString.Append("\"" + drv[i].ToString() + "\",");
                            }
                        }
                    }
                    jsonString.Append("},");
                }
                jsonString.ToString().Remove(jsonString.ToString().LastIndexOf(","));
                jsonString.Append("]}");
                return jsonString.ToString().Replace(",}", "}").Replace(",]", "]");
            }
            return "数据集为空";
        }

        public static string jsonString_Paging_ToXML(DataTable dt)
        {
            string RCount = "0";
            DataSet ds = new DataSet();
            if (dt.Rows.Count > 0)
            {
                ds.Tables.Add(dt);
                RCount = dt.Rows[0]["RecordCount"].ToString();
            }
            StringBuilder jsonString = new StringBuilder();
            jsonString.Append("<Items>\r\n");
            jsonString.Append("<TotalResults>" + RCount + "</TotalResults>\r\n");
            jsonString.Append(ds.GetXml());
            jsonString.Append("\r\n</Items>");
            return jsonString.ToString();
        }

        public static string jsonString_Table_ToXML(DataTable dt)
        {
            DataSet ds = new DataSet();
            ds.Tables.Add(dt);
            StringBuilder jsonString = new StringBuilder();
            jsonString.Append("<Items>\r\n");
            jsonString.Append("<TotalResults>0</TotalResults>\r\n");
            jsonString.Append(ds.GetXml());
            jsonString.Append("\r\n</Items>");
            return jsonString.ToString();
        }

        public static string jsonString_ToTree(string ID, DataTable dt, string cellArray)
        {
            StringBuilder jsonString = new StringBuilder();
            jsonString.Append("[");
            if ((dt != null) && (dt.Rows.Count > 0))
            {
                DataView dv = dt.Copy().DefaultView;
                dv.RowFilter = "[parentID] ='" + ID + "'";
                foreach (DataRowView drv in dv)
                {
                    jsonString.Append("\n{");
                    if (cellArray.Length > 0)
                    {
                        string[] VA = cellArray.Split(new char[] { ',' });
                        if (VA.Length > 0)
                        {
                            for (int i = 0; i < VA.Length; i++)
                            {
                                jsonString.Append(VA[i].ToString().Trim() + ":");
                                jsonString.Append("'" + drv[i].ToString().Trim() + "',");
                            }
                        }
                    }
                    jsonString.Append("leaf:" + HaveChild(dt, drv) + "},");
                }
                jsonString.Append("]");
                return jsonString.ToString().Replace(",}", "}").Replace(",]", "]");
            }
            return "数据集为空";
        }

        public static string jsonString_Tree(DataTable dt, string cellArray)
        {
            StringBuilder jsonString = new StringBuilder();
            jsonString.Append("[");
            if ((dt != null) && (dt.Rows.Count > 0))
            {
                foreach (DataRow drv in dt.Rows)
                {
                    jsonString.Append("{");
                    if (cellArray.Length > 0)
                    {
                        string[] VA = cellArray.Split(new char[] { ',' });
                        if (VA.Length > 0)
                        {
                            for (int i = 0; i < VA.Length; i++)
                            {
                                jsonString.Append(VA[i].ToString() + ":");
                                jsonString.Append("'" + drv[i].ToString() + "',");
                            }
                        }
                    }
                    jsonString.Append("leaf:true},");
                }
                jsonString.ToString().Remove(jsonString.ToString().LastIndexOf(","));
                jsonString.Append("]");
                return jsonString.ToString().Replace(",}", "}").Replace(",]", "]");
            }
            return "数据集为空";
        }

        public static string jsonString_Txt(DataTable dt, string cellArray)
        {
            StringBuilder jsonString = new StringBuilder();
            jsonString.Append("[");
            if ((dt != null) && (dt.Rows.Count > 0))
            {
                foreach (DataRow drv in dt.Rows)
                {
                    jsonString.Append("{");
                    if (cellArray.Length > 0)
                    {
                        string[] VA = cellArray.Split(new char[] { ',' });
                        if (VA.Length > 0)
                        {
                            for (int i = 0; i < VA.Length; i++)
                            {
                                jsonString.Append(VA[i].ToString().Trim() + ":");
                                jsonString.Append("'" + drv[i].ToString().Trim() + "',");
                            }
                        }
                    }
                    jsonString.Append("},");
                }
                jsonString.ToString().Remove(jsonString.ToString().LastIndexOf(","));
                jsonString.Append("]");
                return jsonString.ToString().Replace(",}", "}").Replace(",]", "]");
            }
            return "数据集为空";
        }

        public static string jsonString_Txt2(DataTable dt, string cellArray)
        {
            StringBuilder jsonString = new StringBuilder();
            jsonString.Append("[");
            if ((dt != null) && (dt.Rows.Count > 0))
            {
                foreach (DataRow drv in dt.Rows)
                {
                    jsonString.Append("\n[");
                    if (cellArray.Length > 0)
                    {
                        string[] VA = cellArray.Split(new char[] { ',' });
                        if (VA.Length > 0)
                        {
                            for (int i = 0; i < VA.Length; i++)
                            {
                                jsonString.Append(VA[i].ToString().Trim() + ":");
                                jsonString.Append("'" + drv[i].ToString().Trim() + "',");
                            }
                        }
                    }
                    jsonString.Append("],");
                }
                jsonString.ToString().Remove(jsonString.ToString().LastIndexOf(","));
                jsonString.Append("]");
                return jsonString.ToString().Replace(",}", "}").Replace(",]", "]");
            }
            return "数据集为空";
        }

        public static string jsonString_Txt3(DataTable dt, string cellArray)
        {
            string tt = "0";
            StringBuilder jsonString = new StringBuilder();
            jsonString.Append("{");
            if ((dt != null) && (dt.Rows.Count > 0))
            {
                tt = dt.Rows[0][0].ToString().Trim();
            }
            jsonString.Append("totalCount:" + tt);
            jsonString.Append(", success:true");
            jsonString.Append(", data:[");
            if ((dt != null) && (dt.Rows.Count > 0))
            {
                foreach (DataRow drv in dt.Rows)
                {
                    jsonString.Append("{");
                    if (cellArray.Length > 0)
                    {
                        string[] VA = cellArray.Split(new char[] { ',' });
                        if (VA.Length > 0)
                        {
                            for (int i = 0; i < VA.Length; i++)
                            {
                                jsonString.Append(VA[i].ToString().Trim() + ":");
                                jsonString.Append("'" + drv[i + 1].ToString().Trim() + "',");
                            }
                        }
                    }
                    jsonString.Append("},");
                }
            }
            jsonString.ToString().Remove(jsonString.ToString().LastIndexOf(","));
            jsonString.Append("]");
            return (jsonString.ToString().Replace(",}", "}").Replace(",]", "]") + "}");
        }

        public static string jsonString_Txt4(DataTable dt, string cellArray)
        {
            StringBuilder jsonString = new StringBuilder();
            jsonString.Append("{");
            if ((dt != null) && (dt.Rows.Count > 0))
            {
                jsonString.Append("success:true");
                jsonString.Append(", data:[");
                foreach (DataRow drv in dt.Rows)
                {
                    jsonString.Append("{");
                    if (cellArray.Length > 0)
                    {
                        string[] VA = cellArray.Split(new char[] { ',' });
                        if (VA.Length > 0)
                        {
                            for (int i = 0; i < VA.Length; i++)
                            {
                                jsonString.Append(VA[i].ToString().Trim() + ":");
                                jsonString.Append("'" + drv[i].ToString().Trim() + "',");
                            }
                        }
                    }
                    jsonString.Append("},");
                }
                jsonString.ToString().Remove(jsonString.ToString().LastIndexOf(","));
                jsonString.Append("]");
                return (jsonString.ToString().Replace(",}", "}").Replace(",]", "]") + "}");
            }
            return "数据集为空";
        }

 

        public static string JsonString_XML_tj(DataTable dt, int type, string _x, string[][] _y)
        {
            StringBuilder json = new StringBuilder();
            json.Append("<chart rotateYAxisName='0' baseFont='宋体' baseFontSize='14' bgColor='406181, 6DA5DB' chartBottomMargin='0' bgAlpha='100' baseFontColor='FFFFFF' canvasBgAlpha='0' canvasBorderColor='FFFFFF' divLineColor='FFFFFF' divLineAlpha='100' numVDivlines='10' vDivLineisDashed='1' showAlternateVGridColor='1' lineColor='BBDA00' anchorRadius='4' anchorBgColor='BBDA00' anchorBorderColor='FFFFFF' anchorBorderThickness='2' defaultAnimation='0' showValues='0' toolTipBgColor='406181' toolTipBorderColor='406181' alternateHGridAlpha='5' labelStep='3' formatNumberScale='0' formatNumber='0'>");
            string storeName = _y[type][0].ToString();
            string cellName = _y[type][1].ToString();
            json.Append(JsonString_XML_tjCell(dt, "0", cellName, _x));
            string[] getCellName = storeName.Split(new char[] { '、' });
            string[] getCellNameValue = cellName.Split(new char[] { ',' });
            if (getCellName.Length > 0)
            {
                string CellName = "";
                string CellNameValue = "";
                for (int i = 0; i < getCellName.Length; i++)
                {
                    CellName = getCellName[i].ToString();
                    CellNameValue = getCellNameValue[i].ToString();
                    json.Append(JsonString_XML_tjCell(dt, "1", CellName, CellNameValue));
                }
            }
            json.Append("<styles>");
            json.Append("    <n/><definition>");
            json.Append("        <style name='LineShadow' type='shadow' color='333333' distance='6'/>");
            json.Append("    </definition>");
            json.Append("    <application>");
            json.Append("        <apply toObject='DATAPLOT' styles='LineShadow' />");
            json.Append("    </application>\t");
            json.Append("</styles>");
            json.Append("</chart>");
            return json.ToString();
        }

        public static string JsonString_XML_tjCell(DataTable dt, string type, string stypeName, string _y)
        {
            string CellName = (type == "0") ? "categories" : "dataset ";
            string CellNameStr = ((stypeName != "") && (type != "0")) ? (" seriesname='" + stypeName + "'") : "";
            string CellValue = (type == "0") ? "category label='" : "set value='";
            StringBuilder json = new StringBuilder();
            json.Append("\n<" + CellName + CellNameStr + ">");
            if ((dt != null) && (dt.Rows.Count > 0))
            {
                DataView dv = dt.Copy().DefaultView;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (_y.Length > 0)
                    {
                        string[] VA = _y.Split(new char[] { ',' });
                        if (VA.Length > 0)
                        {
                            json.Append("\n\t\t<" + CellValue);
                            for (int n = 0; n < VA.Length; n++)
                            {
                                json.Append(dv[i][VA[n].ToString() ?? ""].ToString() + "-");
                            }
                            json.Append("' />");
                        }
                        else
                        {
                            json.Append(string.Concat(new object[] { "\n\t\t<", CellValue, dv[i][_y ?? ""], "' />" }));
                        }
                    }
                }
            }
            json.Append("\n</" + CellName + ">");
            return json.ToString().Replace("-' />", "' />");
        }
 
        public string RegValue(int value)
        {
            switch (value)
            {
                case -1:
                    return "存在同名";

                case 1:
                    return "更新成功";
            }
            return "更新成功";
        }

        public static void Return_Josn(string str)
        {
            HttpContext.Current.Response.ContentType = "text/plain";
            HttpContext.Current.Response.ContentEncoding = Encoding.UTF8;
            HttpContext.Current.Response.Write(str);
        }

        public static void Return_JosnMsg(string str)
        {
            HttpContext.Current.Response.ContentType = "text/plain";
            HttpContext.Current.Response.ContentEncoding = Encoding.UTF8;
            string msg = "";
            string bstr = "false";
            switch (str)
            {
                case "-1":
                    bstr = "false";
                    msg = "不存在此信息！请返回";
                    break;

                case "0":
                    bstr = "false";
                    msg = "更新失败！请检查！";
                    break;

                case "10":
                    bstr = "false";
                    msg = "已经审核过！";
                    break;

                case "1":
                    bstr = "true";
                    msg = "更新成功！";
                    break;

                case "2":
                    bstr = "true";
                    msg = "操作成功";
                    break;

                case "4":
                    bstr = "true";
                    msg = "删除成功";
                    break;

                case "40":
                    bstr = "true";
                    msg = "只有本人才能进行“删除”操作";
                    break;
            }
            HttpContext.Current.Response.Write("{success:" + bstr + ",info:'" + msg + "'}");
        }

        public static void Return_JosnMsg(string str, string msg)
        {
            HttpContext.Current.Response.ContentType = "text/plain";
            HttpContext.Current.Response.ContentEncoding = Encoding.UTF8;
            HttpContext.Current.Response.Write("{success:" + str + ",info:'" + msg + "'}");
        }

        public static void ReturnXML(string str)
        {
            HttpContext.Current.Response.ContentType = "text/xml";
            HttpContext.Current.Response.ContentEncoding = Encoding.GetEncoding("gb2312");
            HttpContext.Current.Response.Write("<?xml version=\"1.0\" encoding=\"gb2312\" ?>\n" + str);
        }

        public static string setValue(string str, string splitStr)
        {
            return (str + splitStr);
        }

        public void SeverTable(string CellValue, DataRowView rowv, HtmlTable table)
        {
            if (table != null)
            {
                HtmlTableCell htc = new HtmlTableCell();
                string[] sp = CellValue.Split(new char[] { ',' });
                if (sp.Length > 0)
                {
                    foreach (string i in sp)
                    {
                        htc.Attributes.Add("class", "con_left");
                        htc.InnerHtml = (rowv[i].ToString() != "") ? rowv[i].ToString() : "暂无记录";
                        table.Rows[0].Cells.Add(htc);
                    }
                }
            }
        }

        public static string Table_CellCount(DataTable dt, string cellName)
        {
            StringBuilder jsonString = new StringBuilder();
            if ((dt != null) && (dt.Rows.Count > 0))
            {
                DataView dv = dt.Copy().DefaultView;
                if (cellName != "")
                {
                    dv.RowFilter = cellName;
                }
                return dv.Count.ToString();
            }
            return "数据集为空";
        }

       
    }
}

