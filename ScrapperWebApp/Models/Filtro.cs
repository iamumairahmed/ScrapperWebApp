﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ScrapperWebApp.Models;

public partial class Filtro
{
    public string NoCep { get; set; }

    public DateTime DtInicial { get; set; }

    public DateTime DtFinal { get; set; }

    public int? NoContador { get; set; }

    public DateTime? DtExecucao { get; set; }
    public string CdMei { get; set; }
}