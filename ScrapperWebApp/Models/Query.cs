namespace ScrapperWebApp.Models
{
    using System.Collections.Generic;

    public class Query
    {
        public List<string> termo { get; set; } = new List<string>();
        public List<string> atividade_principal { get; set; } = new List<string>();
        public List<string> natureza_juridica { get; set; } = new List<string>();
        public List<string> uf { get; set; } = new List<string>();
        public List<string> municipio { get; set; } = new List<string>();
        public List<string> bairro { get; set; } = new List<string>();
        public string situacao_cadastral { get; set; }
        public List<string> cep { get; set; } = new List<string>();
        public List<string> ddd { get; set; } = new List<string>();
    }

    public class RangeQuery
    {
        public DateRange data_abertura { get; set; } = new DateRange();
        public NumberRange capital_social { get; set; } = new NumberRange();
    }

    public class DateRange
    {
        public string lte { get; set; }
        public string gte { get; set; }
    }

    public class NumberRange
    {
        public string lte { get; set; }
        public string gte { get; set; }
    }

    public class Extras
    {
        public bool somente_mei { get; set; }
        public bool excluir_mei { get; set; }
        public bool com_email { get; set; }
        public bool incluir_atividade_secundaria { get; set; }
        public bool com_contato_telefonico { get; set; }
        public bool somente_fixo { get; set; }
        public bool somente_celular { get; set; }
        public bool somente_matriz { get; set; }
        public bool somente_filial { get; set; }
    }

    public class RequestObject
    {
        public Query query { get; set; } = new Query();
        public RangeQuery range_query { get; set; } = new RangeQuery();
        public Extras extras { get; set; } = new Extras();
        public int page { get; set; }
    }

}
