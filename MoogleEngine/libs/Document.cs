namespace MoogleEngine
{
    //la clase Document, contiene todos los metodos, objetos y estructuras relacionadas
    //al trabajo con los documentos procesados
    public class Document
    {
        //el constructor Document, recibe el indice de un documento, el string que contiene
        //el texto original, una lista de string,int,int que almacena el contenido del documento
        //procesado y normalizado,con la primera y ultima posicion de cada palabra del txt
        //, y un double que guarda la maxima frecuencia de cualquier 
        //termino en el documento
        public Document(string title,int index, string original, List<Tuple<string,int,int>> content,double maxFreq)
        {
            this.title=title;
            this.index=index;
            this.original=original;
            this.content=content;
            this.maxFreq=maxFreq;
        }

        public string title { get; private set; }
        public int index { get; private set; }
        public string original { get; private set; }
        public List<Tuple<string,int,int>> content { get; private set; }
        public double maxFreq { get; private set; }

        //la lista de Document Documents, almacena todos los objetos Document 
        public static List<Document> Documents=new List<Document>();

        //DocumentToVector va a iterar por todos los documentos del corpus
        //y va a generar un vector de cada uno y lo va a agregar al modelo vectorial
        public static void DocumentToVector()
        {
            //itera por todos los documentos del corpus
            for(int i=0 ; i<Documents.Count ; i++)
            {
                //agrega el vector que devuelve ToVector, a la lista VectorialModel
                Vector vect=ToVector(Documents[i],false);
                DataSet.VectorialModel.Add(vect);

                Vector vect2=ToVector(Documents[i],false);
                DataSet.VectorialModelClone.Add(vect2);
                
                Documents[i].maxFreq=maxFreqFill(vect);
            }
        }

        //el metodo ToVector, se encarga de transformar un documento  
        //en un vector de n dimensiones
        //recibe un documento y devuelve un vector
        public static Vector ToVector(Document Doc,bool isQuery)
        {
            //el arreglo de double Values va a almacenar para cada palabra del nuevo vector
            //su importancia para el documento al que pertenece
            double[] Values=new double[DataSet.Dict.Count];

            //itera por el documento
            for(int j=0 ; j<Doc.content.Count ; j++)
            {
                if(isQuery==true && !DataSet.Dict.ContainsKey(Doc.content[j].Item1))
                {
                    continue;
                }
                //aumentando la frecuencia del termino j del documento en 1
                Values[DataSet.IndexForEachWord[Doc.content[j].Item1]]++;
            }

            //crea un nuevo Vect, con el array Values
            Vector Vect=new Vector(Values);

            return Vect;
        } 

        //maxFreqFill se va a encargar de otorgarle a un documento
        //la mayor frecuencia de cualquier palabra que este contenga
        public static double maxFreqFill(Vector vect)
        {
            //maxFreq va a almacenar la mayor frecuencia de cualquier 
            //palabra en el 
            double maxFreq=0;   

            //itera por el documento
            for(int j=0 ; j<DataSet.Dict.Count ; j++)
            {
                //maxFreq va a ser igual al maximo entre la mayor frecuencia encontrada
                //hasta el momento, y la frecuencia del termino j
                maxFreq=Math.Max(maxFreq,vect.values[j]);
            }

            return maxFreq;
        }
    }
}

