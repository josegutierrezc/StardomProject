//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Cars.DL
{
    using System;
    using System.Collections.Generic;
    
    public partial class CarCategory
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CarTransmissionId { get; set; }
        public double InsurancePremium { get; set; }
        public Nullable<int> IndexOrder { get; set; }
    }
}
