namespace App;

public class User
{
    public string Name;
    public string Email;
    string _password;

    public List<Item> Items = new List<Item>(); //varje user ska ha egen lista

    public User(string name, string email, string password)
    {
        Name = name;
        Email = email;
        _password = password;
    }

    public bool TryLogin(string email, string password)
    {
        return email == Email && password == _password;
    }
}
