
namespace MoogleEngine
{
    //la clase DataSet contiene todos los metodos, objetos y estructuras relacionados con 
    //los datos
    public class DataSet
    {
        //VectorialModel es una lista de vectores, que contiene todos los documentos
        //convertidos a vector
        public static List<Vector> VectorialModel=new List<Vector>();

        public static List<Vector> VectorialModelClone=new List<Vector>();
        
        //Dict es un diccionario, que almacena para cada palabra, la frecuencia de esta 
        //en el corpus
        public static Dictionary<string,int> Dict=new Dictionary<string,int>();

        //WordForEachIndex es un diccionario, que almacena para cada dimension de un vector,
        //la palabra del corpus que le corresponde
        public static Dictionary<int,string> WordForEachIndex=new Dictionary<int,string>();

        //IndexForEachWord es un diccionario, que almacena para cada palabra del corpus,
        //la dimension de un vector que le corresponde
        public static Dictionary<string,int> IndexForEachWord=new Dictionary<string,int>();

        public static Dictionary<string, List<string>> Synonymous = new Dictionary<string, List<string>>();

        //currentIndex es un contador, para saber que indice tendra cada palabra
        //que dimension de un vector le va a corresponder
        public static int currentIndex=0;

        //el metodo Add, modifica los diccionarios Dict,WordForEachIndex e IndexForEachWord
        static public void Add(string word)
        {
            //si Dict ya contiene la palabra, aumenta la frecuencia de esta en el diccionario
            if(Dict.ContainsKey(word))
            {
                Dict[word]++;
            }
            //si no la contiene, la agrega al mismo, y le hace corresponder una dimension
            //para el trabajo con vectores, y a dicha dimension(index) le hace corresponder
            //esta palabra
            else
            {
                Dict.Add(word,1);
                IndexForEachWord.Add(word,currentIndex);
                WordForEachIndex.Add(currentIndex,word);
                currentIndex++;
            }
        }
    }
}