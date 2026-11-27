using System.Diagnostics;
using System.Text;

namespace MindfulnessDesktop;

internal sealed class MainForm : Form
{
    private enum Screen { Home, Preparing, Breathing, Listing, ReflectingIntro, Reflecting, History, Saving, Complete }
    private enum ActivityKind { Breathing, Listing, Reflecting }

    private readonly FlowLayoutPanel _content;
    private readonly System.Windows.Forms.Timer _timer;
    private readonly Stopwatch _clock = new Stopwatch();
    private readonly List<ActivityResponse> _responses = new List<ActivityResponse>();
    private Screen _screen;
    private ActivityKind _kind;
    private Activity? _activity;
    private ListingActivity? _listing;
    private ReflectingActivity? _reflecting;
    private ActivityRecord? _pendingRecord;
    private DateTimeOffset _startedAt;
    private int _duration;
    private int _lastBreathingPhase;
    private int _itemCount;
    private bool _highTone;
    private string _currentPrompt = "";
    private Label? _timeLabel;
    private Label? _breathingLabel;

    public MainForm()
    {
        Text = "Mindfulness";
        StartPosition = FormStartPosition.CenterScreen;
        Size = new Size(900, 720);
        MinimumSize = new Size(800, 620);
        BackColor = Color.FromArgb(245, 248, 245);
        Font = new Font("Segoe UI", 11);

        Panel header = new Panel
        {
            Dock = DockStyle.Top,
            Height = 90,
            BackColor = Color.FromArgb(33, 79, 68),
            Padding = new Padding(30, 17, 30, 10)
        };
        Label title = new Label
        {
            Text = "Mindfulness",
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 22, FontStyle.Bold),
            AutoSize = true
        };
        header.Controls.Add(title);

