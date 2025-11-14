using System;
using System.Collections.Generic;

namespace WebApp.Models;

public partial class Attachment
{
    public int Attachmentid { get; set; }

    public int Appointmentid { get; set; }

    public string Filename { get; set; } = null!;

    public string Filepath { get; set; } = null!;

    public DateTime? Uploadedat { get; set; }

    public virtual Appointment Appointment { get; set; } = null!;
}
