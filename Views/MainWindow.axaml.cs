using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Media;

namespace Memory.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private Border _firstPick = null;

    private async void PickCard(object sender, RoutedEventArgs e)
    {
        // Get text and card from text
        var currentCard = sender as Border;
        var currentCardText = currentCard.Child as TextBlock;

        FlipCard(currentCard);
        LockCard(currentCard);
        
        if (_firstPick == null)
        {
            _firstPick = currentCard;
            Debug.WriteLine("First pick set!");
        }
        else
        {
            // Do cards Match? Do nothing
            var previousCardText = _firstPick.Child as TextBlock;
            if (previousCardText.Text == currentCardText.Text)
            {
                _firstPick = null;
                return;
            }
            else
            {
                this.IsEnabled = false;
                await Task.Delay(1000);
                this.IsEnabled = true;
                ResetCard(currentCard);
                ResetCard(_firstPick);
                _firstPick = null;
            }
        }
        
    }

    private void NewGame(object sender, RoutedEventArgs e)
    {
        // Get Rows and Cols
        var cardList = Cards.GetLogicalDescendants().OfType<TextBlock>().Where(t => (string)t.Tag == "card").ToList();
        // Create list of size (rows*cols)
        int fields = cardList.Count();
        var prelist = new List<int>();

        // Fill with size /2 number pairs
        for (int i = 1; i <= fields; i++)
        {
            double number = Math.Ceiling((double)i / 2);
            prelist.Add((int)number);
        }
        
        // Fill second list with randomized items
        var gameList = new List<int>();
        for (int i = 0; i < fields; i++)
        {
            int index = Random.Shared.Next(prelist.Count);
            gameList.Add(prelist[index]);
            prelist.RemoveAt(index);
        }

        // Fill text of each control
        int listIndex = 0;
        foreach (var card in cardList)
        {
            card.Text = gameList[listIndex].ToString();
            var parent = card.Parent as Border;
            parent.Background = Brushes.BlueViolet;
            parent.PointerPressed += PickCard;
            card.IsVisible = false;
            listIndex++;
        }
        
        // Make next pick the first pick and reset last pick
        _firstPick = null;

    }

    private void LockCard(Border card)
    {
        card.PointerPressed -= PickCard;
    }
    private void UnlockCard(Border card)
    {
        card.PointerPressed += PickCard;
    }

    private void FlipCard(Border card)
    {
        var text = card.Child as TextBlock;
        if (text.IsVisible)
        {
            card.Background = Brushes.BlueViolet;
            text.IsVisible = false;
        } else
        {
            card.Background = Brushes.DarkOrange; 
            text.IsVisible = true ;
        }
    }

    private void ResetCard(Border card)
    {
        UnlockCard(card);
        FlipCard(card);
    }
}