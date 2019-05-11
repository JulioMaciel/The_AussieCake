namespace AussieCake.Question
{
    public abstract class QuestionModel
    {
        public int Id { get; protected set; }

        public string Text { get; protected set; }
        public int Importance { get; protected set; }

        public int IsActive { get; protected set; }

        protected bool IsReal { get; set; } = false;

        protected QuestionModel(int id, string text, int importance, int isActive)
            : this(text, importance, isActive)
        {
            Id = id;

            IsReal = true;
        }

        protected QuestionModel(string text, int importance, int isActive)
        {
            IsActive = isActive;
            Text = text;
            Importance = importance;            
        }

    }
}
