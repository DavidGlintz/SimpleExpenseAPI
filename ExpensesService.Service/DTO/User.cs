using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ExpensesService.Api.Models
{
    public class User
    {
        [JsonIgnore]
        public int IdUser{ get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Currency { get; set; }
        public string FullName => FirstName + " " + LastName; 
    }
}
