﻿using System;
using UnityEngine;

namespace Kadinche.Kassets.Variable
{
    /// <summary>
    /// Jsonable Variable. A variable that can be serialized to json.
    /// </summary>
    /// <typeparam name="T">Type to use on variable system</typeparam>
    public abstract class JsonableVariable<T> : VariableCore<T>, IJsonable<T>
    {
        /// <summary>
        /// Load variable from json string
        /// </summary>
        /// <param name="jsonString"></param>
        public void FromJsonString(string jsonString)
        {
            var simpleType = Type.IsSimpleType();
            Value = simpleType
                ? JsonUtility.FromJson<JsonableWrapper<T>>(jsonString).Value
                : JsonUtility.FromJson<T>(jsonString);
        }

        /// <summary>
        /// Converts variable to json string. Formats with pretty print when in Unity Editor.
        /// </summary>
        /// <returns>Converted json string</returns>
        public string ToJsonString()
        {
            var isSimpleType = typeof(T).IsSimpleType();
            return isSimpleType ?
                JsonUtility.ToJson(new JsonableWrapper<T>(Value), Application.isEditor) :
                JsonUtility.ToJson(Value, Application.isEditor);
        }
    }
    
    public interface IJsonable { }
    public interface IJsonable<T> : IJsonable
    {
        T Value { get; set; }
        
        void FromJsonString(string jsonString);
        string ToJsonString();
    }
    
    [Serializable]
    public readonly struct JsonableWrapper<T>
    {
        public readonly T Value;
        public JsonableWrapper(T value)
        {
            Value = value;
        }
    }
}