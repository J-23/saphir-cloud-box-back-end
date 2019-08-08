using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaphirCloudBox.Host.Infractructure
{
    public class Constants
    {
        public const string ISSUER = "SaphirCloudBox";
        public const string AUDIENCE = "http://saphir-cloud-box-ui/"; // потребитель токена
        const string KEY = "r#ryLh?wwJ2;7,7m6[MU&`R";   // ключ для шифрации
        public const int LIFETIME = 1440;

        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(KEY));
        }

        public class NotificationMessages
        {
            public const string ForgotPasswordSubject = @"Das Kennwort zurücksetzen";

            public const string ForgotPasswordMessage = @"Guten Tag Herr {0},
Sie haben sich auf unserer Webseite {1} registriert. Um Ihre Registrierung abzuschließen, klicken Sie zur Bestätigung auf folgenden Link: {2}";

            public const string ResetPasswordUrl = @"/auth/reset-password?code={0}";
        }
    }
}
