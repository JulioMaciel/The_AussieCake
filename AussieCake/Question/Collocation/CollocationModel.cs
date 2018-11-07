using AussieCake.Util;

namespace AussieCake.Question
{
    public class ColModel : QuestionModel
    {
        public string Prefixes { get; private set; }
        public string Component1 { get; private set; }
        public int IsComp1Verb { get; private set; }
        public string LinkWords { get; private set; }
        public string Component2 { get; private set; }
        public int IsComp2Verb { get; private set; }
        public string Suffixes { get; private set; }

        public ColModel(int id, string pref, string comp1, int isComp1_v, string link, string comp2, int isComp2_v, string suf,
            string ptBr, string definition, int importance, int isActive)
            : base(id, ptBr, definition, importance, isActive)
        {
            SetProperties(pref, comp1, isComp1_v, link, comp2, isComp2_v, suf);
        }

        public ColModel(string pref, string comp1, int isComp1_v, string link, string comp2, int isComp2_v, string suf,
            string ptBr, string definition, int importance, int isActive)
            : base(ptBr, definition, importance, isActive)
        {
            SetProperties(pref, comp1, isComp1_v, link, comp2, isComp2_v, suf);
        }

        private void SetProperties(string pref, string comp1, int isComp1_v, string link, string comp2, int isComp2_v, string suf)
        {
            Prefixes = pref;
            Component1 = comp1;
            IsComp1Verb = isComp1_v;
            LinkWords = link;
            Component2 = comp2;
            IsComp2Verb = isComp2_v;
            Suffixes = suf;
        }

        public ColVM ToVM()
        {
            var pref_vm = Prefixes.ToListString();
            var link_vm = LinkWords.ToListString();
            var suf_vm = Suffixes.ToListString();
            var def_vm = Definition.EmptyIfNull();
            var ptBr_vm = PtBr.EmptyIfNull();

            var isC1v_vm = IsComp1Verb.ToBool();
            var isC2v_vm = IsComp2Verb.ToBool();
            var isActive_vm = IsActive.ToBool();

            if (IsReal)
                return new ColVM(Id, pref_vm, Component1, isC1v_vm, link_vm, Component2, isC2v_vm,
                    suf_vm, def_vm, ptBr_vm, (Importance)Importance, isActive_vm);
            else
                return new ColVM(pref_vm, Component1, isC1v_vm, link_vm, Component2, isC2v_vm,
                    suf_vm, def_vm, ptBr_vm, (Importance)Importance, isActive_vm);
        }
    }
}
