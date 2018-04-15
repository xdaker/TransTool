using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using Tool;

namespace TransTool
{
    class Program
    {
        public static string FilePath = "./TaskControlLanguage.xml";
        static void Main(string[] args)
        {
            var xmlModel = XmlSerializer.LoadFromXml<XmlModel>(FilePath);
            var list = xmlModel.Config.Rows.ToList();
            list.ForEach(v =>
            {
                string str = v.CHINESE_SIMPLIFIED;
                str = Language.Translate(str, LangType.cht);
                v.CHINESE_Traditional = str;
                Console.WriteLine(str);
            });
            xmlModel.Config.Rows = list.ToArray();
            XmlSerializer.SaveToXml(FilePath, xmlModel, null, null);
            
            Console.Read();
        }
    }
}
