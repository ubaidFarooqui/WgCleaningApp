using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WgCleaningApp.Application.DTOs.Auth
{
    public class ForgotPasswordRequest
    {
        public string Email { get; set; } = string.Empty;
        public string WgName { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
}
