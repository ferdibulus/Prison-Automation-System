using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using jailOtomationSystem.Models;

namespace jailOtomationSystem.viewModels
{
    public class OfficerDetails
    {
       public officer officer { get; set; }
       public officerRoom room { get; set; }
       public Job job { get; set; }
       
    }
}