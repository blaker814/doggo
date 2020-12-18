using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DogGo.Models
{
    public class Walk
    {
        public int Id { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy hh:mmtt}")]
        public DateTime Date { get; set; }
        public int Duration { get; set; }

        [DisplayName("Walker")]
        public int WalkerId { get; set; }
        public Walker Walker { get; set; }

        [DisplayName("Dog")]
        public int DogId { get; set; }
        public Dog Dog { get; set; }

        [DisplayName("WalkStatus")]
        public int WalkStatusId { get; set; }
        public WalkStatus WalkStatus { get; set; }
    }
}
