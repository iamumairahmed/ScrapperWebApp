﻿using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using ScrapperWebApp.Models;
using ScrapperWebApp.Services.Interfaces;
using System.Text;
using System.Text.Json;
using Response = ScrapperWebApp.Models.Response;
using System.Globalization;
using HtmlAgilityPack;
using ScrapperWebApp.Data;
using System;

namespace ScrapperWebApp.Services
{
    public class ScrapperService : IScrapperService
    {
        private IFiltroService _filtroService;
        public ScrapperService(IFiltroService filtroService) {
            _filtroService = filtroService;
        }

        private string url = "https://api.casadosdados.com.br/v2/public/cnpj/search";
        public async Task<List<Empresa>> GetScrappedData(Filtro filtro){

            var data = new List<Empresa>();

            var requestObject = new RequestObject();

            requestObject.query.situacao_cadastral = "ATIVA";
            requestObject.query.cep = [filtro.NoCep.ToString()];
            //2024-04-27
            requestObject.range_query.data_abertura.lte = filtro.DtFinal.ToString("yyyy-MM-dd");
            requestObject.range_query.data_abertura.gte = filtro.DtInicial.ToString("yyyy-MM-dd");
            requestObject.page = 1;

            var jsonRequest = JsonSerializer.Serialize(requestObject);
            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
            HttpClient client = new HttpClient();

            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/90.0.4430.93 Safari/537.36");

            var response = await client.PostAsync(url, content);
            var res = await response.Content.ReadAsStringAsync();
            var responseObject = JsonSerializer.Deserialize<Response>(res);
            if (responseObject != null && responseObject.success == true)
            {
                if (responseObject.data.count == 0)
                {
                    return new List<Empresa>();
                }
                // API_1000
                else if (responseObject.data.count > 1000)
                {
                    // Do Nothing
                }
                // API_SCRAPER
                else
                {
                    int totalPages = (int)Math.Ceiling((double)responseObject.data.count / 20);
                    for (int i = 1; i <= totalPages; i++)
                    {
                        if (i > 1)
                        {
                            requestObject.page = 2;
                            jsonRequest = JsonSerializer.Serialize(requestObject);
                            content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                            response = await client.PostAsync(url, content);
                            res = await response.Content.ReadAsStringAsync();
                            responseObject = JsonSerializer.Deserialize<Response>(res);
                            if (responseObject == null || responseObject.success != true)
                            {
                                return null;
                            }
                        }

                        foreach (var item in responseObject.data.cnpj)
                        {
                            Empresa empresa = new Empresa();
                            empresa.NoCnpj = long.Parse(item.cnpj);
                            empresa.CdRzsocial = item.razao_social;
                            empresa.CdFantasia = item.nome_fantasia;
                            //empresa.CdTipo = ;
                            empresa.DtAbertura = item.data_abertura;
                            empresa.CdSituacao = item.situacao_cadastral;
                            //empresa.DtSituacao = ;
                            //empresa.VlCapsocial = ;
                            //empresa.NoNatjur = ;
                            empresa.CdLogra = item.logradouro;
                            empresa.CdNumero = item.numero;
                            empresa.NoCep = filtro.NoCep;
                            //empresa.NoFone = ;
                            //empresa.CdEmail = ;
                            empresa.CdMei = item.cnpj_mei ? "Yes" : "No";

                            string removedDots = item.razao_social.Replace(".", "");
                            string filtered = removedDots.Replace(" ", "-");
                            var details_url = "https://casadosdados.com.br/solucao/cnpj/" + filtered + "-" + item.cnpj;

                            var updatedEmpresa = await ScrapeDetailsHtml(details_url, empresa);
                            data.Add(updatedEmpresa);

                        }
                    }
                }
            }
            else
            {
                return null;
            }

            return data;

            //var obj = new Empresa();
            //obj.NoCnpj = 2;
            //obj.CdRzsocial = "MS CONTA MAIS";
            //obj.CdTipo = "MATRIZ";
            //obj.CdFantasia = "MAPAL";
            //obj.DtAbertura = DateTime.Today;
            //obj.CdSituacao = "ATIVA";
            //obj.DtSituacao = DateTime.Today;
            //obj.VlCapsocial = 10000;
            //obj.NoNatjur = 2062;
            //obj.CdLogra = "ESTRADA DA GAVEA";
            //obj.NoCep = "00640";
            //obj.CdEmail = "test@gmail.com";


            //var socioObj = new Socio();
            //socioObj.DsSocio = "FLAVIA ARAUJO";
            //socioObj.DsTpSocio = "Socio-Administrator";
            //socioObj.NoCnpj = 2;

            //obj.Socios.Add(socioObj);

            //socioObj.DsSocio = "FLAVIA ARAUJO 2";
            //socioObj.DsTpSocio = "Socio-Administrator 2";
            //socioObj.NoCnpj = 2;
            //obj.Socios.Add(socioObj);

            //var atividadeObj = new EmpAtividade();
            //atividadeObj.NoAtividade = 124;
            //atividadeObj.CdAtvPrincipal = "YES";
            //atividadeObj.NoCnpj = 1;

            //obj.EmpAtividades.Add(atividadeObj);

            //data.Add(obj);
            //return data;
        }
        public async Task<List<Filtro>> GetMainRecords(Filtro filtro)
        {
            var data = new List<Filtro>();

            var requestObject = new RequestObject();

            requestObject.query.situacao_cadastral = "ATIVA";
            requestObject.query.cep = [filtro.NoCep.ToString()];
            requestObject.range_query.data_abertura.lte = filtro.DtFinal.ToString("yyyy-MM-dd");
            requestObject.range_query.data_abertura.gte = filtro.DtInicial.ToString("yyyy-MM-dd");
            requestObject.page = 1;

            var jsonRequest = JsonSerializer.Serialize(requestObject);
            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
            HttpClient client = new HttpClient();

            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/90.0.4430.93 Safari/537.36");

            var response = await client.PostAsync(url, content);
            var res = await response.Content.ReadAsStringAsync();
            var responseObject = JsonSerializer.Deserialize<Response>(res);
            if (responseObject != null && responseObject.success == true)
            {
                if (responseObject.data.count == 0)
                {
                    return new List<Filtro>();
                }
                // API_1000
                else if (responseObject.data.count > 1000)
                {
                    filtro.DtExecucao = DateTime.Now;
                    filtro.NoContador = responseObject.data.count;
                    await _filtroService.UpdateFiltroAsync(filtro);

                }
                // API_SCRAPER
                else
                {
                    filtro.DtExecucao = DateTime.Now;
                    filtro.NoContador = responseObject.data.count;
                    await _filtroService.UpdateFiltroAsync(filtro);
                }
                
                data.Add(filtro);
            }
            else
            {
                return null;
            }

            return data;
        }

