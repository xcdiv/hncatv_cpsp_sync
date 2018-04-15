using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoleDomain.Common.JavaScript
{
    public class DynamicJsonObject : DynamicObject
    {
        public IDictionary<string, object> Dictionary { get; set; }
        public DynamicJsonObject(IDictionary<string, object> dictionary)
        {
            this.Dictionary = dictionary;
        }
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = this.Dictionary[binder.Name];
            if (result is IDictionary<string, object>)
            {
                result = new DynamicJsonObject(result as IDictionary<string, object>);
            }
            else if (result is ArrayList && (result as ArrayList) is IDictionary<string, object>)
            {
                result = new List<DynamicJsonObject>((result as ArrayList).ToArray().Select(x => new DynamicJsonObject(x as IDictionary<string, object>)));
            }
            else if (result is ArrayList)
            {
                result = new List<object>((result as ArrayList).ToArray());
            }
            return this.Dictionary.ContainsKey(binder.Name);
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            string key = binder.Name;
            if (this.Dictionary.ContainsKey(key))
                this.Dictionary[key] = value;
            else
                this.Dictionary.Add(key, value);
            return true;
        }

        public bool ContainsKey(string key) {
           return this.Dictionary.ContainsKey(key); 
        }

        /// <summary>
        /// 返回字典
        /// </summary>
        /// <returns></returns>
        public IDictionary<string, object> GetDic()
        {
            return this.Dictionary;
        }

        public string ToJson()
        {
            return ToJson(true);
        }

        public string ToJson(bool checkChar)
        {
            if (checkChar)
            {
                return JsonHelper.ToJson(this.Dictionary).Replace("\r\n", "").Replace("\r", "").Replace("\n", "").Replace("\t", "");
            }
            else
            {
                return JsonHelper.ToJson(this.Dictionary);
            }
        }
    }
}
