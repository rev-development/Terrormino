# 🧬 ClassDiagramGenerator for Unity

[![Unity 2021.3+](https://img.shields.io/badge/Unity-2021.3%2B-blue.svg?style=flat-square&logo=unity)](https://unity.com/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg?style=flat-square)](https://opensource.org/licenses/MIT)
[![Version](https://img.shields.io/badge/Version-2.3.1-success.svg?style=flat-square)](#)

Un outil puissant et moderne pour Unity permettant de générer des diagrammes de classes **PlantUML** directement depuis vos scripts C#. Visualisez l'architecture de votre projet en quelques clics.

---

## 🚀 Fonctionnalités Clés

- 🔍 **Scan Flexible** : Scannez un dossier spécifique, tout le projet ou glissez-déposez des fichiers/dossiers.
- 🛠️ **Parsing Robuste** : Analyse rapide basée sur Regex (Classes, Interfaces, Structs, Enums).
- 🔗 **Associations Avancées** (Nouveau) : 
  - Visualisez les relations entre classes via les champs, propriétés et paramètres.
  - **Détection des appels de méthodes** : Identifie les interactions directes entre classes dans le corps des méthodes.
  - **Labels détaillés** : Les noms des membres (champs, méthodes appelées) s'affichent directement sur les flèches de liaison.
- 📦 **Support des Namespaces** : Organisation automatique sous forme de packages PlantUML.
- 🌐 **Export Multi-format** : 
  - Fichier `.puml` local.
  - URL partageable via le serveur public PlantUML (encodage Deflate).
- 🎨 **Interface Polie** : Fenêtre Editor intuitive avec barre de recherche et sélection multiple.

---

## 📖 Utilisation

1. **Ouvrir la fenêtre**  
   Allez dans `Tools → 🧬 Diagram Generator`.

2. **Sélectionner les scripts**  
   - Choisissez un dossier racine ou cliquez sur **Whole project**.
   - Utilisez le **Drag & Drop** pour ajouter rapidement des fichiers ou dossiers.
   - Cliquez sur **🔍 Scan** pour lister les scripts, puis affinez votre sélection.

3. **Configurer l'export**  
   - Choisissez entre **File (.puml)** ou **URL**.
   - Activez **Include associations** pour un diagramme relationnel riche.

4. **Générer**  
   - Cliquez sur **🛠️ Generate Diagram**.
   - Utilisez **Reveal file** ou **Copy URL** pour accéder au résultat.

---

## 🛠️ Installation

Copiez le dossier `Assets/ClassDiagramGenerator` dans votre projet Unity.

---

## 📝 Détails du Parsing

- **Visibilité** : Support complet (`+` public, `-` private, `#` protected, `~` internal).
- **Modificateurs** : Détection des classes abstraites, méthodes statiques, etc.
- **Relations** : 
  - Héritage (`<|--`) et Implémentation (`<|..`).
  - Associations avec multiplicité automatique pour les collections (`List<T>`, Tableaux).
  - **Limitation intelligente** : Pour garder les diagrammes lisibles, les labels d'association sont limités aux 5 membres les plus pertinents.

---

## ❓ FAQ

**Q : L'outil modifie-t-il mes scripts ?**  
R : Non, l'outil effectue uniquement une lecture seule de vos fichiers `.cs`.

**Q : Est-ce qu'il supporte les classes génériques ?**  
R : Oui, le parser gère les types génériques et les nettoie pour une compatibilité optimale avec PlantUML.

**Q : Peut-on exporter en PNG/SVG ?**  
R : L'outil génère le code source PlantUML. Vous pouvez le convertir en image via n'importe quel moteur de rendu PlantUML (ex: extension VS Code, site officiel).

---

## 📄 Licence

Distribué sous la licence MIT. Voir `LICENSE` pour plus d'informations.

---

**ClassDiagramGenerator** © 2026 • *Amélioré pour les développeurs exigeants.*