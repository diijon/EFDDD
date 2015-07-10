using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace EFDDD.DomainDataRepository.Internal
{
    internal static class Helpers
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
    }
}
