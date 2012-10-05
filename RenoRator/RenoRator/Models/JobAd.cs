using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RenoRator.Models
{
    public class JobAd
    {
        private renoRatorDBEntities _db;
        public int id { get; private set; }
        public User user { get; private set; }
        public Address address { get; private set; }
        public string tags { get; private set; }
        public string description { get; private set; }
        public Gallery gallery { get; private set; }
        public DateTime targetEndDate { get; private set; }
        public bool active { get; private set; }
    }
}