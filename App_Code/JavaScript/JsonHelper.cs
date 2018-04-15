using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;
//using System.Web.Script.Serialization;
using System.Text;
using System.Web.Script.Serialization;
using System.Xml;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text.RegularExpressions;
namespace RoleDomain.Common.JavaScript
{
    /// <summary>
    ///JsonHelper 的摘要说明
    /// </summary>
    public static class JsonHelper
    {
        static JavaScriptSerializer serialier = new JavaScriptSerializer();

        public static dynamic readJSON(string json)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            jss.RegisterConverters(new JavaScriptConverter[] { new DynamicJsonConverter() });
            dynamic dy = jss.Deserialize(json, typeof(object)) as dynamic;

            return dy;
        }


        /// <summary>
        /// 修正诡异的/Date(1494434700000)/格式
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static string fix_localDate(string json)
        {
            json = Regex.Replace(json, @"\\/Date\((\d+)\)\\/", match =>
            {
                DateTime dt = new DateTime(1970, 1, 1);
                dt = dt.AddMilliseconds(long.Parse(match.Groups[1].Value));
                dt = dt.ToLocalTime(); //本地时间
                return dt.ToString("yyyy-MM-dd HH:mm:ss"); ;
            });
            return json;
        }

        /// List转成json  
        /// </summary> 
        /// <typeparam name="T"></typeparam> 
        /// <param name="jsonName"></param> 
        /// <param name="list"></param> 
        /// <returns></returns> 
        public static string ListToJson2<T>(IList<T> list, string jsonName)
        {
            StringBuilder Json = new StringBuilder();
            if (string.IsNullOrEmpty(jsonName))
                jsonName = list[0].GetType().Name;
            Json.Append("{\"" + jsonName + "\":[");
            if (list.Count > 0)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    T obj = Activator.CreateInstance<T>();
                    PropertyInfo[] pi = obj.GetType().GetProperties();
                    Json.Append("{");
                    for (int j = 0; j < pi.Length; j++)
                    {
                        Type type = pi[j].GetValue(list[i], null).GetType();
                        Json.Append("\"" + pi[j].Name.ToString() + "\":" + StringFormat(pi[j].GetValue(list[i], null).ToString(), type));

                        if (j < pi.Length - 1)
                        {
                            Json.Append(",");
                        }
                    }
                    Json.Append("}");
                    if (i < list.Count - 1)
                    {
                        Json.Append(",");
                    }
                }
            }
            Json.Append("]}");
            return Json.ToString();
        }

        public static DataSet ConvertXMLFileToDataSet(XmlDocument xml)
        {
            //读入XML文档
            //XmlDocument xml = new XmlDocument();
            //xml.Load(xmlFile);
            DataSet result = new DataSet();
            using (StringReader stream = new StringReader(xml.InnerXml))
            {
                using (XmlTextReader reader = new XmlTextReader(stream))
                {
                    result.ReadXml(reader);
                    return result;
                }
            }
        }

        public static DataSet Json2DataSet(string sJson)
        {

            return ConvertXMLFileToDataSet(Json2Xml(sJson));
        }

        public static string Json2XmlString(string sJson)
        {
            string xml = "";
            using (MemoryStream ms = new MemoryStream())
            {

                Json2Xml(sJson).Save(ms);
                xml = System.Text.Encoding.UTF8.GetString(ms.ToArray());
                ms.Close();
            }

            return xml;
        }

        public static XmlDocument Json2Xml(string sJson)
        {
            //XmlDictionaryReader reader = JsonReaderWriterFactory.CreateJsonReader(Encoding.UTF8.GetBytes(sJson), XmlDictionaryReaderQuotas.Max);
            //XmlDocument doc = new XmlDocument();
            //doc.Load(reader);

            JavaScriptSerializer oSerializer = new JavaScriptSerializer();
            Dictionary<string, object> Dic = (Dictionary<string, object>)oSerializer.DeserializeObject(sJson);
            XmlDocument doc = new XmlDocument();
            XmlDeclaration xmlDec;
            xmlDec = doc.CreateXmlDeclaration("1.0", "utf-8", "no");
            doc.InsertBefore(xmlDec, doc.DocumentElement);
            XmlElement nRoot = doc.CreateElement("root");
            doc.AppendChild(nRoot);
            foreach (KeyValuePair<string, object> item in Dic)
            {
                XmlElement element = doc.CreateElement(item.Key);
                KeyValue2Xml(element, item);
                nRoot.AppendChild(element);
            }
            return doc;
        }

        private static void KeyValue2Xml(XmlElement node, KeyValuePair<string, object> Source)
        {
            object kValue = Source.Value;

            if (kValue == null)
            {
                kValue = "";
            }

            if (kValue.GetType() == typeof(Dictionary<string, object>))
            {
                foreach (KeyValuePair<string, object> item in kValue as Dictionary<string, object>)
                {

                    //if (item.Value == null) {
                    //    item.Value = "";
                    //}
                    XmlElement element = node.OwnerDocument.CreateElement(item.Key);
                    KeyValue2Xml(element, item);
                    node.AppendChild(element);
                }
            }
            else if (kValue.GetType() == typeof(object[]))
            {
                object[] o = kValue as object[];
                for (int i = 0; i < o.Length; i++)
                {
                    try
                    {
                        XmlElement xitem = node.OwnerDocument.CreateElement("Item");
                        KeyValuePair<string, object> item = new KeyValuePair<string, object>("Item", o[i]);
                        KeyValue2Xml(xitem, item);
                        node.AppendChild(xitem);
                    }
                    catch (Exception err)
                    {

                    }
                }

            }
            else
            {
                XmlText text = node.OwnerDocument.CreateTextNode(kValue.ToString());
                node.AppendChild(text);
            }
        }


        public static string ToJsonString(this object obj)
        {
            return ToJsonString(obj, false);
        }

        /// <summary>
        /// 对象转化为Json字符串
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToJsonString(this object obj, bool ToLower)
        {
            StringBuilder jsonBuilder = new StringBuilder();
            jsonBuilder.Append("{");
            var pvalue = "";
            System.Reflection.PropertyInfo[] pis = obj.GetType().GetProperties();
            for (int j = 0; j < pis.Length; j++)
            {
                jsonBuilder.Append("\"");
                if (ToLower)
                {
                    jsonBuilder.Append(pis[j].Name.ToLower());
                }
                else
                {
                    jsonBuilder.Append(pis[j].Name);
                }
                jsonBuilder.Append("\":\"");
                pvalue = pis[j].GetValue(obj, null).ToString();
                if (pvalue == "1753-1-1 0:00:00")
                {
                    pvalue = "";
                }
                jsonBuilder.Append(pvalue.Replace("\"", @"\\"));
                jsonBuilder.Append("\",");
            }
            jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
            jsonBuilder.Append("}");
            return jsonBuilder.ToString();
        }
        ///// <summary>
        ///// JSON字符串转化为对象
        ///// </summary>
        ///// <param name="obj"></param>
        ///// <returns></returns>
        //public static T ToObject<T>(this string obj)
        //{
        //    JavaScriptSerializer Serializer = new JavaScriptSerializer();
        //    return Serializer.Deserialize<T>(obj);
        //}
        /// <summary>
        /// 对象转化为JSON字符串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string Serialize<T>(T obj)
        {
            System.Runtime.Serialization.Json.DataContractJsonSerializer serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(obj.GetType());
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            serializer.WriteObject(ms, obj);
            string retVal = System.Text.Encoding.UTF8.GetString(ms.ToArray());
            ms.Dispose();
            return retVal;
        }
        /// <summary>
        ///  JSON字符串转化为对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static T Deserialize<T>(string json)
        {
            T obj = Activator.CreateInstance<T>();
            System.IO.MemoryStream ms = new System.IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(json));
            System.Runtime.Serialization.Json.DataContractJsonSerializer serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(obj.GetType());
            obj = (T)serializer.ReadObject(ms);
            ms.Close();
            ms.Dispose();
            return obj;
        }

        /// <summary>
        /// 得到传递参数
        /// </summary>
        /// <param name="name"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static string GetFormParam(string name, string defaultValue)
        {
            string val = HttpContext.Current.Request.Form[name] ?? defaultValue;
            if (val == defaultValue)
            {
                val = HttpContext.Current.Request.QueryString[name] ?? defaultValue;
            }

            return val.ToString().Trim();
        }


        public static string ListToJson<T>(IList<T> list, string jsonName)
        {
            return ListToJson(list, jsonName, false);
        }

        /// <summary>

        /// List转成json

        /// </summary>

        /// <typeparam name="T"></typeparam>

        /// <param name="jsonName"></param>

        /// <param name="list"></param>

        /// <returns></returns>

        public static string ListToJson<T>(IList<T> list, string jsonName, bool ToLower)
        {

            StringBuilder Json = new StringBuilder();

            if (string.IsNullOrEmpty(jsonName))

                jsonName = list[0].GetType().Name;

            Json.Append("{\"" + jsonName + "\":[");

            if (list.Count > 0)
            {

                for (int i = 0; i < list.Count; i++)
                {

                    T obj = Activator.CreateInstance<T>();

                    PropertyInfo[] pi = obj.GetType().GetProperties();

                    Json.Append("{");

                    for (int j = 0; j < pi.Length; j++)
                    {

                        Type type = pi[j].GetValue(list[i], null).GetType();

                        if (!ToLower)
                        {
                            Json.Append("\"" + pi[j].Name.ToString() + "\":" + StringFormat(pi[j].GetValue(list[i], null).ToString(), type));
                        }
                        else
                        {
                            Json.Append("\"" + pi[j].Name.ToString().ToLower() + "\":" + StringFormat(pi[j].GetValue(list[i], null).ToString(), type));

                        }

                        if (j < pi.Length - 1)
                        {

                            Json.Append(",");

                        }

                    }

                    Json.Append("}");

                    if (i < list.Count - 1)
                    {

                        Json.Append(",");

                    }

                }

            }

            Json.Append("]}");

            return Json.ToString();

        }


        public static string ListToJson<T>(IList<T> list)
        {
            return ListToJson(list, false);
        }
        /// <summary>

        /// List转成json

        /// </summary>

        /// <typeparam name="T"></typeparam>

        /// <param name="list"></param>

        /// <returns></returns>

        public static string ListToJson<T>(IList<T> list, bool ToLower)
        {

            object obj = list[0];

            return ListToJson<T>(list, obj.GetType().Name, ToLower);

        }
        public static string ToJson(object jsonObject)
        {
            return ToJson(jsonObject, false);

        }
        public static string ToJson(object jsonObject, bool ToLower)
        {
            return ToJson(jsonObject, new List<string>(), new List<string>(), ToLower);
        }

        /// <summary>
        /// Object转Json 排除列名
        /// </summary>
        /// <param name="jsonObject">对象类</param>
        /// <param name="exclude_list">排除列名</param>
        /// <returns></returns>
        public static string ToJson(object jsonObject, List<string> exclude_list, bool ToLower)
        {
            return ToJson(jsonObject, exclude_list, new List<string>(), ToLower);

        }
        /// <summary>
        /// Object转Json 排除列名
        /// </summary>
        /// <param name="jsonObject">对象类</param>
        /// <param name="json_colume">Json列，值不加"</param>
        /// <param name="ToLower">项名小写</param>
        /// <returns></returns>
        public static string ToJson_JColume(object jsonObject, List<string> json_colume, bool ToLower)
        {
            return ToJson(jsonObject, new List<string>(), json_colume, ToLower);
        }
        /// <summary>
        /// Object转Json 排除列名
        /// </summary>
        /// <param name="jsonObject">对象类</param>
        /// <param name="json_colume">Json列，值不加"</param>
        /// <returns></returns>
        public static string ToJson_JColume(object jsonObject, List<string> json_colume)
        {
            return ToJson(jsonObject, new List<string>(), json_colume, false);
        }

        /// <summary>
        /// 对象转换为Json字符串
        /// </summary>
        /// <param name="jsonObject">一个需要转成json的类</param>
        /// <param name="exclude_list">转换时需要排出的类属性</param>
        /// <param name="json_colume">某一项是Json数据类型，做特殊处理</param>
        /// <returns></returns>
        public static string ToJson(object jsonObject, List<string> exclude_list, List<string> json_colume, bool ToLower)
        {
            return ToJson(jsonObject, exclude_list, json_colume, ToLower, false);

        }

        /// <summary>
        /// 对象转换为Json字符串
        /// </summary>
        /// <param name="jsonObject">一个需要转成json的类</param>
        /// <param name="exclude_list">转换时需要排出的类属性</param>
        /// <param name="json_colume">某一项是Json数据类型，做特殊处理</param>
        /// <param name="ToLower">是否自动转换小写</param>
        /// <param name="onlyMember">是否只转换标记为<code>[DataMember]</code>特性的属性</param>
        /// <returns></returns>
        public static string ToJson(object jsonObject, List<string> exclude_list, List<string> json_colume, bool ToLower, bool onlyMember)
        {

            //try
            //{
            StringBuilder jsonString = new StringBuilder();
            jsonString.Append("{");
            PropertyInfo[] propertyInfo = jsonObject.GetType().GetProperties();
            for (int i = 0; i < propertyInfo.Length; i++)
            {
                if (onlyMember && jsonObject.GetType().Name.IndexOf("f__AnonymousType") == -1)
                {
                    var attr = propertyInfo[i].GetCustomAttribute<DataMemberAttribute>(false);
                    if (attr == null)
                    {
                        continue;
                    }
                }

                object objectValue = propertyInfo[i].GetGetMethod().Invoke(jsonObject, null);
                if (objectValue == null || exclude_list.Contains(propertyInfo[i].Name))
                {
                    continue;
                }

                StringBuilder value = new StringBuilder();
                if (objectValue is DateTime || objectValue is Guid || objectValue is TimeSpan)
                {
                    value.Append("\"" + objectValue.ToString() + "\"");
                }
                else if (objectValue is Int32 || objectValue is Int64 || objectValue is UInt32 || objectValue is UInt64 || objectValue is Int16 || objectValue is UInt16 || objectValue is float || objectValue is Single || objectValue is long || objectValue is double || objectValue is decimal)
                {
                    value.Append("" + objectValue.ToString() + "");
                }
                else if (objectValue is string)
                {
                    //如果此项是Json项目则特殊处理
                    if (json_colume.Contains(propertyInfo[i].Name.ToLower()))
                    {
                        value.Append(objectValue.ToString());
                    }
                    else
                    {
                        value.Append("\"" + objectValue.ToString().clear_json_value() + "\"");
                    }
                }
                else if (objectValue is IEnumerable)
                {
                    value.Append(ToJson((IEnumerable)objectValue));
                }
                else if (objectValue is DataTable)
                {
                    value.Append(ToJson((DataTable)objectValue));
                }
                else
                {
                    value.Append("\"" + objectValue.ToString() + "\"");
                }
                if (!ToLower)
                {
                    jsonString.Append("\"" + propertyInfo[i].Name + "\":" + value + ","); ;
                }
                else
                {
                    jsonString.Append("\"" + propertyInfo[i].Name.ToLower() + "\":" + value + ","); ;
                }
            }
            return jsonString.ToString().TrimEnd(',') + "}";
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
        }



        /// <summary>

        /// 对象集合转换Json

        /// </summary>

        /// <param name="array">集合对象</param>

        /// <returns>Json字符串</returns>

        public static string ToJson(IEnumerable array)
        {
            string jsonString = "[";
            foreach (object item in array)
            {


                if (item is string)
                {
                    jsonString += "\"" + item.ToString() + "\",";
                }
                else if (item is Int32 || item is float || item is long || item is double || item is decimal || item is bool)
                {
                    jsonString += "" + item.ToString() + ",";
                }
                else if (item is DynamicJson)
                {
                    jsonString += ((DynamicJson)item).ToJson() + ",";
                }
                else if (item is DynamicJsonObject)
                {
                    jsonString += ((DynamicJsonObject)item).ToJson() + ",";
                }
                else if (item is Dictionary<string, object>)
                {
                    jsonString += ToJson((Dictionary<string, object>)item) + ",";
                }
                else if (item is ArrayList)
                {
                    jsonString += ToJson((ArrayList)item) + ",";
                }
                else
                {
                    jsonString += ToJson(item) + ",";
                }
            }
            jsonString = jsonString.TrimEnd(',');

            return jsonString + "]";

        }

        public static string ToJson(Object[] array)
        {
            string jsonString = "[";
            foreach (object item in array)
            {
                if (item is DynamicJson)
                {
                    jsonString += ((DynamicJson)item).ToJson() + ",";
                }
                else if (item is DynamicJsonObject)
                {
                    jsonString += ((DynamicJsonObject)item).ToJson() + ",";
                }
                else if (item is Dictionary<string, object>)
                {
                    jsonString += ToJson((Dictionary<string, object>)item) + ",";
                }
                else
                {
                    jsonString += ToJson(item) + ","; ;
                }
            }
            jsonString = jsonString.TrimEnd(',');

            return jsonString + "]";

        }

        /// <summary>

        /// 对象集合转换Json

        /// </summary>

        /// <param name="array">集合对象</param>

        /// <returns>Json字符串</returns>

        public static string ToJson(Dictionary<string, object> dic)
        {
            StringBuilder jsonString = new StringBuilder();
            jsonString.Append("{");
            foreach (var item in dic)
            {
                object objectValue = item.Value;
                //if (objectValue == null)
                //{
                //    continue;
                //}

                StringBuilder value = new StringBuilder();
                if (objectValue == null)
                {
                    value.Append("\"\"");
                }

                else if (objectValue is DateTime || objectValue is Guid || objectValue is TimeSpan)
                {
                    value.Append("\"" + objectValue.ToString() + "\"");
                }
                else if (objectValue is Int32 || objectValue is float || objectValue is long || objectValue is double || objectValue is decimal || objectValue is bool)
                {
                    value.Append(objectValue.ToString());
                }
                else if (objectValue is string)
                {
                    value.Append("\"" + objectValue.ToString() + "\"");
                }
                else if (objectValue is DataTable)
                {
                    value.Append(ToJson((DataTable)objectValue, true));
                }
                else if (objectValue is DynamicJsonValue)
                {
                    value.Append(objectValue.ToString());
                }
                else if (objectValue is DynamicJson)
                {
                    value.Append(((DynamicJson)objectValue).ToJson());
                }
                else if (objectValue is DynamicJsonObject)
                {
                    value.Append(((DynamicJsonObject)objectValue).ToJson());
                }
                else if (objectValue.GetType().FullName.IndexOf("System.Collections.Generic.List`1[[") > -1
                   && objectValue.GetType().FullName.IndexOf("DynamicJson") > -1
                   )
                {
                    value.Append(ToJson((List<DynamicJson>)objectValue));
                }
                else if (objectValue.GetType().FullName.IndexOf("System.Collections.Generic.List`1[[") > -1
             && objectValue.GetType().FullName.IndexOf("DynamicJsonObject") > -1
             )
                {
                    value.Append(ToJson((List<DynamicJsonObject>)objectValue));
                }
                //属于泛型List<T>
                else if (objectValue.GetType().FullName.IndexOf("System.Collections.Generic.List`1[[") > -1
                    && objectValue.GetType().FullName.IndexOf("DynamicJson") == -1
                    && objectValue.GetType().FullName.IndexOf("DynamicJsonObject") == -1
                    && objectValue.GetType().FullName.IndexOf("System.Object") == -1
                    )
                {
                    value.Append(JsonHelper.fix_localDate(
                    serialier.Serialize(objectValue)
                     )
                     );
                }

                else if (objectValue is List<object>)// || objectValue is System.Collections.Generic.List<object>)
                {
                    value.Append(ToJson((List<object>)objectValue));
                }
                else if (objectValue is Dictionary<string, object>)
                {
                    value.Append(ToJson((Dictionary<string, object>)objectValue));
                }
                //else if (objectValue is ArrayList) {
                //    value.Append(ToArrayString((ArrayList)objectValue)); 
                //}
                else if (objectValue is string)
                {
                    value.Append("\"" + objectValue.ToString() + "\"");
                }
                else if (objectValue is Object[])
                {
                    value.Append(ToJson((Object[])objectValue));
                }
                else if (objectValue is Object)
                {
                    //value.Append(ToJson(objectValue, new List<string>(), new List<string>(), true, true));
                    value.Append(JsonHelper.fix_localDate(
                 serialier.Serialize(objectValue)
                  )
                  );
                }
                else
                {
                    value.Append("\"" + objectValue.ToString() + "\"");
                }
                jsonString.Append("\"" + item.Key + "\":" + value + ","); ;
            }
            return jsonString.ToString().TrimEnd(',') + "}";
        }

        //public static string ToJson(IDictionary<string, object> dic)
        //{
        //    StringBuilder jsonString = new StringBuilder();
        //    jsonString.Append("{");
        //    foreach (var item in dic)
        //    {
        //        object objectValue = item.Value;
        //        if (objectValue == null)
        //        {
        //            continue;
        //        }

        //        StringBuilder value = new StringBuilder();
        //        if (objectValue is DateTime || objectValue is Guid || objectValue is TimeSpan)
        //        {
        //            value.Append("\"" + objectValue.ToString() + "\"");
        //        }
        //        else if (objectValue is Int32 || objectValue is float || objectValue is long || objectValue is double || objectValue is decimal)
        //        {
        //            value.Append(objectValue.ToString());
        //        }
        //        else if (objectValue is string)
        //        {
        //            value.Append("\"" + objectValue.ToString() + "\"");
        //        }
        //        else if (objectValue is DataTable)
        //        {
        //            value.Append(ToJson((DataTable)objectValue, true));
        //        }
        //        else if (objectValue is DynamicJson)
        //        {
        //            value.Append(((DynamicJson)objectValue).ToJson());
        //        }
        //        else if (objectValue is DynamicJsonValue)
        //        {
        //            value.Append(objectValue.ToString());
        //        }
        //        else if (objectValue is DynamicJsonObject)
        //        {
        //            value.Append(((DynamicJsonObject)objectValue).ToJson());
        //        }
        //        else if (objectValue is List<object>)
        //        {
        //            value.Append(ToJson((List<object>)objectValue));
        //        }
        //        else if (objectValue is Dictionary<string, object>)
        //        {
        //            value.Append(ToJson((Dictionary<string, object>)objectValue));
        //        }
        //        else if (objectValue is Object[])
        //        {
        //            value.Append(ToJson((Object[])objectValue));
        //        }
        //        else if (objectValue is Object)
        //        {
        //            value.Append(ToJson(objectValue, new List<string>(), new List<string>(), true, true));
        //        }
        //        else
        //        {
        //            value.Append("\"" + objectValue.ToString() + "\"");
        //        }
        //        jsonString.Append("\"" + item.Key + "\":" + value + ","); ;
        //    }
        //    return jsonString.ToString().TrimEnd(',') + "}";
        //}

        //public static string ToJson(Dictionary<string, object> dic)
        //{
        //    StringBuilder jsonString = new StringBuilder();
        //    jsonString.Append("{");
        //    foreach (var item in dic)
        //    {
        //        object objectValue = item.Value;
        //        if (objectValue == null)
        //        {
        //            continue;
        //        }

        //        StringBuilder value = new StringBuilder();
        //        if (objectValue is DateTime || objectValue is Guid || objectValue is TimeSpan)
        //        {
        //            value.Append("\"" + objectValue.ToString() + "\"");
        //        }
        //        else if (objectValue is Int32 || objectValue is float)
        //        {
        //            value.Append(objectValue.ToString());
        //        }
        //        else if (objectValue is string)
        //        {
        //            value.Append("\"" + objectValue.ToString() + "\"");
        //        }
        //        else if (objectValue is IEnumerable)
        //        {
        //            value.Append(ToJson((IEnumerable)objectValue));
        //        }
        //        else if (objectValue is DataTable)
        //        {
        //            value.Append(ToJson((DataTable)objectValue, true));
        //        }
        //        else
        //        {
        //            value.Append("\"" + objectValue.ToString() + "\"");
        //        }
        //        jsonString.Append("\"" + item.Key + "\":" + value + ","); ;
        //    }
        //    return jsonString.ToString().TrimEnd(',') + "}";
        //}



        /// <summary>

        /// 普通集合转换Json

        /// </summary>

        /// <param name="array">集合对象</param>

        /// <returns>Json字符串</returns>

        public static string ToArrayString(IEnumerable array)
        {

            string jsonString = "[";

            foreach (object item in array)
            {

                jsonString = ToJson(item.ToString()) + ",";

            }

            jsonString.Remove(jsonString.Length - 1, jsonString.Length);

            return jsonString + "]";

        }



        /// <summary>

        /// Datatable转换为Json

        /// </summary>

        /// <param name="table">Datatable对象</param>

        /// <returns>Json字符串</returns>

        public static string ToJson(DataTable dt)
        {
            return ToJson(dt, false);
        }

        /// <summary>

        /// Datatable转换为Json

        /// </summary>

        /// <param name="table">Datatable对象</param>
        /// <param name="isFull">日期是否完整显示</param>
        /// <returns>Json字符串</returns>

        public static string ToJson(DataTable dt, bool DateisFull)
        {

            StringBuilder jsonString = new StringBuilder();

            jsonString.Append("[");

            DataRowCollection drc = dt.Rows;

            for (int i = 0; i < drc.Count; i++)
            {

                jsonString.Append("{");

                for (int j = 0; j < dt.Columns.Count; j++)
                {

                    string strKey = dt.Columns[j].ColumnName;

                    string strValue = drc[i][j].ToString();

                    Type type = dt.Columns[j].DataType;

                    jsonString.Append("\"" + strKey + "\":");

                    strValue = StringFormat(strValue, type, DateisFull);

                    if (j < dt.Columns.Count - 1)
                    {
                        jsonString.Append(strValue + ",");
                    }

                    else
                    {

                        jsonString.Append(strValue);

                    }

                }

                jsonString.Append("},");

            }

            if (jsonString.Length > 1)
            {
                jsonString.Remove(jsonString.Length - 1, 1);
            }

            jsonString.Append("]");

            return jsonString.ToString();

        }



        /// <summary>

        /// DataTable转成Json

        /// </summary>

        /// <param name="jsonName"></param>

        /// <param name="dt"></param>

        /// <returns></returns>

        public static string ToJson(DataTable dt, string jsonName)
        {

            StringBuilder Json = new StringBuilder();

            if (string.IsNullOrEmpty(jsonName))

                jsonName = dt.TableName;

            Json.Append("{\"" + jsonName + "\":[");

            if (dt.Rows.Count > 0)
            {

                for (int i = 0; i < dt.Rows.Count; i++)
                {

                    Json.Append("{");

                    for (int j = 0; j < dt.Columns.Count; j++)
                    {

                        Type type = dt.Rows[i][j].GetType();

                        Json.Append("\"" + dt.Columns[j].ColumnName.ToString() + "\":" + StringFormat(dt.Rows[i][j].ToString(), type));

                        if (j < dt.Columns.Count - 1)
                        {

                            Json.Append(",");

                        }

                    }

                    Json.Append("}");

                    if (i < dt.Rows.Count - 1)
                    {

                        Json.Append(",");

                    }

                }

            }

            Json.Append("]}");

            return Json.ToString();

        }



        /// <summary>

        /// DataReader转换为Json

        /// </summary>

        /// <param name="dataReader">DataReader对象</param>

        /// <returns>Json字符串</returns>

        public static string ToJson(IDataReader dataReader)
        {

            StringBuilder jsonString = new StringBuilder();

            jsonString.Append("[");



            while (dataReader.Read())
            {

                jsonString.Append("{");

                for (int i = 0; i < dataReader.FieldCount; i++)
                {

                    Type type = dataReader.GetFieldType(i);

                    string strKey = dataReader.GetName(i);

                    string strValue = dataReader[i].ToString();

                    jsonString.Append("\"" + strKey + "\":");

                    strValue = StringFormat(strValue, type);

                    if (i < dataReader.FieldCount - 1)
                    {

                        jsonString.Append(strValue + ",");

                    }

                    else
                    {

                        jsonString.Append(strValue);

                    }

                }

                jsonString.Append("},");

            }

            dataReader.Close();

            jsonString.Remove(jsonString.Length - 1, 1);

            jsonString.Append("]");

            if (jsonString.Length == 1)
            {

                return "[]";

            }

            return jsonString.ToString();

        }

        /// <summary>
        /// 将DataTable中的数据转换成JSON格式
        /// </summary>
        /// <param name="dt">数据源DataTable</param>
        /// <param name="displayCount">是否输出数据总条数</param>
        /// <param name="totalcount">JSON中显示的数据总条数</param>
        /// <returns></returns>
        public static string CreateJsonParameters(DataTable dt, bool displayCount, int totalcount)
        {
            StringBuilder JsonString = new StringBuilder();
            //Exception Handling        

            if (dt != null)
            {
                JsonString.Append("{ ");
                JsonString.Append("\"rows\":[ ");
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    JsonString.Append("{ ");
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        if (j < dt.Columns.Count - 1)
                        {
                            //if (dt.Rows[i][j] == DBNull.Value) continue;
                            if (dt.Columns[j].DataType == typeof(bool))
                            {
                                JsonString.Append("\"" + dt.Columns[j].ColumnName + "\":" +
                                                  dt.Rows[i][j].ToString().clear_json_value() + ",");
                            }
                            else if (dt.Columns[j].DataType == typeof(string))
                            {
                                JsonString.Append("\"" + dt.Columns[j].ColumnName + "\":" + "\"" +
                                                  dt.Rows[i][j].ToString().clear_json_value() + "\",");
                            }
                            else
                            {
                                JsonString.Append("\"" + dt.Columns[j].ColumnName + "\":" + "\"" + dt.Rows[i][j].ToString().clear_json_value() + "\",");
                            }
                        }
                        else if (j == dt.Columns.Count - 1)
                        {
                            //if (dt.Rows[i][j] == DBNull.Value) continue;
                            if (dt.Columns[j].DataType == typeof(bool))
                            {
                                JsonString.Append("\"" + dt.Columns[j].ColumnName + "\":" +
                                                  dt.Rows[i][j].ToString().clear_json_value());
                            }
                            else if (dt.Columns[j].DataType == typeof(string))
                            {
                                JsonString.Append("\"" + dt.Columns[j].ColumnName + "\":" + "\"" +
                                                  dt.Rows[i][j].ToString().clear_json_value() + "\"");
                            }
                            else
                            {
                                JsonString.Append("\"" + dt.Columns[j].ColumnName + "\":" + "\"" + dt.Rows[i][j].ToString().clear_json_value() + "\"");
                            }
                        }
                    }
                    /*end Of String*/
                    if (i == dt.Rows.Count - 1)
                    {
                        JsonString.Append("} ");
                    }
                    else
                    {
                        JsonString.Append("}, ");
                    }
                }
                JsonString.Append("]");

                if (displayCount)
                {
                    JsonString.Append(",");

                    JsonString.Append("\"total\":");
                    JsonString.Append(totalcount);
                }

                JsonString.Append("}");
                return JsonString.ToString().Replace("\n", "");
            }
            else
            {
                return null;
            }
        }

        public static string CreateJsonForTreeGrid(DataTable dt, bool displayCount, int totalcount)
        {
            StringBuilder JsonString = new StringBuilder();
            //Exception Handling        

            if (dt != null)
            {
                JsonString.Append("{ ");
                JsonString.Append("\"rows\":[ ");
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    JsonString.Append("{ ");
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        if (j < dt.Columns.Count - 1)
                        {
                            //if (dt.Rows[i][j] == DBNull.Value) continue;
                            if (dt.Columns[j].DataType == typeof(bool))
                            {
                                JsonString.Append("\"" + dt.Columns[j].ColumnName + "\":" +
                                                  dt.Rows[i][j].ToString().clear_json_value() + ",");
                            }
                            else if (dt.Columns[j].DataType == typeof(string))
                            {
                                JsonString.Append("\"" + dt.Columns[j].ColumnName + "\":" + "\"" +
                                                  dt.Rows[i][j].ToString().clear_json_value() + "\",");
                            }
                            else
                            {

                                if (dt.Columns[j].ColumnName != "_parentId")
                                {
                                    JsonString.Append("\"" + dt.Columns[j].ColumnName + "\":" + "\"" + dt.Rows[i][j].ToString() + "\",");
                                }

                                else
                                {
                                    if (dt.Rows[i][j].ToString() == "0")
                                    {
                                        JsonString.Append(" \"state\":\"closed\",");
                                    }
                                    else
                                    {
                                        JsonString.Append("\"" + dt.Columns[j].ColumnName + "\":" + "\"" + dt.Rows[i][j].ToString() + "\",");
                                    }

                                }

                            }
                        }
                        else if (j == dt.Columns.Count - 1)
                        {
                            //if (dt.Rows[i][j] == DBNull.Value) continue;
                            if (dt.Columns[j].DataType == typeof(bool))
                            {
                                JsonString.Append("\"" + dt.Columns[j].ColumnName + "\":" +
                                                  dt.Rows[i][j].ToString().clear_json_value());
                            }
                            else if (dt.Columns[j].DataType == typeof(string))
                            {
                                JsonString.Append("\"" + dt.Columns[j].ColumnName + "\":" + "\"" +
                                                  dt.Rows[i][j].ToString().clear_json_value() + "\"");
                            }
                            else
                            {
                                JsonString.Append("\"" + dt.Columns[j].ColumnName + "\":" + "\"" + dt.Rows[i][j].ToString().clear_json_value() + "\"");
                            }
                        }
                    }
                    /*end Of String*/
                    if (i == dt.Rows.Count - 1)
                    {
                        JsonString.Append("} ");
                    }
                    else
                    {
                        JsonString.Append("}, ");
                    }
                }
                JsonString.Append("]");

                if (displayCount)
                {
                    JsonString.Append(",");

                    JsonString.Append("\"total\":");
                    JsonString.Append(totalcount);
                }

                JsonString.Append("}");
                return JsonString.ToString().Replace("\n", "");
            }
            else
            {
                return null;
            }
        }

        /// <summary>

        /// DataSet转换为Json

        /// </summary>

        /// <param name="dataSet">DataSet对象</param>

        /// <returns>Json字符串</returns>

        public static string ToJson(DataSet dataSet)
        {

            string jsonString = "{";

            foreach (DataTable table in dataSet.Tables)
            {

                jsonString += "\"" + table.TableName + "\":" + ToJson(table) + ",";

            }

            jsonString = jsonString.TrimEnd(',');

            return jsonString + "}";

        }



        /// <summary>

        /// 过滤特殊字符

        /// </summary>

        /// <param name="s"></param>

        /// <returns></returns>

        private static string String2Json(String s)
        {
             
            return s;

        }



        /// <summary>

        /// 格式化字符型、日期型、布尔型

        /// </summary>

        /// <param name="str"></param>

        /// <param name="type"></param>

        /// <returns></returns>

        private static string StringFormat(string str, Type type)
        {

            return StringFormat(str, type, false);

        }

        /// <summary>

        /// 格式化字符型、日期型、布尔型

        /// </summary>

        /// <param name="str"></param>

        /// <param name="type"></param>

        /// <returns></returns>

        public static string StringFormat(string str, Type type, bool DateisFull)
        {

            if (type != typeof(string) && string.IsNullOrEmpty(str))
            {

                str = "\"" + str + "\"";

            }

            else if (type == typeof(string))
            {

                str = String2Json(str);

                str = "\"" + str + "\"";

            }

            else if (type == typeof(DateTime))
            {

                if (DateisFull)
                {
                    str = "\"" + str + "\"";
                }
                else
                {
                    str = "\"" + str.Split(' ')[0] + "\"";
                }

            }

            else if (type == typeof(bool))
            {

                str = str.ToLower();

            }



            return str;

        }

        /// <summary>   
        /// Json序列化   
        /// </summary>   
        public static string JsonSerializer<T>(T obj)
        {
            string jsonString = string.Empty;
            try
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));

                using (MemoryStream ms = new MemoryStream())
                {
                    serializer.WriteObject(ms, obj);
                    jsonString = Encoding.UTF8.GetString(ms.ToArray());
                }
            }
            catch
            {
                jsonString = string.Empty;
            }
            return jsonString;
        }

        /// <summary>   
        /// Json反序列化  
        /// </summary>   
        public static T JsonDeserialize<T>(string jsonString)
        {
            T obj = Activator.CreateInstance<T>();
            try
            {
                using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonString)))
                {
                    DataContractJsonSerializer ser = new DataContractJsonSerializer(obj.GetType());//typeof(T)  
                    T jsonObject = (T)ser.ReadObject(ms);
                    ms.Close();

                    return jsonObject;
                }
            }
            catch
            {
                return default(T);
            }
        }

        // 将 DataTable 序列化成 json 字符串  
        public static string DataTableToJson(DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0)
            {
                return "\"\"";
            }
            JavaScriptSerializer myJson = new JavaScriptSerializer();

            List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();

            foreach (DataRow dr in dt.Rows)
            {
                Dictionary<string, object> result = new Dictionary<string, object>();
                foreach (DataColumn dc in dt.Columns)
                {
                    result.Add(dc.ColumnName, dr[dc].ToString());
                }
                list.Add(result);
            }
            return myJson.Serialize(list);
        }

        // 将对象序列化成 json 字符串  
        public static string ObjectToJson(object obj)
        {
            if (obj == null)
            {
                return string.Empty;
            }
            JavaScriptSerializer myJson = new JavaScriptSerializer();

            return myJson.Serialize(obj);
        }

        // 将 json 字符串反序列化成对象  
        public static object JsonToObject(string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                return null;
            }
            JavaScriptSerializer myJson = new JavaScriptSerializer();

            return myJson.DeserializeObject(json);
        }

        // 将 json 字符串反序列化成对象  
        public static T JsonToObject<T>(string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                return default(T);
            }
            JavaScriptSerializer myJson = new JavaScriptSerializer();

            return myJson.Deserialize<T>(json);
        }
    }
}