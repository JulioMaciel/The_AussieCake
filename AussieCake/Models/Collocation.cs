﻿using AussieCake.ViewModels;
using System;

namespace AussieCake.Models
{
    public class Collocation : BaseModel
    {
        public string Prefixes { get; set; } // it's a concatenate list
        public string Component1 { get; set; }
        public string LinkWords { get; set; } // it's a concatenate list
        public string Component2 { get; set; }
        public string Suffixes { get; set; } // it's a concatenate list

        public string PtBr { get; set; }
        public int Importance { get; set; }
        public string SentencesId { get; set; } // it's a concatenate list

        public bool IsActive { get; set; }

        public Collocation()
        {
        }

        public Collocation(CollocationVM viewModel)
        {
            Id = viewModel.Id;

            if (viewModel.Prefixes != null)
                Prefixes = String.Join(";", viewModel.Prefixes.ToArray());

            Component1 = viewModel.Component1;

            if (viewModel.LinkWords != null)
                LinkWords = String.Join(";", viewModel.LinkWords.ToArray());

            Component2 = viewModel.Component2;

            if (viewModel.Suffixes != null)
                Suffixes = String.Join(";", viewModel.Suffixes.ToArray());

            if (viewModel.PtBr != null)
                PtBr = viewModel.PtBr;

            Importance = (int)viewModel.Importance;

            if (viewModel.SentencesId != null)
                SentencesId = String.Join(";", viewModel.SentencesId.ToArray());

            IsActive = viewModel.IsActive;
        }

        public Collocation(string prefixes, string component1, string linkWords, string component2, string suffixes, string ptBr, int importance, string sentencesId, bool isActive)
        {
            Prefixes = prefixes;
            Component1 = component1;
            LinkWords = linkWords;
            Component2 = component2;
            Suffixes = suffixes;
            PtBr = ptBr;
            Importance = importance;
            SentencesId = sentencesId;
            IsActive = isActive;
        }

        public Collocation(int id, string prefixes, string component1, string linkWords, string component2, string suffixes, string ptBr, int importance, string sentencesId, bool isActive)
            : this(prefixes, component1, linkWords, component2, suffixes, ptBr, importance, sentencesId, isActive)
        {
            Id = id;
        }
    }
}
