using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Utilities
{
    public class KeyValueStore
    {
        private Dictionary<string, object> store = new Dictionary<string, object>();

        // Private constructor to prevent external instantiation
        private KeyValueStore() { }

        // Singleton instance
        public static KeyValueStore Instance { get; } = new KeyValueStore();

        public void Set(string key, object value)
        {
            store[key] = value;
        }

        public T? Get<T>(string key)
        {
            if (store.TryGetValue(key, out var value) && value is T)
            {
                return (T)value;
            }
            return default(T);
        }

        public bool ContainsKey(string key)
        {
            return store.ContainsKey(key);
        }

        public void Remove(string key)
        {
            if (store.ContainsKey(key))
            {
                store.Remove(key);
            }
        }
    }
}
