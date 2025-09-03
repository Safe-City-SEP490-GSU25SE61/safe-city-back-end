using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs.RequestModels
{
    using System.ComponentModel.DataAnnotations;

    public class AddInternalNoteRequestModel
    {
      

        [Required]
        [StringLength(1000, MinimumLength = 5, ErrorMessage = "Note must be between 5 and 1000 characters.")]
        public string Content { get; set; }
    }

}
