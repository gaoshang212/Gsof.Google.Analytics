using System.Collections.Generic;

namespace Gsof.Google.Analytics
{
    public interface ITracker
    {
        ITracker Append(object p_obj);
        ITracker Append(IDictionary<string, string> p_dic);
        ITracker Append(IDictionary<string, object> p_dic);

        string GetBody();
    }
}