using App;

List<User> users = new List<User>();
List<Trade> trades = new List<Trade>();

users.Add(new User("Thea", "t.z@hotmail.se", "thea")); //testanvändare
users.Add(new User("Manuel", "manuel@gmail.com", "hej")); //testanavändare2
users.Add(new User("Marcus", "marcus@hotmail.com", "hej")); // test3
users.Add(new User("Malin", "malin@hotmail.se", "malin")); // test 4

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
                Console.WriteLine("---Upload item to trade---");
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

            case "2":
                Console.Clear();
                Console.WriteLine("---Browse items from other users---");

                int count = 0; //räknar hur många andra användare vi visar

                //Loopa igenom alla användare
                for (int i = 0; i < users.Count; i++)
                {
                    if (users[i] == active_user)
                        continue; //hoppa över inloggad användare

                    Console.WriteLine("User" + " " + users[i].Name + " " + "inventory:");

                    if (users[i].Items.Count == 0)
                    {
                        Console.WriteLine("There is no items to show..");
                    }
                    else
                    {
                        foreach (Item item in users[i].Items)
                        {
                            Console.WriteLine("User:" + " " + item.Name + ": " + item.Description);
                        }
                    }

                    count++; //vi visar en användare
                    Console.WriteLine();
                }
                //om count är 0 = inga andra användare
                if (count == 0)
                {
                    Console.WriteLine("No other users yet");
                }
                Console.WriteLine("Press enter to go back to menu");
                Console.ReadLine();
                break;
        }
    }
}
