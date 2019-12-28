using jailOtomationSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace jailOtomationSystem.viewModels
{
    public class cellViewModel
    {
        public List<Prisoner> prisoners { get; set; }
        public prisonerCell cell { get; set; }
    }
}