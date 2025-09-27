using App;

List<User> users = new List<User>();

users.Add(new User("Thea", "t.z@hotmail.se", "thea")); //testanvändare
User? active_user = null; //om man sätter null så betyder det att är det ingen användare som är selected = utloggad

bool running = true;
while (running)
{
    Console.Clear();

    if (active_user == null)
    {
        Console.Write("Email: ");
        string email = Console.ReadLine();
        Console.Clear();

        Console.Write("Password: ");
        string password = Console.ReadLine();
        Console.Clear();

        foreach (User user in users) //kollar igenom alla användare
        {
            if (user.TryLogin(email, password))
            {
                active_user = user;
                break; //sluta leta om vi hittar rätt
            }
        }
        if (active_user == null) // om ingen användare hittas
        {
            Console.WriteLine("Wrong email or password");
            Console.WriteLine("Press space to try again..");
            Console.ReadLine();
        }
    }
    else
    {
        Console.WriteLine("-----Trading system-----");
    }
}
