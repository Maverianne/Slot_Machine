using System;
using System.Collections.Generic;
using System.Linq;
using Managers;
using Newtonsoft.Json;
using UnityEngine;

public static class FileHandler
{
    public static List<T> ReadFromJsonList<T>(string fileName)
    {
        var jsonString = Resources.Load(ConstantsManager.Files.JsonPath + fileName).ToString();
        
        if (string.IsNullOrEmpty(jsonString) || jsonString == "{}") return new List<T>();
        
        return JsonHelper.GetJsonArray<T>(jsonString).ToList();
    }
    
    public static List<List<T>> ReadFromJson<T>(string fileName)
    {
        var jsonString = Resources.Load(ConstantsManager.Files.JsonPath + fileName).ToString();
        
        if (string.IsNullOrEmpty(jsonString) || jsonString == "{}") return new List<List<T>>();
        
        return JsonHelper.GetJsonArrayStrips<T>(jsonString);
    }

    public static string GetPath(string fileName)
    {
        var path = Application.dataPath + ConstantsManager.Files.JsonPath + fileName;
        return path;
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
