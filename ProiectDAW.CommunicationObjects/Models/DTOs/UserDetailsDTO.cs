using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProiectDAW.CommunicationObjects.Models.DTOs
{
    public class UserDetailsDTO
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Address { get; set; }
        public string PostalCode { get; set; }
    }
}
