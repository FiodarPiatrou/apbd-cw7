using System.ComponentModel.DataAnnotations;
using Microsoft.VisualBasic.CompilerServices;

namespace Cw7.Models.DTOs;

public class PostWarehouse
{
    [Required]
    public int IdProduct { get; set; }
    [Required]
    public int IdWarehouse { get; set; }
    [Required]
    [Range(1,int.MaxValue)]
    public int Amount { get; set; }
    [Required]
    public DateTime CreatedAt { get; set; }
}