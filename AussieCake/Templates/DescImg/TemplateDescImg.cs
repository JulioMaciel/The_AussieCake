using AussieCake.Question;
using AussieCake.Util;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AussieCake.Templates
{
    public class TemplateDescImgy : Template
    {
        public static List<int> OptionalExclusives = new List<int>()
        {
            7, 9, 11, 13, 28, 30, 246, 247, 248, 249, 250, 251, 252
        };

        public static List<int> TemplateGaps = new List<int>()
        {
            4, 9, 13, 22, 30, 46, 57, 69, 80, 232
        };

        public static string GetRndType1Img()
        {
            string[] filePaths = Directory.GetFiles(CakePaths.DescImgFolder, ".", 
                                                    searchOption: SearchOption.TopDirectoryOnly);

            return filePaths.PickRandom();
        }

        public static List<IQuest> Words = new List<IQuest>()
        {
            new DescImgVM(0, "The"),
            new DescImgVM(1, "image"),
            new DescImgVM(2, "describes"),
            //new DescImgVM(3, "["),
            new DescImgVM(4, "title"),
            //new DescImgVM(5, "]"),
            //new DescImgVM(6, "("), //*
            new DescImgVM(7, "from"), //1
            //new DescImgVM(8, "["), //1
            new DescImgVM(9, "time1"), //1
            //new DescImgVM(10, "]"), //1
            new DescImgVM(11, "to"), //1
            //new DescImgVM(12, "["), //1
            new DescImgVM(13, "time2"), //1
            //new DescImgVM(14, "]"), //1
            //new DescImgVM(15, ")"), //*
            new DescImgVM(16, "in"),
            new DescImgVM(17, "different"),
            new DescImgVM(18, "categories"),
            new DescImgVM(19, "such"),
            new DescImgVM(20, "as"),
            //new DescImgVM(21, "["),
            new DescImgVM(22, "cats[...]"),
            //new DescImgVM(23, "]"),
            new DescImgVM(24, "and"),
            new DescImgVM(25, "so"),
            new DescImgVM(26, "on"),
            //new DescImgVM(27, "("), //*
            new DescImgVM(28, "in"), //0?
            //new DescImgVM(29, "["), //0?
            new DescImgVM(30, "unit"), //0?
            //new DescImgVM(31, "]"), //0?
            //new DescImgVM(32, ")"), //*
            new DescImgVM(33, "."),
            new DescImgVM(34, Paragraph),
            new DescImgVM(35, "It"),
            new DescImgVM(36, "is"),
            new DescImgVM(37, "clear"),
            new DescImgVM(38, "from"),
            new DescImgVM(39, "the"),
            new DescImgVM(40, "image"),
            new DescImgVM(41, "that"),
            //new DescImgVM(42, "("), //*
            //new DescImgVM(43, "for"), //0?
            //new DescImgVM(44, ")"), //*
            //new DescImgVM(45, "["),
            new DescImgVM(46, "cat-max"), //0?
            //new DescImgVM(47, "/"), //*
            //new DescImgVM(48, "colour"), //0?
            //new DescImgVM(49, "]"), 
            new DescImgVM(50, "has"),
            new DescImgVM(51, "the"),
            new DescImgVM(52, "highest"),
            new DescImgVM(53, ","),
            new DescImgVM(54, "which"),
            new DescImgVM(55, "is"),
            //new DescImgVM(56, "["),
            new DescImgVM(57, "n%"),
            //new DescImgVM(58, "]"),
            new DescImgVM(59, ","),
            new DescImgVM(60, "on"),
            new DescImgVM(61, "the"),
            new DescImgVM(62, "other"),
            new DescImgVM(63, "hand"),
            new DescImgVM(64, ","),
            //new DescImgVM(65, "("), //*
            //new DescImgVM(66, "for"), //0?
            //new DescImgVM(67, ")"), //*
            //new DescImgVM(68, "["),
            new DescImgVM(69, "cat-min"), //0?
            //new DescImgVM(70, "/"), //*
            //new DescImgVM(71, "colour"), //0?
            //new DescImgVM(72, "]"),
            new DescImgVM(73, "has"),
            new DescImgVM(74, "the"),
            new DescImgVM(75, "lowest"),
            //new DescImgVM(76, "("), //*
            new DescImgVM(77, "which"), //0?
            new DescImgVM(78, "is"), //0?
            //new DescImgVM(79, "["), //0?
            new DescImgVM(80, "n%"), //0?
            //new DescImgVM(81, "]"), //0?
            //new DescImgVM(82, ")"), //*
            new DescImgVM(83, "."),
            new DescImgVM(84, Paragraph), 
            new DescImgVM(228, "After"),
            new DescImgVM(229, "analysing"),
            new DescImgVM(230, "the"),
            //new DescImgVM(231, "["),
            new DescImgVM(232, "title"),
            //new DescImgVM(233, "]"),
            new DescImgVM(234, "it"),
            new DescImgVM(235, "can"),
            new DescImgVM(236, "be"),
            new DescImgVM(237, "concluded"),
            new DescImgVM(238, "that"),
            new DescImgVM(239, "this"),
            new DescImgVM(240, "image"),
            new DescImgVM(241, "is"),
            new DescImgVM(242, "showing"),
            new DescImgVM(243, "crucial"),
            new DescImgVM(244, "information"),
            //new DescImgVM(245, "("),
            new DescImgVM(246, "strongly"),
            new DescImgVM(247, "supported"),
            new DescImgVM(248, "by"),
            new DescImgVM(249, "important"),
            new DescImgVM(250, "facts"),
            new DescImgVM(251, "and"),
            new DescImgVM(252, "figures"),
            //new DescImgVM(253, ")"),
            new DescImgVM(254, "."),
        };
    }
}
