using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ExpensesService.Api.Models
{
    [Table("expense")]
    public class ExpenseModel
    {
        [Key]
        [Column("id_expense")]
        public int IdExpense { get; set; }
        [ForeignKey("user_fk")]
        [Column("id_user")]
        public int IdUser { get; set; }
        [Column("date")]
        public DateTime Date { get; set; }
        [Column("type")]
        public string Type { get; set; }
        [Column("amount")]
        public decimal Amount { get; set; }
        [Column("currency")]
        public string Currency { get; set; }
        [Column("comment")]
        public string Comment { get; set; }
        public UserModel? User { get; set; }
    }
}
