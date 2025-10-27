// 📁 Models/CartViewModel.cs
using System.Collections.Generic;
using System.Linq;

namespace July_Team.Models
{
    public class CartViewModel
    {
        public List<CartItem> Items { get; set; } = new List<CartItem>();
        public decimal GrandTotal => Items.Sum(i => i.TotalPrice);
    }
}
