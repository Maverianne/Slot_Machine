using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Managers;
using Newtonsoft.Json;
using UnityEngine;

public static class FileHandler
{
    public static List<T> ReadFromJsonList<T>(string fileName)
    {
        var jsonString = ReadFile(GetPath(fileName));
        
        if (string.IsNullOrEmpty(jsonString) || jsonString == "{}") return new List<T>();
        
        return JsonHelper.GetJsonArray<T>(jsonString).ToList();
    }
    
    public static List<List<T>> ReadFromJson<T>(string fileName)
    {
        var jsonString = ReadFile(GetPath(fileName));
        
        if (string.IsNullOrEmpty(jsonString) || jsonString == "{}") return new List<List<T>>();
        
        return JsonHelper.GetJsonArrayStrips<T>(jsonString);
    }

    private static string GetPath(string fileName)
    {
        var path = Application.dataPath + ConstantsManager.Files.JsonPath + fileName;
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
        public static T[] GetJsonArray<T>(string json)
        {
            var wrapper = JsonUtility.FromJson<WrapperSpin<T>>(json);
            return wrapper.Spins;
        }
        public static List<List<T>> GetJsonArrayStrips<T>(string json)
        {
            var wrapper = JsonConvert.DeserializeObject<WrapperReel<T>>(json);
            return wrapper.ReelStrips;
        }
        
        [Serializable]
        private class WrapperSpin<T>
        {
            public T[] Spins; 
        }
        
        [Serializable]
        private class WrapperReel<T>
        {
            public List<List<T>> ReelStrips { get; set; }
        }
        
    }
}
