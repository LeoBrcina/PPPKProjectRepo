using System;
using System.Collections.Generic;

namespace WebApp.Models;

public partial class Appointment
{
    public int Appointmentid { get; set; }

    public int Patientid { get; set; }

    public DateTime Appointmentdate { get; set; }

    public int Appointmenttypeid { get; set; }

    public virtual Appointmenttype Appointmenttype { get; set; } = null!;

    public virtual ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();

    public virtual Patient Patient { get; set; } = null!;
}
