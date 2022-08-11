using ClosedXML.Excel;
using Excel_Pdf.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Rotativa.AspNetCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Excel_Pdf.Controllers
{
    public class HomeController : Controller
    {
        public ProdutoStore _produtoStore { get; set; }
        public IHostingEnvironment _hostingEnvironment { get; set; }

        public HomeController(ProdutoStore produto, IHostingEnvironment hostingEnvironment)
        {
            _produtoStore = produto;
            _hostingEnvironment = hostingEnvironment;
        }
        public IActionResult Index()
        {
            var produtoList = _produtoStore.ListarProduto();

            return View(produtoList);
        }

        [HttpPost]
        public IActionResult ImportarExcel(IFormFile file)
        {
            var filePathName = $"{_hostingEnvironment.WebRootPath}\\File\\{file.FileName}";

            var extensoesValidas = new List<string> { ".xls", ".xlsx" };

            if (extensoesValidas.Contains(Path.GetExtension(file.FileName)))
            {
                _produtoStore.SalvarArquivo(file, filePathName);

                var listaProdutos = _produtoStore.LerArquivo(filePathName);

                foreach (var produto in listaProdutos)
                {
                    _produtoStore.AdicionarProduto(produto);
                }

                _produtoStore.DeletarArquivo(filePathName);
            }

            return RedirectToAction("Index");
        }

        [HttpGet("open-modal-adicionar-produto")]
        public IActionResult AdicionarProduto()
        {
            return PartialView();
        }

        [HttpPost("post-produto")]
        public IActionResult PostProduto(ProdutoModel produtoModel)
        {
            if (ModelState.IsValid)
            {
                _produtoStore.AdicionarProduto(produtoModel);
            }

            return RedirectToAction("Index");
        }

        public IActionResult GerarExcel(string id)
        {
            var produtos = new List<ProdutoModel>();

            if (id is not null)
            {
                produtos.Add(_produtoStore.GetProdutoById(id));
            }
            else
            {
                produtos.AddRange(_produtoStore.ListarProduto());
            }

            using (var webWorkbook = new XLWorkbook())
            {
                var planilha = webWorkbook.Worksheets.Add("Produtos");
                var caracteres = new string[] { "A", "B", "C", "D", "E", "F" };
                var row = 2;

                planilha.Cell("A1").Value = "Id";
                planilha.Cell("B1").Value = "Nome do produto";
                planilha.Cell("C1").Value = "Tipo";
                planilha.Cell("D1").Value = "Quantidade";
                planilha.Cell("E1").Value = "Preço";
                planilha.Cell("F1").Value = "Data de cadastro";

                for (int i = 0; i < caracteres.Length; i++)
                {
                    planilha.Cell($"{caracteres[i]}1").Style.Fill.SetBackgroundColor(XLColor.Green);
                    planilha.Cell($"{caracteres[i]}1").Style.Font.SetFontColor(XLColor.White);
                }

                foreach (var produto in produtos)
                {
                    planilha.Cell($"A{row}").Value = produto.Id;
                    planilha.Cell($"B{row}").Value = produto.NomeProduto;
                    planilha.Cell($"C{row}").Value = produto.Tipo;
                    planilha.Cell($"D{row}").Value = produto.Quantidade;
                    planilha.Cell($"E{row}").Value = string.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0:C}", produto.Preco);
                    planilha.Cell($"F{row}").Value = produto.DataCadastro.ToString("dd/MM/yyyy");

                    row++;
                }

                row = 2;

                for (int r = 0; r < produtos.Count; r++)
                {
                    for (int c = 0; c < caracteres.Length; c++)
                    {
                        planilha.Cell($"{caracteres[c]}{row}").Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        planilha.Cell($"{caracteres[c]}{row}").Style.Border.OutsideBorderColor = XLColor.Green;
                        planilha.Cell($"{caracteres[c]}{row}").Style.Font.SetFontColor(XLColor.Red);
                    }
                    row++;
                }

                planilha.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    webWorkbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Lista de produtos {DateTime.Now.ToString("dd/MM/yyyyy")}.xlsx");
                }
            }
        }

        public IActionResult GerarPDF(string id)
        {
            var produto = new List<ProdutoModel>();

            if (id is not null)
            {
                produto.Add(_produtoStore.GetProdutoById(id));
            }
            else
            {
                produto.AddRange(_produtoStore.ListarProduto());
            }

            return new ViewAsPdf(produto)
            {
                FileName = $"Lista de produtos {DateTime.Now.ToString("dd/MM/yyyyy")}.pdf",
                PageSize = Rotativa.AspNetCore.Options.Size.A4
            };
        }

    }
}
