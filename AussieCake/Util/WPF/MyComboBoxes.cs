using System;
using System.Data.Entity.Design.PluralizationServices;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

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

        public static ComboChallenge BuildSynonyms(string word, ComboChallenge reference, StackPanel parent, bool isFirstUp, Microsoft.Office.Interop.Word.Application wordApp)
        {
            reference.VerticalContentAlignment = VerticalAlignment.Center;
            reference.Margin = new Thickness(1, 0, 1, 0);
            parent.Children.Add(reference);

            var synonyms = FileHtmlControls.GetSynonyms(word, wordApp).ToList();
            if (isFirstUp)
            {
                synonyms.ForEach(x => x.First().ToString().ToUpper());
                word = word.UpperFirst();
            }

            if (!synonyms.Any())
                Errors.ThrowErrorMsg(ErrorType.SynonymsNotFound, word);

            var service = PluralizationService.CreateService(System.Globalization.CultureInfo.CurrentCulture);
            if (service.IsPlural(word))
                for (int i = 0; i < synonyms.Count; i++)
                    synonyms[i] = service.Pluralize(synonyms[i]);

            //var plural = word.Humanize().Pluralize(false);
            //if (!plural.IsEmpty())
            //{
            //    for (int i = 0; i < synonyms.Count; i++)
            //        synonyms[i] = synonyms[i].Humanize().Pluralize(false);
            //}

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
