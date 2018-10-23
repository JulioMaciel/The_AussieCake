using System;

namespace AussieCake.Attempt
{
    public interface IAttempt
    {
        int Id { get; }

        int IdQuestion { get; set; }

        int Score { get; set; }
        DateTime When { get; set; }

        Model Type { get; }
    }
}
