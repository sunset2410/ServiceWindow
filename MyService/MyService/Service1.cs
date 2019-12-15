using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace MyService
{
    public partial class Service1 : ServiceBase
    {
        Timer timer = new Timer();
        public string htmlString { get; private set; }
        float priceIOTA, priceXRP, priceICX, priceXLM = 0;

        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            WriteToFile("Service is start at " + DateTime.Now);
            timer.Elapsed += new ElapsedEventHandler(OnElapsedTime);
            timer.Interval = 10000; //number in milisecinds  
            timer.Enabled = true;

        }

        protected override void OnStop()
        {
            WriteToFile("Service is recall at " + DateTime.Now);
        }


        private void OnElapsedTime(object source, ElapsedEventArgs e)
        {
            WriteToFile("Service is recall at " + DateTime.Now);

            HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create("https://finance.yahoo.com/cryptocurrencies?count=100&offset=0");
            myRequest.Method = "GET";
            WebResponse myResponse = myRequest.GetResponse();
            StreamReader sr = new StreamReader(myResponse.GetResponseStream(), Encoding.UTF8);
            htmlString = sr.ReadToEnd();
            sr.Close();
            myResponse.Close();

            //Stellar
            priceXLM = parserKey("Stellar");
            //XRP
            priceXRP = parserKey("XRP");
            //ICON
            priceICX = parserKey("ICON");
            //IOTA
            priceIOTA = parserKey("IOTA");

        }

        public float parserKey(string key)
        {
            string test = "title=\"" + key + " USD\"";
            int start = htmlString.IndexOf("title=\"" + key + " USD\"");
            start = htmlString.IndexOf("<span", start);
            start = htmlString.IndexOf(">", start) + 1;
            int end = htmlString.IndexOf("</span>", start);
            string price = htmlString.Substring(start, end - start);
            float priceC;
            float.TryParse(price, out priceC);
            return priceC;
        }

        public void WriteToFile(string Message)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\Logs";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\ServiceLog_" + DateTime.Now.Date.ToShortDateString().Replace('/', '_') + ".txt";
            if (!File.Exists(filepath))
            {
                // Create a file to write to.   
                using (StreamWriter sw = File.CreateText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
        }

    }
}


