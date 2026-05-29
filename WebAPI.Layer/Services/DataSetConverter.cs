using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Data;

namespace WebAPI.Layer.Services
{
    public class DataSetConverter : JsonConverter<DataSet>
    {
        public override void WriteJson(JsonWriter writer, DataSet value, JsonSerializer serializer)
        {
            JObject obj = new JObject();

            foreach (DataTable table in value.Tables)
            {
                JObject tableObj = new JObject
                {
                    ["TableName"] = table.TableName,
                    ["Columns"] = JArray.FromObject(table.Columns, serializer),
                    ["Rows"] = JArray.FromObject(table.Rows, serializer)
                };

                // Include ExtendedProperties
                if (table.ExtendedProperties.Count > 0)
                {
                    JObject extendedProps = new JObject();
                    foreach (string key in table.ExtendedProperties.Keys)
                    {
                        extendedProps[key] = JToken.FromObject(table.ExtendedProperties[key]);
                    }
                    tableObj["ExtendedProperties"] = extendedProps;
                }

                obj[table.TableName] = tableObj;
            }

            obj.WriteTo(writer);
        }

        public override DataSet ReadJson(JsonReader reader, Type objectType, DataSet existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JObject obj = JObject.Load(reader);
            DataSet dataSet = new DataSet();

            foreach (var property in obj.Properties())
            {
                JObject tableObj = (JObject)property.Value;
                DataTable table = new DataTable(tableObj["TableName"].ToString());

                // Restore Columns
                foreach (JObject colObj in tableObj["Columns"])
                {
                    table.Columns.Add(colObj["ColumnName"].ToString(), Type.GetType(colObj["DataType"].ToString()));
                }

                // Restore Rows
                foreach (JArray row in tableObj["Rows"])
                {
                    table.Rows.Add(row.ToObject<object[]>());
                }

                // Restore ExtendedProperties
                if (tableObj["ExtendedProperties"] is JObject extendedProps)
                {
                    foreach (var prop in extendedProps.Properties())
                    {
                        table.ExtendedProperties[prop.Name] = prop.Value.ToObject<object>();
                    }
                }

                dataSet.Tables.Add(table);
            }

            return dataSet;
        }
    }
}
