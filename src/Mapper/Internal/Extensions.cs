using System.Linq;
using AutoMapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace EFDDD.DomainDataMapper.Internal
{
    internal static class Extensions
    {
        internal static T JsonDeserialize<T>(this string _this, JsonSerializerSettings settings)
        {
            var enumConverter = new StringEnumConverter { CamelCaseText = false };

            if (settings == null)
            {
                settings = new JsonSerializerSettings();
                settings.Converters.Add(enumConverter);
                return JsonConvert.DeserializeObject<T>(_this, settings);
            }

            settings.Converters.Add(enumConverter);
            var value = JsonConvert.DeserializeObject<T>(_this, settings);
            return value;
        }

        internal static T JsonDeserialize<T>(this string _this)
        {
            return JsonDeserialize<T>(_this, null);
        }

        internal static T JsonDeserializeAnonymous<T>(this string _this, T obj)
        {
            var value = JsonConvert.DeserializeAnonymousType(_this, obj);
            return value;
        }

        internal static string JsonSerialize<T>(this T _this, JsonSerializerSettings settings)
        {
            var enumConverter = new StringEnumConverter { CamelCaseText = false };

            if (settings == null)
            {
                settings = new JsonSerializerSettings();
                settings.Converters.Add(enumConverter);
                return JsonConvert.SerializeObject(_this, settings);
            }

            settings.Converters.Add(enumConverter);
            var value = JsonConvert.SerializeObject(_this, settings);
            return value;
        }

        internal static string JsonSerialize<T>(this T _this)
        {
            return JsonSerialize(_this, null);
        }

        internal static JsonSerializerSettings PreservesNamespaces()
        {
            return new JsonSerializerSettings
            {
                PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                TypeNameHandling = TypeNameHandling.Objects
            };
        }

        internal static IMappingExpression<TSource, TDestination> IgnoreAllNonExisting<TSource, TDestination>(this IMappingExpression<TSource, TDestination> expression)
        {
            var sourceType = typeof(TSource);
            var destinationType = typeof(TDestination);
            var existingMaps = Mapper.GetAllTypeMaps().First(x => x.SourceType.Equals(sourceType) && x.DestinationType.Equals(destinationType));
            foreach (var property in existingMaps.GetUnmappedPropertyNames())
            {
                expression.ForMember(property, opt => opt.Ignore());
            }
            return expression;
        }
    }
}
