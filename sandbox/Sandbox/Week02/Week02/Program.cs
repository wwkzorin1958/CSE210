namespace Week02;

class Program
{
    static void Main(string[] args)
    {
        // Program.cs only handles running the application logic
        SimpleGoal myGoal = new SimpleGoal("Read 15 minutes", 100);
        myGoal.RecordEvent();
    }
}
