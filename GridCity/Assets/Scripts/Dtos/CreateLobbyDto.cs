namespace Assets.Scripts.Dtos
{
    public class CreateLobbyDto
    {
        public string LobbyName { get; set; }
        public int MapSizeX { get; set; }
        public int MapSizeY { get; set; }
        public int DeckSize { get; set; }
    }
}
