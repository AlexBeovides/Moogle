using System.Text;

namespace MoogleEngine;

public class Snippet
{
    public Snippet(string snippetText,int startIndex,int endIndex,Dictionary<int,int> queryTermsIndex)
    {
        this.snippetText=snippetText;
        this.startIndex=startIndex;
        this.endIndex=endIndex;
        this.queryTermsIndex=queryTermsIndex;
    }

    public string snippetText {get ; private set; }
    public int startIndex {get ; private set; }
    public int endIndex {get ; private set; }

    public Dictionary<int,int> queryTermsIndex {get ; private set; }

    public static Snippet RetrieveSnippet(Document Doc,Query ActualQuery)      /// !!!!!!!!!!
    {
        Dictionary<int,int> queryTermsIndex=new Dictionary<int,int>();

        
        int snippetRange=Math.Min(60,Doc.content.Count);
        double bestSnippetValue=0;

        int snippetStart=Doc.content[0].Item2;
        int snippetEnd=Doc.content[snippetRange-1].Item3;

        int lastWordIndex=-1;
        int nextWordIndex=-1;

        for(int i=0 ; i<snippetRange ; i++)
        {
            nextWordIndex=DataSet.IndexForEachWord[Doc.content[i].Item1];
                
            if(Moogle.termsInQuery.ContainsKey(Doc.content[i].Item1) && queryTermsIndex.ContainsKey(Doc.content[i].Item2)==false)
            {
                bestSnippetValue+=(ActualQuery.QueryVector.values[nextWordIndex]);
                queryTermsIndex.Add(Doc.content[i].Item2,Doc.content[i].Item3);
            }
        }

        double currentSnippetValue=bestSnippetValue;

        for(int i=snippetRange ; i<Doc.content.Count ; i++)
        {
            if(Moogle.termsInQuery.ContainsKey(Doc.content[i].Item1))
            {
                queryTermsIndex.Add(Doc.content[i].Item2,Doc.content[i].Item3);
            }

            nextWordIndex=DataSet.IndexForEachWord[Doc.content[i].Item1];
            currentSnippetValue+=(ActualQuery.QueryVector.values[nextWordIndex]);

            lastWordIndex=DataSet.IndexForEachWord[Doc.content[i-snippetRange].Item1];
            currentSnippetValue-=(ActualQuery.QueryVector.values[lastWordIndex]);

            if(currentSnippetValue>bestSnippetValue)
            {
                bestSnippetValue=currentSnippetValue;
                snippetStart=Doc.content[i-snippetRange+1].Item2;
                snippetEnd=Doc.content[i].Item3;
            }
        }


        string snippetText=retrieveTxtSegment(Doc.original,snippetStart,snippetEnd);
        
        Snippet snippet=new Snippet(snippetText,snippetStart,snippetEnd,queryTermsIndex);

        return snippet;
    }

    public static string retrieveTxtSegment(string Text,int startIndex,int endIndex)
    {
        StringBuilder retrievedText=new StringBuilder();

        for(int i=startIndex ; i<=endIndex ; i++)
        {
            retrievedText.Append(Text[i]);
        }

        return retrievedText.ToString();
    }
}

