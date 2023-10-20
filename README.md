# Ahogado-en-Impuestos

![LogoGluGluGames2](https://github.com/GluGluGames/Ahogado-en-Impuestos/assets/112829139/bb7bf48e-2b66-48a7-9d21-e7d79f0473aa)
## 0-Revisiones
- 16/10/2023 - Se ha actualizado la descripcion de los recursos y se ha metido el flujo de juego,los animales,la descripcion de la niebla de guerra, y de la persecucion de los depredadores.
- 17/10/2023 - Se han corregido ciertas faltas ortográficas.
- 17/10/2023 - Luming - Aumento de detalles en tablas para los edificios, poner tutorial y progresion en game design, adjuntando sus propias diagramas de flujo. Aparte, tambien se ha metido los graficos sobre la recaudacion y la curva de aprendizaje
- 18/10/2023 - Luming - Aumento de explicaciones sobre la probabilidad de recaudacion y la curva de aprendizaje, y aclaracion de game design
- 20/10/2023 - Luming - Corregido el game design, Intensidad de recaudacion -> Variedad de recaudacion segun la productividad, meter recursos de tipo de especial,
- 
## 1 - MÉCANICAS 

### **1.1 - Mecánica principal**: 

Construcción de edificios para el aumento de la producción, y minijuegos ocasionales para la obtención de recursos más raros y la apertura de nuevas estructuras/mecánicas.  
El juego se divide en dos grandes apartados. El primero de ellos es el **escenario principal**, y el segundo será un o varios **minijuegos**. Adelante entramos más en detalle en estos apartados.

### **1.2 - Escenario principal**

Este escenario consiste en un mapa dividido en terrenos hexagonales. Estos terrenos serán generados de forma procedural en el editor del juego, pero podrán y serán modificados por los artistas para hacer que este mundo sea artísticamente agradable y produzca cierto atractivo. Algunos ejemplos de una estructura “grid hexagonal” como esta se encuentran en juegos como “Before we Leave” o “Civilization”. En estos juegos las casillas aportan más o menos recursos dependiendo de su posición o recursos naturales. En nuestro caso no haremos esto, sino que lo simplificaremos, los recursos otorgados solo dependen de los edificios cercanos a esta casilla. 

En el juego de Civilization los mapas son procedurales, pero en el nuestro serán creados a mano por el equipo. 

Estas casillas al principio se encuentran asilvestradas, pero pagando una suma de recursos pasas a poder construir el edificio que se desee.  

Cuando comiences el juego las casillas lejanas van a estar cubiertas por una “niebla de guerra”, donde no se podrá construir. Se eliminará la niebla y se desbloqueará la posibilidad de construir en estas casillas cuando se construya un edificio cercano a la niebla de guerra. Por ejemplo, los edificios otorgan una o dos casillas de visibilidad a su alrededor. Se encuentra bajo discusión si hacemos que los edificios otorguen más visión dependiendo de su nivel y/o tipo de estructura. 

### **1.3 - Minijuegos** 

#### **1.3.1 - Plataforma hexagonal**
Se utilizara el grid de hexagonos para la expedicion, lo que se va a crear son los prompt que se generaran de forma procedural(aleatoria dentro del mapa) para una experiencia nueva hacia el jugador,aparte de ello, se va a introducir prompts de logro que existe una cantidad minima de encontrarlos.

#### **1.3.2 - Niebla de guerra**
En el mágico mundo submarino de nuestro juego, la niebla de guerra es una característica crucial que añade un toque de misterio a cada aventura. A medida que los jugadores se adentran en las profundidades del océano, la visibilidad se ve afectada por la profundidad de la zona y el nivel de su casco submarino. Esta limitación en la visión invita a los jugadores a explorar lo desconocido, estimulando su curiosidad y el deseo de desvelar los secretos que aguardan en las profundidades.

Conforme los jugadores exploran el mapa, las zonas previamente descubiertas permanecen visibles, garantizando un sentido de seguridad y orientación en su entorno. Sin embargo, en las áreas desconocidas, existe la posibilidad de que depredadores detecten al jugador. En tales situaciones, se activará un icono de alerta para notificar al jugador, creando un nivel adicional de tensión y emoción.

Esta combinación de elementos asegura que la exploración sea una experiencia inmersiva y emocionante, donde cada nueva área presenta desafíos y oportunidades, invitando a los jugadores a sumergirse en las profundidades en busca de tesoros y descubrimientos asombrosos.

#### **1.3.3 - Persecucion de los depredadores**
Dentro del entorno de exploración, la presencia de depredadores añade una capa adicional de emoción y desafío al juego. Los jugadores deben mantenerse alerta y vigilar las casillas circundantes en busca de signos de depredadores cercanos. El rango de visión de estos depredadores está inversamente relacionado con el daño que pueden infligir, lo que significa que los jugadores no deben preocuparse por depredadores gigantes y rápidos que sean difíciles de evitar.

Cuando un depredador detecta al jugador, iniciará un movimiento hacia él con la intención de atacar. En esta situación, el jugador debe tomar medidas para escapar y cansar al depredador lo antes posible. La eficacia de esta estrategia dependerá de la rapidez con la que el jugador pueda evadir al depredador.

Es fundamental comprender que las diferentes profundidades en el juego albergan distintos tipos de depredadores. Esto requiere que los jugadores planifiquen sus estrategias de huida con cuidado, ya que intentar escapar de un depredador puede llevarlos directamente hacia la amenaza de otro, potencialmente más peligroso.

Esta mecánica añade un elemento táctico al juego, donde los jugadores deben sopesar cuidadosamente sus opciones y decidir si enfrentarán a un depredador, huirán o buscarán rutas de escape inteligentes para evitar situaciones peligrosas. La gestión adecuada de los encuentros con depredadores es esencial para una exitosa exploración del mundo submarino.

### **1.4 - Recursos** 
**Recursos generales del Juego:**

- Algas Verdes: Se utilizan tanto para el costo de construcción como para el costo de mejoras en la ciudad. Estos son recursos esenciales para la expansión y mejora de la ciudad.

- Algas Rojas: Se utilizan para comprar objetos de aumento (boosts) dentro de la tienda del juego. Estos objetos pueden proporcionar ventajas temporales a los jugadores y pueden ser útiles en situaciones específicas.

- Perlas: Recurso versátil que tiene múltiples usos en el juego. Proporcionan flexibilidad a los jugadores para abordar diferentes aspectos del juego.

- Madera, Corales y Hierro: Estos materiales son utilizados en la construcción y mejora de edificios dentro de la ciudad. Los corales se utilizan para construcciones iniciales, la madera para reparaciones y el hierro para mejoras.

**Recursos espciales del Juego:**
- Tablilla: Las tablillas desempeñarán un papel fundamental en el juego, ya que servirán para desbloquear edificios especiales. En este caso, se han diseñado cuatro tablillas diferentes para desbloquear cuatro edificios distintos. Estos edificios incluyen un generador de electricidad, un paratrápido, un laboratorio y mejoras en las capacidades de buceo.
Estas tablillas ofrecen a los jugadores la oportunidad de acceder a nuevas construcciones y mejoras que enriquecen su experiencia de juego y les brindan herramientas adicionales para la reconstrucción de la ciudad perdida de Atlantis.

- Mapa: Los mapas desempeñan un papel crucial en el juego, ya que permiten a los jugadores desbloquear zonas diferentes, cada una con recursos únicos. Estos recursos pueden variar en cantidad y rareza, lo que fomenta la exploración activa y añade un elemento divertido y desafiante al juego. En total, se han diseñado cinco zonas distintas, cada una con su propio conjunto de tesoros por descubrir.
Estas zonas desbloqueables brindan a los jugadores la oportunidad de diversificar su experiencia y recompensan su curiosidad a medida que se aventuran en lo desconocido en busca de tesoros submarinos. 

### **1.5 - Edificios** 
#### **1.5.1 - Granja de algas**
La granja de algas generara un número de algas por unidad de tiempo. 

Al subirla de nivel: el número de algas generadas aumentará.
| Nivel | Coste de construccion| Coste de mejora | Coste de reparacion| Productividad | Justificacion|
| :--: | :--: | :--: | :--: | :--: | ------------ |
| 1 | 2000 algasverdes + 400 corales| x | 100 madera |  20 algas/5s | Nivel bajo, corresponde con baja productividad, incentivar al jugador ir de expedicion |
| 2 | x| 400 Hierro + 400 corales | 400 madera |  40 algas/3s | Nivel intermedio, corresponde con productividad medio bajo, incentivar al jugador acceder a expedicion para conseguir el mejor nivel de la construccion |
| 3 | x| 600 Hierro + 400 corales | 800 madera |  100 algas/s | Nivel alto, productividad alta como recompensa para el jugador una vez haya conseguido el nivel maximo del edificio. |

#### **1.5.2 - Tienda de objetos**
La tienda de objetos servirá al jugador para intercambiar un número de materiales por otros. 
  - Se actualizará cada cierto tiempo ofreciendo nuevos intercambios.
  - Cada intercambio se podrá realizar un número límitado de veces.

Al subirla de nivel: se ofrecerá un mayor número de intercambios al día y de recursos más valiosos.
| Nivel | Coste de restauracion| Coste de mejora |Intercambios permitidos| Justificacion|
| :--: | :--: | :--: | :--: |------------ |
| 1 | 1000 algas verdes| x | 3 | La tienda se sirve como ayuda para el jugador, no queremos que el jugador se relegue solo en los intercambios de elementos
| 2 | x| 200 hierro | 5 | En esta fase el jugador ya ha avanzado parte del juego, por lo tanto se le permite mas intercambios cada dia
| 3 | x| 300 hierro  | 10 | Aqui el jugador ya habra avanzado bastante en la construccion de la ciudad por lo tanto se permiten hasta 10 ya que puede hacer falta los recursos en ocaciones, sin abusar del intercambio|

#### **1.5.3 - Generador de electricidad**
El generador de electricidad requerirá de anguilas eléctricas para construirse. Su principal función es generar cargas que pueden usarse para:
  - Boostear un edificio seleccionado por el jugador.
  - Recargar el paratridentes para poder usarlo de nuevo.
    
| Nivel | Coste de Construcción | Coste de Mejora | Cargas - Aceleración / Tiempo | Recurso necesario para activar | Justificación |
| :---: | :-------------------: | :-------------: | :------------------: | :-----------------------------: | :-----------: |
| 1| 2000 algas verdes + 5 anguilas electricas | x | +10% de la productividad actual / 5min| 10 pez | Nivel bajo y de tipo acelerador, se pide cantidad mediano de recurso para activar y aumenta poca cantidad de productividad|
| 2| x | 200 hierro + 15 anguilas electricas | 2 slots +30% de la productividad actual/10min | 15 pez | Nivel medio, se aumenta la productivad pero tambien el coste por uso. |
| 3| x | 400 hierro + 25 anguilas electricas | 3 Slots +90% de la productividad actual /15min| 50 pez | Version final de la construccion, mejora significativa pero consumo significativo tambien.|

 Al subirlo de nivel: en el nivel II habrá dos cargas disponibles y en el nivel III, tres cargas disponibles. El uso de estas cargas es asignado por el jugador.

#### **1.5.4 - Paratridente**
El impresionante Paratridente, cuya construcción conlleva un coste significativo, posee una característica única y vital: su capacidad para proteger una zona designada durante una única ráfaga de furia. Para mantener su eficacia, este artefacto debe ser cargado con la energía eléctrica de las anguilas eléctricas, lo que añade un elemento de recolección estratégica a la ecuación. Cada Paratridente está equipado con tres cargas, lo que se traduce en tres oportunidades de activación. Cada vez que la furia de Poseidón desciende sobre una casilla protegida por el Paratridente, este responde de manera inmediata, anulando el peligro y consumiendo una de sus preciadas cargas. Sin embargo, tras cada intervención, el Paratridente requiere un período de tiempo para recargar su cúpula defensiva, que puede realizarse hasta en tres ocasiones. En el caso desafortunado de que la furia de Poseidón impacte en un edificio revelado, el resultado es un ataque devastador que deja la casilla afectada en un estado crítico. Esta casilla quedará inoperativa hasta que sea reparada, lo que añade un elemento de urgencia y gestión de recursos a la estrategia del jugador.
| Nivel | Coste de construccion| Coste de mejora | Coste de reparacion| Numero de protecciones | Tiempo de espera | Justificacion|
| :--: | :--: | :--: | :--: | :--: |:--:| ------------ |
| 1| 5000 algas verdes + 500 corales| x | 2500 algas verdes | 1 | 5min | Sirve como una proteccion basica para el jugador, pero puede ser mejorado en el futuro y aguanta mas iras de poseidon.|
| 2| x| 500 hierro + 500 corales | 2500 algas verdes | 2 | 5min | Se aumenta a 2 veces el numero de protecciones, pero sera necesario cargado por el generador de electricidad.|
| 3| x| 500 hierro + 500 corales | 2500 algas verdes | 3 | 3min | Monumento gigante que para 3 iras de poseidon, version mejorado de tiempo como recompensa para el jugador.|



#### **1.5.5 - Mejoras de buceo**
Después de llevar a cabo un impresionante total de diez expediciones, en las cuales el jugador ha demostrado una destreza y determinación sin igual, se descubre un misterioso casco mágico. Este asombroso hallazgo desencadena la apertura de un nuevo horizonte de posibilidades, en forma de un espléndido edificio dedicado exclusivamente a la mejora del buceo. Este majestuoso edificio de mejoras de buceo se convierte en un centro neurálgico de investigación submarina, donde los secretos de las profundidades marinas se desvelan lentamente. Sin embargo, la búsqueda del conocimiento tiene su precio, y el costo de investigación aumenta de forma exponencial, desafiando aún más la habilidad y el ingenio del jugador.
| Nivel | Coste de construccion | Coste de mejora | Coste de reparacion| 
| :--: | :--: | :--: | :--: |
|  1 | 2500 algas verdes | x | 1000 algas |
|  2 | x | 500 hierro | 1000 algas |
|  3 | x | 750 hierro | 1000 algas |

| Tipo | Nivel requerido de edificio| Nivel Mejora | Coste de mejora | Efecto| Justificacion| 
| :---:| :--:|:---:|:---:|:---:|:---:|
| Casco | 1 | 1->2 | 3000 algas verdes| 2 min -> 3 min | Alargar un poco el tiempo que puede permanecer el jugador dentro de la expedicion, dando lugar a mas posibilidades de recoger recursos|
| Casco | 2 | 2->3 | 5000 algas verdes| 3 min -> 5 min | Extender 2 minutos por el mismo motivo, pero con un coste superior ya que el beneficio de alargar el tiempo de expedicion a 5 minutos puede dar lugar a muchos recursos recogidos|
| Casco | 3 | 3->4 | 10000 algas verdes| 5 min -> 10 min| Extender a 10 minutos ya que en esta etapa el jugador habra cogido soltura el juego, entonces se le ofrece una mejora final del tiempo, asi para ser eficiente.|
| Aletas(Pendiente) | 1 | 1->2 | 1000 algas verdes| +1 casilla de movimiento original| Dar un beneficio inicial y permitir mas movimientos por parte del jugador.|
| Aletas(Pendiente) | 2 | 2->3 | 2000 algas verdes| +3 velocidad de movimiento original| Dar mas pasos al jugador para que pueda enfocarse en la planificacion de recolectar recursos.|
| Aletas(Pendiente) | 3 | 3->4 | 3000 algas verdes| +5 velocidad de movimiento original| El jugador ya tiene experiencia en la expedicion y sabe a donde ir, solo le damos un herramiento de hacer cosas mas rapido.|
| Mochila(Pendiente) | 1 | 1->2 | 1000 algas verdes| +10 casillas para criaturas | Aumentar la capacidad de la mochila a la hora de cazar peces o recolectar.|
| Mochila(Pendiente)| 2 | 2->3 | 1000 algas verdes| +10 casillas para criaturas |  Aumentar la capacidad de la mochila a la hora de cazar peces o recolectar.|
| Mochila(Pendiente) | 3 | 3->4 | 1000 algas verdes| +10 casillas para criaturas| Aumentar la capacidad de la mochila a la hora de cazar peces o recolectar.|





#### **1.5.6 - Edificio de investigacion** 
Un majestuoso edificio de investigación, cuidadosamente desvelado con la adquisición de una enigmática tablilla, y erigido con una preciosa cantidad de corales. Una vez completada su construcción, se convierte en el epicentro del conocimiento y la innovación, permitiendo la realización de exhaustivas investigaciones que abarcan tanto mejoras edilicias como la exploración de los mapas hallados en nuestras expediciones. A lo largo del transcurso del juego, el costo de este edificio se incrementará gradualmente, poniendo a prueba la habilidad estratégica del jugador. Además, el tiempo de espera se convertirá en una variable crucial, si bien la implementación de edificios aceleradores y la aplicación de impulsos dentro de la ciudad pueden desempeñar un papel crucial para optimizar y agilizar estos procesos.
| Nivel | Coste de construccion | Coste de mejora | Coste de reparacion| 
| :--: | :--: | :--: | :--: |
|  1 | 2500 algas verdes | x | 1000 algas |
|  2 | x | 500 hierro  | 1000 algas |
|  3 | x | 750 hierro | 1000 algas |

|Edificio| Nivel requerido del centro| Nivel mejora | Coste| Tiempo |Justificacion|
| :--: | :--: | :--: | :--: | :--: | :--: |
|Granja de algas| 1 | 1->2| 2500 algas verdes | 3 min | La mejora incial de la granja tarda 5 minutos debido a que sube poco la productividad.|
|Granja de algas| 2 | 2->3| 3500 algas verdes | 10 min | Hay una mejora significativa en la productividad, por lo tanto se contraresta con el tiempo de investigacion.|
|Generador de electricidad| 1 | 1->2 | 1000 algas verdes | 10 min| Es un edificio que acelera el progreso del juego por lo tanto se obliga al jugador esperar.|
|Generador de electricidad| 2 | 2->3 | 3000 algas verdes | 10 min| Es un edificio que acelera el progreso del juego por lo tanto se obliga al jugador esperar.|
|Paratridente| 1 | 1->2 | 1000 algas verdes | 10 min| Es un edificio que previene las destrucciones de poseidon, por lo tanto las mejoras deben llevar a cabo mas tiempo para que el jugador pueda experimentar destrucciones.|
|Paratridente| 2 | 2->3 | 2000 algas verdes | 10 min|Es un edificio que previene las destrucciones de poseidon, por lo tanto las mejoras deben llevar a cabo mas tiempo para que el jugador pueda experimentar destrucciones.|
|Mejora de buceo| 1 | 1->2 | 1000 algas verdes | 5 min| Se tarda poco debido a que hay mas coste de algas dentro del propio edificio.|
|Mejora de buceo| 2 | 2->3 | 2000 algas verdes | 5 min| Se tarda poco debido a que hay mas coste de algas dentro del propio edificio.|





#### **1.5.7 - Museo**
Al hacer clic el museo el jugador podrá ver todos los recursos que ha descubierto, además se darán recompensas por desbloquear un número de recursos de las tres categorías:
  - Materiales del mar: como algas y corales.
  - Materiales de construcción: recogidos en naufragios durante las expecidiones, como madera y hierro.
  - Peces: como anguilas eléctricas.

Al mejorar: no tiene mejora.

### 1.6 Animales 
#### Animales Depredadores
|  Nombre   |  Damage  | Descripciones |
| --------- | -------- | ------------- |
| Tiburon   |    2     | Criatura agrevisa distribuida por todas las profundidades, varian el tamaño según la profundidad. Atacan al jugador en caso de desecubrirle |
| Pez Vampiro | 1      | Su principal característica son los colmillos que sobresalen de su mandíbula inferior.Se ocultan en las aguas profundas y lanzan ataques a sus presas tipo emboscada.|
| Pez Leon |     1     | Habita en aguas cálidas y es muy venenoso. Es carnívoro y se alimenta de crustáceos, camarones y otros peces.|
| Medusa | 0.5/segundos| Criatura no ofensiva pero toxica, por lo que si se acerca demasiado puede ser intoxicado por su piel y perder vida de manera continua.
| Rapes abisales | 0.5 | Critatura de tamaño medio, ofensiva pero ataque poco efectivo, distribuido en niveles medio profundo.|

#### Animales Inocentes
|  Nombre   |  Size  | Descripciones |
| --------- | -------- | ------------- |
| molly velífera | Small | Pez de tamaño pequeño que se descubre en profundidad baja, recurso necesario para mantener el generador de electricidad. Distribuido en zonas iniciales.|
| Peces cebra | Small | Pez de tamaño pequeño que se descubre en profundidad baja, recurso necesario para mantener el generador de electricidad. Distribuido en zonas iniciales e medios.|
| Pez neon | Medium | Recurso distribuido en profundidad medio de cada mapa, posee una gran cantidad de carne por lo que puede ser consumido por varias anguilas, de forma que es mas rentable para el generador.|
| Pez dorado | Small | Critatura pacifica que escapa si descubre al jugador, se puede coleecionar para el uso del generador, o para el libro de museo.|
| Boga | Medium| Criatura de tamaño medio no ofensiva, sirve para dar más energía al generador y para el libro de colecciones.|
| Lubina | Medium| Criatura mediano que se puede consumir por el jugador dentro de la expedición para coger energía(Alargar el tiempo de expedicion) .|
| Dorada | Medium| Criatura media que propone un efecto de velocidad durante un tiempo determinado.|






## **2 - CONTROLES**

Los controles del juego están basados en los clásicos controles de los juegos de estrategia. Principalmente, son usados para mover y controlar la cámara, permitiendo así al jugador una mayor inmersión dentro del juego. 

Los controles se dividen en dos subtipos, dependiendo si se usa una pantalla táctil (Dispositivos móviles), o si se usa el teclado y ratón. 

### **2.1 - Teclado y ratón**: 

#### **2.1.1 - Movimiento de la cámara**: 

  - **W, flecha arriba**: Movimiento vertical hacia arriba. 
  - **S, flecha abajo**: Movimiento vertical hacia abajo. 
  - **A, flecha izquierda**: Movimiento horizontal hacia la izquierda. 
  - **D, flecha derecha**: Movimiento horizontal hacia la derecha. 
  - **Click izquierdo**: Al hacer click y arrastrar el ratón por la pantalla, la cámara se moverá al sentido contrario del arrastrado.

#### **2.1.2 - Rotación de la cámara**: 

  - **E**: Rotación en sentido horario. 
  - **Q**: Rotación en sentido antihorario. 

#### **2.1.3 - Zoom**: 

  - **Rueda del ratón**: Dependiendo de la dirección, la cámara se acercará o se alejará.


🎮🎮  
**Morado: Movimiento de camara**  
**Rojo: Rotacion de camara**  
**Amarillo: Zoom de camara**  
![Keyboard layout](https://github.com/GluGluGames/Ahogado-en-Impuestos/blob/main/Game%20Design%20Documents/Mapeo%20de%20controles.jpg)

### 2.2 - Pantalla táctil: 

  - **Movimiento de la cámara**: El jugador mantendrá su dedo en la pantalla y arrastrará. La cámara se moverá al sentido contrario del arrastre. 
  - **Rotación de la cámara**: El jugador mantendrá dos dedos y arrastrará de manera horizontal. La cámara se moverá en el sentido de arrastre. 
  - **Zoom**: El jugador mantendrá dos dedos en la pantalla y juntará o separará estos. El zoom se incrementará o disminuirá en función de si se acercan o alejan, respectivamente.

**Zoom:**   
 ![Zoom](https://github.com/GluGluGames/Ahogado-en-Impuestos/blob/main/Game%20Design%20Documents/zoom-fingers.gif)

## **3 - ARTE 2D** 

### **3.1 - Moodboard – Laura** 

### **3.2 - Concepts – Marco/Laura**

#### **3.2.1 - Ayuntamiento**

![Ayuntamiento](https://github.com/GluGluGames/Ahogado-en-Impuestos/assets/112829139/e85bb4d2-01a7-4272-92c4-bc3a80777b2f)

#### **3.2.2 - Granja de algas (nivel 1)**

![Granja de algas N1](https://github.com/GluGluGames/Ahogado-en-Impuestos/assets/112829139/66bcc66d-caf3-4abf-a128-bc42f478474d)

#### **3.2.3 - Granja de algas (nivel 2)**

![Granja de algas N2](https://github.com/GluGluGames/Ahogado-en-Impuestos/assets/112829139/321e8be9-97e8-4353-8852-199f433df3a9)

#### **3.2.4 - Granja de algas (nivel 3)**

![Granja de algas N3](https://github.com/GluGluGames/Ahogado-en-Impuestos/assets/112829139/ef8f499e-f761-4f33-bfec-da8d80f02f0c)

#### **3.2.5 - Laboratorio**
    
![image](https://github.com/GluGluGames/Ahogado-en-Impuestos/assets/81293482/e18ae0cf-61f9-48d1-9ecb-844aa4c26f6d)

#### **3.2.6 - Museo**

![Museo](https://github.com/GluGluGames/Ahogado-en-Impuestos/assets/112829139/21d670df-64b4-4a0c-b701-cd8d8978d6ce)

#### **3.2.7 - Tienda**
**Caracteristica:**
- Pilares griegos
- Ventanillas colocados por los lados par vender
- Decoracion de una tienda, cartel, pizarra, productos, luz especial.
**Justificacion:**
  En el diseño de tiendas dentro del juego, hemos optado por incorporar principalmente características de tiendas contemporáneas. Esta elección se basa en la proximidad de estas características al público objetivo, lo que facilita a los jugadores la localización y el 
  acceso rápido a las tiendas cuando sea necesario.
![Tienda](https://github.com/GluGluGames/Ahogado-en-Impuestos/assets/112829139/ead2404d-31f6-4163-a839-bc48436c2f64)

#### **3.2.8 - Generador eléctrico**

![Generador eléctrico](https://github.com/GluGluGames/Ahogado-en-Impuestos/assets/112829139/16921824-70bc-4e13-9dbd-2a25c31585c9)

#### **3.2.9 - Alamacén de peces (nivel 1)**

![image](https://github.com/GluGluGames/Ahogado-en-Impuestos/assets/81293482/6c6302cd-5aab-4e76-9176-b5fcd0399685)

#### **3.2.10 - Alamacén de peces (nivel 2)**

![image](https://github.com/GluGluGames/Ahogado-en-Impuestos/assets/81293482/5c8e9690-8699-4400-8fd2-bb0d4be62d4a)

#### **3.2.11 - Poseidón**

![image](https://github.com/GluGluGames/Ahogado-en-Impuestos/assets/81293482/384f2ae0-e269-45a3-b106-1085326a5022)

### **3.3 - Personajes**

#### **3.3.1 - Estilo visual personajes**

- Los personajes dentro de los diálogos tendrán una estética diferente a la del juego, al estilo Hades. Cuando los personajes hablen tendrán una imagen suya a modo de splash art. Se ha tomado esta deicisón porque creemos que favorecerá a la narrativa y a la satisfacción visual del jugador, es decir, que será atractiva para los usuarios.

## **4 - DISEÑO 3D – Adri C.**

## **5 - GAME DESIGN  – Adri/Luming** 
**- Flujo de juego**
#### **Introducción y Configuración del Juego:**
El jugador inicia la partida en una grid de hexágonos, específicamente en la casilla del antiguo ayuntamiento de la ciudad perdida de Atlantis. La tarea del jugador es reconstruir la gloria pasada de esta ciudad sumergida.


#### **Tutoriales:**
- El jugador se somete a un tutorial inicial que le enseña cómo funciona la gestión de recursos dentro de la ciudad. Aprende cómo construir edificios, qué recursos existen y cuáles son sus funcionalidades.
Durante el tutorial, se notifica al jugador que Poseidón recaudará una cantidad de algas como impuesto. Si no se cumple este impuesto, Poseidón procederá a destruir aleatoriamente edificios en la ciudad.
Expediciones:

- Tras completar los tutoriales iniciales, el jugador tiene la opción de embarcarse en expediciones. Cada expedición lleva al jugador a un nuevo mapa.
Tutorial de Expediciones:

- El jugador recibe un segundo tutorial que lo familiariza con las mecánicas de expedición. Aprende sobre los iconos de cada casilla en el mapa, la presencia de depredadores, peces cazables y la existencia de tormellinos, que permiten salir del mapa sin perder recursos. Se destaca la importancia del tiempo como un factor que requiere que el jugador tome decisiones.

![Tutorial](https://github.com/GluGluGames/Ahogado-en-Impuestos/blob/main/Game%20Design%20Documents/DiagramaTutorial.png)

#### **Progresión del Juego:**
La obtención de recursos y nuevas mecánicas depende del número de expediciones y los objetos clave encontrados.
Todo el progreso de juego se tomara en un mismo mapa, y apartir de la septima se podra explorar otras zonas si se haya descubierto el mapa.

- Primera Expedición: El jugador puede encontrar perlas, recursos versátiles pero difíciles de conseguir. También puede cazar peces para mantenerlos en una pecera para uso en el museo o en el generador de electricidad.

- Tercera Expedición: Si el jugador encuentra un mapa, se desbloquea la mecánica de encontrar mapas. Posteriormente, Poseidón exige un impuesto imposible de pagar, lo que resulta en la destrucción de un edificio. El jugador descubre la madera, un recurso necesario para reparar edificios.Tras la reparacion, se le ofrece al jugador una tablilla de paratridente, asi para evitar la ira de poseidon.

- Quinta Expedición: El jugador puede encontrar una tablilla y un suministro de hierro. Esto demuestra la posibilidad de mejorar los edificios a través de la investigación. Los edificios se mejoran utilizando el hierro.

- Sexta Expedición: El jugador encuentra un casco dorado en el mapa, lo que desbloquea mejoras relacionadas con el buceo.

- Séptima Expedición: Si el jugador encuentra la tablilla del generador de electricidad y se encuentra con anguilas, se desbloquea la construcción de generadores de electricidad. Las anguilas se pueden guardar en la pecera para su uso posterior.
![Progresion de expediciones](https://github.com/GluGluGames/Ahogado-en-Impuestos/blob/main/Game%20Design%20Documents/Flujo%20de%20expedicion.png)

#### **Colección y Museo:**
- Los jugadores pueden descubrir y cazar diferentes tipos de peces y recolectar diversas plantas marinas para completar el libro de museo, que debe ser reconstruido.

### **5.1 - Diseño de nivel**
Los niveles en este juego representarán las diferentes zonas de expedición y serán diseñados a mano con distribuciones específicas de niveles de profundidad. Estas distribuciones se han diseñado con el propósito de plantear desafíos y dificultades para el jugador antes de que este adquiera objetos que lo hagan inmune a ciertos peligros.

Sin embargo, es importante destacar que los recursos que no funcionen como desencadenantes clave para avanzar en el juego serán generados aleatoriamente por la zona. Esto permite que el jugador encuentre variedad en cada expedición y promueve la exploración activa para reunir recursos útiles en su búsqueda para reconstruir la ciudad perdida de Atlantis.

#### **Curva de aprendizaje:**
Explicacion:
La única manera de que el jugador adquiera nuevas habilidades es a través de expediciones. En otras palabras, cuantas más expediciones complete, más habilidades podrá desarrollar. El juego presenta un grid de hexágonos, cada uno con diferentes propiedades: algunos albergan depredadores, otros contienen recursos y otros son el hogar de criaturas únicas. Por lo tanto, es responsabilidad del jugador explorar cada zona de manera explícita.

El contenido de cada casilla varía en cada expedición, lo que significa que si el jugador ha adquirido suficiente experiencia de expediciones fallidas, estará mejor preparado para enfrentar los desafíos que se presenten en el mapa, como los depredadores. El jugador debe considerar cuidadosamente su ruta para evitar perder tiempo transitando áreas de menos beneficio, a menos que disponga de objetos o elementos que le permitan acceder a esas zonas. En caso contrario, la expedición se verá afectada por una pérdida de tiempo de expedicion.

![Curva de aprendizaje](https://github.com/GluGluGames/Ahogado-en-Impuestos/blob/main/Game%20Design%20Documents/Curva%20de%20aprendizaje.png)
#### **Flujo de recaudacion:**
Explicacion:
Esta mecánica se ha diseñado para evitar que los jugadores aceleren demasiado el progreso del juego, centrándose en la productividad de las algas que el jugador genera por segundo. Esta elección se ha realizado con el objetivo de garantizar que, a medida que los jugadores avanzan en el juego, no alcancen un nivel de productividad excesivamente alto que pudiera llevar al desinterés y, en última instancia, al abandono del juego.

Para prevenir este escenario, se ha implementado un porcentaje de recaudacion que depende de la productividad de las algas por segundo. De esta manera, se permite una variedad de estrategias para abordar este ajuste de probabilidad. Por ejemplo, los jugadores pueden optar por sacrificar edificios para mantener su productividad, dado que el costo de reparación no es elevado, o incluso buscar un equilibrio entre ganar mucho y gastar mucho. Esta mecánica ofrece a los jugadores una serie de decisiones estratégicas que influyen en su progreso en el juego.
![Intensidad de recaudacion ](https://github.com/GluGluGames/Ahogado-en-Impuestos/blob/main/Game%20Design%20Documents/Intensidad%20de%20recaudacion.png)


## **6 - DIAGRAMA DE FLUJO**

![Glu_Glu_clases](https://github.com/GluGluGames/Ahogado-en-Impuestos/assets/112829139/6aed32c4-675d-47bf-9173-935b61adb73f)

- **MENÚ PRINCIPAL**: el menú principal servirá de pantalla de presentación del juego. Desde aquí se podrá acceder a los ajustes, a los créditos y a la pantalla de juego.
  
- **CRÉDITOS**: en esta pantalla se podrá ver todas las personas involucradas en el juego y sus roles.
  
- **AJUSTES**: se podrán cambiar cosas del juego como el sonido. Desde aquí se puede volver a la pantalla desde donde has accedido, y ver los créditos.
  
- **PANTALLA DE JUEGO**: pantalla principal del juego, donde se gestionará la ciudad y los recursos. Puedes acceder tanto a los ajustes como a la expedición.
  
- **EXPEDICIÓN**: minijuego para recolectar recursos. También puedes acceder a los ajustes, y a la pantalla de derrota si fallas el minijuego.
  
- **DERROTA**: si fallas en la expedición accedes a la pantalla de derrota. Desde aquí puedes volver a la pantalla de juego. 

## **7 - INTERFACES – Laura** 

### **7.1 - MENÚ PRINCIPAL**

![Interfaz - Menú principal](https://github.com/GluGluGames/Ahogado-en-Impuestos/assets/112829139/8b0d2886-a2f7-400f-8fec-e18cc7c84f21)

### **7.2 - CRÉDITOS**

![Interfaz - Créditos](https://github.com/GluGluGames/Ahogado-en-Impuestos/assets/112829139/1aa09aad-d0fd-41e4-aaa2-fbcc238d050f)

### **7.3 - AJUSTES**

![Interfaz - Ajustes (cerrado)](https://github.com/GluGluGames/Ahogado-en-Impuestos/assets/112829139/26dc9f6c-f873-4580-b351-dd8baa0c7c3a)

![Interfaz - Ajustes](https://github.com/GluGluGames/Ahogado-en-Impuestos/assets/112829139/dcf4bed8-23a3-47d7-a736-a85da7ad6bc2)

### **7.4 - PANTALLA DE JUEGO**
#### **7.4.1 - EDICIÓN DE EDIFICIOS**

![Interfaz - Edición de edificios](https://github.com/GluGluGames/Ahogado-en-Impuestos/assets/112829139/975fe570-5a37-4071-b840-9faf713e04c0)

#### **7.4.2 - DIALOGOS**

![Interfaz - Diálogos](https://github.com/GluGluGames/Ahogado-en-Impuestos/assets/112829139/a3bd4aaf-fd3f-4d66-aaf3-511728958735)

#### **7.4.3 - TIENDA DE EDIFICIOS**

![Interfaz - Tienda de edificios](https://github.com/GluGluGames/Ahogado-en-Impuestos/assets/112829139/79f5c2b7-cd87-419c-a716-8e22c429ddc5)

#### **7.4.4 - MUSEO**

![Interfaz - Museo](https://github.com/GluGluGames/Ahogado-en-Impuestos/assets/112829139/24477786-f616-44aa-9dd9-9ea358bbb74e)

#### **7.4.5 - MEJORAS DE BUCEO**

![Interfaz - Mejoras de buceo](https://github.com/GluGluGames/Ahogado-en-Impuestos/assets/112829139/0ccadc28-d681-43bf-98bf-116e64d41014)

#### **7.4.6 - TIENDA DE RECURSOS**

![Interfaz - Tienda](https://github.com/GluGluGames/Ahogado-en-Impuestos/assets/112829139/f3a10538-0711-4210-bdf6-cba0a10cb172)

#### **7.4.7 - EXPEDICIÓN**
#### **7.4.8 - DERROTA**

## **8 - NARRATIVA** 

**Sinopsis:** Hace siglos, fui un habitante de la legendaria ciudad perdida de Atlantis, un lugar misterioso y enigmático que se ocultaba en las profundidades del océano. Sin embargo, mi existencia en ese antiguo reino se mantuvo en un sueño profundo, sepultado bajo las corrientes marinas, hasta que un día, el mismísimo Poseidón, el dios del mar, me despertó de mi largo letargo.

Poseidón me recordó el verdadero propósito de Atlantis, una ciudad concebida como una fábrica de algas. El océano, con el paso de los siglos, se había vuelto cada vez más turbio y tóxico, poniendo en peligro su delicado equilibrio. Poseidón, en su sabiduría, comprendió que era esencial purificar sus aguas para proteger su reino acuático y la vida que lo habitaba.

Con una tarea importante encomendada por el dios del mar, mi misión era clara: reunir una cantidad creciente de algas cada semana. La responsabilidad aumentaría con el tiempo a medida que Atlantis se desarrollara y prosperara. Aunque en el pasado habíamos sido numerosos habitantes, ahora me encontraba solo, es por ello que debía ayudarme de la flora y fauna y valerme de valor y emprender aventuras para cumplir mi propósito.

Las anguilas, criaturas resplandecientes que habían evolucionado para producir electricidad, eran nuestros aliados en esta tarea monumental. Sus destellos luminosos iluminaban los oscuros rincones de la ciudad y proporcionaban la energía necesaria para mantener Atlantis en funcionamiento.

Mi jornada comenzó con la exploración de los vastos jardines de algas que rodeaban la ciudad. Cada semana, debía recolectar más y más algas para satisfacer las crecientes demandas de Poseidón. Estas algas eran el pilar de nuestra civilización submarina, proporcionando alimento y, lo más importante, un sistema de purificación de aguas naturales que mantenía vivas las aguas cristalinas que Poseidón amaba.

A medida que pasarían los años, Atlantis resurgiría de su letargo, con nuevas estructuras y tecnologías olvidadas que una vez más se pondrían en funcionamiento. Nuestra misión era clara: cuidar de las aguas que tanto amaba Poseidón y, al hacerlo, mantener viva la leyenda de la ciudad perdida de Atlantis.

**Mundo:** En el mundo de Atlantis, abundan diversas formas de vida marina, algunas únicas y adaptadas a las profundidades del océano. Las criaturas más importantes son:

Los leviatanes dorados: enormes criaturas acuáticas de piel escamosa que Poseidón ha designado como protectores de las fronteras de Atlantis. Estos seres majestuosos nadan a lo largo de los límites del reino, manteniendo a raya a los depredadores y evitando invitados no deseados.

Las medusas cantaoras: delicadas y hermosas medusas bioluminiscentes que llenan los océanos con su melodioso canto. Estas criaturas, a pesar de su belleza, pueden ser peligrosas para aquellos que se aventuran demasiado cerca sin la protección adecuada.

Los delfines de esmeralda: delfines especialmente inteligentes y dotados de una habilidad excepcional para la navegación. Son conocidos por guiar a los viajeros perdidos de regreso a Atlantis y por su antiguo vínculo con los habitantes de la ciudad.

Los tiburones sombríos: feroces depredadores marinos, ágiles y letales, que acechan en las profundidades en busca de presas desprevenidas. Son conocidos por su destreza y astucia en la caza, representando una amenaza constante para aquellos que se aventuran fuera de los límites de la ciudad.

Los peces vampiro: criaturas oscuras y siniestras que se alimentan de la energía vital de otras formas de vida marina. Estos peces poseen una capacidad innata para el sigilo.

Los peces león: depredadores agresivos y territoriales, con una melena de espinas venenosas que los hace formidables en la lucha. Los peces león protegen sus territorios ferozmente, las leyendas cuentan que protegen objetos de gran valor.

Los rapes abisales: depredadores de aguas profundas que tiene la capacidad de generar una luz bioluminiscente para atraer a presas más pequeñas hacia él. Aunque no es particularmente hábil en la lucha directa, su capacidad para atraer a otros depredadores hacia su ubicación lo convierte en una amenaza indirecta para aquellos que se aventuran más allá de los límites seguros de la ciudad.

Los depredadores están bajo el control de Poseidón, además de mantener el equilibrio en el océano, también sirven como ejecutores de su voluntad en el mundo submarino. Poseidón los ha dotado de inteligencia y fuerza sobrenatural para asegurarse de que su reino permanezca intacto y a salvo.

La desaparición de Atlantis fue el resultado de un cataclismo natural, un gran terremoto submarino que desencadenó una serie de tsunamis devastadores. Aunque los habitantes de Atlantis habían desarrollado una tecnología avanzada para protegerse de muchos peligros, esta catástrofe resultó ser demasiado. A pesar de sus esfuerzos por contener y controlar los efectos, la ciudad no pudo resistir al colapso masivo de su estructura principal, sumergiéndola en lo más profundo de las aguas, donde permaneció oculta durante siglos, hasta que Poseidón decidió despertarla una vez más.

### **8.1 - Personajes – Adri C.**

## **9 - MÚSICA Y SONIDO – David** 

## **10 - PENSAMIENTO COMPUTACIONAL – Todos**
La *destreza principal* que se entrena será la **evaluación**, debido a que en nuestro juego habrá que gestionar los recursos de manera continua. Es importante la planificación tanto para la construccion de la ciudad como para la expedición, detección de errores a través de analizar la velocidad de morir en la expedición, los números de edificios que destruya el jugador.

Las mecánicas cruciales de la construcción de la ciudad son:
- Los *costes* de los edificios a la hora de construirlos
- **Planificación de recursos** para mejorar o construir edificios nuevos
- **Invertir recursos** en investigación para desbloquear nuevo contenido.
- La construcción de ciertos edifcios cercanos para generar **efectos especiales**.
- Evitar que tanto el jugador **borre demasiados** edificios por mala planificacion como que poseidon los destruya.

Las mecánicas cruciales para la expedición son:

- Tener en cuenta la **buena gestión de recursos** que posee, priorizando los objetivos. Debido a que no se permite la estancia permanente dentro de la expedición.
- Registrar si el **pathing** para obtener los recursos es **óptimo**. El jugador tiene varios caminos para alcanzar el mismo objetivo.
- Evitar **morir rápido**, se otorgará una herramienta para poder observar cómo se finaliza la expedición, lo mucho que se muere, el tiempo que se tarda en morir o en completar el nivel.

## **11 - MODELO DE NEGOCIO** 
- **B2P (Buy to Play)**: El usuario deberá pagar cierta cantidad de dinero para poder utilizar el producto.
  
### **11.1 - MAPA DE EMPATÍA DEL USUARIO:**

#### **11.1.1 - ENFOCADO EN NIÑOS**

  ![Mapa de empatía - niños](https://github.com/GluGluGames/Ahogado-en-Impuestos/assets/112829139/2c8842c8-9df5-4b82-b25c-fd792a60b1dc)

#### **11.1.2 - ENFOCADO A GENTE ENTRE 20 y 40 AÑOS**

  ![MapaDeEmpatiaAdultos](https://github.com/GluGluGames/Ahogado-en-Impuestos/assets/112829139/4fa62dcb-7776-4e1e-8f05-a0987c05aa2d)

### **11.2 - CAJA DE HERRAMIENTAS**
  
  ![Caja de herramientas](https://github.com/GluGluGames/Ahogado-en-Impuestos/assets/112829139/afddf48e-8e3c-4307-b3d7-fa677d3fe607)

La caja de herramientas determina las relaciones que existen entre unos determinados bloques (en este caso, las filas y las columnas) entre unos y otros. Siguiendo la tabla, siendo los bloques que hay en cada una de las filas los que dan a los bloques de cada una de las columnas, las relaciones son las siguientes:

#### **11.2.1 - PROPIA EMPRESA**
- **Otras empresas**: Mayoritariamente los anunciantes, mediante los anuncios que introduzcan en los juegos, se les daría tanto expocisión como visibilidad de sus productos.
- **Proovedores**: Principalmente la plataforma Steam, que es donde se publicarían los juegos que sean creados. Al tener unas ciertas tasas de entrada, así como unos impuestos para los productos que están publicados en su plataforma, se les daría principalmente dinero y los algunos de los derechos del producto.
- **Consumidor final**: Siendo estos aquellas personas que jueguen al juego. A estos se les proporcionaría el producto final, es decir, el juego acabado, así como información sobre este, para que estén al tanto de lo que están adquiriendo.
- **Gobierno**: La empresa, en busca de ayudas, proporciona al gobierno el juego, además como su servicio a la hora de llevarlo a centros educativos.
- **Organizaciones sin ánimo de lucro**: Principalmente a los colegios, se les proporcionaría el producto para que lo utilicen, así como el servicio de atención personal por si ocurre algún tipo de incidencia.
#### **11.2.2 - OTRAS EMPRESAS**
- **Propia empresa**: Los anunciantes proporcionarían una cantidad reducida de dinero que pagarían para que se introdujera su producto como publicidad al juego.
#### **11.2.3 - PROVEEDORES**
- **Propia empresa**: Proveen el servicio de publicar el juego en su plataforma, en este caso sería Steam publicando el juego para que el público pueda comprarlo.
- **Consumidor final**: Proporcionan una manera sencilla de hacerles llegar el producto final.
#### **11.2.4 - CONSUMIDOR FINAL**
- **Propia empresa**: Pagan el precio del producto, proporcionando a la empresa una fuente de ingresos. Por otro lado, si el producto se vuelve popular, estos proporcionan visibilidad gracias a las críticas o publicaciones que puedan hacer sobre el juego, así como el intercambio boca a boca. Por último, gracias a las métricas que se incluyen en el juego, el consumidor proporciona información.
- **Otras empresas**: Los anunciantes recibirían dinero de los consumidores si es que a estos les llega a interesar su producto y deciden comprarlo o consumirlo.
- **Proveedores**: Una parte del dinero que se ha usado para comprar el juego se la quedará el proveedor.
#### **11.2.5 - GOBIERNO**
- **Propia empresa**: Proporciona ayudas financieras para el desarrollo del producto, así como exposición en sus diferentes páginas dedicadas al videojuego.
- **Consumidor final**: Los consumidores, principalmente los niños, están amparados por el gobierno, que les proporciona unos derechos y la posibilidad de ganar experiencia gracias al videojuego de la empresa.
- **Organización sin ánimo de lucro**: El gobierno ayudará a los centros educativos dándoles dinero y contribuyendo a que estas sean visibles.
#### **11.2.6 - ORGANIZACIÓN SIN ÁNIMO DE LUCRO**
- **Propia empresa**: Los centros educativos que decidan utilizar el producto deberán pagar cierto precio para ello.
- **Consumidor final**: Al distribuir el producto entre los alumnos, este les permite ganar experienca en la inteligencia computacional.
- **Gobierno**: Proporcionan métricas al gobierno.

### **11.3 - CANVAS:**

  ![Canvas](https://github.com/GluGluGames/Ahogado-en-Impuestos/assets/112829139/f3f7f811-f339-4568-8246-7fa616475d7e)

#### **11.3.1 - Segmento de Mercado**

El juego adopta un enfoque versátil diseñado para atraer a dos segmentos de clientes:

- **Niños de 6 a 12 años**: el enfoque principal se dirige hacia este grupo de edad, incorporando un componente educativo sólido que fomenta el desarrollo del pensamiento computacional y la adquisición de conocimientos de manera lúdica. Este segmento puede acceder a nuestro juego a través de dos canales:
  - **Padres y tutores**: son los responsables de adquirir el juego para sus hijos en un ámbito privado. Además, se les brinda la oportunidad de compartir la experiencia de juego con  sus hijos, generando así una experiencia de juego enriquecida en el ámbito familiar.
  - **Instituciones educativas y gobiernos**: se buscarán oportunidades de colaboración con escuelas y entidades gubernamentales en lo que respecta a los aspectos educativos del juego.  Esto podría incluir la implementación de programas educativos o iniciativas de apoyo a la enseñanza del pensamiento computacional.
- **Adultos de 20 a 40 años, que incluyen**:
	- **Adultos que se adentran en el género**: se busca atraer a adultos que se están introduciendo en el mundo de los juegos de gestión de recursos y construcción de ciudades. Se les  ofrece una curva de aprendizaje gradual y casual, con una experiencia de juego accesible para que puedan familiarizarse con este género de manera efectiva.
	- **Jugadores experimentados en el género**: el juego también se orienta a aquellos jugadores que ya dominan los juegos de gestión de recursos y construcción de ciudades. Se les plantean desafíos y mecánicas avanzadas para mantener su interés y proporcionar una experiencia profundamente enriquecedora.
   
Este enfoque diversificado en nuestros segmentos de mercado nos capacita para llegar a un público amplio y cumplir de manera efectiva con las necesidades y expectativas de cada grupo. Esto, a su vez, asegura el éxito de nuestro juego de gestión de recursos con elementos educativos y de entretenimiento.

#### **11.3.2 - Relación con el cliente**

La empresa se destaca por su compromiso de establecer una relación cercana y completamente transparente con sus clientes, donde se otorga un valor inmenso a sus opiniones y observaciones. El objetivo principal es brindar una experiencia de usuario excepcional. A continuación, se detallan los pilares fundamentales de la interacción con los clientes:

- **Transparencia y Escucha Activa**: la comunicación abierta y transparente es abrazada con fervor hacia los clientes. Se presta atención minuciosa a todas las voces y retroalimentaciones compartidas a través de plataformas de redes sociales como Twitter e Instagram. Cada comentario y sugerencia se considera de un valor incalculable, ya que contribuye en gran medida a la mejora continua de los productos.

- **Comunicación Directa**: la empresa se encuentra disponible en diversas plataformas para facilitar que los clientes se conecten directamente. Ya sea a través de correo electrónico, redes sociales o foros de discusión, se hace un esfuerzo constante por mantenerse accesibles y receptivos a las preguntas, inquietudes y sugerencias de la distinguida clientela.

La empresa se enorgullece de mantener una relación dinámica y en constante evolución con su apreciada clientela. El objetivo es crear un ambiente en el cual los comentarios y las conexiones desempeñen un papel fundamental en el desarrollo de futuros juegos y en la mejora de las experiencias de juego. La comunicación abierta y la flexibilidad son los pilares fundamentales de la relación con los valiosos clientes.
  
#### **11.3.3 - Canales de distribución**

La estrategia de canales de la empresa está meticulosamente diseñada para maximizar su presencia y accesibilidad en el mercado de videojuegos, al mismo tiempo que establece colaboraciones significativas en el ámbito educativo.

- **Plataformas de Venta de Videojuegos**:  se hará uso de plataformas de renombre, como Steam, Epic Games Store, Google Play Store, Apple App Store, itch.io, Humble Bundle y otras relevantes, para la distribución y comercialización de los juegos.
- **Instituciones educativas**: para forjar relaciones sólidas con instituciones educativas, se implementarán las siguientes estrategias:
  - Identificación y contacto directo con escuelas interesadas en incorporar nuestros juegos en programas educativos.
  - Participación activa en conferencias y eventos educativos con el fin de establecer relaciones con educadores.
  - La creación de material promocional específico para el sector educativo en nuestro sitio web.

- **Canales de comunicación y promoción**:
  - **Sitio Web Oficial**: el sitio web oficial servirá como el epicentro de información detallada acerca de los juegos, noticias, actualizaciones y soporte técnico.
	- **Redes sociales**: mantendremos una presencia activa en diversas redes sociales, incluyendo Facebook, Twitter e Instagram, con el propósito de informar y promocionar nuestros productos, así como interactuar de manera cercana con nuestra comunidad de jugadores.
	- **Comunidad en Discord**: se establecerá y gestionará una comunidad en Discord donde los jugadores podrán interactuar, debatir sobre nuestros juegos y ofrecer sus valiosos comentarios.

La estrategia de canales se enfoca en alcanzar de manera eficiente a clientes y colaboradores clave, promoviendo nuestros juegos de forma efectiva y sentando las bases sólidas para el éxito continuo de la empresa de videojuegos.

#### **11.3.4 - Propuestas de valor**

La empresa se dedica a ofrecer productos destinados a fomentar el aprendizaje y el entretenimiento, dirigidos principalmente a estudiantes de 6 a 12 años, sin olvidar la satisfacción de adolescentes y adultos. La propuesta de valor se sustenta en los siguientes pilares esenciales:

- **Diversión educativa para todas las edades**: la oferta de videojuegos combina a la perfección el entretenimiento y la educación, diseñados para abarcar a estudiantes jóvenes, adolescentes y adultos. La accesibilidad de estos juegos para toda la familia asegura que puedan ser disfrutados por personas de diversas edades sin sacrificar la diversión.

- **Narrativa Inmersiva**: cada título envuelve a los jugadores en una narrativa cautivadora. Los jugadores se ven inmersos en una trama intrigante que les permite comprender el contexto y las situaciones dentro del juego, incrementando así la inmersión y el compromiso.

- **Pasión por la experiencia del jugador**: la empresa se impulsa por la pasión de crear experiencias excepcionales para los jugadores. La meta es ofrecer juegos que emocionen y motiven a los usuarios, proporcionando momentos memorables en cada partida, así como ayudándoles a desarrollar su inteligencia computacional.

La propuesta de valor consiste en crear experiencias de juego inolvidables que equilibren el entretenimiento, el aprendizaje y la diversión para jugadores de todas las edades. Esto se respalda con valores empresariales enfocados en la pasión, la honestidad, el respeto y la colaboración. El compromiso constante con la innovación impulsa a la empresa a evolucionar y superar constantemente las expectativas de sus clientes.

#### **11.3.5 - Actividades clave**

La empresa de videojuegos se dedica a actividades fundamentales que aseguran la calidad de sus productos y la satisfacción de su clientela, manteniendo un equilibrio preciso entre la excelencia del juego y una promoción adecuada.

- **Desarrollo de productos de entretenimiento de calidad**: la creación de videojuegos se posiciona como el núcleo de nuestras operaciones. Se invierten recursos sustanciales en la concepción, diseño y desarrollo de juegos que proporcionan experiencias inmersivas y educativas de primera categoría, que permitan el desarrollo de la inteligencia computacional en aquellos que los consuman.

- **Campañas de marketing estratégicas**: a pesar de mantener nuestras actividades de marketing en un nivel óptimo, se ejecutan campañas estratégicas con el fin de promocionar nuestros juegos de manera eficaz. Se busca un equilibrio para garantizar que la calidad del juego siempre ocupe el lugar central.

Las actividades esenciales se orientan hacia la excelencia en el desarrollo de juegos, la interacción constante con los clientes y una estrategia de marketing equilibrada que respalda nuestra misión de proporcionar entretenimiento de alta calidad y experiencias educativas de gran valor.
  
#### **11.3.6 - Recursos clave**
  
La empresa se respalda en una serie de recursos esenciales que resultan fundamentales tanto en el desarrollo como en la gestión de productos de entretenimiento de primera calidad:

- **Equipo de desarrollo especializado**: El equipo de desarrollo se revela como un activo crítico, integrado por profesionales altamente cualificados en diversas disciplinas, entre las que se incluyen el diseño de juegos, la creación artística en 2D y 3D, la gestión de proyectos y la programación entre otras. La sinergia que se crea entre estos individuos es fundamental para la concepción y creación de experiencias de juego.

- **Herramientas de desarrollo**: Se cuenta con una amplia gama de herramientas de desarrollo, que incluyen aplicaciones especializadas como Unity, Visual Studio, 3ds Max y Blender, que desempeñan un papel fundamental en la creación y optimización de nuestros productos. Estas herramientas se revelan como recursos indispensables en todo el proceso de producción.

- **Recursos financieros**: los recursos financieros ostentan un carácter crucial para financiar el desarrollo de los juegos, ya sea para cubrir los costos operativos, como para llevar a cabo campañas de marketing estratégicas. Esta categoría engloba tanto las inversiones iniciales como una gestión precisa de los ingresos generados por las ventas y otras fuentes financieras.

La confluencia de estos recursos clave nos habilita para reunir el talento, las herramientas y los fondos necesarios para llevar a cabo la producción y promoción de los juegos, que no solo cumplan con las expectativas de nuestra clientela, sino que también se adapten a las cambiantes demandas de nuestro mercado.

#### **11.3.7 - Socios claves**

La estrategia de asociaciones se enfoca en la colaboración estratégica, que permite expandir la presencia y optimizar la eficacia en el desarrollo y promoción de los productos.

- **Asociaciones con Publishers y Distribuidoras**: establecer alianzas sólidas con publishers y distribuidoras reviste una importancia fundamental para aumentar la visibilidad y la distribución de los juegos. Se colaborará activamente con estos socios para alcanzar una audiencia más amplia y aprovechar su pericia en las áreas de comercialización y distribución.

- **Colaboración con Organizaciones Educativas (Públicas y Privadas)**: se buscará establecer relaciones estratégicas con organizaciones educativas, tanto de carácter público como privado, con el fin de cooperar en la incorporación de los juegos en entornos educativos. Esto abarca la firma de acuerdos para integrar los productos en programas escolares y promover soluciones educativas innovadoras.

Estas asociaciones estratégicas desempeñarán un papel fundamental en la expansión del alcance en el mercado, la mejora de la eficiencia en el desarrollo y la promoción, y la entrega de soluciones más efectivas tanto en el ámbito de la distribución como en el educativo.

#### **11.3.8 - Estructuras de coste**

La estructura de costos refleja los gastos críticos asociados con la operación de la empresa de videojuegos, que abarca el desarrollo, la promoción y la distribución de los productos:

- **Costo de empleados**: los sueldos y salarios del equipo de desarrollo, que incluye game designers, artistas 2D y 3D, programadores, gestores de proyectos y otros profesionales especializados, constituyen una parte significativa de los costos. La garantía de un equipo talentoso y comprometido se erige como un elemento esencial en la creación de juegos de alta calidad.

- **Costo de hardware**: se efectúan inversiones en hardware de alto rendimiento y estaciones de trabajo para los equipos de desarrollo. Esto comprende la adquisición de computadoras, servidores y equipos de prueba necesarios para el diseño y prueba de los juegos.

- **Costo de marketing**: la promoción de los juegos a través de campañas de marketing estratégicas implica gastos relacionados, que abarcan la publicidad en línea, relaciones públicas, materiales promocionales y la participación en eventos de la industria. Aunque se mantiene un enfoque equilibrado en esta área, se reconoce como un componente esencial para lograr la visibilidad de los productos.

- **Costo de publishers**: la colaboración con publishers y distribuidoras conlleva costos asociados a acuerdos de distribución y marketing compartido. Estos costos forman parte de nuestra estrategia para alcanzar una audiencia más amplia y aumentar las ventas de los juegos.

La administración eficiente de nuestra estructura de costos garantiza una asignación adecuada de recursos a las actividades clave, manteniendo un equilibrio entre la inversión en desarrollo y la promoción de los juegos.

#### **11.3.9 - Fuentes de ingresos**

  La estrategia de ingresos está meticulosamente diseñada para diversificarse y capitalizar una variedad de oportunidades con el objetivo de generar beneficios, teniendo en cuenta la amplia audiencia objetivo y el enfoque tanto en el aprendizaje como en el entretenimiento.

- **Editorial educativa para alumnos**: se implementará una opción de compra colectiva de licencias educativas de los juegos dirigida a instituciones educativas, lo que permitirá a los alumnos acceder a contenido educativo de alta calidad a un precio reducido por unidad. Esta iniciativa estimulará la adopción de nuestros juegos en entornos escolares y colegios.

- **Subvenciones Educativas**: se llevará a cabo una búsqueda activa de subvenciones y colaboraciones con gobiernos y organizaciones educativas para mejorar y ampliar la experiencia educativa ofrecida por nuestros juegos. Esto posibilitará que los estudiantes y educadores accedan a nuestros productos de forma gratuita o a precios reducidos.

- **Anuncios**: se implantarán anuncios no intrusivos en la página web de la empresa con el fin de financiar su mantenimiento y el futuro desarrollo de más videojuegos.

- **Paga lo que puedas**: se introducirá una opción de "Paga lo que Puedas" que otorgará a los jugadores la libertad de determinar cuánto desean pagar por los juegos. Esto promoverá la accesibilidad y permitirá a los usuarios contribuir según sus capacidades financieras.

La estrategia de ingresos se fundamenta en la diversificación y la adaptabilidad, lo que nos capacita para generar beneficios mientras mantenemos un firme compromiso con la oferta de aprendizaje y entretenimiento accesible para una amplia audiencia.
