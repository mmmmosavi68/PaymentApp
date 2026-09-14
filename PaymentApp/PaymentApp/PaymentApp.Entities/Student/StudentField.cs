using PaymentApp.Entities.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace PaymentApp.Entities.Student
{
    public class StudentField : BaseEntity<int>
    {
        public int RegId { get; set; }

        public int StudentId { get; set; }
        [ForeignKey(nameof(StudentId))]
        public Student Student { get; set; }

        public int ExamFieldId { get; set; }
    }
}
