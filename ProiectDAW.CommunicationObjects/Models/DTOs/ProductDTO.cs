using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProiectDAW.CommunicationObjects.Models.DTOs
{
    public class ProductDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public float Price { get; set; }
        public string Description { get; set; }
        public CategoryDTO Category { get; set; }
        public byte[] Image { get; set; }
    }
}
