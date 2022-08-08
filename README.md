# ARKANOID

## Randomizer

Układ poziomów w każdej sesji jest identyczny w każdej sesji w zależności od seed.

## Save & Load

W celu zapisu i odczytu stanu gry zastosowałem BinaryFormatter. Zdecydowałem się na właśnie ten rodzaj serializacji ze względu na kontrolę jaką oferuje.

Plik zapisywany jest na ścieżce Application.persistentDataPath.

Uważam, że trochę przesadziłem implementując na kolejnych klasach [IPersistantObject](Assets/Natoniewski_Arkanoid/Scripts/GameSave/IPersistantObject.cs), ale zrobiłem to w celu bardzo wygodnego wywoływania na kolejnych obiektach .Save() i .Load(), przekazując jako parametr writer/reader.

[Writer](Assets/Natoniewski_Arkanoid/Scripts/GameSave/GameDataWriter.cs) i [reader](Assets/Natoniewski_Arkanoid/Scripts/GameSave/GameDataReader.cs) to klasy stworzone dla wygody w celu łatwego zapisu innych "ciężkich" obiektów typu Vector2. Umożliwia mi to wywołanie po prostu writer.Write(vector2)

## Pozostałe uwagi

Możliwość kliknięcia ESC w celu zatrzymania gry jest wyłączona, gdy piłka nie ruszyła z pozycji startu.

Tween UI zastosowałem jedynie dla tekstu wyświetlanego po naciśnięciu Continue/Save. Zrobiłem to w bardzo leniwy sposób i w prawdziwym projekcie pewnie potrzebna byłaby osobna klasa oferująca tween elementów UI.

Pobrane przeze mnie asset packi były ładnie zapakowane w jednym pliku. Gdybym jednak miał kilka plików graficznych, należałoby stworzyć sprite atlas.

Uważam, że przesadziłem również z deklaracją tak wielu singletonów, jeżeli chodzi o klasy UI. Rozstrzygnąć można by to deklarując klasę, która miałaby dostęp do każdego z aktualnych singletonów, natomiast [Game.cs](Assets/Natoniewski_Arkanoid/Scripts/PersistantGame/Game.cs) po prostu by z niej korzystała.

Game -> Łącznik -> Klasy UI

Chciałem użyć URP ale z jakiegoś powodu w buildzie pojawiał się error uniemożliwiający gre. Na forum napisali że jest był to znany bug ale nie naprawiono go już 2 lata. Postanowiłem zrezygnować z URP.

Znany bug:

power upy poruszają się w każdym stanie. Rozwiązaniem jest napisanie managera power upów. Ten posiadałby listę powerUpów oraz factory, i podobnie jak [BallManager](Assets/Natoniewski_Arkanoid/Scripts/PersistantGame/BallManager.cs) poruszałby powerupy w metodzie OnPlayUpdate(). Toteż rozwiązuje problem leniwego instancjonowania power upów przez [Brick](Assets/Natoniewski_Arkanoid/Scripts/GameLevel/Bricks/Brick.cs).

# UŻYTE ASSETY

- [Arkanoid asset pack](https://kronbits.itch.io/matriax-free-assets) by Kronbits
- [Pixel Art GUI Elements](https://mounirtohami.itch.io/pixel-art-gui-elements) by Mounir Tohami