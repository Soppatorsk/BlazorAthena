﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BlazorAthenaFrontend.Models
{

    public class GetUserRoleModel
    {
        [Required(ErrorMessage = "UserId is required.")]
        public string UserId { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Roles are required.")]
        public List<string> Roles { get; set; }
    }
}
