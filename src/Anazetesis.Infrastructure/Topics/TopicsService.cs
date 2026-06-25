namespace Anazetesis.Infrastructure.Topics;

using Anazetesis.Core.Interfaces;
using Anazetesis.Core.Models;

public sealed class TopicsService : ITopicsService
{
    // Mirrors the frontend's mockTopics.ts exactly — same slugs, titles, colors, example questions
    private static readonly IReadOnlyList<Topic> AllTopics =
    [
        new()
        {
            Slug             = "ten_commandments",
            Title            = "The Ten Commandments",
            Subtitle         = "The foundation of moral law",
            Icon             = "📜",
            Description      = "Explore the Ten Commandments as taught by the Church — their biblical origin, moral implications, and how they apply to modern life.",
            Color            = "from-amber-600 to-yellow-500",
            ExampleQuestions =
            [
                "What is the purpose of the Ten Commandments?",
                "How does the Church interpret the Third Commandment today?",
                "What did St. Augustine teach about the Commandments?"
            ]
        },
        new()
        {
            Slug             = "core_beliefs",
            Title            = "Core Beliefs",
            Subtitle         = "The Creed and fundamental doctrines",
            Icon             = "✝️",
            Description      = "The foundational beliefs of the Catholic faith — the Trinity, Incarnation, Resurrection, and the articles of the Nicene Creed.",
            Color            = "from-blue-600 to-indigo-500",
            ExampleQuestions =
            [
                "What is the doctrine of the Trinity?",
                "Why is the Resurrection central to Christian faith?",
                "What does the Church teach about salvation?"
            ]
        },
        new()
        {
            Slug             = "sacraments",
            Title            = "The Sacraments",
            Subtitle         = "Visible signs of invisible grace",
            Icon             = "🙏",
            Description      = "The seven sacraments instituted by Christ — their biblical basis, historical development, and theological meaning.",
            Color            = "from-emerald-600 to-green-500",
            ExampleQuestions =
            [
                "What is the biblical basis for the Eucharist?",
                "How did the early Church celebrate Baptism?",
                "What did the Church Fathers teach about Confession?"
            ]
        },
        new()
        {
            Slug             = "scripture",
            Title            = "Scripture & Translation",
            Subtitle         = "Understanding the Holy Bible",
            Icon             = "📖",
            Description      = "The interpretation of Scripture according to the Church — translation history, exegesis, and the relationship between Scripture and Tradition.",
            Color            = "from-rose-600 to-pink-500",
            ExampleQuestions =
            [
                "How does the Church interpret Scripture?",
                "What is the difference between the Septuagint and the Vulgate?",
                "What did St. Jerome contribute to biblical translation?"
            ]
        },
        new()
        {
            Slug             = "tradition",
            Title            = "Tradition & Culture",
            Subtitle         = "Faith lived through the ages",
            Icon             = "🏛️",
            Description      = "How Catholic tradition has shaped and been shaped by culture — liturgical development, popular piety, and the inculturation of the faith.",
            Color            = "from-purple-600 to-violet-500",
            ExampleQuestions =
            [
                "What is Sacred Tradition?",
                "How did early Christian art develop?",
                "What is the history of the Rosary?"
            ]
        },
        new()
        {
            Slug             = "apologetics",
            Title            = "Apologetics",
            Subtitle         = "Defending the faith with reason",
            Icon             = "⚔️",
            Description      = "Rational defense of Catholic teachings — arguments for the existence of God, the divinity of Christ, and the authority of the Church.",
            Color            = "from-orange-600 to-red-500",
            ExampleQuestions =
            [
                "How do we know God exists?",
                "What evidence is there for the Resurrection?",
                "Why is the Catholic Church the Church founded by Christ?"
            ]
        },
        new()
        {
            Slug             = "defending_faith",
            Title            = "Defending the Faith",
            Subtitle         = "Engaging with modern challenges",
            Icon             = "🛡️",
            Description      = "Addressing contemporary challenges to faith — atheism, scientism, relativism, and secularism — with charity and reasoned argument.",
            Color            = "from-slate-700 to-gray-600",
            ExampleQuestions =
            [
                "How do Catholics respond to atheism?",
                "Does science disprove God?",
                "How can faith and reason coexist?"
            ]
        }
    ];

    public IReadOnlyList<Topic> GetAllTopics() => AllTopics;
}
