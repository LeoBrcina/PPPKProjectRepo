using System;
using System.Collections.Generic;

namespace WebApp.Models;

public partial class Appointmenttype
{
    public int Appointmenttypeid { get; set; }

    public string? Appointmenttypename { get; set; }

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}
