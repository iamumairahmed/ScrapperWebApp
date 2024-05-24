namespace ScrapperWebApp.Models
{
    public class Person
    {
        public string Telefone { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string Cnpj { get; set; }
        public string Razao { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
    }
}
