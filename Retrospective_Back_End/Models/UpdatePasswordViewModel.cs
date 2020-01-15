using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Retrospective_Back_End.Utils;

namespace Retrospective_Back_End.Models
{
    public class UpdatePasswordViewModel
    {
        [Required(ErrorMessage = MessageConstants.UpdatePasswordRequired)]
        public string Password { get; set; }
    }
}
