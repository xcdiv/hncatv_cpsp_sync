using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoleDomain.Common.JavaScript
{
    public class DynamicJsonValue
    {
        public DynamicJsonValue() { }

        public DynamicJsonValue(string _value) {
            this.value = _value;
        }

        public string value = "";

        public override string ToString()
        {
            return value;
        }
    }
}
