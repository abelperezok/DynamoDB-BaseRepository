using System.Collections.Generic;
using System.Linq;
using Amazon.DynamoDBv2.Model;

namespace DynamoDbRepository
{
    public class DynamoDBItem
    {
        private Dictionary<string, AttributeValue> _data = new Dictionary<string, AttributeValue>();

        public DynamoDBItem()
        {
        }

        internal DynamoDBItem(Dictionary<string, AttributeValue> item)
        {
            _data = item;
        }

        public DynamoDBItem MergeData(DynamoDBItem data)
        {
            var dataDict = data.ToDictionary();
            var merged = _data.Union(dataDict).ToDictionary(k => k.Key, v => v.Value);
            return new DynamoDBItem(merged);
        }

        public void AddPKValue(string value)
        {
            AddKeyValue(DynamoDBConstants.PK, new AttributeValue(value));
        }

        public void AddSKValue(string value)
        {
            AddKeyValue(DynamoDBConstants.SK, new AttributeValue(value));
        }

        public void AddGSI1Value(string value)
        {
            AddKeyValue(DynamoDBConstants.GSI1, new AttributeValue(value));
        }

        public void AddStringValue(string key, string value)
        {
            AddKeyValue(key, new AttributeValue(value));
        }

        public bool IsEmpty
        {
            get { return _data.Count == 0; }
        }

        public string GetStringValue(string key)
        {
            return _data.GetValueOrDefault(key)?.S;
        }


        private void AddKeyValue(string key, AttributeValue value)
        {
            if (!_data.ContainsKey(key))
                _data.Add(key, value);
            else
                _data[key] = value;
        }

        internal Dictionary<string, AttributeValue> ToDictionary()
        {
            return _data;
        }
    }
}