using System.Text;

public class Word
{
    private string _text;
    private bool _isHidden;

    public Word(string text)
    {
        _text = text;
        _isHidden = false;
    }

    public void Hide()
    {
        _isHidden = true;
    }

    public bool IsHidden()
    {
        return _isHidden;
    }

    public string GetDisplayText()
    {
        if (!_isHidden)
        {
            return _text;
        }

        // Hide letters and digits; keep punctuation, spaces, dashes, quotes, etc.
        StringBuilder result = new StringBuilder();
        foreach (char c in _text)
        {
            if (char.IsLetterOrDigit(c))
            {
                result.Append('_');
            }
            else
            {
                result.Append(c);
            }
        }
        return result.ToString();
    }
}