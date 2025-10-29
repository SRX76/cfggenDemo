using System;
using System.Collections.Generic;

namespace Config.Testdemo
{
    public partial class DataStu_name
    {
        public string ID { get; private set; } /* int */
        public string Name { get; private set; } /* string */
        public string Target { get; private set; } /* string */

        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (obj == this) return true;
            var o = obj as DataStu_name;
            return o != null && ID.Equals(o.ID);
        }

        public override string ToString()
        {
            return "(" + ID + "," + Name + "," + Target + ")";
        }

        
        static Config.KeyedList<string, DataStu_name> all = null;

        public static DataStu_name Get(string iD)
        {
            DataStu_name v;
            return all.TryGetValue(iD, out v) ? v : null;
        }

        public static List<DataStu_name> All()
        {
            return all.OrderedValues;
        }

        public static List<DataStu_name> Filter(Predicate<DataStu_name> predicate)
        {
            var r = new List<DataStu_name>();
            foreach (var e in all.OrderedValues)
            {
                if (predicate(e))
                    r.Add(e);
            }
            return r;
        }

        internal static void Initialize(Config.Stream os, Config.LoadErrors errors)
        {
            all = new Config.KeyedList<string, DataStu_name>();
            for (var c = os.ReadInt32(); c > 0; c--)
            {
                var self = _create(os);
                all.Add(self.ID, self);
            }

        }

        internal static DataStu_name _create(Config.Stream os)
        {
            var self = new DataStu_name();
            self.ID = os.ReadString();
            self.Name = os.ReadString();
            self.Target = os.ReadString();
            return self;
        }

    }
}