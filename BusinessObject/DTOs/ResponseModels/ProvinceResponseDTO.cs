using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs.ResponseModels
{
    public class ProvinceResponseDTO
    {
        public int id { get; set; }
        public string Name { get; set; }
    }

    public class ProvinceDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<CommuneForCitizenDTO> Communes { get; set; }
    }
}
