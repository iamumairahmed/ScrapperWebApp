namespace ScrapperWebApp.Models
{
    public class AtividadePrincipal
    {
        public string codigo { get; set; }
        public string descricao { get; set; }
    }

    public class Cnpj
    {
        public string cnpj { get; set; }
        public string cnpj_raiz { get; set; }
        public int filial_numero { get; set; }
        public AtividadePrincipal atividade_principal { get; set; }
        public string bairro { get; set; }
        public bool cnpj_mei { get; set; }
        public DateTime data_abertura { get; set; }
        public string logradouro { get; set; }
        public string municipio { get; set; }
        public string nome_fantasia { get; set; }
        public string numero { get; set; }
        public string razao_social { get; set; }
        public string situacao_cadastral { get; set; }
        public string uf { get; set; }
        public string versao { get; set; }
    }

    public class Response
    {
        public Data data { get; set; }
        public Page page { get; set; }
        public bool success { get; set; }
    }

    public class Data
    {
        public Cnpj[] cnpj { get; set; }
        public int count { get; set; }
    }
    public class Page
    {
        public int current { get; set; }
    }
}
