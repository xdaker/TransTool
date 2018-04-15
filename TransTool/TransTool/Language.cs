using System;
using System.Collections.Generic;
using Tool;

namespace Utilities
{
    public static class Language
    {
        private static TransApi TransApi { get; } =new TransApi("2015063000000001", "1435660288")
        {
            WaitiTime = 10000,
        };

        private static object Lockobject { get; } =new object();
        public static string Translate(string text , LangType langType = LangType.zh)
        {
            lock (Lockobject)
            {
                if (langType == LangType.zh) return text;
                var strArray = SplitStrings(text);
                string result = string.Empty;
                foreach (var str in strArray)
                {
                    result += TransApi.GetTransResult(str, LangType.auto.ToString(), langType.ToString());
                }
                return string.IsNullOrWhiteSpace(result) ? text : result;
            }
        }

        public static string ToZh(string text)
        {
            if(string.IsNullOrEmpty(text)) throw new Exception("ToZh: text is null or empty");
            lock (Lockobject)
            {
                var str = TransApi.GetTransResult(text, LangType.auto.ToString(), LangType.zh.ToString());
                return string.IsNullOrWhiteSpace(str) ? text : str;
            }
        }

        public const int MaxTransLenght = 100;
        private static string[] SplitStrings(string text)
        {
            if (text.Length <= TransApi.MaxTextLength) return new string[] {text};
            List<string> list = new List<string>();
            int index = 0;
            int number = text.Length / MaxTransLenght;
            number += text.Length % MaxTransLenght > 0 ? 1 : 0;
            for (int i = 1; i < number + 1; i++)
            {
                var end = index + MaxTransLenght > text.Length ?
                    index + MaxTransLenght - text.Length - 1 
                    : MaxTransLenght;
                list.Add(text.Substring(index, end));
                index = i * MaxTransLenght;
            }
            return list.ToArray();
        }
    }
}