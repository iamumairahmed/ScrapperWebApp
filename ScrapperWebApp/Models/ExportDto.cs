namespace ScrapperWebApp.Models
{
    public class ExportDto
    {
        //public int?[] AtividadeId { get; set; }
        //public int?[] NatJurId { get; set; }
        //public string? SituacaoCadastral { get; set; }
        //public int? Estado { get; set; }
        //public int? Cep { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateUntil { get; set; }
        //public int? MEIOnly { get; set; }
        //public int? WithPhone { get; set; }
        //public int? WithoutMEI { get; set; }
        //public int? CellOnly { get; set; }
        //public int? WithEmail { get; set; }


        public IEnumerable<int> selectedAtividades { get; set; } = new HashSet<int>() { };
        public IEnumerable<int?> selectedNatJurs { get; set; } = new HashSet<int?>() { };
        public IEnumerable<string> selectedEstado { get; set; } = new HashSet<string>() { };
        public List<string> selectedCep { get; set; } = new List<string>();
        public string selectedSitCad { get; set; }
        //public List<string> selectedEstado { get; set; } = new List<string>();
        public bool somonteMEI { get; set; }
        public bool withPhone { get; set; }
        public bool withEmail { get; set; }
        public bool withoutMEI { get; set; }
        public bool cellOnly { get; set; }
    }
}
