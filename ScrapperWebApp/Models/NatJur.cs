﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace ScrapperWebApp.Models;

public partial class NatJur
{
    public int NoNatjur { get; set; }

    public string DsNatjur { get; set; }

    public virtual ICollection<Empresa> Empresas { get; set; } = new List<Empresa>();
}