using App;

List<User> users = new List<User>();
List<Trade> trades = new List<Trade>();
List<Item> items = new List<Item>();

users.Add(new User("Thea", "thea", "thea")); //testanvändare
users.Add(new User("Manuel", "manuel", "manuel")); //testanavändare2
users.Add(new User("Marcus", "marcus", "marcus")); // test3
users.Add(new User("Malin", "malin", "malin")); // test 4

//filhantering: läs in users om filen finns
if (File.Exists("users.txt"))
{
    string[] userLines = File.ReadAllLines("users.txt");
    foreach (string userRow in userLines)
    {
        string[] parts = userRow.Split(','); //Name, email, password
        if (parts.Length == 3)
        {
            string name = parts[0];
            string email = parts[1];
            string password = parts[2];
            users.Add(new User(name, email, password));
        }
    }
}

//filhantering:läsa in items
if (File.Exists("items.txt"))
{
    string[] itemLines = File.ReadAllLines("items.txt");
    foreach (string itemRow in itemLines)
    {
        string[] parts = itemRow.Split(',');
        if (parts.Length == 3)
        {
            string ownerEmail = parts[0];
            string itemName = parts[1];
            string desc = parts[2];

            // hitta ägaren via email
            User? owner = null;
            foreach (User user in users)
            {
                if (user.Email == ownerEmail)
                {
                    owner = user;
                    break;
                }
            }
            if (owner != null)
            {
                Item item = new Item(itemName, desc, owner);
                owner.Items.Add(item); //lägg i ägarens invetory
                items.Add(item);
            }
        }
    }
}

//Filhantering: Trades
if (File.Exists("trades.txt")) // kolla om trades.txt finns
{
    string[] tradeLines = File.ReadAllLines("trades.txt"); // läser alla rader i trades.txt
    int i = 0; // index för att gå igenom filen rad för rad

    while (i < tradeLines.Length) // loopa tills vi passerarr sista raden
    {
        string firstLine = tradeLines[i]; // läser in första raden för en trade
        if (firstLine == null || firstLine == "") // om raden är tom eller null
        {
            i++; // gå viade till nästa varv
            continue; // hoppa över resten och börja om på nästa rad
        }

        string[] firstParts = firstLine.Split(',');
        if (firstParts.Length == 2 && firstParts[0] != "-")
        {
            string senderName = firstParts[0];
            string receiverName = firstParts[1];

            // hitta sender
            User? sender = null;
            foreach (User user in users)
            {
                if (user.Name == senderName)
                {
                    sender = user;
                    break;
                }
            }

            // hitta receiver
            User? receiver = null;
            foreach (User user in users)
            {
                if (user.Name == receiverName)
                {
                    receiver = user;
                    break;
                }
            }

            List<Item> tradeItems = new List<Item>(); // lista med alla items i traden, wants och offered
            TradeStatus status = TradeStatus.Pending; // default om statusraden saknas
            i++; // hoppa till nästa rad

            while (i < tradeLines.Length) // looopa raden som tillhör denna trade
            {
                string row = tradeLines[i]; // aktuell rad
                if (row == null || row == "") // tom rad = hoppa
                {
                    i++; // gå till nästa rad
                    continue; // hoppa resten av varvet
                }

                string[] part = row.Split(',');
                // Statusrad: "-,Status"
                if (part.Length == 2 && part[0] == "-") // om första delen är " - " så är detta statusraden
                {
                    string low = part[1] == null ? "" : part[1].ToLower(); // ok med lowcaps
                    if (low == "accepted")
                    {
                        status = TradeStatus.Accepted; // sätt status accepted om accepted
                    }
                    else if (low == "denied")
                    {
                        status = TradeStatus.Denied; // sätt status denied om denied
                    }
                    else
                    {
                        status = TradeStatus.Pending; // allt annat blir pending
                    }
                    i++; // flytta indec efter statusraden
                    break; // klar med denna trade
                }
                // Itemrad
                else if (part.Length == 2)
                {
                    string itemName = part[0];
                    string ownerName = part[1];

                    User? owner = null;
                    foreach (User user in users)
                    {
                        if (user.Name == ownerName)
                        {
                            owner = user;
                            break;
                        }
                    }

                    if (owner != null) // om ägare hittad
                    {
                        foreach (Item item in owner.Items) // leta upp item via namn i ägarens lista
                        {
                            if (item.Name == itemName) // matcha på itemets namn
                            {
                                tradeItems.Add(item); // lägg till itemet i tradens item lista
                                break; // klart, lägg ej till dubbelt
                            }
                        }
                    }
                    i++; // gå vidare till nästa rad som hör till traden
                }
                else
                {
                    i++; // om okänd radform, hoppa vidare
                }
            }

            if (sender != null && receiver != null && tradeItems.Count > 0) //kontroll så allt hittats
            {
                trades.Add(new Trade(sender, receiver, tradeItems, status)); // skapa trade och lägg i lista
            }
        }
        else
        {
            i++; // om firstParts inte såg rätt ut, gå vidare
        }
    }
}

