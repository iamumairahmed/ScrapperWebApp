using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using ScrapperWebApp.Models;
using ScrapperWebApp.Services.Interfaces;
using System.Text;
using System.Text.Json;
using Response = ScrapperWebApp.Models.Response;
using System.Globalization;
using ScrapperWebApp.Utility;
using ExcelDataReader;
using System.Data;
using System.Text.RegularExpressions;
using ClosedXML.Excel;
using HtmlAgilityPack;
using ScrapperWebApp.Data;
using System;
using System.Xml.Linq;

namespace ScrapperWebApp.Services
{
    public class ScrapperService : IScrapperService
    {
        private IFiltroService _filtroService;
        private IConfiguration _configurationManager;
        public ScrapperService(IFiltroService filtroService, IConfiguration configuration) {
            _filtroService = filtroService;
            _configurationManager = configuration;
        }

        private string url = "https://api.casadosdados.com.br/v2/public/cnpj/search";
        public async Task<List<Empresa>> GetScrappedData(Filtro filtro){

            ScrapeDetailsHtml("https://casadosdados.com.br/solucao/cnpj/fergus-holding-capital-ltda-53414766000189", new Empresa());
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

        private void ScrapeDetails(string url, ref Empresa empresa)
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
                        if (elem != null && elem.Count > 0)
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
                    var childs = telefoneElements[0].ParentNode.ChildNodes.Where(x => x.Name == "p" && x.InnerText != "-" && x.InnerText != " ");
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
                    var childs = emailElements[0].ParentNode.ChildNodes.Where(x => x.Name == "p" && x.InnerText != "-" && x.InnerText != " ");
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
                    var childs = tipoElements[0].ParentNode.ChildNodes.Where(x => x.Name == "p" && x.InnerText != "-" && x.InnerText != " ");
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


                // GET Natureza Jurídica:
                var naturezaJuridicaElements = document.DocumentNode.SelectNodes("//*[contains(text(), 'Natureza Jurídica:')]");
                if (naturezaJuridicaElements != null)
                {
                    var childs = naturezaJuridicaElements[0].ParentNode.ChildNodes.Where(x => x.Name == "p" && x.InnerText != "-" && x.InnerText != " ");
                    if (childs.Count() > 0)
                    {
                        var tokens = childs.FirstOrDefault().InnerText.Split(" - ");
                        empresa.NoNatjur = tokens != null && tokens.Count() > 0 ? int.Parse(tokens[0]) : 0;
                    }
                    else
                    {
                        Console.WriteLine("Either no or more than one 'Natureza Jurídica' found.");
                    }
                }
                else
                    Console.WriteLine("No elements found with the text 'Natureza Jurídica'.");


                // GET Capital Social:
                var capitalSocialElements = document.DocumentNode.SelectNodes("//*[contains(text(), 'Capital Social:')]");
                if (capitalSocialElements != null)
                {
                    var childs = capitalSocialElements[0].ParentNode.ChildNodes.Where(x => x.Name == "p" && x.InnerText != "-" && x.InnerText != " ");
                    if (childs.Count() > 0)
                    {
                        var value = Regex.Replace(childs.FirstOrDefault().InnerText, @"[^\d]", "");
                        empresa.VlCapsocial = int.Parse(value);
                    }
                    else
                    {
                        Console.WriteLine("Either no or more than one 'Capital Social' found.");
                    }
                }
                else
                    Console.WriteLine("No elements found with the text 'Capital Social'.");


                // GET Data da Situação
                var dataSituacaoElements = document.DocumentNode.SelectNodes("//*[contains(text(), 'Data da Situação:')]");
                if (dataSituacaoElements != null)
                {

                    var childs = dataSituacaoElements[0].ParentNode.ChildNodes.Where(x => x.Name == "p" && x.InnerText != "-" && x.InnerText != " ");
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
                    var childs = cnaePrincipalElements[0].ParentNode.ChildNodes.Where(x => x.Name == "p" && x.InnerText != "-" && x.InnerText != " ");
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
                    var childs = cnaeSecondaryElements[0].ParentNode.ChildNodes.Where(x => x.Name == "p" && x.InnerText != "-" && x.InnerText != " ");
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
                    var childs = sociosElements[0].ParentNode.ChildNodes.Where(x => x.Name == "p" && x.InnerText != "-" && x.InnerText != " ");
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
       
        public Task<bool> CheckRegistered()
        {
            try
            {
                var records = ReadFile();

                ChromeOptions options = new ChromeOptions();
                //options.AddArgument("--headless"); // Run Chrome in headless mode (without opening GUI)

                // Initialize Chrome WebDriver
                using (var driver = new ChromeDriver(options))
                {

                    //driver.Navigate().GoToUrl(fileUrl);

                    // Login Flow 
                    // Navigate to the website
                    var url = "https://c6bank.my.site.com/partners/s/login/";
                    driver.Navigate().GoToUrl(url);

                    //WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
                    driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);

                    var employeeLabel = driver.FindElement(By.CssSelector("#sfdc_username_container"), 10);

                    if (employeeLabel != null)
                    {
                        //wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.Id("sfdc_username_container")));

                        IWebElement divElement = driver.FindElement(By.Id("sfdc_username_container"));
                        IWebElement inputElement = divElement.FindElement(By.TagName("input"));

                        if (inputElement != null)
                        {
                            inputElement.SendKeys("sandra.coelho.001@c6partner.com");
                        }

                        IWebElement passElement = driver.FindElement(By.Id("sfdc_password_container"));
                        IWebElement inputPassElement = passElement.FindElement(By.TagName("input"));

                        if (inputPassElement != null)
                        {
                            inputPassElement.SendKeys("Okbank@2024!!!");
                        }

                        IWebElement buttonElement = driver.FindElement(By.CssSelector(".slds-button.slds-button--brand.loginButton.uiButton--none.uiButton"));
                        if (buttonElement != null)
                        {
                            buttonElement.Click();
                        }

                        //var leadElem = driver.FindElements(By.XPath("//*[@id=\"commThemeNav\"]/div/div/nav/ul/li[5]"));
                    }

                    Thread.Sleep(10000);
                    driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
                    //IWebElement verificationMsgElem = driver.FindElements(By.XPath("verifique seu dispositivo móvel"));
                    bool verificationDone = false;
                    while (verificationDone != true)
                    {
                        driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
                        //var elems = driver.FindElements(By.XPath("verifique seu dispositivo móvel"));
                        var elements = driver.FindElements(By.XPath("//h2[contains(text(), 'Verifique seu dispositivo móvel')]"));
                        var elementss = driver.FindElements(By.XPath("//h2[contains(text(), 'Solicitação cancelada')]"));
                        var problemElements = driver.FindElements(By.XPath("//h2[contains(text(), 'Problema ao aprovar sua solicitação')]"));
                        if ((problemElements != null || problemElements.Count > 0))
                        {
                            var filtered_elements = problemElements.Where(x => x.Text != "");
                            if (filtered_elements == null || filtered_elements.Count() == 0)
                            {

                            }
                            else
                            {
                                Console.WriteLine("Problema ao aprovar sua solicitação");
                                driver.Quit();
                                return Task.FromResult(false);
                            }
                        }
                        if ((elementss != null || elementss.Count > 0))
                        {
                            var filtered_elements = elementss.Where(x => x.Text != "");
                            if (filtered_elements == null || filtered_elements.Count() == 0)
                            {

                            }
                            else
                            {
                                Console.WriteLine("Solicitação cancelada");
                                driver.Quit();
                                return Task.FromResult(false);
                            }
                        }
                        // "Verifique seu dispositivo móvel"
                        // Solicitação cancelada
                        if (elements == null || elements.Count == 0) 
                        {
                            verificationDone = true;
                        }
                        // Keep looping until Enter key is pressed
                    }

                    //Console.WriteLine("Press Enter to Continue to Leads Page...");
                    //while (Console.ReadKey(true).Key != ConsoleKey.Enter)
                    //{
                    // Keep looping until Enter key is pressed
                    //}

                    Thread.Sleep(5000);
                    IWebElement leadLink = driver.FindElement(By.XPath("//a[text()='Leads']"));
                    if (leadLink != null)
                    {
                        leadLink.Click();
                    }

                    Thread.Sleep(5000);
                    //driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);

                    IWebElement novoLEadElem = driver.FindElement(By.CssSelector(".createRecordWrapper.forceCommunityCreateRecordButton"));
                    if (novoLEadElem != null)
                    {
                        novoLEadElem.Click();
                    }
                    Thread.Sleep(3000);
                    foreach (var r in records)
                    {
                        try {
                            IWebElement firstnameElem = driver.FindElement(By.CssSelector(".firstName.compoundBorderBottom.form-element__row.input"));
                            if (firstnameElem != null)
                            {
                                firstnameElem.Clear();
                                firstnameElem.SendKeys(r.Firstname);
                            }

                            IWebElement lastnameElem = driver.FindElement(By.CssSelector(".lastName.compoundBLRadius.compoundBRRadius.form-element__row.input"));
                            if (lastnameElem != null)
                            {
                                lastnameElem.Clear();
                                lastnameElem.SendKeys(r.Lastname);
                            }

                            //IWebElement emailElem = driver.FindElement(By.XPath("/html/body/div[3]/div[2]/div/div[2]/div/div/div/div[1]/section/div/div/div/div/div/div[2]/div[1]/div/div/div/input"));
                            IWebElement divEmailElement = driver.FindElement(By.CssSelector("div.uiInput.uiInputEmail.uiInput--default.uiInput--input"));
                            if (divEmailElement != null)
                            {
                                IWebElement labelElement = divEmailElement.FindElement(By.TagName("label"));
                                if (labelElement != null && labelElement.Text.StartsWith("Email"))
                                {
                                    // Find the input field inside the div
                                    IWebElement inputEmailElement = divEmailElement.FindElement(By.TagName("input"));
                                    if (inputEmailElement != null)
                                    {
                                        inputEmailElement.Clear();
                                        inputEmailElement.SendKeys(r.Email);
                                    }
                                }
                            }

                            var divCnpjElement = driver.FindElements(By.CssSelector("div.uiInput.uiInputText.uiInput--default.uiInput--input"));
                            if (divCnpjElement != null)
                            {
                                var cnpjElem = divCnpjElement.Where(x => x.Text.StartsWith("CNPJ")).FirstOrDefault();
                                IWebElement labelElement = cnpjElem.FindElement(By.TagName("label"));
                                if (labelElement != null && labelElement.Text.StartsWith("CNPJ"))
                                {
                                    // Find the input field inside the div
                                    IWebElement inputCnpjElement = cnpjElem.FindElement(By.TagName("input"));
                                    if (inputCnpjElement != null)
                                    {
                                        string numbersOnly = Regex.Replace(r.Cnpj, @"[^\d]", "");

                                        inputCnpjElement.Clear();
                                        inputCnpjElement.SendKeys(numbersOnly);
                                    }
                                }
                            }


                            IWebElement divPhoneElement = driver.FindElement(By.CssSelector("div.uiInput.uiInputPhone.uiInput--default.uiInput--input"));
                            if (divPhoneElement != null)
                            {
                                IWebElement labelElement = divPhoneElement.FindElement(By.TagName("label"));
                                if (labelElement != null && (labelElement.Text.StartsWith("Telephone") || labelElement.Text.StartsWith("Telefone")))
                                {
                                    // Find the input field inside the div
                                    IWebElement inputPhoneElement = divPhoneElement.FindElement(By.TagName("input"));
                                    if (inputPhoneElement != null)
                                    {
                                        inputPhoneElement.Clear();
                                        inputPhoneElement.SendKeys(r.Telefone);
                                    }
                                }
                            }

                            IWebElement divConfirmarElement = driver.FindElement(By.CssSelector(".slds-button.slds-button--neutral.button.uiButton--default.uiButton--brand.uiButton"));

                            //IWebElement confirmarButtonn = driver.FindElement(By.XPath("//button[contains(text(),'Confirmar')]"));

                            if (divConfirmarElement != null)
                            {
                                divConfirmarElement.Click();
                            }

                            Thread.Sleep(1000);

                            // Check Errors
                            var phoneError = driver.FindElements(By.CssSelector(".has-error.uiInputDefaultError.uiInput.uiInputPhone.uiInput--default.uiInput--input"));
                            if (phoneError != null && phoneError.Count() > 0)
                            {
                                r.Errors.Add(phoneError.FirstOrDefault().Text);
                            }

                            var errorList = driver.FindElements(By.CssSelector(".errorsList"));
                            if (errorList != null && errorList.Count() > 0)
                            {
                                r.Errors.Add(errorList.FirstOrDefault().Text);
                            }

                            Console.WriteLine(DateTime.Now.ToShortTimeString() + ": Processed " + r.Firstname + " " + r.Lastname + ".");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }

                    WriteExcel(records);
                }
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }

        private void WriteExcel(List<Person> peopleList){
            string base64String;

            using (var wb = new XLWorkbook())
            {
                var datatable = Helper.ConvertToDataTable(peopleList);
                var sheet = wb.AddWorksheet(datatable, "URA With Errors");

                // Apply font color to columns 1 to 5
                sheet.Columns(1, 5).Style.Font.FontColor = XLColor.Black;
                wb.SaveAs(_configurationManager["DirectoryPath"] + "URA with Errors.xlsx");
                //using (var ms = new MemoryStream())
                //{
                //    wb.SaveAs(ms);

                //    // Convert the Excel workbook to a base64-encoded string
                //    base64String = Convert.ToBase64String(ms.ToArray());
                //}
            }

        }
        
        private List<Person> ReadFile() 
        {
            List<Person> list = new List<Person>();
            try
            {
                var filepath = _configurationManager["DirectoryPath"] + "URA Short.xlsx";
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                using (var streamval = File.Open(filepath, FileMode.Open, FileAccess.Read))
                {
                    using (var reader = ExcelReaderFactory.CreateReader(streamval))
                    {
                        var configuration = new ExcelDataSetConfiguration
                        {
                            ConfigureDataTable = _ => new ExcelDataTableConfiguration
                            {
                                UseHeaderRow = true
                            }
                        };
                        var dataSet = reader.AsDataSet(configuration);

                        if (dataSet.Tables.Count > 0)
                        {
                            var dataTable = dataSet.Tables[0];
                            foreach (DataRow row in dataTable.Rows)
                            {
                                Person obj = new Person();
                                obj.Telefone = row[0].ToString();
                                var nameTokens = row[1].ToString().Split(" ");
                                if (nameTokens.Count() > 1)
                                {
                                    obj.Firstname = nameTokens[0];
                                    obj.Lastname = row[1].ToString().Substring(row[1].ToString().IndexOf(" "), row[1].ToString().Length - row[1].ToString().IndexOf(" ")).Trim();
                                }
                                obj.Razao = row[2].ToString();
                                obj.Email = row[3].ToString();
                                obj.Cnpj = row[4].ToString();
                               

                                list.Add(obj);
                            }
                        }
                        else
                        {
                            Console.WriteLine("Sheet doesn't exist");
                            return null;
                        }

                        //if (list.Count > 0)
                        //{
                        //    await _cepService.DeleteAllAsync();
                        //    await _cepService.CreateCepsAsync(list);
                        //}
                        //else
                        //{
                        //    Console.WriteLine("No Data Found!");
                        //    return false;
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
            return list;
        }
    }
}
