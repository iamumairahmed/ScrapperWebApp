﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace ScrapperWebApp.Models;

public partial class ScrapperDbContext : DbContext
{
    public ScrapperDbContext(DbContextOptions<ScrapperDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Atividade> Atividades { get; set; }

    public virtual DbSet<Cep> Ceps { get; set; }

    public virtual DbSet<EmpAtividade> EmpAtividades { get; set; }

    public virtual DbSet<Empresa> Empresas { get; set; }

    public virtual DbSet<Filtro> Filtros { get; set; }

    public virtual DbSet<Socio> Socios { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        //optionsBuilder.EnableSensitiveDataLogging();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Atividade>(entity =>
        {
            entity.HasKey(e => e.NoAtividade).HasName("PK__Atividad__00E6730B9DBBF287");

            entity.ToTable("Atividade");

            entity.Property(e => e.NoAtividade)
                .ValueGeneratedNever()
                .HasColumnName("no_atividade");
            entity.Property(e => e.DsAtividade)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("ds_atividade");
        });

        modelBuilder.Entity<Cep>(entity =>
        {
            entity.HasKey(e => new { e.NoCep, e.CdMunicipio }).HasName("PK__Cep__0078B694D03858C7");

            entity.ToTable("Cep");

            entity.Property(e => e.NoCep)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("no_cep");
            entity.Property(e => e.CdMunicipio)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("cd_municipio");
            entity.Property(e => e.CdBairro)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("cd_bairro");
            entity.Property(e => e.CdEstado)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("cd_estado");
            entity.Property(e => e.CdLogradouro)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("cd_logradouro");
        });

        modelBuilder.Entity<EmpAtividade>(entity =>
        {
            entity.HasKey(e => e.NoAtividade).HasName("PK__Emp_Ativ__00E6730BDC7A989A");

            entity.ToTable("Emp_Atividade");

            entity.Property(e => e.NoAtividade)
                .ValueGeneratedNever()
                .HasColumnName("no_atividade");
            entity.Property(e => e.CdAtvPrincipal)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("cd_atv_principal");
            entity.Property(e => e.NoCnpj).HasColumnName("no_cnpj");

            entity.HasOne(d => d.NoCnpjNavigation).WithMany(p => p.EmpAtividades)
                .HasForeignKey(d => d.NoCnpj)
                .HasConstraintName("FK__Emp_Ativi__no_cn__4B7734FF");
        });

        modelBuilder.Entity<Empresa>(entity =>
        {
            entity.HasKey(e => e.NoCnpj).HasName("PK__Empresa__BF7567A43A69EBC2");

            entity.ToTable("Empresa");

            entity.Property(e => e.NoCnpj)
                .ValueGeneratedNever()
                .HasColumnName("no_cnpj");
            entity.Property(e => e.CdEmail)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("cd_email");
            entity.Property(e => e.CdFantasia)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("cd_fantasia");
            entity.Property(e => e.CdLogra)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("cd_logra");
            entity.Property(e => e.CdMei)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("cd_mei");
            entity.Property(e => e.CdNumero)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("cd_numero");
            entity.Property(e => e.CdRzsocial)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("cd_rzsocial");
            entity.Property(e => e.CdSituacao)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("cd_situacao");
            entity.Property(e => e.CdTipo)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("cd_tipo");
            entity.Property(e => e.DtAbertura)
                .HasColumnType("date")
                .HasColumnName("dt_abertura");
            entity.Property(e => e.DtSituacao)
                .HasColumnType("date")
                .HasColumnName("dt_situacao");
            entity.Property(e => e.NoCep)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("no_cep");
            entity.Property(e => e.NoFone)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("no_fone");
            entity.Property(e => e.NoNatjur).HasColumnName("no_natjur");
            entity.Property(e => e.VlCapsocial).HasColumnName("vl_capsocial");
        });

        modelBuilder.Entity<Filtro>(entity =>
        {
            entity.HasKey(e => new { e.NoCep, e.DtInicial, e.DtFinal }).HasName("PK__Filtro__94B19BA4EBBC1B16");

            entity.ToTable("Filtro");

            entity.Property(e => e.NoCep)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("no_cep");
            entity.Property(e => e.DtInicial)
                .HasColumnType("date")
                .HasColumnName("dt_inicial");
            entity.Property(e => e.DtFinal)
                .HasColumnType("date")
                .HasColumnName("dt_final");
            entity.Property(e => e.DtExecucao)
                .HasColumnType("date")
                .HasColumnName("dt_execucao");
            entity.Property(e => e.NoContador).HasColumnName("no_contador");
        });

        modelBuilder.Entity<Socio>(entity =>
        {
            entity.HasKey(e => new { e.DsSocio, e.DsTpSocio, e.NoCnpj }).HasName("PK__Socios__7D54B1BCFDE4D09B");

            entity.Property(e => e.DsSocio)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("ds_socio");
            entity.Property(e => e.DsTpSocio)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("ds_tp_socio");
            entity.Property(e => e.NoCnpj).HasColumnName("no_cnpj");

            entity.HasOne(d => d.NoCnpjNavigation).WithMany(p => p.Socios)
                .HasForeignKey(d => d.NoCnpj)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Socios__no_cnpj__489AC854");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}