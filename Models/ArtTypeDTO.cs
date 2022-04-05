using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Project1.Models
{
    public class ArtTypeDTO
    {
        //public ArtTypeDTO()
        //{
        //    this.Artworks = new HashSet<ArtworkDTO>();
        //}
        public int ID { get; set; }

        
        [Required(ErrorMessage = "You cannot leave the art type name black.")]
        [StringLength(25, ErrorMessage = "Art type cannot be more than 25 characters long.")]
        public string Type { get; set; }

        [Timestamp]
        public Byte[] RowVersion { get; set; }


        public ICollection<ArtworkDTO> Artworks { get; set; }
    }
}
