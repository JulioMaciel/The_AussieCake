using AussieCake.Question;
using System.Collections.Generic;
using System.Linq;

namespace AussieCake.Templates
{
    public class TemplateSumRetell : Template
    {
        public static List<int> TemplateGaps = new List<int>()
        {
            11, 26, 42, 80
        };

        public static List<int> OptionalExclusives = new List<int>()
        {
            91, 94
        };

        public static List<IQuest> Words = new List<IQuest>()
        {
            new SumRetellVM(0, "According"),
            new SumRetellVM(1, "to"),
            new SumRetellVM(2, "the"),
            new SumRetellVM(3, "specific"),
            new SumRetellVM(4, "oration"),
            new SumRetellVM(5, "provided"),
            new SumRetellVM(6, "by"),
            new SumRetellVM(7, "the"),
            new SumRetellVM(8, "speaker"),
            new SumRetellVM(9, ","),
            //new SumRetellVM(10, "["),
            new SumRetellVM(11, "key1"),
            //new SumRetellVM(12, "]"),
            new SumRetellVM(13, "."),
            new SumRetellVM(14, Paragraph),
            new SumRetellVM(15, "In"), //-
            new SumRetellVM(16, "addition"), //-
            new SumRetellVM(17, "to"), //-
            new SumRetellVM(18, "the"), //-
            new SumRetellVM(19, "initial"), //-
            new SumRetellVM(20, "focal"), //-
            new SumRetellVM(21, "point"), //-
            new SumRetellVM(22, ","), //-
            new SumRetellVM(23, "it"),
            //new SumRetellVM(90, "("), //*
            //new SumRetellVM(91, "also"), //+
            //new SumRetellVM(92, ")"), //*
            new SumRetellVM(24, "denotes"),
            //new SumRetellVM(25, "["),
            new SumRetellVM(26, "key2"),
            //new SumRetellVM(27, "]"),
            new SumRetellVM(28, "."),
            new SumRetellVM(29, Paragraph),
            new SumRetellVM(30, "Considering"), //-
            new SumRetellVM(31, "the"), //-
            new SumRetellVM(32, "most"), //-
            new SumRetellVM(33, "substantial"), //-
            new SumRetellVM(34, "insights"), //-
            new SumRetellVM(35, ","), //-
            new SumRetellVM(36, "it"),
            new SumRetellVM(37, "can"),
            //new SumRetellVM(93, "("), //*
            //new SumRetellVM(94, "also"), //+
            //new SumRetellVM(95, ")"), //*
            new SumRetellVM(38, "be"),
            new SumRetellVM(39, "stated"),
            //new SumRetellVM(40, "that"),
            //new SumRetellVM(41, "["),
            new SumRetellVM(42, "key3"),
            //new SumRetellVM(43, "]"),
            new SumRetellVM(44, "."),
            new SumRetellVM(45, Paragraph),
            new SumRetellVM(67, "To"),
            new SumRetellVM(68, "conclude"),
            new SumRetellVM(75, ","),
            new SumRetellVM(76, "the"), //+
            new SumRetellVM(77, "significance"), //+
            new SumRetellVM(78, "of"), //+
            //new SumRetellVM(79, "["), //+
            new SumRetellVM(80, "key+"), //+
            //new SumRetellVM(81, "]"), //+
            new SumRetellVM(82, "is"), //+
            new SumRetellVM(83, "crucial"), //+
            new SumRetellVM(84, "to"), //+
            new SumRetellVM(85, "the"), //+
            new SumRetellVM(86, "subject"), //+
            new SumRetellVM(87, "matter"), //+
            new SumRetellVM(88, "."), //+
        };
    }
}
