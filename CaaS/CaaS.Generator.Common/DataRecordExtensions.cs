using System;
using System.Data;

namespace CaaS.Generator.Common {
    public static class DataRecordExtensions {
        public static T GetValue<T>(this IDataRecord record, string key)
            => (T)GetObject(record, key, typeof(T))!;

        private static object? GetObject(this IDataRecord record, string key, Type targetType) {
            if (targetType == typeof(DateTimeOffset)) {
                return record.GetDateTimeOffset(key);
            }
            if (targetType == typeof(int)) {
                return record.GetIn32(key);
            }
            if (targetType == typeof(Guid)) {
                return record.GetGuid(key);
            }
            if (targetType == typeof(string)) {
                return record.GetString(key);
            }
            return record.GetObject(key);
        }

        private static DateTimeOffset? GetDateTimeOffset(this IDataRecord record, string key) {
            var obj = record.GetObject(key);
            if (obj == null) return null;
            if (obj is DateTimeOffset dateTimeOffset) return dateTimeOffset;
            if (obj is DateTime dateTime) return new DateTimeOffset(dateTime);

            throw new InvalidCastException();
        }

        private static int? GetIn32(this IDataRecord record, string key) {
            var obj = record.GetObject(key);
            return (int?)obj;
        }

        private static Guid? GetGuid(this IDataRecord record, string key) {
            var obj = record.GetObject(key);
            return (Guid?)obj;
        }

        private static string? GetString(this IDataRecord record, string key) {
            var obj = record.GetObject(key);
            return (string?)obj;
        }

        private static object? GetObject(this IDataRecord record, string key) {
            try {
                return record[key];
            }
            catch (IndexOutOfRangeException) {
                return null;
            }
        }
    }
}