        private void ScrapeDetails(string url,ref Empresa empresa) 
        {
            try
            {
                ChromeOptions options = new ChromeOptions();
                //options.AddArgument("--headless"); // Run Chrome in headless mode (without opening GUI)

                // Initialize Chrome WebDriver
                using (var driver = new ChromeDriver(options))
                {
                    // Navigate to the website
                    driver.Navigate().GoToUrl(url);

                    var overlayElement = driver.FindElements(By.ClassName("fc-dialog-container"));

                    if (overlayElement != null && overlayElement.Count > 0)
                    {
                        // Execute JavaScript to remove the overlay element from the DOM
                        IJavaScriptExecutor jsExecutor = (IJavaScriptExecutor)driver;
                        jsExecutor.ExecuteScript("arguments[0].remove()", overlayElement);
                    }

                    var elemDtSituacao = driver.FindElements(By.XPath("//p[contains(text(), 'Data da Situação Cadastral')]"));
                    if (elemDtSituacao != null && elemDtSituacao.Count > 0)
                    {
                        IWebElement elem = elemDtSituacao.FirstOrDefault().FindElement(By.XPath("following-sibling::p[1]"));
                        string format = "dd/MM/yyyy";
                        if (DateTime.TryParseExact(elem.Text, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result))
                        {
                            empresa.DtSituacao = DateTime.Parse(result.ToString("yyyy-MM-dd"));
                            Console.WriteLine("Parsed DateTime: " + result.ToString("yyyy-MM-dd")); // Output: 2024-04-21
                        }
                        else
                        {
                            Console.WriteLine("Failed to parse date string.");
                        }
                    }

                    var elemTelefone = driver.FindElements(By.XPath("//p[contains(text(), 'Telefone')]"));
                    if (elemTelefone != null && elemTelefone.Count > 0)
                    {
                        IWebElement elem = elemTelefone.FirstOrDefault().FindElement(By.XPath("following-sibling::p[1]"));
                        empresa.NoFone = elem.Text;
                    }

                    var elemEmail = driver.FindElements(By.XPath("//p[contains(text(), 'E-MAIL')]"));
                    if (elemEmail != null && elemEmail.Count > 0)
                    {
                        IWebElement elem = elemEmail.FirstOrDefault().FindElement(By.XPath("following-sibling::p[1]"));
                        empresa.CdEmail = elem.Text;
                    }


                    var elemTipo = driver.FindElements(By.XPath("//p[contains(text(), 'Tipo')]"));
                    if (elemTipo != null && elemTipo.Count > 0)
                    {
                        IWebElement elem = elemTipo.FirstOrDefault().FindElement(By.XPath("following-sibling::p[1]"));
                        empresa.CdTipo = elem.Text;
                    }


                    var elemSocios = driver.FindElements(By.XPath("//p[contains(text(), 'Quadro Societário')]"));
                    if (elemSocios != null && elemSocios.Count > 0)
                    {
                        var elem = elemSocios.FirstOrDefault().FindElements(By.XPath("following-sibling::p"));
                        if(elem != null && elem.Count > 0) 
                        {
                            foreach (var e in elem) 
                            {
                                var tokens = e.Text.Split(" - ");
                                var ds_socio = tokens[0];
                                var ds_tp_socio = tokens.Length == 2 ? tokens[1] : null;
                                Socio socio = new Socio();
                                socio.DsSocio = ds_socio;
                                socio.DsTpSocio = ds_tp_socio;
                                socio.NoCnpj = empresa.NoCnpj;
                                empresa.Socios.Add(socio);


                            }
                        }
                    }

                    
                    var elemMainAtividades = driver.FindElements(By.XPath("//p[contains(text(), 'Atividade Principal')]"));
                    if (elemMainAtividades != null && elemMainAtividades.Count > 0)
                    {
                        var elem = elemMainAtividades.FirstOrDefault().FindElements(By.XPath("following-sibling::p"));
                        if (elem != null && elem.Count > 0)
                        {
                            foreach (var e in elem)
                            {
                                var tokens = e.Text.Split(" - ");
                                var no_atividade = tokens[0];
                                EmpAtividade empAtividade = new EmpAtividade();
                                empAtividade.NoAtividade = int.Parse(no_atividade);
                                empAtividade.NoCnpj = empresa.NoCnpj;
                                empAtividade.CdAtvPrincipal = "Yes";
                                empresa.EmpAtividades.Add(empAtividade);


                            }
                        }
                    }
                    
                    var elemAtividades = driver.FindElements(By.XPath("//p[contains(text(), 'Atividades Secundárias')]"));
                    if (elemAtividades != null && elemAtividades.Count > 0)
                    {
                        var elem = elemAtividades.FirstOrDefault().FindElements(By.XPath("following-sibling::p"));
                        if (elem != null && elem.Count > 0)
                        {
                            foreach (var e in elem)
                            {
                                var tokens = e.Text.Split(" - ");
                                var no_atividade = tokens[0];
                                EmpAtividade empAtividade = new EmpAtividade();
                                empAtividade.NoAtividade = int.Parse(no_atividade);
                                empAtividade.NoCnpj = empresa.NoCnpj;
                                empresa.EmpAtividades.Add(empAtividade);


                            }
                        }
                    }
                }
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
        }
        private async Task<Empresa> ScrapeDetailsHtml(string url, Empresa empresa) 
        {
            try
            {
                var requestObject = new RequestObject();
               
                var jsonRequest = JsonSerializer.Serialize(requestObject);
                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                HttpClient client = new HttpClient();

                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/90.0.4430.93 Safari/537.36");

                var response = await client.GetAsync(url);
                var res = await response.Content.ReadAsStringAsync();
                //var responseObject = JsonSerializer.Deserialize<Response>(res);

                HtmlDocument document = new HtmlDocument();
                document.LoadHtml(res);


                // GET TELEFONE
                var telefoneElements = document.DocumentNode.SelectNodes("//*[contains(text(), 'Telefone')]");
                if (telefoneElements != null)
                {
                    var childs = telefoneElements[0].ParentNode.ChildNodes.Where(x => x.Name == "p" && x.InnerText != "-");
                    foreach (var e in childs)
                    {
                        Telefone telefone = new Telefone();
                        telefone.NoFone = e.InnerText;
                        telefone.NoCnpj = empresa.NoCnpj;
                        empresa.Telefones.Add(telefone);
                    }
                }
                else
                    Console.WriteLine("No elements found with the text 'Telefone:'.");


                // GET EMAIL
                var emailElements = document.DocumentNode.SelectNodes("//*[contains(text(), 'Email:')]");
                if (emailElements != null)
                {
                    var childs = emailElements[0].ParentNode.ChildNodes.Where(x => x.Name == "p" && x.InnerText != "-");
                    if (childs.Count() > 0)
                    {
                        empresa.CdEmail = childs.FirstOrDefault().InnerText;
                    }
                    else 
                    {
                        Console.WriteLine("Either no or more than one 'Email' found.");
                    }
                }
                else
                    Console.WriteLine("No elements found with the text 'Email:'.");
                
                
                // GET TIPO
                var tipoElements = document.DocumentNode.SelectNodes("//*[contains(text(), 'Tipo:')]");
                if (tipoElements != null)
                {
                    var childs = tipoElements[0].ParentNode.ChildNodes.Where(x => x.Name == "p" && x.InnerText != "-");
                    if (childs.Count() > 0)
                    {
                        empresa.CdTipo = childs.FirstOrDefault().InnerText;
                    }
                    else
                    {
                        Console.WriteLine("Either no or more than one 'Tipo' found.");
                    }
                }
                else
                    Console.WriteLine("No elements found with the text 'Tipo'.");


                // GET Data da Situação
                var dataSituacaoElements = document.DocumentNode.SelectNodes("//*[contains(text(), 'Data da Situação:')]");
                if (dataSituacaoElements != null)
                {

                    var childs = dataSituacaoElements[0].ParentNode.ChildNodes.Where(x => x.Name == "p" && x.InnerText != "-");
                    if (childs.Count() > 0)
                    {
                        string format = "dd/MM/yyyy";
                        if (DateTime.TryParseExact(childs.FirstOrDefault().InnerText, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result))
                        {
                            empresa.DtSituacao = DateTime.Parse(result.ToString("yyyy-MM-dd"));
                        }
                        else
                        {
                            Console.WriteLine("Failed to parse Data da Situação date string.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Either no or more than one 'Data da Situação' found.");
                    }
                }
                else
                    Console.WriteLine("No elements found with the text 'Data da Situação:'.");


                // GET CNAE Principal
                var cnaePrincipalElements = document.DocumentNode.SelectNodes("//*[contains(text(), 'CNAE Principal:')]");
                if (cnaePrincipalElements != null)
                {
                    var childs = cnaePrincipalElements[0].ParentNode.ChildNodes.Where(x => x.Name == "p" && x.InnerText != "-");
                    if (childs.Count() > 0)
                    {
                        foreach (var e in childs)
                        {
                            var tokens = e.InnerText.Split(" - ");
                            var no_atividade = tokens[0];
                            EmpAtividade empAtividade = new EmpAtividade();
                            empAtividade.NoAtividade = int.Parse(no_atividade);
                            empAtividade.NoCnpj = empresa.NoCnpj;
                            empAtividade.CdAtvPrincipal = "Yes";
                            empresa.EmpAtividades.Add(empAtividade);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Either no or more than one 'CNAE Principal' found.");
                    }
                }
                else
                    Console.WriteLine("No elements found with the text 'CNAE Principal'.");


                // GET CNAE Secondary
                var cnaeSecondaryElements = document.DocumentNode.SelectNodes("//*[contains(text(), 'CNAEs Secundários:')]");
                if (cnaeSecondaryElements != null)
                {
                    var childs = cnaeSecondaryElements[0].ParentNode.ChildNodes.Where(x => x.Name == "p" && x.InnerText != "-");
                    if (childs.Count() > 0)
                    {
                        foreach (var e in childs)
                        {
                            var tokens = e.InnerText.Split(" - ");
                            var no_atividade = tokens[0];
                            EmpAtividade empAtividade = new EmpAtividade();
                            empAtividade.NoAtividade = int.Parse(no_atividade);
                            empAtividade.NoCnpj = empresa.NoCnpj;
                            empresa.EmpAtividades.Add(empAtividade);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Either no or more than one 'CNAE Secundários' found.");
                    }
                }
                else
                    Console.WriteLine("No elements found with the text 'CNAE Secundários'.");


                // GET CNAE Secondary
                var sociosElements = document.DocumentNode.SelectNodes("//*[contains(text(), 'Sócios:')]");
                if (sociosElements != null)
                {
                    var childs = sociosElements[0].ParentNode.ChildNodes.Where(x => x.Name == "p" && x.InnerText != "-");
                    if (childs.Count() > 0)
                    {
                        foreach (var e in childs)
                        {
                            var tokens = e.InnerText.Split(" - ");
                            var ds_socio = tokens[0];
                            var ds_tp_socio = tokens.Length > 0 ? tokens[1] : null;
                            Socio socio = new Socio();
                            socio.DsSocio = ds_socio;
                            socio.DsTpSocio = ds_tp_socio;
                            socio.NoCnpj = empresa.NoCnpj;
                            empresa.Socios.Add(socio);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Either no or more than one 'Sócios' found.");
                    }
                }
                else
                    Console.WriteLine("No elements found with the text 'Sócios'.");
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); return null; }
            return empresa;
        }
       
    }
}
