using System;
using System.ComponentModel.DataAnnotations;

namespace COMP2084_Project_Eventour.Models
{
    public class EventVenue
    {
        public int EventVenueId { get; set; }

        [Required]
        public string? Country { get; set; }

        public string? City { get; set; }

        [Required]
        public string? State { get; set; }

        public string? Address { get; set; }

        public string? Zip { get; set; }

        public EventType Type { get; set; }

        public enum EventType
        {
            // items of enum
            Remote,
            Hybrid,
            InPerson
        }

        // add nullable child ref to EventDetail model
        public List<EventDetail>? EventDetails { get; set; }


    }
}

