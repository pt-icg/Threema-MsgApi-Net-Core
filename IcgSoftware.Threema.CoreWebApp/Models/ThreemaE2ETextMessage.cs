using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IcgSoftware.Threema.CoreWebApp.Models
{
    public class ThreemaE2ETextMessage
    {
        public String Text { get; set; }
        public String To { get; set; }

        public override String ToString()
        {
            return String.Format("To: {0} | Text: {1}", To, Text);
        }
    }

}
