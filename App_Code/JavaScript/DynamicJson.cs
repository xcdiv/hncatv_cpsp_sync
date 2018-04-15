namespace RoleDomain.Common.JavaScript
{
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Runtime.InteropServices;

    public class DynamicJson : DynamicObject
    {
        private Dictionary<string, object> storage = new Dictionary<string, object>();

        public bool CheckChar { get; set; }

        public void Remove(string key)
        {
            if (this.storage.ContainsKey(key))
            {
                this.storage.Remove(key);
            }
        }

        public DynamicJson()
        {
            CheckChar = true;
        }

        public string ToJson()
        {

            return ToJson(this.CheckChar);
        }

        public string ToJson(bool checkChar)
        {
            if (checkChar)
            {
                return JsonHelper.ToJson(this.storage).Replace("\r\n", "").Replace("\r", "").Replace("\n", "").Replace("\t", "");
            }
            else
            {
                return JsonHelper.ToJson(this.storage).Replace("\t", "");
            }
        }
        public bool ContainsKey(string key)
        {
            return this.storage.ContainsKey(key);
        }
        public override bool TryDeleteMember(DeleteMemberBinder binder)
        {
            string key = binder.Name;
            if (this.storage.ContainsKey(key))
            {
                this.storage.Remove(key);
            }
            return true;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (this.storage.ContainsKey(binder.Name))
            {
                result = this.storage[binder.Name];
                return true;
            }
            result = null;
            return false;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            string key = binder.Name;
            if (this.storage.ContainsKey(key))
            {
                this.storage[key] = value;
            }
            else
            {
                this.storage.Add(key, value);
            }
            return true;
        }
    }
}

