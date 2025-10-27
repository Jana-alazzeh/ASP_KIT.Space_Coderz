using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace July_Team.Models
{
    public class CheckoutViewModel
    {
        
        [Required(ErrorMessage = "الاسم الكامل مطلوب")]
        public string ShippingFullName { get; set; }

        [Required(ErrorMessage = "العنوان مفصل مطلوب")]
        public string ShippingAddress { get; set; }

        [Required(ErrorMessage = "رقم الهاتف مطلوب")]
        [Phone]
        public string ShippingPhoneNumber { get; set; }

        public string Notes { get; set; } 

        public List<OrderItemViewModel> Items { get; set; } = new List<OrderItemViewModel>();

        public decimal SubTotal { get; set; }
        public decimal ShippingFee { get; set; } = 3.00m; 
        public decimal GrandTotal => SubTotal + ShippingFee;

        public string PaymentMethod { get; set; } = "CashOnDelivery";
    }

    public class OrderItemViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string Size { get; set; }
    }
}
