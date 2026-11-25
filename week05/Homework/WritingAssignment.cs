class WritingAssignments : Assignment
{
    private string _title;

    public WritingAssignments() : base()
    {
        _title = "";
    }
    public WritingAssignments(string studentName, string topic, string title) : base(studentName, topic)
    {
        _title = title;
    }
    public string GetTitle()
    {
        return _title;
    }
    public void SetTitle(string title)
    {
        _title = title;
    }


    public string GetWritingInformation()
    {
        return $"{_title} by {GetStudentName()}";
    }

}