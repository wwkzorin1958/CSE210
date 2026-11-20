using System;

class Program
{
    static void Main(string[] args)
    {
        Address address1 = new Address("123 Main Street", "Fanling", "New Territories", "Hong Kong");
        Customer customer1 = new Customer("Alice Tam", address1);

        List<Product> products1 = new List<Product>
        {
            new Product("Laptop", "L100", 800.00, 1),
            new Product("Mouse", "M200", 25.00, 2),
            new Product("Keyboard", "K300", 50.00, 1)
        };
        Order order1 = new Order(customer1, products1);

        Address address2 = new Address("45 High Street", "Wanchai", "Hong Kong", "Hong Kong");
        Customer customer2 = new Customer("Nick Welch", address2);

        List<Product> products2 = new List<Product>
        {
            new Product("Monitor", "MO400", 300.00, 1),
            new Product("USB Cable", "U500", 12.00, 3)
        };
        Order order2 = new Order(customer2, products2);

        DisplayOrder(order1);
        DisplayOrder(order2);
    }

    static void DisplayOrder(Order order)
    {
        Console.WriteLine("Packing Label:");
        Console.WriteLine(order.GetPackingLabel());
        Console.WriteLine("\nShipping Label:");
        Console.WriteLine(order.GetShippingLabel());
        Console.WriteLine($"\nTotal Price: ${order.CalculateTotalPrice():0.00}");
        Console.WriteLine(new string('-', 40));
    }
}