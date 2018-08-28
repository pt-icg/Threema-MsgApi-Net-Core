using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IcgSoftware.Threema.CoreWebApp.Models
{
    public class ThreemaMessageCallback
    {
        public String From { get; set; }
        public String To { get; set; }
        //public byte[] MessageId { get; set; }
        public String MessageId { get; set; }
        public long Date { get; set; }
        //public byte[] Nonce { get; set; }
        //public byte[] Box { get; set; }
        //public byte[] Mac { get; set; }
        public String Nonce { get; set; }
        public String Box { get; set; }
        public String Mac { get; set; }
        public String Nickname { get; set; }

        //public DateTime LocalTime => DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(Date)).DateTime.ToLocalTime();
        public DateTime LocalTime => DateTimeOffset.FromUnixTimeSeconds(Date).DateTime.ToLocalTime();
    

        public override String ToString()
        {
            return String.Format("F: {0} | T: {1} | D: {2} | N: {3} | MId: {4} | Nonce: {5} | Box: {6} | Mac: {7}", From, To, LocalTime, Nickname, MessageId, Nonce, Box, Mac);
        }
    }

}
