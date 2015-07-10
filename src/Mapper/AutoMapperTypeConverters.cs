using System;
using AutoMapper;

namespace EFDDD.DomainDataMapper
{
    public class ObjectToGuidConverter : TypeConverter<Object, Guid>
    {
        protected override Guid ConvertCore(object source)
        {
            if (source is Guid)
            {
                return (Guid) source;
            }
            return Guid.Empty;
        }
    }

    public class GuidToObjectConverter : TypeConverter<Guid, Object>
    {
        protected override object ConvertCore(Guid source)
        {
            return source as object;
        }
    }

    public class DateTimeToNullableDateTimeConverter : TypeConverter<DateTime, DateTime?>
    {
        protected override DateTime? ConvertCore(DateTime source)
        {
            return source == DateTime.MinValue || source == DateTime.MaxValue ? (DateTime?)null : source;
        }
    }

    public class DateTimeToStringConverter : TypeConverter<DateTime, string>
    {
        protected override string ConvertCore(DateTime source)
        {
            return source == DateTime.MinValue || source == DateTime.MaxValue ? string.Empty : source.ToString("G");
        }
    }

    public class NullableDateTimeToNullableDateTimeConverter : TypeConverter<DateTime?, DateTime?>
    {
        protected override DateTime? ConvertCore(DateTime? source)
        {
            return source.HasValue ? source.Value : (DateTime?)null;
        }
    }

    public class NullableDateTimeToDateTimeConverter : TypeConverter<DateTime?, DateTime>
    {
        protected override DateTime ConvertCore(DateTime? source)
        {
            return source.HasValue ? source.Value : DateTime.MinValue;
        }
    }

    public class NullableDateTimeToStringConverter : TypeConverter<DateTime?, string>
    {
        protected override string ConvertCore(DateTime? source)
        {
            return source.HasValue ? string.Format("{0} {1}", source.Value.ToShortDateString(), source.Value.ToShortTimeString()) : string.Empty;
        }
    }

    public class NullableGuidToStringConverter : TypeConverter<Guid?, string>
    {
        protected override string ConvertCore(Guid? source)
        {
            return source.HasValue ? source.Value.ToString() : string.Empty;
        }
    }

    public class NullableIntToIntConverter : TypeConverter<int?, int>
    {
        protected override int ConvertCore(int? source)
        {
            return source.HasValue ? source.Value : default(int);
        }
    }

    public class NullableInt32ToIntConverter : TypeConverter<Int32?, int>
    {
        protected override int ConvertCore(Int32? source)
        {
            return source.HasValue ? source.Value : default(int);
        }
    }

    public class NullableInt64ToLongConverter : TypeConverter<Int64?, long>
    {
        protected override long ConvertCore(Int64? source)
        {
            return source.HasValue ? source.Value : default(long);
        }
    }

    public class NullableDecimalToDecimalConverter : TypeConverter<decimal?, decimal>
    {
        protected override decimal ConvertCore(decimal? source)
        {
            return source.HasValue ? source.Value : default(decimal);
        }
    }
}
