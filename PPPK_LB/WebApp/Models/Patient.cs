using System;
using System.Collections.Generic;

namespace WebApp.Models;

public partial class Patient
{
    public int Patientid { get; set; }

    public string Firstname { get; set; } = null!;

    public string Lastname { get; set; } = null!;

    public string Oib { get; set; } = null!;

    public DateTime Dateofbirth { get; set; }

    public char Gender { get; set; }

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual ICollection<Medicalrecord> Medicalrecords { get; set; } = new List<Medicalrecord>();

    public virtual ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
}
