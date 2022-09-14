namespace MoogleEngine;

public class SearchItem
{
    public SearchItem(string title, Snippet snippet, double score)
    {
        this.Title = title;
        this.Snippet = snippet;
        this.Score = score;
    }

    public string Title { get; private set; }

    public Snippet Snippet { get; private set; }

    public double Score { get; private set; }

}
