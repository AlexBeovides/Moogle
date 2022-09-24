namespace MoogleEngine
{
    //la clase Query, contiene todos los metodos, objetos y estructuras relacionadas
    //al trabajo con las querys
    public class Query
    {
        public Query(string original,List<Tuple<string,int,int>> content,Vector QueryVector)
        {
            this.original=original;
            this.content=content;
            this.QueryVector=QueryVector;
        }
        public string original {get ; private set; }
        public List<Tuple<string,int,int>> content {get ; private set; }
        public Vector QueryVector {get ; private set; }
        public static int ResultsAmount;
        public static void QueryTfIdf(Vector QueryVect)
        {
            double freq=Document.maxFreqFill(QueryVect);

            for(int i=0 ; i<DataSet.Dict.Count ; i++)
            {
                QueryVect.values[i]=(QueryVect.values[i]/freq)*Vector.Idf[i];
            }
        }

        public static List<SearchItem> RankDocuments(Query ActualQuery)
        {
            List<SearchItem> items =new List<SearchItem> ();
            List<Tuple<double,int>> toRank=new List<Tuple<double,int>> ();
            

            for(int i=0 ; i<Document.Documents.Count ; i++)
            {
                toRank.Add(Tuple.Create(CosineSimilarity(DataSet.VectorialModel[i],ActualQuery.QueryVector),i));
            }

            toRank.Sort();

            for(int i=0 ; i<toRank.Count ; i++)
            {
                if(toRank[i].Item1==0)
                {
                    toRank.RemoveAt(i);
                }
                else
                {
                    break;
                }
            }

            Operators.Apply(toRank);

            toRank.Sort();
            toRank.Reverse();

            for(int i=0 ; i<toRank.Count ; i++)
            {
                if(toRank[i].Item1==0)   
                {
                    break;
                }
        
                Document DocToProcess=Document.Documents[toRank[i].Item2];

                Snippet snippet=Snippet.RetrieveSnippet(DocToProcess,ActualQuery);

                items.Add(new SearchItem(DocToProcess.title,snippet,toRank[i].Item1));
            }

            ResultsAmount=items.Count;

            return items;
        }

        public static double CosineSimilarity(Vector DocVector,Vector QueryVector)
        {
            double Numerator=0;
            double DocValuesSquaresSumatory=0;
            double QueryValuesSquaresSumatory=0;

            for(int i=0 ; i<DataSet.Dict.Count ; i++)
            {
                Numerator+=(DocVector.values[i]*QueryVector.values[i]);
                DocValuesSquaresSumatory+=(DocVector.values[i]*DocVector.values[i]);
                QueryValuesSquaresSumatory+=(QueryVector.values[i]*QueryVector.values[i]);
            }

            return (Numerator/((Math.Sqrt(DocValuesSquaresSumatory))*(Math.Sqrt(QueryValuesSquaresSumatory))));
        }

        public static bool PurifyQuery(List<Tuple<string,int,int>> content)
        {
            bool showSuggestion=false;

            for(int i=0 ; i<content.Count ; i++)
            {
                string correctedWord="";

                if(DataSet.Dict.ContainsKey(content[i].Item1)==false && DataSet.Synonymous.ContainsKey(content[i].Item1)==false)
                {
                    int better=int.MaxValue;
                    showSuggestion=true;

                    foreach(string word in DataSet.Dict.Keys)
                    {
                        int simCoefficient=TextUtils.EditDistance(content[i].Item1,word);

                        if(simCoefficient<better)
                        {
                            correctedWord=word;
                            better=simCoefficient;
                        }
                    }

                    content[i]=Tuple.Create(correctedWord,content[i].Item2,content[i].Item3);
                }
            }

            return showSuggestion;
        }

        public static void SearchSynonymous(Document QueryDoc, Vector QueryVect)
        {
            //itera por todos los terminos de la query
            for(int i=0 ; i<QueryDoc.content.Count ; i++)
            {
                string word=QueryDoc.content[i].Item1;

                //si la palabra no esta en el corpus y posee sinonimos 
                //en nuestra base de datos de sinonimos
                if(DataSet.Dict.ContainsKey(word)==false && DataSet.Synonymous.ContainsKey(word)==true)
                {
                    //itera por todos los sinonimos de word que estan en el corpus
                    foreach(string sym in DataSet.Synonymous[word])
                    {
                        if(DataSet.Dict.ContainsKey(sym))
                        {
                            QueryVect.values[DataSet.IndexForEachWord[sym]]+=0.5;
                        }
                    }
                }
            }
        }
    }
}