using PaymentApp.Entities.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace PaymentApp.Entities.Payment
{
    public class Exam : BaseEntity<int>
    {
        public string Title { get; set; } = null!;
        public long Amount { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
