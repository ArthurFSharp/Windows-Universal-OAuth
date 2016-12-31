using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Security.Authentication.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using AppProviderOAuth.Models;

namespace AppProviderOAuth.ViewModels
{
    public class GoogleScenarioViewModel : INotifyPropertyChanged
    {
        public GoogleScenarioViewModel()
        {
            ConnectToGoogleCommand = new RelayCommand(async () => await ConnectToGoogle());
        }

        #region Properties

        private GoogleUser _user;
        public GoogleUser User
        {
            get { return _user; }
            set
            {
                if (_user != value)
                {
                    _user = value;
                    RaisePropertyChanged("User");
                }
            }
        }

        #endregion

        #region Commands

        public ICommand ConnectToGoogleCommand { get; private set; }

        #endregion


        #region Private members

        private async Task ConnectToGoogle()
        {
#if WINDOWS_APP
            var auth = await WebAuthenticationBroker.AuthenticateAsync(WebAuthenticationOptions.UseTitle, Configs.GoogleConfig.GoogleStartUri, Configs.GoogleConfig.GoogleEndUri);
            Debug.WriteLine(auth.ResponseData);

            var successCode = GetGoogleSuccessCode(auth.ResponseData);
            if (successCode != null)
            {
                var token = await GetToken(successCode);
                Debug.WriteLine("token : " + token);
                await SetInfos(token);
            }
#else
            WebAuthenticationBroker.AuthenticateAndContinue(Configs.GoogleConfig.GoogleStartUri, Configs.GoogleConfig.GoogleEndUri, null, WebAuthenticationOptions.UseTitle);
#endif
        }

        public async void ContinueWebAuthentication(Windows.ApplicationModel.Activation.WebAuthenticationBrokerContinuationEventArgs args)
        {
            String resp = args.WebAuthenticationResult.ResponseData;
            Debug.WriteLine(resp);

            var successCode = GetGoogleSuccessCode(resp);
            if (successCode != null)
            {
                var token = await GetToken(successCode);
                Debug.WriteLine("token : " + token);
                await SetInfos(token);
            }
        }

        private string GetGoogleSuccessCode(string data)
        {
            if (string.IsNullOrEmpty(data)) return null;
            var parts = data.Split('=');
            for (int i = 0; i < parts.Length; ++i)
            {
                if (parts[i] == "Success code")
                {
                    return parts[i + 1];
                }
            }
            return null;
        }

        private async Task<string> GetToken(string code)
        {
            var client = new HttpClient();
            var auth = await client.PostAsync("https://accounts.google.com/o/oauth2/token", new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("code", code),
                new KeyValuePair<string, string>("client_id", Configs.GoogleConfig.AppId),
                new KeyValuePair<string, string>("client_secret", Configs.GoogleConfig.AppSecret),
                new KeyValuePair<string, string>("grant_type","authorization_code"),
                new KeyValuePair<string, string>("redirect_uri","urn:ietf:wg:oauth:2.0:oob"),
            }));

            var data = await auth.Content.ReadAsStringAsync();
            Debug.WriteLine(data);
            var j = JToken.Parse(data);
            var token = j.SelectToken("access_token");
            return token.ToString();
        }

        private async Task SetInfos(string token)
        {
            HttpClient client = new HttpClient();
            var infos = await client.GetAsync($"https://www.googleapis.com/oauth2/v1/userinfo?alt=json&access_token={token}");
            var data = await infos.Content.ReadAsStringAsync();
            User = JsonConvert.DeserializeObject<GoogleUser>(data);
        }

        #endregion

        #region INotifyPropertyChanged imlementation

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
