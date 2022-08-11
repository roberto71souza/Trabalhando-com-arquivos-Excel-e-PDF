using System;

namespace Excel_Pdf.Models
{
    public class ProdutoModel
    {
        public string Id { get; private set; }
        public string NomeProduto { get; set; }
        public string Tipo { get; set; }
        public int Quantidade { get; set; }
        public decimal Preco { get; set; }
        public DateTime DataCadastro { get; private set; }

        public ProdutoModel()
        {
            DataCadastro = DateTime.UtcNow;
            var valueId = Guid.NewGuid().ToString().Split("-");
            Id = valueId[valueId.Length - 1];
        }
    }
}
