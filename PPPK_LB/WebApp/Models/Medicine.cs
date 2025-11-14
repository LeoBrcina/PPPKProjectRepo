using System;
using System.Collections.Generic;

namespace WebApp.Models;

public partial class Medicine
{
    public int Medicineid { get; set; }

    public string? Medicinename { get; set; }

    public virtual ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
}
