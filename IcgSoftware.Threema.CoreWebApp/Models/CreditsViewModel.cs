using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IcgSoftware.Threema.CoreWebApp.Models
{
    public class CreditsViewModel
    {
        public int? Credits { get; set; }
        public String CreditsText => Credits.HasValue ? String.Format("Remaining credits: {0}", Credits.Value) : "Error fetching credits";
        public String ErrorMessage { get; set; }
    }
}