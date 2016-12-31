using System;
using System.Collections.Generic;
using System.Text;

namespace AppProviderOAuth.Configs
{
    internal class GoogleConfig
    {
        internal static string AppId = "1090963777510-s5imloi4fa2lhc8nkkiubrt2sskhjo22.apps.googleusercontent.com"; // Your Google app id
        internal static string AppSecret = "Y1eQglL5P6N5scWoMcHtXhiS"; // Your Google app secret
        internal static Uri GoogleStartUri =
            new Uri("https://accounts.google.com/o/oauth2/auth?client_id=" + 
                Uri.EscapeDataString(AppId) + 
                "&redirect_uri=" + 
                Uri.EscapeDataString("urn:ietf:wg:oauth:2.0:oob") +
                "&response_type=code&scope=" + 
                Uri.EscapeDataString("profile https://www.googleapis.com/auth/plus.login https://www.googleapis.com/auth/plus.me email"));
        internal static Uri GoogleEndUri = new Uri("https://accounts.google.com/o/oauth2/approval?");
    }
}
