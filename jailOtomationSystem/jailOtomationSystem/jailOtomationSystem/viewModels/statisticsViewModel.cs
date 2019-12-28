using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using jailOtomationSystem.viewModels;

namespace jailOtomationSystem.viewModels
{
    public class statisticsViewModel
    {
        public int totalOfficers { get; set; }
        public int totalPrisoners { get; set; }
        public int prisonCapacity { get; set; }
        public int availableOfficersJobs { get; set; }
    }
}