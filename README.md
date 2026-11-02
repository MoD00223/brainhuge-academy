# 🧠 BrainHuge Academy

> **Apprenez plus intelligemment. Retenez plus vite. Progressez sans limites.**

BrainHuge Academy est une plateforme d'apprentissage en ligne qui associe à chaque cours **Nova**, un tuteur IA adaptatif. Fini l'étude passive : l'apprentissage écoute, s'adapte et répond.



## Sommaire

- [Fonctionnalités](#fonctionnalités)
- [Stack technique](#stack-technique)
- [Prérequis](#prérequis)
- [Installation et lancement](#installation-et-lancement)
- [Lancement avec Docker](#lancement-avec-docker)
- [Structure du projet](#structure-du-projet)
- [Modèle de données](#modèle-de-données)
- [Configuration](#configuration)
- [Pages principales](#pages-principales)
- [Dépannage](#dépannage)
- [Feuille de route](#feuille-de-route)

---

## Fonctionnalités

| Fonctionnalité | Description |
|---|---|
| 📚 **Catalogue de cours** | Parcours les cours par catégorie, niveau (Débutant, Intermédiaire…) et durée |
| 🧩 **Parcours structurés** | Chaque cours est découpé en modules contenant des leçons et des quiz |
| 🤖 **Tuteur IA Nova** | Un assistant conversationnel qui répond aux questions, donne des exemples et propose des questions de suivi |
| 📈 **Suivi de progression** | Avancement par module, scores de quiz, inscription aux cours |
| 👤 **Comptes utilisateurs** | Inscription, connexion et espace « Mes cours » |

---

## Stack technique

- **Framework** : ASP.NET Core MVC (.NET 8)
- **ORM** : Entity Framework Core 8
- **Base de données** : SQLite (mode WAL)
- **Authentification** : ASP.NET Core Identity
- **IA** : moteur Nova (tuteur conversationnel)
- **Conteneurisation** : Docker

---

## Prérequis

- [SDK .NET 8](https://dotnet.microsoft.com/download/dotnet/8.0)
- (Optionnel) [Docker](https://www.docker.com/) pour le lancement en conteneur

Vérifie ton installation :

```bash
dotnet --version
```

---

## Installation et lancement

```bash
# 1. Cloner le dépôt
git clone <URL_DU_DEPOT>
cd brainhuge-academy

# 2. Lancer l'application
dotnet run
```

Au premier démarrage, l'application :

1. crée automatiquement la base SQLite,
2. applique les migrations Entity Framework,
3. insère les données de démonstration (cours, modules, quiz).

Ouvre ensuite **<http://localhost:5183>** dans ton navigateur.

> 💡 Le port est défini dans `Properties/launchSettings.json`.

Pour recharger automatiquement l'application à chaque modification du code :

```bash
dotnet watch run
```

---

## Lancement avec Docker

```bash
# Construire l'image
docker build -t brainhuge-academy .

# Lancer le conteneur
docker run --rm -p 8080:8080 -e PORT=8080 brainhuge-academy
```

Puis ouvre **<http://localhost:8080>**.

⚠️ La variable d'environnement `PORT` est **obligatoire** : le conteneur démarre l'application avec `--urls http://+:${PORT}`.

⚠️ La base SQLite étant un simple fichier, elle est **perdue à chaque suppression du conteneur**. Pour la conserver, monte un volume persistant :

```bash
docker run --rm -p 8080:8080 -e PORT=8080 \
  -v brainhuge-data:/app/data \
  brainhuge-academy
```

*(adapte le chemin `/app/data` à l'emplacement réel du fichier `.db` configuré dans ton projet)*

---

## Structure du projet

```
brainhuge-academy/
├── Controllers/          # Contrôleurs MVC (Home, Account, Learner, AiTutor…)
├── Models/               # Entités et modèles de vue
├── Views/                # Vues Razor (.cshtml)
├── Data/                 # DbContext et données initiales
├── Migrations/           # Migrations Entity Framework Core
├── Properties/
│   └── launchSettings.json
├── wwwroot/              # Fichiers statiques (CSS, JS, images)
├── Program.cs            # Point d'entrée et configuration
├── BrainHugeAcademy.csproj
└── Dockerfile
```

> Cette arborescence est indicative : ajuste-la à la structure réelle du dépôt.

---

## Modèle de données

```
Course ──< CourseModule ──< Lesson
              │
              └──< Quiz ──< QuizQuestion

ApplicationUser ──< Enrollment >── Course
                        │
                        └──< ModuleProgress >── CourseModule

ApplicationUser ──< AiConversation
```

| Table | Rôle |
|---|---|
| `Courses` | Cours (titre, catégorie, niveau, durée, mise en avant) |
| `CourseModules` | Modules ordonnés d'un cours |
| `Lessons` | Leçons d'un module (contenu et type) |
| `Quizzes` / `QuizQuestions` | Quiz de fin de module (4 options, bonne réponse, explication) |
| `Enrollments` | Inscription d'un utilisateur à un cours (unique par couple utilisateur/cours) |
| `ModuleProgresses` | Progression et score par module |
| `AiConversations` | Historique des échanges avec Nova |
| `AspNet*` | Tables ASP.NET Core Identity (utilisateurs, rôles…) |
| `DataProtectionKeys` | Clés de protection des données ASP.NET Core |

---

## Configuration

Les paramètres se trouvent dans `appsettings.json` et `appsettings.Development.json`.

| Paramètre | Description |
|---|---|
| `ConnectionStrings` | Chaîne de connexion SQLite |
| `PORT` (variable d'environnement) | Port d'écoute, utilisé par Docker |
| *(à compléter)* | Clé ou paramètres du moteur IA Nova |

> 🔐 Ne commite jamais de clés d'API. Utilise les [User Secrets](https://learn.microsoft.com/aspnet/core/security/app-secrets) en développement et des variables d'environnement en production.

---

## Pages principales

| Page | URL |
|---|---|
| Accueil | `/` |
| Catalogue | `/Learner/Catalog` |
| Détail d'un cours | `/Learner/Details/{id}` |
| Mes cours | `/Learner/MyCourses` |
| Tuteur IA Nova | `/AiTutor/Chat` |
| Connexion | `/Account/Login` |
| Inscription | `/Account/Register` |

---

## Dépannage

| Problème | Solution |
|---|---|
| `No project found` | Place-toi dans le dossier contenant le fichier `.csproj` |
| Mauvaise version du SDK | Installe le SDK correspondant à `<TargetFramework>` (`net8.0`) |
| Erreur de certificat HTTPS | `dotnet dev-certs https --trust` |
| Le conteneur plante au démarrage | Vérifie que la variable `PORT` est bien définie |
| Réinitialiser la base en local | Supprime le fichier `.db` SQLite puis relance `dotnet run` |
| Avertissement `No XML encryptor configured` | Sans gravité en local ; à configurer pour la production |

---

## Feuille de route

- [ ] Configurer la protection chiffrée des clés pour la production
- [ ] Ajouter des cours et des quiz supplémentaires
- [ ] Persister la base de données en production (volume ou base externe)
- [ ] Tests automatisés

---

<p align="center">Construit avec passion. Propulsé par l'IA. © 2026 BrainHuge Academy</p>
