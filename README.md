*FlipFlOS*


Ein benutzerdefiniertes Betriebssystem in C# mit CosmosOS:
In diesem Projekt entwickeln wir ein Betriebssystem mit Hilfe des CosmosOS-Frameworks. CosmosOS ist eine Open-Source-Plattform,
die es ermöglicht, Betriebssysteme in C# zu schreiben. Dieses Projekt bietet eine großartige Gelegenheit, tiefer in die
Welt der Betriebssystementwicklung einzutauchen und C#-basierte Lösungen zu verwenden.

*Über das Projekt*

FlipFlOS ist ein benutzerdefiniertes Betriebssystem, das in C# unter Verwendung des 
CosmosOS-Frameworks entwickelt wurde. Unser Ziel ist es, ein leichtgewichtiges, aber dennoch lehrreiches Betriebssystem zu erstellen, das 
die Grundlagen der Kernelprogrammierung, Speicherverwaltung und Hardwareinteraktionen vermittelt. Zusätzlich wird 
ein Dateiverwaltungs- und Dateispeichersystem entwickelt und alle grundlegenden Funktionalitäten eines kommandozeilenbasierten Programms erstellt.

*Projektstruktur*

•	/Kernel.cs: Enthält den Haupt-Kernel-Code von FlipFlOS.

•	/Command.cs: Definiert die grundlegende Befehlsverarbeitung.

•	/Directory.cs: Verwaltet Verzeichnisse und Dateistrukturen.

•	/Memory.cs: Steuert das Speichermanagement.


*Commands:*

          "time": For getting current runtime,
          "help": For getting help,
          "write": write [index] [byte] Writing data,
          "read": read [index] reading data,
          "cd": cd [directory] changes directory to chosen directory. `cd ..` for parent,
          "gcd": prints current path of directory,
          "ls" prints list of all elements in current directory,
          "mkdir": mkdir [directory] creates subdirectory in current directory,
          "touch": touch [file] creates File,
          "writeFile": writeFile [file] [content] writes Strings into destined File,
          "readFile": readFile [file] prints content of file


*Beitragende*

•	Noah Di Palma

•	Phillip Stephan
