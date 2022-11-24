using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;

namespace CaaS.Api.DiscountApi.Swagger; 

public class OpenApiReferenceJsonConverter : JsonConverter<OpenApiReference> {
    public override OpenApiReference Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        throw new NotImplementedException();
    }
    
    public override void Write(Utf8JsonWriter writer, OpenApiReference value, JsonSerializerOptions options) {
        using var textWriter = new StringWriter(CultureInfo.InvariantCulture);
        var jsonWriter = new OpenApiJsonWriter(textWriter);
        value.SerializeAsV3(jsonWriter);
        writer.WriteRawValue(textWriter.ToString());
    }
}