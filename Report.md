# Moogle 2022

> Proyecto de Programación. Facultad de Matemática y Computación. Universidad de La Habana. Curso 2022.

> Alex Samuel Bas Beovides 

Nuestro proyecto utiliza un modelo vectorial para buscar texto dentro de un corpus de documentos.

![alt text](Report1.png)


# Clases y métodos

Las clases utilizadas son:
- DataSet
    - Add
- Document
    - DocumentToVector
    - ToVector
    - maxFreqFill
- Operators
    - Apply
- Query
    - QueryTfIdf
    - RankDocuments
    - CosineSimilarity
    - PurifyQuery
    - SearchSynonymous
- Snippet
    - RetrieveSnippet
    - retrieveTxtSegment
- TextUtils
    - getDocNames
    - isValidCharacter
    - checkOper
    - normalize
    - EditDistance
    - buildString
- Vector
    - IdfFill
    - ToVectorTfIdf
    - TfIdf
- Moogle
    - init
    - QueryRequest
- SearchItem
- SearchResult

# Flujo de Datos

- ## Precálculo

El programa al levantar hace una llamada al metodo `init()`, donde se realiza un preprocesamiento de todos los documentos que estan en el corpus. Aqui se guarda el .json de los sinonimos en un diccionario, se guardan los documentos como strings en una lista, luego en el método `normalize()` cada uno de estos se separa por palabras y se guardan como una lista de strings. Posteriormente transformamos cada documento en un vector de `n` dimensiones, que vamos a almacenar en arreglos de tipo `double` y tamaño `n`, donde las coordenadas de cada dimension sera la relevancia que tiene cada palabra para el documento. La relevancia es calculada por la siguiente formula:

${TF_{i,j}} \times {IDF_i}$
$={ \dfrac{freq_{i,j}}{maxfreq_j}} \times {\log_{10} (\dfrac{N}{df_i})}$

${freq_{i,j}}=$ cantidad de veces que aparece el termino i en el documento j.

${maxfreq_j}=$ la maxima frecuencia de cualquier termino en el documento j.

$N=$ cantidad total de documentos en el corpus.

${df_i}=$ cantidad de documentos donde aparece el termino i.

- ## Query

Luego el usuario eventualmente introduce una query, la cual es procesada como un documento, con la unica diferencia de que se extraen los operadores en el caso de que hayan sido introducidos. En el caso de que la query posea una palabra que no aparezca en el corpus, analizamos la posibilidad de que sea un sinonimo de alguna que si tengamos. Si no lo es, suponemos que el usuario se equivoco escribiendo, y procedemos a emplear el `algoritmo de Levenshtein` una metrica que mide la diferencia entre dos cadenas basado en el número mínimo de ediciones de un solo carácter que se requieren para cambiar una cadena por otra. Esta herramienta la utilizaremos entre el termino tipeado por el usuario y todas las palabras en nuestro corpus, quedandonos con la que tenga menor distancia de edicion, la sustituimos por el termino mal escrito, y modificamos el atributo `suggestion` de la clase `SearchResult`. Luego vectorizamos la query, y procedemos a hallar la similaridad entre la query y todos los documentos de nuestro corpus, mediante la siguiente formula denominada `cosine similarity`:

$sim(d_j,q)=$
$$\dfrac{\sum_{i=1}^{n} {w_{i,j} \times {w_{i,q}} }} {{\sqrt{\sum_{i=1}^{n} {w^2_{i,j}} } } \times {\sqrt{\sum_{i=1}^{n} {w^2_{i,q}} }}}$$

$d_j=$ j-esimo documento expresado como vector n-dimensional

$q=$ query expresada como vector n-dimensional

$w_{i,j}=$ relevancia del termino i en el documento j

Ya con las similaridades calculadas, tomaremos estas como puntuaciones, que usaremos para ordenar los documentos por orden de relevancia y luego mostrarselos al usuario. Esta puntuacion solo se vera modificada por los operadores, cuyas funcionalidades explicaremos en el proximo apartado.



- ## Operadores

- ## Snippet