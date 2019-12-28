using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using jailOtomationSystem.Models;

namespace jailOtomationSystem.viewModels
{
    public class OfficerAssignmentData
    {
        public officer officer { get; set; }
        public List<Job> Availablejobs { get; set; }
        public Job selecttedJob { get; set; }
    }
}