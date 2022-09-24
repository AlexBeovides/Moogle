namespace MoogleEngine
{
    public class Operators
    {
        public static List<Tuple<String,Int32>> OpImportance =new List<Tuple<String,Int32>>();
        public static List<String> OpMustBe =new List<String>();
        public static List<String> OpCantBe =new List<String>();
        public static List<Tuple<String,String>> OpNear =new List<Tuple<String,String>>();

        public static List<Tuple<double,int>> Apply(List<Tuple<double,int>> toRank)
        {
            int DocIndx;
            int TermIndx;

            for(int i=0 ; i<OpMustBe.Count ; i++)
            {
                for(int j=0 ; j<toRank.Count ; j++)
                {
                    DocIndx=toRank[j].Item2;
                    TermIndx=DataSet.IndexForEachWord[OpMustBe[i]];

                    if(DataSet.VectorialModelClone[DocIndx].values[TermIndx]==0)
                    {
                        toRank[j]=Tuple.Create(0d,DocIndx);
                    }
                }
            }

            for(int i=0 ; i<OpCantBe.Count ; i++)
            {
                for(int j=0 ; j<toRank.Count ; j++)
                {
                    if(toRank[j].Item1==0d)
                    {
                        continue;
                    }

                    DocIndx=toRank[j].Item2;
                    TermIndx=DataSet.IndexForEachWord[OpCantBe[i]];

                    if(DataSet.VectorialModelClone[DocIndx].values[TermIndx]>0)
                    {
                        toRank[j]=Tuple.Create(0d,DocIndx);
                    }
                }
            }

            for(int i=0 ; i<OpImportance.Count ; i++)
            {
                for(int j=0 ; j<toRank.Count ; j++)
                {
                    if(toRank[j].Item1==0d)
                    {
                        continue;
                    }

                    DocIndx=toRank[j].Item2;
                    TermIndx=DataSet.IndexForEachWord[OpImportance[i].Item1];

                    double increase=Math.Pow(2,OpImportance[i].Item2);

                    if(DataSet.VectorialModelClone[DocIndx].values[TermIndx]>0)
                    {
                        toRank[j]=Tuple.Create(toRank[j].Item1*increase,DocIndx);
                    }
                }
            }

            for(int i=0 ; i<OpNear.Count ; i++)
            {
                for(int j=0 ; j<toRank.Count ; j++)
                {
                    if(toRank[j].Item1==0d)
                    {
                        continue;
                    }

                    DocIndx=toRank[j].Item2;
                    int Term1Indx=DataSet.IndexForEachWord[OpNear[i].Item1];
                    int Term2Indx=DataSet.IndexForEachWord[OpNear[i].Item2];

                    //si este documento no tiene ninguna d las dos palabras que tienen que estar cerca
                    //no nos sirve
                    if(DataSet.VectorialModelClone[DocIndx].values[Term1Indx]==0 || 
                    DataSet.VectorialModelClone[DocIndx].values[Term2Indx]==0)
                    {
                        continue;
                    }

                    int fwi=Int32.MaxValue;
                    int swi=Int32.MaxValue;
                    int bestdist=Int32.MaxValue;

                    List<Tuple<string,int,int>> content=Document.Documents[DocIndx].content;

                    for(int k=0 ; k<content.Count ; k++)
                    {
                        if(content[k].Item1==OpNear[i].Item1)
                        {
                            fwi=k;
                            bestdist=Math.Abs(fwi-swi);
                        }
                        else if(content[k].Item1==OpNear[i].Item2)
                        {
                            swi=k;
                            bestdist=Math.Abs(fwi-swi);
                        }
                    }

                    double increase=(1000/bestdist);

                    toRank[j]=Tuple.Create(toRank[j].Item1*increase,DocIndx);
                }
            }

            OpCantBe.Clear();
            OpMustBe.Clear();
            OpNear.Clear();
            OpImportance.Clear();

            return toRank;
        }
    }
}