using System;
using System.Collections.Generic;
using System.Text;

namespace SaphirCloudBox.Services.Utils
{
    public static class Base64Converter
    {
        public static byte[] ToByteArray(this string content)
        {
            return Convert.FromBase64String(content);
        }
    }
}
