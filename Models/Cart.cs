using QFD.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QFD.Models
{
    public class Cart
    {
        [Key]
        public int CartId { get; set; }

        private List<CartProduct> CartProducts;

        public Cart()
        {
            CartProducts = new List<CartProduct>();
        }

        public void AddCartProduct(CartProduct CartProduct)
        {
            CartProducts.Add(CartProduct);
            Console.WriteLine($"{CartProduct.Name} added to cart.");
        }

        public void RemoveCartProduct(CartProduct CartProduct)
        {
            if (CartProducts.Remove(CartProduct))
            {
                Console.WriteLine($"{CartProduct.Name} removed from cart.");
            }
            else
            {
                Console.WriteLine($"{CartProduct.Name} not found in cart.");
            }
        }

        public void ViewCart()
        {
            if (CartProducts.Count == 0)
            {
                Console.WriteLine("Cart is empty.");
            }
            else
            {
                Console.WriteLine("CartProducts in the cart:");
                foreach (var CartProduct in CartProducts)
                {
                    Console.WriteLine($"{CartProduct.Name} - {CartProduct.Price:C}");
                }
            }
        }

        public decimal CalculateTotal()
        {
            decimal total = 0;
            foreach (var CartProduct in CartProducts)
            {
                total += CartProduct.Price;
            }
            return total;
        }
    }

    public class CartProduct
    {
        public string Name { get; }
        public decimal Price { get; }

        public CartProduct(string name, decimal price)
        {
            Name = name;
            Price = price;
        }
    }
}