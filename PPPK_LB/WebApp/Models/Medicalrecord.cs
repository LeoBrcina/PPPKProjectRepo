using System;
using System.Collections.Generic;

namespace WebApp.Models
{
    public partial class Medicalrecord
    {
        public int Recordid { get; set; }

        public int Patientid { get; set; }

        public DateOnly Startdate { get; set; }

        public DateOnly? Enddate { get; set; }

        public int Diseaseid { get; set; } 

        public virtual Disease Disease { get; set; } = null!; 

        public virtual Patient Patient { get; set; } = null!;
    }
}
