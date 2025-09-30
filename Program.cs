using App;

List<User> users = new List<User>();
List<Trade> trades = new List<Trade>();
List<Item> items = new List<Item>();

users.Add(new User("Thea", "thea", "thea")); //testanvändare
users.Add(new User("Manuel", "manuel", "manuel")); //testanavändare2
users.Add(new User("Marcus", "marcus", "marcus")); // test3
users.Add(new User("Malin", "malin", "malin")); // test 4

User? active_user = null; //om man sätter null så betyder det att är det ingen användare som är selected = utloggad

bool running = true;
while (running) //kommentar till mig själv: borde man kunna skapa konto?
{
    if (active_user == null)
    {
        Console.Clear();
        Console.WriteLine("---Welcome to trading system!---"); //// kommentar till mig själv:måste lägga till att registrera account
        Console.WriteLine("1) Log in");
        Console.WriteLine("2) Make an account");

        string pickoption = Console.ReadLine();
        switch (pickoption)
        {
            case "1":
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
                    Console.WriteLine("Press enter to try again or make an account..");
                    Console.ReadLine();
                }
                break;

            case "2":
                Console.Clear();
                Console.WriteLine("---Lets create an account!---");
                Console.WriteLine("Firstname: ");
                string newName = Console.ReadLine();
                Console.WriteLine("Email:  ");
                string newEmail = Console.ReadLine();
                Console.WriteLine("Password:  ");
                string newPassword = Console.ReadLine();
                User newUser = new User(newName, newEmail, newPassword);
                users.Add(newUser);

                Console.WriteLine("Account created! Press enter to log in");
                Console.ReadLine();
                break;
        }
    }
    else
    { // kommentar till mig själv: borde man kunna se inventory?
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
                foreach (User user in users)
                {
                    if (user != active_user) //visa bara andra användare
                    {
                        Console.WriteLine("User " + user.Name + " inventory:");
                        bool found = false; //utgå ifrån att vi inte hittar några items
                        foreach (Item item in user.Items)
                        {
                            Console.WriteLine(item.Name + ": " + item.Description);
                            found = true; //så fort vi hittar ett item ändra till true
                        }
                        if (found == false) //om vi inte hittar några
                        {
                            Console.WriteLine("[no items]");
                        }
                        Console.WriteLine();
                    }
                }
                Console.WriteLine("Press enter to go back to menu");
                Console.ReadLine();
                break;

            case "3":

                Console.Clear();
                Console.WriteLine("---Request a trade---");
                //fråga vem man vill skicka en request till
                Console.Write("Owner name: ");
                string ownerName = Console.ReadLine();

                User owner = null; //spara ägaren
                //Leta efter användaren i listan:
                foreach (User user in users)
                {
                    if (user != active_user && user.Name == ownerName)
                    {
                        owner = user;
                        break;
                    }
                }
                if (owner == null)
                {
                    Console.WriteLine("Owner not found, press enter to go back");
                    Console.ReadLine();
                    break;
                }
                //kolla om owner har några items
                bool foundOwnerItem = false;
                foreach (Item item in owner.Items)
                {
                    Console.WriteLine(item.Name + ": " + item.Description);
                }
                Console.Write("Enter exact item name you want: ");
                string wantedName = Console.ReadLine();

                Item wantedItem = null;
                foreach (Item item in owner.Items)
                {
                    if (item.Name == wantedName)
                    {
                        wantedItem = item;
                        break;
                    }
                }
                if (wantedItem == null)
                {
                    Console.WriteLine("Item not found, press enter to go back");
                    Console.ReadLine();
                    break;
                }
                Trade trade = new Trade(active_user, owner, wantedItem);
                trades.Add(trade);
                Console.WriteLine("Trade request sent [pending]");
                Console.ReadLine();
                break;

            case "4":
                Console.Clear();
                Console.WriteLine("Incoming trade requests to you:  ");
                foreach (Trade t in trades) // loopa igenom alla trades
                {
                    if (t.Receiver == active_user) //om den inloggade är mottagare
                    { //skriv ut vem som skickade samt vilket item och status
                        Console.WriteLine(
                            t.Sender.Name + " " + "wants" + t.Item.Name + "[" + t.Status + "]"
                        );
                    }
                }
                Console.WriteLine();
                Console.WriteLine("Here is your outgoing requests:  ");
                foreach (Trade t in trades)
                {
                    if (t.Sender == active_user) //om den inloggade är avsändare
                    {
                        Console.WriteLine(
                            "Sent to"
                                + t.Receiver.Name
                                + " for "
                                + t.Item.Name
                                + "["
                                + t.Status
                                + "]"
                        );
                    }
                }
                Console.WriteLine("\nPress enter to go back to menu");
                Console.ReadLine();
                break;

            // case "5":
            //     Console.Clear();
            //     Console.WriteLine("--Handle trade requests--");
            // //visa alla pending requests som är skickade till inloggad användare
        }
    }
}
