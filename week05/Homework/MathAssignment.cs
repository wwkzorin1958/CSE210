using System.ComponentModel;

public class MathAssignment : Assignment
{
    private string _textbookSection;
    private string _problems;

    public MathAssignment() : base()
    {
        _textbookSection = "";
        _problems = "";
    }

    public MathAssignment(string studentName, string topic, string textbookSection, string problem) : base(studentName, topic)
    {
        _textbookSection = textbookSection;
        _problems = problem;
    }

    public string GetTextbookSection()
    {
        return _textbookSection;
    }
    public void SetTextbookSection(string textbookSection)
    {
        _textbookSection = textbookSection;
    }

    public string GetProblem()
    {
        return _problems;
    }
    public void SetProblem(string problems)
    {
        _problems = problems;
    }

    public string GetHomeworkList()
    {
        return $"{_textbookSection} - {_problems}";
    }


}