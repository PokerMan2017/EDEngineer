using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using EDEngineer.Models;

namespace EDEngineer
{
    public class ShoppingListViewModel : INotifyPropertyChanged, IEnumerable<Blueprint>
    {
        private readonly ILanguage languages;
        private readonly List<Blueprint> blueprints;
        private bool any;

        public ShoppingListViewModel(List<Blueprint> blueprints, ILanguage languages)
        {
            this.blueprints = blueprints;
            this.languages = languages;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public IEnumerator<Blueprint> GetEnumerator()
        {
            var added = blueprints
                .SelectMany(b => Enumerable.Repeat(b, b.ShoppingListCount))
                .ToList();

            if (added.Any())
            {
                var metaBlueprint = new Blueprint(languages, "Shopping", "Shopping List", 0, new BlueprintIngredient[0],
                    new string[0]);
                metaBlueprint = added
                    .Aggregate(metaBlueprint, (acc, current) =>
                    {
                        acc.Ingredients = acc.Ingredients.Concat(current.Ingredients).ToList();
                        return acc;
                    });
                metaBlueprint.BlueprintName = "Shopping List";
                metaBlueprint.Type = "Shopping";
                metaBlueprint.Grade = 0;
                metaBlueprint.Engineers = new List<string>();

                metaBlueprint.Ingredients = metaBlueprint.Ingredients
                                                         .GroupBy(i => i.Entry.Data.Name)
                                                         .Select(
                                                             i =>
                                                                 new BlueprintIngredient(i.First().Entry,
                                                                     i.Sum(c => c.Size)))
                                                         .ToList();
                yield return metaBlueprint;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}