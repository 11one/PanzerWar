using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class JsonHelper
{
	
	//Usage:
	//YouObject[] objects = JsonHelper.getJsonArray<YouObject> (jsonString);
	public static List<T> getJsonArray<T> (string json)
	{
		string newJson = "{ \"array\": " + json + "}";
		Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>> (newJson);
		return wrapper.array;
	}
	//Usage:
	//string jsonString = JsonHelper.arrayToJson<YouObject>(objects);
	public static string arrayToJson<T> (List<T> array)
	{
		Wrapper<T> wrapper = new Wrapper<T> ();
		wrapper.array = array;
		return JsonUtility.ToJson (wrapper);
	}

	[Serializable]
	private class Wrapper<T>
	{
		public List<T> array;
	}


    //Usage:
    //YouObject[] objects = JsonHelper.getJsonArray<YouObject> (jsonString);
    public static Hashtable getJsonArray (string json)
    {
        string newJson = "{ \"array\": " + json + "}";
        Wrapper wrapper = JsonUtility.FromJson<Wrapper> (newJson);
        return wrapper.array;
    }
    //Usage:
    //string jsonString = JsonHelper.arrayToJson<YouObject>(objects);
    public static string arrayToJson (Hashtable array)
    {
        Wrapper wrapper = new Wrapper();
        wrapper.array = array;
        return JsonUtility.ToJson (wrapper);
    }

    [Serializable]
    private class Wrapper
    {
        public Hashtable array;
    }
}