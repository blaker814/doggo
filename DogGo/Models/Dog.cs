using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace DogGo.Models
{
    public class Dog
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Breed { get; set; }
        public string Notes { get; set; }
        public string ImageUrl { get; set; }

        [DisplayName("Owner")]
        public int OwnerId { get; set; }
        public Owner Owner { get; set; }
    }
}
