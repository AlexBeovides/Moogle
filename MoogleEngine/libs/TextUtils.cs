using System.Text;

namespace MoogleEngine
{
    //la clase TextUtils contiene todos los metodos y estructuras relacionadas al trabajo con
    //textos
    public static class TextUtils
    {
        //funcion que obtiene los nombres de los documentos
        public static List<string> getDocNames(string dir)
        {
            //creando una lista que va a contener todos los nombres de los archivos
            List<string> names = new List<string> (Directory.GetFiles(dir, "*.txt", SearchOption.AllDirectories));
            
            //iterando por los archivos de la carpeta content
            for(int i=0 ; i<names.Count ; i++)
            {
                //recortando, quedandonos con solo el nombre de los archivos
                names[i]=names[i].Substring(dir.Length);
            }

            //devolviendo los nombres de los ficheros
            return names;
        }
        //evalua la validez del caracter
        //si es un caracter ASCII y es un digito o una letra
        public static bool isValidCharacter(int caracter)
        {
            if(caracter<256 && Char.IsLetterOrDigit((char)caracter))
            {
                return true;
            }

            return false;
        }
        //funcion para normalizar los documentos (llevar las palabras a minusculas 
        //y remover los caracteres q no sean ni digitos ni letras) 
        public static List<Tuple<string,int,int>> normalize(string words,bool isQuery)
        {
            //en este array vamos a retornar el documento normalizado
            List<Tuple<string,int,int>> normalized=new List<Tuple<string,int,int>>();

            // volviendo minusculas todas las letras
            words=words.ToLower();

            //aqui se van a ir generando las palabras validas normalizadas
            StringBuilder newword=new StringBuilder();

            int FirstLetter=-1;
            int LastLetter=-1;

            //iterando por las palabras del documento
            for(int i=0 ; i<words.Length ; i++)
            {
                //evalua la validez del caracter
                //si es un caracter ASCII y es un digito o una letra
                if(isValidCharacter((char)words[i]))
                {
                    //agrega el caracter a la palabra que estamos formando
                    newword.Append(words[i]);

                    if(newword.Length==1)
                    {
                        FirstLetter=i;
                    }
                }
                else
                {
                    //si encuentra un caracter no valido
                    //agrega el string actual a la lista q vamos a retornar
                    //y la incorpora al dataset
                    if(newword.Length>0)
                    {
                        LastLetter=i-1;

                        normalized.Add(Tuple.Create(newword.ToString(),FirstLetter,LastLetter));

                        if(isQuery==false)
                        {
                            DataSet.Add(newword.ToString());
                        }

                        newword.Clear();
                    }
                } 
            }

            //si encuentra un caracter no valido
            //agrega el string actual a la lista q vamos a retornar
            //y la incorpora al dataset
            if(newword.Length>0)
            {
                //la ultima posicion del documento
                LastLetter=words.Length-1;

                normalized.Add(Tuple.Create(newword.ToString(),FirstLetter,LastLetter));

                if(isQuery==false)
                {
                    DataSet.Add(newword.ToString());
                }
                
                newword.Clear();
            }

            return normalized;
        } 

        public static int EditDistance(string wordA, string wordB)
        {
            List<int[]>dp=new List<int[]>();
            
            wordA='*'+wordA;
            wordB='*'+wordB;

            for(int i=0 ; i<wordA.Length ; i++)
            {
                dp.Add(new int[50]);

                dp[i][0]=i;
            }
            for(int i=1 ; i<wordB.Length ; i++)
            {
                dp[0][i]=i;
            }

            for(int i=1 ; i<wordA.Length ; i++)
            {
                for(int j=1 ; j<wordB.Length ; j++)
                {
                    dp[i][j]=Math.Min(dp[i-1][j],dp[i][j-1]);
                    dp[i][j]=Math.Min(dp[i][j],dp[i-1][j-1]);

                    if(wordA[i]==wordB[j] && dp[i][j]==dp[i-1][j-1])
                    {
                        continue;
                    }
                    else
                    {
                        dp[i][j]++;
                    }
                }
            }

            return dp[wordA.Length-1][wordB.Length-1];
        }

        public static string buildString(List<Tuple<string,int,int>> content)
        {
            StringBuilder buildedString=new StringBuilder();

            for(int i=0 ; i<content.Count ; i++)
            {
                for(int j=0 ; j<content[i].Item1.Length ; j++)
                {
                    buildedString.Append(content[i].Item1[j]);
                }

                //evitamos agregar un espacio en blanco al final
                if(i+1<content.Count)
                {
                    buildedString.Append(" ");
                }
            }

            return buildedString.ToString();
        }
    }
        
}

