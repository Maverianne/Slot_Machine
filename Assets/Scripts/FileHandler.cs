using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public static class FileHandler
{

    public static List<T> ReadFromJson<T>(string fileName)
    {
        var jsonString = ReadFile(GetPath(fileName));
        
        if (string.IsNullOrEmpty(jsonString) || jsonString == "{}")
        {
            return new List<T>();
        }
        return JsonHelper.FromJson<T>(jsonString).ToList();
    }
    

    private static string GetPath(string fileName)
    {
        var path = Application.dataPath + "/Scripts/JSON/" + fileName;
        return path;
    }

    private static string ReadFile(string path)
    {
        if (!File.Exists(path)) return "";
        using StreamReader reader = new StreamReader(path);
        var jsonString = reader.ReadToEnd();
        return jsonString;
    }
    
    
    public static class JsonHelper
    {
        public static T[] FromJson<T>(string json)
        {
            var wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
            return wrapper.Spins;
        }
        
        [Serializable]
        private class Wrapper<T>
        {
            public T[] Spins; 
        }
        
    }
}
