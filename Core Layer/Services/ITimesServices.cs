﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core_Layer.Services
{
    public interface ITimesServices
    {
        IActionResult AddDayTimes(int dayId, List<String> times);
        IActionResult DeleteAppointment(int timeId);
        IActionResult EditAppointment(int timeId, string newTime);
    }
}
