using App;

List<User> users = new List<User>();
List<Trade> trades = new List<Trade>();

users.Add(new User("Thea", "t.z@hotmail.se", "thea")); //testanvändare
users.Add(new User("Manuel", "manuel@gmail.com", "hej")); //testanavändare2
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
            Console.WriteLine("Press enter to try again..");
            Console.ReadLine();
        }
    }
    else
    { // borde man kunna se inventory?
        Console.WriteLine("-----Trading system-----");
        Console.WriteLine("1) Upload item to trade");
        Console.WriteLine("2) Browse items from other users");
        Console.WriteLine("3) Request trade");
        Console.WriteLine("4) Browse trade requests");
        Console.WriteLine("5) Handle trade requests");
        Console.WriteLine("6) Browse completed requests");
        Console.WriteLine("7) Log out");

        string choice = Console.ReadLine();

        switch (choice)
        {
            case "1": //Lägg till item på den inloggade användaren
                Console.Clear();
                Console.WriteLine("---Upload new item---");
                Console.Write("Item name:  ");
                string name = Console.ReadLine();
                Console.Write("Description:  ");
                string desc = Console.ReadLine();
                // skapa ett nytt item objekt med ägare = active_user:
                Item newItem = new Item(name, desc, active_user);
                //lägg in item i den inloggade användarens inventory:
                active_user.Items.Add(newItem);

                Console.WriteLine("Item added!");
                Console.WriteLine("Press enter to go back to the menu..");
                Console.ReadLine();
                break;
        }
    }
}
