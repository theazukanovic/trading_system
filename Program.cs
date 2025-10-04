using App;

List<User> users = new List<User>(); // minneslista med alla användare
List<Trade> trades = new List<Trade>(); // minneslista med alla trades
List<Item> items = new List<Item>(); // minneslista med alla items

users.Add(new User("Thea", "thea", "thea")); // testanvändare
users.Add(new User("Manuel", "manuel", "manuel")); //testanvändare
users.Add(new User("Marcus", "marcus", "marcus")); //testanvändare
users.Add(new User("Malin", "malin", "malin")); //testanvändare

//Filhantering: Users
if (File.Exists("users.txt")) // kolla om users.txt finns
{
    string[] userLines = File.ReadAllLines("users.txt"); // läs alla rader till en array
    foreach (string userRow in userLines) // loopa rad för rad
    {
        string[] parts = userRow.Split(','); // dela på name, email, password
        if (parts.Length == 3)
        {
            string name = parts[0];
            string email = parts[1];
            string password = parts[2];
            users.Add(new User(name, email, password)); // skapa User och lägg i listan
        }
    }
}

//Filhantering: Items
if (File.Exists("items.txt")) // kolla om items.txt finns
{
    string[] itemLines = File.ReadAllLines("items.txt"); // läs alla rader
    foreach (string itemRow in itemLines) // loopa rad för rad
    {
        string[] parts = itemRow.Split(','); // delar på ownerEmail, itemName, description
        if (parts.Length == 3) // kontrollera fält
        {
            string ownerEmail = parts[0];
            string itemName = parts[1];
            string desc = parts[2];

            User? owner = null; // kommer peka på hittad ägare eller null
            foreach (User user in users) // hitta rätt User via email
            {
                if (user.Email == ownerEmail) // jämför email
                {
                    owner = user; // spara träff
                    break; // sluta leta efter vi hittar rätt
                }
            }
            if (owner != null) // om ägare hittades
            {
                Item item = new Item(itemName, desc, owner); // skapa item kopplad till owner
                owner.Items.Add(item); // lägg i ägarens inventory
                items.Add(item); // lägga in i itemslistan
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

User? active_user = null; // null = ingen inloggad användare

bool running = true; // styr huvudloopen
while (running) // huvudloop för programmet
{
    if (active_user == null)
    {
        Console.Clear();
        Console.WriteLine("---Welcome to trading platform!---");
        Console.WriteLine("1) Log in");
        Console.WriteLine("2) Make an account");
        Console.WriteLine("0) EXIT program...");

        string? pickoption = Console.ReadLine(); // menyval
        switch (pickoption) // hantera valet
        {
            case "1": // logga in
                Console.Write("Email: ");
                string? email = Console.ReadLine(); // läs email
                Console.Clear();

                Console.Write("Password: ");
                string? password = Console.ReadLine(); // läs lösen
                Console.Clear();

                foreach (User user in users) // leta användare som matchar
                {
                    if (user.TryLogin(email, password)) // logga in kontroll
                    {
                        active_user = user; // sätt inloggas användare
                        break; // sluta leta om vi hittat något
                    }
                }
                if (active_user == null) // om ingen matchade
                {
                    Console.WriteLine("Wrong email or password");
                    Console.WriteLine("Press enter to try again or make an account..");
                    Console.ReadLine();
                }
                break;

            case "2": // skapa nytt konto
                Console.Clear();
                Console.WriteLine("---Lets create an account!---");
                Console.WriteLine("Firstname: ");
                string? newName = Console.ReadLine(); // namn för ny user
                Console.WriteLine("Email:  ");
                string? newEmail = Console.ReadLine(); // email för ny user
                Console.WriteLine("Password:  ");
                string? newPassword = Console.ReadLine(); // lösen för ny user

                User newUser = new User(newName, newEmail, newPassword); //skaåar ny user
                users.Add(newUser); // lägger till i user listan

                List<string> newUserCreated = new List<string>(); // lista med alla rader som ska skrivas till fil
                if (File.Exists("users.txt")) // om fil finns
                {
                    string[] oldLines = File.ReadAllLines("users.txt"); // hämta redan befintliga rader
                    foreach (string line in oldLines) // loppa dem
                    {
                        newUserCreated.Add(line); // behåll gamla rader
                    }
                }
                newUserCreated.Add(newName + "," + newEmail + "," + newPassword); // lägg till den nya sist
                File.WriteAllLines("users.txt", newUserCreated); // skriv tillbaka allt

                Console.WriteLine("Account created! Press enter to log in");
                Console.ReadLine();
                break;

            case "0": //Exit program
                running = false; //running = false är för att lämna huvudloop, alltså hela programmet
                break;
        }
    }
    else // här inne är någon inloggad
    {
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

        string? choice = Console.ReadLine(); // läs menyval

        switch (choice) // hantera menyval
        {
            // Lägg till item
            case "1":
                Console.Clear();
                Console.WriteLine("---Upload item to trade---");
                Console.Write("Item name:  ");
                string? name = Console.ReadLine(); // nytt items namn
                Console.Write("Description:  ");
                string? desc = Console.ReadLine(); // nytt items beskrivning

                Item newItem = new Item(name, desc, active_user); // skapa item kopplat till inloggade
                active_user.Items.Add(newItem); // lägg i ägarens inventory
                items.Add(newItem); // lägg även i lista

                List<string> itemLines = new List<string>(); // temporär lista för items.txt
                if (File.Exists("items.txt")) // om fil finns
                {
                    string[] existingItemRows = File.ReadAllLines("items.txt"); // läs befintliga rader
                    foreach (string row in existingItemRows) // loopa dem
                    {
                        itemLines.Add(row); // behåll gamla rader
                    }
                }
                itemLines.Add(active_user.Email + "," + name + "," + desc); // lägg ny rad sist
                File.WriteAllLines("items.txt", itemLines); // skriv tillbaka allt

                Console.WriteLine("Item added!");
                Console.WriteLine("Press enter to go back to the menu..");
                Console.ReadLine();
                break;

            case "2":
                Console.Clear();
                Console.WriteLine("---Your inventory---");

                bool hasItem = false; // flagga om vi skrivit ut minst ett item
                foreach (Item item in active_user.Items) // loopa mina items
                {
                    Console.WriteLine(item.Name + ": " + item.Description);
                    hasItem = true; // markerar att det finns något att visa
                }
                if (hasItem == false) // om inget skrevs
                {
                    Console.WriteLine("[no items in your inventory]");
                }
                Console.WriteLine("\nPress enter to go back");
                Console.ReadLine();
                break;

            case "3":
                Console.Clear();
                Console.WriteLine("---Browse items from other users---");
                foreach (User user in users) // loopa alla användare
                {
                    if (user != active_user) // hoppa mina egna grejer
                    {
                        Console.WriteLine("User " + user.Name + " inventory:");
                        bool found = false; // flagga om användaren hade minst ett item
                        foreach (Item item in user.Items) // skriv alla deras items
                        {
                            Console.WriteLine(item.Name + ": " + item.Description);
                            found = true; // nu vet vi att listan inte är tom
                        }
                        if (found == false) // om tomt
                        {
                            Console.WriteLine("[no items]");
                        }
                        Console.WriteLine(); // endast för läsbarhet
                    }
                }
                Console.WriteLine("Press enter to go back to menu");
                Console.ReadLine();
                break;

            case "4":
                Console.Clear();
                Console.WriteLine("---Request a trade---");
                Console.Write("Owner name: ");
                string? ownerName = Console.ReadLine(); // vem jag vill be om saker från

                User? owner = null; // refererart till den ägare man vill tradea med
                foreach (User user in users) // leta upp rätt user via namn
                {
                    if (user != null && user != active_user && user.Name == ownerName) // ej mig själv
                    {
                        owner = user;
                        break;
                    }
                }
                if (owner == null || owner.Items.Count == 0) // om inte hittad eller saknar items
                {
                    Console.WriteLine("Owner not found or has no items. Press enter to go back");
                    Console.ReadLine();
                    break;
                }

                Console.WriteLine("User " + owner.Name + " inventory:");
                for (int i = 0; i < owner.Items.Count; i++) // numrera ägarens items för val
                {
                    Item item = owner.Items[i];
                    Console.WriteLine((i + 1) + ") " + item.Name + " : " + item.Description); // visa med nummer
                }

                List<Item> requestedItems = new List<Item>(); // hjälplista för vad jag ber om
                while (true) // låt mig välja flera
                {
                    Console.Write("Pick number to request and press enter to finish: ");
                    string? input = Console.ReadLine(); // läs val eller tom rad

                    if (input == "") // tom rad = klart
                    {
                        break;
                    }
                    int number; // valt nummer
                    bool numb = int.TryParse(input, out number); // försök tolka tal
                    if (numb && number > 0 && number <= owner.Items.Count) // giltigt val
                    {
                        Item item = owner.Items[number - 1]; // hämta valt item

                        bool isChosen = false; // undvik dubbelt val av samma
                        foreach (Item it in requestedItems) // kolla om item redan är valt
                        {
                            if (it == item) // jämför referens
                            {
                                isChosen = true; // markera att det redan finns
                                break;
                            }
                        }
                        if (!isChosen) // om inte redan vald
                        {
                            requestedItems.Add(item); // lägg till i wants
                            Console.WriteLine("Requested: " + item.Name); // bekräftelse
                        }
                        else
                        {
                            Console.WriteLine("That item is already selected."); // info om dubblett
                        }
                    }
                    else // ogiltigt val
                    {
                        Console.WriteLine("Please enter a valid number.");
                    }
                }
                if (requestedItems.Count == 0) // man måste välja minst en att requesta för trade
                {
                    Console.WriteLine("You must request at least one item");
                    Console.ReadLine();
                    break;
                }

                if (active_user.Items.Count > 0) // visa mina egna som jag kan erbjuda
                {
                    Console.WriteLine("---YOUR ITEMS---");
                    for (int i = 0; i < active_user.Items.Count; i++) // numrera mina items
                    {
                        Item item = active_user.Items[i]; // hämtar items
                        Console.WriteLine((i + 1) + ") " + item.Name); // skriver rad
                    }
                }
                else // om jag inte har något att erbjuda
                {
                    Console.WriteLine("You have no items to offer"); // endast info man behlver ej erbjuda
                }

                List<Item> offeredItems = new List<Item>(); // hjälplista med erbjudna items
                while (true) // välj flera tills tom rad
                {
                    Console.Write("Pick number to offer and enter to finish: ");
                    string? input = Console.ReadLine(); // läs val eller tom rad

                    if (input == "") // tom = klart
                    {
                        break;
                    }
                    int number; // valt index
                    bool numb = int.TryParse(input, out number); // tolka tal
                    if (numb && number > 0 && number <= active_user.Items.Count) // giltigt index
                    {
                        Item item = active_user.Items[number - 1]; // hämta mitt item

                        bool isChosen = false; // undvik dubbelt i listan
                        foreach (Item it in offeredItems)
                        {
                            if (it == item) // redan valt
                            {
                                isChosen = true;
                                break;
                            }
                        }
                        if (!isChosen) // lägg om inte redan tillagd
                        {
                            offeredItems.Add(item); // lägg till i offered
                            Console.WriteLine("Offered: " + item.Name);
                        }
                        else
                        {
                            Console.WriteLine("That item is already selected");
                        }
                    }
                    else // ogiltigt val
                    {
                        Console.WriteLine("Please enter a valid number.");
                    }
                }

                List<Item> tradeItems = new List<Item>(); // listan som ska in i trade
                foreach (Item item in requestedItems) // lägg in alla requested
                {
                    tradeItems.Add(item);
                } // lägg requested
                foreach (Item item in offeredItems) // lägg in alla offered
                {
                    tradeItems.Add(item);
                } // lägg offered

                Trade trade = new Trade(active_user, owner, tradeItems, TradeStatus.Pending); // skapa pending trade
                trades.Add(trade); // lägg in trade i lista

                string newTradeRow = ""; // bygg upp trade raderna som ska appendas i trades.txt
                newTradeRow += trade.Sender.Name + ", " + trade.Receiver.Name + "\n"; // första raden

                foreach (Item item in trade.Items) // skriver alla items i traden rad för rad
                {
                    newTradeRow += item.Name + ", " + item.Owner.Name + "\n";
                }
                newTradeRow += "-," + trade.Status.ToString() + "\n"; // sista raden
                File.AppendAllText("trades.txt", newTradeRow); // spara raden sist i trades.txt

                Console.WriteLine("Trade request sent...");
                Console.WriteLine("Status: [pending..]");
                Console.WriteLine("Press enter to go back to menu");
                Console.ReadLine();
                break;

            case "5":
                Console.Clear();
                Console.WriteLine("--Incoming trade requests--");

                bool incoming = false; // flagga om vi hittar inkommande
                foreach (Trade tr in trades) // loopa alla trades
                {
                    if (
                        tr != null
                        && tr.Receiver == active_user
                        && tr.Status == TradeStatus.Pending
                    ) // min inbox, pending
                    {
                        incoming = true; // markera att något finns

                        Console.WriteLine("From: " + tr.Sender.Name); // visa vem som skickat
                        Console.WriteLine("Wants:");
                        bool userWants = false; // flagga om de vill ha något

                        foreach (Item item in tr.Items) // loopa items i traden
                        {
                            bool ownedByReceiver = false; // kolla om item ägs av receiver
                            foreach (Item it in tr.Receiver.Items) // loopa mina items
                            {
                                if (item == it) // samma objekt?
                                {
                                    ownedByReceiver = true; // ja det är något de vill ha
                                    break; // sluta leta
                                }
                            }
                            if (ownedByReceiver) // om de vill ha något från mig
                            {
                                Console.WriteLine("- " + item.Name); // skriv ut det
                                userWants = true; // markera att det fanns want
                            }
                        }
                        if (!userWants) // om inget vill ha något hittades
                        {
                            Console.WriteLine("- [nothing]");
                        }

                        Console.WriteLine("They offer:");
                        bool anyOffers = false; // flagga om sändaren erbjuder något
                        foreach (Item it in tr.Items) // loopa alla items i traden iegen
                        {
                            bool ownedBySender = false; // kolla om item ägs av sändaren
                            foreach (Item item in tr.Sender.Items) // loopa avsändarens items
                            {
                                if (item == it) // samma ?
                                {
                                    ownedBySender = true; // svar ja, detta är ett erbjudet item
                                    break; //sluta leta
                                }
                            }
                            if (ownedBySender)
                            {
                                Console.WriteLine("- " + it.Name); //skriv erbjuder item
                                anyOffers = true; // markera att det erbjudits
                            }
                        }
                        if (!anyOffers) // om inga erbjudanden
                        {
                            Console.WriteLine("- [nothing]");
                        }

                        Console.WriteLine("Status: " + tr.Status);
                        Console.WriteLine();
                    }
                }

                if (!incoming) // om inga inkommande finns
                {
                    Console.WriteLine("[no incoming requests]");
                }

                Console.WriteLine("--Your outgoing requests--");

                bool outgoing = false; // visa om jag har skickat några trades
                foreach (Trade tr in trades) // loopa alla trades
                {
                    if (tr != null && tr.Sender == active_user) // trades jag har skickat
                    {
                        outgoing = true; // markera att något finns

                        Console.WriteLine("To: " + tr.Receiver.Name);
                        Console.WriteLine("Wants:");
                        bool anyWants = false; // vill ha något
                        foreach (Item it in tr.Items) //loopa items
                        {
                            bool ownedByReceiver = false; // true om item ägs av mottagare
                            foreach (Item item in tr.Receiver.Items) // loopa mottagarens items
                            {
                                if (item == it) // samma ?
                                {
                                    ownedByReceiver = true; // svar ja, detta är något jag vill ha
                                    break;
                                }
                            }
                            if (ownedByReceiver)
                            {
                                Console.WriteLine("- " + it.Name); //skriv item
                                anyWants = true; // markera att wants fanns
                            }
                        }
                        if (!anyWants) // inget wants
                        {
                            Console.WriteLine("- [nothing]");
                        }

                        Console.WriteLine("You offered:");
                        bool anyOffers = false; // om jag erbjöd något markera
                        foreach (Item it in tr.Items) // loopa items
                        {
                            bool ownedBySender = false; // true om item ägs av mig
                            foreach (Item item in tr.Sender.Items) // loopa mina items
                            {
                                if (item == it) //samma?
                                {
                                    ownedBySender = true; //svar ja detta erbjöd jag
                                    break;
                                }
                            }
                            if (ownedBySender)
                            {
                                Console.WriteLine("- " + it.Name); // skriv erbjudande item
                                anyOffers = true; // markera att npgot erbjöds
                            }
                        }
                        if (!anyOffers) // om jag inte erbjöd något
                        {
                            Console.WriteLine("- [nothing]");
                        }

                        Console.WriteLine("Status: " + tr.Status);
                        Console.WriteLine();
                    }
                }

                if (!outgoing) // inga utgående
                {
                    Console.WriteLine("[no outgoing requests]");
                }

                Console.WriteLine("Press enter to go back");
                Console.ReadLine();
                break;

            case "6":
                Console.Clear();
                Console.WriteLine("--Handle trade requests--");

                bool foundAny = false; // om det fanns något att hantera

                foreach (Trade tr in trades) // loopa alla trades
                {
                    if (
                        tr != null
                        && tr.Receiver == active_user
                        && tr.Status == TradeStatus.Pending
                    ) // mina inkommande pending
                    {
                        foundAny = true; // markera att vi har något att hantera

                        Console.WriteLine("From: " + tr.Sender.Name);

                        Console.WriteLine("Wants:");
                        bool anyWants = false; // flagga för wants
                        foreach (Item it in tr.Items) // loopa items
                        {
                            bool ownedByReceiver = false; // true om item ägs av mig
                            foreach (Item item in tr.Receiver.Items) // loopa mina items
                            {
                                if (item == it) // samma?
                                {
                                    ownedByReceiver = true; // detta vill de ha
                                    break;
                                }
                            }
                            if (ownedByReceiver)
                            {
                                Console.WriteLine("- " + it.Name); // skriv wants
                                anyWants = true; // markera att wants fanns
                            }
                        }
                        if (!anyWants) // om inga wants
                            Console.WriteLine("- [nothing]");

                        Console.WriteLine("They offer:");
                        bool anyOffers = false; // flagga för offers
                        foreach (Item it in tr.Items) // loopa items
                        {
                            bool ownedBySender = false; // true om item ägs av avsändareb
                            foreach (Item item in tr.Sender.Items) // loopa avsändarens items
                            {
                                if (item == it) // samma objekt?
                                {
                                    ownedBySender = true; // detta erbjuder dem
                                    break;
                                }
                            }
                            if (ownedBySender)
                            {
                                Console.WriteLine("- " + it.Name); // skriv offer
                                anyOffers = true; // markera att offers finns
                            }
                        }
                        if (!anyOffers) // om inga offers
                            Console.WriteLine("- [nothing]");

                        Console.WriteLine("Accept (A) or Deny (D)?");
                        string? decision = Console.ReadLine(); // läs beslut
                        if (decision == null) // skydda mot null
                        {
                            decision = ""; //fallback till tom sträng
                        }
                        else
                        {
                            decision = decision.ToLower(); // ok caps
                        }

                        if (decision == "a") // acceptera
                        {
                            tr.Accept(); // låt Trade flytta items + sätta Accepted
                            Console.WriteLine("Trade accepted!");
                        }
                        else if (decision == "d") // neka
                        {
                            tr.Status = TradeStatus.Denied; // markera denied
                            Console.WriteLine("Trade denied.");
                        }
                        else // varken a eller d
                        {
                            Console.WriteLine("No action taken.");
                        }

                        Console.WriteLine();
                    }
                }

                if (!foundAny) // inget att hantera
                {
                    Console.WriteLine("You have no pending trade requests.");
                }

                // skriv ALLA trades tillbaka till fil med aktuella ägare på varje item
                List<string> outLines = new List<string>(); // temporary list för alla rader som ska skrivas

                foreach (Trade tr in trades) // loopa alla trades
                {
                    outLines.Add(tr.Sender.Name + ", " + tr.Receiver.Name); // första rad med senderName och ReceiverName

                    foreach (Item item in tr.Items) // loopa alla items
                    {
                        outLines.Add(item.Name + ", " + item.Owner.Name); // andra rad för ItemName och OwnerName
                    }
                    outLines.Add("-," + tr.Status.ToString()); // status rad
                }
                File.WriteAllLines("trades.txt", outLines); // skriv hela filen trades.txt på nytt med alla trades
                // Skriv om items.txt, nya ägare efter Accept
                List<string> itemsLines = new List<string>(); // temporary list för items.txt
                foreach (User user in users) // loopa alla användare
                {
                    foreach (Item item in user.Items) // loopa alla deras items
                    {
                        itemsLines.Add(user.Email + "," + item.Name + "," + item.Description); //skriv en rad per item
                    }
                }
                File.WriteAllLines("items.txt", itemsLines); // skriv om hela items.txt

                Console.WriteLine("Press enter to go back");
                Console.ReadLine();
                break;

            // Visa completed (Accepted/Denied)
            case "7":
                Console.Clear();
                Console.WriteLine("---Completed trades---");

                bool anyFound = false; // flagga om det finns något klart

                foreach (Trade tr in trades) // loopa alla trades
                {
                    if (tr.Status == TradeStatus.Accepted || tr.Status == TradeStatus.Denied) // completed
                    {
                        anyFound = true; // markera att vi skev något

                        Console.WriteLine("From: " + tr.Sender.Name + " to " + tr.Receiver.Name);

                        Console.WriteLine("Wants:");
                        bool anyWants = false; // flagga för wants
                        foreach (Item it in tr.Items) // loopa items
                        {
                            bool ownedByReceiver = false; // true om item ägs av receiver
                            foreach (Item item in tr.Receiver.Items) // kolla i receiver items
                            {
                                if (item == it) // samma objekt?
                                {
                                    ownedByReceiver = true; // ja detta va ett want
                                    break;
                                }
                            }
                            if (ownedByReceiver)
                            {
                                Console.WriteLine("- " + it.Name); // skriv wants item
                                anyWants = true; // markera att det fanns wants
                            }
                        }
                        if (anyWants == false) // inget wants hittat
                        {
                            Console.WriteLine("- [nothing]");
                        }

                        Console.WriteLine("Offered:");
                        bool anyOffers = false; // flagga för offers
                        foreach (Item it in tr.Items) // loopa items
                        {
                            bool ownedBySender = false; // true om items ägs av sender
                            foreach (Item item in tr.Sender.Items) // kolla i sender items
                            {
                                if (item == it) // samma objekt?
                                {
                                    ownedBySender = true; // detta var ett offer
                                    break;
                                }
                            }
                            if (ownedBySender)
                            {
                                Console.WriteLine("- " + it.Name); // skriv offers items
                                anyOffers = true; // markera att det fanns offers
                            }
                        }
                        if (anyOffers == false) // inget offered hittat
                        {
                            Console.WriteLine("- [nothing]");
                        }

                        Console.WriteLine("Status: " + tr.Status); // skriv slutstatus
                        Console.WriteLine();
                    }
                }

                if (anyFound == false) // inga completed att visa
                {
                    Console.WriteLine("[no completed trades]");
                }

                Console.WriteLine("Press enter to go back");
                Console.ReadLine();
                break;

            // Logga ut
            case "8":
                Console.Clear();
                Console.WriteLine("Logging out...");
                active_user = null; // rensa inloggad användare
                Console.WriteLine("You are now logged out, press enter");
                Console.ReadLine();
                break;
        }
    }
}
