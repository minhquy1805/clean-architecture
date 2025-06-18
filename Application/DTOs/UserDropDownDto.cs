using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class UserDropDownDto
    {
        public int UserId { get; set; }
        public string FullName { get; set; } = default!;
    }
}
