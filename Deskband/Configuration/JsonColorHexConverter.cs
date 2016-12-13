using Newtonsoft.Json;
using System;
using System.Drawing;

namespace Deskband.Configuration
{
    public class JsonColorHexConverter: JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(Color));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var c = (Color)value;
            writer.WriteValue("#" + c.A.ToString("X2") + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2"));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return ColorTranslator.FromHtml(reader.Value.ToString());
        }
    }
}
