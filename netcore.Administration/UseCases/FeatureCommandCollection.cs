using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Entities;

namespace Administration.UseCases
{
    internal class FeatureCommandCollection : IEnumerable<KeyValuePair<string, ICollection<ICommand>>>
    {
        private readonly IDictionary<string, ICollection<ICommand>> _dictionary;

        public FeatureCommandCollection()
        {
            _dictionary = new Dictionary<string, ICollection<ICommand>>(
                StringComparer.CurrentCultureIgnoreCase);
        }

        public void Add(IDictionary<string, IEnumerable<ICommand>> lhs)
        {
            foreach (var pair in lhs)
            {
                if (_dictionary.ContainsKey(pair.Key))
                    _dictionary[pair.Key] = pair.Value.ToArray();
                else
                    _dictionary.Add(pair.Key, pair.Value.ToArray());
            }
        }

        public IEnumerator<KeyValuePair<string, ICollection<ICommand>>> GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool ContainsKey(string keyname)
        {
            return _dictionary.ContainsKey(keyname);
        }

        public IEnumerable<ICommand> this[string keyname] => _dictionary[keyname];
    }
}