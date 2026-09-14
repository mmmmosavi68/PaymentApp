using PaymentApp.Entities.Common;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace PaymentApp.Entities.User
{
    public class UserField : BaseEntity<int>
    {
        public  Guid UserID { get; set; }

        [ForeignKey(nameof(UserID))]
        public User Users { get; set; }

        public int ExamFieldID { get; set; }
    }
}
