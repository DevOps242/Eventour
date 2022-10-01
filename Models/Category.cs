using System;
using System.ComponentModel.DataAnnotations;

namespace COMP2084_Project_Eventour.Models
{
    public class Category
    {
        public int CategoryId { get; set; }

        [Required]
        public string? Name { get; set; }

        [Required]
        [MaxLength(100)]
        public string? Description { get; set; }

        // add nullable child ref to Event Detail model
        public List<EventDetail>? EventDetails { get; set; }

    }
}

