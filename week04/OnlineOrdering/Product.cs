class Product
{
    private String _name;
    private String _productId;
    private Double _price;
    private int _quantity;

    public Product(string name, string productId, double price, int quantity)
    {
        _name = name;
        _productId = productId;
        _price = price;
        _quantity = quantity;
    }

    public Double GetTotalCost()
    {
        return _price * _quantity;
    }

    public String GetName()
    {
        return _name;
    }

    public void SetName(String name)
    {
        _name = name;
    }

    public String GetProductId()
    {
        return _productId;
    }

    public void SetProductId(String productId)
    {
        _productId = productId;
    }

    public double GetPrice()
    {
        return _price;
    }

    public void SetPrice(double price)
    {
        _price = price;
    }

    public int GetQuantity()
    {
        return _quantity;
    }

    public void SetQuantity(int quantity)
    {
        _quantity = quantity;
    }
}