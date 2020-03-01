using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ASPNETConnectionTester.Models
{
    public class Tester
    {
        public string Url { set; get;}

        public List<Dictionary<string,string>> Headers { set; get; }
    }
}