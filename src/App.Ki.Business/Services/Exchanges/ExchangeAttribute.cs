namespace App.Ki.Business.Services.Exchanges;

[AttributeUsage(AttributeTargets.Class)]
public class ExchangeAttribute : Attribute
{
    public string Name { get; }

    public ExchangeAttribute(string name)
    {
        Name = name;
    }
}