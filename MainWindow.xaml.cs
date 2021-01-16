using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;  
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CefSharp;
using CefSharp.Wpf;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace ms_Login
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        public class xbox_1
        {
            public class Xui
            {public string uhs { get; set; }}
            public class DisplayClaims
            {
                public List<Xui> xui { get; set; }
            }

            public class Root
            {
                public string IssueInstant { get; set; }
                public string NotAfter { get; set; }
                public string Token { get; set; }
                public DisplayClaims DisplayClaims { get; set; }
            }

        }
        public class ms_json{
            public class Root
            {
            public string token_type { get; set; }
            public int expires_in { get; set; }
            public string scope { get; set; }
            public string access_token { get; set; }
            public string refresh_token { get; set; }
            public string user_id { get; set; }
            public string foci { get; set; }
            }

        }
        public static string JsonPost(string url, string xmlString)

        {
            HttpContent httpContent = new StringContent(xmlString);
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpClient httpClient = new HttpClient();



            HttpResponseMessage response = httpClient.PostAsync(url, httpContent).Result;

            if (response.IsSuccessStatusCode)
            {
                Task<string> t = response.Content.ReadAsStringAsync();
                return t.Result;
            }
            return string.Empty;
        }
        public static string Post(string url, string xmlString)

        {
            HttpContent httpContent = new StringContent(xmlString);
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
            HttpClient httpClient = new HttpClient();



            HttpResponseMessage response = httpClient.PostAsync(url, httpContent).Result;

            if (response.IsSuccessStatusCode)
            {
                Task<string> t = response.Content.ReadAsStringAsync();
                return t.Result;
            }
            return string.Empty;
        }
        private void Browser_AddressChanged(object sender, AddressChangedEventArgs e) 
        { 
            this._UUID.Content = e.Address; 
        }
        string livekey,liveurl;
        ChromiumWebBrowser MyBrowser = new ChromiumWebBrowser("https://login.live.com/oauth20_authorize.srf?client_id=00000000402b5328&response_type=code&scope=service%3A%3Auser.auth.xboxlive.com%3A%3AMBI_SSL&redirect_uri=https%3A%2F%2Flogin.live.com%2Foauth20_desktop.srf");
        private void get_Click(object sender, RoutedEventArgs e)
        {
            _static.Content = "请在弹出的窗口中继续操作...... Now:get livecode";
            a1.Children.Add(MyBrowser);
            MyBrowser.AddressChanged += MyBrowser_AddressChanged;
           _UUID.Content= MyBrowser.Address;
            
        }
        
        private void nextstep()
        {

            string[] livecode_1 = liveurl.Split('=');
            string[] livecode_2= livecode_1[1].Split('&');
            livekey = livecode_2[0];
            _UUID.Content = livekey;
            _static.Content = "已经获得LiveCode，准备进行ms验证";
            string next = "client_id=00000000402b5328&code=" + livekey + "&grant_type=authorization_code&redirect_uri=https://login.live.com/oauth20_desktop.srf";
            string msjson=Post("https://login.live.com/oauth20_token.srf", next);
            ms_json.Root msjson_rt = JsonConvert.DeserializeObject<ms_json.Root>(msjson);
            string ms_access_token = msjson_rt.access_token;
            _UUID.Content = ms_access_token;
            _static.Content = "已获取到ms_access_token，即将执行验证XBOX LIVE登录key";
            next = " {\"Properties\":{\"AuthMethod\":\"RPS\",\"SiteName\":\"user.auth.xboxlive.com\",\"RpsTicket\":\"d=" + ms_access_token + "\"},\"RelyingParty\":\"http://auth.xboxlive.com\",\"TokenType\": \"JWT\" }";
            string xboxlive_1= JsonPost("https://user.auth.xboxlive.com/user/authenticate", next);
            MessageBox.Show(xboxlive_1);
            xbox_1.Root xbox_S1= JsonConvert.DeserializeObject<xbox_1.Root>(xboxlive_1);
            _UUID.Content = xbox_S1.Token;

        }
        private void MyBrowser_AddressChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            this._UUID.Content = MyBrowser.Address;
            liveurl=MyBrowser.Address;
            if (liveurl.Contains("https://login.live.com/oauth20_desktop.srf?code="))
               nextstep();
        }
    }
}
