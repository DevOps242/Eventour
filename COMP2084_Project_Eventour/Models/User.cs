using System;
using System.ComponentModel.DataAnnotations;

namespace COMP2084_Project_Eventour.Models
{
    public class User
    {
        public int UserId { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? BusinessName { get; set; }

        [Required]
        public string? Email { get; set; }

        [Required]
        public string? Password { get; set; }

        public enum UserType
        {
            // Add items
            Host,
            General
        }

        // add nullable child ref to Event model
        public List<Event>? Events { get; set; }
    }
}

