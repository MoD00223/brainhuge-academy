using Microsoft.EntityFrameworkCore;
using BrainHugeAcademy.Models;

namespace BrainHugeAcademy.Data;

public static class SeedData
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        using var context = new ApplicationDbContext(
            new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlite("Data Source=brainhuge.db")
                .Options);

        if (context.Courses.Any()) return;

        var courses = new List<Course>
        {
            new()
            {
                Title = "Les Essentiels de la Visualisation de Données",
                Description = "Transformez des données brutes en tableaux de bord clairs et en récits visuels convaincants.",
                Category = "Données",
                Level = "Débutant",
                DurationHours = 10,
                IsFeatured = true,
                CreatedAt = DateTime.UtcNow,
                Modules = new List<CourseModule>
                {
                    new()
                    {
                        Title = "Fondamentaux de la Visualisation",
                        Description = "Apprenez les principes derrière une visualisation de données efficace.",
                        OrderIndex = 1,
                        Content = "La visualisation de données est la représentation graphique d'informations et de données. En utilisant des éléments visuels comme des graphiques, des courbes et des cartes, les outils de visualisation de données offrent un moyen accessible de voir et de comprendre les tendances, les valeurs aberrantes et les schémas dans les données.",
                        Lessons = new List<Lesson>
                        {
                            new() { Title = "Pourquoi Visualiser les Données ?", OrderIndex = 1, Content = "Les humains traitent les visuels 60 000 fois plus vite que le texte. Une bonne visualisation transforme la complexité en clarté.", LessonType = "Théorie" },
                            new() { Title = "Types de Graphiques et Quand les Utiliser", OrderIndex = 2, Content = "Les diagrammes à barres pour la comparaison, les courbes pour les tendances, les nuages de points pour les relations, les camemberts pour la composition.", LessonType = "Théorie" },
                            new() { Title = "Couleur et Accessibilité", OrderIndex = 3, Content = "Utilisez la couleur de manière délibérée. Assurez-vous de palettes adaptées aux daltoniens et d'un contraste suffisant pour tous les utilisateurs.", LessonType = "Théorie" }
                        },
                        Quizzes = new List<Quiz>
                        {
                            new()
                            {
                                Title = "Quiz des Essentiels de la Viz",
                                Questions = new List<QuizQuestion>
                                {
                                    new() { QuestionText = "Quel type de graphique est le meilleur pour montrer les tendances au fil du temps ?", OptionA = "Camembert", OptionB = "Courbe", OptionC = "Nuage de points", OptionD = "Diagramme à barres", CorrectOption = "B", Explanation = "Les courbes excellent dans la montration des changements continus au fil du temps." },
                                    new() { QuestionText = "Quel est l'objectif principal de la visualisation de données ?", OptionA = "Rendre les données jolies", OptionB = "Cacher la complexité", OptionC = "Communiquer les insights clairement", OptionD = "Augmenter la taille du fichier", CorrectOption = "C", Explanation = "L'objectif fondamental de la visualisation de données est de rendre les données compréhensibles et exploitables." },
                                    new() { QuestionText = "Pourquoi les palettes adaptées aux daltoniens sont-elles importantes ?", OptionA = "C'est à la mode", OptionB = "Pour l'accessibilité et la conception inclusive", OptionC = "Pour économiser l'encre", OptionD = "Ça charge plus vite", CorrectOption = "B", Explanation = "La conception accessible garantit que tout le monde peut interpréter vos visualisations." }
                                }
                            }
                        }
                    }
                }
            },
            new()
            {
                Title = "ASP.NET Core MVC en Pratique",
                Description = "Construisez de vraies applications web avec l'architecture MVC et Entity Framework Core.",
                Category = "Développement Web",
                Level = "Intermédiaire",
                DurationHours = 18,
                IsFeatured = true,
                CreatedAt = DateTime.UtcNow,
                Modules = new List<CourseModule>
                {
                    new()
                    {
                        Title = "Plongée dans l'Architecture MVC",
                        Description = "Maîtrisez le patron Modèle-Vue-Contrôleur en .NET.",
                        OrderIndex = 1,
                        Content = "ASP.NET Core MVC fournit une séparation claire des responsabilités. Le Modèle représente vos données, la Vue gère la présentation, et le Contrôleur manage l'interaction utilisateur et le flux de l'application.",
                        Lessons = new List<Lesson>
                        {
                            new() { Title = "Comprendre le Flux MVC", OrderIndex = 1, Content = "Requête -> Routeur -> Contrôleur -> Modèle -> Vue -> Réponse. Chaque composant a une seule responsabilité.", LessonType = "Théorie" },
                            new() { Title = "Modèles et ViewModels", OrderIndex = 2, Content = "Les modèles de domaine représentent vos données. Les ViewModels façonnent les données spécifiquement pour la vue. Gardez-les séparés.", LessonType = "Théorie" },
                            new() { Title = "Routage et Middleware", OrderIndex = 3, Content = "Le routage par convention et par attribut vous donne un contrôle total. Le pipeline de middleware traite chaque requête.", LessonType = "Théorie" }
                        },
                        Quizzes = new List<Quiz>
                        {
                            new()
                            {
                                Title = "Quiz des Fondamentaux MVC",
                                Questions = new List<QuizQuestion>
                                {
                                    new() { QuestionText = "Dans MVC, qui gère l'entrée utilisateur ?", OptionA = "Modèle", OptionB = "Vue", OptionC = "Contrôleur", OptionD = "Routeur", CorrectOption = "C", Explanation = "Les contrôleurs reçoivent et traitent l'entrée utilisateur, puis décident quoi renvoyer." },
                                    new() { QuestionText = "Quel est le rôle de la Vue ?", OptionA = "Logique métier", OptionB = "Accès aux données", OptionC = "Couche de présentation", OptionD = "Authentification", CorrectOption = "C", Explanation = "Les vues sont responsables uniquement du rendu des données en HTML." },
                                    new() { QuestionText = "Que fournit Entity Framework Core ?", OptionA = "Rendu frontend", OptionB = "Mapping objet-relationnel (ORM)", OptionC = "Bundling JavaScript", OptionD = "Services d'email", CorrectOption = "B", Explanation = "EF Core mappe les objets C# aux tables de base de données, simplifiant l'accès aux données." }
                                }
                            }
                        }
                    },
                    new()
                    {
                        Title = "Entity Framework Core en Action",
                        Description = "Travail pratique avec les bases de données : migrations, requêtes et relations.",
                        OrderIndex = 2,
                        Content = "Entity Framework Core est un ORM qui élimine le besoin de la plupart du code d'accès aux données. Il fonctionne avec SQLite, SQL Server, PostgreSQL et plus encore.",
                        Lessons = new List<Lesson>
                        {
                            new() { Title = "Migrations et Configuration de la BDD", OrderIndex = 1, Content = "Utilisez 'dotnet ef migrations add' et 'dotnet ef database update' pour gérer votre schéma version par version.", LessonType = "Pratique" },
                            new() { Title = "Requêter avec LINQ", OrderIndex = 2, Content = "Écrivez des requêtes typées en toute sécurité avec LINQ. EF les traduit en SQL automatiquement.", LessonType = "Pratique" }
                        },
                        Quizzes = new List<Quiz>
                        {
                            new()
                            {
                                Title = "Quiz EF Core",
                                Questions = new List<QuizQuestion>
                                {
                                    new() { QuestionText = "Quelle commande crée une nouvelle migration EF Core ?", OptionA = "dotnet ef database update", OptionB = "dotnet ef migrations add Init", OptionC = "dotnet build", OptionD = "dotnet run", CorrectOption = "B", Explanation = "La commande 'migrations add' crée la classe de migration." },
                                    new() { QuestionText = "Quelle base de données utilise SQLite ?", OptionA = "Modèle client-serveur", OptionB = "Fichier autonome sans serveur", OptionC = "Cloud uniquement", OptionD = "Mémoire uniquement", CorrectOption = "B", Explanation = "SQLite est une base de données autonome basée sur des fichiers — parfaite pour les déploiements légers." }
                                }
                            }
                        }
                    }
                }
            },
            new()
            {
                Title = "Découvrez l'IA Générative",
                Description = "Comprenez comment fonctionnent les modèles d'IA générative et construisez votre premier cas d'usage.",
                Category = "Intelligence Artificielle",
                Level = "Débutant",
                DurationHours = 8,
                IsFeatured = true,
                CreatedAt = DateTime.UtcNow,
                Modules = new List<CourseModule>
                {
                    new()
                    {
                        Title = "Comment Fonctionne l'IA Générative",
                        Description = "Des tokens aux transformers — comprenez la magie derrière la génération par l'IA.",
                        OrderIndex = 1,
                        Content = "Les modèles d'IA générative apprennent des schémas à partir de vastes ensembles de données et utilisent ces connaissances pour créer du nouveau contenu — texte, images, code et plus encore. L'architecture transformer est le fondement des grands modèles de langage modernes.",
                        Lessons = new List<Lesson>
                        {
                            new() { Title = "Tokens et Embeddings", OrderIndex = 1, Content = "Les modèles de langage ne lisent pas des mots — ils lisent des tokens. Chaque token est mappé à un vecteur haute dimension qui capture le sens.", LessonType = "Théorie" },
                            new() { Title = "L'Architecture Transformer", OrderIndex = 2, Content = "L'auto-attention permet au modèle d'évaluer l'importance de chaque token par rapport à tous les autres tokens. C'est ce qui rend les LLM modernes si puissants.", LessonType = "Théorie" },
                            new() { Title = "Bases de l'Ingénierie de Prompts", OrderIndex = 3, Content = "La manière dont vous formulez un prompt affecte dramatiquement la qualité de la sortie. Soyez spécifique, fournissez du contexte, et itérez.", LessonType = "Pratique" }
                        },
                        Quizzes = new List<Quiz>
                        {
                            new()
                            {
                                Title = "Quiz de l'IA Générative",
                                Questions = new List<QuizQuestion>
                                {
                                    new() { QuestionText = "Quelle est l'unité fondamentale du texte dans les LLM ?", OptionA = "Mots", OptionB = "Caractères", OptionC = "Tokens", OptionD = "Phrases", CorrectOption = "C", Explanation = "Les LLM traitent le texte sous forme de tokens, qui peuvent être des parties de mots, des mots entiers ou des unités multi-mots." },
                                    new() { QuestionText = "Qu'est-ce qui rend les transformers spéciaux ?", OptionA = "Ils transforment les types de données", OptionB = "Le mécanisme d'auto-attention", OptionC = "Ils sont plus rapides que les RNN", OptionD = "Ils nécessitent moins de données", CorrectOption = "B", Explanation = "L'auto-attention permet aux transformers de capturer les relations entre tous les tokens simultanément." },
                                    new() { QuestionText = "Qu'est-ce que l'ingénierie de prompts ?", OptionA = "Construire des prompts physiques", OptionB = "Rédiger des entrées efficaces pour les modèles d'IA", OptionC = "Coder en Python", OptionD = "Entraîner un modèle de zéro", CorrectOption = "B", Explanation = "L'ingénierie de prompts est l'art de concevoir des entrées qui guident l'IA vers les résultats souhaités." }
                                }
                            }
                        }
                    }
                }
            }
        };

        context.Courses.AddRange(courses);
        await context.SaveChangesAsync();
    }
}