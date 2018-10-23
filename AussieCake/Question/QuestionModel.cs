namespace AussieCake.Question
{
    public abstract class QuestionModel
    {
        public int Id { get; protected set; }

        public string PtBr { get; protected set; }
        public string Definition { get; protected set; }
        public int Importance { get; protected set; }
        public string SentencesId { get; protected set; }

        public int IsActive { get; protected set; }

        protected bool IsReal { get; set; } = false;

        protected QuestionModel(int id, string ptBr, string definition, int importance, string sentencesId, int isActive)
            : this(ptBr, definition, importance, isActive)
        {
            Id = id;
            SentencesId = sentencesId;

            IsReal = true;
        }

        protected QuestionModel(string ptBr, string definition, int importance, int isActive)
        {
            IsActive = isActive;
            PtBr = ptBr;
            Definition = definition;
            Importance = importance;            
        }

    }
}
