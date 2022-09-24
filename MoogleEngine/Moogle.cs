using System.Text.Json;

namespace MoogleEngine
{
    public static class Moogle
    {
        public static Dictionary<string,bool> termsInQuery=new Dictionary<string,bool>();

        public static TimeSpan QueryTime= new TimeSpan();
       
        //inicializando las estructuras necesarias para la busqueda
        public static void init()
        {
            //en esta lista se guardan los nombres de los documentos
            List<string>docNames=TextUtils.getDocNames("../Content/");

            string SynJsonString = File.ReadAllText("..//Synonymous\\Synonymous.json");
            DataSet.Synonymous= JsonSerializer.Deserialize<Dictionary<string, List<string>>>(SynJsonString)!;

            //it es un iterador, para saber que indice tiene cada documento en el corpus
            int it=0;

            //iterando por los nombres de los documentos
            foreach (string name in docNames)
            {
                //aqui guardaremos el documento completo
                string content = File.ReadAllText("../Content/"+name);

                //arreglo de strings que va a contener el documento normalizado
                List<Tuple<string,int,int>> words=TextUtils.normalize(content,false);

                //agrega a la lista Documents de la clase Document, un nuevo Document
                Document.Documents.Add(new Document(name,it,content,words,0));

                it++;
            }

            //transforma en vector a todos los documentos
            Document.DocumentToVector(); 
            
            //calcula la frecuencia de cada palabra en el corpus
            Vector.IdfFill();
       
            //transforma el valor de cada dimension de cada vector en su TfIdf
            Vector.ToVectorTfIdf();
        }
        
        public static SearchResult QueryRequest(string query) 
        {
            termsInQuery.Clear();
        
            DateTime start=DateTime.Now;

            List<Tuple<string,int,int>> content=TextUtils.normalize(query,true);

            for(int i=0 ; i<content.Count ; i++)
            {
                termsInQuery.Add(content[i].Item1,true);
            }

            string suggestion="";

            if(Query.PurifyQuery(content))
            {
                suggestion=TextUtils.buildString(content);
            }

            Document QueryDoc=new Document("",Document.Documents.Count,query,content,0);

            Vector QueryVect=Document.ToVector(QueryDoc,true);

            Query.SearchSynonymous(QueryDoc,QueryVect);

            Query.QueryTfIdf(QueryVect);

            Query ActualQuery=new Query(query,content,QueryVect);
    
            List<SearchItem> items=Query.RankDocuments(ActualQuery);
            
            //tiempo de ejecucion
            DateTime end=DateTime.Now;

            QueryTime=end-start;

            return new SearchResult(items,suggestion);
        }
    }
}

