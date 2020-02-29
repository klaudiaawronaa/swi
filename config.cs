using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;


namespace swi
{

    [XmlRoot("objects")]
    [JsonConverter(typeof(ObjectsConverter))]
    public class Objects
    {
        [XmlElement("object")]
        public List<Object> objects { get; set; }
      
   
    }

    [JsonConverter(typeof(ObjectConverter))]
    public class Object
    {
        [XmlElement("obj_name")]
        public string objectName { get; set; }

        [XmlElement("field")]
        public List<Field> fields { get; set; }

    }

    [JsonConverter(typeof(FieldConverter))]
    public class Field
    {
        
        [XmlElement("name")]
        public string name { get; set; }
        [JsonIgnore]
        [XmlElement("type")]
        public string type { get; set; }
        [JsonIgnore]
        [XmlElement(ElementName = "value")]
        public string value { get; set; }

    }


    public class FieldConverter : JsonConverter
    {
        
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Field);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return new Field { name = reader.Value.ToString() };
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (((Field)value).value != "" && ((Field)value).name != "" &&
               ((Field)value).value != null && ((Field)value).name != null)
            {
                if (((Field)value).type == "string")
                    writer.WriteRawValue(" " + "\"" +((Field)value).name + "\": \"" + ((Field)value).value + "\"");
                if (((Field)value).type == "int")
                    writer.WriteRawValue(" " + "\""  + ((Field)value).name + "\": " + ((Field)value).value);

            }
        }
    }

    public class ObjectConverter : JsonConverter
    {

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Object);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return new Object { objectName = reader.Value.ToString() };
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            IEnumerable array = ((Object)value).fields;
            writer.WriteStartObject();
            writer.WritePropertyName(((Object)value).objectName);
            writer.WriteStartArray();
            foreach (object item in array)
            {
                serializer.Serialize(writer, item);
            }
            writer.WriteEndArray();
            writer.WriteEndObject();
        }
    }

    public class ObjectsConverter : JsonConverter
    {

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Objects);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return null;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            IEnumerable array = ((Objects)value).objects;
            writer.WriteStartObject();
            writer.WritePropertyName("");
            writer.WriteStartArray();
            foreach (Object item in array)
            {
                if(item.objectName != " " && item.fields.Count()>=1) serializer.Serialize(writer, item);
            }
            writer.WriteEndArray();
            writer.WriteEndObject();
        }
    }
}
