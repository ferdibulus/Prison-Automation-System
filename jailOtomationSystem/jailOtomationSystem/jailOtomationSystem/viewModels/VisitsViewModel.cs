using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using jailOtomationSystem.Models;

namespace jailOtomationSystem.viewModels
{
    public class VisitsViewModel
    {
        public List<System.TimeSpan?> availableAppointments { get; set; }
        public visit visit { get; set; }
        public System.TimeSpan? selectedAppointment { get; set; }
    }
}