        _content = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(30, 25, 30, 25),
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            AutoScroll = true
        };
        Controls.Add(_content);
        Controls.Add(header);

        _timer = new System.Windows.Forms.Timer { Interval = 100 };
        _timer.Tick += TimerTick;
        ShowHome();
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        if (HasUnfinishedActivity() &&
            MessageBox.Show(this, "The current activity has not been saved. Exit anyway?",
                "Exit Mindfulness", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
        {
            e.Cancel = true;
        }

        base.OnFormClosing(e);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _timer.Dispose();
        }

        base.Dispose(disposing);
    }

    private bool HasUnfinishedActivity()
    {
        return _screen == Screen.Preparing || _screen == Screen.Breathing ||
            _screen == Screen.Listing || _screen == Screen.ReflectingIntro ||
            _screen == Screen.Reflecting || _screen == Screen.Saving;
    }

    private void ClearPage()
    {
        AcceptButton = null;
        _timeLabel = null;
        _breathingLabel = null;
        foreach (Control control in _content.Controls.Cast<Control>().ToArray())
        {
            control.Dispose();
        }
    }

    private Label AddText(string text, bool heading = false)
    {
        Label label = new Label
        {
            Text = text,
            AutoSize = true,
            MaximumSize = new Size(730, 0),
            ForeColor = Color.FromArgb(35, 55, 48),
            Font = new Font("Segoe UI", heading ? 17 : 11, heading ? FontStyle.Bold : FontStyle.Regular),
            Margin = new Padding(0, 0, 0, heading ? 16 : 12)
        };
        _content.Controls.Add(label);
        return label;
    }

    private Button AddButton(string text, EventHandler action)
    {
        Button button = new Button
        {
            Text = text,
            AutoSize = true,
            MinimumSize = new Size(190, 42),
            Padding = new Padding(12, 4, 12, 4),
            Margin = new Padding(0, 0, 0, 10),
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(54, 108, 91),
            ForeColor = Color.White
        };
        button.FlatAppearance.BorderSize = 0;
        button.Click += action;
        _content.Controls.Add(button);
        return button;
    }

    private TextBox AddInput()
    {
        TextBox input = new TextBox
        {
            Width = 700,
            Margin = new Padding(0, 0, 0, 14)
        };
        _content.Controls.Add(input);
        return input;
    }

    private void AddCancelButton()
    {
        AddButton("Cancel activity", (_, _) =>
        {
            if (MessageBox.Show(this, "Cancel this activity without saving it?", "Cancel activity",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                _timer.Stop();
                _clock.Stop();
                ShowHome();
            }
        });
    }

    private void ShowHome()
    {
        _screen = Screen.Home;
        _pendingRecord = null;
        ClearPage();
        AddText("Take a moment for yourself", true);
        AddText("Choose a session length, then select an activity.");

        NumericUpDown duration = new NumericUpDown
        {
            Minimum = 1,
            Maximum = int.MaxValue,
            Value = 30,
            Width = 150,
            Margin = new Padding(0, 0, 0, 18)
        };
        AddText("Session length (seconds)");
        _content.Controls.Add(duration);
        AddButton("Breathing", (_, _) => Prepare(ActivityKind.Breathing, (int)duration.Value));
        AddButton("Reflecting", (_, _) => Prepare(ActivityKind.Reflecting, (int)duration.Value));
        AddButton("Listing", (_, _) => Prepare(ActivityKind.Listing, (int)duration.Value));
        AddButton("Review activity history", (_, _) => ShowHistory());
        AddButton("Quit", (_, _) => Close());
    }

    private void Prepare(ActivityKind kind, int duration)
    {
        _kind = kind;
        _duration = duration;
        _responses.Clear();
        _itemCount = 0;
        _screen = Screen.Preparing;
        ClearPage();
        AddText("Get ready", true);
        _timeLabel = AddText("Starting in 5 seconds...");
        AddCancelButton();
        _clock.Restart();
        _timer.Start();
    }

    private void TimerTick(object? sender, EventArgs e)
    {
        if (_screen == Screen.Preparing)
        {
            int remaining = Math.Max(0, 5 - (int)_clock.Elapsed.TotalSeconds);
            if (_timeLabel != null) _timeLabel.Text = $"Starting in {remaining} seconds...";
            if (_clock.Elapsed >= TimeSpan.FromSeconds(5))
            {
                _timer.Stop();
                BeginActivity();
            }
        }
        else if (_screen == Screen.Breathing)
        {
            int phase = (int)(_clock.Elapsed.TotalSeconds / 5);
            if (phase != _lastBreathingPhase)
            {
                if (phase % 2 == 0 && _clock.Elapsed.TotalSeconds >= _duration)
                {
                    FinishActivity();
                    return;
                }

                _lastBreathingPhase = phase;
                PlayBowlSound(phase % 2 != 0);
            }

            if (_breathingLabel != null)
            {
                int count = 5 - (int)(_clock.Elapsed.TotalSeconds % 5);
                _breathingLabel.Text = $"{(phase % 2 == 0 ? "Breathe in" : "Breathe out")}: {count}";
            }

            UpdateTimeLabel();
        }
        else if (_screen == Screen.Listing || _screen == Screen.Reflecting)
        {
            UpdateTimeLabel();
        }
    }

    private void BeginActivity()
    {
        switch (_kind)
        {
            case ActivityKind.Breathing:
                _activity = new BreathingActivity("Breathing", "Relax by breathing slowly.", _duration);
                StartClock();
                _screen = Screen.Breathing;
                ClearPage();
                AddText("Breathing activity", true);
                AddText("Follow the rhythm and breathe slowly.");
                _breathingLabel = AddText("Breathe in: 5", true);
                _timeLabel = AddText("");
                AddCancelButton();
                _lastBreathingPhase = 0;
                PlayBowlSound(false);
                _timer.Start();
                break;
            case ActivityKind.Listing:
                _listing = new ListingActivity("Listing", "List things in a certain area.", _duration, 0);
                _activity = _listing;
                _currentPrompt = _listing.GetRandomPrompt();
                _responses.Add(new ActivityResponse(_currentPrompt, new List<string>()));
                PlayBowlSound(false);
                StartClock();
                _screen = Screen.Listing;
                ShowListing();
                _timer.Start();
                break;
            case ActivityKind.Reflecting:
                _reflecting = new ReflectingActivity("Reflecting", "Reflect on meaningful experiences.", _duration);
                _activity = _reflecting;
                _currentPrompt = _reflecting.GetRandomPrompt();
                _screen = Screen.ReflectingIntro;
                PlayBowlSound(false);
                ShowReflectingIntro();
                break;
        }
    }

    private void StartClock()
    {
        _startedAt = DateTimeOffset.Now;
        _clock.Restart();
    }

    private void UpdateTimeLabel()
    {
        if (_timeLabel == null) return;
        int remaining = (int)Math.Max(0, Math.Ceiling(_duration - _clock.Elapsed.TotalSeconds));
        _timeLabel.Text = remaining > 0
            ? $"Time remaining: {remaining} seconds"
            : _screen == Screen.Breathing
                ? "Finishing the current breathing cycle..."
                : "Time is up. Submit your current response to finish.";
    }

    private void ShowListing()
    {
        ClearPage();
        AddText("Listing activity", true);
        _timeLabel = AddText("");
        UpdateTimeLabel();
        AddText(_currentPrompt);
        AddText($"Items listed: {_itemCount}. Enter an item, or submit a blank entry for another prompt.");
        TextBox input = AddInput();
        ActivityResponse current = _responses[_responses.Count - 1];
        if (current.Answers.Count > 0)
        {
            AddText("Items for this prompt: " + string.Join(", ", current.Answers));
        }

        Button submit = AddButton("Add item / continue", (_, _) => SubmitListing(input.Text));
        AcceptButton = submit;
        AddButton("Next prompt", (_, _) =>
        {
            if (_clock.Elapsed.TotalSeconds >= _duration)
            {
                SubmitListing(input.Text);
            }
            else if (!string.IsNullOrWhiteSpace(input.Text))
            {
                MessageBox.Show(this, "Add the current item before changing prompts.", "Listing");
            }
            else
            {
                NextListingPrompt();
            }
        });
        AddCancelButton();
        input.Focus();
    }

    private void SubmitListing(string item)
    {
        if (!string.IsNullOrWhiteSpace(item))
        {
            _responses[_responses.Count - 1].Answers.Add(item);
            _itemCount++;
        }

        if (_clock.Elapsed.TotalSeconds >= _duration)
        {
            PlayBowlSound(true);
            FinishActivity();
        }
        else if (string.IsNullOrWhiteSpace(item))
        {
            NextListingPrompt();
        }
        else
        {
            ShowListing();
        }
    }

    private void NextListingPrompt()
    {
        ListingActivity listing = _listing ?? throw new InvalidOperationException("Listing activity is not active.");
        List<string> alternatives = listing.GetPrompt().Where(prompt => prompt != _currentPrompt).ToList();
        if (alternatives.Count == 0)
        {
            MessageBox.Show(this, "There are no other prompts available.", "Listing");
            return;
        }

        _currentPrompt = alternatives[Random.Shared.Next(alternatives.Count)];
        _responses.Add(new ActivityResponse(_currentPrompt, new List<string>()));
        ShowListing();
    }

    private void ShowReflectingIntro()
    {
        ClearPage();
        AddText("Reflecting activity", true);
        AddText(_currentPrompt);
        AddText("Write your thoughts, or leave the box blank. The timed questions start when you continue.");
        TextBox input = AddInput();
        Button next = AddButton("Continue", (_, _) =>
        {
            _responses.Add(new ActivityResponse(_currentPrompt, new List<string> { input.Text }));
            _highTone = true;
            StartClock();
            _screen = Screen.Reflecting;
            NextReflectingQuestion();
            _timer.Start();
        });
        AcceptButton = next;
        AddCancelButton();
        input.Focus();
    }

    private void NextReflectingQuestion()
    {
        ReflectingActivity reflecting = _reflecting ?? throw new InvalidOperationException("Reflecting activity is not active.");
        _currentPrompt = reflecting.GetRandomQuestion();
        PlayBowlSound(_highTone);
        _highTone = !_highTone;
        ClearPage();
        AddText("Reflecting activity", true);
        _timeLabel = AddText("");
        UpdateTimeLabel();
        AddText(_currentPrompt);
        AddText("Write a response, or leave it blank to skip.");
        TextBox input = AddInput();
        Button next = AddButton("Next question / finish", (_, _) =>
        {
            _responses.Add(new ActivityResponse(_currentPrompt, new List<string> { input.Text }));
            if (_clock.Elapsed.TotalSeconds >= _duration)
            {
                FinishActivity();
            }
            else
            {
                NextReflectingQuestion();
            }
        });
        AcceptButton = next;
        AddCancelButton();
        input.Focus();
    }

    private void PlayBowlSound(bool highTone)
    {
        Activity activity = _activity ?? throw new InvalidOperationException("No activity is active.");
        try
        {
            activity.PlayBowlSound(highTone);
        }
        catch (IOException exception)
        {
            MessageBox.Show(this, $"Unable to play the bowl sound: {exception.Message}", "Sound error",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        catch (UnauthorizedAccessException exception)
        {
            MessageBox.Show(this, $"Unable to play the bowl sound: {exception.Message}", "Sound error",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        catch (TimeoutException exception)
        {
            MessageBox.Show(this, $"Unable to play the bowl sound: {exception.Message}", "Sound error",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        catch (InvalidDataException exception)
        {
            MessageBox.Show(this, $"Unable to play the bowl sound: {exception.Message}", "Sound error",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        catch (InvalidOperationException exception)
        {
            MessageBox.Show(this, $"Unable to play the bowl sound: {exception.Message}", "Sound error",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    private void FinishActivity()
    {
        _timer.Stop();
        _clock.Stop();
        Activity activity = _activity ?? throw new InvalidOperationException("No activity is active.");
        _pendingRecord = new ActivityRecord(
            activity.GetName(), _startedAt, _duration, _clock.Elapsed.TotalSeconds, new List<ActivityResponse>(_responses));
        SaveActivity();
    }

    private void SaveActivity()
    {
        ActivityRecord record = _pendingRecord ?? throw new InvalidOperationException("There is no activity to save.");
        _screen = Screen.Saving;
        try
        {
            new ActivityHistory().Save(record);
        }
        catch (IOException exception)
        {
            ShowSaveError(exception.Message);
            return;
        }
        catch (UnauthorizedAccessException exception)
        {
            ShowSaveError(exception.Message);
            return;
        }

        _pendingRecord = null;
        _screen = Screen.Complete;
        ClearPage();
        AddText("Well done!", true);
        AddText($"You completed {record.ActivityName.ToLowerInvariant()} in {record.ElapsedSeconds:F1} seconds.");
        if (_kind == ActivityKind.Listing) AddText($"You listed {_itemCount} items.");
        AddText("Your activity has been saved to history.");
        AddButton("Return to menu", (_, _) => ShowHome());
    }

    private void ShowSaveError(string message)
    {
        ClearPage();
        AddText("Could not save your activity", true);
        AddText(message);
        AddText("Your activity is still available in this window. Fix the problem and try again.");
        AddButton("Retry saving", (_, _) => SaveActivity());
    }

    private void ShowHistory()
    {
        ActivityHistory history = new ActivityHistory();
        List<ActivityRecord> records;
        try
        {
            records = history.Load();
        }
        catch (IOException exception)
        {
            MessageBox.Show(this, $"Unable to read history: {exception.Message}", "History error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }
        catch (UnauthorizedAccessException exception)
        {
            MessageBox.Show(this, $"Unable to read history: {exception.Message}", "History error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }
        catch (InvalidDataException exception)
        {
            MessageBox.Show(this, $"Unable to read history: {exception.Message}", "History error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        _screen = Screen.History;
        ClearPage();
        AddText("Activity history", true);
        AddText($"Saved locally at: {history.PathName}");
        StringBuilder text = new StringBuilder();
        if (records.Count == 0) text.AppendLine("No completed activities have been saved yet.");
        foreach (ActivityRecord record in records)
        {
            text.AppendLine($"{record.StartedAt.ToLocalTime():yyyy-MM-dd HH:mm:ss zzz} - {record.ActivityName}");
            text.AppendLine($"Requested: {record.RequestedSeconds} seconds; elapsed: {record.ElapsedSeconds:F1} seconds");
            foreach (ActivityResponse response in record.Responses)
            {
                text.AppendLine($"  {response.Prompt}");
                if (response.Answers.Count == 0) text.AppendLine("    (No response recorded)");
                foreach (string answer in response.Answers)
                {
                    text.AppendLine($"    - {(string.IsNullOrWhiteSpace(answer) ? "(No response recorded)" : answer)}");
                }
            }
            text.AppendLine();
        }

        TextBox historyBox = new TextBox
        {
            Multiline = true,
            ReadOnly = true,
            ScrollBars = ScrollBars.Vertical,
            Width = 700,
            Height = 300,
            Text = text.ToString(),
            Margin = new Padding(0, 0, 0, 14)
        };
        _content.Controls.Add(historyBox);
        AddButton("Return to menu", (_, _) => ShowHome());
    }
}
