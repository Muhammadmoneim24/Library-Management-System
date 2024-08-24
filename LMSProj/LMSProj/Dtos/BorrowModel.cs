using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMSProj.Dtos
{
    public class BorrowModel
    {
        public int BorrowID { get; set; }
        public int BookID { get; set; }
        public int MemberID { get; set; }

        public string BorrowDate { get; set; }
        public string DueDate { get; set; }
        public string ReturnDate { get; set; }

        public int? AvailableCopies { get; set; }



    }
}
