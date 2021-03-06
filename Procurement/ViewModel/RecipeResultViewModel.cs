﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Procurement.ViewModel.Recipes;
using System.ComponentModel;
using POEApi.Model;
using System.Windows.Controls;

namespace Procurement.ViewModel
{
    public class RecipeResultViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private Dictionary<string, List<RecipeResult>> results;
        private RecipeManager manager;

        public Dictionary<string, List<RecipeResult>> Results
        {
            get { return results; }
            set 
            { 
                results = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Results"));
            }
        }

        private Item selectedItem;

        public Item SelectedItem
        {
            get { return selectedItem; }
            set 
            { 
                selectedItem = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("SelectedItem"));
            }
        }
        

        public RecipeResultViewModel()
        {
            manager = new RecipeManager();
            ApplicationState.LeagueChanged += new System.ComponentModel.PropertyChangedEventHandler(ApplicationState_LeagueChanged);
            updateResults();
        }

        public void RadioButtonSelected(Item item)
        {
            SelectedItem = item;
        }

        Dictionary<Tab, List<Item>> GetUsableCurrentLeagueItemsByTab()
        {
            Dictionary<Tab, List<Item>> itemsByTab = new Dictionary<Tab, List<Item>>();
            Stash stash = ApplicationState.Stash[ApplicationState.CurrentLeague];

            var usableTabs = stash.Tabs.Where(t => !Settings.Lists["IgnoreTabsInRecipes"].Contains(t.Name)).ToList();
            foreach (var tab in usableTabs)
            {
                itemsByTab.Add(tab, stash.GetItemsByTab(tab.i));
            }

            return itemsByTab;
        }

        private void updateResults()
        {
            Dictionary<Tab, List<Item>> itemsByTab = GetUsableCurrentLeagueItemsByTab();
            Results = manager.Run(itemsByTab);
            if (Results.Count > 0)
                SelectedItem = Results.Values.First().First().MatchedItems[0];
        }

        void ApplicationState_LeagueChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            updateResults();
        }
    }
}
