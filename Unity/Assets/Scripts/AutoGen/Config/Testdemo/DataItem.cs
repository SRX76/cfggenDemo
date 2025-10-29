using System;
using System.Collections.Generic;

namespace Config.Testdemo
{
    public partial class DataItem
    {
        public string Id { get; private set; } /* string */

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (obj == this) return true;
            var o = obj as DataItem;
            return o != null && Id.Equals(o.Id);
        }

        public override string ToString()
        {
            return "(" + Id + ")";
        }

        
        static Config.KeyedList<string, DataItem> all = null;

        public static DataItem Get(string id)
        {
            DataItem v;
            return all.TryGetValue(id, out v) ? v : null;
        }

        public static List<DataItem> All()
        {
            return all.OrderedValues;
        }

        public static List<DataItem> Filter(Predicate<DataItem> predicate)
        {
            var r = new List<DataItem>();
            foreach (var e in all.OrderedValues)
            {
                if (predicate(e))
                    r.Add(e);
            }
            return r;
        }

        internal static void Initialize(Config.Stream os, Config.LoadErrors errors)
        {
            all = new Config.KeyedList<string, DataItem>();
            for (var c = os.ReadInt32(); c > 0; c--)
            {
                var self = _create(os);
                all.Add(self.Id, self);
            }

        }

        internal static DataItem _create(Config.Stream os)
        {
            var self = new DataItem();
            self.Id = os.ReadString();
            return self;
        }

    }
}