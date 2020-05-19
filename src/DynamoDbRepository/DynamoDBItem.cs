using System;
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

        public void AddPK(string value)
        {
            AddKeyAttrValue(DynamoDBConstants.PK, new AttributeValue(value));
        }

        public void AddSK(string value)
        {
            AddKeyAttrValue(DynamoDBConstants.SK, new AttributeValue(value));
        }

        public void AddGSI1(string value)
        {
            AddKeyAttrValue(DynamoDBConstants.GSI1, new AttributeValue(value));
        }

        public void AddString(string key, string value)
        {
            AddKeyAttrValue(key, new AttributeValue(value));
        }

        public void AddNumber(string key, int value)
        {
            AddKeyAttrValue(key, BaseNumberAttributeValue(Convert.ToString(value)));
        }

        public void AddNumber(string key, double value)
        {
            AddKeyAttrValue(key, BaseNumberAttributeValue(Convert.ToString(value)));
        }

        public bool IsEmpty
        {
            get { return _data.Count == 0; }
        }

        public string GetString(string key)
        {
            return _data.GetValueOrDefault(key)?.S;
        }

        public int GetInt32(string key)
        {
            return Convert.ToInt32(_data.GetValueOrDefault(key)?.N);
        }

        public double GetDouble(string key)
        {
            return Convert.ToDouble(_data.GetValueOrDefault(key)?.N);
        }        

        private void AddKeyAttrValue(string key, AttributeValue value)
        {
            if (!_data.ContainsKey(key))
                _data.Add(key, value);
            else
                _data[key] = value;
        }

        private AttributeValue BaseNumberAttributeValue(string value)
        {
            var result = new AttributeValue();
            result.N = value;
            return result;
        }

        internal Dictionary<string, AttributeValue> ToDictionary()
        {
            return _data;
        }
    }
}