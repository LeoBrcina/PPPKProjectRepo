using System;
using System.Collections.Generic;

namespace WebApp.Models;

public partial class Prescription
{
    public int Prescriptionid { get; set; }

    public int Patientid { get; set; }

    public int Medicineid { get; set; }  

    public virtual Medicine Medicine { get; set; } = null!;

    public virtual Patient Patient { get; set; } = null!;
}
