using UnityEngine;

public class CardSelectionManager : MonoBehaviour
{
    public CardDisplay SelectedCard { get; private set; }

    public void SelectCard(CardDisplay card)
    {
        if (SelectedCard != null)
        {
            SelectedCard.SetHighlight(false);
        }

        if (SelectedCard != card)
        {
            SelectedCard = card;
            SelectedCard.SetHighlight(true);
        }
        else
        {
            SelectedCard = null;
        }
    }
}
