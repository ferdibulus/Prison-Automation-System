using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using jailOtomationSystem.Models;

namespace jailOtomationSystem.viewModels
{
    public class prisonerDetails
    {
        public Prisoner prisoner { get; set; }
        public prisonerCell cell { get; set; }
        public Job job { get; set; }
        public Adjudication adjudication { get; set; }

    }
}