using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Web;
using Json;
using System.Net.Http;

namespace FapiaoReview
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public class Config
        {
            public string user { get; set; }
            public string password { get; set; }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            test();
        }

        public static void test()
        {
            DirectoryInfo dirInfo = new DirectoryInfo("data");
            var accessToken = getAccessToken();
            foreach (var fileInfo in dirInfo.GetFiles())
            {
                var image = File.ReadAllBytes(fileInfo.FullName);
                MikValSor.Encoding.Base64 base64 = new MikValSor.Encoding.Base64(image);
                //var str = System.Web.HttpUtility.UrlEncode(base64.ToByteArray());
                getInvoice(accessToken, base64.ToString());
                getPic(accessToken, base64.ToString());
            }
            
        }

        public static string getAccessToken()
        {
            string authHost = "https://aip.baidubce.com/oauth/2.0/token";
            HttpClient client = new HttpClient();
            var APP_ID = "11254584";
            var API_KEY = "qQvhq1t0mH3lDaZfq1L4gEh1";
            var SECRET_KEY = "LuBmrxINLTGfW8kcAtuXd5vNZuOtgvYN";
            List<KeyValuePair<string, string>> paraList = new List<KeyValuePair<string, string>>();
            paraList.Add(new KeyValuePair<string, string>("grant_type", "client_credentials"));
            paraList.Add(new KeyValuePair<string, string>("client_id", API_KEY));
            paraList.Add(new KeyValuePair<string, string>("client_secret", SECRET_KEY));

            HttpResponseMessage response = client.PostAsync(authHost, new FormUrlEncodedContent(paraList)).Result;
            string result = response.Content.ReadAsStringAsync().Result;
            return result;
        }

        public static string getPic(string accessToken, string image)
        {
            string authHost = "https://aip.baidubce.com/rest/2.0/ocr/v1/general_basic";
            HttpClient client = new HttpClient();
            List<KeyValuePair<string, string>> paraList = new List<KeyValuePair<string, string>>();
            paraList.Add(new KeyValuePair<string, string>("access_token", accessToken));
            paraList.Add(new KeyValuePair<string, string>("image", image));

            HttpResponseMessage response = client.PostAsync(authHost, new FormUrlEncodedContent(paraList)).Result;
            string result = response.Content.ReadAsStringAsync().Result;
            Console.WriteLine(result);
            return result;
        }

        public static string getInvoice(string accessToken, string image)
        {
            string authHost = "https://aip.baidubce.com/";
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(authHost);
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));

            var req_body = "{\"access_token\":\"" +accessToken +  "\", \"image\":\"" + image + "\"}";
            HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Post, "/rest/2.0/ocr/v1/vat_invoice");
            //HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Post, "rest/2.0/ocr/v1/general_basic");
            req.Content = new StringContent(req_body, Encoding.UTF8, "application/x-www-form-urlencoded");

            client.SendAsync(req).ContinueWith(resp =>
            {
                Console.WriteLine("11111");
                Console.WriteLine(resp.Result.ToString());
                var s = resp.Result.Content.ReadAsStringAsync();
                var p = s.GetAwaiter().GetResult();
                Console.WriteLine(p);
            });
            return "";
        }
    }
}
