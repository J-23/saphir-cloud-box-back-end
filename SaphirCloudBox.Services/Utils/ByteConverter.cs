using System;
using System.Collections.Generic;
using System.Text;

namespace SaphirCloudBox.Services.Utils
{
    public static class ByteConverter
    {
        private static readonly string[] SIZE_SUFFIXES = { "bytes", "KB", "MB", "GB", "TB" };

        private const long OneKb = 1024;
        private const long OneMb = OneKb * 1024;
        private const long OneGb = OneMb * 1024;
        private const long OneTb = OneGb * 1024;

        private const int DECIMAL_PLACES = 0;

        public static (int Size, string SizeType) ToPrettySize(this long value)
        {
            var sizeInTb = Math.Round((double)value / OneTb, DECIMAL_PLACES);
            var sizeInGb = Math.Round((double)value / OneGb, DECIMAL_PLACES);
            var sizeInMb = Math.Round((double)value / OneMb, DECIMAL_PLACES);
            var sizeInKb = Math.Round((double)value / OneKb, DECIMAL_PLACES);

            (int Size, string SizeType) result = sizeInTb > 1 ? (Convert.ToInt32(sizeInTb), "TB")
                : sizeInGb > 1 ? (Convert.ToInt32(sizeInGb), "GB")
                : sizeInMb > 1 ? (Convert.ToInt32(sizeInMb), "MB")
                : sizeInKb > 1 ? (Convert.ToInt32(sizeInKb), "KB")
                : (Convert.ToInt32(value), "bytes");

            return result;
        }
    }
}
