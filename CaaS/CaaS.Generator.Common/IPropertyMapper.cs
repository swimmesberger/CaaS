using System.Collections.Generic;

namespace CaaS.Generator.Common {
    public interface IPropertyMapper {
        IEnumerable<string> Keys { get; }

        string MapName(string key);
    }
}