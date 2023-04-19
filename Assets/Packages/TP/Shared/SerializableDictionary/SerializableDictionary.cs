using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using UnityEngine;
using MyBox;

namespace TP.SerializableDictionary
{
    [Serializable]
    public struct StringEntry
    {
        [SerializeField, HideInInspector] private string key;
        [SerializeField, ReadOnly] private string value;

        public StringEntry(string key, string value)
        {
            this.key = key;
            this.value = value;
        }
    }
    
    [Serializable]
    public struct Entry<TKey,TValue>
    {
        [SerializeField] private TKey key;
        [SerializeField] private TValue value;

        public Entry(TKey key, TValue value)
        {
            this.key = key;
            this.value = value;
        }

        public TKey Key
        {
            get { return key; }
        }

        public TValue Value
        {
            get { return value; }
        }
    }
    
    [Serializable] 
    public class SerializableDictionary<TKey, TValue> : VirtualDictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        // [SerializeField, ReadOnly] private List<StringEntry> entries = new List<StringEntry>();
        [SerializeField] private List<TKey> serializedKeys = new List<TKey>();
        [SerializeField] private List<TValue> serializedValues = new List<TValue>();

        public SerializableDictionary() : base() { }
        public SerializableDictionary(SerializationInfo info, StreamingContext context) : base(info, context) { }

        private void DictToList()
        {
            serializedKeys.Clear();
            serializedValues.Clear();
            foreach (var item in this)
            {
                serializedKeys.Add(item.Key);
                serializedValues.Add(item.Value);
            }
        }

        private void ListToDict()
        {
            SerializableDictionary<TKey, TValue> tempDict = new SerializableDictionary<TKey, TValue>();
            Clear();
            for (int i = 0; i < serializedKeys.Count; i++)
            {
                tempDict[serializedKeys[i]] = serializedValues[i];
            }
            
            foreach (var item in tempDict)
            {
                this[item.Key] = item.Value;
            }
        } 

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            ListToDict(); 
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        { 
            DictToList();
        } 

        public override void Add(TKey key, TValue value)
        {
            base.Add(key, value);
        }

        public override bool Remove(TKey key)
        {
            bool result = base.Remove(key);
            return result; 
        }

        public override TValue this[TKey key]
        {
            get { return base[key]; }
            set
            {
                base[key] = value;
            }
        }
    }
}