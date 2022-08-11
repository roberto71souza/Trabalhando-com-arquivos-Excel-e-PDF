using Excel_Pdf.Models;
using ExcelDataReader;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Excel_Pdf
{
    public class ProdutoStore
    {
        private List<ProdutoModel> _Produto { get; set; }

        public ProdutoStore()
        {
            _Produto = new List<ProdutoModel>() {
                new ProdutoModel() {NomeProduto = "Mouse gamer",Tipo = "Informática",Quantidade = 55,Preco = 59.90M} ,
                new ProdutoModel() {NomeProduto = "Teclado RGB",Tipo = "Informática",Quantidade = 15,Preco = 214.90M} ,
                new ProdutoModel() {NomeProduto = "Fogão 6 bocas",Tipo = "Cozinha",Quantidade = 60,Preco = 650.00M} ,
            };
        }

        public List<ProdutoModel> ListarProduto()
        {
            return _Produto;
        }

        public ProdutoModel GetProdutoById(string id)
        {
            var produto = _Produto.FirstOrDefault(p => p.Id == id);
            return produto;
        }

        public void AdicionarProduto(ProdutoModel model)
        {
            _Produto.Add(model);
        }

        public void SalvarArquivo(IFormFile file, string destinoArquivo)
        {
            using (FileStream stream = new FileStream(destinoArquivo, FileMode.Create))
            {
                file.CopyTo(stream);
            }
        }

        public List<ProdutoModel> LerArquivo(string file)
        {
            var listaArquivos = new List<ProdutoModel>();

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            using (FileStream stream = new FileStream(file, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    while (reader.Read())
                    {
                        decimal.TryParse(reader.GetValue(3).ToString(), NumberStyles.Any, CultureInfo.CreateSpecificCulture("pt-BR"), out var preco);

                        listaArquivos.Add(new ProdutoModel()
                        {
                            NomeProduto = Convert.ToString(reader.GetValue(0)),
                            Tipo = Convert.ToString(reader.GetValue(1)),
                            Quantidade = Convert.ToInt32(reader.GetValue(2)),
                            Preco = preco
                        });
                    }
                }
            }
            return listaArquivos;
        }

        public void DeletarArquivo(string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }
}
