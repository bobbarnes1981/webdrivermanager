using System;
using System.Collections.Generic;
using System.IO;

namespace WebDriverManager
{
    public class Properties
    {
        private Dictionary<string, string> dict;

        public Properties()
        {
            dict = new Dictionary<string, string>();
        }

        public void Load(Stream stream)
        {
            using (StreamReader reader = new StreamReader(stream))
            {
                string line;
                while((line = reader.ReadLine()) != null)
                {
                    if (line.StartsWith("#"))
                    {
                        continue;
                    }
                    if (string.IsNullOrEmpty(line))
                    {
                        continue;
                    }
                    string[] parts = line.Split('=');
                    dict.Add(parts[0], parts[1]);
                }
            }
        }
        public string GetProperty(string name)
        {
            if (!dict.ContainsKey(name))
            {
                return null;
            }
            return dict[name];
        }
    }
}
