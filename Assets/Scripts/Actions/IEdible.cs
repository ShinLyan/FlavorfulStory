namespace FlavorfulStory.Actions
{
    /// <summary> Интерфейс съедобного предмета. </summary>
    public interface IEdible
    {
        /// <summary> Съесть предмет и применить его эффект к игроку. </summary>
        public void Eat();

        // TODO: На будущее
        //public void Eat(PlayerStats stats);
    }
}