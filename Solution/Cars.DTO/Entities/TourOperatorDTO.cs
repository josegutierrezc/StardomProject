using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cars.DTO
{
    public class TourOperatorDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public Nullable<bool> IncludesInsurance { get; set; }
        public Nullable<int> ParentId { get; set; }
        public Nullable<int> ImageId { get; set; }
    }
}
