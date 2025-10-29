using System;
using System.Collections.Generic;

namespace Config.Testdemo
{
    public partial class DataGlobal_num
    {
        public string Id { get; private set; } /* int */
        public string ColorType { get; private set; } /* enum */

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (obj == this) return true;
            var o = obj as DataGlobal_num;
            return o != null && Id.Equals(o.Id);
        }

        public override string ToString()
        {
            return "(" + Id + "," + ColorType + ")";
        }

        
        static Config.KeyedList<string, DataGlobal_num> all = null;

        public static DataGlobal_num Get(string id)
        {
            DataGlobal_num v;
            return all.TryGetValue(id, out v) ? v : null;
        }

        public static List<DataGlobal_num> All()
        {
            return all.OrderedValues;
        }

        public static List<DataGlobal_num> Filter(Predicate<DataGlobal_num> predicate)
        {
            var r = new List<DataGlobal_num>();
            foreach (var e in all.OrderedValues)
            {
                if (predicate(e))
                    r.Add(e);
            }
            return r;
        }

        internal static void Initialize(Config.Stream os, Config.LoadErrors errors)
        {
            all = new Config.KeyedList<string, DataGlobal_num>();
            for (var c = os.ReadInt32(); c > 0; c--)
            {
                var self = _create(os);
                all.Add(self.Id, self);
            }

        }

        internal static DataGlobal_num _create(Config.Stream os)
        {
            var self = new DataGlobal_num();
            self.Id = os.ReadString();
            self.ColorType = os.ReadString();
            return self;
        }

    }
}