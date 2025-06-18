using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs.RequestModels
{
    public class AssignDistrictToOfficerDTO
    {
        [Required(ErrorMessage = "District ID is required.")]
        public int DistrictId { get; set; }

        [Required(ErrorMessage = "Account ID is required.")]
        public Guid AccountId { get; set; }
    }
}
