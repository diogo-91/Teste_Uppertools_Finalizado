using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CadastroEmpresa.Models
{
    public class Empresa
    {
        public int Id { get; set; }

        [Required (ErrorMessage = "Informe uma Empresa")]
        [JsonProperty("cnpj")]
        public string CNPJ { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("nome")]
        public string Nome { get; set; }

        [JsonProperty("fantasia")]
        public string NomeFantasia { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("telefone")]
        public string Telefone { get; set; }

        public Empresa FormataCNPJ()
        {
            CNPJ = Convert.ToInt64(CNPJSemFormatacao()).ToString(@"00\.000\.000\/0000\-00");
            return this;
        }

        public string CNPJSemFormatacao()
        {
            return CNPJ?.Replace(".", string.Empty).Replace("-", string.Empty).Replace("/", string.Empty);
        }
    }
}
