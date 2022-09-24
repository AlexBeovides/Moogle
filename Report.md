# Moogle! 2022

> Proyecto de Programación. Facultad de Matemática y Computación. Universidad de La Habana. Curso 2022.

> Alex Samuel Bas Beovides 

Moogle! es un motor de búsqueda que opera con un conjunto de documentos y una consulta. Para su realización escogimos el modelo vectorial de recuperación de información por las ventajas que ofrece: el esquema de ponderación y la estrategia de coincidencia parcial. Con coincidencia parcial nos referimos a que la consulta no tiene que coincidir exactamente con un documento para ser considerado relevante. Por ejemplo, un documento relevante puede no cumplir con todos los requerimientos de la consulta sino con un subconjunto de ellos. Son precisamente estas diferencias las que permiten realizar una ordenación de los documentos recuperados.

![alt text](Report1.png)

# Clases y métodos

Las clases utilizadas y sus métodos son:
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

El programa al levantar hace una llamada al método `init()`, donde se realiza un pre procesamiento de todos los documentos que están en el corpus. Aquí se guarda el .json de los sinónimos en un diccionario, se guardan los documentos como strings en una lista, luego en el método `normalize()` cada uno de estos se separa por palabras y se guardan como una lista de strings. Posteriormente transformamos cada documento en un vector de `n` dimensiones, que vamos a almacenar en arreglos de tipo `double` y tamaño `n`, donde las coordenadas de cada dimensión será la relevancia que tiene cada palabra para el documento. La relevancia es calculada por la siguiente fórmula:

${TF_{i,j}} \times {IDF_i}$
$={ \dfrac{freq_{i,j}}{maxfreq_j}} \times {\log_{10} (\dfrac{N}{df_i})}$

${freq_{i,j}}=$ cantidad de veces que aparece el termino i en el documento j.

${maxfreq_j}=$ la máxima frecuencia de cualquier termino en el documento j.

$N=$ cantidad total de documentos en el corpus.

${df_i}=$ cantidad de documentos donde aparece el termino i.

- ## Query

Luego el usuario eventualmente introduce una query, la cual es procesada como un documento, con la única diferencia de que se extraen los operadores en el caso de que hayan sido introducidos. En el caso de que la query posea una palabra que no aparezca en el corpus, analizamos la posibilidad de que sea un sinónimo de alguna que si tengamos. Si no lo es, suponemos que el usuario se equivoco escribiendo, y procedemos a emplear el `algoritmo de Levenshtein` una métrica que mide la diferencia entre dos cadenas basado en el número mínimo de ediciones de un solo carácter que se requieren para cambiar una cadena por otra. Esta herramienta la utilizaremos entre el termino escrito por el usuario y todas las palabras en nuestro corpus, quedándonos con la que tenga menor distancia de edición, la sustituimos por el termino mal escrito, y modificamos el atributo `suggestion` de la clase `SearchResult`. Luego vectorizamos la query, y procedemos a hallar la similaridad entre la query y todos los documentos de nuestro corpus, mediante la siguiente formula denominada `cosine similarity`:

$sim(d_j,q)$
$=\dfrac{\sum_{i=1}^{n} {w_{i,j} \times {w_{i,q}} }} {{\sqrt{\sum_{i=1}^{n} {w^2_{i,j}} } } \times {\sqrt{\sum_{i=1}^{n} {w^2_{i,q}} }}}$

$d_j=$ j-ésimo documento expresado como vector n-dimensional

$q=$ query expresada como vector n-dimensional

$w_{i,j}=$ relevancia del término i en el documento j

Ya con las similaridades calculadas, tomaremos estas como puntuaciónes, que usaremos para ordenar los documentos por orden de relevancia y luego mostrárselos al usuario. Esta puntuación solo se verá modificada por los operadores, cuyas funcionalidades explicaremos en el próximo apartado.


- ## Operadores

    - ! (no debe aparecer): iteramos sobre las palabras que tengan a este operador antes, luego iteramos por todos los documentos que tengan a esta palabra, y hacemos que su `puntuación` sea 0.

    - ^ (tiene que aparecer): iteramos sobre las palabras que tengan a este operador antes, luego iteramos por todos los documentos que no tengan a esta palabra, y hacemos que su `puntuación` sea 0.

    - <p>* (importancia): iteramos sobre las palabras que tengan a este operador antes, luego iteramos por todos los documentos que tengan a esta palabra, y hacemos que su `puntuación` sea aumentada `n` veces, siendo n la cantidad de veces que aparezca este operador antes de la palabra.</p>

    - ~ (cercanía): iteramos sobre los pares de palabras que tengan a este operador antes, luego iteramos por todos los documentos que tengan a este par de  palabras, buscamos el par que más cerca este, y la puntuación de este documento se verá afectada por la siguiente ecuación de proporcionalidad inversa:
    <br>$p_j={p_j} \times {\dfrac{1000}{bestdist}}$ 
    <br>donde $p_j$ es la `puntuación` del documento j-ésimo, y $bestdist$ es la menor distancia entre todos los pares de las palabras relacionadas por el operador de cercanía en el documento j-ésimo.

- ## Snippet

Una vez ordenados los documentos por orden de relevancia y que ya han sido aplicados los operadores, necesitamos mostrar al usuario una porción de cada resultado, lo que denominamos `Snippet`. Para esto, tomamos un rango de 60 palabras, y para cada documento vamos revisando todos los segmentos de 60 palabras de largo que hay, acumulando en la variable `currentSnippetValue` los valores de relevancia para la query de cada palabra del documento, y cada vez que encontramos un rango con mejor valor que el anterior, modificamos la variable `bestSnippetValue`, así nos quedamos, con el fragmento de texto que mas se asimile a la query del usuario.

