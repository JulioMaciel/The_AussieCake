using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Data.Entity.Design.PluralizationServices;

namespace AussieCake.Util.WPF
{
    public static class MyCbBxs
    {
        public static ComboBox GetImportance(ComboBox reference, int row, int column, Grid parent, Importance imp, bool isFilter)
        {
            var cb = Get(reference, row, column, parent);
            var source = Enum.GetValues(typeof(Importance)).Cast<Importance>();

            if (!isFilter) // remove Any when edit line
                source = source.Where(i => i != Importance.Any);

            cb.ItemsSource = source;
            cb.SelectedValue = imp;

            return cb;
        }

        public static ComboBox Get(ComboBox reference, int row, int column, Grid parent)
        {
            var cb = Get(reference);
            UtilWPF.SetGridPosition(cb, row, column, parent);

            return cb;
        }

        public static ComboBox Get(ComboBox reference, StackPanel parent)
        {
            var cb = Get(reference);
            parent.Children.Add(cb);

            return cb;
        }

        private static ComboBox Get(ComboBox reference)
        {
            reference.VerticalContentAlignment = VerticalAlignment.Center;
            reference.Margin = new Thickness(1, 0, 1, 0);

            return reference;
        }

        public static ComboChallenge GetSynonyms(string word, ComboChallenge reference, StackPanel parent, bool isFirstUp)
        {
            reference.VerticalContentAlignment = VerticalAlignment.Center;
            reference.Margin = new Thickness(1, 0, 1, 0);
            parent.Children.Add(reference);

            var synonyms = FileHtmlControls.GetSynonyms(word).ToList();

            if (isFirstUp)
                synonyms.ForEach(x => x.First().ToString().ToUpper());

            if (!synonyms.Any())
                Errors.ThrowErrorMsg(ErrorType.SynonymsNotFound, word);

            var sv = PluralizationService.CreateService(System.Globalization.CultureInfo.CurrentCulture);

            if (sv.IsPlural(word))
                for (int i = 0; i < synonyms.Count; i++)
                    synonyms[i] = sv.Pluralize(synonyms[i]);

            synonyms.Add(word);


            var shuffled = synonyms.OrderBy(x => Guid.NewGuid()).ToList();

            reference.ItemsSource = shuffled;
            reference.SelectedIndex = 0;

            reference.CorrectIndex = reference.Items.IndexOf(word);

            return reference;
        }
    }

    public class ComboChallenge : ComboBox
    {
        public int CorrectIndex { get; set; }

        public bool IsCorrect()
        {
            return CorrectIndex == SelectedIndex;
        }
    }
}
