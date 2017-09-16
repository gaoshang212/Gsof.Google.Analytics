using System.Collections.Generic;
using System.Linq;
using Gsof.Extensions;

namespace Gsof.Google.Analytics
{
    public class Tracker : ITracker
    {
        private readonly Dictionary<string, string> _dic;

        public Tracker(object p_obj) : this(p_obj.GetProperties())
        {

        }

        public Tracker(IDictionary<string, object> p_dic)
        {
            _dic = p_dic == null ? new Dictionary<string, string>() : p_dic.ToDictionary(i => i.Key, j => j.Value?.ToString());
        }

        public ITracker Append(object p_obj)
        {
            return Append(p_obj.GetProperties());
        }

        public ITracker Append(IDictionary<string, string> p_dic)
        {
            if (p_dic != null && p_dic.Count > 0)
            {
                _dic.AddRange(p_dic);
            }

            return this;
        }

        public ITracker Append(IDictionary<string, object> p_dic)
        {
            return Append(p_dic.ToDictionary(i => i.Key, j => j.Value?.ToString()));
        }

        public string GetBody()
        {
            var list = _dic.Select(i => $"{i.Key}={i.Value ?? ""}");
            return string.Join("&", list);
        }
    }
}