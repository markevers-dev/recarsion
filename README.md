# 🚗 ReCarsion

Dit project bevat een applicatie die in staat is om afbeeldingen van auto's te classificeren op basis van verschillende ML-modellen. De applicatie maakt gebruik van ML.NET voor machine learning en stelt gebruikers in staat om modellen te laden, toe te passen en te beheren met behulp van reflection.

## Functies

- **Machine Learning Models**: De applicatie ondersteunt het laden van verschillende ML-modellen voor het classificeren van afbeeldingen. De modellen worden opgeslagen in een aparte map (``Models``) en kunnen op elk moment worden geüpdatet. Door het gebruik van reflection herkent de applicatie automatisch nieuwe modellen zonder dat er handmatige wijzigingen in de code nodig zijn.
- **XAML UI (WPF)**:
  - **Keuze van ML Model**: De applicatie biedt een dropdownmenu waarmee gebruikers het model kunnen kiezen dat ze willen gebruiken voor classificatie. Als er geen model beschikbaar is, wordt de gebruiker op een gebruiksvriendelijke manier geïnformeerd over het ontbreken van een model.
  - **Selectie van Afbeeldingen**: De gebruikersinterface bevat een knop waarmee gebruikers afbeeldingen kunnen selecteren. Nadat de bestanden zijn geselecteerd, sorteert de applicatie de afbeeldingen op basis van hun geclassificeerde categorieën.
 
## Getrained Auto Modellen

De verschillende modellen zijn getraind op twee tot vijf van de volgende auto's:
- Audi Q3
- Hyundai Creta
- Mahindra Scorpio
- Rolls Royce Phantom
- Suzuki Swift

Afhankelijk van het model kunnen de afbeeldingen in verschillende categorieën van deze auto's worden geclassificeerd.
 
## Technische Details

- **MVVM**: De applicatie volgt het MVVM-patroon (Model-View-ViewModel) om een scheiding van verantwoordelijkheden te waarborgen en de code goed testbaar en onderhoudbaar te houden. Dit betekent dat er geen Code-Behind aanwezig is.
- **Reflection**: Het project maakt gebruik van ``System.Reflection`` om dynamisch ML-modellen te laden, de beschikbare types te inspecteren en nieuwe modelinstantie aan te maken. Dit zorgt ervoor dat de applicatie flexibel is in het omgaan met verschillende ML-modellen zonder de code te hoeven aanpassen.
- **Assemblies en Interfaces**: Alle ML-modellen implementeren dezelfde interface, zodat de applicatie consistent kan werken met verschillende modellen.

## Gebruiksaanwijzing

1. Start de applicatie en gebruik de knop om afbeeldingen of mappen te selecteren.

2. Kies het model dat je wilt gebruiken uit de dropdown.

3. De applicatie sorteert de afbeeldingen automatisch op basis van hun classificatie door het geselecteerde model.
