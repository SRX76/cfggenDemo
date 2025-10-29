using System.Collections.Generic;

namespace Config
{
    public static class Processor
    {
        public static readonly LoadErrors Errors = new LoadErrors();

        public static void Process(Config.Stream os)
        {
            var configNulls = new List<string>
            {
                "testdemo.global_num",
                "testdemo.item",
                "testdemo.stu_name",
            };
            for(;;)
            {
                var csv = os.ReadCfg();
                if (csv == null)
                    break;
                switch(csv)
                {
                    case "testdemo.global_num":
                        configNulls.Remove(csv);
                        Config.Testdemo.DataGlobal_num.Initialize(os, Errors);
                        break;
                    case "testdemo.item":
                        configNulls.Remove(csv);
                        Config.Testdemo.DataItem.Initialize(os, Errors);
                        break;
                    case "testdemo.stu_name":
                        configNulls.Remove(csv);
                        Config.Testdemo.DataStu_name.Initialize(os, Errors);
                        break;
                    default:
                        Errors.ConfigDataAdd(csv);
                        break;
                }
            }
            foreach (var csv in configNulls)
                Errors.ConfigNull(csv);
        }

    }
}
