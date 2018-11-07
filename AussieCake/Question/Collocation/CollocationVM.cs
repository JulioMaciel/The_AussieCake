using AussieCake.Util;
using System.Collections.Generic;

namespace AussieCake.Question
{
    public class ColVM : QuestVM
    {
        public List<string> Prefixes { get; private set; }
        public string Component1 { get; private set; }
        public bool IsComp1Verb { get; private set; }
        public List<string> LinkWords { get; private set; }
        public string Component2 { get; private set; }
        public bool IsComp2Verb { get; private set; }
        public List<string> Suffixes { get; private set; }

        public ColVM(int id, List<string> prefixes, string component1, bool isC1_v, List<string> linkWords,
                        string component2, bool isC2_v, List<string> suffixes, string definition, string ptBr,
                        Importance importance, bool isActive)
            : base(id, definition, ptBr, importance, isActive, Model.Col)
        {
            SetProperties(prefixes, component1, isC1_v, linkWords, component2, isC2_v, suffixes);
        }

        public ColVM(List<string> prefixes, string component1, bool isC1_v, List<string> linkWords,
                string component2, bool isC2_v, List<string> suffixes, string definition, string ptBr,
                Importance importance, bool isActive)
            : base(definition, ptBr, importance, isActive, Model.Col)
        {
            SetProperties(prefixes, component1, isC1_v, linkWords, component2, isC2_v, suffixes);
        }

        private void SetProperties(List<string> prefixes, string component1, bool isC1_v, List<string> linkWords, string component2, bool isC2_v, List<string> suffixes)
        {
            Prefixes = prefixes;
            Component1 = component1;
            IsComp1Verb = isC1_v;
            LinkWords = linkWords;
            Component2 = component2;
            IsComp2Verb = isC2_v;
            Suffixes = suffixes;
        }

        public ColModel ToModel()
        {
            var pref_raw = Prefixes.ToText();
            var link_raw = LinkWords.ToText();
            var suf_raw = Suffixes.ToText();

            var isC1v_raw = IsComp1Verb.ToInt();
            var isC2v_raw = IsComp2Verb.ToInt();
            var isActive_raw = IsActive.ToInt();

            if (IsReal)
                return new ColModel(Id, pref_raw, Component1, isC1v_raw, link_raw, Component2, isC2v_raw,
                    suf_raw, PtBr, Definition, (int)Importance, isActive_raw);
            else
                return new ColModel(pref_raw, Component1, isC1v_raw, link_raw, Component2, isC2v_raw,
                                suf_raw, PtBr, Definition, (int)Importance, isActive_raw);
        }

        public override void LoadCrossData()
        {
            if (Component1 == "give" && Component2 == "information")
                System.Diagnostics.Debug.WriteLine("debug:LoadCrossData(give_information)");

            base.LoadCrossData();
        }
    }
}
