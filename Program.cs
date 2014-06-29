using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FIFA_Crawler
{
    class Program
    {
        static void Main(string[] args)
        {

            List<string> urls = new List<string>();
            List<Agent> agents = new List<Agent>();

            using (System.Net.WebClient webClient = new System.Net.WebClient())
            {
                webClient.Headers["User-Agent"] = "Mozilla/4.0 (Compatible; Windows NT 5.1; MSIE 6.0) (compatible; MSIE 6.0; Windows NT 5.1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";
                string uriString = "http://www.fifa.com/aboutfifa/organisation/footballgovernance/playeragents/list.html";
                byte[] responseArray = webClient.DownloadData(uriString);
                string download = Encoding.ASCII.GetString(responseArray);

                int beginning = 0;

                while (download.IndexOf("/aboutfifa/organisation/footballgovernance/playeragents/association=", beginning) > 0)
                {
                    beginning = download.IndexOf("/aboutfifa/organisation/footballgovernance/playeragents/association=", beginning);
                    int end = download.IndexOf("\"", beginning + 1);
                    string newURL = "http://www.fifa.com" + download.Substring(beginning, end - beginning);
                    urls.Add(newURL);
                    beginning = end;
                    //break;
                }
            }

            for (int j = 0; j < urls.Count; j++)
            {
                using (System.Net.WebClient webClient = new System.Net.WebClient())
                {
                    webClient.Headers["User-Agent"] = "Mozilla/4.0 (Compatible; Windows NT 5.1; MSIE 6.0) (compatible; MSIE 6.0; Windows NT 5.1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";
                    string uriString = urls[j];
                    byte[] responseArray = webClient.DownloadData(uriString);
                    string download = Encoding.ASCII.GetString(responseArray);

                    string[] hmtl = Regex.Split(download, "<table class=\"people\">");

                    string toWorkOn = Regex.Split(hmtl[1], "</table>")[0];
                    int off = 0;
                    while (toWorkOn.IndexOf("<tr ", off + 1) > 0)
                    {
                        off = toWorkOn.IndexOf("<tr ", off);
                        int end = toWorkOn.IndexOf("</tr>", off);
                        string newWork = toWorkOn.Substring(off, end - off);

                        off = end;

                        string Name = "";
                        string Company = "";
                        string Address = "";
                        string Other = "";

                        int newBeginning = 0;
                        int newEnd = 0;
                        newBeginning = newWork.IndexOf("<td>", newBeginning + 1) + 4;
                        newEnd = newWork.IndexOf("</td>", newBeginning);
                        Name = newWork.Substring(newBeginning, newEnd - newBeginning);
                        //
                        newBeginning = newWork.IndexOf("<td>", newBeginning + 1) + 4;
                        newEnd = newWork.IndexOf("</td>", newBeginning);
                        Company = newWork.Substring(newBeginning, newEnd - newBeginning);
                        //
                        newBeginning = newWork.IndexOf("<td>", newBeginning + 1) + 4;
                        newEnd = newWork.IndexOf("</td>", newBeginning);
                        Address = newWork.Substring(newBeginning, newEnd - newBeginning);
                        //
                        newBeginning = newWork.IndexOf("<td>", newBeginning + 1) + 4;
                        newEnd = newWork.IndexOf("</td>", newBeginning);
                        Other = newWork.Substring(newBeginning, newEnd - newBeginning);

                        string Email = "";
                        string Telephone = "";
                        string Mobile = "";

                        newBeginning = Other.IndexOf("mailto:", 0) + 7;
                        newEnd = Other.IndexOf("\"", newBeginning);
                        if (newEnd >= 0 && newEnd >= 0)
                            Email = Other.Substring(newBeginning, newEnd - newBeginning);

                        newBeginning = Other.IndexOf("Tel:", 0) + 4;
                        newEnd = Other.IndexOf("<br />", newBeginning);
                        if (newEnd >= 0 && newEnd >= 0)
                            Telephone = Other.Substring(newBeginning, newEnd - newBeginning);

                        newBeginning = Other.IndexOf("Mobile:", 0) + 7;
                        newEnd = Other.IndexOf("<br />", newBeginning);
                        if (newEnd >= 0 && newEnd >= 0)
                            Mobile = Other.Substring(newBeginning, newEnd - newBeginning);

                        agents.Add(new Agent()
                        {
                            Name = Name,
                            Company = Company,
                            Address = Address,
                            Other = Other,
                            Country = uriString.Replace("http://www.fifa.com/aboutfifa/organisation/footballgovernance/playeragents/association=", "").Replace(".html", ""),
                            Email = Email,
                            Telephone = Telephone,
                            Mobile = Mobile
                        });
                        //off = newBeginning;

                        //<a href="mailto:algabo60@yahoo.com">algabo60@yahoo.com</a><br />Tel:+389-70/918 106<br />Mobile:+355-69/406 99 71<br />
                    }
                }
            }


            StringBuilder sb = new StringBuilder();
            foreach (Agent agent in agents)
            {
                sb.Append(agent.Name + "\t" +
                          agent.Company + "\t" +
                          agent.Address + "\t" +
                          agent.Other + "\t" +
                          agent.Email + "\t" +
                          agent.Telephone + "\t" +
                          agent.Mobile + "\t" +
                          agent.Country + "\r\n");
            }
            System.IO.File.WriteAllText("WriteText.txt", sb.ToString());
        }

        public class Agent
        {
            public string Name { get; set; }
            public string Company { get; set; }
            public string Address { get; set; }
            public string Other { get; set; }
            public string Email { get; set; }
            public string Telephone { get; set; }
            public string Mobile { get; set; }

            public string Country { get; set; }
        }
    }
}
