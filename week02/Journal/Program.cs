// I have saved the user response from *.txt to *.json.
// I have commented out those codes for saving and loading to *.txt
// Currently, the user response should be saved or loaded from *.json
// Nevertheless, I do not know why the data file is
// saved under ./bin/debug/net10.0  Please advise.
using System;

class Program
{
    static void Main(string[] args)
    {
        Journal journal = new Journal();
        Menu menu = new Menu(journal);
        menu.Run();
    }
}