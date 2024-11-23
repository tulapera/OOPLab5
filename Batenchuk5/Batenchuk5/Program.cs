using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

public enum ProductType
{
    Electronics,
    Food,
    Clothing,
    Furniture
}

public class Product
{
    public int WarehouseNumber { get; set; }
    public ProductType ProductType { get; set; }
    public string ProductCode { get; set; }
    public string Name { get; set; }
    public DateTime ArrivalDate { get; set; }
    public int ShelfLifeDays { get; set; }
    public int Quantity { get; set; }
    public decimal PricePerUnit { get; set; }
}

class Program
{
    private const string FileName = "warehouse_data.xml";

    static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        while (true)
        {
            Console.WriteLine("\n1. Додати новий товар");
            Console.WriteLine("2. Показати всі товари");
            Console.WriteLine("3. Пошук товарів");
            Console.WriteLine("4. Вийти");
            Console.Write("Виберіть опцію: ");
            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    AddNewProduct();
                    break;
                case "2":
                    ShowAllProducts();
                    break;
                case "3":
                    SearchProducts();
                    break;
                case "4":
                    Console.WriteLine("До побачення!");
                    return;
                default:
                    Console.WriteLine("Невірний вибір, спробуйте ще раз.");
                    break;
            }
        }
    }

    static void AddNewProduct()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        try
        {
            Console.Write("Номер складу: ");
            int warehouseNumber = int.Parse(Console.ReadLine());

            Console.Write($"Тип товару ({string.Join(", ", Enum.GetNames(typeof(ProductType)))}): ");
            ProductType productType = (ProductType)Enum.Parse(typeof(ProductType), Console.ReadLine(), true);

            Console.Write("Код товару: ");
            string productCode = Console.ReadLine();

            Console.Write("Найменування товару: ");
            string name = Console.ReadLine();

            Console.Write("Дата появи (YYYY-MM-DD): ");
            DateTime arrivalDate = DateTime.Parse(Console.ReadLine());

            Console.Write("Термін збереження (днів): ");
            int shelfLifeDays = int.Parse(Console.ReadLine());

            Console.Write("Кількість одиниць: ");
            int quantity = int.Parse(Console.ReadLine());

            Console.Write("Ціна за одиницю: ");
            decimal pricePerUnit = decimal.Parse(Console.ReadLine());

            var product = new Product
            {
                WarehouseNumber = warehouseNumber,
                ProductType = productType,
                ProductCode = productCode,
                Name = name,
                ArrivalDate = arrivalDate,
                ShelfLifeDays = shelfLifeDays,
                Quantity = quantity,
                PricePerUnit = pricePerUnit
            };

            AppendToFile(product);
            Console.WriteLine("Товар успішно додано!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Помилка: {ex.Message}");
        }
    }

    static void ShowAllProducts()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        var products = ReadFromFile();
        if (products.Any())
        {
            foreach (var product in products)
            {
                Console.WriteLine($"Номер складу: {product.WarehouseNumber}");
                Console.WriteLine($"Тип товару: {product.ProductType}");
                Console.WriteLine($"Код товару: {product.ProductCode}");
                Console.WriteLine($"Найменування товару: {product.Name}");
                Console.WriteLine($"Дата появи: {product.ArrivalDate:yyyy-MM-dd}");
                Console.WriteLine($"Термін збереження: {product.ShelfLifeDays} днів");
                Console.WriteLine($"Кількість одиниць: {product.Quantity}");
                Console.WriteLine($"Ціна за одиницю: {product.PricePerUnit} грн");
                Console.WriteLine();
            }
        }
        else
        {
            Console.WriteLine("Немає даних для відображення.");
        }
    }

    static void SearchProducts()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        var products = ReadFromFile();

        Console.WriteLine("\n1. Пошук за номером складу");
        Console.WriteLine("2. Пошук за типом товару");
        Console.WriteLine("3. Пошук за найменуванням");
        Console.Write("Виберіть опцію: ");
        var choice = Console.ReadLine();

        List<Product> results = new();

        switch (choice)
        {
            case "1":
                Console.Write("Введіть номер складу: ");
                int warehouseNumber = int.Parse(Console.ReadLine());
                results = products.Where(p => p.WarehouseNumber == warehouseNumber).ToList();
                break;

            case "2":
                Console.Write($"Введіть тип товару ({string.Join(", ", Enum.GetNames(typeof(ProductType)))}): ");
                ProductType productType = (ProductType)Enum.Parse(typeof(ProductType), Console.ReadLine(), true);
                results = products.Where(p => p.ProductType == productType).ToList();
                break;

            case "3":
                Console.Write("Введіть найменування товару: ");
                string name = Console.ReadLine().ToLower();
                results = products.Where(p => p.Name.ToLower().Contains(name)).ToList();
                break;

            default:
                Console.WriteLine("Невірний вибір.");
                return;
        }

        if (results.Any())
        {
            foreach (var product in results)
            {
                Console.WriteLine($"Номер складу: {product.WarehouseNumber}");
                Console.WriteLine($"Тип товару: {product.ProductType}");
                Console.WriteLine($"Код товару: {product.ProductCode}");
                Console.WriteLine($"Найменування товару: {product.Name}");
                Console.WriteLine($"Дата появи: {product.ArrivalDate:yyyy-MM-dd}");
                Console.WriteLine($"Термін збереження: {product.ShelfLifeDays} днів");
                Console.WriteLine($"Кількість одиниць: {product.Quantity}");
                Console.WriteLine($"Ціна за одиницю: {product.PricePerUnit}");
                Console.WriteLine();
            }
        }
        else
        {
            Console.WriteLine("Товари не знайдено.");
        }
    }

    static List<Product> ReadFromFile()
    {
        FileInfo fileInfo = new FileInfo(FileName);
        if (!fileInfo.Exists) return new List<Product>();

        var serializer = new XmlSerializer(typeof(List<Product>));
        using (var stream = fileInfo.OpenRead())
        {
            return (List<Product>)serializer.Deserialize(stream) ?? new List<Product>();
        }
    }

    static void AppendToFile(Product product)
    {
        var products = ReadFromFile();
        products.Add(product);
        WriteToFile(products);
    }

    static void WriteToFile(List<Product> products)
    {
    var serializer = new XmlSerializer(typeof(List<Product>));
        using (var stream = new FileStream(FileName, FileMode.Create))
        {
            serializer.Serialize(stream, products);
        }
    }
}
