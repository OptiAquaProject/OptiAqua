# OptiAqua
Servicio Api para la optimización del riego.

Aplicativo desarrollado en C# que proporciona información del estado hídrico de las parcelas teniendo en cuenta las caraterísticas del suelo, el cultivo, la climatología y los riegos.
La aplicaición mantiene una base de datos propia denominada OptiAqua en SQLServer.
La informácion climática la toma de "http://apisiar.larioja.org/"
La información de las parcelas y riegos la toma de la base de datos de la comunidad de Regantes.

El procedimento para el cálculo del balance hídrico está basado en el método FAO 56.

