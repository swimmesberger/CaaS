using System.Text.Json;

namespace CaaS.Core.Base; 

public static class JsonConstants {
    public static readonly JsonElement EmptyElement = new JsonElement();
    public static readonly JsonElement EmptyObject;
    
    static JsonConstants() {
        using var document = JsonDocument.Parse("{}");
        EmptyObject = document.RootElement.Clone();
    }
}