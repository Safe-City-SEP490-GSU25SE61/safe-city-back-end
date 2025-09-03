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
        [Required(ErrorMessage = "Quận ID là bắt buộc.")]
        public int DistrictId { get; set; }

        [Required(ErrorMessage = "Account ID là bắt buộc.")]
        public Guid AccountId { get; set; }
    }
}
