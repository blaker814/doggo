using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DogGo.Models.ViewModels
{
    public class RequestWalkViewModel
    {
        public Walker Walker { get; set; }

        [Required()]
        public Walk Walk { get; set; }
        
        [Required()]
        public List<Dog> Dogs { get; set; }
    }
}
