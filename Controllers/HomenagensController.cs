using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using HomenagensApp.Data;
using HomenagensApp.Models;
using HomenagensApp.SignalR;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace HomenagensApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HomenagensController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IHubContext<HomenagemHub> _hub;
    private readonly IWebHostEnvironment _env;

    public HomenagensController(AppDbContext context, IHubContext<HomenagemHub> hub, IWebHostEnvironment env)
    {
        _context = context;
        _hub = hub;
        _env = env;
    }

    [HttpPost("enviar")]
    [RequestSizeLimit(5_000_000)]
    public async Task<IActionResult> Enviar([FromForm] HomenagemDto dto)
    {
        string? urlFoto = null;
        if (dto.Foto != null && dto.Foto.Length > 0)
        {
            var uploads = Path.Combine(_env.WebRootPath ?? "wwwroot", "uploads");
            Directory.CreateDirectory(uploads);
            var filename = $"{Guid.NewGuid()}{Path.GetExtension(dto.Foto.FileName)}";
            var filePath = Path.Combine(uploads, filename);
            await using var stream = new FileStream(filePath, FileMode.Create);
            await dto.Foto.CopyToAsync(stream);
            urlFoto = $"/uploads/{filename}";
        }

        var homenagem = new Homenagem
        {
            NomeCliente = dto.NomeCliente,
            NomeMae = dto.NomeMae,
            Mensagem = dto.Mensagem,
            FotoUrl = urlFoto,
            WhatsApp = dto.WhatsApp,
            ParticipaSorteio = dto.ParticipaSorteio
        };

        _context.Homenagens.Add(homenagem);
        await _context.SaveChangesAsync();

        await _hub.Clients.All.SendAsync("NovaHomenagem", homenagem);
        return Ok(homenagem);
    }

    [HttpGet("export")]
    public IActionResult Exportar()
    {
        var lista = _context.Homenagens
                             .OrderByDescending(h => h.DataEnvio)
                             .ToList();
        return Ok(lista);
    }
}

public class HomenagemDto
{
    public string NomeCliente { get; set; } = string.Empty;
    public string NomeMae { get; set; } = string.Empty;
    public string Mensagem { get; set; } = string.Empty;
    public IFormFile? Foto { get; set; }
    public string WhatsApp { get; set; } = string.Empty;
    public bool ParticipaSorteio { get; set; }
}