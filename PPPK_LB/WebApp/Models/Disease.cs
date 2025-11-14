using System;
using System.Collections.Generic;

namespace WebApp.Models;

public partial class Disease
{
    public int Diseaseid { get; set; }

    public string? Diseasename { get; set; }

    public virtual ICollection<Medicalrecord> Medicalrecords { get; set; } = new List<Medicalrecord>();
}
