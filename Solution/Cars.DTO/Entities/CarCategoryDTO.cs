using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cars.DTO
{
    public class CarCategoryDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CarTransmissionId { get; set; }
        public double InsurancePremium { get; set; }
        public Nullable<int> IndexOrder { get; set; }
    }
}
