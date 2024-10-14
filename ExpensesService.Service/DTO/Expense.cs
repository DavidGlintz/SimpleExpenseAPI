using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.CompilerServices;

namespace ExpensesService.Api.Models
{
    public class Expense
    {
        [JsonIgnore]
        public int IdExpense { get; set; }
        public int IdUser { get; set; }
        
        [JsonIgnore]
        public User? User { get; set; }
        public DateTime Date { get; set; }
        public string Type { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string Comment { get; set; }
    }
}
