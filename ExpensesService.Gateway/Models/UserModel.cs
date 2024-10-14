using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpensesService.Api.Models
{
    [Table("user")]
    public class UserModel
    {
        [Key]
        [Column("id_user")]
        public int IdUser{ get; set; }
        [Column("first_name")]
        public string FirstName { get; set; }
        [Column("last_name")]
        public string LastName { get; set; }
        [Column("currency")]
        public string Currency { get; set; }
        public virtual ICollection<ExpenseModel> Expenses { get; set; }
        public string FullName => FirstName + " " + LastName;
    }
}
