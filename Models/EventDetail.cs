using System;
using System.ComponentModel.DataAnnotations;

namespace COMP2084_Project_Eventour.Models
{
    public class EventDetail
    {

        public int EventDetailId {get; set;}

        [Required]
        [Range(0, 10000, ErrorMessage = "Price of an event can be 0 (free) - 10,000")]
        public double Price { get; set; }

        public DateTime eventDate { get; set; }

        [Required]
        // FK for Category item
        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        [Required]
        // FK for EventVenue item
        public int EventVenueId { get; set; }
        public EventVenue? EventVenue { get; set; }

        // add nullable child ref to Product model
        public List<Event>? Events { get; set; }


        
    }
}

