﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable

namespace ScrapperWebApp.Models;

public partial class UraError
{
    public int NoUraErr { get; set; }

    public string NoFone1 { get; set; }

    public string DsSocio { get; set; }

    public string CdRzsocial { get; set; }

    public string CdEmail { get; set; }

    public long? NoCnpj { get; set; }

    public string CdErrors { get; set; }

    public string NoFone2 { get; set; }

    public string NoFone3 { get; set; }

    public string NoFone4 { get; set; }

    public virtual Empresa NoCnpjNavigation { get; set; }
}