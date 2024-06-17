﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using ScrapperWebApp.Data;
using System;
using System.Collections.Generic;

namespace ScrapperWebApp.Models;

public partial class Empresa
{
    public long NoCnpj { get; set; }

    public string CdRzsocial { get; set; }

    public string CdFantasia { get; set; }

    public string CdTipo { get; set; }

    public string CdSituacao { get; set; }

    public DateTime? DtSituacao { get; set; }

    public int? VlCapsocial { get; set; }

    public int? NoNatjur { get; set; }

    public string CdMei { get; set; }

    public string CdLogra { get; set; }

    public string CdNumero { get; set; }

    public string NoCep { get; set; }

    public string NoFone { get; set; }

    public string CdEmail { get; set; }

    public DateTime? DtAbertura { get; set; }

    public virtual ICollection<EmpAtividade> EmpAtividades { get; set; } = new List<EmpAtividade>();

    public virtual NatJur NoNatjurNavigation { get; set; }

    public virtual ICollection<Socio> Socios { get; set; } = new List<Socio>();

    public virtual ICollection<Telefone> Telefones { get; set; } = new List<Telefone>();
    public virtual ICollection<UraError> UraErrors { get; set; } = new List<UraError>();

}