using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMSProj.Dtos
{
    public class BookModel
    {
        public int BookID { get; set; }
        public string Title { get; set; } 
        public string Author { get; set; }
        public int ISBN { get; set; }
        public string PublicationYear { get; set; }
        public int CategoryID { get; set; }
        public int TotalCopies { get; set; }
        public int AvailableCopies { get; set; }

    }
}
