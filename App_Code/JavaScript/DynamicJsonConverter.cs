namespace RoleDomain.Common.JavaScript
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Web.Script.Serialization;
    /// <summary>
    /// json转换
    /// </summary>
    public class JsonParser
    {

        /// <summary>
        /// 从json字符串到对象。
        /// </summary>
        /// <param name="jsonStr"></param>
        /// <returns></returns>
        public dynamic FromJson(string jsonStr)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            jss.RegisterConverters(new JavaScriptConverter[] { new DynamicJsonConverter() });

            dynamic glossaryEntry = jss.Deserialize(jsonStr, typeof(object)) as dynamic;
            return glossaryEntry;
        }
    }
 
    public class DynamicJsonConverter : JavaScriptConverter
    {
        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            if (dictionary == null)
            {
                throw new ArgumentNullException("dictionary");
            }
            if (type == typeof(object))
            {
                return new DynamicJsonObject(dictionary);
            }
            return null;
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<Type> SupportedTypes
        {
            get
            {
                return new ReadOnlyCollection<Type>(new List<Type>(new Type[] { typeof(object) }));
            }
        }
    }


    public class TreeNodeJSConverter : JavaScriptConverter
    {
        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            TreeNode node = new TreeNode();

            object value = null;
            if (dictionary.TryGetValue("id", out value))
                node.id = (string)value;
            if (dictionary.TryGetValue("text", out value))
                node.text = (string)value;
            if (dictionary.TryGetValue("children", out value))
            {
                if (value != null && value.GetType() == typeof(ArrayList))
                {
                    var list = (ArrayList)value;
                    node.children = new List<TreeNode>();

                    foreach (Dictionary<string, object> item in list)
                    {
                        node.children.Add((TreeNode)this.Deserialize(item, type, serializer));
                    }
                }
                else
                {
                    node.children = null;
                }
            }
            return node;
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            var node = obj as TreeNode;
            if (node == null)
                return null;
            if (!string.IsNullOrEmpty(node.id))
                dic.Add("id", node.id);
            if (!string.IsNullOrEmpty(node.text))
                dic.Add("text", node.text);
            if (node.isChecked.HasValue)
                dic.Add("checked", node.isChecked.Value);
            if (node.children != null)
                dic.Add("children", node.children);

            return dic;
        }

        public override IEnumerable<Type> SupportedTypes
        {
            get
            {
                return new Type[] { typeof(TreeNode) };
            }
        }
        public class TreeNode
        {
            public string id { get; set; }
            public string text { get; set; }
            public bool? isChecked { get; set; }
            public List<TreeNode> children { get; set; }
        }
    }
}

