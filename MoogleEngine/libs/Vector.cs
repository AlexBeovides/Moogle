namespace MoogleEngine
{
    //la clase vector contiene todas las estructuras y metodos que tienen que ver con
    //el modelo vectorial y los correspondientes vectores
    public class Vector 
    {
        //el constructor vector, contiene el parametro values, que contiene 
        //los valores de relevancia de cada palabra de cada documento
        //calculados mediante la funcion tf-idf
        public Vector(double[] values)
        {
            this.values=values;
        }

        public double[] values {get ; private set; }

        //el array de double Idf contiene la cantidad de veces que se repite cada palabra
        //en todos los documentos del corpus
        public static double[] Idf=new double[DataSet.Dict.Count];

        //el metodo IdfFill, se encarga de llenar el array Idf,
        public static void IdfFill()
        {
            //este ciclo itera por todas las palabras del diccionario
            for(int i=0 ; i<DataSet.Dict.Count ; i++)
            {
                //cont va a guardar la cantidad de documentos en los que aparece 
                //la palabra i
                double cont=0;

                //este ciclo itera por todos los documentos del corpus
                for(int j=0 ; j<Document.Documents.Count ; j++)
                {
                    //aumentamos cont en 1 si la palabra i aparece en el documento j
                    if(DataSet.VectorialModel[j].values[i]>0)
                    {
                        cont++;
                    }
                }

                //la InverseDocumentFrecuency del termino i es igual al logaritmo en base diez 
                //de la cantidad de documentos del corpus, dividida por la cantidad de 
                //documentos en los que aparece el termino i
                Idf[i]=Math.Log10(Document.Documents.Count/cont);
            }
        }

        //el metodo ToVectorTfIdf transforma la frecuencia de cada palabra j en el
        //documento i, con el TfIdf de la palabra, que equivale a su relevancia para 
        //el corpus
        public static void ToVectorTfIdf()
        {
            //itera por cada documento del corpus
            for(int i=0 ; i<Document.Documents.Count ; i++)
            {
                //itera por cada palabra del diccionario
                for(int j=0 ; j<DataSet.Dict.Count ; j++)
                {
                    //reemplaza la frecuencia del termino, con su TfIdf
                    DataSet.VectorialModel[i].values[j]=TfIdf(i,j);
                }
            }
        }

        //el metodo TfIdf, recibe el indice del documento y el termino a procesar
        //y devuelve un double, que representa la importancia de ese termino para ese 
        //documento en especifico
        public static double TfIdf(int DocumentIndex, int TermIndex)
        {
            //TermFrecuency almacena la frecuencia del termino en el documento
            double TermFrecuency=0;
            
            //la frecuencia del termino en el documento es igual a la cantidad de veces que
            //se repite el termino en el documento, dividido por la cantidad de palabras que 
            //contiene el documento
            TermFrecuency=(DataSet.VectorialModel[DocumentIndex].values[TermIndex])/Document.Documents[DocumentIndex].maxFreq;

            return TermFrecuency*Idf[TermIndex];
        }
    }
}