User? active_user = null; //om man sätter null så betyder det att är det ingen användare som är selected = utloggad

bool running = true;
while (running)
{
    if (active_user == null)
    {
        Console.Clear();
        Console.WriteLine("---Welcome to trading system!---");
        Console.WriteLine("1) Log in");
        Console.WriteLine("2) Make an account");

        string? pickoption = Console.ReadLine();
        switch (pickoption)
        {
            case "1":
                Console.Write("Email: ");
                string? email = Console.ReadLine();
                Console.Clear();

                Console.Write("Password: ");
                string? password = Console.ReadLine();
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
                string? newName = Console.ReadLine();
                Console.WriteLine("Email:  ");
                string? newEmail = Console.ReadLine();
                Console.WriteLine("Password:  ");
                string? newPassword = Console.ReadLine();
                //lägg till i minnet
                User newUser = new User(newName, newEmail, newPassword);
                users.Add(newUser);

                //Skapar en lista som ska sparas till filen
                List<string> newUserCreated = new List<string>();
                //läser in gamla rader (om filen finns) och kopiera in den i listan
                if (File.Exists("users.txt"))
                {
                    string[] oldLines = File.ReadAllLines("users.txt");
                    foreach (string line in oldLines)
                    {
                        newUserCreated.Add(line);
                    }
                }
                //lägg till den nya användaren sist
                newUserCreated.Add(newName + "," + newEmail + "," + newPassword);
                //skriv hela listan tillbaka till filen
                File.WriteAllLines("users.txt", newUserCreated);

                Console.WriteLine("Account created! Press enter to log in");
                Console.ReadLine();
                break;
        }
    }
    else
    { // kommentar till mig själv: borde man kunna se inventory?
        Console.Clear();
        Console.WriteLine("-----TRADING PROGRAM-----");
        Console.WriteLine("1) Upload item to trade");
        Console.WriteLine("2) See your inventory");
        Console.WriteLine("3) Browse items from other users");
        Console.WriteLine("4) Request trade");
        Console.WriteLine("5) Browse trade requests");
        Console.WriteLine("6) Handle trade requests");
        Console.WriteLine("7) Browse completed requests");
        Console.WriteLine("8) Log out");

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
                items.Add(newItem);

                //läs in gamla rader i fil om det finns
                List<string> itemLines = new List<string>();
                if (File.Exists("items.txt"))
                {
                    string[] existingItemRows = File.ReadAllLines("items.txt");
                    foreach (string row in existingItemRows)
                    {
                        itemLines.Add(row);
                    }
                }
                //lägg till den nya item sist
                itemLines.Add(active_user.Email + "," + name + "," + desc);
                File.WriteAllLines("items.txt", itemLines);

                Console.WriteLine("Item added!");
                Console.WriteLine("Press enter to go back to the menu..");
                Console.ReadLine();
                break;

            case "2":
                Console.Clear();
                Console.WriteLine("---Your inventory---");

                bool hasItem = false;
                foreach (Item it in active_user.Items)
                {
                    Console.WriteLine(it.Name + ": " + it.Description);
                    hasItem = true;
                }
                if (hasItem == false)
                {
                    Console.WriteLine("[no items in your inventory]");
                }
                Console.WriteLine("\nPress enter to go back");
                Console.ReadLine();
                break;

            case "3":

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

            case "4": // request trade
                Console.Clear();
                Console.WriteLine("---Request a trade---");

                // Fråga användaren vems items de vill kolla
                Console.Write("Owner name: ");
                string ownerName = Console.ReadLine();

                // Leta upp användaren med det namnet (inte den inloggade)
                User owner = null;
                foreach (User user in users)
                {
                    if (user != null && user != active_user && user.Name == ownerName)
                    {
                        owner = user;
                        break; // sluta leta när vi hittat rätt
                    }
                }

                // Om ingen användare hittades
                if (owner == null)
                {
                    Console.WriteLine("Owner not found, press enter to go back");
                    Console.ReadLine();
                    break;
                }

                // Om ägaren inte har några items
                if (owner.Items.Count == 0)
                {
                    Console.WriteLine(owner.Name + " has no items. Press enter to go back");
                    Console.ReadLine();
                    break;
                }

                // Skriv ut ägarens items
                Console.WriteLine("User " + owner.Name + " inventory:");
                foreach (Item it in owner.Items)
                {
                    Console.WriteLine(it.Name + ": " + it.Description);
                }

                // Fråga vilket item användaren vill ha
                Console.Write("Enter the item you want: ");
                string wantedName = Console.ReadLine();
                string wantedNameLower = wantedName == null ? "" : wantedName.ToLower();

                // Leta upp itemet i ägarens lista
                Item wantedItem = null;
                foreach (Item it in owner.Items)
                {
                    if (it.Name != null && it.Name.ToLower() == wantedNameLower)
                    {
                        wantedItem = it;
                        break;
                    }
                }

                // Om itemet inte hittades
                if (wantedItem == null)
                {
                    Console.WriteLine("Item not found, press enter to go back");
                    Console.ReadLine();
                    break;
                }

                // Om användaren inte har några egna items att erbjuda
                if (active_user.Items.Count == 0)
                {
                    Console.WriteLine("You have no items to offer. Press enter to go back");
                    Console.ReadLine();
                    break;
                }

                // Skriv ut användarens egna items med nummer
                Console.WriteLine("---YOUR ITEMS---");
                int showIdx = 1;
                foreach (Item it in active_user.Items)
                {
                    Console.WriteLine(showIdx + ") " + it.Name);
                    showIdx++;
                }

                // Lista för items man väljer att erbjuda
                List<Item> offeredItems = new List<Item>();

                // Låt användaren välja flera items (skriv index, enter för att avsluta)
                while (true)
                {
                    Console.WriteLine("Pick number (enter to finish): ");
                    string pick = Console.ReadLine();

                    if (string.IsNullOrEmpty(pick))
                    {
                        break; // avsluta när enter trycks
                    }
                    int i;
                    bool pickedNumber = int.TryParse(pick, out i);

                    if (pickedNumber && i > 0 && i <= active_user.Items.Count)
                    {
                        // Hämta item baserat på index
                        Item selectedItem = active_user.Items[i - 1];

                        // Undvik att lägga till samma item två gånger
                        if (offeredItems.Contains(selectedItem))
                        {
                            Console.WriteLine("That item is already selected..");
                        }
                        else
                        {
                            offeredItems.Add(selectedItem);
                            Console.WriteLine("Added: " + selectedItem.Name);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Please enter a valid number or press enter to finish");
                    }
                }

                // Skapa trade objektet
                Trade trade = new Trade(
                    active_user,
                    owner,
                    wantedItem,
                    offeredItems,
                    TradeStatus.Pending
                );
                trades.Add(trade);

                // Bygg upp en sträng av alla offered items
                string offeredNamesStr = "";
                if (offeredItems.Count > 0)
                {
                    foreach (Item itof in trade.OfferedItem)
                    {
                        if (offeredNamesStr == "")
                        {
                            offeredNamesStr = itof.Name;
                        }
                        else
                        {
                            offeredNamesStr += "|" + itof.Name;
                        }
                    }
                }

                // Spara traden till fil
                string newTradeRow =
                    trade.Sender.Email
                    + ","
                    + trade.Receiver.Email
                    + ","
                    + trade.RequestedItem.Name
                    + ","
                    + offeredNamesStr
                    + ","
                    + trade.Status.ToString();

                File.AppendAllText("trades.txt", newTradeRow + "\n");

                Console.WriteLine("Trade request sent...");
                Console.WriteLine("Status: [pending..]");
                Console.WriteLine("Press enter to go back to menu");
                Console.ReadLine();
                break;

            case "5":
                Console.Clear();
                Console.WriteLine("--Incoming trade requests---");

                // Se om vi hittar några inkommande trades
                bool incomingTr = false;

                foreach (Trade t in trades)
                {
                    // Kolla om traden är till den inloggade användaren och fortfarande pending
                    if (
                        t != null
                        && t.Receiver != null
                        && t.Receiver.Email == active_user.Email
                        && t.Status == TradeStatus.Pending
                    )
                    {
                        Console.WriteLine(t.Sender.Name + " wants your " + t.RequestedItem.Name);

                        // Om inga offered items finns
                        if (t.OfferedItem.Count == 0)
                        {
                            Console.WriteLine("They offer: [nothing]");
                        }
                        else
                        {
                            Console.WriteLine("They offer:");
                            foreach (Item itof in t.OfferedItem)
                            {
                                Console.WriteLine("- " + itof.Name);
                            }
                        }

                        Console.WriteLine("Status: " + t.Status);
                        Console.WriteLine();
                        incomingTr = true;
                    }
                }

                // Om inga inkommande trades hittades
                if (incomingTr == false)
                {
                    Console.WriteLine("[no requests to show]");
                }

                Console.WriteLine("\n---Your outgoing requests---");

                // se om vi hittar några utgående trades
                bool outgoingTr = false;

                foreach (Trade t in trades)
                {
                    // Kolla om traden är skickad av den inloggade användaren
                    if (t != null && t.Sender != null && t.Sender.Email == active_user.Email)
                    {
                        Console.WriteLine("Sent to " + t.Receiver.Name);
                        Console.WriteLine("Wants: " + t.RequestedItem.Name);

                        if (t.OfferedItem.Count == 0)
                        {
                            Console.WriteLine("You offered: [nothing]");
                        }
                        else
                        {
                            Console.WriteLine("You offered:");
                            foreach (Item itof in t.OfferedItem)
                            {
                                Console.WriteLine("- " + itof.Name);
                            }
                        }

                        Console.WriteLine("Status: " + t.Status);
                        Console.WriteLine();
                        outgoingTr = true;
                    }
                }

                // Om inga utgående trades hittades
                if (outgoingTr == false)
                {
                    Console.WriteLine("[no outgoing requests]");
                }

                Console.WriteLine("\nPress enter to go back to menu");
                Console.ReadLine();
                break;

            case "6":
                Console.Clear();
                Console.WriteLine("--Handle trade requests--");

                //se om vi hittade minst en pending trade till inloggade
                bool foundAny = false;

                // Loopa igenom alla trades
                foreach (Trade t in trades)
                {
                    // Visa bara trades som är pending och där inloggade är mottagare
                    if (
                        t != null
                        && t.Receiver != null
                        && t.Receiver.Email == active_user.Email
                        && t.Status == TradeStatus.Pending
                    )
                    {
                        foundAny = true;

                        // Visa vad som efterfrågas
                        Console.WriteLine("From: " + t.Sender.Name);
                        Console.WriteLine("Wants: " + t.RequestedItem.Name);

                        // Visa erbjudna items
                        if (t.OfferedItem.Count == 0)
                        {
                            Console.WriteLine("They offer: [nothing]");
                        }
                        else
                        {
                            Console.WriteLine("They offer:");
                            foreach (Item itof in t.OfferedItem)
                            {
                                Console.WriteLine("- " + itof.Name);
                            }
                        }

                        // Fråga om beslut
                        Console.WriteLine("Accept (A) or Deny (D)?");
                        string decision = Console.ReadLine();
                        if (decision == null)
                            decision = "";
                        decision = decision.ToLower();

                        if (decision == "a")
                        {
                            // Byt ägare, requested item flyttar från receiver till sender
                            t.Receiver.Items.Remove(t.RequestedItem);
                            t.Sender.Items.Add(t.RequestedItem);

                            // Alla offered flyttar från sender till receiver
                            foreach (Item itof in t.OfferedItem)
                            {
                                t.Sender.Items.Remove(itof);
                                t.Receiver.Items.Add(itof);
                            }

                            // Markera som Accepted
                            t.Status = TradeStatus.Accepted;
                            Console.WriteLine("Trade [accepted] and items swapped");

                            // Uppdatera items.txt (skriv om hela filen från alla users)
                            List<string> itemFileLines = new List<string>();
                            foreach (User u in users)
                            {
                                foreach (Item it in u.Items)
                                {
                                    itemFileLines.Add(
                                        u.Email + "," + it.Name + "," + it.Description
                                    );
                                }
                            }
                            File.WriteAllLines("items.txt", itemFileLines);
                        }
                        else if (decision == "d")
                        {
                            // Markera som Denied (ingen flytt av items)
                            t.Status = TradeStatus.Denied;
                            Console.WriteLine("Trade denied.");
                        }
                        else
                        {
                            // Ogiltigt val, gör inget
                            Console.WriteLine("No action taken..");
                        }

                        Console.WriteLine(); // luft i program
                    }
                }

                // Om inga pending hittades
                if (foundAny == false)
                {
                    Console.WriteLine("You have no pending trade requests.");
                }

                // Spara ALLA trades till trades.txt
                List<string> tradeLinesToSave = new List<string>();
                foreach (Trade tr in trades)
                {
                    // Bygg offered names
                    string offeredNames = "";
                    int idx = 0;
                    int total = tr.OfferedItem.Count;
                    while (idx < total)
                    {
                        Item itof = tr.OfferedItem[idx];
                        if (offeredNames == "")
                        {
                            offeredNames = itof.Name;
                        }
                        else
                        {
                            offeredNames = offeredNames + "|" + itof.Name;
                        }
                        idx = idx + 1;
                    }

                    string row =
                        tr.Sender.Email
                        + ","
                        + tr.Receiver.Email
                        + ","
                        + tr.RequestedItem.Name
                        + ","
                        + offeredNames
                        + ","
                        + tr.Status.ToString();

                    tradeLinesToSave.Add(row);
                }
                File.WriteAllLines("trades.txt", tradeLinesToSave);

                Console.WriteLine("Press enter to go back");
                Console.ReadLine();
                break;

            case "7":
                Console.Clear();
                Console.WriteLine("---Completed trades---");

                // se om något finns att visa
                bool anyFound = false;

                foreach (Trade t in trades)
                {
                    // Completed = Accepted eller Denied
                    if (t.Status == TradeStatus.Accepted || t.Status == TradeStatus.Denied)
                    {
                        anyFound = true;

                        Console.WriteLine("From: " + t.Sender.Name + " to " + t.Receiver.Name);
                        Console.WriteLine("Wanted: " + t.RequestedItem.Name);

                        // Visa offereditems
                        if (t.OfferedItem.Count == 0)
                        {
                            Console.WriteLine("They offered: [nothing]");
                        }
                        else
                        {
                            Console.WriteLine("They offered:");
                            foreach (Item itof in t.OfferedItem)
                            {
                                Console.WriteLine("- " + itof.Name);
                            }
                        }

                        Console.WriteLine("Status: " + t.Status);
                        Console.WriteLine();
                    }
                }

                if (anyFound == false)
                {
                    Console.WriteLine("[no completed trades]");
                }

                Console.WriteLine("Press enter to go back");
                Console.ReadLine();
                break;

            case "8":
            {
                Console.Clear();
                Console.WriteLine("Logging out...");
                active_user = null; //tillbaka till inloggning
                Console.WriteLine("You are now logged out, press enter");
                Console.ReadLine();
                break;
            }
        }
    }
}
