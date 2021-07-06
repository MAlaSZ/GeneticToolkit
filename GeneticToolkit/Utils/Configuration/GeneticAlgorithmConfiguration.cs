using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using Newtonsoft.Json;

namespace GeneticToolkit.Utils.Configuration
{
    public class GeneticAlgorithmConfiguration
    {
    }

    public class DynamicObjectInfo
    {
        public string Name { get; set; }
        public string Type { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public object Value { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public List<DynamicObjectInfo> Properties { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public List<DynamicObjectInfo> Parameters { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore)]
        public List<string> GenericParameters { get; set; }
    }

    public static class DynamicObjectFactory<T>
    {
        public static T Build(DynamicObjectInfo objectInfo)
        {
            var type = Type.GetType(objectInfo.Type);
            if (type == null)
            {
                return default;
            }

            if (type.IsPrimitive || type == typeof(string))
            {
                return (T) objectInfo.Value;
            }

            if (type.IsGenericType && objectInfo.GenericParameters?.Count > 0)
            {
                type = type.MakeGenericType(objectInfo.GenericParameters.Select(Type.GetType).ToArray());
            }

            T instance = default;
            if (objectInfo.Parameters?.Count > 0)
            {
                var constructors = type.GetConstructors();
                var constructor = constructors
                    .FirstOrDefault(c => c.GetParameters()
                        .Select(p => p.ParameterType.FullName)
                        .OrderBy(p => p)
                        .SequenceEqual(objectInfo.Parameters.Select(o => o.Type).OrderBy(p => p)));
                if (constructor == null)
                {
                    return instance;
                }

                var parameterValues =
                    objectInfo.Parameters.ToDictionary(p => p.Name.ToLower(), p => p.Value);
                var parameters = constructor?
                    .GetParameters().OrderBy(p => p.Position)
                    .Select(p => parameterValues[p.Name!.ToLower()])
                    .ToArray();
                instance = (T) constructor.Invoke(parameters);
            }
            else
            {
                instance = (T) Activator.CreateInstance(type);
            }

            if (objectInfo.Properties == null)
            {
                return instance;
            }

            foreach (var property in objectInfo.Properties)
            {
                var propertyType = Type.GetType(property.Type);
                if (propertyType.IsPrimitive || propertyType == typeof(string))
                {
                    type.GetProperty(property.Name)?.SetValue(instance, property.Value);
                    continue;
                }

                var value = DynamicObjectFactory<dynamic>.Build(property);
                type.GetProperty(property.Name)?.SetValue(instance, value);
            }

            return instance;
        }


        public static DynamicObjectInfo Serialize(object instance, string name)
        {
            if (instance == null)
            {
                return null;
            }

            var type = instance.GetType();
            if (type.IsAssignableTo(typeof(MulticastDelegate)))
            {
                return null;
            }

            if (type.IsPrimitive || type == typeof(string) || type.IsEnum || type == typeof(DateTime) || type == typeof(Guid))
            {
                return new DynamicObjectInfo
                {
                    Name = name,
                    Properties = null,
                    GenericParameters = null,
                    Type = type.FullName,
                    Value = instance
                };
            }

            if (type.IsArray)
            {
                type = type.UnderlyingSystemType;
                var typeName= type.GetGenericArguments().ToList().Count > 0 
                    ? type.FullName.Remove(type.FullName.IndexOf('`')) : type.FullName;
                return new DynamicObjectInfo
                {
                    Name = name,
                    Properties = null,
                    GenericParameters = null,
                    Type = typeName,
                    Value = (instance as object[])?.Select(obj => Serialize(obj, name))
                };
            }
            
            var info = new DynamicObjectInfo
            {
                Name = name,
                Properties = new List<DynamicObjectInfo>(),
                GenericParameters = type.GetGenericArguments().Select(a => a.FullName).ToList()
            };
            info.Type = info.GenericParameters?.Count > 0 ? type.FullName.Remove(type.FullName.IndexOf('`')) : type.Name;

            var properties = type.GetProperties().Where(p => p.CanWrite && p.CanRead && p.GetIndexParameters().Length == 0);
            foreach (var property in properties)
            {
                var serialized = Serialize(property.GetValue(instance), property.Name);
                info.Properties.Add(serialized);
            }

            return info;
        }
    }
}