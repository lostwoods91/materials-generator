# materials-generator

guida per importare modello obj esportato da txe:

- importare il package "UTesyMaterials.unitypackage" nel progetto;
- trascinare la cartella contenente i file .obj, .mtl, .utm e la cartella "Resources" da qualche parte sotto la cartella "Assets" di Unity (preferibilmente sotto "Assets/Models/");
- attendere che Unity importi il nuovo asset, dopodichè eliminare l'eventuale cartella "Materials" generata (poichè Unity in importazione genera automaticamente i materiali non riconosciuti);
- creare un GameObject e assegnargli lo script "MaterialsGenerator";
- impostare il parametro "Model Folder Path" dello script con il percorso della cartella contenente il modello importato;
- eseguire il gioco per attivare la creazione automatica dei materiali custom e poi interromperlo: i materiali sono stati creati;
- selezionare il file .obj e, nell'inspector, impostare la ricerca dei materiali al valore "Project-Wide", quindi premere "Apply";
- attendere che i materiali siano correttamente impostati.