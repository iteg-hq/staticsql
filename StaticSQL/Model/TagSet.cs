using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StaticSQL
{
    public class TagSet : HashSet<string>
    {
        public string GetValue(string key)
        {
            foreach (string tag in this)
            {
                if (tag.ToLower().StartsWith(key.ToLower() + ":"))
                {
                    return tag.Substring(key.Count() + 1);
                }
            }
            return Properties.Resources.UndefinedValue;
        }
    }
}
