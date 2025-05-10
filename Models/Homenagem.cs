#nullable enable
using System;

namespace HomenagensApp.Models
{
    public class Homenagem
    {
        public int Id { get; set; }
        public string NomeCliente { get; set; } = string.Empty;
        public string NomeMae { get; set; } = string.Empty;
        public string Mensagem { get; set; } = string.Empty;
        public string? FotoUrl { get; set; }
        public string WhatsApp { get; set; } = string.Empty;
        public bool ParticipaSorteio { get; set; }
        public DateTime DataEnvio { get; set; } = DateTime.UtcNow;
    }
}