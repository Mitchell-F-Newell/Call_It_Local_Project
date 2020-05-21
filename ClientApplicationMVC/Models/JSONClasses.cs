using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClientApplicationMVC.Models
{
    public class Review
    {
        public string companyName { get; set; }
        public string username { get; set; }
        public string review { get; set; }
        public int stars { get; set; }
        public int timestamp { get; set; }

    }

    public class CompanyName
    {
        public string companyName { get; set;}
    }

    public class Reviews
    {
        public Review review { get; set; }
    }

    public class Response
    {
        public string response { get; set; }
    }

    public class GetResponse
    {
        public GetResponse (int i)
        {
            reviews = new Review[i];
        }
        public string response { get; set; }
        public Review[] reviews { get; set; }
    }
}