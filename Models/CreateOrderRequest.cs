using System.ComponentModel.DataAnnotations;

namespace LogiTrack.Models
{
  public class CreateOrderRequest
  {
    [Required]
    [MaxLength(100)]
    public string CustomerName { get; set; } = string.Empty;

    [Required]
    public DateTime DatePlaced { get; set; }

    public List<int> ItemIds { get; set; } = new List<int>();
  }
}